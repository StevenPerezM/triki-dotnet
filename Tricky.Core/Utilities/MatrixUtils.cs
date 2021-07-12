using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tricky.Core.Utilities
{
    public static class MatrixUtils
    {

        public static bool IsGameOver(string[,] matrix)
        {
            if (ValidateColumns(matrix) || ValidateRows(matrix)
                || ValidateArray(GetPrincipalDiagonal(matrix))
                 || ValidateArray(GetSecondaryDiagonal(matrix)))
                return true;
            return false;
        }

        private static bool ValidateRows(string[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                if (ValidateArray(GetRow(matrix, i)))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool ValidateColumns(string[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if (ValidateArray(GetColumn(matrix, i)))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool ValidateArray(string[] array)
        {
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] == null || array[i] == "" || array[i - 1] != array[i]) return false;
            }
            return true;
        }

        private static string[] GetColumn(string[,] matrix, int columnNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(0))
                    .Select(x => matrix[x, columnNumber])
                    .ToArray();
        }

        private static string[] GetRow(string[,] matrix, int rowNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                    .Select(x => matrix[rowNumber, x])
                    .ToArray();
        }

        private static string[] GetPrincipalDiagonal(string[,] matrix)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                    .Select(x => matrix[x, x])
                    .ToArray();
        }
        private static string[] GetSecondaryDiagonal(string[,] matrix)
        {
            int size = matrix.GetLength(1);
            return Enumerable.Range(0, size)
                    .Select(x => matrix[x, size - 1 - x])
                    .ToArray();
        }


    }
}
