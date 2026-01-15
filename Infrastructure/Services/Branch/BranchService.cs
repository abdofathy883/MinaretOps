using AutoMapper;
using Core.DTOs.Branch;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Branch
{
    public class BranchService : IBranchService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IMapper mapper;

        public BranchService(MinaretOpsDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<List<BranchDTO>> GetAllAsync()
        {
            var branches = await context.Branches
                .Include(b => b.Vault)
                    .ThenInclude(v => v!.Currency)
                .Include(b => b.Vault)
                    .ThenInclude(v => v!.Transactions)
                .ToListAsync();

            return mapper.Map<List<BranchDTO>>(branches);
        }

        public async Task<BranchDTO> GetByIdAsync(int id)
        {
            var branch = await context.Branches
                .Include(b => b.Vault)
                    .ThenInclude(v => v!.Currency)
                .Include(b => b.Vault)
                    .ThenInclude(v => v!.Transactions)
                .FirstOrDefaultAsync(b => b.Id == id)
                ?? throw new KeyNotFoundException("Branch not found");

            return mapper.Map<BranchDTO>(branch);
        }

        public async Task<BranchDTO> CreateAsync(CreateBranchDTO createBranchDTO)
        {
            // Validate currency exists
            var currency = await context.Currencies
                .FirstOrDefaultAsync(c => c.Id == createBranchDTO.CurrencyId)
                ?? throw new KeyNotFoundException("Currency not found");

            // Check if branch with same name or code already exists
            var existingBranch = await context.Branches
                .FirstOrDefaultAsync(b => b.Name == createBranchDTO.Name 
                    || (!string.IsNullOrEmpty(createBranchDTO.Code) && b.Code == createBranchDTO.Code));

            if (existingBranch != null)
                throw new AlreadyExistObjectException("Branch with this name or code already exists");

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // Create branch
                var branch = new Core.Models.Branch
                {
                    Name = createBranchDTO.Name,
                    Code = createBranchDTO.Code,
                    CreatedAt = DateTime.UtcNow
                };

                await context.Branches.AddAsync(branch);
                await context.SaveChangesAsync();

                // Create vault for the branch
                var vault = new Core.Models.Vault
                {
                    VaultType = VaultType.Branch,
                    BranchId = branch.Id,
                    CurrencyId = createBranchDTO.CurrencyId,
                    Currency = currency,
                    CreatedAt = DateTime.UtcNow
                };

                await context.Vaults.AddAsync(vault);
                await context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Reload with navigation properties
                var createdBranch = await context.Branches
                    .Include(b => b.Vault)
                        .ThenInclude(v => v!.Currency)
                    .FirstOrDefaultAsync(b => b.Id == branch.Id);

                return mapper.Map<BranchDTO>(createdBranch);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<BranchDTO> UpdateAsync(int id, UpdateBranchDTO updateBranchDTO)
        {
            var branch = await context.Branches
                .Include(b => b.Vault)
                    .ThenInclude(v => v!.Currency)
                .FirstOrDefaultAsync(b => b.Id == id)
                ?? throw new KeyNotFoundException("Branch not found");

            // Check if another branch with same name or code exists
            var existingBranch = await context.Branches
                .FirstOrDefaultAsync(b => b.Id != id 
                    && (b.Name == updateBranchDTO.Name 
                        || (!string.IsNullOrEmpty(updateBranchDTO.Code) && b.Code == updateBranchDTO.Code)));

            if (existingBranch != null)
                throw new AlreadyExistObjectException("Another branch with this name or code already exists");

            branch.Name = updateBranchDTO.Name;
            branch.Code = updateBranchDTO.Code;
            branch.UpdatedAt = DateTime.UtcNow;

            context.Branches.Update(branch);
            await context.SaveChangesAsync();

            // Reload with navigation properties
            var updatedBranch = await context.Branches
                .Include(b => b.Vault)
                    .ThenInclude(v => v!.Currency)
                .FirstOrDefaultAsync(b => b.Id == id);

            return mapper.Map<BranchDTO>(updatedBranch);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var branch = await context.Branches
                .Include(b => b.Vault)
                    .ThenInclude(v => v!.Transactions)
                .FirstOrDefaultAsync(b => b.Id == id)
                ?? throw new KeyNotFoundException("Branch not found");

            // Check if vault has transactions
            if (branch.Vault?.Transactions != null && branch.Vault.Transactions.Any())
                throw new InvalidObjectException("Cannot delete branch with existing transactions");

            context.Branches.Remove(branch);
            return await context.SaveChangesAsync() > 0;
        }
    }
}
