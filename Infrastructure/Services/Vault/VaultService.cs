using AutoMapper;
using Core.DTOs.Vault;
using Core.DTOs.VaultTransaction;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Vault
{
    public class VaultService : IVaultService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IMapper mapper;

        public VaultService(MinaretOpsDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<List<VaultDTO>> GetAllAsync()
        {
            var vaults = await context.Vaults
                .Include(v => v.Branch)
                .Include(v => v.Currency)
                .Include(v => v.Transactions)
                .ToListAsync();

            var vaultDTOs = vaults.Select(v => new VaultDTO
            {
                Id = v.Id,
                VaultType = v.VaultType,
                BranchId = v.BranchId,
                BranchName = v.Branch?.Name,
                CurrencyId = v.CurrencyId,
                CurrencyName = v.Currency.Name,
                CurrencyCode = v.Currency.Code,
                CreatedAt = v.CreatedAt,
                Balance = CalculateBalance(v.Transactions, v.CurrencyId)
            }).ToList();

            return vaultDTOs;
        }

        public async Task<VaultDTO> GetByIdAsync(int id)
        {
            var vault = await context.Vaults
                .Include(v => v.Branch)
                .Include(v => v.Currency)
                .Include(v => v.Transactions)
                .FirstOrDefaultAsync(v => v.Id == id)
                ?? throw new KeyNotFoundException("Vault not found");

            return new VaultDTO
            {
                Id = vault.Id,
                VaultType = vault.VaultType,
                BranchId = vault.BranchId,
                BranchName = vault.Branch?.Name,
                CurrencyId = vault.CurrencyId,
                CurrencyName = vault.Currency.Name,
                CurrencyCode = vault.Currency.Code,
                CreatedAt = vault.CreatedAt,
                Balance = CalculateBalance(vault.Transactions, vault.CurrencyId)
            };
        }

        public async Task<VaultDTO> GetUnifiedVaultAsync(int currencyId)
        {
            // Unified vault is a calculated view - get currency for display
            var currency = await context.Currencies
                .FirstOrDefaultAsync(c => c.Id == currencyId)
                ?? throw new KeyNotFoundException("Currency not found");

            // Get all transactions from all branch vaults (and unified vault if exists) with the specified currency
            var allTransactions = await context.VaultTransactions
                .Include(t => t.Vault)
                    .ThenInclude(v => v.Branch)
                .Include(t => t.Currency)
                .Where(t => t.CurrencyId == currencyId)
                .ToListAsync();

            return new VaultDTO
            {
                Id = 0, // No actual vault ID for unified view
                VaultType = VaultType.Unified,
                BranchId = null,
                BranchName = "Unified",
                CurrencyId = currencyId,
                CurrencyName = currency.Name,
                CurrencyCode = currency.Code,
                CreatedAt = DateTime.UtcNow, // Not applicable for calculated view
                Balance = allTransactions.Sum(t => t.TransactionType == TransactionType.Incoming ? t.Amount : -t.Amount)
            };
        }

        public async Task<decimal> GetVaultBalanceAsync(int vaultId, int? currencyId = null)
        {
            var vault = await context.Vaults
                .Include(v => v.Transactions)
                .FirstOrDefaultAsync(v => v.Id == vaultId)
                ?? throw new KeyNotFoundException("Vault not found");

            var transactions = vault.Transactions.AsQueryable();
            if (currencyId.HasValue)
            {
                transactions = transactions.Where(t => t.CurrencyId == currencyId.Value);
            }

            return CalculateBalance(transactions.ToList(), currencyId ?? vault.CurrencyId);
        }

        public async Task<decimal> GetUnifiedVaultBalanceAsync(int currencyId)
        {
            return CalculateUnifiedBalance(currencyId);
        }

        public async Task<List<VaultTransactionDTO>> GetVaultTransactionsAsync(int vaultId, VaultTransactionFilterDTO? filter = null)
        {
            var vault = await context.Vaults
                .FirstOrDefaultAsync(v => v.Id == vaultId)
                ?? throw new KeyNotFoundException("Vault not found");

            var query = context.VaultTransactions
                .Include(t => t.Vault)
                    .ThenInclude(v => v.Branch)
                .Include(t => t.Currency)
                .Include(t => t.CreatedBy)
                .Where(t => t.VaultId == vaultId)
                .AsQueryable();

            if (filter != null)
            {
                if (filter.TransactionType.HasValue)
                    query = query.Where(t => t.TransactionType == filter.TransactionType.Value);

                if (filter.ReferenceType.HasValue)
                    query = query.Where(t => t.ReferenceType == filter.ReferenceType.Value);

                if (filter.CurrencyId.HasValue)
                    query = query.Where(t => t.CurrencyId == filter.CurrencyId.Value);

                if (filter.FromDate.HasValue)
                    query = query.Where(t => t.TransactionDate >= filter.FromDate.Value);

                if (filter.ToDate.HasValue)
                    query = query.Where(t => t.TransactionDate <= filter.ToDate.Value);

                if (filter.ReferenceId.HasValue)
                    query = query.Where(t => t.ReferenceId == filter.ReferenceId.Value);
            }

            var transactions = await query.OrderByDescending(t => t.TransactionDate).ToListAsync();

            return transactions.Select(t => new VaultTransactionDTO
            {
                Id = t.Id,
                VaultId = t.VaultId,
                VaultBranchName = t.Vault.Branch?.Name ?? "Unified",
                VaultType = t.Vault.VaultType,
                TransactionType = t.TransactionType,
                Amount = t.Amount,
                CurrencyId = t.CurrencyId,
                CurrencyName = t.Currency.Name,
                CurrencyCode = t.Currency.Code,
                TransactionDate = t.TransactionDate,
                Description = t.Description,
                ReferenceType = t.ReferenceType,
                ReferenceId = t.ReferenceId,
                Notes = t.Notes,
                CreatedById = t.CreatedById,
                CreatedByName = $"{t.CreatedBy.FirstName} {t.CreatedBy.LastName}",
                CreatedAt = t.CreatedAt
            }).ToList();
        }

        public async Task<List<VaultTransactionDTO>> GetUnifiedVaultTransactionsAsync(int currencyId, VaultTransactionFilterDTO? filter = null)
        {
            var query = context.VaultTransactions
                .Include(t => t.Vault)
                    .ThenInclude(v => v.Branch)
                .Include(t => t.Currency)
                .Include(t => t.CreatedBy)
                .Where(t => t.CurrencyId == currencyId)
                .AsQueryable();

            if (filter != null)
            {
                if (filter.TransactionType.HasValue)
                    query = query.Where(t => t.TransactionType == filter.TransactionType.Value);

                if (filter.ReferenceType.HasValue)
                    query = query.Where(t => t.ReferenceType == filter.ReferenceType.Value);

                if (filter.FromDate.HasValue)
                    query = query.Where(t => t.TransactionDate >= filter.FromDate.Value);

                if (filter.ToDate.HasValue)
                    query = query.Where(t => t.TransactionDate <= filter.ToDate.Value);

                if (filter.ReferenceId.HasValue)
                    query = query.Where(t => t.ReferenceId == filter.ReferenceId.Value);
            }

            var transactions = await query.OrderByDescending(t => t.TransactionDate).ToListAsync();

            return transactions.Select(t => new VaultTransactionDTO
            {
                Id = t.Id,
                VaultId = t.VaultId,
                VaultBranchName = t.Vault.Branch?.Name ?? "Unified",
                VaultType = t.Vault.VaultType,
                TransactionType = t.TransactionType,
                Amount = t.Amount,
                CurrencyId = t.CurrencyId,
                CurrencyName = t.Currency.Name,
                CurrencyCode = t.Currency.Code,
                TransactionDate = t.TransactionDate,
                Description = t.Description,
                ReferenceType = t.ReferenceType,
                ReferenceId = t.ReferenceId,
                Notes = t.Notes,
                CreatedById = t.CreatedById,
                CreatedByName = $"{t.CreatedBy.FirstName} {t.CreatedBy.LastName}",
                CreatedAt = t.CreatedAt
            }).ToList();
        }

        public async Task<VaultTransactionDTO> CreateTransactionAsync(CreateVaultTransactionDTO createTransactionDTO)
        {
            var vault = await context.Vaults
                .Include(v => v.Currency)
                .FirstOrDefaultAsync(v => v.Id == createTransactionDTO.VaultId)
                ?? throw new KeyNotFoundException("Vault not found");

            var currency = await context.Currencies
                .FirstOrDefaultAsync(c => c.Id == vault.CurrencyId)
                ?? throw new KeyNotFoundException("Currency not found");

            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Id == createTransactionDTO.UserId)
                ?? throw new KeyNotFoundException("User not found");

            if (createTransactionDTO.Amount <= 0)
                throw new InvalidObjectException("Transaction amount must be greater than zero");

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var vaultTransaction = new VaultTransaction
                {
                    VaultId = createTransactionDTO.VaultId,
                    TransactionType = createTransactionDTO.TransactionType,
                    Amount = createTransactionDTO.Amount,
                    CurrencyId = vault.CurrencyId,
                    TransactionDate = createTransactionDTO.TransactionDate,
                    Description = createTransactionDTO.Description,
                    ReferenceType = createTransactionDTO.ReferenceType,
                    ReferenceId = createTransactionDTO.ReferenceId,
                    Notes = createTransactionDTO.Notes,
                    CreatedById = user.Id,
                    CreatedAt = DateTime.UtcNow
                };

                await context.VaultTransactions.AddAsync(vaultTransaction);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Reload with navigation properties
                var createdTransaction = await context.VaultTransactions
                    .Include(t => t.Vault)
                        .ThenInclude(v => v.Branch)
                    .Include(t => t.Currency)
                    .Include(t => t.CreatedBy)
                    .FirstOrDefaultAsync(t => t.Id == vaultTransaction.Id);

                return new VaultTransactionDTO
                {
                    Id = createdTransaction!.Id,
                    VaultId = createdTransaction.VaultId,
                    VaultBranchName = createdTransaction.Vault.Branch?.Name ?? "Unified",
                    VaultType = createdTransaction.Vault.VaultType,
                    TransactionType = createdTransaction.TransactionType,
                    Amount = createdTransaction.Amount,
                    CurrencyId = createdTransaction.CurrencyId,
                    CurrencyName = createdTransaction.Currency.Name,
                    CurrencyCode = createdTransaction.Currency.Code,
                    TransactionDate = createdTransaction.TransactionDate,
                    Description = createdTransaction.Description,
                    ReferenceType = createdTransaction.ReferenceType,
                    ReferenceId = createdTransaction.ReferenceId,
                    Notes = createdTransaction.Notes,
                    CreatedById = createdTransaction.CreatedById,
                    CreatedByName = $"{createdTransaction.CreatedBy.FirstName} {createdTransaction.CreatedBy.LastName}",
                    CreatedAt = createdTransaction.CreatedAt
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<VaultTransactionDTO> UpdateTransactionAsync(int id, UpdateVaultTransactionDTO updateTransactionDTO)
        {
            var transaction = await context.VaultTransactions
                .Include(t => t.Vault)
                    .ThenInclude(v => v.Branch)
                .Include(t => t.Currency)
                .Include(t => t.CreatedBy)
                .FirstOrDefaultAsync(t => t.Id == id)
                ?? throw new KeyNotFoundException("Transaction not found");

            transaction.TransactionDate = updateTransactionDTO.TransactionDate;
            transaction.Description = updateTransactionDTO.Description;
            transaction.Notes = updateTransactionDTO.Notes;

            context.VaultTransactions.Update(transaction);
            await context.SaveChangesAsync();

            // Reload with navigation properties
            var updatedTransaction = await context.VaultTransactions
                .Include(t => t.Vault)
                    .ThenInclude(v => v.Branch)
                .Include(t => t.Currency)
                .Include(t => t.CreatedBy)
                .FirstOrDefaultAsync(t => t.Id == id);

            return new VaultTransactionDTO
            {
                Id = updatedTransaction!.Id,
                VaultId = updatedTransaction.VaultId,
                VaultBranchName = updatedTransaction.Vault.Branch?.Name ?? "Unified",
                VaultType = updatedTransaction.Vault.VaultType,
                TransactionType = updatedTransaction.TransactionType,
                Amount = updatedTransaction.Amount,
                CurrencyId = updatedTransaction.CurrencyId,
                CurrencyName = updatedTransaction.Currency.Name,
                CurrencyCode = updatedTransaction.Currency.Code,
                TransactionDate = updatedTransaction.TransactionDate,
                Description = updatedTransaction.Description,
                ReferenceType = updatedTransaction.ReferenceType,
                ReferenceId = updatedTransaction.ReferenceId,
                Notes = updatedTransaction.Notes,
                CreatedById = updatedTransaction.CreatedById,
                CreatedByName = $"{updatedTransaction.CreatedBy.FirstName} {updatedTransaction.CreatedBy.LastName}",
                CreatedAt = updatedTransaction.CreatedAt
            };
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            var transaction = await context.VaultTransactions
                .FirstOrDefaultAsync(t => t.Id == id)
                ?? throw new KeyNotFoundException("Transaction not found");

            context.VaultTransactions.Remove(transaction);
            return await context.SaveChangesAsync() > 0;
        }

        private decimal CalculateBalance(List<VaultTransaction> transactions, int currencyId)
        {
            return transactions
                .Where(t => t.CurrencyId == currencyId)
                .Sum(t => t.TransactionType == TransactionType.Incoming ? t.Amount : -t.Amount);
        }

        private decimal CalculateUnifiedBalance(int currencyId)
        {
            var transactions = context.VaultTransactions
                .Where(t => t.CurrencyId == currencyId)
                .ToList();

            return transactions.Sum(t => t.TransactionType == TransactionType.Incoming ? t.Amount : -t.Amount);
        }
    }
}
