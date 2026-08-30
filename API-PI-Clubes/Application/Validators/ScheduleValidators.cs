using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Validators.Common;
using FluentValidation;

namespace API_PI_Clubes.Application.Validators
{
    public class CreatScheduleDTOValidator : AbstractValidator<CreatScheduleDTO>
    {
        public CreatScheduleDTOValidator()
        {
            RuleFor(x => x.CourtId).NotEmpty();
            RuleFor(x => x.DayOfWeek).IsInEnum();

            RuleFor(x => x.EndTime)
                .GreaterThan(x => x.StartTime)
                .WithMessage("O horário de término deve ser depois do horário de início.");
        }
    }

    public class UpdateScheduleDTOValidator : AbstractValidator<UpdateScheduleDTO>
    {
        public UpdateScheduleDTOValidator()
        {
            RuleFor(x => x.DayOfWeek).IsInEnum();
            RuleFor(x => x.State).IsInEnum();

            RuleFor(x => x.EndTime)
                .GreaterThan(x => x.StartTime)
                .WithMessage("O horário de término deve ser depois do horário de início.");
        }
    }

    public class CreateBulkScheduleDTOValidator : AbstractValidator<CreateBulkScheduleDTO>
    {
        public CreateBulkScheduleDTOValidator()
        {
            RuleFor(x => x.CourtId).NotEmpty();

            RuleFor(x => x.DaysOfWeek)
                .NotEmpty().WithMessage("Selecione ao menos um dia da semana.")
                .Must(days => days.Distinct().Count() == days.Count)
                .WithMessage("Dias da semana duplicados.");

            RuleForEach(x => x.DaysOfWeek).IsInEnum();

            RuleFor(x => x.EndTime)
                .GreaterThan(x => x.StartTime)
                .WithMessage("O horário de término deve ser depois do horário de início.");

            RuleFor(x => x.SlotDurationMinutes)
                .InclusiveBetween(ValidationConstants.MinSlotDurationMinutes,
                    ValidationConstants.MaxSlotDurationMinutes)
                .WithMessage(
                    $"A duração do slot deve estar entre {ValidationConstants.MinSlotDurationMinutes} e {ValidationConstants.MaxSlotDurationMinutes} minutos.");
        }
    }
}