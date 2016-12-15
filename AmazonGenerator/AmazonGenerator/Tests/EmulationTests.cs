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
        private static string _testPath = @"D:\Cipix\tech-on\spec\gen.spec.js";

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

        private string solutionsFolder = "D:\\Cipix\\tech-on\\solutions\\";

        [ClassInitialize]
        public static void TestInit(TestContext context)
        {
            var testPath = @"D:\Cipix\tech-on\spec\gen.spec.js";
            File.WriteAllText(testPath, _EmulatorTestTemplateHead + "\n");
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

            gen.Declare(localCounter, true);
            gen.Declare(count, true);
            gen.DeclareArray(input, 10, true);

            gen.Declare(i, true);
            gen.Declare(j, true);

            gen.Set(i, 0, true);
            gen.Set(j, 0, true);
            gen.Set(localCounter, 0, true);

            gen.SetArrrayValue(input, 0.ToString(), 1.ToString(), true);
            gen.SetArrrayValue(input, 1.ToString(), 1.ToString(), true);

            for (int k = 2; k < 10; k++)
            {
                gen.SetArrrayValue(input, k.ToString(), (10*k).ToString(), true);
            }

            var curValue = gen.GetArrayValue(input, gen.Get(i));
            var isObjectOfRightCode = gen.Equal(curValue, "3");
            var isNotEndOfArray = gen.Get(count) + " 3 * " + " " + gen.Get(i) + " > ";

            gen.While(isNotEndOfArray, true);
            {
                gen.If(isObjectOfRightCode, true);
                {                    
                    gen.SetArrrayValue(resultArray, gen.Get(j), gen.GetArrayValue(input, gen.Get(i) + " 1 + "), true);
                    gen.SetArrrayValue(resultArray, gen.Get(j) + " 1 + ", gen.GetArrayValue(input, gen.Get(i) + " 2 + "), true);

                    gen.Increment(j, 2, true);
                    gen.Increment(localCounter, true);
                }
                gen.EndIf(true);
                gen.Increment(i, 3, true);
            }
            gen.Repeat(isNotEndOfArray, true);

            gen.Set(i, 0, true);
            gen.Set(j, 0, true);
        }

        [TestMethod]
        public void AddTest()
        {
            string testName = "AddTest";
            Writer generator = new Writer(new StreamWriter(solutionsFolder + testName + ".txt"));

            // Write code 

            //
            GenerateTest(testName,  new int[] { 3 }, new [] { new Tuple<string, string>("x", "3") });
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
                sb.AppendLine(@"expect(context.getVariable('" + varVal.Item1 +  "'))" + ".to.be(" + varVal.Item2 + ");");
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
