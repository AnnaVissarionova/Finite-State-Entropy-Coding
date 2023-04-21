using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finite_State_Entropy_Coding
{
    public class CodingChars
    {


        public int f_cond { get; set; } = -1;
       // public char lastChar;
        char[] alph;
        Dictionary<char, int[]> q_table;
        Dictionary<char, int[]> q_arr;
        Dictionary<char, string[]> encoding;
        Dictionary<char, int[]> q_amounts;


        public CodingChars(int q, char[] symbls, int[] prob)
        {
            f_cond = q;
            alph = symbls;
            q_table = DefineInputOutputConditionsForSymb(symbls, prob);  //таблица входных-выходных состояний для каждого символа
            encoding = DefineConditionsEncoding(symbls, prob);           //таблица кодировок состояний для каждого символа
            q_arr = DefineArrOfConditionsForSymb(symbls, prob);          //словарь вариаций состояний для каждого символа  
            q_amounts = DefineAmountOfConditionsForSymb(symbls, prob);   //словарь количества состояний для каждого символа 

        }

        public int GetFirstCond(char f_char)
        {
            var res = 0;
            foreach(var i in q_arr)
            {
                if (i.Key == f_char)
                {
                    res = i.Value[0];
                }
            }
            return res;
        }

       
        public string CodeStringCycle(int q, string s)
        {
            var res = "";
            while (s.Length > 0)
            {
                res += encoding.GetValueOrDefault(s[0])[q - 1];
                q = q_table.GetValueOrDefault(s[0])[q - 1];
                f_cond = q;
                s = s[1..] ?? "";
            }
            return res;
        }


        public string DecodeStringCycle(int q, string s)
        {
           
            var res = "";
            var lastChar = FindChar(q);
            while (s.Length > 0)
            {
                var ind = FindIndex(lastChar, s, q, q_arr.GetValueOrDefault(lastChar)[0]);
                var prevCond = ind + 1;
                s = s[..^(encoding.GetValueOrDefault(lastChar)[ind]).Length];
                res = lastChar + res;
                lastChar = FindChar(prevCond);
                q = prevCond;
            }
            return res;
        }




        //поиск предыдущего символа по выходному состоянию
        char FindChar(int n)
        {
            foreach (var e in q_table)
            {
                if (e.Value.Contains(n))
                {
                    return e.Key;
                }
            }
            return '(';
        }

        
        //создает словарь "символ - массив из чисел", где каждое число - одно из состояний, соотв символу согласно частоте его появления
        public static Dictionary<char, int[]> DefineArrOfConditionsForSymb(char[] arr, int[] prob)
        {
            var dict = new Dictionary<char, int[]>();
            var ind = 0;
            for (var i = 0; i < arr.Length; i++)
            {
                dict.Add(arr[i], GenerateArrOfCond(prob[i], ind));
                ind += prob[i];
            }
            return dict;
        }

        //создает словарь вида "символ - массив из чисел", где каждое число - выходное состояние, каждая позиция числа - входное состояние
        public static Dictionary<char, int[]> DefineInputOutputConditionsForSymb(char[] symbls, int[] prob)
        {
            var dict = new Dictionary<char, int[]>();
            var total = prob.Sum();
            var start_num = 1;
            for (var i = 0; i < symbls.Length; i++)
            {
                var counts = GenerateAmountsOfOneNumConditions(total, prob[i]);
                var nums_arr = new int[total];
                var start_ind = 0;

                for (var j = 0; j < prob[i]; j++)
                {
                    for (var k = 0; k < counts[j]; k++)
                    {
                        nums_arr[start_ind + k] = j + start_num;
                    }
                    start_ind += counts[j];
                }
                dict.Add(symbls[i], nums_arr);
                start_num += prob[i];
            }
            return dict;
        }

        //создает кодировку - словарь вида "символ - массив из строк", где каждая строка - двоичный код символа
        //(который зависит от входного состояния), каждая позиция числа - входное состояние
        public static Dictionary<char, string[]> DefineConditionsEncoding(char[] symbls, int[] prob)
        {
            var res = new Dictionary<char, string[]>();
            int total = prob.Sum();
            for (var i = 0; i < symbls.Length; i++)
            {
                var coding = new string[0];
                var totals = GenerateAmountsOfOneNumConditions(total, prob[i]);

                for (var j = 0; j < prob[i]; j++)
                {
                    foreach (var s in CreateCoding(totals[j]))
                    {
                        coding = coding.Append(s).ToArray();
                    }
                }

                res.Add(symbls[i], coding);
            }
         
            return res;
        }

        //создает словарь вида "символ - массив чисел", где каждое число - количество вариаций одного состояния
        //пример: символ А встречается с вероятностью 6 из 8, символ B встречается с вероятностью 2 из 8,
        //для 'A' определено 6 состояний от 1 до 6, для 'B' определено 2 состояний от 7 до 8
        //результатом функции будет словарь из двух пар : ('A', [1,1,1,1,2,2]) и ('B', [4,4])
        static Dictionary<char, int[]> DefineAmountOfConditionsForSymb(char[] symbls, int[] prob)
        {
            var dict = new Dictionary<char, int[]>();
            var total = prob.Sum();
            for (var i = 0; i < symbls.Length; i++)
            {
                var counts = GenerateAmountsOfOneNumConditions(total, prob[i]);
                dict.Add(symbls[i], counts);
            }
            return dict;
        }

        //поиск индекса предыдущего состояния
        int FindIndex(char c, string s, int q, int startNum)
        {
            var codes = encoding.GetValueOrDefault(c);
            var ind = q_amounts.GetValueOrDefault(c)[0..(q - startNum)].Sum();
            codes = codes[ind..];
            for (var i = 0; i < q_table.GetValueOrDefault(c)[^1]; i++)
            {
                if (s.EndsWith(codes[i]))
                {
                    return i + ind;
                }
            }
            return -1;
        }

        //Создает кодировку всех вариаций (total) одного состояния
        static string[] CreateCoding(int total)
        {
            if (total == 1)
            {
                return new string[] { "1" };
            }
            var res = new string[total];
            for (var i = 0; i < total; i++)
            {
                var s = "";
                var n = i;
                while (n > 0)
                {
                    s = (n % 2) + s;
                    n /= 2;
                }

                while (s.Length < Math.Round(Math.Floor(Math.Log2(total))))
                {
                    s = "0" + s;
                }
                res[i] = s;
            }
            return res;
        }

        //Создает массив из чисел, каждое число - количество позиций в таблице состояний, которое занимает одно состояние из всех
        //пример : символ А встречается с вероятностью 6 из 16, для 'A' определено 6 состояний от 1 до 6
        //состояние 1 в таблице состояний занимает 2 позиции, то же самое для состояний 2, 3 и 4
        //а состояния 5 и 6 занимают по 4 позиции ( итого 6 состояний занимают все 16 возможных позиций)
        //массив [2, 2, 2, 2, 4, 4] и будет результатом функции
        static int[] GenerateAmountsOfOneNumConditions(int total, int count)
        {

            var res = new int[count];
            if (count == total)   //вариант, когда состояний столько же, сколько и позиций в таблице состояний
            {
                for (var i = 0; i < count; i++)
                {
                    res[i] = 1;
                }
            }
            else if (count > total / 2)  //вариант, когда состояний больше половины позиций в таблице состояний
            {
                res = GenerateAmountsOfOneNumConditions(total / 2, total / 2).Concat(GenerateAmountsOfOneNumConditions(total / 2, count - total / 2)).ToArray();
            }
            else if ((total % count == 0) && (IsPowOfTwo(total / count))) //вариант, когда количество позиций под каждое состояние можно поделить поровну
            {
                var amount = total / count;
                for (var i = 0; i < count; i++)
                {
                    res[i] = amount;
                }
            }
            else
            {
                var amount = (int)Math.Pow(2, Math.Round(Math.Floor(Math.Log2(total / count))));
                for (var j = 0; j < count; j++)
                {
                    res[j] = amount;
                }
                var last = total - count * amount;
                var i = count - 1;
                while (i > 0)
                {
                    if (IsPowOfTwo(amount + last / (count - i)))
                    {
                        for (var j = count - 1; j >= i; j--)
                        {
                            res[j] += last / (count - i);
                        }
                    }
                    i--;
                }
            }

            return res;
        }

        static bool IsPowOfTwo(int n)
        {
            return (Math.Round(Math.Ceiling(Math.Log2(n))) == Math.Round(Math.Floor(Math.Log2(n))));
        }

        static int[] GenerateArrOfCond(int n, int start)
        {
            var res = new int[0];
            for (var i = start + 1; i < n + start + 1; i++)
            {
                res = res.Append(i).ToArray();
            }
            return res;
        }









        // методы для печати информации о кодировании

        public void PrintCodingDict()
        {
            foreach (var c in alph)
            {
                Console.WriteLine($"char {c} : ");
                for (var i = 0; i < q_table.GetValueOrDefault(c).Length; i++)
                {
                    Console.WriteLine($"  q{i + 1} -> q{q_table.GetValueOrDefault(c)[i]} : {encoding.GetValueOrDefault(c)[i]}");
                }

                Console.WriteLine();
            }
        }

        public void PrintCondTable()
        {
            int[] width = new int[0];
            for (var i = 0; i < q_arr.Count; i++)
            {
                width = width.Append(q_arr.GetValueOrDefault(alph[i]).Length).ToArray();
                //Console.WriteLine($"#{alph[i]} {q_arr.GetValueOrDefault(alph[i]).Length}");
            }

            var maxint = q_table.GetValueOrDefault(alph[^1])[^1];
            var size = Math.Round(Math.Ceiling(Math.Log10(maxint))) == Math.Round(Math.Floor(Math.Log10(maxint))) ? Math.Round(Math.Log10(maxint)) + 1 : Math.Round(Math.Ceiling(Math.Log10(maxint)));

            var line1 = new string[0];
            Console.Write("  |");
            for (var i = 0; i < width.Length; i++)
            {

                Console.Write(alph[i]);
                for (var j = 0; j < (size + 1) * width[i] - 2; j++)
                {
                    Console.Write($" ");
                }
                Console.Write("|");
            }
            Console.WriteLine();
            for (var k = 0; k < ((size + 1) * width.Sum() + 3); k++)
            {
                Console.Write("-");
            }
            Console.WriteLine();



            Console.Write("  |");
            foreach (var c in alph)
            {
                var nums_arr = q_table.GetValueOrDefault(c).Distinct().ToArray();

                for (var i = 0; i < nums_arr.Length; i++)
                {
                    var sint = nums_arr[i].ToString();
                    while (sint.Length < size)
                    {
                        sint = " " + sint;
                    }
                    Console.Write($"{sint}|");
                }
            }
            Console.WriteLine();
            for (var k = 0; k < ((size + 1) * width.Sum() + 3); k++)
            {
                Console.Write("-");
            }
            Console.WriteLine();
            foreach (var c in alph)
            {
                Console.Write($"{c} |");

                var nums_arr = q_arr.GetValueOrDefault(c);
                for (var i = 0; i < nums_arr.Length; i++)
                {
                    for (var j = 0; j < q_amounts.GetValueOrDefault(c)[i]; j++)
                    {
                        var sint = nums_arr[i].ToString();
                        while (sint.Length < size)
                        {
                            sint = " " + sint;
                        }
                        Console.Write($"{sint}|");
                    }
                }

                Console.WriteLine();
                for (var k = 0; k < ((size + 1) * width.Sum() + 3); k++)
                {
                    Console.Write("-");
                }
                Console.WriteLine();

            }
        }

    }

}
