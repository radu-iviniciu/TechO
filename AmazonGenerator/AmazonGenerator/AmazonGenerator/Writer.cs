using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AmazonGenerator
{
    class Writer
    {
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

        private StreamWriter _file;
        private int _indetation;

        private string _tabs;

        public Writer(StreamWriter stream)
        {
            _file = stream;
            Indetation = 0;
        }

        public void Close()
        {
            _file.Flush();
            _file.Close();
        }

        public string EndIf(bool comit)
        {
            if (comit)
            {
                _file.WriteLine(_tabs + "then");
            }
            Indetation--;
            return " then";
        }

        public string If(string condition, bool comit = false)
        {
            string res = condition + " " + "if";
            if (comit)
            {
                _file.WriteLine(_tabs + res);
            }
            Indetation++;
            return res;
        }

        public string Not(string x, bool comit = false)
        {
            var not = x + " 1 <";
            if (comit)
            {
                _file.WriteLine(_tabs + not);
            }
            return not;
        }

        // a condininal expresion on the stack
        public string While(string condition, bool comit)
        {
            var res = condition + " while";
            if (comit)
            {
                _file.WriteLine(_tabs + res);
            }
            Indetation++;
            return res;            
        }

        public void Repeat(string condition, bool comit)
        {

            var res = condition + " repeat";
            if (comit)
            {
                _file.WriteLine(_tabs + res);
            }
            Indetation--;
        }

        public string Comment(string comment, bool comit = false)
        {
            string res = "(" + comment + ")";
            if (comit)
            {
                _file.WriteLine(_tabs + res);
            }
            return res;
        }

        public  string GreaterEqual(string x, string y, bool comit = false) // 
        {
            var areEqual = x + " " + y + " >= ";
            if (comit)
            {
                _file.WriteLine(_tabs + areEqual);
            }
            return areEqual;
        }

        public string And(string x, string y, bool comit = false)
        {
            var and = x + " = 1 " + y + " 1 =" + " 1 = ";
            if (comit)
            {
                _file.WriteLine(_tabs + and);
            }
            return and;
        }

        // 1 or 0 to Stack
        public string AreNotEqual(string x, string y, bool comit = false) // 
        {
            var areNotEqual = AreEqual(x, y) + " 1 < ";

            if (comit)
            {
                _file.WriteLine(_tabs + areNotEqual);
            }
            return areNotEqual;
        }

        public string AreEqual(string x, string y, bool comit = false) // 
        {
            var areEqual = x + " " + y + " = ";
            if (comit)
            {
                _file.WriteLine(_tabs + areEqual);
            }
            return areEqual;
        }

        public string Declare(string variable, bool comit = false)
        {
            var code = "variable " + variable;
            if (comit)
            {
                _file.WriteLine(_tabs + code);
            }            
            return code;
        }

        public string DeclareArray(string variable, int size, bool comit= false)
        {
            var code = "variable " + variable + " " + size + " cells";
            if (comit)
            {
                _file.WriteLine(_tabs + code);
            }
            return code;
        }

        // Nothing on stack
        public string Increment(string x, bool comit = false)
        {
            var code = x + " @ 1 + " + x + " ! ";
            if (comit)
            {
                _file.WriteLine(_tabs + code);
            }
            return code;
        }

        // Nothing on stack
        public string Increment(string x, int value, bool comit = false)
        {
            var code = x + " @ " + value + " + " + x + " ! ";
            if (comit)
            {
                _file.WriteLine(_tabs + code);
            }
            return code;
        }

        public string Increment(string x, string expresion, bool comit = false)
        {
            var code = x + " @ " + expresion + " + " + x + " ! ";
            if (comit)
            {
                _file.WriteLine(_tabs + code);
            }
            return code;
        }

        // Returns to stack
        public string GetArrayValue(string arrayAddr, string index, bool comit = false)
        {
            var code = arrayAddr + " " + index + " + @ ";
            if (comit)
            {
                _file.WriteLine(_tabs + code);
            }
            return code;
        }

        // no prereq on stack
        public string SetArrraValue(string arrayAddr, string index, string expresion, bool comit = false)
        {
            var code = expresion + " " + arrayAddr + " " + index + " + ! ";
            if (comit)
            {
                _file.WriteLine(_tabs + code);
            }
            return code;
        }

        // nothing on stack needed nor left
        public string Set(string addr, string value, bool comit = false)
        {
            var code = value + " " + addr + " ! ";
            _file.WriteLine(_tabs + code);
            return code;
        }

        // nothing on stack needed nor left
        public string Set(string addr, int val, bool comit = false)
        {
            var code = val + " " + addr + " ! ";
            if (comit)
            {
                _file.WriteLine(_tabs + code);
            }
            return code;
        }

        // Adds to stack
        public string Get(string addr, bool comit = false)
        {
            var code = addr + " @ ";
            if (comit)
            {
                _file.WriteLine(_tabs + code);
            }
            return code;
        }
    }
}
