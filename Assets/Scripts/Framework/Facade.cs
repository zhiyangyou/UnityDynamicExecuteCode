using UnityEngine;

namespace Framework
{
    public static class Facade
    {
        public static void SayHello()
        {
            Debug.Log($"Framework::Facade::Say Hello");
        }
    }
}