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
    }
}