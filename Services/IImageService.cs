namespace Hangman_Game.Services;

public interface IImageService
{
    string? SelectImageFromFileDialog();
    List<string> GetPredefinedImages();
}
