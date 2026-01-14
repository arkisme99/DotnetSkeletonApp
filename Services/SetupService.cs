using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Data;
using DotnetSkeletonApp.Models;

namespace DotnetSkeletonApp.Services
{
    public class SetupService(
        ApplicationDbContext context
    ) : BaseCrudService<Setup, Guid, Setup>(
        context
    )
    {
        protected override async Task<Setup> BeforeUpdateAsync(Setup model, Setup tviewmodel)
        {
            Console.WriteLine($"Masuk Upload {model.LogoForm} , - {model.LogoApp} , - {model.NameApp} , - {model.Phone}");

            var fileName = await ProcessUpload(model.LogoForm!, "apps");

            if (fileName != null)
            {
                //delete photo lama
                ProcessDelete(model.LogoApp!, "apps");

                //ubah ke baru
                model.LogoApp = fileName;
            }

            Console.WriteLine($"Proses Upload {model.LogoApp}");

            return model;
        }
    }
}