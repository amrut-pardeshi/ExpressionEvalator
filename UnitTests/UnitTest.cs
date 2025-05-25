using ExpressionEvaluator;

namespace UnitTests
{
    [TestClass]
    public class UnitTest
    {
        #region Literal Evaluation
        [TestMethod]
        public void It_Evaluates_An_Integer_Literal()
        {
            var r = Evaluator.Evaluate(new Literal(new Value(42)));
            Assert.AreEqual(r.Get<int>(), 42);
        }

        [TestMethod]
        public void It_Evaluates_A_Boolean_Literal()
        {
            var r = Evaluator.Evaluate(new Literal(new Value(true)));
            Assert.AreEqual(r.Get<bool>(), true);
        }

        [TestMethod]
        public void It_Evaluates_A_String_Literal()
        {
            var r = Evaluator.Evaluate(new Literal(new Value("test")));
            Assert.AreEqual(r.Get<string>(), "test");
        }
        #endregion Literal Evaluation

        #region Not Function Tests
        [TestMethod]
        public void It_Evaluates_Not_Function_True()
        {
            var r = Evaluator.Evaluate(new Function("not", [new Literal(new Value(true))]));
            Assert.AreEqual(r.Get<bool>(), false);
        }

        [TestMethod]
        public void It_Evaluates_Not_Function_False()
        {
            var r = Evaluator.Evaluate(new Function("not", [new Literal(new Value(false))]));
            Assert.AreEqual(r.Get<bool>(), true);
        }
        #endregion Not Function Tests

        #region Add Function Tests
        [TestMethod]
        public void It_Evaluates_Add_Function_Positive()
        {
            var r = Evaluator.Evaluate(new Function("add", [
                new Literal(new Value(5)),
                new Literal(new Value(7))
            ]));
            Assert.AreEqual(r.Get<int>(), 12);
        }

        [TestMethod]
        public void It_Evaluates_Add_ThreeParameters_Function_Positive()
        {
            var r = Evaluator.Evaluate(new Function("add", [
                new Literal(new Value(5)),
                new Literal(new Value(6)),
                new Literal(new Value(7))
            ]));
            Assert.AreEqual(r.Get<int>(), 18);
        }

        [TestMethod]
        public void It_Evaluates_Add_Function_Negative()
        {
            var r = Evaluator.Evaluate(new Function("add", [
                new Literal(new Value(-3)),
                new Literal(new Value(-7))
            ]));
            Assert.AreEqual(r.Get<int>(), -10);
        }

        [TestMethod]
        public void It_Evaluates_Add_Function_Zero()
        {
            var r = Evaluator.Evaluate(new Function("add", [
                new Literal(new Value(0)),
                new Literal(new Value(0))
            ]));
            Assert.AreEqual(r.Get<int>(), 0);
        }
        #endregion Add Function Tests

        #region Contains Function Tests

        [TestMethod]
        public void It_Evaluates_Contains_Function_Positive()
        {
            var r = Evaluator.Evaluate(new Function("contains", [
                new Literal(new Value("abcde")),
                new Literal(new Value("bcd"))
            ]));
            Assert.IsTrue(r.Get<bool>());
        }

        [TestMethod]
        public void It_Evaluates_Contains_Function_Negative()
        {
            var r = Evaluator.Evaluate(new Function("contains", [
                new Literal(new Value("abcde")),
                new Literal(new Value("xyz"))
            ]));
            Assert.IsFalse(r.Get<bool>());
        }

        [TestMethod]
        public void It_Evaluates_Contains_Function_Empty_Substring()
        {
            var r = Evaluator.Evaluate(new Function("contains", [
                new Literal(new Value("abcde")),
                new Literal(new Value(""))
            ]));
            Assert.IsTrue(r.Get<bool>());
        }

        [TestMethod]
        public void It_Evaluates_Contains_Function_Empty_String()
        {
            var r = Evaluator.Evaluate(new Function("contains", [
                new Literal(new Value("")),
                new Literal(new Value("a"))
            ]));
            Assert.IsFalse(r.Get<bool>());
        }
        #endregion Contains Function Tests

        #region FetchGet Function (network-dependent) Tests
        [TestMethod]
        public void It_Evaluates_FetchGet_Function()
        {
            var r = Evaluator.Evaluate(new Function("fetchGet", [
                new Literal(new Value("https://www.example.com"))
            ]));
            var content = r.Get<string>();
            Assert.IsTrue(content.Contains("Example Domain"));
        }
        #endregion FetchGet Function (network-dependent) Tests

        #region Nested/Composed Functions Tests
        [TestMethod]
        public void It_Evaluates_Not_ComplexFunction_True()
        {
            var ast = new Function(
                "equals", [
                    new Function("add", [
                        new Literal(new Value(1)),
                        new Literal(new Value(1))
                    ]),
                    new Literal(new Value(2))
                ]);
            var r = Evaluator.Evaluate(ast);
            Assert.IsTrue(r.Get<bool>());
        }

