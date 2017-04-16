using System;
using System.Text;
using VSCodeTutorial.Common;

namespace VSCodeTutorial.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8; // 设置控制台编码
            while(true)
            {
                Console.WriteLine("请输入一个小于10的数字,回车结束:");
                string input_str = Console.ReadLine();
                if(int.TryParse(input_str ,out var input_int))
                {
                    if(input_int>0 && input_int<=10){
                       int result =  MathHelper.Factorial(input_int);
                       Console.WriteLine("你输入的数字是{0},它的阶乘结果是{1},退出请按ctrl+c,按其他键再试一次",input_int,result);
                       Console.ReadKey();
                    }
                }
                else{
                    Console.WriteLine("输入的字符不是有效的数字");
                }
            }

        }
    }
}
