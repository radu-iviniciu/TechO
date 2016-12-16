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
            _codeGen.Declare(ObjectCount);

            _codeGen.Declare(robotX);
            _codeGen.Declare(robotY);
            _codeGen.Declare(count);
            _codeGen.Declare(i);
            _codeGen.Declare(j);

            _codeGen.Declare(curTargetX);
            _codeGen.Declare(curTargetY);

            _codeGen.DeclareArray(targets, 100, true);
            _codeGen.DeclareArray(destinations, 100, true);


            _codeGen.If(_codeGen.Equal(j, 0.ToString()));
            {
                _codeGen.Set(j, -1);
                _codeGen.Comment("INIT MAP");
                for (int k = 0; k < 9; k++)
                {
                    _codeGen.SetArrrayValue(Map, k.ToString(), k.ToString());
                }
                _codeGen.Set(ObjectCount, 3);

                _codeGen.Comment("INIT MAP");

                _codeGen.Set(curTargetX, "0 1 -");
                _codeGen.Set(curTargetY, "0 1 -");

                FindRobot();

                _codeGen.Get(robotX);
                _codeGen.Get(robotY);

                _codeGen.Comment("Find Targets");
                // FindPointsOfInterest(targets, targetCode, count);

                _codeGen.Comment("Find Destinations");
                // FindPointsOfInterest(destinations, destinationCode, count);
            }
            _codeGen.Else();
            {
                _codeGen.Push(-1);
            }
            _codeGen.EndIf();

            _codeGen.WriteToFile();
        }

        int indentation = 0;

        public void FindPointsOfInterest(string resultArray, string code, string localCounter)
        {
            _codeGen.Set(i, 0);
            _codeGen.Set(j, 0);
            _codeGen.Set(localCounter, 0);

            var curValue = _codeGen.GetArrayValue(Map, _codeGen.Get(i));
            var isObjectOfRightCode = _codeGen.Equal(curValue, code);
            var isNotEndOfArray = _codeGen.Get(ObjectCount) + " 3 * " + " " + _codeGen.Get(i) + " > ";

            _codeGen.While(isNotEndOfArray);
            {
                _codeGen.If(isObjectOfRightCode);
                {
                    _codeGen.SetArrrayValue(resultArray, _codeGen.Get(j), _codeGen.GetArrayValue(Map, _codeGen.Get(i) + " 1 + "));
                    _codeGen.SetArrrayValue(resultArray, _codeGen.Get(j) + " 1 + ", _codeGen.GetArrayValue(Map, _codeGen.Get(i) + " 2 + "));

                    _codeGen.Increment(j, 2);
                    _codeGen.Increment(localCounter);
                }
                _codeGen.EndIf();
                _codeGen.Increment(i, 3);
            }
            _codeGen.Repeat(isNotEndOfArray);

            _codeGen.Set(i, 0);
            _codeGen.Set(j, 0);
        }

        void FindRobot()
        {
            _codeGen.Set(robotX, "0 1 -");
            _codeGen.Set(robotY, "0 1 -");

            _codeGen.Set(i, 0);

            var isCurentObjectTheRobot = _codeGen.NotEqual(_codeGen.GetArrayValue(Map, _codeGen.Get(i)), robotCode);
            var isEndofArray = _codeGen.GreaterEqual(i, _codeGen.Get(ObjectCount) + " 3 * 3 -");
            _codeGen.While(_codeGen.Or(isEndofArray, isCurentObjectTheRobot));
            {
                _codeGen.Increment(i, 1);
                _codeGen.Repeat(isEndofArray);
            }

            _codeGen.Set(robotX, _codeGen.GetArrayValue(Map, _codeGen.Get(i) + " 1 +"));
            _codeGen.Set(robotY, _codeGen.GetArrayValue(Map, _codeGen.Get(i) + " 2 +"));
        }      
    }
}

