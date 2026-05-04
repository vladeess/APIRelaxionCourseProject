using System;
using System.Net;
using System.Net.Sockets;
using System.Xml.Schema;
using OperateNode.Classes;
using OperateNode.enums;

// Это вычислительный узел - фактически это сервер, к которому должен подключиться клиент.
namespace OperateNode
{
    internal class Program
    {
        static int iterNumber = 0;
        static int N = -1; // Размер СЛАУ. Становится известным с пересылкой первого вектора.
        static int rowIndex = 0; // Номер строки, получения матрицы.

        // Данные в байтах (максимально).
        static byte[] data = new byte[50000 * sizeof(float)];

        static float[][] A; // Матрица коэффициентов.
        static float[] B; // Вектор свободных членов.
        static float[] x; // Вектор неизвестных.


        static byte[] command = new byte[Constants.COMMAND_SIZE]; // Команда пересылки.
        static byte[] answer = new byte[Constants.COMMAND_SIZE]; // Команда пересылки.

        static void Main(string[] args)
        {
            Console.WriteLine("Получение параметров программы ...");
            var UZEL = int.Parse(args[0]);
            var port = 11000 + UZEL;
            Console.WriteLine("Номер вычислительного узла: " + UZEL);
            Console.WriteLine("Порт прослушки: " + port);

            // Устанавливаем для сокета локальную конечную точку
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            // Создаем сокет Tcp/Ip
            Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // Назначаем сокет локальной конечной точке и слушаем входящие сокеты            	
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);

