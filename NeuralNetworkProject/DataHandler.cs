using System;
using System.IO;
using System.Linq;

public class DataHandler
{
    public static (double[][], double[][]) LoadData(string filePath)
    {
        // 1️⃣ Verifica si el archivo existe
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"❌ ERROR: No se encontró el archivo en la ruta: {filePath}");
            Environment.Exit(1);
        }

        var lines = File.ReadAllLines(filePath);

        // 2️⃣ Verifica si el archivo tiene datos suficientes
        if (lines.Length <= 1)
        {
            Console.WriteLine("❌ ERROR: El archivo CSV no tiene suficientes datos.");
            Environment.Exit(1);
        }

        try
        {
            // 3️⃣ Ignorar la primera línea (encabezados)
            var dataLines = lines.Skip(1).ToArray();

            // 4️⃣ Convertir datos a números
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
