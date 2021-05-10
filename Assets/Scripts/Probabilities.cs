using System;
using System.Collections.Generic;

public static class Probabilities
{
    //=================
    //Utility Functions
    //=================
    /// <summary>
    /// Calculates the factorial of an integer
    /// </summary>
    /// <param name="n">
    /// </param>
    /// <returns>
    /// </returns>
    private static int Factorial(int n)
    {
        if (n == 1 || n == 0)
            return 1;
        else
            return n * Factorial(n - 1);
    }

    /// <summary>
    /// Computes the value of Gamma(k/2) for the given integer k
    /// </summary>
    /// <param name="k"></param>
    /// <returns></returns>
    private static float GammaOfKOver2(int k)
    {
        //Even k have simple closed-form solution
        //Odd k have approximate closed-form solution 

        bool isOddBelow9 = (k < 9) && (k % 2 == 1);
        bool isEvenInt = (k % 2 == 0);

        if (isOddBelow9)
        {
            double[] coefficients = { 1, 0.5, 0.75, 1.875 };
            int index = (k - 1) / 2;
            var product = coefficients[index] * Math.Sqrt(Math.PI);

            return (float)product;
        }
        else if (isEvenInt)
        {
            var n = k / 2;
            return Factorial(n - 1);
        }
        else
        {
            Console.WriteLine("GAMMA FUNCTION ERROR: Please use values of 1, 3, 5, 7, or an even integer.");
            return 0;
        }

    }

    /// <summary>
    /// Numerically integrates the given function to the given resolution over the given bounds
    /// </summary>
    /// <param name="func">The function to integrate</param>
    /// <param name="startVal">The lower bound</param>
    /// <param name="endVal">The upper bound</param>
    /// <param name="numRects">The number of subdivisions used; the resolution</param>
    /// <returns>
    /// An approximation of the definite integral from startVal to endVal of func
    /// </returns>
    private static float NumericalIntegration(Func<float, float> func, float startVal, float endVal, int numRects)
    {
        List<float> startPoints = new List<float>();
        List<float> endPoints = new List<float>();
        List<float> areas = new List<float>();

        //Sanity check
        if (numRects < 1)
        {
            Console.WriteLine("Please enter a positive integer for the number of rectangles in numerical integration()");
            return 0;
        }

        //Generate start and end points
        var rectWidth = (endVal - startVal) / (float)numRects;
        startPoints.Add(startVal);
        endPoints.Add(startVal + rectWidth);

        if (numRects > 1)
        {
            for (int i = 1; i < numRects; i++)
            {
                startPoints.Add(startPoints[i - 1] + rectWidth);
                endPoints.Add(endPoints[i - 1] + rectWidth);
            }
        }

        //Calculate areas using generated rectangles
        for (int i = 0; i < numRects; i++)
        {
            var centerVal = (startPoints[i] + endPoints[i]) / 2;
            var width = rectWidth;
            var height = func(centerVal);
            var area = width * height;

            areas.Add(area);
        }

        //Sum areas and return sum
        var sum = 0f;

        foreach (float f in areas)
            sum += f;

        return sum;
    }

    /// <summary>
    /// Calculates the lower incomplete Gamma function to the given parameters
    /// </summary>
    /// <param name="s"></param>
    /// <param name="x"></param>
    /// <param name="numRecs"></param>
    /// <returns></returns>
    private static float LowerGamma(float s, float x, int numRecs)
    {
        var term1Exponent = s - 1.0f;

        Func<float, float> func = t => (float)(Math.Pow(t, term1Exponent) * Math.Exp(-t));
        var composite = NumericalIntegration(func, 0, x, numRecs);

        return composite;
    }

    /// <summary>
    /// Uses binary search to find the solution to func(x) = value 
    /// </summary>
    /// <param name="func">The function to evaluate</param>
    /// <param name="target">The target value to which the input will resolve</param>
    /// <param name="min">The minimum bound on the search</param>
    /// <param name="max">The maximum bound on the search</param>
    /// <param name="maxIter">The maximum number of iterations the search may take</param>
    /// <returns></returns>
    private static float BinarySearch(Func<float, float> func, float target, float min, float max, int maxIter)
    {
        var currentMin = min;
        var currentMax = max;

        Func<float, float> toZero = x => func(x) - target;
        float guess = 0;

        //We must find the zero to the function func(x) - target = 0
        for (int i = 0; i < maxIter; i++)
        {
            guess = (currentMax + currentMin) / 2.0f;
            float value = toZero(guess);

            if (value > 0)
            {
                currentMax = guess;
            }
            else if (value < 0)
            {
                currentMin = guess;
            }
            else if (value == 0)
            {
                return guess;
            }
        }

        return guess;
    }

    //====
    //PDFs
    //====
    /// <summary>
    /// Calculates the probability density of a specific value within a uniform continuous distribution specified by the given parameters
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float UniformPDF(float min, float max, float value)
    {
        var range = max - min;
        double toReturn = 0;

        if (min <= value || value <= max)
        {
            toReturn = Math.Pow(range, -1.0);
        }
        else
        {
            toReturn = 0;
        }

        return (float)toReturn;
    }

    /// <summary>
    /// Calculates the probability density of a specific value within a normal distribution of the given parameters
    /// </summary>
    /// <param name="mean"></param>
    /// <param name="variance"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float NormalPDF(float mean, float variance, float value)
    {
        var exponent = -1 * Math.Pow(value - mean, 2.0f) / (2 * variance);
        var term1 = Math.Exp(exponent);
        var term2 = Math.Sqrt(2 * Math.PI * (variance));

        var toReturn = term1 / term2;
        return (float)toReturn;
    }

