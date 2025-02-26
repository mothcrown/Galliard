namespace Galliard.Application.Novelize.Commands.Novelize;

public class NovelizeCommandValidator : AbstractValidator<NovelizeCommand>
{
    public NovelizeCommandValidator()
    {
        RuleFor(v => v.FileName)
            .NotNull()
            .NotEmpty()
            .Must(v => v!.Contains(".mp3")).WithMessage("Invalid file format");
        RuleFor(v => v.Contents)
            .NotNull()
            .NotEmpty();
    }
}
