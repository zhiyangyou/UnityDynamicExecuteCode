using Framework;

namespace DynamicNamespace
{
    public class DynamicClass
    {
        public static int DynamicMethod()
        {
            Facade.SayHello();
            int a = 3;
            int b = 2;
            var ret = a + b;
            return ret;
        }
    }
}