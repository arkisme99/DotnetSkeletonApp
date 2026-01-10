using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace DotnetSkeletonApp.Validator
{
    public static class ValidationExtension
    {
        private static readonly string[] sourceArray = [".jpg", ".jpeg", ".png"];
        private static readonly string[] sourceArray0 = [".pdf", ".docx"];

        // Rule untuk Gambar
        public static IRuleBuilderOptions<T, IFormFile?> IsValidImage<T>(this IRuleBuilder<T, IFormFile?> ruleBuilder)
        {
            return ruleBuilder
                .Must(file => file == null || file.Length <= 2 * 1024 * 1024)
                // .WithMessage("Maksimal ukuran file adalah 2MB")
                .Must(file => file == null || sourceArray.Contains(Path.GetExtension(file.FileName).ToLower()));
            // .WithMessage("Format file harus .jpg, .jpeg, atau .png");
        }

        // Rule untuk Dokumen (PDF/Docx)
        public static IRuleBuilderOptions<T, IFormFile?> IsValidDocument<T>(this IRuleBuilder<T, IFormFile?> ruleBuilder)
        {
            return ruleBuilder
                .Must(file => file == null || file.Length <= 5 * 1024 * 1024)
                // .WithMessage("Maksimal ukuran dokumen adalah 5MB")
                .Must(file => file == null || sourceArray0.Contains(Path.GetExtension(file.FileName).ToLower()));
            // .WithMessage("Format file harus .pdf atau .docx");
        }
    }
}