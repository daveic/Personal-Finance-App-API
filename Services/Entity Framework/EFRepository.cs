﻿using Microsoft.EntityFrameworkCore;
using PersonalFinance.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinance.Services.EntityFramework
{
    public class EFRepository : IRepository
    {
        //Instantiate Context
        private readonly PersonalFinanceContext PersonalFinanceContext;
        public EFRepository(PersonalFinanceContext PersonalFinanceContext)
        {
            this.PersonalFinanceContext = PersonalFinanceContext;
        }


        //GET ALL Methods
        public virtual Task<IQueryable<Credit>> GetAllCreditsAsync(string User_OID)
        {
            return Task.FromResult(PersonalFinanceContext.Set<Credit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID));
        }
        public virtual Task<IQueryable<Debit>> GetAllDebitsAsync(string User_OID)
        {
            return Task.FromResult(PersonalFinanceContext.Set<Debit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID));
        }
        public virtual Task<IQueryable<KnownMovement>> GetAllKnownMovementsAsync(string User_OID)
        {
            return Task.FromResult(PersonalFinanceContext.Set<KnownMovement>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID));
        }
        public virtual Task<IQueryable<Transaction>> GetAllTransactionsAsync(string User_OID)
        {
            return Task.FromResult(PersonalFinanceContext.Set<Transaction>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID));
        }
        public virtual Task<IQueryable<Bank>> GetAllBanksAsync(string User_OID)
        {
            return Task.FromResult(PersonalFinanceContext.Set<Bank>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID));
        }
        public virtual Task<IQueryable<Deposit>> GetAllDepositsAsync(string User_OID)
        {
            return Task.FromResult(PersonalFinanceContext.Set<Deposit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID));
        }
        public virtual Task<IQueryable<Ticket>> GetAllTicketsAsync(string User_OID)
        {
            return Task.FromResult(PersonalFinanceContext.Set<Ticket>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID));
        }
        public virtual Task<IQueryable<Balance>> GetAllBalancesAsync(string User_OID)
        {
            return Task.FromResult(PersonalFinanceContext.Set<Balance>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID));
        }
        public virtual Task<IQueryable<Expiration>> GetAllExpirationsAsync(string User_OID)
        {
            return Task.FromResult(PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID));
        }

        //ADD NEW Methods
        public virtual Task<bool> AddCreditAsync(Credit c)
        {
            this.PersonalFinanceContext.Add(c);
            return Task.FromResult(true);
        }
        public virtual Task<bool> AddDebitAsync(Debit d)
        {
            this.PersonalFinanceContext.Add(d);
            return Task.FromResult(true);
        }
        public virtual Task<bool> AddKnownMovementAsync(KnownMovement k)
        {
            this.PersonalFinanceContext.Add(k);
            return Task.FromResult(true);
        }
        public virtual Task<bool> AddTransactionAsync(Transaction t)
        { 
            this.PersonalFinanceContext.Add(t);
            return Task.FromResult(true);
        }
        public virtual Task<bool> AddBankAsync(Bank b)
        {
            this.PersonalFinanceContext.Add(b);
            return Task.FromResult(true);
        }
        public virtual Task<bool> AddDepositAsync(Deposit d)
        {
            this.PersonalFinanceContext.Add(d);
            return Task.FromResult(true);
        }
        public virtual Task<bool> AddTicketAsync(Ticket t)
        {
            this.PersonalFinanceContext.Add(t);
            return Task.FromResult(true);
        }
        public virtual Task<bool> AddBalanceAsync(Balance b)
        {
            this.PersonalFinanceContext.Add(b);
            return Task.FromResult(true);
        }
        public virtual Task<bool> AddExpirationAsync(Expiration e)
        {
            this.PersonalFinanceContext.Add(e);
            return Task.FromResult(true);
        }

        //DELETE Methods
        public virtual Task<bool> DeleteCreditAsync(Credit c)
        {
            this.PersonalFinanceContext.Remove(c);
            return Task.FromResult(true);
        }
        public virtual Task<bool> DeleteDebitAsync(Debit d)
        {
            this.PersonalFinanceContext.Remove(d);
            return Task.FromResult(true);
        }
        public virtual Task<bool> DeleteTransactionAsync(Transaction t)
        {
            this.PersonalFinanceContext.Remove(t);
            return Task.FromResult(true);
        }
        public virtual Task<bool> DeleteKnownMovementAsync(KnownMovement k)
        {
            this.PersonalFinanceContext.Remove(k);
            return Task.FromResult(true);
        }
        public virtual Task<bool> DeleteBankAsync(Bank b)
        {
            this.PersonalFinanceContext.Remove(b);
            return Task.FromResult(true);
        }
        public virtual Task<bool> DeleteDepositAsync(Deposit d)
        {
            this.PersonalFinanceContext.Remove(d);
            return Task.FromResult(true);
        }
        public virtual Task<bool> DeleteTicketAsync(Ticket t)
        {
            this.PersonalFinanceContext.Remove(t);
            return Task.FromResult(true);
        }
        public virtual Task<bool> DeleteExpirationAsync(Expiration e)
        {
            this.PersonalFinanceContext.Remove(e);
            return Task.FromResult(true);
        }


        //GET BY ID Methods
        public virtual async Task<Credit> GetCreditAsync(int id, string User_OID)
        {
            return (await GetAllCreditsAsync(User_OID)).FirstOrDefault(x => x.ID == id);
        }
        public virtual async Task<Debit> GetDebitAsync(int id, string User_OID)
        {
            return (await GetAllDebitsAsync(User_OID)).FirstOrDefault(x => x.ID == id);
        }
        public virtual async Task<Transaction> GetTransactionAsync(int id, string User_OID)
        {
            return (await GetAllTransactionsAsync(User_OID)).FirstOrDefault(x => x.ID == id);
        }
        public virtual async Task<KnownMovement> GetKnownMovementAsync(int id, string User_OID)
        {
            return (await GetAllKnownMovementsAsync(User_OID)).FirstOrDefault(x => x.ID == id);
        }
        public virtual async Task<Bank> GetBankAsync(int id, string User_OID)
        {
            return (await GetAllBanksAsync(User_OID)).FirstOrDefault(x => x.ID == id);
        }
        public virtual async Task<Deposit> GetDepositAsync(int id, string User_OID)
        {
            return (await GetAllDepositsAsync(User_OID)).FirstOrDefault(x => x.ID == id);
        }
        public virtual async Task<Ticket> GetTicketAsync(int id, string User_OID)
        {
            return (await GetAllTicketsAsync(User_OID)).FirstOrDefault(x => x.ID == id);
        }
        public virtual async Task<Expiration> GetExpirationAsync(int id, string User_OID)
        {
            return (await GetAllExpirationsAsync(User_OID)).FirstOrDefault(x => x.ID == id);
        }


        //UPDATE ID Methods
        public virtual Task<bool> UpdateCreditAsync(Credit c)
        {
            PersonalFinanceContext.Attach(c);
            PersonalFinanceContext.Entry(c).State =
                Microsoft.EntityFrameworkCore.EntityState.Modified;
            return Task.FromResult(true);
        }
        public virtual Task<bool> UpdateDebitAsync(Debit d)
        {
            PersonalFinanceContext.Attach(d);
            PersonalFinanceContext.Entry(d).State =
                Microsoft.EntityFrameworkCore.EntityState.Modified;
            return Task.FromResult(true);
        }
        public virtual Task<bool> UpdateTransactionAsync(Transaction t)
        {
            PersonalFinanceContext.Attach(t);
            PersonalFinanceContext.Entry(t).State =
                Microsoft.EntityFrameworkCore.EntityState.Modified;
            return Task.FromResult(true);
        }
        public virtual Task<bool> UpdateKnownMovementAsync(KnownMovement k)
        {
            PersonalFinanceContext.Attach(k);
            PersonalFinanceContext.Entry(k).State =
                Microsoft.EntityFrameworkCore.EntityState.Modified;
            return Task.FromResult(true);
        }
        public virtual Task<bool> UpdateBankAsync(Bank b)
        {
            PersonalFinanceContext.Attach(b);
            PersonalFinanceContext.Entry(b).State =
                Microsoft.EntityFrameworkCore.EntityState.Modified;
            return Task.FromResult(true);
        }
        public virtual Task<bool> UpdateDepositAsync(Deposit d)
        {
            PersonalFinanceContext.Attach(d);
            PersonalFinanceContext.Entry(d).State =
                Microsoft.EntityFrameworkCore.EntityState.Modified;
            return Task.FromResult(true);
        }
        public virtual Task<bool> UpdateTicketAsync(Ticket t)
        {
            PersonalFinanceContext.Attach(t);
            PersonalFinanceContext.Entry(t).State =
                Microsoft.EntityFrameworkCore.EntityState.Modified;
            return Task.FromResult(true);
        }

        //SAVE CHANGES ID Method
        public virtual async Task<bool> SaveChangesAsync()
        {
            return await PersonalFinanceContext.SaveChangesAsync() > 0;
        }
    }
}

