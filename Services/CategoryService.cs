using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hangman_Game.Services
{
    public interface ICategoryService
    {
        Task InitializeAsync();
        IEnumerable<string> GetCategories();
        IEnumerable<string> GetWords(string category);
        string GetRandomWord(string category);
        bool IsInitialized { get; }
    }

    public class CategoryService : ICategoryService
    {
        private Dictionary<string, List<string>> _categoriesData = new(StringComparer.OrdinalIgnoreCase);
        private const string JsonFilePath = "Data/Categories/categories.json";
        public const string AllCategoriesKey = "All Categories";
        private readonly Random _random = new();

        public bool IsInitialized { get; private set; }

        public async Task InitializeAsync()
        {
            if (IsInitialized) return;

            try
            {
                if (!File.Exists(JsonFilePath))
                {
                    // Handle missing file gracefully by just logging/keeping empty dict.
                    return;
                }

                string jsonContent = await File.ReadAllTextAsync(JsonFilePath);
                var data = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data != null)
                {
                    _categoriesData = new Dictionary<string, List<string>>(data, StringComparer.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load categories: {ex.Message}");
            }
            finally
            {
                IsInitialized = true;
            }
        }

        public IEnumerable<string> GetCategories()
        {
            if (!IsInitialized || _categoriesData.Count == 0) return Enumerable.Empty<string>();

            var categories = _categoriesData.Keys.ToList();
            categories.Insert(0, AllCategoriesKey);
            return categories;
        }

        public IEnumerable<string> GetWords(string category)
        {
            if (!IsInitialized) return Enumerable.Empty<string>();

            if (string.Equals(category, AllCategoriesKey, StringComparison.OrdinalIgnoreCase))
            {
                return _categoriesData.Values.SelectMany(x => x);
            }

            if (_categoriesData.TryGetValue(category, out var words))
            {
                return words;
            }

            return Enumerable.Empty<string>();
        }

        public string GetRandomWord(string category)
        {
            var words = GetWords(category).ToList();
            if (words.Count == 0) return string.Empty;

            int index = _random.Next(words.Count);
            return words[index];
        }
    }
}
