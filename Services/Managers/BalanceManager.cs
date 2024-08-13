using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyFinances.WebApi.Data;
using MyFinances.WebApi.Models;
using MyFinances.WebApi.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinances.WebApi.Repository.Managers
{
    public class BalanceManager : IBalance
    {
        private readonly DbFinancesContext _context;
        private readonly ILogger<BalanceManager> _logger;
        public BalanceManager(DbFinancesContext context, ILogger<BalanceManager> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Balance> ActualBalance(Transaction transaction, bool isLower = false)
        {
            try
            {
                List<Transaction> transactions = await _context.Transactions.Where(t => t.UserId == transaction.UserId).ToListAsync();
                Balance balance = await _context.Balance.FirstOrDefaultAsync(b => b.Id == transaction.BalanceId);
                balance ??= new Balance();

                switch (transaction.TransactionType)
                {
                    case TransactionType.Income:
                        if (!transaction.IsActive || isLower)
                            balance.TotalBalance -= transaction.Amount;
                        else
                            balance.TotalBalance += transaction.Amount;
                        break;
                    case TransactionType.Expense:
                        if (!transaction.IsActive || isLower)
                            balance.TotalBalance += transaction.Amount;
                        else
                            balance.TotalBalance -= transaction.Amount;
                        break;
                }

                if (transactions.Count() < 1)
                    await _context.Balance.AddAsync(balance);
                else
                    _context.Entry(balance).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Balance updated. Transaction: {transaction.Details}");
                return balance;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Balance> GoalBalance(
                                     FinancialGoal goal,
                                     double? currentAmount,
                                     bool isModified = false,
                                     bool isLower = false,
                                     bool isDeleted = false)
        {
            try
            {
                List<Transaction> transactions = await _context.Transactions
                                                        .Where(t => t.UserId == goal.UserId)
                                                        .ToListAsync();

                Balance balance = await GetByUserId(goal.UserId) ?? new Balance();
                Category category =
                    await _context.Categories.Where(c => c.Title == TransactionType.Reserve.ToString())
                                             .FirstOrDefaultAsync();

                string detailModified = "", detailReserveWithdraw = "";
                double transactionAmount = 0.0;
                if (!goal.Withdrawn && !isDeleted)
                {
                    detailReserveWithdraw = $"Reserve - Goal: {goal.Title}";
                    transactionAmount = currentAmount.Value;
                    if (!isModified)
                        balance.TotalBalance -= currentAmount.Value;
                    else
                    {
                        detailModified = !isLower ?
                                            $"Update/Greater amount - Goal: {goal.Title}" :
                                            $"Update/Lower amount - Goal: {goal.Title}";
                        balance.TotalBalance = !isLower ?
                                              balance.TotalBalance - currentAmount.Value :
                                              balance.TotalBalance + currentAmount.Value;
                    }
                }
                else if (goal.Withdrawn || isDeleted)
                {
                    detailReserveWithdraw = !isDeleted ?
                                                $"Withdraw - Goal: {goal.Title}" :
                                                $"Deleted Goal: {goal.Title}";
                    transactionAmount = !isDeleted ? goal.FinalAmount : goal.CurrentAmount ?? 0;
                    balance.TotalBalance += transactionAmount;
                }

                if (transactions.Count() < 1)
                {
                    _context.Balance.Add(balance);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.Entry(balance).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }

                Transaction newTransaction = new Transaction()
                {
                    Date = DateTime.Now,
                    Details = !isModified ? detailReserveWithdraw : detailModified,
                    Amount = transactionAmount,
                    TransactionType = TransactionType.Reserve,
                    CategoryId = category.Id,
                    BalanceId = balance.Id,
                    UserId = goal.UserId,
                    IsActive = !isDeleted
                };

                _context.Transactions.Add(newTransaction);
                await _context.SaveChangesAsync();

                balance.TransactionId = newTransaction.Id;
                _context.Entry(balance).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Balance updated: {balance.TotalBalance} | Goal: {goal.Title}");
                return balance;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Balance> GetByUserId(int userId)
        {
            try
            {
                var balance = await (from b in _context.Balance
                                     join t in _context.Transactions on b.Id equals t.BalanceId
                                     where t.UserId == userId
                                     select b).FirstOrDefaultAsync();

                return balance;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }
    }
}