    /// <summary>
    /// Calculates the probability density of a specific value within an exponential distribution of the given parameter
    /// </summary>
    /// <param name="rate"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float ExponentialPDF(float rate, float value)
    {
        var exponent = -1 * rate * value;
        var toReturn = rate * Math.Exp(exponent);

        return (float)toReturn;
    }

    /// <summary>
    /// Calculates the probability density of a specific value within a chi-squared distribution of the given parameter
    /// </summary>
    /// <param name="degFreedom"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float X2PDF(int degFreedom, float value)
    {
        if (value > 0)
        {

            var k = degFreedom;
            var x = value;

            var term1Exponent = (k / 2.0) - 1;
            var term2Exponent = -1 * value / 2.0;
            var term3Exponent = k / 2.0;

            var term1 = Math.Pow(x, term1Exponent);
            var term2 = Math.Exp(term2Exponent);
            var term3 = Math.Pow(2, term3Exponent);
            var term4 = GammaOfKOver2(k);

            var composition = term1 * term2 / (term3 * term4);

            return (float)composition;
        }
        else
        {
            return 0;
        }
    }

    //====
    //CDFs
    //====
    /// <summary>
    /// Calculates the cumulative distribution of a specific value within a uniform distribution characterized by the given parameters
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float UniformCDF(float min, float max, float value)
    {
        var a = min;
        var b = max;
        var x = value;

        if (x < a)
        {
            return 0;
        }
        else if (x >= b)
        {
            return 1.0f;
        }
        else
        {
            return (x - a) / (b - a);
        }
    }

    /// <summary>
    /// Calculates the cumulative distribution of a specific value within a normal distribution of the given parameters
    /// </summary>
    /// <param name="mean"></param>
    /// <param name="variance"></param>
    /// <param name="startVal"></param>
    /// <param name="endVal"></param>
    /// <param name="numRects"></param>
    /// <returns></returns>
    public static float NormalCDF(float mean, float variance, float startVal, float endVal, int numRects)
    {
        //Uses numerical integration
        Func<float, float> pdf = x => NormalPDF(mean, variance, x);
        var value = NumericalIntegration(pdf, startVal, endVal, numRects);

        return (float)value;
    }

    /// <summary>
    /// Calculates the cumulative distribution of a specific value within an exponential distribution characterized by the given parameter
    /// </summary>
    /// <param name="scale"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float ExponentialCDF(float scale, float value)
    {
        var exponent = -1 * scale * value;
        var composite = 1 - Math.Exp(exponent);
        return (float)composite;
    }

    /// <summary>
    /// Calculates the cumulative distribution of a specific value within a chi-squared distribution characterized by the given parameter
    /// </summary>
    /// <param name="degFreedom"></param>
    /// <param name="value"></param>
    /// <param name="numRecs"></param>
    /// <returns></returns>
    public static float X2CDF(int degFreedom, float value, int numRecs)
    {
        //Uses analytic solution for uppercase Gamma 
        //Uses numerical integration for lowercase gamma
        var k = (float)degFreedom;
        var x = value;
        var term1 = LowerGamma(k / 2.0f, x / 2.0f, numRecs);
        var term2 = GammaOfKOver2(degFreedom);

        var composite = term1 / term2;

        return composite;
    }

    //==========================
    //Random Variable Generators
    //==========================
    /// <summary>
    /// Generates a random number from the specified normal distribution
    /// </summary>
    /// <param name="mean"></param>
    /// <param name="variance"></param>
    /// <param name="maxIterations">The number of iterations the binary search can take to find the correct value</param>
    /// <param name="numRects">The number of rectangles the numeric iteration can use to find NormalCDF</param>
    /// <returns></returns>
    public static float RandomNormalVariable(float mean, float variance, int maxIterations, int numRects)
    {
        var CDFValue = UnityEngine.Random.value;
        var minValue = mean - 3 * variance;
        var maxValue = mean + 3 * variance; 
        Func<float, float> func = x => NormalCDF(mean, variance, minValue, x, numRects);

        var value = BinarySearch(func, CDFValue, minValue, maxValue, maxIterations);

        return value;

    }

    /// <summary>
    /// Generates a random number from the specified exponential distribution
    /// </summary>
    /// <param name="scale">The scale parameter of the distribution</param>
    /// <returns></returns>
    public static float RandomExponentialVairable(float scale)
    {
        var CDFValue = UnityEngine.Random.value;
        float value = (1.0f / -scale) * (float)Math.Log(1.0 - CDFValue, Math.E);

        return value;

    }

    /// <summary>
    /// Generates a random number from the specified X2 distribution
    /// </summary>
    /// <param name="degFreedom">The degrees of freedom parameter of the distribution</param>
    /// <param name="maxIterations">The number of iterations the binary search can take to find the correct value</param>
    /// <param name="numRects">The number of rectangles the numeric iteration can use to find NormalCDF</param>
    /// <returns></returns>
    public static float RandomX2Variable(int degFreedom, int maxIterations, int numRects)
    {
        var CDFValue = UnityEngine.Random.value;
        var minValue = 0;
        var maxValue = 10 + 10 * (degFreedom / 10);
        Func<float, float> func = x => X2CDF(degFreedom, x, numRects);

        var value = BinarySearch(func, CDFValue, minValue, maxValue, maxIterations);

        return value;
    }
}
