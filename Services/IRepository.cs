using System.Linq;
using System.Threading.Tasks;
using PersonalFinance.Models;

namespace PersonalFinance.Services
{
    public interface IRepository
    {
		//IRepository interfaces for GET ALL api call
		Task<IQueryable<Credit>> GetAllCreditsAsync();
		Task<IQueryable<Debit>> GetAllDebitsAsync();
		Task<IQueryable<KnownMovement>> GetAllKnownMovementsAsync();
		Task<IQueryable<Transaction>> GetAllTransactionsAsync();
		Task<IQueryable<Bank>> GetAllBanksAsync();
		Task<IQueryable<Deposit>> GetAllDepositsAsync();
		Task<IQueryable<Ticket>> GetAllTicketsAsync();
		Task<IQueryable<Balance>> GetAllBalancesAsync();
		Task<IQueryable<Expiration>> GetAllExpirationsAsync();

		//IRepository interfaces for GET-by-ID api call
		Task<Credit> GetCreditAsync(int id);
		Task<Debit> GetDebitAsync(int id);
		Task<Transaction> GetTransactionAsync(int id);
		Task<KnownMovement> GetKnownMovementAsync(int id);
		Task<Bank> GetBankAsync(int id);
		Task<Deposit> GetDepositAsync(int id);
		Task<Ticket> GetTicketAsync(int id);
		Task<Expiration> GetExpirationAsync(int id);

		//IRepository interfaces for ADD-by-ID api call
		Task<bool> AddCreditAsync(Credit p);
		Task<bool> AddDebitAsync(Debit p);
		Task<bool> AddKnownMovementAsync(KnownMovement p);
		Task<bool> AddTransactionAsync(Transaction p);
		Task<bool> AddBankAsync(Bank p);
		Task<bool> AddDepositAsync(Deposit p);
		Task<bool> AddTicketAsync(Ticket p);
		Task<bool> AddBalanceAsync(Balance b);
		Task<bool> AddExpirationAsync(Expiration e);

		//IRepository interfaces for UPDATE-by-ID api call
		Task<bool> UpdateCreditAsync(Credit p);
		Task<bool> UpdateDebitAsync(Debit p);
		Task<bool> UpdateTransactionAsync(Transaction p);
		Task<bool> UpdateKnownMovementAsync(KnownMovement p);
		Task<bool> UpdateBankAsync(Bank p);
		Task<bool> UpdateDepositAsync(Deposit p);
		Task<bool> UpdateTicketAsync(Ticket p);

		//IRepository interfaces for DELETE-by-ID api call
		Task<bool> DeleteCreditAsync(Credit p);
		Task<bool> DeleteDebitAsync(Debit p);
		Task<bool> DeleteTransactionAsync(Transaction p);
		Task<bool> DeleteKnownMovementAsync(KnownMovement p);
		Task<bool> DeleteBankAsync(Bank p);
		Task<bool> DeleteDepositAsync(Deposit p);
		Task<bool> DeleteTicketAsync(Ticket p);
		Task<bool> DeleteExpirationAsync(Expiration e);

		//IRepository interfaces for SAVE api call
		Task<bool> SaveChangesAsync();
	}
}
