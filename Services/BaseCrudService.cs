using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Data;
using DotnetSkeletonApp.Helpers;
using DotnetSkeletonApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetSkeletonApp.Services
{
    public class BaseCrudService<TModel, TKey>(
        ApplicationDbContext context
    ) where TModel : class
    {
        protected readonly ApplicationDbContext _context = context;

        public virtual async Task<Dictionary<string, object>> IndexData()
        {
            return await Task.FromResult(new Dictionary<string, object>());
        }

        public virtual async Task<Dictionary<string, object>> CreateData()
        {
            return await Task.FromResult(new Dictionary<string, object>());
        }
        public virtual async Task<Dictionary<string, object>> EditData(TKey Id)
        {
            return await Task.FromResult(new Dictionary<string, object>());
        }

        protected virtual async Task<string?> ProcessUpload(IFormFile file, string subFolder)
        {
            return await FileHelper.UploadFile(file, subFolder);
        }

        protected virtual string? ProcessDelete(string fileName, string subFolder)
        {
            return FileHelper.DeleteFile(fileName, subFolder);
        }


        public virtual IQueryable<TModel> GetQueryAble()
        {
            return _context.Set<TModel>().AsQueryable();
        }

        public virtual async Task<List<TModel>> GetAllData()
        {
            return await _context.Set<TModel>().ToListAsync();

        }

        public virtual async Task<TModel> GetByIdAsync(TKey id)
        {
            var data = await _context.Set<TModel>().FindAsync(id);
            return data!;
        }

        protected virtual async Task<TModel> BeforeCreateAsync(TModel model, IFormCollection RawFormData)
        {
            return model;
        }

        protected virtual async Task<TModel> AfterCreateAsync(TModel model, IFormCollection RawFormData)
        {
            return model;
        }

        protected virtual async Task<TModel> BeforeUpdateAsync(TModel model, IFormCollection RawFormData)
        {
            return model;
        }

        protected virtual async Task<TModel> AfterUpdateAsync(TModel model, IFormCollection RawFormData)
        {
            return model;
        }

        protected virtual async Task<TModel> BeforeDeleteAsync(TModel model)
        {
            return model;
        }

        protected virtual async Task<TModel> AfterDeleteAsync(TModel model)
        {
            return model;
        }

        public virtual async Task<TModel> CreateAsync(TModel tmodel, IFormCollection RawFormData)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                tmodel = await BeforeCreateAsync(tmodel, RawFormData);

                _context.Set<TModel>().Add(tmodel);
                await _context.SaveChangesAsync();

                tmodel = await AfterCreateAsync(tmodel, RawFormData);

                await transaction.CommitAsync();
                return tmodel;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public virtual async Task<TModel> UpdateAsync(TModel tmodel, IFormCollection RawFormData)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                tmodel = await BeforeUpdateAsync(tmodel, RawFormData);

                _context.Set<TModel>().Update(tmodel);
                await _context.SaveChangesAsync();

                tmodel = await AfterUpdateAsync(tmodel, RawFormData);

                await transaction.CommitAsync();
                return tmodel;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(TKey Id)
        {

            var tmodel = await GetByIdAsync(Id) ?? throw new Exception("Data not found");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                tmodel = await BeforeDeleteAsync(tmodel);

                _context.Set<TModel>().Remove(tmodel);

                await _context.SaveChangesAsync();

                tmodel = await AfterDeleteAsync(tmodel);

                // Commit transaction
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}