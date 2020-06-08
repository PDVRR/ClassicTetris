namespace Tetris.Model
{
    public class Tetromino
    {
        public int[][] Matrix { get; set; }
        public TetrominoType Type { get; set; }
    }

    public enum TetrominoType
    {
        None = 0,
        IType,
        OType,
        TType,
        SType,
        ZType,
        JType,
        LType
    }
}