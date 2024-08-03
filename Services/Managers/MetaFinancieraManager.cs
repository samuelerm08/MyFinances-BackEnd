using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaFinanciero.WebApi.Data;
using SistemaFinanciero.WebApi.Models;
using SistemaFinanciero.WebApi.Models.DTOs;
using SistemaFinanciero.WebApi.Models.Pagination;
using SistemaFinanciero.WebApi.Repository.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaFinanciero.WebApi.Repository.Managers
{
    public class MetaFinancieraManager : IMetaFinanciera
    {
        private readonly DbFinancesContext _context;
        private readonly IBalance _balanceService;
        private readonly ILogger<MetaFinancieraManager> _logger;
        public MetaFinancieraManager(DbFinancesContext context, IBalance balanceService, ILogger<MetaFinancieraManager> logger)
        {
            _context = context;
            _balanceService = balanceService;
            _logger = logger;
        }

        public async Task<MetaFinanciera> AltaAsync(MetaFinanciera metaFinanciera)
        {
            try
            {
                await _context.MetaFinanciera.AddAsync(metaFinanciera);
                await _context.SaveChangesAsync();
                return metaFinanciera;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        public async Task<MetaFinanciera> ModificarAsync(int id, MetaFinanciera metaFinanciera)
        {
            try
            {
                if (id != metaFinanciera.Id) return null;

                MetaFinanciera goalToModify = await _context.MetaFinanciera
                                                    .FirstOrDefaultAsync(m => m.Id == metaFinanciera.Id);
                bool titleHasChanged = goalToModify.Titulo != metaFinanciera.Titulo;
                bool finalAmountHasChanged = goalToModify.MontoFinal != metaFinanciera.MontoFinal;
                bool currentAmountHasChanged = goalToModify.MontoActual != metaFinanciera.MontoActual;

                goalToModify.Titulo = titleHasChanged ? metaFinanciera.Titulo : goalToModify.Titulo;

                if (!finalAmountHasChanged)
                {
                    if (currentAmountHasChanged)
                    {
                        if (metaFinanciera.MontoActual <= goalToModify.MontoFinal)
                        {
                            await ModifyCurrentAmount(metaFinanciera, goalToModify);
                            _logger.LogInformation($"Monto actual modificado. Meta: {goalToModify.Titulo}");
                        }
                        else
                        {
                            _logger.LogError($"Monto actual sobrepasa el monto final\n\t" +
                                             $"Monto Actual: {metaFinanciera.MontoActual}\n\t" +
                                             $"Monto Final: {goalToModify.MontoFinal}");
                            throw new InvalidOperationException();
                        }
                    }
                }
                else
                {
                    if (metaFinanciera.MontoFinal < metaFinanciera.MontoActual)
                    {
                        _logger.LogError($"El monto final no puede ser menor al monto actual\n\t" +
                                         $"Monto Final: {metaFinanciera.MontoFinal}\n\t" +
                                         $"Monto Actual: {metaFinanciera.MontoActual}");
                        throw new InvalidOperationException();
                    }
                    else
                    {
                        if (currentAmountHasChanged)
                        {
                            await ModifyCurrentAmount(metaFinanciera, goalToModify);
                            _logger.LogInformation($"Monto actual modificado. Meta: {goalToModify.Titulo}");
                        }
                        goalToModify.MontoFinal = metaFinanciera.MontoFinal;
                        _logger.LogInformation($"Monto final modificado. Meta: {goalToModify.Titulo}");
                    }
                }

                goalToModify.Completada = goalToModify.MontoActual == goalToModify.MontoFinal;
                _context.Entry(goalToModify).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return goalToModify;
            }
            catch
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }

        private async Task ModifyCurrentAmount(MetaFinanciera metaFinanciera, MetaFinanciera goalToModify)
        {
            double amountToDiscount = !goalToModify.MontoActual.HasValue ? 
                                      metaFinanciera.MontoActual.Value :
                                      metaFinanciera.MontoActual.Value - goalToModify.MontoActual.Value;
            double amountToModify = amountToDiscount > 0 ? amountToDiscount : -amountToDiscount;
            bool isLower = metaFinanciera.MontoActual < goalToModify.MontoActual;
            await _balanceService.BalanceMeta(goalToModify, amountToModify, true, isLower);
            goalToModify.MontoActual = metaFinanciera.MontoActual;
        }

        public async Task<MetaFinanciera> EliminarAsync(int goalId)
        {
            try
            {
                MetaFinanciera goalToDelete = 
                    await _context.MetaFinanciera.SingleOrDefaultAsync(m => m.Id == goalId);

                if (goalToDelete != null)
                {
                    await _balanceService.BalanceMeta(goalToDelete, isDeleted: true);
                    _context.MetaFinanciera.Remove(goalToDelete);
                    await _context.SaveChangesAsync();
                }

                return goalToDelete;
            }
            catch
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }

        public async Task<PagedList<MetaFinanciera>> MetasPorUserId(MetaFinancieraPayload metaFinancieraPayload, PaginationPayloadDTO parameters)
        {
            try
            {
                IQueryable<MetaFinanciera> metasQuery = _context.MetaFinanciera
                                                        .Include(m => m.Usuario)
                                                        .Where(m => m.UsuarioId == metaFinancieraPayload.UserId)
                                                        .OrderByDescending(m => m.Id);

                PagedList<MetaFinanciera> pagedTransactions = await PagedList<MetaFinanciera>.Paginar(
                                                              metasQuery,
                                                              parameters.Page,
                                                              parameters.PageSize);
                return pagedTransactions;
            }
            catch (Exception)
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }

        public async Task<PagedList<MetaFinanciera>> MetasPorEstado(MetaFinancieraPayload metaFinancieraPayload, PaginationPayloadDTO parameters)
        {
            try
            {
                IQueryable<MetaFinanciera> metasQuery = _context.MetaFinanciera
                                                        .Include(m => m.Usuario)
                                                        .Where(m =>
                                                              !m.Retirada &&
                                                               m.Completada == metaFinancieraPayload.Completada &&
                                                               m.UsuarioId == metaFinancieraPayload.UserId)
                                                        .OrderByDescending(m => m.Id);

                PagedList<MetaFinanciera> pagedTransactions = await PagedList<MetaFinanciera>.Paginar(
                                                              metasQuery,
                                                              parameters.Page,
                                                              parameters.PageSize);
                return pagedTransactions;
            }
            catch (Exception)
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }

        public async Task<MetaFinanciera> RetirarMontoMeta(int metaId)
        {
            MetaFinanciera meta = null;
            try
            {
                meta = await _context.MetaFinanciera
                            .FirstOrDefaultAsync(meta => meta.Id == metaId);

                if (meta != null)
                {
                    if (meta.Retirada)
                    {
                        _logger.LogError("La meta que se intenta retirar ya esta retirada.");
                        throw new InvalidOperationException(new Exception().Message);
                    }

                    if (!meta.Completada)
                    {
                        _logger.LogError("La meta debe estar completada para poder ser retirada.");
                        throw new InvalidOperationException(new Exception().Message);
                    }

                    meta.Retirada = true;
                    await _balanceService.BalanceMeta(meta);
                    _context.Entry(meta).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }

                return meta;
            }
            catch (Exception)
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }
    }
}
