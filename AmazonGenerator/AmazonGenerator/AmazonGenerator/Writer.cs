using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AmazonGenerator
{
    public class Writer
    {
        private HashSet<string> Words = new HashSet<string>();

        private Dictionary<string, IEnumerable<string>> methoToParamsList;

        private string context = "";

        private int Indetation
        {
            get { return _indetation; }
            set
            {
                StringBuilder sb = new StringBuilder();
                _indetation = value;
                for (int i = 0; i < _indetation; i++)
                {
                    sb.Append("\t");
                }
                _tabs = sb.ToString();
            }
        } 

        private StringBuilder worker;
        private StringBuilder method;
        private StringBuilder declarations;
        private StringBuilder main;


        private int _indetation;

        private string _tabs;
        private StreamWriter _fileStream;

        public Writer(StreamWriter stream)
        {
            _fileStream = stream;

            declarations = new StringBuilder();

            method = new StringBuilder();

            main = new StringBuilder();

            worker = main;

            Indetation = 0;
        }

        public void WriteToFile()
        {
            _fileStream.WriteLine("(DECLARATIONS)");
            _fileStream.WriteLine(declarations.ToString());

            _fileStream.WriteLine("(MAIN)");
            _fileStream.WriteLine(worker.ToString());
            _fileStream.Close();
        }

        public void BeginMethod(string name, string[] parameterNames)
        {
            method = new StringBuilder();
            worker = method;

            methoToParamsList[name] = parameterNames;

            foreach (var parameter in parameterNames)
            {
                Declare(parameter, true);
            }

            worker.AppendLine(": " + name);
        }

        public void EndMethod()
        {
            worker.AppendLine(";");
            main.Insert(0, worker.ToString());
            worker = main;
        }

        public void MethodCall(string name, string[] parameterValues)
        {
            var localBuilder = new StringBuilder();

            var paramNames = methoToParamsList[name];

            if(parameterValues.Length != paramNames.Count())
            {
                throw new InvalidDataException("Method called with invalid number of parameters");
            }

            int i = 0;

            foreach(var param in paramNames)
            {
                if (Words.Contains(param))
                {

                }
                else
                {
                    // Pass by value
                    Set(param, parameterValues[i], true);
                }
            }
        }

        public string Else(bool comit = false)
        {
            Indetation--;
            string result = _tabs + "(ELSE) else";
            if (comit)
            {
                worker.AppendLine(result);
            }
            Indetation++;
            return result;            
        }

        public string EndIf(bool comit = false)
        {
            Indetation--;
            if (comit)
            {
                worker.AppendLine(_tabs + "(THEN) then");
            }
            return " then";
        }

        public string If(string condition, bool comit = false)
        {
            string res = "(IF) " + condition + " " + "if";
            if (comit)
            {
                worker.AppendLine(_tabs + res);
            }
            Indetation++;
            return res;
        }

        public string Not(string x, bool comit = false)
        {
            var not = x + " 1 <";
            if (comit)
            {
                worker.AppendLine(_tabs + not);
            }
            return not;
        }

        // a condininal expresion on the stack
        public string While(string condition, bool comit = false)
        {
            var res = "(WHILE)" + condition + " while";
            if (comit)
            {
                worker.AppendLine(_tabs + res);
            }
            Indetation++;
            return res;            
        }

        public void Repeat(string condition, bool comit = false)
        {
            var res = "(REPEAT IF)" + condition + "\n"+ _tabs + "repeat";
            if (comit)
            {
                worker.AppendLine(_tabs + res);
            }
            Indetation--;
        }

        public string Comment(string comment, bool comit = false)
        {
            string res = "(" + comment + ")";
            if (comit)
            {
                worker.AppendLine(_tabs + res);
            }
            return res;
        }

        public  string GreaterEqual(string x, string y, bool comit = false) // 
        {
            var areEqual = x + " " + y + " >= ";
            if (comit)
            {
                worker.AppendLine(_tabs + areEqual);
            }
            return areEqual;
        }

        public string Less(string x, string y, bool comit = false) // 
        {
            var areEqual = x + " " + y + " < ";
            if (comit)
            {
                worker.AppendLine(_tabs + areEqual);
            }
            return areEqual;
        }

        public string LessOrEqual(string x, string y, bool comit = false) // 
        {
            var areEqual = x + " " + y + " <= ";
            if (comit)
            {
                worker.AppendLine(_tabs + areEqual);
            }
            return areEqual;
        }

        public string Greater(string x, string y, bool comit = false) // 
        {
            var areEqual = x + " " + y + " > ";
            if (comit)
            {
                worker.AppendLine(_tabs + areEqual);
            }
            return areEqual;
        }

        public string And(string x, string y, bool comit = false)
        {
            var and = x + " = 1 " + y + " 1 =" + " 1 = ";
            if (comit)
            {
                worker.AppendLine(_tabs + and);
            }
            return and;
        }

        public string Or(string x, string y, bool comit = false)
        {
            var and = x + " 1 = " + y + " 1 =" + " + 0 > ";
            if (comit)
            {
                worker.AppendLine(_tabs + and);
            }
            return and;
        }

        // 1 or 0 to Stack
        public string NotEqual(string x, string y, bool comit = false) // 
        {
            var areNotEqual = Equal(x, y) + " 1 < ";

            if (comit)
            {
                worker.AppendLine(_tabs + areNotEqual);
            }
            return areNotEqual;
        }

        public string Equal(string x, string y, bool comit = false) // 
        {
            var areEqual = x + " " + y + " = ";
            if (comit)
            {
                worker.AppendLine(_tabs + areEqual);
            }
            return areEqual;
        }

        public string Add(string x, string y, bool comit = false) // 
        {
            var areEqual = x + " " + y + " + ";
            if (comit)
            {
                worker.AppendLine(_tabs + areEqual);
            }
            return areEqual;
        }

        public string Subtract(string x, string y, bool comit = false) // 
        {
            var areEqual = x + " " + y + " - ";
            if (comit)
            {
                worker.AppendLine(_tabs + areEqual);
            }
            return areEqual;
        }
        public string Multiply(string x, string y, bool comit = false) // 
        {
            var areEqual = x + " " + y + " * ";
            if (comit)
            {
                worker.AppendLine(_tabs + areEqual);
            }
            return areEqual;
        }

        public string Declare(string variable, bool comit = false)
        {
            variable = context + variable;
            if(Words.Contains(variable))
            {
                throw new InvalidDataException("This variable is already declared: " + variable);
            }

            var code = "variable " + variable;
            if (comit)
            {
                declarations.AppendLine(_tabs + code);
            }            
            return code;
        }

        public string DeclareArray(string variable, int size, bool comit= false)
        {
            variable = context + variable;
            if (Words.Contains(variable))
            {
                throw new InvalidDataException("This variable is already declared: " + variable);
            }

            var code = "variable " + variable + " " + size + " cells";
            if (comit)
            {
                declarations.AppendLine(_tabs + code);
            }
            return code;
        }

        // Nothing on stack
        public string Increment(string x, bool comit = false)
        {
            var code = x + " @ 1 + " + x + " ! ";
            if (comit)
            {
                worker.AppendLine(_tabs + code);
            }
            return code;
        }

        // Nothing on stack
        public string Increment(string x, int value, bool comit = false)
        {
            var code = x + " @ " + value + " + " + x + " ! ";
            if (comit)
            {
                worker.AppendLine(_tabs + code);
            }
            return code;
        }

        public string Increment(string x, string expresion, bool comit = false)
        {
            var code = x + " @ " + expresion + " + " + x + " ! ";
            if (comit)
            {
                worker.AppendLine(_tabs + code);
            }
            return code;
        }

        // Returns to stack
        public string GetArrayValue(string arrayAddr, string index, bool comit = false)
        {
            var code = arrayAddr + " " + index + " + @ ";
            if (comit)
            {
                worker.AppendLine(_tabs + code);
            }
            return code;
        }

        // no prereq on stack
        public string SetArrrayValue(string arrayAddr, string index, string expresion, bool comit = false)
        {
            var code = expresion + " " + arrayAddr + " " + index + " + ! ";
            if (comit)
            {
                worker.AppendLine(_tabs + code);
            }
            return code;
        }

        // nothing on stack needed nor left
        public string Set(string addr, string value, bool comit = false)
        {
            var code = value + " " + addr + " ! ";
            worker.AppendLine(_tabs + code);
            return code;
        }

        // nothing on stack needed nor left
        public string Set(string addr, int val, bool comit = false)
        {
            var code = val + " " + addr + " ! ";
            if (comit)
            {
                worker.AppendLine(_tabs + code);
            }
            return code;
        }

        // Adds to stack
        public string Get(string addr, bool comit = false)
        {
            var code = addr + " @ ";
            if (comit)
            {
                worker.AppendLine(_tabs + code);
            }
            return code;
        }

        public void Push(int i, bool comit = false)
        {
            if (comit)
            {
                worker.AppendLine(_tabs + i);
            }
        }

        public void Push(string expresion, bool comit = false)
        {
            if (comit)
            {
                worker.AppendLine(_tabs + expresion);
            }
        }
    }
}
