using AutoMapper;
using Core.DTOs.Contract;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Contract
{
    public class ContractService : IContractService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IMapper mapper;

        public ContractService(MinaretOpsDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<ContractDTO> Create(CreateContractDTO contractDTO)
        {
            var client = await context.Clients
                .Include(c => c.ClientServices)
                    .ThenInclude(cs => cs.Service)
                .Include(c => c.AccountManager)
                .FirstOrDefaultAsync(c => c.Id == contractDTO.ClientId)
                ?? throw new KeyNotFoundException("Can't add contract for not existed client");

            var currency = await context.Currencies
                .FirstOrDefaultAsync(c => c.Id == contractDTO.CurrencyId)
                ?? throw new KeyNotFoundException("Couldn't find the currency");

            var contract = new CustomContract
            {
                Client = client,
                Currency = currency,
                ContractDuration = contractDTO.ContractDuration,
                ContractTotal = contractDTO.ContractTotal,
                PaidAmount = contractDTO.PaidAmount,
                CreatedAt = DateTime.UtcNow
            };

            await context.Contracts.AddAsync(contract);

            var tran = new VaultTransaction
            {
                VaultId = contractDTO.VaultId,
                CurrencyId = contractDTO.CurrencyId,
                TransactionType = Core.Enums.TransactionType.Incoming,
                TransactionDate = DateTime.UtcNow,
                ReferenceType = Core.Enums.TransactionReferenceType.ContractPayment,
                CreatedById = contractDTO.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                Amount = contractDTO.PaidAmount
            };
            await context.VaultTransactions.AddAsync(tran);
            await context.SaveChangesAsync();
            
            // Reload contract with all navigation properties for mapping
            var createdContract = await context.Contracts
                .Include(c => c.Client)
                    .ThenInclude(cl => cl.ClientServices)
                        .ThenInclude(cs => cs.Service)
                .Include(c => c.Client)
                    .ThenInclude(cl => cl.AccountManager)
                .Include(c => c.Currency)
                .FirstOrDefaultAsync(c => c.Id == contract.Id);
            
            return mapper.Map<ContractDTO>(createdContract);
        }
        public async Task<bool> Delete(int id)
        {
            var contract = await context.Contracts
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new KeyNotFoundException("Contract doesn't exist");

            context.Contracts.Remove(contract);
            return await context.SaveChangesAsync() > 0;
        }
        public async Task<List<ContractDTO>> GetAll()
        {
            var contracts = await context.Contracts
                .Include(c => c.Client)
                    .ThenInclude(cl => cl.ClientServices)
                        .ThenInclude(cs => cs.Service)
                .Include(c => c.Client)
                    .ThenInclude(cl => cl.AccountManager)
                .Include(c => c.Currency)
                .ToListAsync();
            return mapper.Map<List<ContractDTO>>(contracts);
        }
        public async Task<ContractDTO> GetById(int id)
        {
            var contract = await context.Contracts
                .Include(c => c.Client)
                    .ThenInclude(cl => cl.ClientServices)
                        .ThenInclude(cs => cs.Service)
                .Include(c => c.Client)
                    .ThenInclude(cl => cl.AccountManager)
                .Include(c => c.Currency)
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new KeyNotFoundException("Couldn't find that contract");
            return mapper.Map<ContractDTO>(contract);
        }
        public async Task<ContractDTO> Update(UpdateContract updateContract)
        {
            var contract = await context.Contracts
                .FirstOrDefaultAsync(c => c.Id == updateContract.Id)
                ?? throw new KeyNotFoundException();

            if (contract.ContractDuration != updateContract.ContractDuration
                && updateContract.ContractDuration != 0)
                contract.ContractDuration = updateContract.ContractDuration;

            if (contract.ContractTotal != updateContract.ContractTotal
                && updateContract.ContractTotal != 0)
                contract.ContractTotal = updateContract.ContractTotal;

            if (contract.PaidAmount != updateContract.PaidAmount
                && updateContract.PaidAmount != 0)
                contract.PaidAmount = updateContract.PaidAmount;

            if (contract.CurrencyId != updateContract.CurrencyId
                && updateContract.CurrencyId != 0)
            {
                var currency = await context.Currencies
                    .FirstOrDefaultAsync(c => c.Id == updateContract.CurrencyId)
                    ?? throw new KeyNotFoundException();
                contract.CurrencyId = currency.Id;
                contract.Currency = currency;
            }

            context.Contracts.Update(contract);
            await context.SaveChangesAsync();
            return mapper.Map<ContractDTO>(contract);
        }
    }
}
