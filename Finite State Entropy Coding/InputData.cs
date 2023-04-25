using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finite_State_Entropy_Coding
{
    public class InputData
    {

        public static void CodingProg()
        {
            var inp = "";
            string[] inp_ss = new string[3];
            Console.Write("Введите количество символов в алфавите (целое число) : ");
            inp_ss[0] = Console.ReadLine();
            Console.Write("Введите символы алфавита и их частоты (в виде A:3 B:1 ... Z:1) : ");
            inp_ss[1] = Console.ReadLine();
            Console.Write("Введите строку, которую необходимо сжать : ");
            inp_ss[2] = Console.ReadLine();

            (char[] charArr, int[] prob, string text) = InputData.ParseData(inp_ss);
            var codingObj = new CodingChars(0, charArr, prob);
            codingObj.f_cond = codingObj.GetFirstCond(text[0]);
            var coded = codingObj.CodeStringCycle(codingObj.f_cond, text);

            Console.WriteLine($"---> Конечное состояние : {codingObj.f_cond}");
            Console.WriteLine($"---> Сжатая строка : {coded}");
            Console.WriteLine();

            Console.Write("Вывести информацию о кодировке? (Y/N) : ");
            if (Console.ReadLine().Equals("Y"))
            {
                Console.WriteLine();
                PrintEncodingInfo(codingObj);
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

        public static void DecodingProg()
        {
            var inp = "";
            string[] inp_ss = new string[4];
            Console.Write("Введите количество символов в алфавите (целое число) : ");
            inp_ss[0] = Console.ReadLine();
            Console.Write("Введите символы алфавита и их частоты (в виде A:3 B:1 ... Z:1) : ");
            inp_ss[1] = Console.ReadLine();
            Console.Write("Введите строку, которую необходимо расжать : ");
            inp_ss[2] = Console.ReadLine();
            Console.Write("Введите финальное состояние  : ");
            inp_ss[3] = Console.ReadLine();


            (char[] charArr, int[] prob, string text) = InputData.ParseData(inp_ss);
            int q = int.Parse(inp_ss[3]);
            var decodingObj = new CodingChars(q, charArr, prob);
            var decoded = decodingObj.DecodeStringCycle(q, text);
            
            Console.WriteLine();
            Console.WriteLine($"---> Расжатая строка : {decoded}");

            Console.WriteLine();

            Console.Write("Вывести информацию о кодировке? (Y/N) : ");
            if (Console.ReadLine().Equals("Y"))
            {
                Console.WriteLine();
                PrintEncodingInfo(decodingObj);
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();


        }

        static void PrintEncodingInfo(CodingChars codingObj)
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~~ Таблица состояний ~~~~~~~~~~~~~~~~~");
            Console.WriteLine();
            codingObj.PrintCondTable();
            Console.WriteLine();

            Console.WriteLine("~~~~~~~~~~~~~~~~~ Таблица кодировок ~~~~~~~~~~~~~~~~~");
            Console.WriteLine();
            codingObj.PrintCodingDict();
        }


        static (char[], int[], string s) ParseData(string[] ss)
        {
            var charArr = new char[int.Parse(ss[0])];
            var prob = new int[charArr.Length];
            var groups = ss[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
           for(var i = 0; i < charArr.Length; i++)
            {
                charArr[i] = groups[i][0];
                prob[i] = int.Parse(groups[i].Split(':')[1]);
            }
            return (charArr, prob, ss[2]);
        }

    }
}
