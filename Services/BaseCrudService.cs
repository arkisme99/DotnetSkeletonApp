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
        ApplicationDbContext _context
    ) where TModel : class
    {
        public virtual IQueryable<TModel> GetAll()
        {
            return _context.Set<TModel>().AsQueryable();
        }

        public virtual async Task<List<TModel>> GetAllData()
        {
            return await _context.Set<TModel>().ToListAsync();

        }
    }
}