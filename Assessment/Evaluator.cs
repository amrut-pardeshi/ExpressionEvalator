
namespace ExpressionEvaluator
{
    public static class Evaluator
    {
        public static Value Evaluate(Node expression)
        {
            if (expression is Literal literal)
            {
                return literal.Value;
            }
            else if (expression is Function function)
            {
                switch (function.Name)
                {
                    case "add":
                        {
                            if (function.Parameters.Count < 2)
                                throw new Exception("Function 'add' expects at least 2 parameters.");

                            var values = new List<Value>();
                            for (int i = 0; i < function.Parameters.Count; i++)
                            {
                                values.Add(Evaluate(function.Parameters[i]));
                            }

                            if (values.Any(v => v.Get<object>() is not int))
                                throw new Exception("Function 'add' only supports integer parameters.");

                            int sum = 0;
                            for (int i = 0; i < values.Count; i++)
                            {
                                sum += values[i].Get<int>();
                            }
                            return new Value(sum);
                        }
                    case "equals":
                        {
                            if (function.Parameters.Count != 2)
                                throw new Exception("Function 'equals' expects 2 parameters.");
                            var param1 = Evaluate(function.Parameters[0]);
                            var param2 = Evaluate(function.Parameters[1]);
                            return Functions.Equals(param1, param2);
                        }
                    case "not":
                        {
                            if (function.Parameters.Count != 1)
                                throw new Exception("Function 'not' expects 1 parameter.");
                            var param1 = Evaluate(function.Parameters[0]);
                            return Functions.Not(param1);
                        }
                    case "fetchGet":
                        {
                            if (function.Parameters.Count != 1)
                                throw new Exception("Function 'fetchGet' expects 1 parameter.");
                            var url = Evaluate(function.Parameters[0]);
                            return Functions.FetchGet(url);
                        }
                    case "contains":
                        {
                            if (function.Parameters.Count != 2)
                                throw new Exception("Function 'contains' expects 2 parameters.");
                            var source = Evaluate(function.Parameters[0]);
                            var substring = Evaluate(function.Parameters[1]);
                            return Functions.Contains(source, substring);
                        }
                    default:
                        throw new Exception($"Unknown function: {function.Name}");
                }
            }
            else
            {
                throw new Exception("Unknown node type");
            }
        }
    }
}
