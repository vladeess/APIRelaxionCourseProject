using APIRelaxion.Classes;

namespace TestAPI
{
    public class UnitTestCreateRelaxation
    {
        [Fact]
        public void TestCreateRelaxation()
        {
            // Подставить матрицы из консольного ...
            var relaxationMethod = new RelaxationMethod(new float[10][], new float[10]);
            Assert.NotNull(relaxationMethod);
        }
    }
}