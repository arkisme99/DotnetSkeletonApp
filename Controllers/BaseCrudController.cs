using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Models.ViewModels;
using DotnetSkeletonApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSkeletonApp.Controllers
{
    public class BaseCrudController<TModel, TService>(
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

        /* public virtual async Task<IActionResult> Index()
        {

            // SetBreadcrumbs(BreadcrumbItems);

            _logger.LogInformation($"Masuk Ke Index {_service!.GetType().Name}");

            var data = await _service.GetAllData();
            return Ok(new { count = data.Count, items = data });
        } */

        private List<BreadcrumbsViewModel> GetBaseBreadcrumbs()
        {
            return
            [
                new ("Home", false, "Home", "Index"),
                new (ControllerName, false, ControllerName, "Index")
            ];
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
    }
}