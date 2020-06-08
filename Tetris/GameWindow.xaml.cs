using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Tetris.Model;
using Shape = Tetris.Model.Shape;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private int x = 10, y = 20;
        private Engine gameEngine;
        Random rand = new Random();
        private Rectangle[][] tetrisFieldRectangles = new Rectangle[20][];
        private Rectangle[][] nextShapeRectangles = new Rectangle[2][];
        private bool spacePressed, downPressed, pausePressed;

        private Dictionary<int, BitmapImage> colorsDictionary = new Dictionary<int, BitmapImage>();

        private readonly Color tetrisFieldBackgroundColor = Color.FromRgb(31, 31, 31);
        private readonly Color nextShapeFieldBackgroundColor = Colors.Gray;
        public GameWindow()
        {
            InitializeComponent();
            InitializeGameEngine();
            RandomizeShapeColors();
            InitializeTetrisField();
            InitializeNextShapeField();

            StartButton.Focusable = false;
            pauseButton.Focusable = false;
        }

        private void InitializeGameEngine()
        {
            gameEngine = new Engine();
            gameEngine.OnPointChanged += OnPointChanged;
            gameEngine.OnLevelChanged += OnLevelChanged;
            gameEngine.OnScoreChanged += OnScoreChanged;
            gameEngine.OnLinesChanged += OnLinesChanged;
            gameEngine.OnGameOver += OnGameOver;
            gameEngine.OnNextShapeGenerated += OnNextShapeGenerated;
        }

        private void RandomizeShapeColors()
        {
            for (int i = 1; i <= 7; i++)
            {
                colorsDictionary.Add(i, new BitmapImage(new Uri(@"pack://application:,,,/Tetris;component/Images/Blocks/" + rand.Next(3) + ".png", UriKind.Absolute)));
            }
        }

        private void InitializeTetrisField()
        {
            for (int i = 0; i < y; i++)
            {
                tetrisFieldRectangles[i] = new Rectangle[x];
            }

            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    var rect = new Rectangle()
                    {
                        Width = 30,
                        Height = 30,
                        Stroke = new SolidColorBrush(tetrisFieldBackgroundColor),
                        Fill = new SolidColorBrush(tetrisFieldBackgroundColor)
                    };
                    Canvas.SetLeft(rect, 30 * j);
                    Canvas.SetTop(rect, 30 * i);
                    gameCanvas.Children.Add(rect);
                    tetrisFieldRectangles[i][j] = rect;
                }
            }
        }

        private void InitializeNextShapeField()
        {
            for (int i = 0; i < 2; i++)
            {
                nextShapeRectangles[i] = new Rectangle[4];
            }

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    var rect = new Rectangle()
                    {
                        Width = 30,
                        Height = 30,
                        Stroke = new SolidColorBrush(nextShapeFieldBackgroundColor),
                        Fill = new SolidColorBrush(nextShapeFieldBackgroundColor)
                    };
                    Canvas.SetLeft(rect, 30 * j);
                    Canvas.SetTop(rect, 30 * i);
                    nextShapeCanvas.Children.Add(rect);
                    nextShapeRectangles[i][j] = rect;
                }
            }
        }

        private void OnNextShapeGenerated(object sender, Shape e)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    nextShapeRectangles[i][j].Stroke = new SolidColorBrush(nextShapeFieldBackgroundColor);
                    nextShapeRectangles[i][j].Fill = new SolidColorBrush(nextShapeFieldBackgroundColor);
                }
            }
            int upperBound = 0;
            switch (e.Size)
            {
                case 1:
                    upperBound = 0;
                    break;
                case 3:
                    upperBound = 1;
                    break;
                case 4:
                    upperBound = 2;
                    break;
            }
            for (int i = upperBound; i < e.Size; i++)
            {
                for (int j = 0; j < e.Size; j++)
                {
                    if (e.Matrix[i][j] != 0)
                    {
                        nextShapeRectangles[i - upperBound][j].Stroke = new SolidColorBrush(Color.FromRgb(31, 31, 31));
                        nextShapeRectangles[i - upperBound][j].Fill = new ImageBrush
                            {ImageSource = colorsDictionary[e.Matrix[i][j]]};
                        //nextShapeRectangles[i - upperBound][j].Fill = new SolidColorBrush(colorsDictionary[e.Matrix[i][j]]);
                    }
                }
            }
        }

        private void OnGameOver(object sender, EventArgs e)
        {
            levelTextBox.Focusable = true;
            if (MessageBox.Show($"Game Over! Level: {levelLabel.Content}. Score: {scoreLabel.Content}. Try again?", "Game Over", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                StartGame();
            }
        }

        private void OnLinesChanged(object sender, int e)
        {
            linesLabel.Content = e.ToString();
        }

        private void OnScoreChanged(object sender, int e)
        {
            scoreLabel.Content = e.ToString();
        }

        private void OnLevelChanged(object sender, int e)
        {
            levelLabel.Content = e.ToString();
        }


        private void OnPointChanged(object sender, DrawPoint e)
        {
            if (e.Y >= y || e.Y < 0)
            {
                return;
            }
            if (e.ColorNumber != 0)
            {
                tetrisFieldRectangles[e.Y][e.X].Stroke = new SolidColorBrush(Color.FromRgb(31, 31, 31));
                tetrisFieldRectangles[e.Y][e.X].Fill = new ImageBrush
                    {ImageSource = colorsDictionary[e.ColorNumber]};
            }
            else
            {
                tetrisFieldRectangles[e.Y][e.X].Stroke = new SolidColorBrush(tetrisFieldBackgroundColor);
                tetrisFieldRectangles[e.Y][e.X].Fill = new SolidColorBrush(tetrisFieldBackgroundColor);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            PauseTheGame();
        }

        void StartGame()
        {
            int.TryParse(levelTextBox.Text, out var number);
            levelLabel.Content = number.ToString();
            linesLabel.Content = "0";
            scoreLabel.Content = "0";
            gameEngine.StartNewGame(number);
            levelTextBox.Focusable = false;
        }

        private void PauseTheGame()
        {
            if (!pausePressed)
            {
                gameEngine.Pause();
                pauseButton.Content = "Resume";
            }
            else
            {
                gameEngine.Resume();
                pauseButton.Content = "Pause";
            }

            pausePressed = !pausePressed;
        }

        private void MainWindow1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                    gameEngine.SlowDown();
                    downPressed = false;
                    break;
                case Key.Space:
                    spacePressed = false;
                    break;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                PauseTheGame();
                return;
            }
            if (pausePressed)
            {
                return;
            }
            switch (e.Key)
            {
                case Key.Left:
                    gameEngine.MoveLeft();
                    break;
                case Key.Right:
                    gameEngine.MoveRight();
                    break;
                case Key.Down:
                    if (!downPressed)
                    {
                        gameEngine.SpeedUp();
                        downPressed = true;
                    }
                    break;
                case Key.Up:
                    gameEngine.RotateAntiClockwise();
                    break;
                case Key.Space:
                    if (!spacePressed)
                    {
                        gameEngine.Drop();
                        spacePressed = true;
                    }
                    break;
            }
        }
    }
}
