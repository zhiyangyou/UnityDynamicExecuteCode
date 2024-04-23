# 作用

在运行时动态编译并运行一段代码



# 业务价值

在开发过程中往往会遇到这样的一种场景：

1. 遇到了某个Bug
2. 在Bug的现场希望可以通过一些编码快速dump出虚拟机实例中某些变量的值



​	在某些脚本语言中，在程序附着控制台窗口下，敲一些预设的函数名字，并执行，可以dump出一些运行时的数据信息。 

​	故，在Unity下也希望有这种调试手段（其他.Net生态的程序同理）



# 实现思路

.net生态拥有实现该功能基础设施：

1. 运行时获取程序域信息
2. 运行时分析代码语法，并编译
3. 运行时生成程序集，并加载
4. 反射机制



# 效果

![](https://github.com/zhiyangyou/UnityDynamicExecuteCode/blob/main/Doc/xiaoguotu2.gif)


