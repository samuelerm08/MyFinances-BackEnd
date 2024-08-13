using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyFinances.WebApi.Data;
using MyFinances.WebApi.Models;
using MyFinances.WebApi.Models.DTOs;
using MyFinances.WebApi.Models.Pagination;
using MyFinances.WebApi.Repository.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinances.WebApi.Repository.Managers
{
    public class FinancialGoalManager : IFinancialGoal
    {
        private readonly DbFinancesContext _context;
        private readonly IBalance _balanceService;
        private readonly ILogger<FinancialGoalManager> _logger;
        public FinancialGoalManager(DbFinancesContext context, IBalance balanceService, ILogger<FinancialGoalManager> logger)
        {
            _context = context;
            _balanceService = balanceService;
            _logger = logger;
        }

        public async Task<FinancialGoal> CreateAsync(FinancialGoal financialGoal)
        {
            try
            {
                await _context.FinancialGoals.AddAsync(financialGoal);
                await _context.SaveChangesAsync();
                return financialGoal;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        public async Task<FinancialGoal> ModifyAsync(int id, FinancialGoal financialGoal)
        {
            try
            {
                if (id != financialGoal.Id) return null;

                FinancialGoal goalToModify = await _context.FinancialGoals
                                                    .FirstOrDefaultAsync(m => m.Id == financialGoal.Id);
                bool titleHasChanged = goalToModify.Title != financialGoal.Title;
                bool finalAmountHasChanged = goalToModify.FinalAmount != financialGoal.FinalAmount;
                 bool currentAmountHasChanged = goalToModify.CurrentAmount != financialGoal.CurrentAmount;

                goalToModify.Title = titleHasChanged ? financialGoal.Title : goalToModify.Title;

                if (!finalAmountHasChanged)
                {
                    if (currentAmountHasChanged)
                    {
                        if (financialGoal.CurrentAmount <= goalToModify.FinalAmount)
                        {
                            await ModifyCurrentAmount(financialGoal, goalToModify);
                            _logger.LogInformation($"Current amount modified. Goal: {goalToModify.Title}");
                        }
                        else
                        {
                            _logger.LogError($"Current amount over final amount\n\t" +
                                             $"Current amount: {financialGoal.CurrentAmount}\n\t" +
                                             $"Final amount: {goalToModify.FinalAmount}");
                            throw new InvalidOperationException();
                        }
                    }
                }
                else
                {
                    if (financialGoal.FinalAmount < financialGoal.CurrentAmount)
                    {
                        _logger.LogError($"The final amount cannot be less than the current amount\n\t" +
                                         $"Final Amount: {financialGoal.FinalAmount}\n\t" +
                                         $"Current Amount: {financialGoal.CurrentAmount}");

                        throw new InvalidOperationException();
                    }
                    else
                    {
                        if (currentAmountHasChanged)
                        {
                            await ModifyCurrentAmount(financialGoal, goalToModify);
                            _logger.LogInformation($"Current amount modified. Goal: {goalToModify.Title}");
                        }
                        goalToModify.FinalAmount = financialGoal.FinalAmount;
                        _logger.LogInformation($"Final amount modified. Goal: {goalToModify.Title}");
                    }
                }

                goalToModify.Completed = goalToModify.CurrentAmount == goalToModify.FinalAmount;
                _context.Entry(goalToModify).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return goalToModify;
            }
            catch
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }

        private async Task ModifyCurrentAmount(FinancialGoal metaFinanciera, FinancialGoal goalToModify)
        {
            double amountToDiscount = !goalToModify.CurrentAmount.HasValue ?
                                      metaFinanciera.CurrentAmount.Value :
                                      metaFinanciera.CurrentAmount.Value - goalToModify.CurrentAmount.Value;
            double amountToModify = amountToDiscount > 0 ? amountToDiscount : -amountToDiscount;
            bool isLower = metaFinanciera.CurrentAmount < goalToModify.CurrentAmount;
            await _balanceService.GoalBalance(goalToModify, amountToModify, true, isLower);
            goalToModify.CurrentAmount = metaFinanciera.CurrentAmount;
        }

        public async Task<FinancialGoal> DeleteAsync(int goalId)
        {
            try
            {
                FinancialGoal goalToDelete =
                    await _context.FinancialGoals.SingleOrDefaultAsync(m => m.Id == goalId);

                if (goalToDelete != null)
                {
                    await _balanceService.GoalBalance(goalToDelete, isDeleted: true);
                    _context.FinancialGoals.Remove(goalToDelete);
                    await _context.SaveChangesAsync();
                }

                return goalToDelete;
            }
            catch
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }

        public async Task<PagedList<FinancialGoal>> GoalsByUserId(FinancialGoalPayload financialGoalPayload, PaginationPayloadDTO parameters)
        {
            try
            {
                IQueryable<FinancialGoal> goalsQuery =
                    _context.FinancialGoals.Include(m => m.User).Where(m => m.UserId == financialGoalPayload.UserId)
                                                                .OrderByDescending(m => m.Id);
                PagedList<FinancialGoal> pagedTransactions =
                    await PagedList<FinancialGoal>.Paginate(goalsQuery, parameters.Page, parameters.PageSize);
                return pagedTransactions;
            }
            catch (Exception)
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }

        public async Task<PagedList<FinancialGoal>> GoalsByState(FinancialGoalPayload financialGoalPayload, PaginationPayloadDTO parameters)
        {
            try
            {
                IQueryable<FinancialGoal> goalsQuery =
                    _context.FinancialGoals.Include(m => m.User)
                                           .Where(m => !m.Withdrawn &&
                                                  m.Completed == financialGoalPayload.Completed &&
                                                  m.UserId == financialGoalPayload.UserId)
                                           .OrderByDescending(m => m.Id);

                PagedList<FinancialGoal> pagedTransactions = await PagedList<FinancialGoal>.Paginate(
                                                              goalsQuery,
                                                              parameters.Page,
                                                              parameters.PageSize);
                return pagedTransactions;
            }
            catch (Exception)
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }

        public async Task<FinancialGoal> Withdraw(int goalId)
        {
            FinancialGoal goal = null;
            try
            {
                goal = await _context.FinancialGoals.FirstOrDefaultAsync(goal => goal.Id == goalId);

                if (goal != null)
                {
                    if (goal.Withdrawn)
                    {
                        _logger.LogError("Goal is already withdrawn.");
                        throw new InvalidOperationException(new Exception().Message);
                    }

                    if (!goal.Completed)
                    {
                        _logger.LogError("Goal must be completed to be withdrawn.");
                        throw new InvalidOperationException(new Exception().Message);
                    }

                    goal.Withdrawn = true;
                    await _balanceService.GoalBalance(goal);
                    _context.Entry(goal).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }

                return goal;
            }
            catch (Exception)
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }
    }
}