        [TestMethod]
        public void It_Evaluates_Nested_Functions()
        {
            // not(contains("abc", "d")) => not(false) => true
            var ast = new Function(
                "not", [
                    new Function("contains", [
                        new Literal(new Value("abc")),
                        new Literal(new Value("c"))
                    ])
            ]);
            var r = Evaluator.Evaluate(ast);
            Assert.IsFalse(r.Get<bool>());
        }

        [TestMethod]
        public void It_Evaluates_Composed_FetchGet_And_Contains()
        {
            var ast = new Function("contains", [
                new Function("fetchGet", [
                    new Literal(new Value("https://www.example.com"))
                ]),
                new Literal(new Value("Example Domain"))
            ]);
            var r = Evaluator.Evaluate(ast);
            Assert.IsTrue(r.Get<bool>());
        }
        #endregion Nested/Composed Functions Tests

        #region Error Handling Tests
        [TestMethod]
        public void It_Throws_For_Invalid_Literal_Cast()
        {
            var literal = Evaluator.Evaluate(new Literal(new Value("not an int")));
            Assert.ThrowsException<InvalidCastException>(() => literal.Get<int>());
        }

        [TestMethod]
        public void It_Throws_For_Unknown_Function()
        {
            Assert.ThrowsException<Exception>(() =>
                Evaluator.Evaluate(new Function("unknownFunc", []))
            );
        }

        [TestMethod]
        public void It_Throws_For_Invalid_Function_Arguments()
        {
            var ex = Assert.ThrowsException<Exception>(() =>
                Evaluator.Evaluate(new Function("add", [
                    new Literal(new Value("a")),
                    new Literal(new Value("b"))
                ]))
            );
            Assert.AreEqual("Function 'add' only supports integer parameters.", ex.Message);
        }
        #endregion Error Handling Tests

        #region Edge Cases Validation Tests
        [TestMethod]
        public void It_Evaluates_Add_Function_With_One_Argument()
        {
            var ex = Assert.ThrowsException<Exception>(() =>
                Evaluator.Evaluate(new Function("add", [
                    new Literal(new Value(5))
                ]))
            );
            Assert.AreEqual("Function 'add' expects at least 2 parameters.", ex.Message);
        }

        [TestMethod]
        public void It_Evaluates_Empty_Function_Arguments()
        {
            var ex = Assert.ThrowsException<Exception>(() =>
               Evaluator.Evaluate(new Function("add", []))
            );
            Assert.AreEqual("Function 'add' expects at least 2 parameters.", ex.Message);
        }

        [TestMethod]
        public void It_Throws_For_Not_With_No_Arguments()
        {
            var ex = Assert.ThrowsException<Exception>(() =>
                Evaluator.Evaluate(new Function("not", []))
            );
            Assert.AreEqual("Function 'not' expects 1 parameter.", ex.Message);
        }

        [TestMethod]
        public void It_Throws_For_Not_With_Multiple_Arguments()
        {
            var ex = Assert.ThrowsException<Exception>(() =>
                Evaluator.Evaluate(new Function("not", [
                    new Literal(new Value(true)),
            new Literal(new Value(false))
                ]))
            );
            Assert.AreEqual("Function 'not' expects 1 parameter.", ex.Message);
        }

        [TestMethod]
        public void It_Throws_For_Equals_With_One_Argument()
        {
            var ex = Assert.ThrowsException<Exception>(() =>
                Evaluator.Evaluate(new Function("equals", [
                    new Literal(new Value(1))
                ]))
            );
            Assert.AreEqual("Function 'equals' expects 2 parameters.", ex.Message);
        }

        [TestMethod]
        public void It_Throws_For_Equals_With_Three_Arguments()
        {
            var ex = Assert.ThrowsException<Exception>(() =>
                Evaluator.Evaluate(new Function("equals", [
                    new Literal(new Value(1)),
            new Literal(new Value(2)),
            new Literal(new Value(3))
                ]))
            );
            Assert.AreEqual("Function 'equals' expects 2 parameters.", ex.Message);
        }

        [TestMethod]
        public void It_Throws_For_Contains_With_One_Argument()
        {
            var ex = Assert.ThrowsException<Exception>(() =>
                Evaluator.Evaluate(new Function("contains", [
                    new Literal(new Value("abc"))
                ]))
            );
            Assert.AreEqual("Function 'contains' expects 2 parameters.", ex.Message);
        }

        [TestMethod]
        public void It_Throws_For_Contains_With_Three_Arguments()
        {
            var ex = Assert.ThrowsException<Exception>(() =>
                Evaluator.Evaluate(new Function("contains", [
                    new Literal(new Value("abc")),
            new Literal(new Value("a")),
            new Literal(new Value("b"))
                ]))
            );
            Assert.AreEqual("Function 'contains' expects 2 parameters.", ex.Message);
        }
        #endregion Edge Cases Validation Tests
    }
}