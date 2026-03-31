using System.Collections.Generic;
using Hangman_Game.Models;

namespace Hangman_Game.Services;

public interface IStatisticsService
{
    List<UserStatistics> GetAllStatistics();
    UserStatistics GetUserStatistics(string username);
    void UpdateAfterGame(string username, string category, bool isWin);
    void DeleteUserStatistics(string username);
}
