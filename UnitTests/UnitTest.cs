using ExpressionEvaluator;

namespace UnitTests
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void It_Evaluates_A_Literal()
        {
            var r = Evaluator.Evaluate(new Literal(13));

            Assert.AreEqual(r.Value<int>(), 13);
        }

        [TestMethod]
        public void It_Evaluates_A_Not_Function()
        {
            Assert.AreEqual(Evaluator.Evaluate(new Function("not", [new Literal(true)])).Value<bool>(), false);
        }

        [TestMethod]
        public void It_Evaluates_A_Add_Function()
        {
            Assert.AreEqual(Evaluator.Evaluate(new Function("add", [new Literal(3), new Literal(6)])).Value<int>(), 9);
        }

        [TestMethod]
        public void It_throws_for_invalid_Expression()
        {
            var literal = Evaluator.Evaluate(new Literal(""));
            Assert.ThrowsException<InvalidCastException>(() => literal.Value<int>());
        }

        [TestMethod]
        public void It_throws_for_invalid_function_expression()
        {
            Assert.ThrowsException<Exception>(() => Evaluator.Evaluate(new Function("toString", [])));
        }
    }
}