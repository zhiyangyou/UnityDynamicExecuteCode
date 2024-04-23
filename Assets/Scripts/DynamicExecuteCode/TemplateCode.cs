using Framework;
using UnityEngine;

namespace DynamicNamespace
{
    public class DynamicClass
    {
        public static int DynamicMethod()
        {
            Facade.SayHello();
            Debug.Log("'Facade.SayHello()' do complete!>>>>>>>>>>>>");
            int a = 3;
            int b = 2;
            var ret = a + b;
            return ret;
        }
    }
}