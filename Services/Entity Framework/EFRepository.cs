using Microsoft.EntityFrameworkCore;
using PersonalFinance.Models;
using PersonalFinance.Services.EntityFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public virtual Task<IQueryable<Credit>> GetAllCreditsAsync()
        {
            return Task.FromResult(PersonalFinanceContext.Set<Credit>().AsNoTracking().AsQueryable());
        }
        public virtual Task<IQueryable<Debit>> GetAllDebitsAsync()
        {
            return Task.FromResult(PersonalFinanceContext.Set<Debit>().AsNoTracking().AsQueryable());
        }
        public virtual Task<IQueryable<KnownMovement>> GetAllKnownMovementsAsync()
        {
            return Task.FromResult(PersonalFinanceContext.Set<KnownMovement>().AsNoTracking().AsQueryable());
        }
        public virtual Task<IQueryable<Transaction>> GetAllTransactionsAsync()
        {
            return Task.FromResult(PersonalFinanceContext.Set<Transaction>().AsNoTracking().AsQueryable());
        }
        public virtual Task<IQueryable<Bank>> GetAllBanksAsync()
        {
            return Task.FromResult(PersonalFinanceContext.Set<Bank>().AsNoTracking().AsQueryable());
        }
        public virtual Task<IQueryable<Deposit>> GetAllDepositsAsync()
        {
            return Task.FromResult(PersonalFinanceContext.Set<Deposit>().AsNoTracking().AsQueryable());
        }
        public virtual Task<IQueryable<Ticket>> GetAllTicketsAsync()
        {
            return Task.FromResult(PersonalFinanceContext.Set<Ticket>().AsNoTracking().AsQueryable());
        }
        public virtual Task<IQueryable<Balance>> GetAllBalancesAsync()
        {
            return Task.FromResult(PersonalFinanceContext.Set<Balance>().AsNoTracking().AsQueryable());
        }
        public virtual Task<IQueryable<Expiration>> GetAllExpirationsAsync()
        {
            return Task.FromResult(PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable());
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
        public virtual async Task<Credit> GetCreditAsync(int id)
        {
            return (await GetAllCreditsAsync()).FirstOrDefault(x => x.ID == id);
        }
        public virtual async Task<Debit> GetDebitAsync(int id)
        {
            return (await GetAllDebitsAsync()).FirstOrDefault(x => x.ID == id);
        }
        public virtual async Task<Transaction> GetTransactionAsync(int id)
        {
            return (await GetAllTransactionsAsync()).FirstOrDefault(x => x.ID == id);
        }
        public virtual async Task<KnownMovement> GetKnownMovementAsync(int id)
        {
            return (await GetAllKnownMovementsAsync()).FirstOrDefault(x => x.ID == id);
        }
        public virtual async Task<Bank> GetBankAsync(int id)
        {
            return (await GetAllBanksAsync()).FirstOrDefault(x => x.ID == id);
        }
        public virtual async Task<Deposit> GetDepositAsync(int id)
        {
            return (await GetAllDepositsAsync()).FirstOrDefault(x => x.ID == id);
        }
        public virtual async Task<Ticket> GetTicketAsync(int id)
        {
            return (await GetAllTicketsAsync()).FirstOrDefault(x => x.ID == id);
        }
        public virtual async Task<Expiration> GetExpirationAsync(int id)
        {
            return (await GetAllExpirationsAsync()).FirstOrDefault(x => x.ID == id);
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

