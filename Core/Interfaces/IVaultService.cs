using Core.DTOs.Vault;
using Core.DTOs.VaultTransaction;

namespace Core.Interfaces
{
    public interface IVaultService
    {
        Task<List<VaultDTO>> GetAllAsync();
        Task<List<VaultDTO>> GetAllLocalAsync();
        Task<VaultDTO> GetByIdAsync(int id);
        Task<VaultDTO> GetUnifiedVaultAsync(int currencyId);
        Task<decimal> GetVaultBalanceAsync(int vaultId, int? currencyId = null);
        Task<decimal> GetUnifiedVaultBalanceAsync(int currencyId);
        Task<List<VaultTransactionDTO>> GetVaultTransactionsAsync(int vaultId, VaultTransactionFilterDTO? filter = null);
        Task<List<VaultTransactionDTO>> GetUnifiedVaultTransactionsAsync(int currencyId, VaultTransactionFilterDTO? filter = null);
        Task<VaultTransactionDTO> CreateTransactionAsync(CreateVaultTransactionDTO createTransactionDTO);
        Task<VaultTransactionDTO> UpdateTransactionAsync(int id, UpdateVaultTransactionDTO updateTransactionDTO);
        Task<bool> DeleteTransactionAsync(int id);
    }
}
