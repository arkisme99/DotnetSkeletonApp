using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Data;
using DotnetSkeletonApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetSkeletonApp.Services
{
    public class BaseCrudService<TModel>(
        ApplicationDbContext context
    ) where TModel : class
    {
        protected readonly ApplicationDbContext _context = context;
        public virtual IQueryable<TModel> GetQueryAble()
        {
            return _context.Set<TModel>().AsQueryable();
        }

        public virtual async Task<List<TModel>> GetAllData()
        {
            return await _context.Set<TModel>().ToListAsync();

        }

        public virtual async Task<TModel> GetByIdAsync(Guid id)
        {
            var data = await _context.Set<TModel>().FindAsync(id);
            return data!;
        }

        public virtual async Task<TModel> UpdateAsync(TModel tmodel)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Set<TModel>().Update(tmodel);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return tmodel;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}