using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Class1
    {
      
        public int A { get; set; }
        public int B { get; set; }

        public static Class1 operator +(Class1 c1, Class1 c2)
        {
           
            return new Class1 { A=c1.A + c2.A, B=c1.B + c2.B };

        }


    }



}
