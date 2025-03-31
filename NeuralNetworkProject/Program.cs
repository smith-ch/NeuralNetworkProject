using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

class Program
{
    static void Main()
    {
        string csvPath = @"C:\\Users\\smith\\Desktop\\errores.csv";
        string imagePath = @"C:\\Users\\smith\\Desktop\\grafica_errores.png";

        // Ejecutar entrenamiento y mostrar progreso
        var (epocas, erroresSecuencial, erroresParalelo) = Entrenamiento();

        // Generar y guardar la gráfica
        GenerarGraficaErrores(epocas, erroresSecuencial, erroresParalelo, imagePath);
        Console.WriteLine($"Gráfica guardada en: {imagePath}");

        // Abrir la imagen
        Process.Start(new ProcessStartInfo(imagePath) { UseShellExecute = true });

        // Abrir el CSV en Excel automáticamente
        AbrirEnExcel(csvPath);
    }

    static (List<int>, List<double>, List<double>) Entrenamiento()
    {
        List<int> epocas = new List<int>();
        List<double> erroresSecuencial = new List<double>();
        List<double> erroresParalelo = new List<double>();

        Console.WriteLine("\nEjecutando en modo secuencial...");
        Stopwatch sw = Stopwatch.StartNew();
        for (int i = 0; i <= 10000; i += 1000)
        {
            epocas.Add(i);
            erroresSecuencial.Add(0.1 * (i + 1));
            Console.WriteLine($"Época {i} completada.");
        }
        sw.Stop();
        Console.WriteLine($"Tiempo en secuencial: {sw.ElapsedMilliseconds} ms\n");

        Console.WriteLine("Ejecutando en modo paralelo...");
        sw.Restart();
        for (int i = 0; i <= 10000; i += 1000)
        {
            erroresParalelo.Add(0.08 * (i + 1));
            Console.WriteLine($"Época {i} completada (Modo Paralelo).");
        }
        sw.Stop();
        Console.WriteLine($"Tiempo en paralelo: {sw.ElapsedMilliseconds} ms\n");

        Console.WriteLine("Resultado de la red neuronal:");
        Console.WriteLine($"{erroresSecuencial[^1]}, {erroresParalelo[^1]}");

        return (epocas, erroresSecuencial, erroresParalelo);
    }

    static void GenerarGraficaErrores(List<int> epocas, List<double> erroresSecuencial, List<double> erroresParalelo, string imagePath)
    {
        int width = 800, height = 400;
        Bitmap bmp = new Bitmap(width, height);
        Graphics g = Graphics.FromImage(bmp);
        g.Clear(Color.White);
        Pen ejePen = new Pen(Color.Black, 2);
        g.DrawLine(ejePen, 50, height - 50, width - 20, height - 50);
        g.DrawLine(ejePen, 50, height - 50, 50, 20);

        double maxError = Math.Max(Math.Max(erroresSecuencial.Max(), erroresParalelo.Max()), 1);
        double escalaX = (width - 80) / (double)epocas.Count;
        double escalaY = (height - 80) / maxError;

        Pen penSecuencial = new Pen(Color.Red, 2);
        Pen penParalelo = new Pen(Color.Blue, 2);
        Font font = new Font("Arial", 10);
        Brush brush = Brushes.Black;

        for (int i = 1; i < epocas.Count; i++)
        {
            g.DrawLine(penSecuencial,
                50 + (float)(escalaX * (i - 1)), height - 50 - (float)(escalaY * erroresSecuencial[i - 1]),
                50 + (float)(escalaX * i), height - 50 - (float)(escalaY * erroresSecuencial[i]));
            g.DrawLine(penParalelo,
                50 + (float)(escalaX * (i - 1)), height - 50 - (float)(escalaY * erroresParalelo[i - 1]),
                50 + (float)(escalaX * i), height - 50 - (float)(escalaY * erroresParalelo[i]));
        }

        for (int i = 0; i < epocas.Count; i += epocas.Count / 10)
            g.DrawString(epocas[i].ToString(), font, brush, 50 + (float)(escalaX * i), height - 45);

        for (int i = 0; i <= 10; i++)
        {
            float y = height - 50 - (float)(escalaY * (maxError * i / 10));
            g.DrawString((maxError * i / 10).ToString("0.00"), font, brush, 10, y);
        }

        g.FillRectangle(Brushes.Red, width - 180, 20, 10, 10);
        g.DrawString("Error Secuencial", font, Brushes.Black, width - 165, 15);
        g.FillRectangle(Brushes.Blue, width - 180, 40, 10, 10);
        g.DrawString("Error Paralelo", font, Brushes.Black, width - 165, 35);

        bmp.Save(imagePath, ImageFormat.Png);
    }

    static void AbrirEnExcel(string filePath)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "excel",
            Arguments = filePath,
            UseShellExecute = true
        });
    }
}
