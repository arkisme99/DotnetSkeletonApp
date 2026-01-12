using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DotnetSkeletonApp.Helpers;
using DotnetSkeletonApp.Models.ViewModels;
using DotnetSkeletonApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DotnetSkeletonApp.Controllers
{
    public abstract class BaseCrudController<TModel, TService, TKey>(
        ILogger<BaseCrudController<TModel, TService, TKey>> logger,
        TService service,
        IStringLocalizer<SharedResource> _localizer
    ) : BaseController
        where TModel : class
        where TService : BaseCrudService<TModel, TKey>
    {
        protected virtual string ModelName => typeof(TModel).Name;
        protected virtual string ControllerName => Request.RouteValues["controller"]?.ToString() ?? "";
        protected readonly TService _service = service;
        protected readonly ILogger<BaseCrudController<TModel, TService, TKey>> _logger = logger;
        // protected readonly IStringLocalizer<SharedResource> _localizer = localizer;

        public List<BreadcrumbsViewModel> GetBaseBreadcrumbs()
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

        public virtual async Task<IActionResult> Create()
        {
            // Di sini false, karena kita tambah level "Tambah" yang true
            var breadcrumbs = GetBaseBreadcrumbs();
            breadcrumbs.Add(new BreadcrumbsViewModel($"Tambah {ControllerName}", true, ControllerName, "Create"));
            SetBreadcrumbs([.. breadcrumbs]);

            // AMBIL DATA DARI SERVICE
            var extraData = await _service.CreateData();

            // Pindahkan isi dictionary ke ViewData agar bisa diakses langsung di View
            foreach (var item in extraData)
            {
                ViewData[item.Key] = item.Value;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(TModel tmodel)
        {
            var RawFormData = Request.Form;
            try
            {
                if (ModelState.IsValid)
                {
                    // var choosePermissions = Request.Form["choosePermissions[]"].ToList();
                    // Console.WriteLine("Choose Permissions: " + string.Join(", ", choosePermissions));
                    await _service.CreateAsync(tmodel, RawFormData);
                    TempData["Notify.Type"] = "success";
                    TempData["Notify.Message"] = _localizer["PesanTambahSukses"].Value;
                    return RedirectToAction(nameof(Index));
                }
                return View(tmodel);
            }
            catch (Exception ex)
            {
                TempData["Notify.Type"] = "error";
                TempData["Notify.Message"] = ex.Message;
                // return BadRequest(ex.Message);
                // return View(user);
                // return RedirectToAction(nameof(Edit), user);
                return RedirectToAction(nameof(Index));
            }
        }

        public virtual async Task<IActionResult> Edit(TKey Id)
        {
            var breadcrumbs = GetBaseBreadcrumbs();
            breadcrumbs.Add(new BreadcrumbsViewModel($"Edit {ControllerName}", true, ControllerName, "Edit"));
            SetBreadcrumbs([.. breadcrumbs]);

            var DataModel = await _service.GetByIdAsync(Id);
            if (DataModel == null) return NotFound();

            // AMBIL DATA DARI SERVICE
            var extraData = await _service.EditData(Id);

            // Pindahkan isi dictionary ke ViewData agar bisa diakses langsung di View
            foreach (var item in extraData)
            {
                ViewData[item.Key] = item.Value;
            }

            return View(DataModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Edit(TKey id, TModel tmodel)
        {
            var RawFormData = Request.Form;
            var modelId = (TKey)((dynamic)tmodel).Id;
            if (id == null || !id.Equals(modelId))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(tmodel, RawFormData);

                TempData["Notify.Type"] = "success";
                TempData["Notify.Message"] = _localizer["PesanUbahSukses"].Value;
                return RedirectToAction(nameof(Index));
            }
            return View(tmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Delete(TKey Id)
        {
            try
            {
                await _service.DeleteAsync(Id);

                TempData["Notify.Type"] = "info";
                TempData["Notify.Message"] = _localizer["PesanHapusSukses"].Value;
            }
            catch (Exception ex)
            {
                TempData["Notify.Type"] = "error";
                TempData["Notify.Message"] = ex.Message;

            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public virtual async Task<IActionResult> MultiDelete(string datahapus)
        {
            // Console.WriteLine("Di sinix : " + datahapus);
            try
            {

                var deletedCount = await _service.DeleteMultisAsync(datahapus);

                if (deletedCount > 0)
                {
                    TempData["Notify.Type"] = "success";
                    TempData["Notify.Message"] = $"{deletedCount} Data {_localizer["PesanHapusSukses"].Value}";
                }
                else
                {
                    TempData["Notify.Type"] = "warning";
                    TempData["Notify.Message"] = _localizer["PesanHapusBatal"].Value;
                }
            }
            catch (Exception ex)
            {
                TempData["Notify.Type"] = "error";
                TempData["Notify.Message"] = ex.Message;
                // return BadRequest(ex.Message);
            }

            return RedirectToAction(nameof(Index));
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