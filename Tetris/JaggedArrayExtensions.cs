using System.Linq;

namespace Tetris
{
    public static class JaggedArrayExtensions
    {
        public static int[][] GetMatrixCopy(this int[][] matrix)
        {
            return matrix.Select(a => a.ToArray()).ToArray();
        }
    }
}