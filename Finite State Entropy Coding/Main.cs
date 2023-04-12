using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Finite_State_Entropy_Coding
{
    internal class main
    {

        static void Main(string[] args)
        {
            var inp = "";
            while(!inp.Equals("exit"))
            {
                Console.Write("Введите команду (1 - для сжатия 2 - для расжатия, exit - для выхода) : ");
                inp = Console.ReadLine();
                if(inp.Equals("1"))
                {
                    InputData.CodingProg();
                }
                else if (inp.Equals("2"))
                {
                    InputData.DecodingProg();
                }
            }
            
        }

        
    }


    
}
