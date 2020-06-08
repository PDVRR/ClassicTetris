using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Tetris.Model;

namespace Tetris
{
    public class Engine
    {
        private int _x = 10, _y = 23;
        private int _speed = 480;
        private int _score;
        private int _level;
        private int _linesCleared;
        private int[][] _field = new int[23][];
        private DispatcherTimer _timer;
        private readonly ShapeHandler shapeHandler;
        public EventHandler<DrawPoint> OnPointChanged;
        public EventHandler<int> OnScoreChanged;
        public EventHandler<int> OnLevelChanged;
        public EventHandler<int> OnLinesChanged;
        public EventHandler<Shape> OnNextShapeGenerated;
        public EventHandler OnGameOver;

        public Engine()
        {
            InitializeField();
            InitializeTimer();

            shapeHandler = new ShapeHandler();
        }

        private void InitializeField()
        {
            for (int i = 0; i < _y; i++)
            {
                _field[i] = new int[_x];
            }
        }

        private void InitializeTimer()
        {
            _timer = new DispatcherTimer();
            SetUpdateSpeed(_speed);
            _timer.Tick += Update;
        }

        public void StartNewGame(int startLevel)
        {
            _level = startLevel;
            _score = 0;
            _linesCleared = 0;
            ChangeSpeedDependingOnLevel();
            InitializeField();
            SpawnNewShape();
            OnNextShapeGenerated.Invoke(this, shapeHandler.NextShape);
            DrawField();
            _timer.Start();
        }

        public void Pause()
        {
            _timer.Stop();
        }

        public void Resume()
        {
            _timer.Start();
        }

        private void SpawnNewShape()
        {
            shapeHandler.GenerateNextShape();
        }

        private void Update(object sender, EventArgs e)
        {
            ClearArea();
            if (CanMoveDown())
            {
                shapeHandler.MoveDown();
                DrawShape();
            }
            else
            {
                DrawShape();
                CheckLines();
                DrawField();
                SetUpdateSpeed(_speed);
                if (!CanSpawn())
                {
                    Pause();
                    OnGameOver.Invoke(this, null);
                    return;
                }
                shapeHandler.GenerateNextShape();
                OnNextShapeGenerated.Invoke(this, shapeHandler.NextShape);
            }
        }

