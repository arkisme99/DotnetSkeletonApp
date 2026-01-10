using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DotnetSkeletonApp.Helpers;
using DotnetSkeletonApp.Models.ViewModels;
using DotnetSkeletonApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSkeletonApp.Controllers
{
    public abstract class BaseCrudController<TModel, TService>(
        ILogger<BaseCrudController<TModel, TService>> logger,
        TService service
    ) : BaseController
        where TModel : class
        where TService : BaseCrudService<TModel>
    {
        protected virtual string ModelName => typeof(TModel).Name;
        protected virtual string ControllerName => Request.RouteValues["controller"]?.ToString() ?? "";
        protected readonly TService _service = service;
        protected readonly ILogger<BaseCrudController<TModel, TService>> _logger = logger;

        private List<BreadcrumbsViewModel> GetBaseBreadcrumbs()
        {
            return
            [
                new ("Home", false, "Home", "Index"),
                new (ControllerName, false, ControllerName, "Index")
            ];
        }

        // String konstan untuk folder (opsional)
        protected const string UploadDir = "others";

        protected async Task<string?> ProcessUpload(IFormFile file, string subFolder)
        {
            return await FileHelper.UploadFile(file, subFolder);
        }

        protected void ProcessDelete(string fileName, string subFolder)
        {
            FileHelper.DeleteFile(fileName, subFolder);
        }

        public virtual IActionResult Index()
        {
            var breadcrumbs = GetBaseBreadcrumbs();
            breadcrumbs.Last().Active = true;
            SetBreadcrumbs([.. breadcrumbs]);

            _logger.LogInformation($"Masuk Ke Index {_service!.GetType().Name}");
            return View();
        }

        public virtual IActionResult Create()
        {
            // Di sini false, karena kita tambah level "Tambah" yang true
            var breadcrumbs = GetBaseBreadcrumbs();
            breadcrumbs.Add(new BreadcrumbsViewModel($"Tambah {ControllerName}", true, ControllerName, "Create"));
            SetBreadcrumbs([.. breadcrumbs]);

            return View();
        }

        public virtual IActionResult Edit(Guid id)
        {
            // Di sini false, karena kita tambah level "Tambah" yang true
            var breadcrumbs = GetBaseBreadcrumbs();
            breadcrumbs.Add(new BreadcrumbsViewModel($"Edit {ControllerName}", true, ControllerName, "Edit"));
            SetBreadcrumbs([.. breadcrumbs]);

            // var product = await _service.GetByIdAsync(id);
            // if (product == null) return NotFound();
            // return View(product);

            return View(id);
        }

        protected virtual Dictionary<string, Expression<Func<TModel, object>>> GetColumnMap()
        {
            // Defaultnya kosong, anak tidak wajib override
            return [];
        }

        [HttpPost]
        public virtual IActionResult GetDataTable()
        {
            var req = DataTableHelper.GetDataTableRequest(Request);

            var columnMap = GetColumnMap();

            var query = _service.GetQueryAble()
                                .ApplyDataTableRequest(req, columnMap);

            // var sqlnya = query.ToQueryString();
            var recordsTotal = query.Count();
            var data = query.Skip(req.Start).Take(req.Length).ToList();

            return Json(new DataTableResponse<TModel>
            {
                Draw = req.Draw,
                RecordsFiltered = recordsTotal,
                RecordsTotal = recordsTotal,
                // QueryString = sqlnya,
                Data = data,
            });
        }
    }
}