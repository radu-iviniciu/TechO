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

        static readonly string testPath = "D:\\Cipix\\tech-on\\solutions\\Radu.txt";

        static void Main(string[] args)
        {
            var generator = new PrgGenerator(testPath);
            generator.GenerateProgram();
        }
    }

    class PrgGenerator
    {
        private Writer _codeGen;
        
        // Predefined
        readonly string Rows = "input_rows";
        readonly string Cols = "input_cols";
        readonly string ObjectCount = "My_input_objects_count";
        readonly string Map = "My_input_objects";

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

        public PrgGenerator(string path)
        {
            _codeGen = new Writer(new StreamWriter(path));
        }

        public void GenerateProgram()
        {            
            _codeGen.DeclareArray(Map, 9, true);
            _codeGen.Declare(ObjectCount, true);

            _codeGen.Declare(robotX, true);
            _codeGen.Declare(robotY, true);
            _codeGen.Declare(count, true);
            _codeGen.Declare(i, true);
            _codeGen.Declare(j, true);

            _codeGen.Declare(curTargetX, true);
            _codeGen.Declare(curTargetY, true);

            _codeGen.DeclareArray(targets, 100, true);
            _codeGen.DeclareArray(destinations, 100, true);


            _codeGen.If(_codeGen.Equal(j, 0.ToString()), true);
            {
                _codeGen.Set(j, -1, true);
                _codeGen.Comment("INIT MAP", true);
                for (int k = 0; k < 9; k++)
                {
                    _codeGen.SetArrrayValue(Map, k.ToString(), k.ToString(), true);
                }
                _codeGen.Set(ObjectCount, 3, true);

                _codeGen.Comment("INIT MAP", true);

                _codeGen.Set(curTargetX, "0 1 -", true);
                _codeGen.Set(curTargetY, "0 1 -", true);

                FindRobot();

                _codeGen.Get(robotX, true);
                _codeGen.Get(robotY, true);

                _codeGen.Comment("Find Targets", true);
                // FindPointsOfInterest(targets, targetCode, count);

                _codeGen.Comment("Find Destinations", true);
                // FindPointsOfInterest(destinations, destinationCode, count);
            }
            _codeGen.Else(true);
            {
                _codeGen.Push(-1, true);
            }
            _codeGen.EndIf(true);

            _codeGen.WriteToFile();
        }

        int indentation = 0;

        public void FindPointsOfInterest(string resultArray, string code, string localCounter)
        {
            _codeGen.Set(i, 0, true);
            _codeGen.Set(j, 0, true);
            _codeGen.Set(localCounter, 0, true);

            var curValue = _codeGen.GetArrayValue(Map, _codeGen.Get(i));
            var isObjectOfRightCode = _codeGen.Equal(curValue, code);
            var isNotEndOfArray = _codeGen.Get(ObjectCount) + " 3 * " + " " + _codeGen.Get(i) + " > ";

            _codeGen.While(isNotEndOfArray, true);
            {
                _codeGen.If(isObjectOfRightCode, true);
                {
                    _codeGen.SetArrrayValue(resultArray, _codeGen.Get(j), _codeGen.GetArrayValue(Map, _codeGen.Get(i) + " 1 + "), true);
                    _codeGen.SetArrrayValue(resultArray, _codeGen.Get(j) + " 1 + ", _codeGen.GetArrayValue(Map, _codeGen.Get(i) + " 2 + "), true);

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

            _codeGen.Set(i, 0, true);

            var isCurentObjectTheRobot = _codeGen.NotEqual(_codeGen.GetArrayValue(Map, _codeGen.Get(i)), robotCode);
            var isEndofArray = _codeGen.GreaterEqual(i, _codeGen.Get(ObjectCount) + " 3 * 3 -");
            _codeGen.While(_codeGen.Or(isEndofArray, isCurentObjectTheRobot), true);
            {
                _codeGen.Increment(i, 1, true);
                _codeGen.Repeat(isEndofArray, true);
            }

            _codeGen.Set(robotX, _codeGen.GetArrayValue(Map, _codeGen.Get(i) + " 1 +"), true);
            _codeGen.Set(robotY, _codeGen.GetArrayValue(Map, _codeGen.Get(i) + " 2 +"), true);
        }      
    }
}

