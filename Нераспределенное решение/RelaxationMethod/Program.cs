using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RelaxationMethod
{
    internal class Program
    {

        static void Main(string[] args)
        {

            // Ограничение стандартное - 2Гб 

            /* 1) Нужно добавть в App.config для больших размеров.
            <runtime>
            <gcAllowVeryLargeObjects enabled = "true" />
            </runtime>

            2) В свойствах проекта убрать галочку "Предпочтительная 32-разрядная версия".
            */

            Environment.GetEnvironmentVariable("gcAllowVeryLargeObjects");
            Environment.GetEnvironmentVariable("COMPlus_gcAllowVeryLargeObjects");



            // float[,] AA = new float[50000, 50000];


            /*
            float[,] A = {

                { 4, -1, 0 }, // 1 
                { -1, 4, -1 }, // 2
                { 0, -1, 4 }
            };

            float[] B = { 3, 2, 3 };
            */

            // 1) Объявишь и создашь матрицу 50000 х 50000 и вектор свободных членов размера 50000.
            
            int n = 2000;
            float omega = 1.25f;
            float tolerance = 0.001f; // пробовать для 50000 = 0.01 !!!!!!
            // При n=500 достаточно 3000 итерация для достижения точности 0.000001.
            // При n=1000 достаточно 7000 итерация для достижения точности 0.000001.
            int maxIter = 1000;




            float[,] AA = new float[n, n];
            float[] BB = new float[n];

            // 2) Заполнишь вектоh свободных членов случайными числами от 1 до 1000.




            // 3) Заполниешь матрицу над главной диагональю случайными числами от 1 до 1000 и сразу ставишь равный симметричный элемент.
            /*
            * $ $ $
            $ * $ $
            * * * $
            * * * *
            */


            /*
             a11*x1 + a12*x2 + ... = b1
             ...
            */



            Random rnd = new Random();
            for(int i = 0; i < n; i++)
            {
                // Для матрицы 1000 х 1000 можно использовать формулу для заполнения: i * 10 + 1;
                // Для матрицы 30000 х 30000 можно использовать формулу для заполнения: i * 10 + 1;
                //BB[i] = i * 10 + 1;
                BB[i] = /*0.001f * (i + 1) / (i + 1000)*/n+i;
            }


            //for (int row = 0; row < n - 1; row++)
            //{
            //    for (int col = row + 1; col < n; col++)
            //    {

            //        // Для матрицы 1000 х 1000 можно использовать формулу для заполнения: row * col + 1;
            //        // Для матрицы 30000 х 30000 можно использовать формулу для заполнения: row * col + 1;
            //        //AA[row, col] = row * col + 1;
            //        AA[row, col] = 0.0001f / (Math.Abs(row - col) + 1);
            //        AA[col, row] = AA[row, col];
            //    }
            //    //Console.WriteLine(row);
            //}

            for (int row = 0; row < n; row++)
            {
                for(int col = 0; col < n; col++)
                {
                    AA[row, col] = 1;
                    AA[col, row] = AA[row, col];
                }
            }

            // 4) Заполнить диагональ случайными числами от 1 до 1000.
            for (int row = 0; row < n; row++)
            {
                // Для матрицы 1000 х 1000 можно использовать формулу для заполнения: (float)row * (float)row * 8.0 + 1.0;
                // Для матрицы 30000 х 30000 можно использовать формулу для заполнения: (float)row * (float)row * 8.0 + 1.0;

                /*AA[row, row] = (float)row * (float)row * 8.0f + 1.0f;*/           // Math.Sqrt, деление 
                AA[row, row] = row+1/*0.1f * (float)Math.Sqrt(row + 1) + 100.0f*/ ;
            }

            Console.WriteLine("Матрица подготовлена!");
           

            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // Достаточно MVC-проекта как распределенной системы? Сервер(MVC проект) - первый узел, Клиент (браузер) - это второй узел. 
            // А в консольном приложении получить время решения в нераспределенной системе.
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


            // Нераспределенное решение.
            RelaxationMethod relaxationMethod = new RelaxationMethod(AA, BB);
            Stopwatch stopwatch = new Stopwatch();
            if (relaxationMethod.IsSolved())
            {
                
                stopwatch.Start();
                var x = relaxationMethod.Solve(out int iter, omega, tolerance, maxIter);
                stopwatch.Stop();
                Console.WriteLine("Количество итераций: " + iter);
                Console.WriteLine("Максимальное допустимое кол-во итераций: " + maxIter);
                Console.WriteLine();
                Console.WriteLine("Вектор решений X: ");
                for(int i = 0;i < n; i++)
                {
                    Console.Write(Math.Round(x[i], 8).ToString("0.#####################") + " ");
                }
                Console.WriteLine();
                
                Console.WriteLine("Время решения: " + stopwatch.ElapsedMilliseconds + " мс");
                Console.WriteLine("Проверка по первой строке: ");
                Console.WriteLine("a11 * x1 + a12 * x2 + ... = b1");
                // Цикл перемножения первой строки с вектором найденного решения и получение суммы.
                //первая строка А: ******
                //строка решения X: ******

                float sum = 0.0f;
                for(int col = 0; col < n; col++)
                {
                     sum += AA[0, col] * x[col];
                }
                Console.WriteLine(sum.ToString("0.##########") + " = " + BB[0].ToString("0.##########") + " невязка = " + Math.Abs(BB[0] - sum).ToString("0.##########"));
                Console.WriteLine("Заданная точность =" + tolerance.ToString("0.##########"));
            }
            else Console.WriteLine("Решения нет");
            //


            
            AA = null;
            BB = null;
            GC.Collect(); // Принудительная сборка мусора.
            GC.WaitForPendingFinalizers(); // Ожидание завершения очистки [1].

        }
    }
}