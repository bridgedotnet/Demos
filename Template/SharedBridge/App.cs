#if BRIDGE
using Bridge;
using Bridge.Html5;
#else
using System;
#endif

namespace SharedBridge
{
    public class App
    {

        #if BRIDGE
        [Ready]
        #endif
        public static void Main()
        {
            
        }
    }
}
