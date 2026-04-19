using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Hangman_Game.Commands;
using Hangman_Game.Models;
using Hangman_Game.Services;

namespace Hangman_Game.ViewModels;

public partial class StartViewModel : ObservableObject
{
    private readonly IUserRepository _userRepository;
    private readonly INavigationService _navigationService;
    private readonly IImageService _imageService;
    private readonly IDialogService _dialogService;
    private readonly IUserManagementService _userManagementService;

    [ObservableProperty]
    private ObservableCollection<User> _users;

    [ObservableProperty]
    private User? _selectedUser;

    public bool HasUsers => Users.Any();

    [ObservableProperty]
    private string _newUsername = string.Empty;

    [ObservableProperty]
    private string? _newSelectedImagePath;

    [ObservableProperty]
    private ObservableCollection<string> _predefinedImages;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public List<string> AvatarPaths { get; private set; } = new();

    [ObservableProperty]
    private int _currentAvatarIndex;

    [ObservableProperty]
    private string? _currentAvatarPath;

    public RelayCommand PlayCommand { get; }
    public RelayCommand CreateUserCommand { get; }
    public RelayCommand DeleteUserCommand { get; }
    public RelayCommand CancelCommand { get; }
    public RelayCommand BrowseImageCommand { get; }
    public RelayCommand ShowStatisticsCommand { get; }
    public RelayCommand NextAvatarCommand { get; }
    public RelayCommand PreviousAvatarCommand { get; }
    public RelayCommand SelectAvatarCommand { get; }

    public StartViewModel(IUserRepository userRepository, INavigationService navigationService, IImageService imageService, IDialogService dialogService, IUserManagementService userManagementService)
    {
        _userRepository = userRepository;
        _navigationService = navigationService;
        _imageService = imageService;
        _dialogService = dialogService;
        _userManagementService = userManagementService;
        
        PlayCommand = new RelayCommand(_ => Play(), _ => CanPlay());
        CreateUserCommand = new RelayCommand(_ => CreateUser(), _ => CanCreateUser());
        DeleteUserCommand = new RelayCommand(_ => DeleteUser(), _ => CanDeleteUser());
        CancelCommand = new RelayCommand(_ => Cancel());
        BrowseImageCommand = new RelayCommand(_ => BrowseImage());
        ShowStatisticsCommand = new RelayCommand(_ => ShowStatistics());
        NextAvatarCommand = new RelayCommand(_ => NextAvatar());
        PreviousAvatarCommand = new RelayCommand(_ => PreviousAvatar());
        SelectAvatarCommand = new RelayCommand(_ => SelectAvatar());

        LoadUsers();
        LoadPredefinedImages();
        
        Users.CollectionChanged += (s, e) => OnPropertyChanged(nameof(HasUsers));
    }

    private void LoadUsers()
    {
        Users = new ObservableCollection<User>(_userRepository.LoadUsers());
    }

    private void LoadPredefinedImages()
    {
        PredefinedImages = new ObservableCollection<string>(_imageService.GetPredefinedImages());
        AvatarPaths = PredefinedImages.ToList();
        
        if (AvatarPaths.Any())
        {
            CurrentAvatarIndex = 0;
            CurrentAvatarPath = AvatarPaths[CurrentAvatarIndex];
        }
    }

    private void NextAvatar()
    {
        if (!AvatarPaths.Any()) return;
        
        CurrentAvatarIndex = (CurrentAvatarIndex + 1) % AvatarPaths.Count;
        CurrentAvatarPath = AvatarPaths[CurrentAvatarIndex];
    }

    private void PreviousAvatar()
    {
        if (!AvatarPaths.Any()) return;
        
        CurrentAvatarIndex = (CurrentAvatarIndex - 1 + AvatarPaths.Count) % AvatarPaths.Count;
        CurrentAvatarPath = AvatarPaths[CurrentAvatarIndex];
    }

    private void SelectAvatar()
    {
        if (CurrentAvatarPath != null)
        {
            NewSelectedImagePath = CurrentAvatarPath;
            _dialogService.ShowMessage("Avatar Selected", "Successfully selected avatar for the new user.");
        }
    }

    private void Play()
    {
        if (SelectedUser != null)
        {
            _navigationService.NavigateToGame(SelectedUser);
        }
    }

    private bool CanPlay() => SelectedUser != null;

    private void BrowseImage()
    {
        var imagePath = _imageService.SelectImageFromFileDialog();
        if (imagePath != null)
        {
            NewSelectedImagePath = imagePath;
        }
    }

    private void CreateUser()
    {
        ErrorMessage = string.Empty;
        var username = NewUsername.Trim();
        var imagePath = NewSelectedImagePath ?? "Assets/default.jpg"; // Fallback image

        var newUser = new User 
        { 
            Username = username,
            ImagePath = imagePath
        };

        _userRepository.AddUser(newUser);
        Users.Add(newUser);
        
        NewUsername = string.Empty;
        NewSelectedImagePath = null;
    }

    private bool CanCreateUser()
    {
        if (string.IsNullOrWhiteSpace(NewUsername))
        {
            ErrorMessage = string.Empty;
            return false;
        }
        
        var username = NewUsername.Trim();
        if (username.Contains(' '))
        {
            ErrorMessage = "Username cannot contain spaces.";
            return false;
        }

        if (Users.Any(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase)))
        {
            ErrorMessage = "Username already exists.";
            return false;
        }
        
        if (string.IsNullOrEmpty(NewSelectedImagePath))
        {
            ErrorMessage = "An avatar must be selected.";
            return false;
        }

        ErrorMessage = string.Empty;
        return true;
    }

    private void DeleteUser()
    {
        if (SelectedUser != null)
        {
            if (_dialogService.ConfirmDelete(SelectedUser.Username))
            {
                _userManagementService.DeleteUserAndAssociatedData(SelectedUser.Username);
                Users.Remove(SelectedUser);
                SelectedUser = null;
            }
        }
    }

    private bool CanDeleteUser() => SelectedUser != null;

    private void Cancel()
    {
        System.Windows.Application.Current.Shutdown();
    }

    private void ShowStatistics()
    {
        _navigationService.NavigateToStatistics();
    }
}
