using System.IO;
using Microsoft.Win32;

namespace Hangman_Game.Services;

public class ImageService : IImageService
{
    private readonly string _assetsDirectory;
    private readonly string _avatarsDirectory;

    public ImageService()
    {
        _assetsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
        if (!Directory.Exists(_assetsDirectory))
        {
            Directory.CreateDirectory(_assetsDirectory);
        }

        _avatarsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Avatars");
        if (!Directory.Exists(_avatarsDirectory))
        {
            Directory.CreateDirectory(_avatarsDirectory);
        }
    }

    public string? SelectImageFromFileDialog()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Image Files|*.jpg;*.jpeg;*.gif",
            Title = "Select Profile Image"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            return CopyToAssets(openFileDialog.FileName);
        }

        return null;
    }

    public List<string> GetPredefinedImages()
    {
        if (!Directory.Exists(_avatarsDirectory)) return new List<string>();

        var files = Directory.GetFiles(_avatarsDirectory)
                             .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                         f.EndsWith(".gif", StringComparison.OrdinalIgnoreCase));

        return files.Select(f => $"Assets/Avatars/{Path.GetFileName(f)}").ToList();
    }

    private string CopyToAssets(string sourceFilePath)
    {
        var fileName = Path.GetFileName(sourceFilePath);
        var targetFilePath = Path.Combine(_assetsDirectory, fileName);
        var relativePath = $"Assets/{fileName}";

        if (!File.Exists(targetFilePath))
        {
            File.Copy(sourceFilePath, targetFilePath);
        }

        return relativePath;
    }
}
