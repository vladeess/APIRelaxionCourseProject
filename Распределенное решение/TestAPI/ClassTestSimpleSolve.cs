using APIRelaxion.Classes;
using System.Security.Cryptography.Xml;

namespace TestAPI
{
    public class ClassTestSimpleSolve
    {
        [Fact]
        public void TestSimpleSolve()
        {
            // из консольного приложения взять простой пример СЛАУ
            // решить
            // проверить правильность решения
            //.Solve(...)
            //получаешь x
            //Assert.True(Math.Abs(x[0]-точное значение) < 0.01 && x[1] ...)

            int n = 100;
            float[][] A = new float[n][];
            float[] B = new float[n];
            for (int i = 0; i < n; i++) A[i] = new float[n];
            for (int i = 0; i < n; i++)  B[i] = n + i;



            for (int i = 0; i < n; i++)
                Array.Fill(A[i], 1);

            for (int row = 0; row < n; row++)
            {
                A[row][row] = row + 1;
            }

            float omega = 1.25f;
            float tolerance = 0.01f;
            int maxIter = 1000;

            RelaxationMethod relaxationMethod = new RelaxationMethod(A, B);

            Assert.True(relaxationMethod.IsSolved());

            var x = relaxationMethod.Solve(out int iter, omega, tolerance, maxIter);

            Assert.True(Math.Abs(x[1] - 1.0f) < tolerance &&
                Math.Abs(x[2] - 1.0f) < tolerance);
        }
    }
}