                // Начинаем слушать соединения
                while (true)
                {
                    // Получаем данные.
                    Console.WriteLine("Ожидаем соединение через порт " + port);
                    Socket handler = sListener.Accept();
                    int bytesRec = handler.Receive(command); // Получаем команду.                 

                    // Преобразуем команду в перечисление.
                    var commandRequest = (CommandRequest)command[0];

                    switch (commandRequest)
                    {
                        case CommandRequest.SEND_ROW_A: // Пересылка строки матрицы.
                            iterNumber = 0;
                            Console.WriteLine("Получена команда пересылки строки " + rowIndex + " матрицы А.");

                            // Ответ.
                            answer[0] = (byte)CommandAnswer.OK;
                            handler.Send(answer);
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                            Console.WriteLine("Успешный ответ передан");

                            // Получаем данные строки матрицы А.
                            handler = sListener.Accept();
                            bytesRec = handler.Receive(data);

                            // Если первая пересылка.
                            if (rowIndex == 0)
                            {
                                Program.N = bytesRec / sizeof(float);
                                A = new float[Program.N][];
                            }
                            A[rowIndex] = new float[Program.N];
                            Buffer.BlockCopy(data, 0, A[rowIndex], 0, bytesRec);
                            Console.WriteLine("Данные строки " + rowIndex + " матрицы А получены");
                            /*
                            for (int i = 0; i < Program.N; i++)
                            {
                                Console.Write(A[rowIndex][i] + " ");
                            }
                            Console.WriteLine();
                            */
                            rowIndex++; // В следующий раз будет получаться следующая строка.

                            // Ответ.
                            answer[0] = (byte)CommandAnswer.OK;
                            handler.Send(answer);
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                            Console.WriteLine("Успешный ответ передан");
                            break;

                        case CommandRequest.SEND_B: // Пересылка вектора свободных членов B.

                            // Контрольный вывод.
                            Console.WriteLine("Матрица А:");
                            rowIndex = 0;// Значит, матрица А уже получена. Сбрасываем счётчик строк.
                            for (int i = 0; i < Program.N; i++)
                            {
                                for (int j = 0; j < Program.N; j++)
                                {
                                    Console.Write(A[i][j] + " ");
                                }
                                Console.WriteLine();
                            }

                            Console.WriteLine("Получена команда пересылки вектора B.");

                            // Ответ.
                            answer[0] = (byte)CommandAnswer.OK;
                            handler.Send(answer);
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                            Console.WriteLine("Успешный ответ передан");

                            // Получаем данные вектора В.
                            handler = sListener.Accept();
                            bytesRec = handler.Receive(data);
                            B = new float[Program.N];
                            Buffer.BlockCopy(data, 0, B, 0, bytesRec);
                            Console.WriteLine("Данные вектора В получены");
                            /*
                            for (int i = 0; i < Program.N; i++)
                            {
                                Console.Write(B[i] + " ");
                            }
                            Console.WriteLine();
                            */
                            // Ответ.
                            answer[0] = (byte)CommandAnswer.OK;
                            handler.Send(answer);
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                            Console.WriteLine("Успешный ответ передан");
                            break;

                        case CommandRequest.SEND_X: // Пересылка x.
                            Console.WriteLine("Получена команда пересылки вектора x.");

                            // Ответ.
                            answer[0] = (byte)CommandAnswer.OK;
                            handler.Send(answer);
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                            Console.WriteLine("Успешный ответ передан");

                            // Получаем данные вектора x.
                            handler = sListener.Accept();
                            bytesRec = handler.Receive(data);
                            x = new float[Program.N];
                            Buffer.BlockCopy(data, 0, x, 0, bytesRec);
                            Console.WriteLine("Данные вектора x получены");
                            /*
                            for (int i = 0; i < Program.N; i++)
                            {
                                Console.Write(x[i] + " ");
                            }
                            Console.WriteLine();
                            */
                            // Ответ.
                            answer[0] = (byte)CommandAnswer.OK;
                            handler.Send(answer);
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                            Console.WriteLine("Успешный ответ передан");
                            break;

                        case CommandRequest.SOLVE_AND_GET_X: // Команда расчёта части x и возврата её.

                            iterNumber++; // Номер итерации.
                            Console.WriteLine("ИТЕРАЦИЯ " + iterNumber);
                            Console.WriteLine("Получена команда расчёта части x.");

                            // Расчёт                            
                            var omega = 1.25f;
                            float coeff = 1.0f - omega; // (1 - omega) 

                            if (UZEL == 1)
                            {
                                for (int i = 0; i < N / 2; i++)
                                {
                                    float sum = 0.0f;

                                    for (int j = 0; j < i; j++)
                                    {
                                        sum += A[i][j] * x[j];
                                    }

                                    for (int j = i + 1; j < N; j++)
                                    {
                                        sum += A[i][j] * x[j];
                                    }

                                    float nextX = coeff * x[i] + omega / A[i][i] * (B[i] - sum);
                                    x[i] = nextX;

                                    Console.WriteLine("sum=" + sum + ", x[" + i + "]=" + x[i]);
                                }
                                
                                
                                
                            }

                            if (UZEL == 2)
                            {

                                for (int i = N / 2; i < N; i++)
                                {
                                    float sum = 0.0f;

                                    for (int j = 0; j < i; j++)
                                    {
                                        sum += A[i][j] * x[j];
                                    }

                                    for (int j = i + 1; j < N; j++)
                                    {
                                        sum += A[i][j] * x[j];
                                    }

                                    float nextX = coeff * x[i] + omega / A[i][i] * (B[i] - sum);
                                    x[i] = nextX;
                                    Console.WriteLine("sum=" + sum + ", x[" + i + "]=" + x[i]);
                                }
                                
                                
                                
                            }
                            Console.WriteLine("Рассчитана часть вектора x");


                            // Отправляем весь вектор x.
                            var dataX = new byte[Program.N * sizeof(float)];
                            Buffer.BlockCopy(x, 0, dataX, 0, Program.N * sizeof(float));
                            handler.Send(dataX);
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                            Console.WriteLine("Вектор x передан (часть его рассчитана).");
                            break;

                        default:
                            throw new Exception("Команда запроса не определена!");
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }

        }
    }
}