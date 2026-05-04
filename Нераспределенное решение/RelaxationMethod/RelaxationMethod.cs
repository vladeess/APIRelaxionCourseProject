using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelaxationMethod
{

    // Метод гарантированно сходится,  (имеет решение) если матрица A:
    // - симметрична!!!!!
    // - положительно определена, или имеет диагональное преобладание (модуль элемента на главной диагонали больше (или равен) сумме модулей всех
    // остальных элементов в его строке).

    // Пример СЛАУ:
    // 4x1 - x2 + 0x3 = 3
    // -x1 + 4x2 - x3 = 2
    // 0x1 - x2 + 4x3 = 3



    // Симметрична!
    // Диагональное преоладание!


    public class RelaxationMethod
    {
        private readonly float[,] A;
        private readonly float[] B;



        public RelaxationMethod(float[,] A, float[] B)
        {
           this.A = A; 
           this.B = B;
        }


        public bool IsSolved()
        {
            return true;// IsSymmetric() && (IsPositive() || IsDiagonal());
        }


        // Метод гарантированно сходится,  (имеет решение) если матрица A:
        // - симметрична!!!!!
        // - положительно определена, или имеет диагональное преобладание (модуль элемента на главной диагонали больше (или равен) сумме модулей всех
        // остальных элементов в его строке).

        // Метод проверки схождения!!! То есть метод должен ответить, есть ли решение или нет.
        /*
        - симметричность - ДА
            - или положительность НЕТ или преобладание ДА

            ответ ДА и (НЕТ или ДА) = ДА

        Симметричность:
        bool sum = true;
        1. С первой строки до предпоследней
        2. Начинаем с row + 1 ячейки в строки до последней
        3. sum = sum && (A[row,col] == A[col,row]);


        . 0 * * 1 | row = 0 | col = 1 - col = row + 1 Мы нашли взаимосвязь номер ячейки, с которой начинаем, с номером строки в которой рассчитываем.
        0 . * 7 3 | row = 1 | col = 2 - col = row + 1
        * * . * 5 | row = 2 | col = 3 - col = row + 1
        * 7 * . * | row = 3 | col = 4 - col = row + 1
        1 3 5 * . | row = 4 | 
        */

        public bool IsSymmetric() //Метод проверки симметричности
        {
            bool result = true; // пусть предварительно все элементы симметричны
            for(int row = 0; row < A.GetLength(0) - 1; row++) // row - номер строки.
            {
                
                for(int col = row + 1; col < B.GetLength(0); col++) // col - номер ячейки в строке.
                {
                    // A[row,col] == A[col,row] - проверка на равенство симметричных элементов относительное диагонали, к примеру, A[1,2] == A[2,1]
                    result = result && (A[row,col] == A[col,row]); // && - приоритет выше остальных логических операндов!
                }
            }
            return result;
        }

        // диагональное преобладание(модуль элемента на главной диагонали больше (или равен) сумме модулей всех
        // остальных элементов в его строке).
        // Math.Abs() - модуль
        // 1) пройтись по каждой строке
        // 2) рассчитать сумму в каждой строке без диагонального элемента
        // 3) сравнить сумму с диагональным элементов
        // 4) все сравнения должны быть true - тогда диагональное преобладание.

   
        public bool IsDiagonal() 
        {
            bool result = true; //Пусть имеется диагональное преобладание - во всех строках модуль диагонального элемента больше или равен сумме модулей элементов
            for (int row = 0; row < A.GetLength(0); row++)// Проходимся по каждой строке (перебираем индексы строк)
            {
                float sum = 0.0f;//Пусть изначально сумма равна нулю
                for (int col = 0; col < A.GetLength(1); col++) //Проходимся по каждому элементу в строке row
                {
                    if (row != col)//Если это не диагональный элемент
                    {
                        sum += Math.Abs(A[row, col]);
                    }
                }

                result = result && (Math.Abs(A[row, row]) >= sum);  // && - приоритет выше остальных логических операндов!

            }
            return result;
        }



        //метод проверки положительной определенности матрицы
        public bool IsPositive()
        {
            bool result = true;//пускай все элементы предварительно положительные
            for(int row = 0; row < A.GetLength(0); row++)
            {
                for(int col = 0; col< A.GetLength(0); col++)
                {
                    result = result && (A[col, row] > 0); // && - приоритет выше остальных логических операндов!
                }
            }

            return result;
        }


        // Метод улучшает метод Зейделя, вводя параметр релаксации (1 <omega < 2) для ускорения сходимости.
        // omega - параметр релаксации (1 < omega < 2)
        // tolerance  - точность вычисления (eps)/
        public float[] Solve(out int iter, float omega, float tolerance, int maxIterations)
        {
            //Console.WriteLine("Start");

            int n = B.Length; // Размер матрицы (кол-во неизвестных), или размер вектора свободных членов.
            // Вектор неизвестных
            float[] x = new float[n]; // Начальное приближение.
            for (int i = 0; i < n; i++)
            {
                x[i] = 0;
            }
//
            Console.WriteLine("x ready");

            iter = -1; // УБрать потом!!!!!!!!!!!!!!!!!!!!!!!!!!!
            float maxDiff = tolerance + 1.0f;
            for (iter = 0; iter < maxIterations && maxDiff > tolerance; iter++)
            {
                maxDiff = 0;
                for (int i = 0; i < n; i++)
                {
                    float sum = 0;
                    for (int j = 0; j < n; j++)
                    {
                        if (i != j) sum += A[i, j] * x[j];
                    }

                    // Формула SOR: x_i^(k+1) = (1-w)x_i^k + (w/A_ii)*(b_i - sum(A_ij * x_j))
                    float nextX = (1 - omega) * x[i] + (omega / A[i, i]) * (B[i] - sum);

                    maxDiff = Math.Max(maxDiff, Math.Abs(nextX - x[i]));
                    x[i] = nextX;

                //Console.WriteLine(i);
                }
            }

            return x;
        }

    }
}