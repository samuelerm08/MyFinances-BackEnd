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
    public class TransaccionesManager : ITransacciones
    {
        private readonly DbFinancesContext _context;
        private readonly IBalance _balanceService;
        public TransaccionesManager(DbFinancesContext context, IBalance balanceService)
        {
            _context = context;
            _balanceService = balanceService;
        }

        public async Task<IEnumerable<Transaccion>> ObtenerTodoAsync()
        {
            try
            {
                return await _context.Transacciones
                            .Include(t => t.Categoria)
                            .Include(t => t.Balance)
                            .Include(t => t.Usuario)
                            .ToListAsync();
            }
            catch
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }
        public async Task<PagedList<Transaccion>> GetTransaccionesUserId(TransaccionPayloadDTO transaccionPayload, PaginationPayloadDTO parameters)
        {
            try
            {
                IQueryable<Transaccion> transaccionesQuery = _context.Transacciones
                                                            .Include(t => t.Categoria)
                                                            .Include(t => t.Balance)
                                                            .Include(t => t.Usuario)
                                                            .Where(t => t.UsuarioId == int.Parse(transaccionPayload.UserId))
                                                            .OrderByDescending(t => t.Id);

                PagedList<Transaccion> pagedTransactions = await PagedList<Transaccion>.Paginar(
                                                            transaccionesQuery,
                                                            parameters.Page,
                                                            parameters.PageSize);

                return pagedTransactions;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        public async Task<Transaccion> ObtenerPorIdAsync(int id)
        {
            try
            {
                Transaccion transaccion = await _context.Transacciones
                    .Include(t => t.Categoria)
                    .Include(t => t.Balance)
                    .Include(t => t.Usuario)
                    .SingleOrDefaultAsync(t => t.Id == id);
                return transaccion;
            }
            catch
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }

        public async Task<Transaccion> AltaAsync(Transaccion transaccion)
        {
            try
            {
                Balance balanceCreado = await _balanceService.BalanceActual(transaccion);
                transaccion.Balance_Id = balanceCreado.Id;

                await _context.Transacciones.AddAsync(transaccion);
                await _context.SaveChangesAsync();

                balanceCreado.TransaccionId = transaccion.Id;
                _context.Entry(balanceCreado).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return transaccion;
            }
            catch
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }

        public async Task<Transaccion> ModificarAsync(int id, Transaccion transaccion)
        {
            try
            {
                if (id != transaccion.Id)
                {
                    return null;
                }

                Transaccion transactionToModify = await ObtenerPorIdAsync(id);
                transactionToModify.Fecha = transaccion.Fecha;
                transactionToModify.Detalle = transaccion.Detalle;
                transactionToModify.Cat_Id = transaccion.Cat_Id;
                transactionToModify.Categoria = _context.Categoria.Find(transactionToModify.Cat_Id);

                bool amountHasChanged = transactionToModify.Monto != transaccion.Monto;
                bool typeHasChanged = transactionToModify.TipoTransaccion != transaccion.TipoTransaccion;

                if (amountHasChanged || typeHasChanged)
                {
                    if (!typeHasChanged)
                    {
                        if (transaccion.Monto < transactionToModify.Monto)
                        {
                            transactionToModify.Monto -= transaccion.Monto;
                            await _balanceService.BalanceActual(transactionToModify, true);
                            transactionToModify.Monto = transaccion.Monto;
                        }
                        else
                        {
                            transactionToModify.Monto = transaccion.Monto - transactionToModify.Monto;
                            await _balanceService.BalanceActual(transactionToModify);
                            transactionToModify.Monto = transaccion.Monto;
                        }
                    }
                    else
                    {
                        transactionToModify.TipoTransaccion = transaccion.TipoTransaccion;
                        transactionToModify.Monto += transaccion.Monto;
                        await _balanceService.BalanceActual(transactionToModify);
                        transactionToModify.Monto = transaccion.Monto;
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

        public async Task<Transaccion> BajaAsync(int id)
        {
            try
            {
                Transaccion transaccion = await _context.Transacciones.FindAsync(id);

                if (transaccion != null && transaccion.TipoTransaccion != TipoTransaccion.Reserva)
                {
                    transaccion.EstaActiva = false;
                    _context.Entry(transaccion).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    await _balanceService.BalanceActual(transaccion);
                    return transaccion;
                }

                return null;
            }
            catch
            {
                throw new InvalidOperationException("Error");
            }
        }

        public async Task<PagedList<Transaccion>> GetTransaccionesTipo(TransaccionPayloadDTO transaccionPayload, PaginationPayloadDTO parameters)
        {
            try
            {
                IQueryable<Transaccion> transaccionQuery = _context.Transacciones
                                                           .Include(t => t.Categoria)
                                                           .Include(t => t.Balance)
                                                           .Include(t => t.Usuario)
                                                           .Where(t =>
                                                                  t.TipoTransaccion == transaccionPayload.Tipo &&
                                                                  t.UsuarioId == int.Parse(transaccionPayload.UserId))
                                                           .OrderByDescending(t => t.Id);

                PagedList<Transaccion> pagedTransactions = await PagedList<Transaccion>.Paginar(
                                                            transaccionQuery,
                                                            parameters.Page,
                                                            parameters.PageSize);
                return pagedTransactions;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        public async Task<PagedList<Transaccion>> GetFilteredTransactions(TransaccionPayloadDTO transaccionPayload, PaginationPayloadDTO parameters)
        {
            try
            {
                IQueryable<Transaccion> transaccionQuery = null;
                if (transaccionPayload.MontoHasta != null &&
                    transaccionPayload.Tipo == null &&
                    transaccionPayload.Fecha == null &&
                    transaccionPayload.EstaActiva == null)
                {
                    transaccionQuery = _context.Transacciones
                                       .Include(t => t.Categoria)
                                       .Include(t => t.Balance)
                                       .Include(t => t.Usuario)
                                       .Where(t =>
                                            t.UsuarioId == int.Parse(transaccionPayload.UserId) &&
                                            t.Monto <= transaccionPayload.MontoHasta)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transaccionPayload.MontoHasta != null &&
                        transaccionPayload.Tipo != null &&
                        transaccionPayload.Fecha == null &&
                        transaccionPayload.EstaActiva == null)
                {
                    transaccionQuery = _context.Transacciones
                                       .Include(t => t.Categoria)
                                       .Include(t => t.Balance)
                                       .Include(t => t.Usuario)
                                       .Where(t =>
                                            t.UsuarioId == int.Parse(transaccionPayload.UserId) &&
                                            t.Monto <= transaccionPayload.MontoHasta &&
                                            t.TipoTransaccion == transaccionPayload.Tipo)
                                       .OrderByDescending(t => t.Id);
                }

                else if (transaccionPayload.MontoHasta != null &&
                        transaccionPayload.Tipo != null &&
                        transaccionPayload.Fecha != null &&
                        transaccionPayload.EstaActiva == null)
                {
                    transaccionQuery = _context.Transacciones
                                       .Include(t => t.Categoria)
                                       .Include(t => t.Balance)
                                       .Include(t => t.Usuario)
                                       .Where(t =>
                                            t.UsuarioId == int.Parse(transaccionPayload.UserId) &&
                                            t.Monto <= transaccionPayload.MontoHasta &&
                                            t.TipoTransaccion == transaccionPayload.Tipo &&
                                            t.Fecha.ToString() == transaccionPayload.Fecha)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transaccionPayload.MontoHasta != null &&
                        transaccionPayload.Tipo == null &&
                        transaccionPayload.Fecha != null &&
                        transaccionPayload.EstaActiva == null)
                {
                    transaccionQuery = _context.Transacciones
                                       .Include(t => t.Categoria)
                                       .Include(t => t.Balance)
                                       .Include(t => t.Usuario)
                                       .Where(t =>
                                            t.UsuarioId == int.Parse(transaccionPayload.UserId) &&
                                            t.Monto <= transaccionPayload.MontoHasta &&
                                            t.Fecha.ToString() == transaccionPayload.Fecha)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transaccionPayload.MontoHasta != null &&
                        transaccionPayload.Tipo == null &&
                        transaccionPayload.Fecha != null &&
                        transaccionPayload.EstaActiva != null)
                {
                    transaccionQuery = _context.Transacciones
                                       .Include(t => t.Categoria)
                                       .Include(t => t.Balance)
                                       .Include(t => t.Usuario)
                                       .Where(t =>
                                            t.UsuarioId == int.Parse(transaccionPayload.UserId) &&
                                            t.Monto <= transaccionPayload.MontoHasta &&
                                            t.Fecha.ToString() == transaccionPayload.Fecha &&
                                            t.EstaActiva == transaccionPayload.EstaActiva)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transaccionPayload.MontoHasta != null &&
                        transaccionPayload.Tipo == null &&
                        transaccionPayload.Fecha == null &&
                        transaccionPayload.EstaActiva != null)
                {
                    transaccionQuery = _context.Transacciones
                                       .Include(t => t.Categoria)
                                       .Include(t => t.Balance)
                                       .Include(t => t.Usuario)
                                       .Where(t =>
                                            t.UsuarioId == int.Parse(transaccionPayload.UserId) &&
                                            t.Monto <= transaccionPayload.MontoHasta &&
                                            t.EstaActiva == transaccionPayload.EstaActiva)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transaccionPayload.MontoHasta != null &&
                        transaccionPayload.Tipo != null &&
                        transaccionPayload.Fecha == null &&
                        transaccionPayload.EstaActiva != null)
                {
                    transaccionQuery = _context.Transacciones
                                       .Include(t => t.Categoria)
                                       .Include(t => t.Balance)
                                       .Include(t => t.Usuario)
                                       .Where(t =>
                                            t.UsuarioId == int.Parse(transaccionPayload.UserId) &&
                                            t.Monto <= transaccionPayload.MontoHasta &&
                                            t.EstaActiva == transaccionPayload.EstaActiva &&
                                            t.TipoTransaccion == transaccionPayload.Tipo)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transaccionPayload.MontoHasta == null &&
                        transaccionPayload.Tipo != null &&
                        transaccionPayload.Fecha == null &&
                        transaccionPayload.EstaActiva == null)
                {
                    transaccionQuery = _context.Transacciones
                                       .Include(t => t.Categoria)
                                       .Include(t => t.Balance)
                                       .Include(t => t.Usuario)
                                       .Where(t =>
                                            t.UsuarioId == int.Parse(transaccionPayload.UserId) &&
                                            t.TipoTransaccion == transaccionPayload.Tipo)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transaccionPayload.MontoHasta == null &&
                        transaccionPayload.Tipo != null &&
                        transaccionPayload.Fecha != null &&
                        transaccionPayload.EstaActiva == null)
                {
                    transaccionQuery = _context.Transacciones
                                       .Include(t => t.Categoria)
                                       .Include(t => t.Balance)
                                       .Include(t => t.Usuario)
                                       .Where(t =>
                                            t.UsuarioId == int.Parse(transaccionPayload.UserId) &&
                                            t.TipoTransaccion == transaccionPayload.Tipo &&
                                            t.Fecha.ToString() == transaccionPayload.Fecha)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transaccionPayload.MontoHasta == null &&
                        transaccionPayload.Tipo != null &&
                        transaccionPayload.Fecha != null &&
                        transaccionPayload.EstaActiva != null)
                {
                    transaccionQuery = _context.Transacciones
                                       .Include(t => t.Categoria)
                                       .Include(t => t.Balance)
                                       .Include(t => t.Usuario)
                                       .Where(t =>
                                            t.UsuarioId == int.Parse(transaccionPayload.UserId) &&
                                            t.TipoTransaccion == transaccionPayload.Tipo &&
                                            t.Fecha.ToString() == transaccionPayload.Fecha &&
                                            t.EstaActiva == transaccionPayload.EstaActiva)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transaccionPayload.MontoHasta == null &&
                        transaccionPayload.Tipo != null &&
                        transaccionPayload.Fecha == null &&
                        transaccionPayload.EstaActiva != null)
                {
                    transaccionQuery = _context.Transacciones
                                       .Include(t => t.Categoria)
                                       .Include(t => t.Balance)
                                       .Include(t => t.Usuario)
                                       .Where(t =>
                                            t.UsuarioId == int.Parse(transaccionPayload.UserId) &&
                                            t.TipoTransaccion == transaccionPayload.Tipo &&
                                            t.EstaActiva == transaccionPayload.EstaActiva)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transaccionPayload.MontoHasta == null &&
                        transaccionPayload.Tipo == null &&
                        transaccionPayload.Fecha != null &&
                        transaccionPayload.EstaActiva == null)
                {
                    transaccionQuery = _context.Transacciones
                                       .Include(t => t.Categoria)
                                       .Include(t => t.Balance)
                                       .Include(t => t.Usuario)
                                       .Where(t =>
                                            t.UsuarioId == int.Parse(transaccionPayload.UserId) &&
                                            t.Fecha.ToString() == transaccionPayload.Fecha)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transaccionPayload.MontoHasta == null &&
                        transaccionPayload.Tipo == null &&
                        transaccionPayload.Fecha != null &&
                        transaccionPayload.EstaActiva != null)
                {
                    transaccionQuery = _context.Transacciones
                                       .Include(t => t.Categoria)
                                       .Include(t => t.Balance)
                                       .Include(t => t.Usuario)
                                       .Where(t =>
                                            t.UsuarioId == int.Parse(transaccionPayload.UserId) &&
                                            t.Fecha.ToString() == transaccionPayload.Fecha &&
                                            t.EstaActiva == transaccionPayload.EstaActiva)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transaccionPayload.MontoHasta == null &&
                        transaccionPayload.Tipo == null &&
                        transaccionPayload.Fecha == null &&
                        transaccionPayload.EstaActiva != null)
                {
                    transaccionQuery = _context.Transacciones
                                       .Include(t => t.Categoria)
                                       .Include(t => t.Balance)
                                       .Include(t => t.Usuario)
                                       .Where(t =>
                                            t.UsuarioId == int.Parse(transaccionPayload.UserId) &&
                                            t.EstaActiva == transaccionPayload.EstaActiva)
                                       .OrderByDescending(t => t.Id);
                }
                else if (transaccionPayload.MontoHasta == null &&
                        transaccionPayload.Tipo == null &&
                        transaccionPayload.Fecha == null &&
                        transaccionPayload.EstaActiva == null)
                {
                    transaccionQuery = _context.Transacciones
                                       .Include(t => t.Categoria)
                                       .Include(t => t.Balance)
                                       .Include(t => t.Usuario)
                                       .Where(t => t.UsuarioId == int.Parse(transaccionPayload.UserId))
                                       .OrderByDescending(t => t.Id);
                }
                else
                {
                    transaccionQuery = _context.Transacciones
                                       .Include(t => t.Categoria)
                                       .Include(t => t.Balance)
                                       .Include(t => t.Usuario)
                                       .Where(t =>
                                            t.UsuarioId == int.Parse(transaccionPayload.UserId) &&
                                            t.EstaActiva == transaccionPayload.EstaActiva &&
                                            t.Monto <= transaccionPayload.MontoHasta &&
                                            t.TipoTransaccion == transaccionPayload.Tipo &&
                                            t.Fecha.ToString() == transaccionPayload.Fecha)
                                       .OrderByDescending(t => t.Id);
                }

                if (transaccionQuery != null)
                {
                    PagedList<Transaccion> pagedTransactions = await PagedList<Transaccion>.Paginar(
                                                            transaccionQuery,
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
