using System.IO;
using System.Text.Json;
using Hangman_Game.Models;
using Hangman_Game.Helpers;

namespace Hangman_Game.Services;

public class UserRepository : IUserRepository
{
    private readonly string _filePath;
    private static readonly object _fileLock = new object();

    public UserRepository()
    {
        var dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        _filePath = Path.Combine(dataDirectory, "users.json");
    }

    public List<User> LoadUsers()
    {
        lock (_fileLock)
        {
            return JsonFileHelper.Load<List<User>>(_filePath) ?? new List<User>();
        }
    }

    public void SaveUsers(List<User> users)
    {
        lock (_fileLock)
        {
            JsonFileHelper.Save(_filePath, users);
        }
    }

    public void AddUser(User user)
    {
        var users = LoadUsers();
        users.Add(user);
        SaveUsers(users);
    }

    public void DeleteUser(User user)
    {
        var users = LoadUsers();
        var userToRemove = users.FirstOrDefault(u => u.Username == user.Username);
        if (userToRemove != null)
        {
            users.Remove(userToRemove);
            SaveUsers(users);
        }
    }
}
