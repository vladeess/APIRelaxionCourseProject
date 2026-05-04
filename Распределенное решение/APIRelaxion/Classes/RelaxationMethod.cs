using OperateNode.Classes;
using OperateNode.enums;
using System.Net;
using System.Net.Sockets;

namespace APIRelaxion.Classes
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
        // СЛАУ.
        private int n;
        private readonly float[][] A;
        private readonly float[] B;

        // Объекты синхронизации потоков.
        int comp = 0; // Количество выполненных (завершенных) процессов.
        object obj = new object(); // Критическая секция.
        private float[] x; // Вектор решения.

        public RelaxationMethod(float[][] A, float[] B)
        {
            this.A = A;
            this.B = B;
        }


        public bool IsSolved()
        {
            return IsSymmetric() && (IsPositive() || IsDiagonal());
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
            for (int row = 0; row < A.GetLength(0) - 1; row++) // row - номер строки.
            {

                for (int col = row + 1; col < B.GetLength(0); col++) // col - номер ячейки в строке.
                {
                    // A[row,col] == A[col,row] - проверка на равенство симметричных элементов относительное диагонали, к примеру, A[1,2] == A[2,1]
                    result = result && (A[row][col] == A[col][row]); // && - приоритет выше остальных логических операндов!
                }
            }
            ;
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
            int n = A.GetLength(0);
            bool result = true; //Пусть имеется диагональное преобладание - во всех строках модуль диагонального элемента больше или равен сумме модулей элементов
            for (int row = 0; row < n; row++)// Проходимся по каждой строке (перебираем индексы строк)
            {
                float sum = 0.0f;//Пусть изначально сумма равна нулю
                for (int col = 0; col < n; col++) //Проходимся по каждому элементу в строке row
                {
                    if (row != col)//Если это не диагональный элемент
                    {
                        sum += Math.Abs(A[row][col]);
                    }
                }

                result = result && (Math.Abs(A[row][row]) >= sum);  // && - приоритет выше остальных логических операндов!

            }
            ;
            return result;
        }



        //метод проверки положительной определенности матрицы
        public bool IsPositive()
        {
            int n = A.GetLength(0);
            bool result = true;//пускай все элементы предварительно положительные
            for (int row = 0; row < n; row++)
            {
                for (int col = 0; col < n; col++)
                {
                    result = result && (A[col][row] > 0); // && - приоритет выше остальных логических операндов!
                }
            }
            ;

            return result;
        }


        // Метод пересылки данных и получения ответа.
        void SendMessageFromSocket(int port, byte[] sendBytes, ref byte[] getBytes)
        {
            // Соединяемся с удаленным устройством           
            // Устанавливаем удаленную точку для сокета
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            sender.Connect(ipEndPoint);

            // Отправляем данные через сокет
            int bytesSent = sender.Send(sendBytes);

            // Получаем ответ
            int bytesRec = sender.Receive(getBytes);

            // Освобождаем сокет
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }





        // Метод улучшает метод Зейделя, вводя параметр релаксации (1 <omega < 2) для ускорения сходимости.
        // omega - параметр релаксации (1 < omega < 2)
        // tolerance  - точность вычисления (eps)/
        public float[] Solve(out int iter, float omega, float tolerance, int maxIterations)
        {

            n = B.Length; // Размер матрицы (кол-во неизвестных), или размер вектора свободных членов.
            
            byte[] data = new byte[n * sizeof(float)];
            byte[] command = new byte[Constants.COMMAND_SIZE];
            byte[] answer = new byte[Constants.COMMAND_SIZE];

            // Пересылаем матрицу А.
            command[0] = (byte)CommandRequest.SEND_ROW_A; // Команда пересылки строки матрицы А.

            for (int i = 0; i < n; i++)
            {
                SendMessageFromSocket(11001, command, ref answer); // answer[0] должна быть = OK.
                Buffer.BlockCopy(A[i], 0, data, 0, n * sizeof(float)); // Строку матрицы в массив байт.
                SendMessageFromSocket(11001, data, ref answer); // answer[0] должна быть = OK.
            }

            for (int i = 0; i < n; i++)
            {
                SendMessageFromSocket(11002, command, ref answer); // answer[0] должна быть = OK.
                Buffer.BlockCopy(A[i], 0, data, 0, n * sizeof(float)); // Строку матрицы в массив байт.
                SendMessageFromSocket(11002, data, ref answer); // answer[0] должна быть = OK.
            }

            // Пересылаем вектор В.
            command[0] = (byte)CommandRequest.SEND_B; // Команда пересылки вектора В.

            SendMessageFromSocket(11001, command, ref answer); // answer[0] должна быть = OK.
            Buffer.BlockCopy(B, 0, data, 0, n * sizeof(float)); // Строку матрицы в массив байт.
            SendMessageFromSocket(11001, data, ref answer); // answer[0] должна быть = OK.

            SendMessageFromSocket(11002, command, ref answer); // answer[0] должна быть = OK .
            Buffer.BlockCopy(B, 0, data, 0, n * sizeof(float)); // Строку матрицы в массив байт.
            SendMessageFromSocket(11002, data, ref answer); // answer[0] должна быть = OK.

            // Вектор неизвестных
            x = new float[n]; // Начальное приближение.
            Array.Clear(x); // Заполняет нулями.
            var lastX = new float[n]; // Предыдущее значение.

            var maxDiff = tolerance + 1.0f;
            var coeff = 1.0f - omega; // (1 - omega) 

            for (iter = 0; iter < maxIterations && maxDiff > tolerance; iter++)
            {
                // Пересылаем вектор x.
                command[0] = (byte)CommandRequest.SEND_X; // Команда пересылки вектора x.

                SendMessageFromSocket(11001, command, ref answer); // answer[0] должна быть = OK.
                Buffer.BlockCopy(x, 0, data, 0, n * sizeof(float)); // Строку матрицы в массив байт.
                SendMessageFromSocket(11001, data, ref answer); // answer[0] должна быть = OK.

                SendMessageFromSocket(11002, command, ref answer); // answer[0] должна быть = OK.
                Buffer.BlockCopy(x, 0, data, 0, n * sizeof(float)); // Строку матрицы в массив байт.
                SendMessageFromSocket(11002, data, ref answer); // answer[0] должна быть = OK.

                // Запоминаем предыдущее значение.
                Array.Copy(x, lastX, n);

                comp = 0; // Количество завершённых процессов вычислительными узлами.
                Thread thread1 = new Thread(ThreadFun1);
                Thread thread2 = new Thread(ThreadFun2);
                thread1.Start();
                thread2.Start();

                bool exit = false;

                // Ожидание завершение потоков.
                // Синхронизация потоков при помощи критической секции
                // и общей переменной comp.
                while (!exit)
                {
                    lock (obj)
                    {
                        if (comp == 2) exit = true;
                    }
                }

                // Наибольшее расхождение.
                maxDiff = 0;
                for (int i = 0; i < n; i++)
                {
                    maxDiff = Math.Max(maxDiff, Math.Abs(lastX[i] - x[i]));
                }

            }
            return x;
        }

        // Метод потока 1.
        void ThreadFun1()
        {
            byte[] command = new byte[Constants.COMMAND_SIZE]; // Массив команды (буфер).
            byte[] data = new byte[n * sizeof(float)]; // Массив данных (в байтах).
            command[0] = (byte)CommandRequest.SOLVE_AND_GET_X; // Команда расчёта и получения вектора x.
            SendMessageFromSocket(11001, command, ref data); // Посылка команды и получение ответа.
            
            lock (obj)
            {
                Buffer.BlockCopy(data, 0, x, 0, n / 2 * sizeof(float));
                comp++;
            }
        }

        // Метод потока 2.
        void ThreadFun2()
        {
            byte[] command = new byte[Constants.COMMAND_SIZE]; // Массив команды (буфер).
            byte[] data = new byte[n * sizeof(float)]; // Массив данных (в байтах).
            command[0] = (byte)CommandRequest.SOLVE_AND_GET_X; // Команда расчёта и получения вектора x.
            SendMessageFromSocket(11002, command, ref data); // Посылка команды и получение ответа.           

            lock (obj)
            {
                Buffer.BlockCopy(data, n / 2 * sizeof(float), x, n / 2 * sizeof(float), n / 2 * sizeof(float));
                comp++;
            }
        }
    }
}