        public bool CanSpawn()
        {
            for (int i = shapeHandler.NextShape.Y; i < shapeHandler.NextShape.Y + shapeHandler.NextShape.Size; i++)
            {
                for (int j = shapeHandler.NextShape.X; j < shapeHandler.NextShape.X + shapeHandler.NextShape.Size; j++)
                {
                    if (_field[i][j] != 0 && shapeHandler.NextShape.Matrix[i - shapeHandler.NextShape.Y][j - shapeHandler.NextShape.X] != 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void DrawShape()
        {
            for (int i = shapeHandler.CurrentShape.Y; i < shapeHandler.CurrentShape.Y + shapeHandler.CurrentShape.Size; i++)
            {
                for (int j = shapeHandler.CurrentShape.X; j < shapeHandler.CurrentShape.X + shapeHandler.CurrentShape.Size; j++)
                {
                    if (shapeHandler.CurrentShape.Matrix[i - shapeHandler.CurrentShape.Y][j - shapeHandler.CurrentShape.X] != 0)
                    {
                        int colorNumber =
                            shapeHandler.CurrentShape.Matrix[i - shapeHandler.CurrentShape.Y][j - shapeHandler.CurrentShape.X];
                        _field[i][j] = colorNumber;
                        NotifyThatPointChanged(j, i, colorNumber);
                    }
                }
            }
        }

        private void NotifyThatPointChanged(int x, int y, int colorNumber)
        {
            y -= 3;
            OnPointChanged.Invoke(this, new DrawPoint()
            {
                X = x,
                Y = y,
                ColorNumber = colorNumber
            });
        }

        public void ClearArea()
        {
            for (int i = shapeHandler.CurrentShape.Y; i < shapeHandler.CurrentShape.Y + shapeHandler.CurrentShape.Size; i++)
            {
                for (int j = shapeHandler.CurrentShape.X; j < shapeHandler.CurrentShape.X + shapeHandler.CurrentShape.Size; j++)
                {
                    if (i < _y && j < _x && j >= 0 && shapeHandler.CurrentShape.Matrix[i - shapeHandler.CurrentShape.Y][j - shapeHandler.CurrentShape.X] != 0)
                    {
                        _field[i][j] = 0;
                        NotifyThatPointChanged(j, i, 0);
                    }
                }
            }
        }

        public void DrawField()
        {
            for (int i = 0; i < _y; i++)
            {
                for (int j = 0; j < _x; j++)
                {
                    int colorNumber = _field[i][j];
                    NotifyThatPointChanged(j, i, colorNumber);
                }
            }
        }

        internal bool CanMoveDown()
        {
            for (int i = shapeHandler.CurrentShape.Y + shapeHandler.CurrentShape.Size - 1; i >= shapeHandler.CurrentShape.Y; i--)
            {
                for (int j = shapeHandler.CurrentShape.X; j < shapeHandler.CurrentShape.X + shapeHandler.CurrentShape.Size; j++)
                {
                    if (shapeHandler.CurrentShape.Matrix[i - shapeHandler.CurrentShape.Y][j - shapeHandler.CurrentShape.X] != 0)
                    {
                        if (i + 1 >= _y)
                        {
                            return false;
                        }

                        if (_field[i + 1][j] != 0)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        internal bool CanMoveHorizontal(int direction)
        {
            for (int i = shapeHandler.CurrentShape.Y; i < shapeHandler.CurrentShape.Y + shapeHandler.CurrentShape.Size; i++)
            {
                for (int j = shapeHandler.CurrentShape.X; j < shapeHandler.CurrentShape.X + shapeHandler.CurrentShape.Size; j++)
                {
                    if (shapeHandler.CurrentShape.Matrix[i - shapeHandler.CurrentShape.Y][j - shapeHandler.CurrentShape.X] != 0)
                    {
                        if (j + direction >= _x || j + direction < 0)
                            return false;
                        if (_field[i][j + direction] != 0)
                        {
                            if (j - shapeHandler.CurrentShape.X + direction >= shapeHandler.CurrentShape.Size ||
                                j - shapeHandler.CurrentShape.X + direction < 0)
                                return false;
                            if (shapeHandler.CurrentShape.Matrix[i - shapeHandler.CurrentShape.Y][j - shapeHandler.CurrentShape.X + direction] == 0)
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        internal void MoveLeft()
        {
            if (CanMoveHorizontal(-1))
            {
                ClearArea();
                shapeHandler.MoveLeft();
                DrawShape();
            }
        }

        internal void MoveRight()
        {
            if (CanMoveHorizontal(1))
            {
                ClearArea();
                shapeHandler.MoveRight();
                DrawShape();
            }
        }

        //internal void RotateClockwise()
        //{
        //    ClearArea();
        //    shapeHandler.RotateClockwise();
        //    if (!IsRotateCorrect())
        //    {
        //        shapeHandler.RotateAntiClockwise();
        //    }
        //    DrawShape();

        //}

        internal void RotateAntiClockwise()
        {
            ClearArea();

            Shape rotatedShape = new Shape
            {
                X = shapeHandler.CurrentShape.X,
                Y = shapeHandler.CurrentShape.Y,
                Size = shapeHandler.CurrentShape.Size,
                Matrix = shapeHandler.CurrentShape.Matrix.GetMatrixCopy()
            };
            shapeHandler.RotateAntiClockwise(rotatedShape);

            if (IsRotateCorrect(rotatedShape))
            {
                shapeHandler.CurrentShape = rotatedShape;
            }
            DrawShape();
        }

        private bool IsRotateCorrect(Shape shape)
        {
            for (int i = shape.Y; i < shape.Y + shape.Size; i++)
            {
                for (int j = shape.X; j < shape.X + shape.Size; j++)
                {
                    if (shape.Matrix[i - shape.Y][j - shape.X] != 0)
                    {
                        if (j >= _x || j < 0)
                        {
                            return false;
                        }

                        if (_field[i][j] != 0)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void CheckLines()
        {
            int filledCells = 0;
            List<int> shiftIndexes = new List<int>();
            for (int i = 0; i < _y; i++)
            {
                for (int j = 0; j < _x; j++)
                {
                    if (_field[i][j] != 0)
                    {
                        filledCells++;
                    }
                }

                if (filledCells == _x)
                {
                    _field[i] = new int[_x];
                    shiftIndexes.Add(i);
                }

                filledCells = 0;
            }

            int linesCount = shiftIndexes.Count;
            if (linesCount != 0)
            {
                ShiftLines(shiftIndexes);
                AddPoints(linesCount);
                UpdateLines(linesCount);
                UpdateLevel();
            }
        }

        private void ShiftLines(List<int> shiftIndexes)
        {
            foreach (var shiftIndex in shiftIndexes)
            {
                for (int i = shiftIndex; i > 0; i--)
                {
                    for (int j = 0; j < _x; j++)
                    {
                        _field[i][j] = _field[i - 1][j];
                    }
                }
            }
        }

        private void AddPoints(int linesCount)
        {
            switch (linesCount)
            {
                case 1:
                    _score += 40 * (_level + 1);
                    break;
                case 2:
                    _score += 100 * (_level + 1);
                    break;
                case 3:
                    _score += 300 * (_level + 1);
                    break;
                case 4:
                    _score += 1200 * (_level + 1);
                    break;
            }
            OnScoreChanged.Invoke(this, _score);
        }

        private void UpdateLines(int linesCount)
        {
            _linesCleared += linesCount;
            OnLinesChanged.Invoke(this, _linesCleared);
        }

        private void UpdateLevel()
        {
            int currentLevel = _linesCleared / 10;
            if (currentLevel != _level)
            {
                _level = currentLevel;
                ChangeSpeedDependingOnLevel();
                OnLevelChanged.Invoke(this, _level);
            }
        }

        private void ChangeSpeedDependingOnLevel()
        {
            switch (_level)
            {
                case 0:
                    _speed = 480;
                    break;
                case 1:
                    _speed = 430;
                    break;
                case 2:
                    _speed = 380;
                    break;
                case 3:
                    _speed = 330;
                    break;
                case 4:
                    _speed = 280;
                    break;
                case 5:
                    _speed = 230;
                    break;
                case 6:
                    _speed = 180;
                    break;
                case 7:
                    _speed = 130;
                    break;
                case 8:
                    _speed = 80;
                    break;
                case 9:
                    _speed = 60;
                    break;
                default:
                    if (_level >= 10 && _level <= 12)
                    {
                        _speed = 50;
                    }
                    else if (_level >= 13 && _level <= 15)
                    {
                        _speed = 40;
                    }
                    else if (_level >= 16 && _level <= 18)
                    {
                        _speed = 30;
                    }
                    else if (_level >= 19 && _level <= 28)
                    {
                        _speed = 20;
                    }
                    else if (_level >= 29)
                    {
                        _speed = 10;
                    }
                    break;
            }
            SetUpdateSpeed(_speed);
        }

        public void SpeedUp()
        {
            SetUpdateSpeed(_speed / 8);
            Update(null, null);
        }

        public void SlowDown()
        {
            SetUpdateSpeed(_speed);
        }

        public void Drop()
        {
            SetUpdateSpeed(1);
            Update(null, null);
        }

        private void SetUpdateSpeed(int speed)
        {
            _timer.Interval = TimeSpan.FromMilliseconds(speed);
        }
    }
}