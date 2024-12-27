using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IntegralGraphic
{
    public class Program
    {
        static double Function(double x)
        {
            return x * x;
        }

        static int processorCount = Environment.ProcessorCount;

        static double RectangleMethod(Func<double, double> func, double a, double b, double tolerance)
        {
            double previousArea = double.NaN; // начальное значение как недействительное
            double currentArea = 0;
            int n = 1;

            do
            {
                currentArea = CalculateArea(func, a, b, n);
                if (previousArea != double.NaN && Math.Abs(currentArea - previousArea) < tolerance)
                {
                    break; // прекращаем, если ошибка меньше допустимого
                }

                previousArea = currentArea;
                n *= 2; // увеличиваем количество разбиений
            } while (true);

            return currentArea;
        }

        private static double CalculateArea(Func<double, double> func, double a, double b, int n)
        {
            double width = (b - a) / n;
            double area = 0;

            for (int i = 0; i < n; i++)
            {
                double x = a + i * width;
                area += func(x) * width; // суммируем площади
            }

            return area;
        }

        double a = 0;
        double b = 10;
        static double tolerance = 0.000001;

        static double IntegrateWithSegments(double a, double b, int segments)
        {
            double segmentSize = (b-a) / segments;
            double areaSum = 0;
            for (int i = 0; i  < segments; i++)
            {
                areaSum += RectangleMethod(Function, i * segmentSize, i * segmentSize + segmentSize, tolerance);
            }
            return areaSum;
        }
        static double RectangleMethodParallel(double a, double b, int segments)
        {
            double segmentSize = (b - a) / segments;
            double sum = 0.0;
            object locker = new object();

            Parallel.For(0, segments, i =>
            {
                double tempSum = RectangleMethod(Function, a + i * segmentSize, a + (i + 1) * segmentSize, tolerance);

                lock (locker)
                {
                    sum += tempSum;
                }
            });

            return sum;
        }

        static double IntegrateThreads(double a, double b, int segments)
        {
            double segmentSize = (b - a) / segments;
            double areaSum = 0;
            object locker = new object();
            Thread[] threads = new Thread[segments];

            void IntegrateSegment(int index)
            {
                double tempSum = RectangleMethod(Function, a + index * segmentSize, a + (index + 1) * segmentSize, tolerance);
                lock (locker)
                {
                    areaSum += tempSum;
                }
            }

            for (int i = 0; i < segments; i++)
            {
                int tempIndex = i; // захватываем текущее значение индекса
                threads[i] = new Thread(() => IntegrateSegment(tempIndex));
                threads[i].Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join(); // ожидаем завершения всех потоков
            }

            return areaSum;
        }

        static void Main(string[] args)
        {
            double a = 0;
            double b = 10;
            double tolerance = 0.000001;

            double result = RectangleMethod(Function, a, b, tolerance);
            Console.WriteLine($"Приближённое значение интеграла от {a} до {b}: {result}");
            Console.WriteLine($"Значение интеграла с сегментами: {IntegrateWithSegments(a, b, 4)}");
            Console.WriteLine($"Значение интеграла с сегментами (Thread): {IntegrateThreads(a, b, processorCount)}");
            Console.WriteLine($"Значение интеграла с сегментами (Task): {RectangleMethodParallel(a, b, processorCount)}");
            Console.ReadKey();
        }
    }
}