使用VsCode编写和调试.NET Core项目
---

​	本来我还想介绍以下VSCode或者donet core，但是发现都是废话，没有必要，大家如果对这个不了解可以直接google这两个关键字，或者也根本不会看我这边文章。

​	好直接进入主题了，本文的前提条件：

1. 已经安装好了.NET Core SDK
2. 已经安装了VSCode



## 0x00. 磨刀不误砍柴工

使用VSCode编写dotnet core项目除了其 默认的功能外，我推荐还要安装一些非常有特色，并且有用的扩展，正是因为VSCode的插件机制，才让它变得更加强大，满足我们各式各样的需求

[C#语言扩展]: https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp
这个是使用VSCode编写C#代码必须的，安装之后在默认打开.cs文件时 还会自动下载调试器等（不过过程可能比较慢，在墙外的原因）
[C# XML注释]: https://marketplace.visualstudio.com/items?itemName=k--kato.docomment
这个可以插件可以快速的帮你添加注释，选择安装吧
[C# Extensions]: https://marketplace.visualstudio.com/items?itemName=jchannon.csharpextensions
这个插件，强烈推荐，可以帮你在建立文件的时候初始化文件内容包括对应的命名空间等

还有一些其他辅助类的，比如EditorConfig,Guildes,One Dark Theme,Project Manager ,Setting Sync等。


##  0x01. 新建多项目解决方案

打开命令行工具，在命令行工具中输入:

```shell
$:> dotnet new sln -o vscode_tutorial //在当前目录下 创建名为vscode_tutorial
```

以上命令使用dotnet sdk，新建一个解决方案文件，你可以不用命令行手动创建，但是使用`dotnet new` 可以更加方便的创建dotnet core相关的项目. 顺便提一下使用dotnet new 命令可以新建类库项目，控制台项目，网站项目等等，详细使用可以使用`dotnet help new` 命令来查看，如下图所示：

![](http://ww1.sinaimg.cn/large/697065c1gy1feoku81gjlj20s20g9jsi.jpg)

建完解决方案我们要来建立项目了，包括一个控制台项目，一个类库项目和一个单元测试项目

首先建立一个公共的类库项目用于存放我们的业务方法（假设我们在做一个真实的项目）

```shell
$:> dotnet new classlib -o VSCodeTutorial.Common //在当前目录下新建类库项目VSCodeTutorial.Common
$:> dotnet sln add VSCodeTutorial.Common/VSCodeTutorial.Common.csproj //将项目添加到解决方案中
```

通过同样的方式，我们建立好控制台项目和单元测试项目

```shell
$:> dotnet new console -o VSCodeTutorial.ConsoleApp
$:> dotnet sln add VSCodeTutorial.ConsoleApp/VSCodeTutorial.ConsoleApp.csproj
$:> dotnet new xunit -o VSCodeTutorial.UnitTest
$:> dotnet sln add VSCodeTutorail.UnitTest/VSCodeTutorial.UnitTest.csproj
```

这里要注意控制的模板名称叫`console`而单元测试我们使用`xunit`

这个时候我们的项目结构已经建立完成了，我们用VsCode来打开当前目录来看看完成的项目结构吧，如下图所示

![](http://ww1.sinaimg.cn/large/697065c1gy1feokuqo7bmj20da0jldgj.jpg)



## 0x02. 添加项目间的依赖关系
使用VsCode打开项目文件VSCodeTutorial.ConsoleApp.csproj,在其中添加对Common项目的引用
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
 <!--添加项目引用-->
  <ItemGroup>
    <ProjectReference Include="..\VSCodeTutorial.Common\VSCodeTutorial.Common.csproj" />
  </ItemGroup>
</Project>

```

同样打开VSCodeTutorial.UnitTest.csproj项目文件，在其中添加对Common项目的引用
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
<!--nuget 上的类库引用-->
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="15.0.0" />
    <PackageReference Include="xunit" Version="2.2.0" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.2.0" />
  </ItemGroup>
<!--本地项目引用-->
 <ItemGroup>
    <ProjectReference Include="..\VSCodeTutorial.Common\VSCodeTutorial.Common.csproj" />
  </ItemGroup>
</Project>
```
和上面的那个项目不同，这里有一些额外的依赖，这里可以刚好了解下，如果添加nuget中包的依赖，只需像上面一样使用```PackageReference```并填入类库名称和版本号即可

添加完依赖后，我们在根目录下使用```dotnet restore```来初始化以下,也可以再用`dotnet build`命令来尝试编译一下先




项目依赖关系如图2：

![](http://ww1.sinaimg.cn/large/697065c1gy1feol1juewrj20kq07r3yp.jpg)

## 0x03. 开始编写代码

​	这个项目的整体需求：我需要打开一个控制台程序，运行时需要用户输入一个小于50的整数，控制台接收到这个数字后计算出这个数字的阶乘，并把结果输出到控制台上。

​	经过简单的思考，我决定把阶乘的实现放到Common项目中，并且对其进行单元测试，测试的代码则放到UnitTest项目中

![](http://ww1.sinaimg.cn/large/697065c1gy1feol1juewrj20kq07r3yp.jpg)

首先我们把之前生成的项目中不需要的文件给删除掉VsCodeTutorial.Common中的Class1.cs和VSCodeTutorial.UnitTest中的UnitTest1.cs ，当然你也可以留着。

第一步，我们在`VsCodeTutorial.Common`项目中新建文件`MathHelper.cs`并在文件中添加如下代码,实现我们的阶乘，代码比较简单就不详述了。

```C#
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
```

第二步，我们要来测试这个代码，看看是否达到了我们的目标，在`VSCodeTutorial.UnitTest`项目中新建文件`MathHelpTest.cs`向文件中添加测试`Factorial`函数的方法，如下所示：

```C#
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
```



## 0x04 使用命令行运行单元测试

​	在使用配置VSCode之前 我还是建议大家先使用命令行来运行一下单元测试，这有利于更好的理解配置内容。

在根目录下输入命令:`dotnet test ./VSCodeTutorial.UnitTest/VSCodeTutorial.UnitTest.csproj` 查看运行结果：

![](http://ww1.sinaimg.cn/large/697065c1gy1feomgnvhrjj20sb0ae3zf.jpg)

很差劲会出现编码错误，而且这个错误暂时还没有办法解决..但是我猜单元测试通过了，这个问题相信在后续的版本中肯定会得到解决，事实上在Console项目中是可以解决输出乱码问题的。不过可喜的是在VSCode中运行单元测试是没有乱码的问题的😁。

## 0x05 使用VSCode 运行单元测试

首先当你打开项目的时候，VSCode 可能已经建议你配置一下相关的内容，如下图所示:

![](http://ww1.sinaimg.cn/large/697065c1gy1feomnmm2kwj21ah03bt96.jpg)



选择Yes, 会帮你新建这个一个目录和两个文件，luanch.json是用来执行调试程序的配置，而tasks.json则是配置各种任务的，其中运行单元测试就是一种任务。

![](http://ww1.sinaimg.cn/large/697065c1gy1feomqq26tyj207u09m74l.jpg)



首先我们打开`tasks.json` ，默认已经添加好了一个任务，如下所示

```json
{
    "version": "0.1.0",
    "command": "dotnet", //全局命令，即所有的任务都使用这个命令，也可以在各个任务中设置
    "isShellCommand": true,
    "args": [],
    "tasks": [
        {
            "taskName": "build", //任务名称 当设置了主的command 之后这个taskName也会作为一个命令参数
            "args": [
                "${workspaceRoot}\\VSCodeTutorial.ConsoleApp\\VSCodeTutorial.ConsoleApp.csproj"
            ],
            "isBuildCommand": true, //一个解决方案只能设置一个编译任务，多设置了也是白搭，当然也能执行，只是不能利用快捷方式运行了
            "problemMatcher": "$msCompile"//C#项目的problemMatcher
        }
    ]
}
```

默认使用了全局命令行，这样可以在任务中省去配置dotnet命令，但是如果你的解决方案中包括多个项目需要不同的命令行编译方式，如果前端网站使用grunt打包资源，那么顶部应该留空，而在各个子任务中配置command。还有如果存在多个编译项目时（如客户端和服务端在一个解决方案时），也应该把command配置在子任务中，并设置个性化的taskName以便区别，所以我推荐把command设置在任务中，下面我们修改一下以上代码，并添加一个运行单元测试的人。

```json
{
    "version": "0.1.0",
    "isShellCommand": true,
    "args": [],
    "tasks": [
        {
            "taskName": "build_console",
            "command":"dotnet"
            "args": [
                "build", //组成dotnet build
                //设置需要编译的项目，如果存在多个启动项目可以设置成解决方案文件（.sln）,这里只有一个项目所以设置运行项目也可以
                "${workspaceRoot}\\VSCodeTutorial.ConsoleApp\\VSCodeTutorial.ConsoleApp.csproj"
            ],
            "isBuildCommand": true, //设置是否编译项目
            "problemMatcher": "$msCompile"
        },
        {
            "taskName": "UnitTest",
            "command":"dotnet",
            "args": [
                "test",//组成dotnet test 命令
                "${workspaceRoot}\\VSCodeTutorial.UnitTest\\VSCodeTutorial.UnitTest.csproj"
            ],
            "isTestCommand": true,//设置为单元测试项目
            "problemMatcher": "$msCompile"
        }
    ]
}
```

上面的代码中，我将command命令移到了任务中，并给每个任务起了一个好识别的名字，现在这里一个有2个任务了

第一个任务`build_console`  运行时 会编译`VSCodeTutorial.ConsoleApp`项目及其依赖的项目

第二个任务`UnitTest`则是单元测试项目，运行`dotnet test`命令，这里有个特殊的设置就是`"isTestCommand": true` 标识为测试项目后可以通过快捷方式运行该命令



任务建好了，我们来运行任务把，windows按下 ctrl+shift+p,在弹出的对话框中输入:task 过滤命令可以得到以下的选项

![](http://ww1.sinaimg.cn/large/697065c1gy1feotyfpchzj20ig0aqmy1.jpg)



选择`任务:运行测试任务` 这条来运行我们之前编写好的单元测试项目，可以看到运行成功的情况，如下图所示

![](http://ww1.sinaimg.cn/large/697065c1gy1feou0lpbsej20y708fzlr.jpg)

这里中文显示正常，没有乱码哦，但是我不知道是什么原因..就是这么神奇

对于经常执行的任务，可以通过设置键盘快捷方式来方便调用，可以看到我分别设置了ctrl+shift+t 运行测试任务ctrl+shift+b 运行编译任务，ctrl+shift+r 启动选择任务，大家可以根据自己的喜好来设置。

## 0x06 开始编写控制台代码

打开VSCodeTutorial.ConsoleApp项目中的Program.cs文件，修改其中的代码，如下所示

```c#
using System;
using VSCodeTutorial.Common;

namespace VSCodeTutorial.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
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
```

代码比较 简单，就不做解释了，我们直接来看运行的结果，这里顺便提一下啊，在我们之前做的众多工作之后，我们这里编写代码有美美哒的智能提示哦，如下图所示：

![](http://ww1.sinaimg.cn/large/697065c1gy1feoudwzdenj20qe0gu40h.jpg)

好，再根目录下输入以下命令运行ConsoleApp

```shell
$:> dotnet run -p ./VSCodeTutorial.ConsoleApp/VSCodeTutorial.ConsoleApp.csproj
```
也可以在`VSCodeTutorial.ConsoleApp` 目录下直接运行`dotnet run` 命令即可. 

结果运行还是乱码中，但是这次我们有办法解决，我们在控制台代码中添加一句代码即可`onsole.OutputEncoding = Encoding.UTF8`

```c#
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
```

使用dotnet build编译后，再次运行Console项目看到了我们期望的界面

![](http://ww1.sinaimg.cn/large/697065c1gy1feoupo8qinj20pv02rjre.jpg)

程序运行正确，当然了，我们都跑过单元测试了不是。。

## 0x07 开始调试程序了

如下图提示操作

![](http://ww1.sinaimg.cn/large/697065c1gy1feouu8xzrpj21720g5jts.jpg)

终于轮到我们之前生成的launch.json文件出场了，先来看下它的代码，代码中已经添加了配置的说明

```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": ".NET Core Launch (console)", //配置名称 可以改成更好识别的名字
            "type": "coreclr", // .net core类型的调试
            "request": "launch", //调试方式 不用改
            "preLaunchTask": "build", // 前置任务，这里是编译，但是默认的编译任务，已经被我改了名字了，所以这里要改一下哦
            "program": "${workspaceRoot}\\VSCodeTutorial.ConsoleApp\\bin\\Debug\\netcoreapp1.1\\VSCodeTutorial.ConsoleApp.dll", //需要调试的DLL的位置 
            "args": [], //额外的参数
            "cwd": "${workspaceRoot}\\VSCodeTutorial.ConsoleApp", //工作目录
            "console": "internalConsole", //控制台模式，这里是内嵌控制台，一会要改成外置的，不然没法交互输入
            "stopAtEntry": false,
            "internalConsoleOptions": "openOnSessionStart"
        },
        {
            "name": ".NET Core Attach", //名称
            "type": "coreclr", //类型
            "request": "attach", //使用附加的方式
            "processId": "${command:pickProcess}" //附加的进程ID
        }
    ]
}
```

根据实际情况，需要对上面的配置进行以下变更，变更的部分已经添加了注释，附加调试不是本文的重点，就不改了

```c#
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "调试ConsoleApp", //修改下命令
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build_console", //修改前置任务名和task.json中配置一致
            "program": "${workspaceRoot}\\VSCodeTutorial.ConsoleApp\\bin\\Debug\\netcoreapp1.1\\VSCodeTutorial.ConsoleApp.dll",
            "args": [],
            "cwd": "${workspaceRoot}\\VSCodeTutorial.ConsoleApp",
            "externalConsole":true, //使用外置的控制台
            "stopAtEntry": false,
            "internalConsoleOptions": "openOnSessionStart"
        },
        {
            "name": ".NET Core Attach",
            "type": "coreclr",
            "request": "attach",
            "processId": "${command:pickProcess}"
        }
    ]
}
```

修改完成后，我们点击运行按钮可以开始调试了，调试的方式和使用VS是一致的，快捷键为F5 F10 F11

![](http://ww1.sinaimg.cn/large/697065c1gy1feov8e1scqj20rm0gaq58.jpg)



简直太强大了！

## 0x08 多项目启动调试

有时候我们会同时在一个解决方案中 同时启动两个项目来调试，那么怎么配置呢，其实很简单，另外一个项目和之前的一样各自配置一个编译的Task（当然最好是两个项目使用.sln来统一编译），然后各配置一个launch配置，然后使用`compounds` 配置来同时启动即可，示例如下

```c#
{
    "version": "0.2.0",
    "configurations": [
        "HelloRpcClientLaunch"
        {
            "name": "HelloRpcServer",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "buildServer",
            "program": "${workspaceRoot}\\src\\sample\\HelloRpc\\HelloRpc.Server\\bin\\Debug\\netcoreapp1.1\\HelloRpc.Server.dll",
            "args": [],
            "cwd": "${workspaceRoot}\\src\\sample\\HelloRpc\\HelloRpc.Server",
            "externalConsole":true,
            "stopAtEntry": false,
            "internalConsoleOptions": "openOnSessionStart"
        },
        "HelloRpcClientLaunch"
        {
            "name": "HelloRpcClient",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "buildClient",
            "program": "${workspaceRoot}\\src\\sample\\HelloRpc\\HelloRpc.Client\\bin\\Debug\\netcoreapp1.1\\HelloRpc.Client.dll",
            "args": [],
            "cwd": "${workspaceRoot}\\src\\sample\\HelloRpc\\HelloRpc.Client",
            "externalConsole":true,
            "stopAtEntry": false,
            "internalConsoleOptions": "openOnSessionStart"
        }
    ]
    "compounds": [
        {
            "name": "Server/Client",
            "configurations": ["HelloRpcServer", "HelloRpcClient"]
        }
    ]
}
```

这时候就会有个Sever/Client的调试选项供选择了，如图:

![](http://ww1.sinaimg.cn/large/697065c1gy1feovdxuj89j207g04daa0.jpg)

好了，让我们一起使用VSCode来编写.NetCore项目吧