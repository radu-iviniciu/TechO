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
                                // new Tuple<string, string>("resultArray", "[0, 10, 20, 30, 40, 49, 59, 69, 79, 89]"),
                                new Tuple<string, string>("localCounter", "30")
                              });

            // Code To test
            ArrayWork(generator);
        }

        public void BoolOps(Writer generator)
        {
            var x = "x";
            var y = "y";
            generator.Declare(y, true);
            generator.Declare(x, true);

            generator.Set(x, 3, true);
            generator.Set(y, 4, true);

            generator.If(generator.Or(generator.GreaterEqual(generator.Get(x), generator.Get(y)), generator.Equal(generator.Get(y), "4")), true);
            {
                generator.Push(1, true);
            }
            generator.Else(true);
            {
                generator.Push(0, true);
            }
            generator.EndIf(true);

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
            gen.Declare(localCounter, true);
            gen.Declare(count, true);
            gen.DeclareArray(input, 10, true);

            gen.Declare(i, true);
            gen.Declare(j, true);

            gen.Set(i, 0, true);
            gen.Set(count, 10, true);
            gen.Set(j, 0, true);
            gen.Set(localCounter, 0, true);

            for (int k = 0; k < 10; k++)
            {
                gen.SetArrrayValue(input, k.ToString(), (10*k).ToString(), true);
            }

            var curValue = gen.GetArrayValue(input, gen.Get(i));
            var isObjectOfRightCode = gen.Less(curValue, "50");
            gen.Increment(count, true);
            var isNotEndOfArray = gen.Less(gen.Get(i), gen.Subtract(gen.Get(count), "1"));

            gen.Set(i, 0, true);
            gen.Set(j, 0, true);

            gen.While(isNotEndOfArray, true);
            {
                gen.If(isObjectOfRightCode, true);
                {
                    gen.SetArrrayValue(resultArray, gen.Get(j), gen.GetArrayValue(input, gen.Get(i)), true);
                }
                gen.Else(true);
                {
                    gen.SetArrrayValue(resultArray, gen.Get(j), gen.Subtract(gen.GetArrayValue(input, gen.Get(i)), 1.ToString()), true);                    
                }                
                gen.EndIf(true);
                gen.Increment(j, 1, true);
                gen.Increment(i, 1, true);
            }
            gen.Repeat(isNotEndOfArray, true);

            gen.Set(localCounter, gen.GetArrayValue(resultArray, 3.ToString(), true));
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
                    sb.AppendLine(@"expect(context.getArrayVariable('" + varVal.Item1 + "'))" + ".to.be(" + varVal.Item2 + ");");
                }
                else
                {
                    sb.AppendLine(@"expect(context.getVariable('" + varVal.Item1 + "'))" + ".to.be(" + varVal.Item2 + ");");
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
