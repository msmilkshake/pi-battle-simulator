using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public class Generator : MonoBehaviour
{
    private static string logFilePath = "mixed.txt";


    private void Start()
    {
        WriteNumbersToFile(200_000, "ldd.txt",
            new LevelDiscreteDistribution());
        
        WriteNumbersToFile(200_000, "gtd.txt",
            new GaussianDistribution(0f, 2.25f, -7f, 7f));
        WriteNumbersToFile(200_000, "gd.txt",
            new GaussianDistribution(45f / 4f, 2f * 60f / 75f + 0.1f));
        WriteNumbersToFile(200_000, "gbd.txt",
            new GaussianBimodalDistribution(-1f, 0.5f, 4f, 0.75f, 0.125f));
        WriteNumbersToFile(200_000, "ed.txt",
            new ExponentialDistribution(0.5f, 0.5f, 0.5f, 1.5f));
        WriteNumbersToFile(200_000, "wd.txt",
            new WeibullDistribution(0, 35, 12));
        
        WriteNumbersToFile(200_000, "med.txt",
            new MixedExpDistribution());
    }

    public static double GetSample()
    {
        float mu = 0.0f;
        float sig = 0.85f;

        float a = 0.0001987332514f;
        float b = 0.9998012667486f;
        float const1 = b - a;

        float y = a + Random.value * const1;

        return sig * Mathf.Sqrt(2) * ErfInv(2 * y - 1) + mu;
    }

    public static double GetSample3()
    {
        float mu = 3.0f;
        float sig = 1.35f;
        float s = sig * Mathf.Sqrt(2);

        float lam = Erf((mu) / s);
        float eta = Erf((6 - mu) / s);

        return mu + s * ErfInv((eta + lam) * Random.value - lam);
    }

    public static double GetSample2()
    {
        float mu = 0.0f;
        float sig = 1.35f;
        float s = sig * Mathf.Sqrt(2);
        float lam = Erf(mu / s);
        float eta = Erf((6 - mu) / s);
        return mu + s * ErfInv((eta + lam) * Random.value - lam);
    }

    public void WriteNumbersToFile(int iterations, String filename, IDistribution d)
    {
        using (StreamWriter sw = File.CreateText(filename))
        {
            for (int i = 0; i < iterations; i++)
            {
                sw.WriteLine("" + d.Get());
                if (i % 10_000 == 0)
                {
                    Debug.Log("Sampled " + i + " random numbers...");
                }
            }
        }

        Debug.Log("Done.");
    }


    public static void Log(string message)
    {
        using (StreamWriter sw = File.CreateText(logFilePath))
        {
            sw.WriteLine(message);
        }
    }

    public static float Erf(float x)
    {
        return Mathf.Sign(x) * Mathf.Sqrt(1 - Mathf.Exp(-(x * x) * (4 / Mathf.PI + a * x * x) / (1 + a * x * x)));
    }


    private static float a = 0.1400122886867f;

    public static float ErfInv(float x)
    {
        float ln = LnFun(x);
        float b = ln / 2;
        float term1 = 2 / (Mathf.PI * a) + b;
        float term2 = Mathf.Pow(term1, 2);
        float term3 = ln / a;

        return Mathf.Sign(x) * Mathf.Sqrt(Mathf.Sqrt(term2 - term3) - term1);
    }

    private static float LnFun(float x)
    {
        return Mathf.Log(1 - x * x);
    }
}