using System;
using System.Linq;
using Tetris.Model;

namespace Tetris
{
    public class ShapeHandler
    {
        public Shape CurrentShape {set; get; }
        public Shape NextShape { private set; get; }

        private static int _lastShape;

        private static Random rand;
        public ShapeHandler()
        {
            rand = new Random();
            CurrentShape = GenerateShape();
            NextShape = GenerateShape();
        }

        private Shape GenerateShape()
        {
            var shape = new Shape();
            
            int randShape = rand.Next(7);
            if (randShape == _lastShape)
            {
                randShape = rand.Next(7);
            }
            _lastShape = randShape;

            shape.Matrix = Tetrominoes.TetrominoesList[randShape].Matrix.GetMatrixCopy();
            shape.Size = shape.Matrix[0].Length;
            shape.X = 4;

            switch (shape.Size)
            {
                case 3:
                    shape.Y = 1;
                    break;
                case 2:
                    shape.Y = 2;
                    break;
                default:
                    shape.Y = 0;
                    break;
            }

            return shape;
        }

        public void GenerateNextShape()
        {
            CurrentShape.X = NextShape.X;
            CurrentShape.Y = NextShape.Y;
            CurrentShape.Matrix = NextShape.Matrix.GetMatrixCopy();
            CurrentShape.Size = NextShape.Size;
            NextShape = GenerateShape();
        }

        public void MoveDown()
        {
            CurrentShape.Y++;
        }

        public void MoveLeft()
        {
            CurrentShape.X--;
        }
        public void MoveRight()
        {
            CurrentShape.X++;
        }

        public void RotateClockwise()
        {
            int totalNumOfLevels = CurrentShape.Size / 2;
            int level = 0;
            int last = CurrentShape.Size - 1;
            while (level < totalNumOfLevels) 
            {
                for (int i = 0; i < last; i++)
                {
                    Swap(ref CurrentShape.Matrix[level][i], ref CurrentShape.Matrix[i][last]);
                    Swap(ref CurrentShape.Matrix[level][i], ref CurrentShape.Matrix[last][last - i + level]);
                    Swap(ref CurrentShape.Matrix[level][i], ref CurrentShape.Matrix[last - i + level][level]);
                }

                ++level;
                --last;
            }
        }

        public void RotateAntiClockwise(Shape shape)
        {
            int totalNumOfLevels = shape.Size / 2;
            int level = 0;
            int last = shape.Size - 1;
            while (level < totalNumOfLevels) 
            {
                for (int i = last; i > level; i--)
                {
                    Swap(ref shape.Matrix[level][i], ref shape.Matrix[last - i + level][level]);
                    Swap(ref shape.Matrix[level][i], ref shape.Matrix[last][last - i + level]);
                    Swap(ref shape.Matrix[level][i], ref shape.Matrix[i][last]);
                }

                ++level;
                --last;
            }
        }

        private void Swap(ref int num1, ref int num2)
        {
            int temp = num1;
            num1 = num2;
            num2 = temp;
        }

    }
}