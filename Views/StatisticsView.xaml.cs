using System.Windows.Controls;
using Hangman_Game.ViewModels;

namespace Hangman_Game.Views;

public partial class StatisticsView : UserControl
{
    public StatisticsView(StatisticsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}