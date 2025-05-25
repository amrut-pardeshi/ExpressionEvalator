using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionEvaluator
{
    internal static class Functions
    {
        public static Value Add(Value param1, Value param2)
        {
            return new Value(param1.Get<int>() + param2.Get<int>());
        }

        public static Value Equals(Value param1, Value param2)
        {
            return new Value(param1.Get<bool>() == param2.Get<bool>());
        }

        public static Value Not(Node arg)
        {
            return new Value(!((Literal)arg).Value.Get<bool>());
        }
    }
}
