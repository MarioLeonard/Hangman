using System;

namespace Hangman_Game.Services;

public interface IUserManagementService
{
    void DeleteUserAndAssociatedData(string username);
}
