using Hangman_Game.Models;

namespace Hangman_Game.Services;

public interface IUserRepository
{
    List<User> LoadUsers();
    void SaveUsers(List<User> users);
    void AddUser(User user);
    void DeleteUser(User user);
}
