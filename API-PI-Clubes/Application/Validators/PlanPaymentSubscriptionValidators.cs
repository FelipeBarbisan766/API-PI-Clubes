using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Validators.Common;
using FluentValidation;

namespace API_PI_Clubes.Application.Validators
{
    public class CreatePlanDtoValidator : AbstractValidator<CreatePlanDto>
    {
        public CreatePlanDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome do plano é obrigatório.")
                .Length(ValidationConstants.NameMinLength, ValidationConstants.NameMaxLength);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(ValidationConstants.DescriptionMaxLength);

            RuleFor(x => x.Price)
                .InclusiveBetween(ValidationConstants.MinPrice, ValidationConstants.MaxPlanPrice);

            RuleFor(x => x.QuantClub)
                .InclusiveBetween(1, 1000).WithMessage("Quantidade de clubes deve ser entre 1 e 1000.");

            RuleFor(x => x.QuantCourt)
                .InclusiveBetween(1, 1000).WithMessage("Quantidade de quadras deve ser entre 1 e 1000.");

            RuleFor(x => x.DurationDays)
                .InclusiveBetween(1, 3650).WithMessage("Duração deve ser entre 1 dia e 10 anos.");
        }
    }

    public class UpdatePlanDtoValidator : AbstractValidator<UpdatePlanDto>
    {
        public UpdatePlanDtoValidator()
        {
            RuleFor(x => x.Name)
                .Length(ValidationConstants.NameMinLength, ValidationConstants.NameMaxLength)
                .When(x => x.Name is not null);

            RuleFor(x => x.Description)
                .MaximumLength(ValidationConstants.DescriptionMaxLength)
                .When(x => x.Description is not null);

            RuleFor(x => x.Price)
                .InclusiveBetween(ValidationConstants.MinPrice, ValidationConstants.MaxPlanPrice)
                .When(x => x.Price.HasValue);

            RuleFor(x => x.QuantClub)
                .InclusiveBetween(1, 1000)
                .When(x => x.QuantClub.HasValue);

            RuleFor(x => x.QuantCourt)
                .InclusiveBetween(1, 1000)
                .When(x => x.QuantCourt.HasValue);

            RuleFor(x => x.DurationDays)
                .InclusiveBetween(1, 3650)
                .When(x => x.DurationDays.HasValue);
        }
    }

    public class CreatePaymentDtoValidator : AbstractValidator<CreatePaymentDto>
    {
        public CreatePaymentDtoValidator()
        {
            RuleFor(x => x.AdminId).NotEmpty();
            RuleFor(x => x.PlanId).NotEmpty();
            RuleFor(x => x.Method).IsInEnum();
        }
    }

    public class CreateSubscriptionDtoValidator : AbstractValidator<CreateSubscriptionDto>
    {
        public CreateSubscriptionDtoValidator()
        {
            RuleFor(x => x.AdminId).NotEmpty();
            RuleFor(x => x.PlanId).NotEmpty();
            RuleFor(x => x.PaymentId).NotEmpty();
        }
    }

    // UpdateAdminDTO e UpdatePlayerDTO não têm propriedades hoje — nada a validar.
    // Se ganharem campos no futuro, crie os validators correspondentes aqui.

    public class CreatAdminDTOValidator : AbstractValidator<CreatAdminDTO>
    {
        public CreatAdminDTOValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }

    public class CreatPlayerDTOValidator : AbstractValidator<CreatPlayerDTO>
    {
        public CreatPlayerDTOValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}