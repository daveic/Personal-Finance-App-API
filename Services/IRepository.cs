using System.Linq;
using System.Threading.Tasks;
using PersonalFinance.Models;

namespace PersonalFinance.Services
{
    public interface IRepository
    {
		//IRepository interfaces for GET ALL api call
		Task<IQueryable<Credit>> GetAllCreditsAsync(string User_OID);
		Task<IQueryable<Debit>> GetAllDebitsAsync(string User_OID);
		Task<IQueryable<KnownMovement>> GetAllKnownMovementsAsync(string User_OID);
		Task<IQueryable<Transaction>> GetAllTransactionsAsync(string User_OID);
		Task<IQueryable<Bank>> GetAllBanksAsync(string User_OID);
		Task<IQueryable<Deposit>> GetAllDepositsAsync(string User_OID);
		Task<IQueryable<Ticket>> GetAllTicketsAsync(string User_OID);
		Task<IQueryable<Balance>> GetAllBalancesAsync(string User_OID);
		Task<IQueryable<Expiration>> GetAllExpirationsAsync(string User_OID);

		//IRepository interfaces for GET-by-ID api call
		Task<Credit> GetCreditAsync(int id, string User_OID);
		Task<Debit> GetDebitAsync(int id, string User_OID);
		Task<Transaction> GetTransactionAsync(int id, string User_OID);
		Task<KnownMovement> GetKnownMovementAsync(int id, string User_OID);
		Task<Bank> GetBankAsync(int id, string User_OID);
		Task<Deposit> GetDepositAsync(int id, string User_OID);
		Task<Ticket> GetTicketAsync(int id, string User_OID);
		Task<Expiration> GetExpirationAsync(int id, string User_OID);

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
