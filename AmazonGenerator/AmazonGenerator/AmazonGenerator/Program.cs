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

        static readonly string testPath = "01_helloWorld_automatic.txt";

        static void Main(string[] args)
        {
            var generator = new PrgGenerator(testPath);
            generator.GenerateProgram();
        }
    }

    class PrgGenerator
    {

        // Predefined
        readonly string Rows = "input_rows";
        readonly string Cols = "input_cols";
        readonly string ObjectCount = "input_objects_count";
        readonly string Map = "input_objects";

        // Need declaration
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

        private Writer _codeGen;

        public PrgGenerator(string path)
        {
            _codeGen = new Writer(new StreamWriter(path));
        }

        public void GenerateProgram()
        {
            _codeGen.Declare(robotX, true);
            _codeGen.Declare(robotY, true);
            _codeGen.Declare(count, true);
            _codeGen.Declare(i, true);
            _codeGen.Declare(j, true);

            _codeGen.Declare(curTargetX, true);
            _codeGen.Declare(curTargetY, true);

            _codeGen.DeclareArray(targets, 100, true);

            _codeGen.Set(curTargetX, "0 1 -", true);
            _codeGen.Set(curTargetY, "0 1 -", true);

            FindRobot();

            _codeGen.Comment("Find Targets", true);
            FindPointsOfInterest(targets, targetCode, count);

            _codeGen.Comment("Find Destinations", true);
            FindPointsOfInterest(destinations, destinationCode, count);

            _codeGen.Close();
        }

        int indentation = 0;

        void FindPointsOfInterest(string resultArray, string code, string localCounter)
        {
            _codeGen.Set(i, 0, true);
            _codeGen.Set(j, 0, true);
            _codeGen.Set(localCounter, 0, true);

            var curValue = _codeGen.GetArrayValue(Map, _codeGen.Get(i));
            var isObjectOfRightCode = _codeGen.AreEqual(curValue, code);
            var isNotEndOfArray = _codeGen.Get(ObjectCount) + " 3 * " + " " + _codeGen.Get(i) + " > ";

            _codeGen.While(isNotEndOfArray, true);
            {
                _codeGen.If(isObjectOfRightCode, true);
                {
                    _codeGen.SetArrraValue(resultArray, _codeGen.Get(j), _codeGen.GetArrayValue(Map, _codeGen.Get(i) + " 1 + "), true);
                    _codeGen.SetArrraValue(resultArray, _codeGen.Get(j) + " 1 + ", _codeGen.GetArrayValue(Map, _codeGen.Get(i) + " 2 + "), true);

                    _codeGen.Increment(j, 2, true);
                    _codeGen.Increment(localCounter, true);
                }
                _codeGen.EndIf(true);
                _codeGen.Increment(i, 3, true);
            }
            _codeGen.Repeat(isNotEndOfArray, true);

            _codeGen.Set(i, 0, true);
            _codeGen.Set(j, 0, true);
        }

        void FindRobot()
        {
            _codeGen.Set(robotX, "0 1 -", true);
            _codeGen.Set(robotY, "0 1 -", true);

            var isCurentObjectTheRobot = _codeGen.AreNotEqual(_codeGen.GetArrayValue(Map, _codeGen.Get(i)), robotCode.ToString());
            _codeGen.While(isCurentObjectTheRobot, true);
            {
                _codeGen.Increment(i, 3, true);
                _codeGen.Repeat(isCurentObjectTheRobot, true);
            }

            _codeGen.Set(robotX, _codeGen.GetArrayValue(Map, _codeGen.Get(i) + " 1 +"), true);
            _codeGen.Set(robotY, _codeGen.GetArrayValue(Map, _codeGen.Get(i) + " 2 +"), true);
        }      
    }
}

