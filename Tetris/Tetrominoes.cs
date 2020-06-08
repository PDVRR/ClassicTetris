using System.Collections.Generic;
using Tetris.Model;

namespace Tetris
{
    public static class Tetrominoes
    {
        public static List<Tetromino> TetrominoesList = new List<Tetromino>();

        static Tetrominoes()
        {
            TetrominoesList.Add(
                new Tetromino()
                {
                    Matrix = new[]{
                        new[] {0, 0, 0, 0},
                        new[] {0, 0, 0, 0},
                        new[] {1, 1, 1, 1},
                        new[] {0, 0, 0, 0}
                    },
                    Type = TetrominoType.IType
                }
                );
            TetrominoesList.Add(
                new Tetromino()
                {
                    Matrix = new[]{
                        new[] {2,2},
                        new[] {2,2}
                    },
                    Type = TetrominoType.OType
                }
            );
            TetrominoesList.Add(
                new Tetromino()
                {
                    Matrix = new[]{
                        new[] {0,0,0},
                        new[] {3,3,3},
                        new[] {0,3,0}
                    },
                    Type = TetrominoType.TType
                }
            );
            TetrominoesList.Add(
                new Tetromino()
                {
                    Matrix = new[]{
                        new[] {0,0,0},
                        new[] {4,4,0},
                        new[] {0,4,4}
                    },
                    Type = TetrominoType.SType
                }
            );
            TetrominoesList.Add(
                new Tetromino()
                {
                    Matrix = new[]{
                        new[] {0,0,0},
                        new[] {0,5,5},
                        new[] {5,5,0}
                    },
                    Type = TetrominoType.ZType
                }
            );
            TetrominoesList.Add(
                new Tetromino()
                {
                    Matrix = new[]{
                        new[] {0,0,0},
                        new[] {6,6,6},
                        new[] {0,0,6}
                    },
                    Type = TetrominoType.JType
                }
            );
            TetrominoesList.Add(
                new Tetromino()
                {
                    Matrix = new[]{
                        new[] {0,0,0},
                        new[] {7,7,7},
                        new[] {7,0,0}
                    },
                    Type = TetrominoType.LType
                }
            );
        }
    }
}