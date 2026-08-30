using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Validators.Common;
using FluentValidation;

namespace API_PI_Clubes.Application.Validators
{
    public class CreatCourtDTOValidator : AbstractValidator<CreatCourtDTO>
    {
        public CreatCourtDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome da quadra é obrigatório.")
                .Length(ValidationConstants.NameMinLength, ValidationConstants.NameMaxLength);

            RuleFor(x => x.Type).IsInEnum();
            RuleFor(x => x.Surface).IsInEnum();

            RuleFor(x => x.PricePerHour)
                .InclusiveBetween(ValidationConstants.MinPrice, ValidationConstants.MaxPricePerHour)
                .WithMessage(
                    $"O preço por hora deve estar entre {ValidationConstants.MinPrice} e {ValidationConstants.MaxPricePerHour}.");

            RuleFor(x => x.Description)
                .MaximumLength(ValidationConstants.DescriptionMaxLength)
                .When(x => x.Description is not null);

            RuleFor(x => x.ClubId)
                .NotEmpty().WithMessage("O clube é obrigatório.");

            RuleFor(x => x.Images)
                .ValidImageList();
        }
    }

    public class UpdateCourtDTOValidator : AbstractValidator<UpdateCourtDTO>
    {
        public UpdateCourtDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(ValidationConstants.NameMinLength, ValidationConstants.NameMaxLength);

            RuleFor(x => x.Type).IsInEnum();
            RuleFor(x => x.Surface).IsInEnum();

            RuleFor(x => x.PricePerHour)
                .InclusiveBetween(ValidationConstants.MinPrice, ValidationConstants.MaxPricePerHour);

            RuleFor(x => x.Description)
                .MaximumLength(ValidationConstants.DescriptionMaxLength)
                .When(x => x.Description is not null);
        }
    }

    public class CourtQueryDTOValidator : AbstractValidator<CourtQueryDTO>
    {
        public CourtQueryDTOValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(ValidationConstants.NameMaxLength)
                .When(x => x.Name is not null);

            RuleFor(x => x.City)
                .MaximumLength(ValidationConstants.AddressFieldMaxLength)
                .When(x => x.City is not null);

            RuleForEach(x => x.Types)
                .IsInEnum()
                .When(x => x.Types is not null);

            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(ValidationConstants.MinPageSize);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(ValidationConstants.MinPageSize, ValidationConstants.MaxPageSize);
        }
    }
}