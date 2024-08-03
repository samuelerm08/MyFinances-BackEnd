using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaFinanciero.WebApi.Data;
using SistemaFinanciero.WebApi.Models;
using SistemaFinanciero.WebApi.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaFinanciero.WebApi.Repository.Managers
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

        public async Task<Balance> BalanceActual(Transaccion transaccion, bool isLower = false)
        {
            try
            {
                List<Transaccion> transacciones = await _context.Transacciones.Where(t => t.UsuarioId == transaccion.UsuarioId).ToListAsync();
                Balance balance = await _context.Balance.FirstOrDefaultAsync(b => b.Id == transaccion.Balance_Id);
                balance ??= new Balance();

                switch (transaccion.TipoTransaccion)
                {
                    case TipoTransaccion.Ingreso:
                        if (!transaccion.EstaActiva || isLower)
                            balance.Saldo_Total -= transaccion.Monto;
                        else
                            balance.Saldo_Total += transaccion.Monto;
                        break;
                    case TipoTransaccion.Egreso:
                        if (!transaccion.EstaActiva || isLower)
                            balance.Saldo_Total += transaccion.Monto;
                        else
                            balance.Saldo_Total -= transaccion.Monto;
                        break;
                }

                if (transacciones.Count() < 1)
                    await _context.Balance.AddAsync(balance);
                else
                    _context.Entry(balance).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Balance actualizado. Transaccion: {transaccion.Detalle}");
                return balance;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Balance> BalanceMeta(
                                     MetaFinanciera meta,
                                     double? montoActual,
                                     bool isModified = false,
                                     bool isLower = false,
                                     bool isDeleted = false)
        {
            try
            {
                List<Transaccion> transacciones = await _context.Transacciones
                                                        .Where(t => t.UsuarioId == meta.UsuarioId)
                                                        .ToListAsync();

                Balance balance = await GetBalanceByUserId(meta.UsuarioId) ?? new Balance();
                Categoria categoria = await _context.Categoria
                                            .Where(c => c.Titulo == TipoTransaccion.Reserva.ToString())
                                            .FirstOrDefaultAsync();

                string detailModified = "", detailReserveWithdraw = "";
                double transactionAmount = 0.0;
                if (!meta.Retirada && !isDeleted)
                {
                    detailReserveWithdraw = $"Reserva - Meta: {meta.Titulo}";
                    transactionAmount = montoActual.Value;
                    if (!isModified)
                        balance.Saldo_Total -= montoActual.Value;
                    else
                    {
                        detailModified = !isLower ?
                                            $"Modificacion/Monto Mayor - Meta: {meta.Titulo}" :
                                            $"Modificacion/Monto Menor - Meta: {meta.Titulo}";
                        balance.Saldo_Total = !isLower ?
                                              balance.Saldo_Total - montoActual.Value :
                                              balance.Saldo_Total + montoActual.Value;
                    }
                }
                else if (meta.Retirada || isDeleted)
                {
                    detailReserveWithdraw = !isDeleted ?
                                                $"Retiro - Meta: {meta.Titulo}" :
                                                $"Meta Eliminada: {meta.Titulo}";
                    transactionAmount = !isDeleted ? meta.MontoFinal : meta.MontoActual ?? 0;
                    balance.Saldo_Total += transactionAmount;
                }

                if (transacciones.Count() < 1)
                {
                    _context.Balance.Add(balance);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.Entry(balance).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }

                Transaccion newTransaction = new Transaccion()
                {
                    Fecha = DateTime.Now,
                    Detalle = !isModified ? detailReserveWithdraw : detailModified,
                    Monto = transactionAmount,
                    TipoTransaccion = TipoTransaccion.Reserva,
                    Cat_Id = categoria.Id,
                    Balance_Id = balance.Id,
                    UsuarioId = meta.UsuarioId,
                    EstaActiva = true
                };

                _context.Transacciones.Add(newTransaction);
                await _context.SaveChangesAsync();

                balance.TransaccionId = newTransaction.Id;
                _context.Entry(balance).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Balance actualizado. Meta: {meta.Titulo}");
                return balance;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Balance> GetBalanceByUserId(int userId)
        {
            try
            {
                var balance = await (from b in _context.Balance
                                     join t in _context.Transacciones on b.Id equals t.Balance_Id
                                     where t.UsuarioId == userId
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
