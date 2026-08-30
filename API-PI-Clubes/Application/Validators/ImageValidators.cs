using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Validators.Common;
using FluentValidation;

namespace API_PI_Clubes.Application.Validators
{
    public class UploadImageDTOValidator : AbstractValidator<UploadImageDTO>
    {
        public UploadImageDTOValidator()
        {
            RuleFor(x => x.Images)
                .NotEmpty().WithMessage("Envie pelo menos uma imagem.")
                .ValidImageList();
        }
    }

    public class DeleteImageDtoValidator : AbstractValidator<DeleteImageDto>
    {
        public DeleteImageDtoValidator()
        {
            RuleFor(x => x.ImageIds)
                .NotEmpty().WithMessage("Informe ao menos um Id de imagem para excluir.")
                .Must(ids => ids.Count <= ValidationConstants.MaxImagesPerUpload)
                .WithMessage(
                    $"Não é possível excluir mais de {ValidationConstants.MaxImagesPerUpload} imagens por vez.");

            RuleForEach(x => x.ImageIds).NotEmpty();
        }
    }

    public class ReorderImageDTOValidator : AbstractValidator<ReorderImageDTO>
    {
        public ReorderImageDTOValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
        }
    }

    public class ReorderImagesRequestDTOValidator : AbstractValidator<ReorderImagesRequestDTO>
    {
        public ReorderImagesRequestDTOValidator()
        {
            RuleFor(x => x.Orders)
                .NotEmpty().WithMessage("Informe a nova ordenação das imagens.")
                .Must(orders => orders.Count <= ValidationConstants.MaxImagesPerUpload)
                .WithMessage(
                    $"Não é possível reordenar mais de {ValidationConstants.MaxImagesPerUpload} imagens de uma vez.")
                .Must(orders => orders.Select(o => o.Id).Distinct().Count() == orders.Count)
                .WithMessage("Ids de imagem duplicados na requisição de reordenação.");

            RuleForEach(x => x.Orders).SetValidator(new ReorderImageDTOValidator());
        }
    }
}