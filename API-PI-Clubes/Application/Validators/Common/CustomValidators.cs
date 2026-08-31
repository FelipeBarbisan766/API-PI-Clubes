using System.Text.RegularExpressions;
using FluentValidation;

namespace API_PI_Clubes.Application.Validators.Common
{
    public static class CustomValidators
    {
        public static IRuleBuilderOptions<T, string> IsValidCpf<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Must(CpfIsValid)
                .WithMessage("CPF inválido.");
        }

        public static IRuleBuilderOptions<T, string> IsValidCep<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Matches(@"^\d{5}-?\d{3}$")
                .WithMessage("CEP inválido. Formato esperado: 00000-000.");
        }

        public static IRuleBuilderOptions<T, string> IsValidBrazilianPhone<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Matches(@"^\d{10,11}$")
                .WithMessage("Telefone inválido. Use apenas números com DDD (ex: 11999999999 ou 1188888888).");
        }

        public static IRuleBuilderOptions<T, string> IsValidUF<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Must(uf => !string.IsNullOrWhiteSpace(uf) &&
                            ValidationConstants.ValidUFs.Contains(uf.ToUpperInvariant()))
                .WithMessage("Estado (UF) inválido.");
        }


        public static IRuleBuilderOptions<T, List<IFormFile>?> ValidImageList<T>(
            this IRuleBuilder<T, List<IFormFile>?> ruleBuilder)
        {
            return ruleBuilder
                .Must(images =>
                {
                    if (images is null || images.Count == 0) return true;
                    if (images.Count > ValidationConstants.MaxImagesPerUpload) return false;

                    return images.All(img =>
                        img.Length > 0 &&
                        img.Length <= ValidationConstants.MaxImageSizeBytes &&
                        ValidationConstants.AllowedImageContentTypes.Contains(img.ContentType?.ToLowerInvariant()));
                })
                .WithMessage(
                    $"Cada imagem deve ter entre 1 byte e {ValidationConstants.MaxImageSizeBytes / 1024 / 1024}MB, " +
                    $"formato JPEG/PNG/WEBP, com no máximo {ValidationConstants.MaxImagesPerUpload} imagens por envio.");
        }

        private static bool CpfIsValid(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return false;

            cpf = Regex.Replace(cpf, "[^0-9]", "");
            if (cpf.Length != 11) return false;

            if (new string(cpf[0], 11) == cpf) return false;

            int[] multiplier1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplier2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            var tempCpf = cpf.Substring(0, 9);
            var sum = 0;
            for (var i = 0; i < 9; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multiplier1[i];

            var remainder = sum % 11;
            remainder = remainder < 2 ? 0 : 11 - remainder;

            var digit = remainder.ToString();
            tempCpf += digit;
            sum = 0;
            for (var i = 0; i < 10; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multiplier2[i];

            remainder = sum % 11;
            remainder = remainder < 2 ? 0 : 11 - remainder;
            digit += remainder;

            return cpf.EndsWith(digit);
        }
    }
}