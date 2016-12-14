using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonGenerator
{
    class Program
    {
        // variabile
        readonly string Rows = "input_rows";
        readonly string Cols = "input_cols";
        readonly string ObjectCount = "input_objects_count";
        readonly string Objects = "input_objects";

        static readonly string testPath = "D:\\Amazon Techo(n)\\01_helloWorld_automatic.txt";

        static void Main(string[] args)
        {
            var generator = new PrgGenerator(testPath);
            generator.GenerateProgram();
        }
    }

    class PrgGenerator
    {
        string _path;
        StreamWriter _file;

        readonly string Rows = "input_rows";
        readonly string Cols = "input_cols";
        readonly string ObjectCount = "input_objects_count";
        readonly string Objects = "input_objects";

        readonly string wallCode = "1";
        readonly string targetCode = "2";
        readonly string destinationCode = "3";
        readonly string robotCode = "4";
        readonly string strayCode = "5";

        readonly string robotX = "robotX";
        readonly string robotY = "robotY";

        readonly string targets = "targets";
        readonly string destinations = "destinations";
        readonly string count = "count";

        readonly string curTargetX = "curTargetX";
        readonly string curTargetY = "curTargetY";
        readonly string curDestX = "targetX";
        readonly string curDestY = "targetY";

        readonly string i = "i";
        readonly string j = "j";

        public PrgGenerator(string path)
        {
            _path = path;
            _file = new StreamWriter(path);
        }

        public void GenerateProgram()
        {
            Declare(robotX);
            Declare(robotY);
            Declare(count);
            Declare(i);
            Declare(j);

            Declare(curTargetX);
            Declare(curTargetY);

            DeclareArray(targets, 100);

            Set(curTargetX, "0 1 -", true);
            Set(curTargetY, "0 1 -", true);

            FindRobot();

            _file.WriteLine("Find Targets");
            FindPointsOfInterest(targets, targetCode, count);
            _file.WriteLine("");

            _file.WriteLine("Find Destinations");
            FindPointsOfInterest(destinations, destinationCode, count);
            _file.WriteLine("");

            _file.Flush();
            _file.Close();
        }

        void Assert(string expr, string y)
        {
            AreEqual(y, expr, true);
            _file.WriteLine("if");
            _file.WriteLine("2");
            _file.WriteLine("else");
            _file.WriteLine("4");
            _file.WriteLine("then");
        }

        int indentation = 0;

        void FindPointsOfInterest(string resultArray, string code, string localCounter)
        {
            Set(i, 0, true);
            Set(j, 0, true);
            Set(localCounter, 0, true);

            var curValue = GetArrayValue(Objects, Get(i));
            var isObjectOfRightCode = AreEqual(curValue, code);
            var isNotEndOfArray = Get(ObjectCount) + " 3 * " + " " + Get(i) + " > ";

            While(isNotEndOfArray);
            {
                If(isObjectOfRightCode);
                {
                    SetArrraValue(targets, Get(j), GetArrayValue(Objects, Get(i) + " 1 + "), true);
                    SetArrraValue(targets, Get(j) + " 1 + ", GetArrayValue(Objects, Get(i) + " 2 + "), true);

                    Increment(j, 2, true);
                    Increment(localCounter, true);
                }
                EndIf();
                Increment(i, 3, true);
            }
            Repeat(isNotEndOfArray);

            Set(i, 0, true);
            Set(j, 0, true);
        }

        private void EndIf()
        {
            _file.WriteLine("then");
        }

        void If(string condition)
        {
            _file.Write(condition + " ");
            _file.WriteLine("if");
        }

        string Not(string x, bool writeToFile = false)
        {
            var not = x + " 1 <";
            if (writeToFile)
            {
                _file.WriteLine(not);
            }
            return not;
        }

        string And(string x, string y, bool writeToFile = false)
        {
            var and = x + " = 1 " + y + " 1 =" + " 1 = ";
            if (writeToFile)
            {
                _file.WriteLine(and);
            }
            return and;
        }

        void FindRobot()
        {
            Set(robotX, "0 1 -", true);
            Set(robotY, "0 1 -", true);

            var isCurentObjectTheRobot = AreNotEqual(GetArrayValue(Objects, Get(i)), robotCode.ToString());
            While(isCurentObjectTheRobot);
            {
                Increment(i, 3, true);
                Repeat(isCurentObjectTheRobot);
            }

            Set(robotX, GetArrayValue(Objects, Get(i) + " 1 +"), true);
            Set(robotY, GetArrayValue(Objects, Get(i) + " 2 +"), true);
        }

        // a condininal expresion on the stack
        void While(string condition)
        {
            _file.WriteLine(condition);
            _file.WriteLine("while");
            indentation++;
        }

        void Repeat(string condition)
        {
            _file.WriteLine(condition);
            _file.WriteLine("repeat");
            indentation--;
        }

        string GreaterEqual(string x, string y, bool writeToFile = false) // 
        {
            var areEqual = x + " " + y + " >= ";
            if (writeToFile)
            {
                _file.WriteLine(areEqual);
            }
            return areEqual;
        }

        // 1 or 0 to Stack
        string AreNotEqual(string x, string y, bool writeToFile = false) // 
        {
            var areNotEqual = AreEqual(x, y) + " 1 < ";

            if (writeToFile)
            {
                _file.WriteLine(areNotEqual);
            }
            return areNotEqual;
        }

        string AreEqual(string x, string y, bool writeToFile = false) // 
        {
            var areEqual = x + " " + y + " = ";
            if (writeToFile)
            {
                _file.WriteLine(areEqual);
            }
            return areEqual;
        }

        string Declare(string variable)
        {
            var code = "variable " + variable;
            _file.WriteLine(code);
            return code;
        }

        string DeclareArray(string variable, int size)
        {
            var code = "variable " + variable + " " + size + " cells";
            _file.WriteLine(code);
            return code;
        }

        // Nothing on stack
        string Increment(string x, bool writeToFile = false)
        {
            var code = x + " @ 1 + " + x + " ! ";
            if (writeToFile)
            {
                _file.WriteLine(code);
            }
            return code;
        }

        // Nothing on stack
        string Increment(string x, int value, bool writeToFile = false)
        {
            var code = x + " @ " + value + " + " + x + " ! ";
            if (writeToFile)
            {
                _file.WriteLine(code);
            }
            return code;
        }

        string Increment(string x, string expresion, bool writeToFile = false)
        {
            var code = x + " @ " + expresion + " + " + x + " ! ";
            if (writeToFile)
            {
                _file.WriteLine(code);
            }
            return code;
        }

        // Returns to stack
        string GetArrayValue(string arrayAddr, string index, bool writeToFile = false)
        {
            var code = arrayAddr + " " + index + " + @ ";
            if (writeToFile)
            {
                _file.WriteLine(code);
            }
            return code;
        }

        // no prereq on stack
        string SetArrraValue(string arrayAddr, string index, string expresion, bool writeToFile = false)
        {
            var code = expresion + " " + arrayAddr + " " + index + " + ! ";
            if (writeToFile)
            {
                _file.WriteLine(code);
            }
            return code;
        }

        // nothing on stack needed nor left
        string Set(string x, string expr, bool writeToFile = false)
        {
            var code = expr + " " + x + " ! ";
            _file.WriteLine(code);
            return code;
        }

        // nothing on stack needed nor left
        string Set(string x, int val, bool writeToFile = false)
        {
            var code = val + " " + x + " ! ";
            if (writeToFile)
            {
                _file.WriteLine(code);
            }
            return code;
        }

        // Adds to stack
        string Get(string x, bool writeToFile = false)
        {
            var code = x + " @ ";
            if (writeToFile)
            {
                _file.WriteLine(code);
            }
            return code;
        }
    }
}

