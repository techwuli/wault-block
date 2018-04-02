using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WaultBlock.Utils.Test
{
    [TestClass]
    public class RandomUtilsTest
    {
        [TestMethod]
        public void RandomNumberShouldWork()
        {
            var randomResult = RandomUtils.RandomNumber(3);
            Assert.IsTrue(randomResult >= 100);
            Assert.IsTrue(randomResult <= 999);
        }
    }
}
