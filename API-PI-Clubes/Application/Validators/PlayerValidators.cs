using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Validators.Common;
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
    public class SetProfileNameDTOValidator : AbstractValidator<SetProfileNameDTO>
    {
        public SetProfileNameDTOValidator()
        {
            RuleFor(x => x.ProfileName)
                .NotEmpty().WithMessage("O nome de perfil é obrigatório.")
                .Length(ValidationConstants.ProfileNameMinLength, ValidationConstants.ProfileNameMaxLength)
                .WithMessage($"O nome de perfil deve ter entre {ValidationConstants.ProfileNameMinLength} e {ValidationConstants.ProfileNameMaxLength} caracteres.")
                .Matches(ValidationConstants.ProfileNameRegex)
                .WithMessage("O nome de perfil deve conter apenas letras e números.");
        }
    }
    public class AddFavoriteSportsDTOValidator : AbstractValidator<AddFavoriteSportsDTO>
    {
        public AddFavoriteSportsDTOValidator()
        {
            RuleFor(x => x.SportIds)
                .NotNull().NotEmpty().WithMessage("Informe ao menos um esporte.");
        }
    }
}