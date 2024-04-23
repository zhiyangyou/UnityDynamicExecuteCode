using UnityEngine;

namespace Framework
{
    public static class Facade
    {
        public static void SayHello()
        {
            Debug.LogError($" Facade : Hello");
        }
    }
}