using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Validators.Common;
using FluentValidation;

namespace API_PI_Clubes.Application.Validators
{
    public class CreatUserDTOValidator : AbstractValidator<CreatUserDTO>
    {
        public CreatUserDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .Length(ValidationConstants.NameMinLength, ValidationConstants.NameMaxLength);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("E-mail inválido.")
                .MaximumLength(256);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("A senha é obrigatória.")
                .MinimumLength(8).WithMessage("A senha deve ter ao menos 8 caracteres.")
                .MaximumLength(100)
                .Matches("[A-Z]").WithMessage("A senha deve conter ao menos uma letra maiúscula.")
                .Matches("[a-z]").WithMessage("A senha deve conter ao menos uma letra minúscula.")
                .Matches("[0-9]").WithMessage("A senha deve conter ao menos um número.");
        }
    }

    // Etapa 2 do fluxo LGPD (PATCH /api/Auth/complete-profile)
    public class CompleteProfileDTOValidator : AbstractValidator<CompleteProfileDTO>
    {
        public CompleteProfileDTOValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("O telefone é obrigatório.")
                .IsValidBrazilianPhone();

            RuleFor(x => x.Cpf)
                .NotEmpty().WithMessage("O CPF é obrigatório.")
                .IsValidCpf();

            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage("A data de nascimento é obrigatória.")
                .Must(BeAtLeast16YearsOld).WithMessage("É necessário ter ao menos 16 anos.")
                .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-120)))
                .WithMessage("Data de nascimento inválida.");
        }

        private static bool BeAtLeast16YearsOld(DateOnly birthDate)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (birthDate > today) return false;

            var age = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-age)) age--;
            return age >= 16;
        }
    }

    public class UpdateUserDTOValidator : AbstractValidator<UpdateUserDTO>
    {
        public UpdateUserDTOValidator()
        {
            RuleFor(x => x.Name)
                .Length(ValidationConstants.NameMinLength, ValidationConstants.NameMaxLength)
                .When(x => x.Name is not null);

            RuleFor(x => x.PhoneNumber)
                .IsValidBrazilianPhone()
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
        }
    }

    public class UpdateAvatarDTOValidator : AbstractValidator<UpdateAvatarDTO>
    {
        public UpdateAvatarDTOValidator()
        {
            RuleFor(x => x.AvatarImage)
                .NotNull().WithMessage("Envie uma imagem de avatar.")
                .Must(img => img is null || (img.Length > 0 && img.Length <= ValidationConstants.MaxImageSizeBytes))
                .WithMessage($"A imagem deve ter no máximo {ValidationConstants.MaxImageSizeBytes / 1024 / 1024}MB.")
                .Must(img =>
                    img is null ||
                    ValidationConstants.AllowedImageContentTypes.Contains(img.ContentType?.ToLowerInvariant()))
                .WithMessage("Formato de imagem inválido. Use JPEG, PNG ou WEBP.");
        }
    }
}