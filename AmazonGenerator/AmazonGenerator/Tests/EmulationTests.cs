using System;
using System.CodeDom;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AmazonGenerator;

namespace Tests
{        
    [TestClass]
    public class EmulationTests
    {
        private static string _testPath = @"D:\Repos\Cipix\tech-on\spec\gen.spec.js";

        private static string _EmulatorTestTemplateHead = @"var emulator = require('../sources/emulator.js');
var tokenList = require('../sources/tokenize.js').tokenList;
var t = require('../sources/tokenize.js').tokens;
var expect = require('expect.js');
var fs = require('fs');";



        private static string _testTemplate =
            @"var test = fs.readFileSync('solutions/test.txt', 'utf8');

describe('test', function () {
    it('can execute test', function () {
        var context = new emulator.Context();
        context.load(test);
        context.execute();
";

        private static string _testTemplateEnd = @"
    });
});";

        private string solutionsFolder = "D:\\Repos\\Cipix\\tech-on\\solutions\\";

        [ClassInitialize]
        public static void TestInit(TestContext context)
        {
            File.WriteAllText(_testPath, _EmulatorTestTemplateHead + "\n");
        }

        [TestMethod]
        public void BoolOperationsTests()
        {
            string testName = "BoolOps";
            Writer generator = new Writer(new StreamWriter(solutionsFolder + testName + ".txt"));

            GenerateTest(
                testName: testName,
                stackContent: new [] { 1 },
                varVal:new[] { new Tuple<string, string>("x", "3") });

            // Code To test
            BoolOps(generator);
        }

        [TestMethod]
        public void ArrayWorkTest()
        {
            string testName = "ArrayOps";
            Writer generator = new Writer(new StreamWriter(solutionsFolder + testName + ".txt"));

            GenerateTest(
                testName: testName,
                stackContent: null,
                varVal: new[] {
                                new Tuple<string, string>("resultArray", "[0, 10, 20, 30, 40, 49, 59, 69, 79, 89]"),
                                new Tuple<string, string>("localCounter", "30")
                              });

            // Code To test
            ArrayWork(generator);
        }

        [TestMethod]
        public void MethodCallTest()
        {
            string testName = "MethodCallTest";
            Writer generator = new Writer(new StreamWriter(solutionsFolder + testName + ".txt"));

            GenerateTest(
                testName: testName,
                stackContent: null,
                varVal: new[] {
                                new Tuple<string, string>("x", "45")
                              });

            // Code To test
            MethodCalTest(generator);
        }

        public void MethodCalTest(Writer generator)
        {
            // variable names
            var x = "x";

            // Metode
            var local = "SQTFParameter";
            generator.BeginMethod("SquareTimesFive", new string[] { local });
            {
                var i = "i";
                generator.Declare(i);
                generator.Set(i, 0);

                generator.Set(i, 5);

                generator.Return(generator.Multiply(i, generator.Multiply(local, local, false), false));
            }
            generator.EndMethod();

            // MAIN
            generator.Declare(x);

            generator.Set(x, 3);
            
            generator.MethodCall("SquareTimesFive", x, new [] { x });

            generator.WriteToFile();
        }

        public void BoolOps(Writer generator)
        {
            var x = "x";
            var y = "y";
            generator.Declare(y);
            generator.Declare(x);

            generator.Set(x, 3);
            generator.Set(y, 4);

            generator.If(generator.Or(generator.GreaterEqual(x, y, false), generator.Equal(y, "4", false), false));
            {
                generator.Push(1);
            }
            generator.Else();
            {
                generator.Push(0);
            }
            generator.EndIf();

            generator.WriteToFile();
        }

        public void ArrayWork(Writer gen)
        {
            var i = "i";
            var j = "j";
            var input = "array";
            var localCounter = "localCounter";
            var count = "count";
            string resultArray = "resultArray";

            gen.DeclareArray(resultArray, 10, true);
            gen.Declare(localCounter);
            gen.Declare(count);
            gen.DeclareArray(input, 10, true);

            gen.Declare(i);
            gen.Declare(j);

            gen.Set(i, 0);
            gen.Set(count, 10);
            gen.Set(j, 0);
            gen.Set(localCounter, 0);

            for (int k = 0; k < 10; k++)
            {
                gen.SetArrrayValue(input, k.ToString(), (10*k).ToString());
            }

            gen.Increment(count);

            var curValue = gen.GetArrayValue(input, i, false);
            var isObjectOfRightCode = gen.Less(curValue, "50", false);            
            var isNotEndOfArray = gen.Less(i, gen.Subtract(count, "1", false), false);

            gen.Set(i, 0);
            gen.Set(j, 0);

            gen.While(isNotEndOfArray);
            {
                gen.If(isObjectOfRightCode);
                {
                    gen.SetArrrayValue(resultArray, j, gen.GetArrayValue(input, i, false));
                }
                gen.Else();
                {
                    gen.SetArrrayValue(resultArray, j, gen.Subtract(gen.GetArrayValue(input, i, false), 1.ToString(), false));                    
                }                
                gen.EndIf();
                gen.Increment(j);
                gen.Increment(i);
            }
            gen.Repeat(isNotEndOfArray);

            gen.Set(localCounter, gen.GetArrayValue(resultArray, 3.ToString(), false));
            gen.WriteToFile();
        }

        private void GenerateTest(string testName, int[] stackContent, Tuple<string, string>[] varVal)
        {
            var testCode = GetTestTemplate(testName);

            StringBuilder sb = new StringBuilder(testCode);
            if (stackContent != null)
            {
                sb = AddStackAssertion(stackContent, sb);
            }

            if (varVal != null)
            {
                sb = AddvariableValueAssertions(varVal, sb);
            }

            sb.AppendLine(_testTemplateEnd);

            File.AppendAllText(_testPath, sb.ToString());          
        }

        private StringBuilder AddvariableValueAssertions(Tuple<string, string>[] variableValueAssertions, StringBuilder sb)
        {
            foreach (var varVal in variableValueAssertions)
            {
                if (varVal.Item2.Contains("["))
                {
                    sb.AppendLine(@"expect(context.getArrayVariable('" + varVal.Item1 + "'))" + ".to.eql(" + varVal.Item2 + ");");
                }
                else
                {
                    sb.AppendLine(@"expect(context.getVariable('" + varVal.Item1 + "'))" + ".to.eql(" + varVal.Item2 + ");");
                }
            }
            return sb;
        }


        private static StringBuilder AddStackAssertion(int[] stackContentToAssert, StringBuilder sb)
        {
            sb.Append("expect(context.stackContent()).to.eql([");
            stackContentToAssert.ToList().ForEach(x => sb.Append(x.ToString() + ","));
            sb.Remove(sb.Length - 1, 1);
            sb.Append("]);");
            sb.AppendLine();

            return sb;
        }

        private static string GetTestTemplate(string testName)
        {
            var testCode = _testTemplate.Replace("test.txt", testName + ".txt");
            testCode = testCode.Replace("test", testName);
            return testCode;
        }
    }
}
