using System;

class MatrixUtils
{
    public static double[,] RandomMatrix(int rows, int cols)
    {
        Random rnd = new Random();
        double[,] matrix = new double[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                matrix[i, j] = rnd.NextDouble();
            }
        }
        return matrix;
    }

    public static (double[][], double[][]) LoadData(string filePath)
    {
        var lines = File.ReadAllLines(filePath).Skip(1).ToArray(); 
        double[][] inputs = lines.Select(line => line.Split(',').Take(2).Select(double.Parse).ToArray()).ToArray();
        double[][] outputs = lines.Select(line => line.Split(',').Skip(2).Take(2).Select(double.Parse).ToArray()).ToArray(); 

        return (inputs, outputs);
    }

}
