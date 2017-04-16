namespace VSCodeTutorial.Common
{
    public class MathHelper
    {
        /// <summary>
        /// 阶乘，本例中暂不考虑 溢出的问题哦 Factorial(n) = n*（n-1）*(n-2)...*1;
        /// </summary>
        /// <param name="n">输入参数n</param>
        /// <returns></returns>
        public static int Factorial(int n){
            if(n <=0 ){
                throw new System.ArgumentOutOfRangeException("n","参数错误，不能小于等于零");
            }
            if(n == 1){
                return 1;
            }
            return n*Factorial(n-1);
        }
    }
}