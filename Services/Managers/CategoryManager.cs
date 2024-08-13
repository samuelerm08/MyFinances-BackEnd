using Microsoft.EntityFrameworkCore;
using MyFinances.WebApi.Data;
using MyFinances.WebApi.Models;
using MyFinances.WebApi.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFinances.WebApi.Repository.Managers
{
    public class CategoryManager : ICategory
    {
        private readonly DbFinancesContext _context;
        public CategoryManager(DbFinancesContext context)
        {
            _context = context;
        }
        public async Task<Category> CreateAsync(Category category)
        {
            try
            {
                bool exists = await _context.Categories.AnyAsync(c => c.Title == category.Title);
                if (exists) throw new InvalidOperationException("Category already exists.");

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return category;
            }
            catch { throw new InvalidOperationException(); }
        }
        public async Task<Category> DeleteAsync(int id)
        {
            try
            {
                Category category = await _context.Categories.SingleOrDefaultAsync(c => c.Id == id);
                if (category != null)
                {
                    _context.Categories.Remove(category);
                    await _context.SaveChangesAsync();
                    return category;
                }
                return null;
            }
            catch
            {
                throw new InvalidOperationException();
            }
        }

        public async Task<Category> ModifyAsync(int? id, Category category)
        {
            try
            {
                if (id != category.Id)
                {
                    return null;
                }
                _context.Entry(category).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return category;
            }
            catch
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }

        public async Task<Category> GetByIdAsync(int? id)
        {
            try
            {
                return await _context.Categories.SingleOrDefaultAsync(c => c.Id == id);
            }
            catch
            {
                throw new InvalidOperationException();
            }
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            try
            {
                return await _context.Categories.ToListAsync();
            }
            catch
            {
                throw new InvalidOperationException();
            }
        }
    }
}
