using System.Collections.Generic;
using System;

public class BellCurve
{
    private Random random;
    private double mean;
    private double standardDeviation;

    public BellCurve(double mean, double standardDeviation)
    {
        this.random = new Random();
        this.mean = mean;
        this.standardDeviation = standardDeviation;
    }

    public double GenerateRandomNumber(double x)
    {
        double randomNumber;
        
        do
        {
            double u1 = 1.0 - random.NextDouble();
            double u2 = 1.0 - random.NextDouble();
            double normalRandomNumber = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
            randomNumber = (normalRandomNumber * standardDeviation) + mean; 
        } while (Math.Abs(randomNumber) > x);
        
        return randomNumber;
    }

    public double[] GenerateCurvePoints(double threshold, int numberOfPoints)
    {
        double[] curvePoints = new double[numberOfPoints];
        double interval = threshold / (numberOfPoints - 1);

        for (int i = 0; i < numberOfPoints; i++)
        {
            double x = i * interval;
            double y = GenerateRandomNumber(x);
            curvePoints[i] = y;
        }

        return curvePoints;
    }
}