using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DotnetSkeletonApp.Helpers;
using DotnetSkeletonApp.Models.ViewModels;
using DotnetSkeletonApp.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DotnetSkeletonApp.Controllers
{
    public abstract class BaseCrudController<TModel, TService, TKey, TViewModel>(
        TService service
    ) : BaseController
        where TModel : class
        where TViewModel : class
        where TService : BaseCrudService<TModel, TKey, TViewModel>
    {
        protected readonly TService _service = service;
        protected readonly TViewModel? _tviewmodel;
        private IValidator<TViewModel>? _validator;
        protected IValidator<TViewModel> Validator =>
            _validator ??= HttpContext.RequestServices.GetRequiredService<IValidator<TViewModel>>();
        protected IStringLocalizer<SharedResource>? _localizer;
        protected IStringLocalizer<SharedResource> Localizer =>
        _localizer ??= HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
        protected virtual string ControllerName => Request.RouteValues["controller"]?.ToString() ?? "";
        // protected virtual string ModelName => typeof(TModel).Name;

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

            // _logger.LogInformation($"Masuk Ke Index {_service!.GetType().Name}");
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
        public virtual async Task<IActionResult> Create(TModel tmodel, TViewModel viewModel)
        {
            // var choosePermissions = Request.Form["choosePermissions[]"].ToList();
            // Console.WriteLine("Choose ViewModel C: " + string.Join(", ", viewModel));

            var validationResult = await Validator.ValidateAsync(viewModel, options => options.IncludeRuleSets("Create", "default"));
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(x => x.ErrorMessage);

                TempData["Notify.Type"] = "error";
                TempData["Notify.Message"] = string.Join(", ", errors);
                // return View("Create");
                return RedirectToAction(nameof(Create));
            }

            // var RawFormData = Request.Form;
            try
            {

                await _service.CreateAsync(tmodel, viewModel);
                TempData["Notify.Type"] = "success";
                TempData["Notify.Message"] = Localizer["PesanTambahSukses"].Value;
                return RedirectToAction(nameof(Index));

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
        public virtual async Task<IActionResult> Edit(TKey id, TModel tmodel, TViewModel viewmodel)
        {

            var validationResult = await Validator.ValidateAsync(viewmodel, options => options.IncludeRuleSets("Update", "default"));
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(x => x.ErrorMessage);

                TempData["Notify.Type"] = "error";
                TempData["Notify.Message"] = string.Join(", ", errors);
                return RedirectToAction(nameof(Edit), id);
            }

            // var RawFormData = Request.Form;
            var modelId = (TKey)((dynamic)tmodel).Id;
            if (id == null || !id.Equals(modelId))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(tmodel, viewmodel);

                TempData["Notify.Type"] = "success";
                TempData["Notify.Message"] = Localizer["PesanUbahSukses"].Value;
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
                TempData["Notify.Message"] = Localizer["PesanHapusSukses"].Value;
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
                    TempData["Notify.Message"] = $"{deletedCount} Data {Localizer["PesanHapusSukses"].Value}";
                }
                else
                {
                    TempData["Notify.Type"] = "warning";
                    TempData["Notify.Message"] = Localizer["PesanHapusBatal"].Value;
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