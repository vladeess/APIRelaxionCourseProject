using APIRelaxion.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using APIRelaxion.Classes;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.Xml;

namespace APIRelaxion.Controllers
{
    public class HomeController : Controller
    {
        static float[]? BB = null;
        static float[][]? AA = null;
        static float[]? x = null;

        public HomeController()
        {

        }

        public IActionResult Index()
        {          
            
            return View(null); // x = null
        }


        [HttpPost]
        public IActionResult Index(float[][] A, float[] B)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            // Решить, получить х-вектор
            // передать через модель масив Х
            RelaxationMethod relaxation = new RelaxationMethod(A,B);

            // Проверяем существование решения. Если нет решения.
            if (!relaxation.IsSolved())
            {
                ViewBag.A = null;
                ViewBag.B = null;
                ViewBag.n = 0;
                ViewBag.Error = "Ошибка: СЛАУ не имеет решения и(или) метод релаксации расходится.";
                return View(null); // x = null
            }

            var x = relaxation.Solve(out int iter, 1.25f, 0.001f, 1000);
            stopwatch.Stop();
            ViewBag.iter = iter;
            ViewBag.omega = 1.25f;
            ViewBag.tolerance = 0.001f;
            ViewBag.maxIterations = 1000;
            ViewBag.leadtime = stopwatch.ElapsedMilliseconds;
            return View(x);
        }


        public IActionResult Dynamic(int n = 10)
        {
            //RelaxationMethod relaxation = new RelaxationMethod(A, B);
            var BB = new float[n];
            var AA = new float[n][];
            for (int i = 0; i < n; i++) AA[i] = new float[n];

            Random rnd = new Random();
            for (int i = 0; i < n; i++)
            {
                // Для матрицы 1000 х 1000 можно использовать формулу для заполнения: i * 10 + 1;
                // Для матрицы 30000 х 30000 можно использовать формулу для заполнения: i * 10 + 1;
                BB[i] = i * 10 + 1;
                //BB[i] = 0.001f * (i + 1) / (i + 1000);
            }


            for (int row = 0; row < n - 1; row++)
            {
                for (int col = row + 1; col < n; col++)
                {

                    // Для матрицы 1000 х 1000 можно использовать формулу для заполнения: row * col + 1;
                    // Для матрицы 30000 х 30000 можно использовать формулу для заполнения: row * col + 1;
                    AA[row][col] =  row * col + 1;
                    //AA[row][col] = 0.0001f / (Math.Abs(row - col) + 1);
                    AA[col][row] = AA[row][col];
                }
                //Console.WriteLine(row);
            }

            // 4) Заполнить диагональ случайными числами от 1 до 1000.
            for (int row = 0; row < n; row++)
            {
                // Для матрицы 1000 х 1000 можно использовать формулу для заполнения: (float)row * (float)row * 8.0 + 1.0;
                // Для матрицы 30000 х 30000 можно использовать формулу для заполнения: (float)row * (float)row * 8.0 + 1.0;

                AA[row][row] =  (float)row * (float)row * 8.0f + 1.0f;           // Math.Sqrt, деление 
                //AA[row][row] = 0.1f * (float)Math.Sqrt(row + 1) + 100.0f;
            }

            ViewBag.A = AA;
            ViewBag.B = BB;
            ViewBag.n = n;
            //ViewBag.iter = ;
            //ViewBag.tolerance = tolerance;
            //ViewBag.omega = omega;
            //ViewBag.maxIterations = maxIterations;
            return View(null); // x = null
        }


        [HttpPost]
        public IActionResult Dynamic(float[][] A, float[] B)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            // Решить, получить х-вектор
            // передать через модель масив Х
            RelaxationMethod relaxation = new RelaxationMethod(A,B);

            // Проверяем существование решения. Если нет решения.
            if (!relaxation.IsSolved())
            {   
                ViewBag.A = null;
                ViewBag.B = null;
                ViewBag.n = 0;
                ViewBag.Error = "Ошибка: СЛАУ не имеет решения и(или) метод релаксации расходится.";
                return View(null); // x = null
            }

