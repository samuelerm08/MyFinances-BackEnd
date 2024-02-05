using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.EntityFrameworkCore;
using SistemaFinanciero.WebApi.Data;
using SistemaFinanciero.WebApi.Models;
using SistemaFinanciero.WebApi.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaFinanciero.WebApi.Repository.Managers
{
    public class CategoriaManager : ICategoria
    {

        private readonly DbFinancesContext _context;
        public CategoriaManager(DbFinancesContext context)
        {
            _context = context;
        }

        public async Task<Categoria> AltaAsync(Categoria cat)
        {
            try
            {

                bool yaExiste = await _context.Categoria.AnyAsync(c => c.Titulo == cat.Titulo);
                if (yaExiste)
                {
                    throw new InvalidOperationException("Categoria existente");
                }

                _context.Categoria.Add(cat);
                await _context.SaveChangesAsync();
                return cat;
            }
            catch
            {
                throw new InvalidOperationException();

            }
        }

        public async Task<Categoria> BajaAsync(int id)
        {
            try
            {
                Categoria cat = await _context.Categoria.SingleOrDefaultAsync(c => c.Id == id);

                if (cat != null)
                {
                    _context.Categoria.Remove(cat);
                    await _context.SaveChangesAsync();

                    return cat;
                }
                return null;
            }
            catch
            {
                throw new InvalidOperationException();

            }
        }

        public async Task<Categoria> ModificarAsync(int id, Categoria entidad)
        {
            try
            {
                if (id != entidad.Id)
                {
                    return null;
                }
                _context.Entry(entidad).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return entidad;
            }
            catch
            {
                throw new InvalidOperationException(new Exception().Message);

            }
        }

        public async Task<Categoria> ObtenerPorIdAsync(int id)
        {
            try
            {
                return await _context.Categoria.SingleOrDefaultAsync(c => c.Id == id);
            }
            catch
            {
                throw new InvalidOperationException();
            }

        }

        public async Task<IEnumerable<Categoria>> ObtenerTodoAsync()
        {
            try
            {
                IEnumerable<Categoria> categorias = await _context.Categoria.ToListAsync();

                return categorias;
            }

            catch
            {
                throw new InvalidOperationException();

            }

        }
    }
}
