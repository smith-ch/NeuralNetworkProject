using System;
using System.IO;
using System.Linq;

public class DataHandler
{
    public static (double[][], double[][]) LoadData(string filePath)
    {
        
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"❌ ERROR: No se encontró el archivo en la ruta: {filePath}");
            Environment.Exit(1);
        }

        var lines = File.ReadAllLines(filePath);

       
        if (lines.Length <= 1)
        {
            Console.WriteLine("❌ ERROR: El archivo CSV no tiene suficientes datos.");
            Environment.Exit(1);
        }

        try
        {
           
            var dataLines = lines.Skip(1).ToArray();

           
            double[][] inputs = dataLines.Select(line => line.Split(',').Take(2).Select(double.Parse).ToArray()).ToArray();
            double[][] outputs = dataLines.Select(line => line.Split(',').Skip(2).Select(double.Parse).ToArray()).ToArray();

            return (inputs, outputs);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR: No se pudo leer el archivo CSV. Verifica que los datos sean numéricos.\n{ex.Message}");
            Environment.Exit(1);
            return (null, null);
        }
    }
}
