using System;

class ActivationUtils
{
    public static double[] Sigmoid(double[] values)
    {
        double[] result = new double[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            result[i] = 1.0 / (1.0 + Math.Exp(-values[i]));
        }
        return result;
    }
}
