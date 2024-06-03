using System.Data;

namespace MultimediaBuilder.Maths
{
    /// <summary>
    /// Simple matrix with custom number of rows and columns, starter coordinate is (0, 0)
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public struct Matrix<T>
    {
        private T[] items;

        public int numRows;
        public int numCols;

        public Matrix(int numRows, int numCols)
        {
            this.numRows = numRows;
            this.numCols = numCols;

            items = new T[numRows * numCols];
        }

        //public methods

        //Get value at coords
        public T GetAtCoords(int row, int col)
        {
            return items[GetIndex(row, col)];
        }

        //Set value at coords
        public void SetAtCoords(int row, int col, T value)
        {
            items[GetIndex(row, col)] = value;
        }

        //Get all values as array
        public T[] GetAllValues()
        {
            return items;
        }

        //private method for gettig inde of 1D array from 2D coords
        private int GetIndex(int row, int col)
        {
            return row * numCols + col;
        }
    }
}