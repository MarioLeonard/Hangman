namespace Hangman_Game.Services;

public interface INavigationService
{
    void NavigateToStart();
    void NavigateToGame(Models.User currentUser);
    void NavigateToStatistics(Models.User currentUser = null);
}
