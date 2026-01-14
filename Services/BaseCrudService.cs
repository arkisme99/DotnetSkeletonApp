using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Data;
using DotnetSkeletonApp.Helpers;
using DotnetSkeletonApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetSkeletonApp.Services
{
    public class BaseCrudService<TModel, TKey, TViewModel>(
        ApplicationDbContext context
    )
        where TModel : class
        where TViewModel : class
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
            var query = _context.Set<TModel>().AsQueryable();
            // var sql = query.ToQueryString();
            // Console.WriteLine("Cek SQL : " + sql);
            return query;
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

        protected virtual async Task<TModel> BeforeCreateAsync(TModel model, TViewModel tviewmodel)
        {
            return await Task.FromResult(model);
        }

        protected virtual async Task<TModel> AfterCreateAsync(TModel model, TViewModel tviewmodel)
        {
            return await Task.FromResult(model);
        }

        protected virtual async Task<TModel> BeforeUpdateAsync(TModel model, TViewModel tviewmodel)
        {
            return await Task.FromResult(model);
        }

        protected virtual async Task<TModel> AfterUpdateAsync(TModel model, TViewModel tviewmodel)
        {
            return await Task.FromResult(model);
        }

        protected virtual async Task<TModel> BeforeDeleteAsync(TModel model)
        {
            return await Task.FromResult(model);
        }

        protected virtual async Task<TModel> AfterDeleteAsync(TModel model)
        {
            return await Task.FromResult(model);
        }

        // public virtual async Task<TModel> CreateAsync(TModel tmodel, IFormCollection RawFormData)
        public virtual async Task<TModel> CreateAsync(TModel tmodel, TViewModel tviewmodel)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                tmodel = await BeforeCreateAsync(tmodel, tviewmodel);

                // Console.WriteLine($"Check Data {tmodel.ToString()}");

                _context.Set<TModel>().Add(tmodel);
                await _context.SaveChangesAsync();

                tmodel = await AfterCreateAsync(tmodel, tviewmodel);

                await transaction.CommitAsync();
                return tmodel;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public virtual async Task<TModel> UpdateAsync(TModel tmodel, TViewModel tviewmodel)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                tmodel = await BeforeUpdateAsync(tmodel, tviewmodel);

                _context.Set<TModel>().Update(tmodel);
                await _context.SaveChangesAsync();

                tmodel = await AfterUpdateAsync(tmodel, tviewmodel);

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

        public async Task<int> DeleteMultisAsync(string ids)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrWhiteSpace(ids))
                    return 0;

                var idList = ids.Split(',')
                                .Select(id => id.Trim())
                                .Where(id => !string.IsNullOrWhiteSpace(id))
                                .ToList();

                int deletedCount = 0;

                foreach (var Id in idList)
                {
                    TKey convertedId = (TKey)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromInvariantString(Id)!;

                    var tmodel = await GetByIdAsync(convertedId) ?? throw new Exception("Data not found");

                    // Console.WriteLine("Di sini : " + tmodel.ToString());

                    tmodel = await BeforeDeleteAsync(tmodel);

                    _context.Set<TModel>().Remove(tmodel);

                    await _context.SaveChangesAsync();

                    tmodel = await AfterDeleteAsync(tmodel);

                    deletedCount++;
                }

                if (deletedCount > 0)
                    await _context.SaveChangesAsync();
                // Commit transaction
                await transaction.CommitAsync();

                return deletedCount;

            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

    }
}