using System.Collections.Generic;

namespace Hangman_Game.Services;

public interface IWordRepository
{
    Dictionary<string, List<string>> LoadCategories();
}
