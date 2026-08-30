using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Validators.Common;
using FluentValidation;

namespace API_PI_Clubes.Application.Validators
{
    public class CreatReserveDTOValidator : AbstractValidator<CreatReserveDTO>
    {
        public CreatReserveDTOValidator()
        {
            RuleFor(x => x.ScheduleId).NotEmpty();
            RuleFor(x => x.PlayerId).NotEmpty();

            RuleFor(x => x.Date)
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("Não é possível reservar em uma data passada.");
        }
    }

    public class UpdateReserveDTOValidator : AbstractValidator<UpdateReserveDTO>
    {
        public UpdateReserveDTOValidator()
        {
            RuleFor(x => x.Status).IsInEnum();
            RuleFor(x => x.Date).NotEmpty();
        }
    }

    public class ReserveQueryDTOValidator : AbstractValidator<ReserveQueryDTO>
    {
        public ReserveQueryDTOValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(ValidationConstants.NameMaxLength)
                .When(x => x.Name is not null);

            RuleFor(x => x.Status)
                .IsInEnum()
                .When(x => x.Status.HasValue);

            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(ValidationConstants.MinPageSize);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(ValidationConstants.MinPageSize, ValidationConstants.MaxPageSize);
        }
    }
}