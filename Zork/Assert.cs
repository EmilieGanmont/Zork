using System;
using System.Diagnostics;

namespace Zork
{
    class Assert
    {
        [Conditional("DEBUG")]

        public static void IsTrue(bool expression, string message = null)
        {
            if(expression == false)
            {
                throw new Exception(message);
            }
        }

    }
}
