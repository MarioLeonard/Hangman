using System;
using System.IO;
using System.Linq;

namespace Hangman_Game.Services;

public class UserManagementService : IUserManagementService
{
    private readonly IUserRepository _userRepository;
    private readonly ISaveService _saveService;
    private readonly IStatisticsService _statisticsService;
    private readonly IImageService _imageService;

    public UserManagementService(
        IUserRepository userRepository,
        ISaveService saveService,
        IStatisticsService statisticsService,
        IImageService imageService)
    {
        _userRepository = userRepository;
        _saveService = saveService;
        _statisticsService = statisticsService;
        _imageService = imageService;
    }

    public void DeleteUserAndAssociatedData(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return;

        var users = _userRepository.LoadUsers();
        var targetUser = users.FirstOrDefault(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
        
        if (targetUser == null) return;

        bool hasError = false;

        // 1. Delete all saves
        try
        {
            _saveService.DeleteAllSaves(username);
        }
        catch (Exception)
        {
            hasError = true;
            // Log or ignore failure, continue deletion process
        }

        // 2. Delete statistics
        try
        {
            _statisticsService.DeleteUserStatistics(username);
        }
        catch (Exception)
        {
            hasError = true;
            // Log or ignore failure, continue deletion process
        }

        // 3. Remove image reference (Optional cleanup if it's a custom uploaded image specific to this user)
        // Note: Predefined assets should ideally stay. If a user uploads a new custom file, it can be deleted.
        try
        {
            if (!string.IsNullOrEmpty(targetUser.ImagePath))
            {
                var predefinedImages = _imageService.GetPredefinedImages();
                if (!predefinedImages.Contains(targetUser.ImagePath))
                {
                    string absoluteImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, targetUser.ImagePath);
                    if (File.Exists(absoluteImagePath))
                    {
                        File.Delete(absoluteImagePath);
                    }
                }
            }
        }
        catch (Exception)
        {
            hasError = true;
            // Ignore if image is locked/in-use or already removed.
        }

        // 4. Finally, remove user from repository and save changes.
        try 
        {
            _userRepository.DeleteUser(targetUser);
        }
        catch (Exception)
        {
            // Even if something failed, we try to remove the user.
        }
    }
}
