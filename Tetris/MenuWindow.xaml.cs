using System.Windows;


namespace Tetris
{
    /// <summary>
    /// Interaction logic for MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        private GameWindow gameWindow;
        public MenuWindow()
        {
            gameWindow = new GameWindow();
            InitializeComponent();
        }

        private void startNewGameButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
