using System;
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

        // stack assertion
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
            
            GenerateTest(testName, 1);

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

            generator.If(generator.Or(generator.GreaterEqual(generator.Get(x), generator.Get(y)), generator.AreEqual(generator.Get(y), "4")), true);
            {
                generator.Push(1, true);
            }
            generator.Else(true);
            {
                generator.Push(0, true);
            }
            generator.EndIf(true);

            generator.Close();
        }

        [TestMethod]
        public void AddTest()
        {
            string testName = "AddTest";
            Writer generator = new Writer(new StreamWriter(solutionsFolder + testName + ".txt"));

            // Write code 

            //
            GenerateTest(testName, 3);
        }

        private void GenerateTest(string testName, params int[] stackContentToAssert)
        {
            var testCode = GetTestTemplate(testName);

            testCode = AddStackAssertion(stackContentToAssert, testCode);

            File.AppendAllText(_testPath, testCode);          
        }

        private static string AddStackAssertion(int[] stackContentToAssert, string testCode)
        {
            var stack = new StringBuilder();
            stack.Append("expect(context.stackContent()).to.eql([");
            stackContentToAssert.ToList().ForEach(x => stack.Append(x.ToString() + ","));
            stack.Remove(stack.Length - 1, 1);
            stack.Append("]);");


            testCode = testCode.Replace("// stack assertion", stack.ToString());
            return testCode;
        }

        private static string GetTestTemplate(string testName)
        {
            var testCode = _testTemplate.Replace("test.txt", testName + ".txt");
            testCode = testCode.Replace("test", testName);
            return testCode;
        }
    }
}
