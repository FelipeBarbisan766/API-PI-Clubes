using API_PI_Clubes.Application.DTOs;
using FluentValidation;

namespace API_PI_Clubes.Application.Validators
{
    public class UpdatePlayerDTOValidator : AbstractValidator<UpdatePlayerDTO>
    {
        public UpdatePlayerDTOValidator()
        {
            RuleForEach(x => x.FavoriteSportIds)
                .NotEmpty()
                .When(x => x.FavoriteSportIds is not null);
        }
    }
}