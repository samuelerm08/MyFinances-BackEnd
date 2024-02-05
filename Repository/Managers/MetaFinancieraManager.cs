using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaFinanciero.WebApi.Data;
using SistemaFinanciero.WebApi.Models;
using SistemaFinanciero.WebApi.Models.DTOs;
using SistemaFinanciero.WebApi.Models.Pagination;
using SistemaFinanciero.WebApi.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaFinanciero.WebApi.Repository.Managers
{
    public class MetaFinancieraManager : IMetaFinanciera
    {
        private readonly DbFinancesContext _context;
        private readonly IBalance _balanceService;
        public MetaFinancieraManager(DbFinancesContext context, IBalance balanceService)
        {
            _context = context;
            _balanceService = balanceService;
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
                if (id != metaFinanciera.Id)
                {
                    return null;
                }
                _context.Entry(metaFinanciera).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return metaFinanciera;
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

        public async Task<MetaFinanciera> AgregarMonto(MetaPayload payload)
        {
            MetaFinanciera meta = null;
            try
            {
                meta = await _context.MetaFinanciera
                            .FirstOrDefaultAsync(meta => meta.Id == payload.MetaId);

                if (meta != null)
                {
                    meta.MontoActual = !meta.MontoActual.HasValue ? payload.Monto : meta.MontoActual + payload.Monto;

                    if (meta.MontoActual == meta.MontoFinal)
                    {
                        meta.Completada = true;
                        await _balanceService.BalanceMeta(meta, payload.Monto);
                        _context.Entry(meta).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                        return meta;
                    }
                    else if (meta.MontoActual <= meta.MontoFinal)
                    {
                        await _balanceService.BalanceMeta(meta, payload.Monto);
                        _context.Entry(meta).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                        return meta;
                    }
                    else
                    {
                        throw new Exception("Monto actual sobrepasa el monto final");
                    }
                }

                return meta;
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
