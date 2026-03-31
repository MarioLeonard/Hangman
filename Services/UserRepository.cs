using System.IO;
using System.Text.Json;
using Hangman_Game.Models;

namespace Hangman_Game.Services;

public class UserRepository : IUserRepository
{
    private readonly string _dataDirectory;
    private readonly string _filePath;
    private static readonly object _fileLock = new object();

    public UserRepository()
    {
        _dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        _filePath = Path.Combine(_dataDirectory, "users.json");
    }

    public List<User> LoadUsers()
    {
        lock (_fileLock)
        {
            try
            {
                if (!Directory.Exists(_dataDirectory))
                {
                    Directory.CreateDirectory(_dataDirectory);
                }

                if (!File.Exists(_filePath))
                {
                    return new List<User>();
                }

                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
            }
            catch (Exception)
            {
                // Handle or log exception
                return new List<User>();
            }
        }
    }

    public void SaveUsers(List<User> users)
    {
        lock (_fileLock)
        {
            try
            {
                if (!Directory.Exists(_dataDirectory))
                {
                    Directory.CreateDirectory(_dataDirectory);
                }

                var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
            catch (Exception)
            {
                // Handle or log exception
            }
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
