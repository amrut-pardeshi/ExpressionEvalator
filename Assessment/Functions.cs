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
            var v1 = param1.Get<object>();
            var v2 = param2.Get<object>();
            return new Value(Equals(v1, v2));
        }

        public static Value Not(Value param1)
        {
            return new Value(!param1.Get<bool>());
        }

        public static Value FetchGet(Value url)
        {
            ArgumentNullException.ThrowIfNull(url);

            var urlString = url.Get<string>();
            using var client = new HttpClient();
            var response = client.GetStringAsync(urlString).GetAwaiter().GetResult();
            return new Value(response);
        }

        public static Value Contains(Value source, Value substring)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(substring);

            var sourceString = source.Get<string>();
            var substringString = substring.Get<string>();
            return new Value(sourceString != null && substringString != null && sourceString.Contains(substringString));
        }
    }
}
