
namespace ExpressionEvaluator
{
    /// <summary>
    /// Base class for all AST nodes.
    /// </summary>
    public abstract class Node;

    /// <summary>
    /// Represents a strongly-typed value in the AST.
    /// </summary>
    public sealed class Value
    {
        private readonly object _value;

        public Value(string value) => _value = value;
        public Value(int value) => _value = value;
        public Value(DateTime value) => _value = value;
        public Value(bool value) => _value = value;
        public Value(Array value) => _value = value;

        public T Get<T>() => (T)_value;

    }

    /// <summary>
    /// Represents a literal value node.
    /// </summary>
    public sealed class Literal(Value value) : Node
    {
        public Value Value { get; } = value;
    }

    /// <summary>
    /// Represents a function call node.
    /// </summary>
    public sealed class Function(string name, IReadOnlyList<Node> parameters) : Node
    {
        public string Name { get; } = name;
        public IReadOnlyList<Node> Parameters { get; } = parameters;
    }

}