            var x = relaxation.Solve(out int iter, 1.25f, 0.0001f, 1000);
            stopwatch.Stop();
            ViewBag.A = A;
            ViewBag.B = B;
            ViewBag.n = B.Length;
            ViewBag.iter = iter;
            ViewBag.omega = 1.25f;
            ViewBag.tolerance = 0.0001f;
            ViewBag.maxIterations = 1000;
            ViewBag.leadtime = stopwatch.ElapsedMilliseconds;
            return View(x);
        }

        [HttpPost]
        public IActionResult Big(int row1 , int row2, int col1, int col2)
        {

            if (AA == null)
            {
                ViewBag.n = 0;
                return View(null);
            }

            if (row1 < 0 || row2 < 0 || col1 < 0 || col2 < 0)
            {
                ViewBag.Error = "Ошибка: введите корректные значения";
                ViewBag.row1 = -1;
                ViewBag.row2 = -1;
                ViewBag.col1 = -1;
                ViewBag.col2 = -1;
                return View();
            }

            ViewBag.n = BB.Length;

            ViewBag.omega = 1.25f;
            ViewBag.tolerance = 0.1f;
            ViewBag.maxIterations = 1000;
            ViewBag.A = AA;
            ViewBag.B = BB;
            ViewBag.row1 = row1;
            ViewBag.row2 = row2;
            ViewBag.col1 = col1;
            ViewBag.col2 = col2;

            return View(x);
        }



        // Получение при помощи GET-метода
        // [HttpGet] по умолчанию
        public IActionResult Big(int n = 0, int row1 = 0, int row2 = 0, int col1 = 0, int col2 = 0)
        {
            if (n == 0)
            {
                ViewBag.n = 0;
                return View(null);
            }

            if (n > 50000)
            {
                ViewBag.Error = "Ошибка: максимальный размер матрицы 50000";
                ViewBag.n = 0;
                return View();
            }

           



            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Используем статические поля.
            BB = new float[n];
            AA = new float[n][];
            for (int i = 0; i < n; i++) AA[i] = new float[n];

            Random rnd = new Random();



            // Заведомо строим СЛАУ, чтобы решение х содержало все 1.

            // 3 x 3:
            // 1 * x1 + 1 * x2 + 1 * x3 = 3
            // 1 * x1 + 2 * x2 + 1 * x3 = 4
            // 1 + x1 + 1 * x3 + 3 * x3 = 5

            // 50000 x 50000:
            // 1 * x1 + 1 * x2 + 1 * x3 + ... 1 * x50000 = 50000
            // 1 * x1 + 2 * x2 + 1 * x3 + ... 1 * x50000 = 50001
            //
            // 1 + x1 + 1 * x3 + 1 * x3 + 50000 * x50000 = 50000 + 50000

            for (int i = 0; i < n; i++)
            {
                BB[i] = n + i;
            }


            for(int i = 0; i <  n; i++)
            {
                Array.Fill(AA[i], 1);
            }

            for (int row = 0; row < n; row++)
            {
                AA[row][row] = row + 1;
            }

            RelaxationMethod relaxation = new RelaxationMethod(AA, BB);



            // Проверяем существование решения. Если нет решения.
            
            if (!relaxation.IsSolved())
            {
                ViewBag.A = null;
                ViewBag.B = null;
                ViewBag.n = 0;
                ViewBag.Error = "Ошибка: СЛАУ не имеет решения и(или) метод релаксации расходится.";
                return View(null); // x = null
            }
            

            x = relaxation.Solve(out int iter, 1.25f, 0.1f, 1000);
            stopwatch.Stop();


            ViewBag.n = n;
            ViewBag.iter = iter;

            ViewBag.omega = 1.25f;
            ViewBag.tolerance = 0.1f;
            ViewBag.maxIterations = 1000;
            ViewBag.A = AA;
            ViewBag.B = BB;
            ViewBag.row1 = row1;
            ViewBag.row2 = row2;
            ViewBag.col1 = col1;
            ViewBag.col2 = col2;
            ViewBag.leadtime = stopwatch.ElapsedMilliseconds;
            return View(x); // x = null
        }

        public IActionResult TaskAndAuthor()
        {
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
