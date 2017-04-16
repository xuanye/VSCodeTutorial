using System;
using VSCodeTutorial.Common;
using Xunit;

namespace VSCodeTutorial.UnitTest
{
    public class MathHelperTest
    {
        [Fact]
        public void TestFactorial()
        {

            //先测试一下边界的情况
            int zero = 0 ;
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => MathHelper.Factorial(zero));

            int one = 1;
            var oneResult = MathHelper.Factorial(one);
            Assert.Equal(1, oneResult);

            //再测一下正常的情况
            int five = 5;
            var fiveResult = MathHelper.Factorial(five);
            Assert.Equal(5*4*3*2*1, fiveResult);

            int ten = 10;
            var tenResult = MathHelper.Factorial(ten);
            Assert.Equal(10*9*8*7*6*5*4*3*2*1, tenResult);
        }
    }
}