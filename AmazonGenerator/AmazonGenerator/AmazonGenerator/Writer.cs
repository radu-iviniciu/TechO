using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AmazonGenerator
{
    public class Writer
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

        private StringBuilder sb;
        private int _indetation;

        private string _tabs;
        private StreamWriter _fileStream;
        private string _tabsDec;

        public Writer(StreamWriter stream)
        {
            _fileStream = stream;
            sb = new StringBuilder();
            Indetation = 0;
        }

        public void WriteToFile()
        {
            _fileStream.WriteLine(sb.ToString());
            _fileStream.Close();
        }

        public string Else(bool comit = false)
        {
            Indetation--;
            string result = _tabs + "(ELSE) else";
            if (comit)
            {
                sb.AppendLine(result);
            }
            Indetation++;
            return result;            
        }

        public string EndIf(bool comit = false)
        {
            Indetation--;
            if (comit)
            {
                sb.AppendLine(_tabs + "(THEN) then");
            }
            return " then";
        }

        public string If(string condition, bool comit = false)
        {
            string res = "(IF) " + condition + " " + "if";
            if (comit)
            {
                sb.AppendLine(_tabs + res);
            }
            Indetation++;
            return res;
        }

        public string Not(string x, bool comit = false)
        {
            var not = x + " 1 <";
            if (comit)
            {
                sb.AppendLine(_tabs + not);
            }
            return not;
        }

        // a condininal expresion on the stack
        public string While(string condition, bool comit = false)
        {
            var res = "(WHILE)" + condition + " while";
            if (comit)
            {
                sb.AppendLine(_tabs + res);
            }
            Indetation++;
            return res;            
        }

        public void Repeat(string condition, bool comit = false)
        {
            var res = "(REPEAT IF)" + condition + "\n"+ _tabs + "repeat";
            if (comit)
            {
                sb.AppendLine(_tabs + res);
            }
            Indetation--;
        }

        public string Comment(string comment, bool comit = false)
        {
            string res = "(" + comment + ")";
            if (comit)
            {
                sb.AppendLine(_tabs + res);
            }
            return res;
        }

        public  string GreaterEqual(string x, string y, bool comit = false) // 
        {
            var areEqual = x + " " + y + " >= ";
            if (comit)
            {
                sb.AppendLine(_tabs + areEqual);
            }
            return areEqual;
        }

        public string Less(string x, string y, bool comit = false) // 
        {
            var areEqual = x + " " + y + " < ";
            if (comit)
            {
                sb.AppendLine(_tabs + areEqual);
            }
            return areEqual;
        }

        public string LessOrEqual(string x, string y, bool comit = false) // 
        {
            var areEqual = x + " " + y + " <= ";
            if (comit)
            {
                sb.AppendLine(_tabs + areEqual);
            }
            return areEqual;
        }

        public string Greater(string x, string y, bool comit = false) // 
        {
            var areEqual = x + " " + y + " > ";
            if (comit)
            {
                sb.AppendLine(_tabs + areEqual);
            }
            return areEqual;
        }

        public string And(string x, string y, bool comit = false)
        {
            var and = x + " = 1 " + y + " 1 =" + " 1 = ";
            if (comit)
            {
                sb.AppendLine(_tabs + and);
            }
            return and;
        }

        public string Or(string x, string y, bool comit = false)
        {
            var and = x + " 1 = " + y + " 1 =" + " + 0 > ";
            if (comit)
            {
                sb.AppendLine(_tabs + and);
            }
            return and;
        }

        // 1 or 0 to Stack
        public string NotEqual(string x, string y, bool comit = false) // 
        {
            var areNotEqual = Equal(x, y) + " 1 < ";

            if (comit)
            {
                sb.AppendLine(_tabs + areNotEqual);
            }
            return areNotEqual;
        }

        public string Equal(string x, string y, bool comit = false) // 
        {
            var areEqual = x + " " + y + " = ";
            if (comit)
            {
                sb.AppendLine(_tabs + areEqual);
            }
            return areEqual;
        }

        public string Add(string x, string y, bool comit = false) // 
        {
            var areEqual = x + " " + y + " + ";
            if (comit)
            {
                sb.AppendLine(_tabs + areEqual);
            }
            return areEqual;
        }

        public string Subtract(string x, string y, bool comit = false) // 
        {
            var areEqual = x + " " + y + " - ";
            if (comit)
            {
                sb.AppendLine(_tabs + areEqual);
            }
            return areEqual;
        }
        public string Multiply(string x, string y, bool comit = false) // 
        {
            var areEqual = x + " " + y + " * ";
            if (comit)
            {
                sb.AppendLine(_tabs + areEqual);
            }
            return areEqual;
        }

        public string Declare(string variable, bool comit = false)
        {
            var code = "variable " + variable;
            if (comit)
            {
                sb.AppendLine(_tabs + code);
            }            
            return code;
        }

        public string DeclareArray(string variable, int size, bool comit= false)
        {
            var code = "variable " + variable + " " + size + " cells";
            if (comit)
            {
                sb.AppendLine(_tabs + code);
            }
            return code;
        }

        // Nothing on stack
        public string Increment(string x, bool comit = false)
        {
            var code = x + " @ 1 + " + x + " ! ";
            if (comit)
            {
                sb.AppendLine(_tabs + code);
            }
            return code;
        }

        // Nothing on stack
        public string Increment(string x, int value, bool comit = false)
        {
            var code = x + " @ " + value + " + " + x + " ! ";
            if (comit)
            {
                sb.AppendLine(_tabs + code);
            }
            return code;
        }

        public string Increment(string x, string expresion, bool comit = false)
        {
            var code = x + " @ " + expresion + " + " + x + " ! ";
            if (comit)
            {
                sb.AppendLine(_tabs + code);
            }
            return code;
        }

        // Returns to stack
        public string GetArrayValue(string arrayAddr, string index, bool comit = false)
        {
            var code = arrayAddr + " " + index + " + @ ";
            if (comit)
            {
                sb.AppendLine(_tabs + code);
            }
            return code;
        }

        // no prereq on stack
        public string SetArrrayValue(string arrayAddr, string index, string expresion, bool comit = false)
        {
            var code = expresion + " " + arrayAddr + " " + index + " + ! ";
            if (comit)
            {
                sb.AppendLine(_tabs + code);
            }
            return code;
        }

        // nothing on stack needed nor left
        public string Set(string addr, string value, bool comit = false)
        {
            var code = value + " " + addr + " ! ";
            sb.AppendLine(_tabs + code);
            return code;
        }

        // nothing on stack needed nor left
        public string Set(string addr, int val, bool comit = false)
        {
            var code = val + " " + addr + " ! ";
            if (comit)
            {
                sb.AppendLine(_tabs + code);
            }
            return code;
        }

        // Adds to stack
        public string Get(string addr, bool comit = false)
        {
            var code = addr + " @ ";
            if (comit)
            {
                sb.AppendLine(_tabs + code);
            }
            return code;
        }

        public void Push(int i, bool comit = false)
        {
            if (comit)
            {
                sb.AppendLine(_tabs + i);
            }
        }
    }
}
