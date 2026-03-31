using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hangman_Game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, Services.INavigationService
    {
        public MainWindow()
        {
            InitializeComponent();
            var userRepository = new Services.UserRepository();
            var imageService = new Services.ImageService(); 
            var dialogService = new Services.DialogService(); 
            var saveService = new Services.SaveService();
            var statisticsService = new Services.StatisticsService();
            var userManagementService = new Services.UserManagementService(userRepository, saveService, statisticsService, imageService);
            DataContext = new ViewModels.StartViewModel(userRepository, this, imageService, dialogService, userManagementService);
            Content = new Views.StartView();
        }

        public void NavigateToStart()
        {
            var userRepository = new Services.UserRepository();
            var imageService = new Services.ImageService();
            var dialogService = new Services.DialogService(); 
            var saveService = new Services.SaveService();
            var statisticsService = new Services.StatisticsService();
            var userManagementService = new Services.UserManagementService(userRepository, saveService, statisticsService, imageService);
            var vm = new ViewModels.StartViewModel(userRepository, this, imageService, dialogService, userManagementService);
            DataContext = vm;
            Content = new Views.StartView { DataContext = vm };
        }

        public void NavigateToGame(Models.User currentUser)
        {
            var wordRepository = new Services.WordRepository();
            var saveService = new Services.SaveService();
            var statisticsService = new Services.StatisticsService();
            var dialogService = new Services.DialogService();
            var vm = new ViewModels.GameViewModel(currentUser, this, wordRepository, saveService, statisticsService, dialogService);
            DataContext = vm;
            Content = new Views.GameView { DataContext = vm };
        }

        public void NavigateToStatistics(Models.User currentUser = null)
        {
            var statisticsService = new Services.StatisticsService();
            var vm = new ViewModels.StatisticsViewModel(statisticsService, this, currentUser);
            DataContext = vm;
            Content = new Views.StatisticsView(vm) { DataContext = vm };
        }
    }
}