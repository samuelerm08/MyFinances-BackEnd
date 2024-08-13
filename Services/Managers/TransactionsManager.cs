using Microsoft.EntityFrameworkCore;
using MyFinances.WebApi.Data;
using MyFinances.WebApi.Models;
using MyFinances.WebApi.Models.DTOs;
using MyFinances.WebApi.Models.Pagination;
using MyFinances.WebApi.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaFinanciero.WebApi.Repository.Managers
{
    public class TransactionsManager : ITransactions
    {
        private readonly DbFinancesContext _context;
        private readonly IBalance _balanceService;
        public TransactionsManager(DbFinancesContext context, IBalance balanceService)
        {
            _context = context;
            _balanceService = balanceService;
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            try
            {
                return await _context.Transactions
                            .Include(t => t.Category)
                            .Include(t => t.Balance)
                            .Include(t => t.User)
                            .ToListAsync();
            }
            catch
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }
        public async Task<PagedList<Transaction>> GetByUserIdAsync(TransactionPayloadDTO transactionPayload, PaginationPayloadDTO parameters)
        {
            try
            {
                IQueryable<Transaction> transactionsQuery = _context.Transactions
                                                            .Include(t => t.Category)
                                                            .Include(t => t.Balance)
                                                            .Include(t => t.User)
                                                            .Where(t => t.UserId == int.Parse(transactionPayload.UserId))
                                                            .OrderByDescending(t => t.Id);

                PagedList<Transaction> pagedTransactions = await PagedList<Transaction>.Paginate(
                                                            transactionsQuery,
                                                            parameters.Page,
                                                            parameters.PageSize);

                return pagedTransactions;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        public async Task<Transaction> GetByIdAsync(int? id)
        {
            try
            {
                Transaction transaccion = await _context.Transactions
                    .Include(t => t.Category)
                    .Include(t => t.Balance)
                    .Include(t => t.User)
                    .SingleOrDefaultAsync(t => t.Id == id);
                return transaccion;
            }
            catch
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }

        public async Task<Transaction> CreateAsync(Transaction transaction)
        {
            try
            {
                Balance createdBalance = await _balanceService.ActualBalance(transaction);
                transaction.BalanceId = createdBalance.Id;

                await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();

                createdBalance.TransactionId = transaction.Id;
                _context.Entry(createdBalance).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return transaction;
            }
            catch
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }

        public async Task<Transaction> ModifyAsync(int? id, Transaction transaction)
        {
            try
            {
                if (id != transaction.Id)
                {
                    return null;
                }

                Transaction transactionToModify = await GetByIdAsync(id);
                transactionToModify.Date = transaction.Date;
                transactionToModify.Details = transaction.Details;
                transactionToModify.CategoryId = transaction.CategoryId;
                transactionToModify.Category = _context.Categories.Find(transactionToModify.CategoryId);

                bool amountHasChanged = transactionToModify.Amount != transaction.Amount;
                bool typeHasChanged = transactionToModify.TransactionType != transaction.TransactionType;

                if (amountHasChanged || typeHasChanged)
                {
                    if (!typeHasChanged)
                    {
                        if (transaction.Amount < transactionToModify.Amount)
                        {
                            transactionToModify.Amount -= transaction.Amount;
                            await _balanceService.ActualBalance(transactionToModify, true);
                            transactionToModify.Amount = transaction.Amount;
                        }
                        else
                        {
                            transactionToModify.Amount = transaction.Amount - transactionToModify.Amount;
                            await _balanceService.ActualBalance(transactionToModify);
                            transactionToModify.Amount = transaction.Amount;
                        }
                    }
                    else
                    {
                        transactionToModify.TransactionType = transaction.TransactionType;
                        transactionToModify.Amount += transaction.Amount;
                        await _balanceService.ActualBalance(transactionToModify);
                        transactionToModify.Amount = transaction.Amount;
                    }
                }

                _context.Entry(transactionToModify).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return transactionToModify;
            }
            catch
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }

        public async Task<Transaction> DeleteAsync(int id)
        {
            try
            {
                Transaction transaction = await _context.Transactions.FindAsync(id);
                if (transaction != null && transaction.TransactionType != TransactionType.Reserve)
                {
                    transaction.IsActive = false;
                    _context.Entry(transaction).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    await _balanceService.ActualBalance(transaction);
                    return transaction;
                }

                return null;
            }
            catch
            {
                throw new InvalidOperationException("Error");
            }
        }

        public async Task<PagedList<Transaction>> GetByTypeAsync(TransactionPayloadDTO transactionPayload, PaginationPayloadDTO parameters)
        {
            try
            {
                IQueryable<Transaction> transactionQuery = _context.Transactions
                                                           .Include(t => t.Category)
                                                           .Include(t => t.Balance)
                                                           .Include(t => t.User)
                                                           .Where(t =>
                                                                  t.TransactionType == transactionPayload.TransactionType &&
                                                                  t.UserId == int.Parse(transactionPayload.UserId))
                                                           .OrderByDescending(t => t.Id);

                PagedList<Transaction> pagedTransactions = await PagedList<Transaction>.Paginate(
                                                            transactionQuery,
                                                            parameters.Page,
                                                            parameters.PageSize);
                return pagedTransactions;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        public async Task<PagedList<Transaction>> FilterAsync(TransactionPayloadDTO transactionPayload, PaginationPayloadDTO parameters)
        {
            try
            {
                IQueryable<Transaction> transactionQuery = null;
                if (transactionPayload.AmountUpTo != null &&
                    transactionPayload.TransactionType == null &&
                    transactionPayload.Date == null &&
                    transactionPayload.IsActive == null)
                {
                    transactionQuery = _context.Transactions
                                       .Include(t => t.Category)
                                       .Include(t => t.Balance)
                                       .Include(t => t.User)
                                       .Where(t =>
                                            t.UserId == int.Parse(transactionPayload.UserId) &&
                                            t.Amount <= transactionPayload.AmountUpTo)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transactionPayload.AmountUpTo != null &&
                        transactionPayload.TransactionType != null &&
                        transactionPayload.Date == null &&
                        transactionPayload.IsActive == null)
                {
                    transactionQuery = _context.Transactions
                                       .Include(t => t.Category)
                                       .Include(t => t.Balance)
                                       .Include(t => t.User)
                                       .Where(t =>
                                            t.UserId == int.Parse(transactionPayload.UserId) &&
                                            t.Amount <= transactionPayload.AmountUpTo &&
                                            t.TransactionType == transactionPayload.TransactionType)
                                       .OrderByDescending(t => t.Id);
                }

                else if (transactionPayload.AmountUpTo != null &&
                        transactionPayload.TransactionType != null &&
                        transactionPayload.Date != null &&
                        transactionPayload.IsActive == null)
                {
                    transactionQuery = _context.Transactions
                                       .Include(t => t.Category)
                                       .Include(t => t.Balance)
                                       .Include(t => t.User)
                                       .Where(t =>
                                            t.UserId == int.Parse(transactionPayload.UserId) &&
                                            t.Amount <= transactionPayload.AmountUpTo &&
                                            t.TransactionType == transactionPayload.TransactionType &&
                                            t.Date.ToString() == transactionPayload.Date)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transactionPayload.AmountUpTo != null &&
                        transactionPayload.TransactionType == null &&
                        transactionPayload.Date != null &&
                        transactionPayload.IsActive == null)
                {
                    transactionQuery = _context.Transactions
                                       .Include(t => t.Category)
                                       .Include(t => t.Balance)
                                       .Include(t => t.User)
                                       .Where(t =>
                                            t.UserId == int.Parse(transactionPayload.UserId) &&
                                            t.Amount <= transactionPayload.AmountUpTo &&
                                            t.Date.ToString() == transactionPayload.Date)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transactionPayload.AmountUpTo != null &&
                        transactionPayload.TransactionType == null &&
                        transactionPayload.Date != null &&
                        transactionPayload.IsActive != null)
                {
                    transactionQuery = _context.Transactions
                                       .Include(t => t.Category)
                                       .Include(t => t.Balance)
                                       .Include(t => t.User)
                                       .Where(t =>
                                            t.UserId == int.Parse(transactionPayload.UserId) &&
                                            t.Amount <= transactionPayload.AmountUpTo &&
                                            t.Date.ToString() == transactionPayload.Date &&
                                            t.IsActive == transactionPayload.IsActive)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transactionPayload.AmountUpTo != null &&
                        transactionPayload.TransactionType == null &&
                        transactionPayload.Date == null &&
                        transactionPayload.IsActive != null)
                {
                    transactionQuery = _context.Transactions
                                       .Include(t => t.Category)
                                       .Include(t => t.Balance)
                                       .Include(t => t.User)
                                       .Where(t =>
                                            t.UserId == int.Parse(transactionPayload.UserId) &&
                                            t.Amount <= transactionPayload.AmountUpTo &&
                                            t.IsActive == transactionPayload.IsActive)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transactionPayload.AmountUpTo != null &&
                        transactionPayload.TransactionType != null &&
                        transactionPayload.Date == null &&
                        transactionPayload.IsActive != null)
                {
                    transactionQuery = _context.Transactions
                                       .Include(t => t.Category)
                                       .Include(t => t.Balance)
                                       .Include(t => t.User)
                                       .Where(t =>
                                            t.UserId == int.Parse(transactionPayload.UserId) &&
                                            t.Amount <= transactionPayload.AmountUpTo &&
                                            t.IsActive == transactionPayload.IsActive &&
                                            t.TransactionType == transactionPayload.TransactionType)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transactionPayload.AmountUpTo == null &&
                        transactionPayload.TransactionType != null &&
                        transactionPayload.Date == null &&
                        transactionPayload.IsActive == null)
                {
                    transactionQuery = _context.Transactions
                                       .Include(t => t.Category)
                                       .Include(t => t.Balance)
                                       .Include(t => t.User)
                                       .Where(t =>
                                            t.UserId == int.Parse(transactionPayload.UserId) &&
                                            t.TransactionType == transactionPayload.TransactionType)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transactionPayload.AmountUpTo == null &&
                        transactionPayload.TransactionType != null &&
                        transactionPayload.Date != null &&
                        transactionPayload.IsActive == null)
                {
                    transactionQuery = _context.Transactions
                                       .Include(t => t.Category)
                                       .Include(t => t.Balance)
                                       .Include(t => t.User)
                                       .Where(t =>
                                            t.UserId == int.Parse(transactionPayload.UserId) &&
                                            t.TransactionType == transactionPayload.TransactionType &&
                                            t.Date.ToString() == transactionPayload.Date)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transactionPayload.AmountUpTo == null &&
                        transactionPayload.TransactionType != null &&
                        transactionPayload.Date != null &&
                        transactionPayload.IsActive != null)
                {
                    transactionQuery = _context.Transactions
                                       .Include(t => t.Category)
                                       .Include(t => t.Balance)
                                       .Include(t => t.User)
                                       .Where(t =>
                                            t.UserId == int.Parse(transactionPayload.UserId) &&
                                            t.TransactionType == transactionPayload.TransactionType &&
                                            t.Date.ToString() == transactionPayload.Date &&
                                            t.IsActive == transactionPayload.IsActive)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transactionPayload.AmountUpTo == null &&
                        transactionPayload.TransactionType != null &&
                        transactionPayload.Date == null &&
                        transactionPayload.IsActive != null)
                {
                    transactionQuery = _context.Transactions
                                       .Include(t => t.Category)
                                       .Include(t => t.Balance)
                                       .Include(t => t.User)
                                       .Where(t =>
                                            t.UserId == int.Parse(transactionPayload.UserId) &&
                                            t.TransactionType == transactionPayload.TransactionType &&
                                            t.IsActive == transactionPayload.IsActive)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transactionPayload.AmountUpTo == null &&
                        transactionPayload.TransactionType == null &&
                        transactionPayload.Date != null &&
                        transactionPayload.IsActive == null)
                {
                    transactionQuery = _context.Transactions
                                       .Include(t => t.Category)
                                       .Include(t => t.Balance)
                                       .Include(t => t.User)
                                       .Where(t =>
                                            t.UserId == int.Parse(transactionPayload.UserId) &&
                                            t.Date.ToString() == transactionPayload.Date)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transactionPayload.AmountUpTo == null &&
                        transactionPayload.TransactionType == null &&
                        transactionPayload.Date != null &&
                        transactionPayload.IsActive != null)
                {
                    transactionQuery = _context.Transactions
                                       .Include(t => t.Category)
                                       .Include(t => t.Balance)
                                       .Include(t => t.User)
                                       .Where(t =>
                                            t.UserId == int.Parse(transactionPayload.UserId) &&
                                            t.Date.ToString() == transactionPayload.Date &&
                                            t.IsActive == transactionPayload.IsActive)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transactionPayload.AmountUpTo == null &&
                        transactionPayload.TransactionType == null &&
                        transactionPayload.Date == null &&
                        transactionPayload.IsActive != null)
                {
                    transactionQuery = _context.Transactions
                                       .Include(t => t.Category)
                                       .Include(t => t.Balance)
                                       .Include(t => t.User)
                                       .Where(t =>
                                            t.UserId == int.Parse(transactionPayload.UserId) &&
                                            t.IsActive == transactionPayload.IsActive)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transactionPayload.AmountUpTo == null &&
                        transactionPayload.TransactionType == null &&
                        transactionPayload.Date == null &&
                        transactionPayload.IsActive == null)
                {
                    transactionQuery = _context.Transactions
                                       .Include(t => t.Category)
                                       .Include(t => t.Balance)
                                       .Include(t => t.User)
                                       .Where(t => t.UserId == int.Parse(transactionPayload.UserId))
                                       .OrderByDescending(t => t.Id);
                }
                else
                {
                    transactionQuery = _context.Transactions
                                       .Include(t => t.Category)
                                       .Include(t => t.Balance)
                                       .Include(t => t.User)
                                       .Where(t =>
                                            t.UserId == int.Parse(transactionPayload.UserId) &&
                                            t.IsActive == transactionPayload.IsActive &&
                                            t.Amount <= transactionPayload.AmountUpTo &&
                                            t.TransactionType == transactionPayload.TransactionType &&
                                            t.Date.ToString() == transactionPayload.Date)
                                       .OrderByDescending(t => t.Id);
                }

                if (transactionQuery != null)
                {
                    PagedList<Transaction> pagedTransactions = await PagedList<Transaction>.Paginate(
                                                            transactionQuery,
                                                            parameters.Page,
                                                            parameters.PageSize);
                    return pagedTransactions;
                }
                return null;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }
    }
}
