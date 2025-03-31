using System;
using System.Linq;
using System.IO;

public class NeuralNetwork
{
    private int inputSize;
    private int hiddenSize;
    private int outputSize;
    private double[,] weightsInputHidden;
    private double[,] weightsHiddenOutput;
    private double[] hiddenBias;
    private double[] outputBias;
    private Random rand = new Random();

    public NeuralNetwork(int inputSize, int hiddenSize, int outputSize)
    {
        this.inputSize = inputSize;
        this.hiddenSize = hiddenSize;
        this.outputSize = outputSize;

        weightsInputHidden = new double[inputSize, hiddenSize];
        weightsHiddenOutput = new double[hiddenSize, outputSize];
        hiddenBias = new double[hiddenSize];
        outputBias = new double[outputSize];

        InitializeWeights();
    }

    private void InitializeWeights()
    {
        for (int i = 0; i < inputSize; i++)
            for (int j = 0; j < hiddenSize; j++)
                weightsInputHidden[i, j] = rand.NextDouble() * 2 - 1;

        for (int i = 0; i < hiddenSize; i++)
            for (int j = 0; j < outputSize; j++)
                weightsHiddenOutput[i, j] = rand.NextDouble() * 2 - 1;

        for (int i = 0; i < hiddenSize; i++)
            hiddenBias[i] = rand.NextDouble() * 2 - 1;

        for (int i = 0; i < outputSize; i++)
            outputBias[i] = rand.NextDouble() * 2 - 1;
    }

    private double Sigmoid(double x) => 1.0 / (1.0 + Math.Exp(-x));
    private double SigmoidDerivative(double x) => x * (1 - x);

    public double[] Forward(double[] inputs)
    {
        double[] hiddenLayer = new double[hiddenSize];
        double[] outputLayer = new double[outputSize];

        for (int i = 0; i < hiddenSize; i++)
        {
            hiddenLayer[i] = hiddenBias[i];
            for (int j = 0; j < inputSize; j++)
                hiddenLayer[i] += inputs[j] * weightsInputHidden[j, i];
            hiddenLayer[i] = Sigmoid(hiddenLayer[i]);
        }

        for (int i = 0; i < outputSize; i++)
        {
            outputLayer[i] = outputBias[i];
            for (int j = 0; j < hiddenSize; j++)
                outputLayer[i] += hiddenLayer[j] * weightsHiddenOutput[j, i];
            outputLayer[i] = Sigmoid(outputLayer[i]);
        }

        return outputLayer;
    }


    public double[] Predict(double[] input)
    {
        return Forward(input);
    }

    private double Backpropagate(double[] input, double[] expectedOutput, double learningRate)
    {
        if (input.Length != inputSize)
            throw new Exception($"Error: Tamaño de input incorrecto. Esperado {inputSize}, recibido {input.Length}");

        if (expectedOutput.Length != outputSize)
            throw new Exception($"Error: Tamaño de expectedOutput incorrecto. Esperado {outputSize}, recibido {expectedOutput.Length}");

       
        double[] hiddenLayer = new double[hiddenSize];
        double[] outputLayer = new double[outputSize];

        for (int i = 0; i < hiddenSize; i++)
        {
            hiddenLayer[i] = hiddenBias[i];
            for (int j = 0; j < inputSize; j++)
                hiddenLayer[i] += input[j] * weightsInputHidden[j, i];
            hiddenLayer[i] = Sigmoid(hiddenLayer[i]);
        }

        for (int i = 0; i < outputSize; i++)
        {
            outputLayer[i] = outputBias[i];
            for (int j = 0; j < hiddenSize; j++)
                outputLayer[i] += hiddenLayer[j] * weightsHiddenOutput[j, i];
            outputLayer[i] = Sigmoid(outputLayer[i]);
        }

        
        double[] outputErrors = new double[outputSize];
        double[] outputDeltas = new double[outputSize];

        double totalError = 0; 

        for (int i = 0; i < outputSize; i++)
        {
            outputErrors[i] = expectedOutput[i] - outputLayer[i];
            outputDeltas[i] = outputErrors[i] * SigmoidDerivative(outputLayer[i]);

            totalError += Math.Pow(outputErrors[i], 2); 
        }

        totalError /= outputSize; 
        

        double[] hiddenErrors = new double[hiddenSize];
        double[] hiddenDeltas = new double[hiddenSize];

        for (int i = 0; i < hiddenSize; i++)
        {
            hiddenErrors[i] = 0;
            for (int j = 0; j < outputSize; j++)
                hiddenErrors[i] += outputDeltas[j] * weightsHiddenOutput[i, j];

            hiddenDeltas[i] = hiddenErrors[i] * SigmoidDerivative(hiddenLayer[i]);
        }

        
        for (int i = 0; i < hiddenSize; i++)
            for (int j = 0; j < outputSize; j++)
                weightsHiddenOutput[i, j] += learningRate * outputDeltas[j] * hiddenLayer[i];

        for (int i = 0; i < outputSize; i++)
            outputBias[i] += learningRate * outputDeltas[i];

        for (int i = 0; i < inputSize; i++)
            for (int j = 0; j < hiddenSize; j++)
                weightsInputHidden[i, j] += learningRate * hiddenDeltas[j] * input[i];

        for (int i = 0; i < hiddenSize; i++)
            hiddenBias[i] += learningRate * hiddenDeltas[i];

        return totalError; 
    }


    public List<double> TrainSequential(double[][] inputs, double[][] expectedOutputs, int epochs, double learningRate, string csvPath)
    {
        List<string> errorLog = new List<string>();
        List<double> errores = new List<double>(); 

        for (int epoch = 0; epoch < epochs; epoch++)
        {
            double totalError = 0;

            for (int i = 0; i < inputs.Length; i++)
            {
                totalError += Backpropagate(inputs[i], expectedOutputs[i], learningRate);
            }

            double averageError = totalError / inputs.Length;
            errores.Add(averageError); 
            errorLog.Add($"{epoch},{averageError}");

            if (epoch % 1000 == 0)
                Console.WriteLine($"Época {epoch} - Error: {averageError}");
        }

        File.WriteAllLines(csvPath, errorLog);
        return errores; 
    }

    public List<double> TrainParallel(double[][] inputs, double[][] expectedOutputs, int epochs, double learningRate, string csvPath)
    {
        List<string> errorLog = new List<string>();
        List<double> errores = new List<double>();

        for (int epoch = 0; epoch < epochs; epoch++)
        {
            double totalError = 0;

            Parallel.For(0, inputs.Length, i =>
            {
                double error = Backpropagate(inputs[i], expectedOutputs[i], learningRate);
                lock (inputs) totalError += error;
            });

            double averageError = totalError / inputs.Length;
            errores.Add(averageError);
            errorLog.Add($"{epoch},{averageError}");

            if (epoch % 1000 == 0)
                Console.WriteLine($"Época {epoch} (Modo Paralelo) - Error: {averageError}");
        }

        File.WriteAllLines(csvPath, errorLog);
        return errores; 
    }


    
    private double MSE(double[] output, double[] expected)
    {
        double sum = 0;
        for (int i = 0; i < output.Length; i++)
        {
            sum += Math.Pow(output[i] - expected[i], 2);
        }
        return sum / output.Length;
    }





}

