using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegralGraphic
{
    public class Program
    {
        static double Function(double x)
        {
            return x * x;
        }

        static double RectangleMethod(Func<double, double> func, double a, double b)
        {
            int n = 1000;
            double width = (b - a) / n;
            double sum = 0.0;

            for (int i = 0; i < n; i++)
            {
                double x = a + i * width;
                sum += func(x) * width;
            }

            return sum;
        }

        static double IntegrateWithSegments(double a, double b, int segments)
        {
            double segmentSize = (b-a) / segments;
            double areaSum = 0;
            for (int i = 0; i  < segments; i++)
            {
                areaSum += RectangleMethod(Function, i * segmentSize, i * segmentSize + segmentSize);
            }
            return areaSum;
        }

        static void Main(string[] args)
        {
            double a = 0.0;
            double b = 1.0;
            int n = 1000;

            double result = RectangleMethod(Function, a, b);
            Console.WriteLine($"Приближённое значение интеграла от {a} до {b}: {result}");
            Console.WriteLine($"Приближённое значение интеграла от {a} до {b}: {IntegrateWithSegments(a, b, 4)}");
            Console.ReadKey();
        }
    }
}
