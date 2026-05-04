
using APIRelaxion.Classes;
using APIRelaxion.Controllers;

namespace TestAPI
{
    public class ClassTestController
    {
        [Fact]
        public void TestCreateController()
        {
            // чтобы экземпляр контроллера создавался ...
            var homecontroller = new HomeController();
            Assert.NotNull(homecontroller);
        }
    }
}
