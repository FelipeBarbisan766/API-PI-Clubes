using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Validators.Common;
using FluentValidation;

namespace API_PI_Clubes.Application.Validators
{
    public class CreateClubDTOValidator : AbstractValidator<CreateClubDTO>
    {
        public CreateClubDTOValidator()
        {
            RuleFor(x => x.adminId)
                .NotEmpty().WithMessage("O administrador é obrigatório.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome do clube é obrigatório.")
                .Length(ValidationConstants.NameMinLength, ValidationConstants.NameMaxLength);

            RuleFor(x => x.PhoneNumber)
                .IsValidBrazilianPhone()
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.Description)
                .MaximumLength(ValidationConstants.DescriptionMaxLength)
                .When(x => x.Description is not null);

            RuleFor(x => x.ZipCode)
                .NotEmpty().WithMessage("O CEP é obrigatório.")
                .IsValidCep();

            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("A rua é obrigatória.")
                .MaximumLength(ValidationConstants.AddressFieldMaxLength);

            RuleFor(x => x.Number)
                .MaximumLength(10)
                .When(x => x.Number is not null);

            RuleFor(x => x.Neighborhood)
                .NotEmpty().WithMessage("O bairro é obrigatório.")
                .MaximumLength(ValidationConstants.AddressFieldMaxLength);

            RuleFor(x => x.Complement)
                .MaximumLength(ValidationConstants.ComplementMaxLength)
                .When(x => x.Complement is not null);

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("A cidade é obrigatória.")
                .MaximumLength(ValidationConstants.AddressFieldMaxLength);

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("O estado é obrigatório.")
                .IsValidUF();

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("O país é obrigatório.")
                .MaximumLength(ValidationConstants.AddressFieldMaxLength);

            RuleFor(x => x.Images)
                .ValidImageList();
        }
    }

    public class UpdateClubDTOValidator : AbstractValidator<UpdateClubDTO>
    {
        public UpdateClubDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome do clube é obrigatório.")
                .Length(ValidationConstants.NameMinLength, ValidationConstants.NameMaxLength);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("O telefone é obrigatório.")
                .IsValidBrazilianPhone();

            RuleFor(x => x.Description)
                .MaximumLength(ValidationConstants.DescriptionMaxLength)
                .When(x => x.Description is not null);

            RuleFor(x => x.ZipCode)
                .NotEmpty().WithMessage("O CEP é obrigatório.")
                .IsValidCep();

            RuleFor(x => x.Street)
                .NotEmpty()
                .MaximumLength(ValidationConstants.AddressFieldMaxLength);

            RuleFor(x => x.Number)
                .NotEmpty()
                .MaximumLength(10);

            RuleFor(x => x.Neighborhood)
                .NotEmpty()
                .MaximumLength(ValidationConstants.AddressFieldMaxLength);

            RuleFor(x => x.Complement)
                .MaximumLength(ValidationConstants.ComplementMaxLength)
                .When(x => x.Complement is not null);

            RuleFor(x => x.City)
                .NotEmpty()
                .MaximumLength(ValidationConstants.AddressFieldMaxLength);

            RuleFor(x => x.State)
                .NotEmpty()
                .IsValidUF();

            RuleFor(x => x.Country)
                .NotEmpty()
                .MaximumLength(ValidationConstants.AddressFieldMaxLength);
        }
    }

    public class ClubQueryDTOValidator : AbstractValidator<ClubQueryDTO>
    {
        public ClubQueryDTOValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(ValidationConstants.NameMaxLength)
                .When(x => x.Name is not null);

            RuleFor(x => x.City)
                .MaximumLength(ValidationConstants.AddressFieldMaxLength)
                .When(x => x.City is not null);

            RuleForEach(x => x.SportIds)
                .NotEmpty()
                .When(x => x.SportIds is not null);

            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(ValidationConstants.MinPageSize);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(ValidationConstants.MinPageSize, ValidationConstants.MaxPageSize)
                .WithMessage(
                    $"PageSize deve estar entre {ValidationConstants.MinPageSize} e {ValidationConstants.MaxPageSize}.");
        }
    }
}