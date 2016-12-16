using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AmazonGenerator
{
    public class Writer
    {
        private HashSet<string> Words = new HashSet<string>();

        private Dictionary<string, IEnumerable<string>> methoToParamsList = new Dictionary<string, IEnumerable<string>>();

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
                Declare(parameter);
            }

            Commit("\n: " + name);
        }

        public void EndMethod()
        {
            Commit(";\n\n");
            main.Insert(0, worker.ToString());
            worker = main;
        }

        public void Return(string expresion, bool comit = true)
        {
            Decode(ref expresion);
            if (worker == main)
            {
                throw new InvalidDataException("Cannot return value from main method");
            }
            if (comit)
            {
                Commit(_tabs + expresion);
            }
        }

        public void MethodCall(string name, string returnAddress, string[] parameterValues)
        {
            var paramNames = methoToParamsList[name];

            if(parameterValues.Length != paramNames.Count())
            {
                throw new InvalidDataException("Method called with invalid number of parameters");
            }

            int i = 0;

            Commit(_tabs + "(SETUP PARAMETERS)");

            foreach(var param in paramNames)
            {
                var value = parameterValues[i];
                Decode(ref value);
                Set(param, value);
            }

            Commit(_tabs + "(CALL " + name + " )");
            Commit(_tabs + name + " ");

            if (returnAddress != null)
            {
                Set(returnAddress, ""); // Set Nothing actually means get from stack
            }
        }

        public string Else(bool comit = true)
        {
            Indetation--;
            string result = _tabs + "(ELSE) else";
            if (comit)
            {
                Commit(result);
            }
            Indetation++;
            return result;            
        }

        public string EndIf(bool comit = true)
        {
            Indetation--;
            if (comit)
            {
                Commit(_tabs + "(THEN) then");
            }
            return " then";
        }

        public string If(string condition, bool comit = true)
        {
            string res = "(IF) " + condition + " " + "if";
            if (comit)
            {
                Commit(_tabs + res);
            }
            Indetation++;
            return res;
        }

        public string Not(string x, bool comit = true)
        {
            Decode(ref x);
            var not = x + " 1 <";
            if (comit)
            {
                Commit(_tabs + not);
            }
            return not;
        }

        // a condininal expresion on the stack
        public string While(string condition, bool comit = true)
        {
            var res = "(WHILE)" + condition + " while";
            if (comit)
            {
                Commit(_tabs + res);
            }
            Indetation++;
            return res;            
        }

        public void Repeat(string condition, bool comit = true)
        {
            Indetation--;
            var res = "(REPEAT IF)" + condition + "repeat";
            if (comit)
            {
                Commit(_tabs + res);
            }            
        }

        public string Comment(string comment, bool comit = true)
        {
            string res = "(" + comment + ")";
            if (comit)
            {
                Commit(_tabs + res);
            }
            return res;
        }

        public string GreaterEqual(string x, string y, bool comit = true) // 
        {
            Decode(ref x);
            Decode(ref y);
            var areEqual = x + " " + y + " >= ";
            if (comit)
            {
                Commit(_tabs + areEqual);
            }
            return areEqual;
        }

        public string Less(string x, string y, bool comit = true) // 
        {
            Decode(ref x);
            Decode(ref y);
            var areEqual = x + " " + y + " < ";
            if (comit)
            {
                Commit(_tabs + areEqual);
            }
            return areEqual;
        }

        public string LessOrEqual(string x, string y, bool comit = true) // 
        {
            Decode(ref x);
            Decode(ref y);
            var areEqual = x + " " + y + " <= ";
            if (comit)
            {
                Commit(_tabs + areEqual);
            }
            return areEqual;
        }

        public string Greater(string x, string y, bool comit = true) // 
        {
            Decode(ref x);
            Decode(ref y);
            var areEqual = x + " " + y + " > ";
            if (comit)
            {
                Commit(_tabs + areEqual);
            }
            return areEqual;
        }

        public string And(string x, string y, bool comit = true)
        {
            Decode(ref x);
            Decode(ref y);
            var and = x + " = 1 " + y + " 1 =" + " 1 = ";
            if (comit)
            {
                Commit(_tabs + and);
            }
            return and;
        }

        public string Or(string x, string y, bool comit = true)
        {
            Decode(ref x);
            Decode(ref y);
            var and = x + " 1 = " + y + " 1 =" + " + 0 > ";
            if (comit)
            {
                Commit(_tabs + and);
            }
            return and;
        }

        // 1 or 0 to Stack
        public string NotEqual(string x, string y, bool comit = true) // 
        {
            Decode(ref x);
            Decode(ref y);
            var areNotEqual = Equal(x, y) + " 1 < ";

            if (comit)
            {
                Commit(_tabs + areNotEqual);
            }
            return areNotEqual;
        }

        public string Equal(string x, string y, bool comit = true) // 
        {
            Decode(ref x);
            Decode(ref y);
            var areEqual = x + " " + y + " = ";
            if (comit)
            {
                Commit(_tabs + areEqual);
            }
            return areEqual;
        }

        public string Add(string x, string y, bool comit = true) // 
        {
            Decode(ref x);
            Decode(ref y);
            var areEqual = x + " " + y + " + ";
            if (comit)
            {
                Commit(_tabs + areEqual);
            }
            return areEqual;
        }

        public string Subtract(string x, string y, bool comit = true) // 
        {
            Decode(ref x);
            Decode(ref y);
            var subtract = x + " " + y + " - ";
            if (comit)
            {
                Commit(_tabs + subtract);
            }
            return subtract;
        }
        public string Multiply(string x, string y, bool comit = true) // 
        {
            Decode(ref x);
            Decode(ref y);
            var areEqual = x + " " + y + " * ";
            if (comit)
            {
                Commit(_tabs + areEqual);
            }
            return areEqual;
        }

        public string Declare(string variable, bool comit = true)
        {
            variable = context + variable;
            if(Words.Contains(variable))
            {
                throw new InvalidDataException("This variable is already declared: " + variable);
            }

            Words.Add(variable);

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

            Words.Add(variable);

            var code = "variable " + variable + " " + size + " cells";
            if (comit)
            {
                declarations.AppendLine(_tabs + code);
            }
            return code;
        }

        // Nothing on stack
        public string Increment(string x, bool comit = true)
        {
            var code = x + " @ 1 + " + x + " ! ";
            if (comit)
            {
                Commit(_tabs + code);
            }
            return code;
        }

        // Nothing on stack
        public string Increment(string x, int value, bool comit = true)
        {
            var code = x + " @ " + value + " + " + x + " ! ";
            if (comit)
            {
                Commit(_tabs + code);
            }
            return code;
        }

        public string Increment(string x, string expresion, bool comit = true)
        {
            Decode(ref expresion);
            var code = x + " @ " + expresion + " + " + x + " ! ";
            if (comit)
            {
                Commit(_tabs + code);
            }
            return code;
        }

        // Returns to stack
        public string GetArrayValue(string arrayAddr, string index, bool comit = true)
        {
            Decode(ref index);
            var code = arrayAddr + " " + index + " + @ ";
            if (comit)
            {
                Commit(_tabs + code);
            }
            return code;
        }

        // no prereq on stack
        public string SetArrrayValue(string arrayAddr, string index, string expresion, bool comit = true)
        {
            Decode(ref expresion);
            Decode(ref index);
            var code = expresion + " " + arrayAddr + " " + index + " + ! ";
            if (comit)
            {
                Commit(_tabs + code);
            }
            return code;
        }

        // nothing on stack needed nor left
        public string Set(string addr, string value, bool comit = true)
        {
            Decode(ref value);
            var code = value + " " + addr + " ! ";
            Commit(_tabs + code);
            return code;
        }

        // nothing on stack needed nor left
        public string Set(string addr, int val, bool comit = true)
        {
            var code = val + " " + addr + " ! ";
            if (comit)
            {
                Commit(_tabs + code);
            }
            return code;
        }

        // Adds to stack
        public string Get(string addr, bool comit = true)
        {
            var code = addr + " @ ";
            if (comit)
            {
                Commit(_tabs + code);
            }
            return code;
        }

        public void Push(int i, bool comit = true)
        {
            if (comit)
            {
                Commit(_tabs + i);
            }
        }

        public void Push(string expresion, bool comit = true)
        {
            Decode(ref expresion);
            if (comit)
            {
                Commit(_tabs + expresion);
            }
        }

        private void Decode(ref string expresion)
        {
            if (Words.Contains(expresion))
            {
                expresion = Get(expresion, false);
            }            
        }


        private void Commit(string expresion)
        {
            worker.AppendLine(expresion);
        }
    }
}
