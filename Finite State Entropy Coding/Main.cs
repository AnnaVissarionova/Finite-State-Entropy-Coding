using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finite_State_Entropy_Coding
{
    internal class main
    {

        //Dictionary<char, int[]> dict3 = DefineAmountOfPos(new char[] { 'A', 'B', 'C', 'D', 'E' }, new int[] { 6, 1, 3, 4, 2 });
        static void Main(string[] args)
        {
            
            /*var s = "1001000010110";
            Console.WriteLine(s);
            Compression.Compress(s, 0, 'a');
            Console.WriteLine("res " + Compression.Decompress());*/

            /*   var arr = GenerateArr(16, 1);
                foreach(var i in arr)
                {
                    Console.Write($"{i} ");
                }*/

             var dict = DefineInputCondForSymb(new char[] { 'A', 'B', 'C', 'D', 'E' }, new int[] { 6, 1, 3, 4, 2});
            /*foreach (var e in dict) {
                foreach(var a in e.Value)
                {
                    Console.Write($"{a} ");
                }
                Console.WriteLine();
            }*/
            var dict1 = DefineCondForSymb(new char[] { 'A', 'B', 'C', 'D', 'E' }, new int[] { 6, 1, 3, 4, 2 });
           /* foreach(var t in dict1)
            {
                Console.Write(t.Key + " ");
                foreach(var x in t.Value)
                {
                    Console.Write($"{x} ");
                }
            }*/

            var dict2 = DefineCondCoding(new char[] { 'A', 'B', 'C', 'D', 'E' }, new int[] { 6, 1, 3, 4, 2 }, 16);
            var d = dict2.GetValueOrDefault('A');
            /*foreach (var a in d)
            {
                Console.Write($"{a} ");
            }
            Console.WriteLine();*/

            var dict3 = DefineAmountOfPos(new char[] { 'A', 'B', 'C', 'D', 'E' }, new int[] { 6, 1, 3, 4, 2 });
            /* foreach (var a in dict3)
             {
                 foreach(var p in a.Value)
                 {
                     Console.Write($"{p} ");
                 }
             }
             Console.WriteLine();*/
            /* var ind = 8;
             Console.WriteLine(dict2.GetValueOrDefault('C')[ind-1]);*/

            /*  var arr = CreateCoding(8);
              foreach(var s in arr)
              {
                  Console.WriteLine(s);
              }*/

            var text = Compression.ReadTextFile("tt.txt");
            var codedText = CodeString(4, text, dict2, dict);
            Console.WriteLine(text);
            Compression.Compress(codedText, 8, 'C');
            var tt = Compression.Decompress();
            (int q, char lc, int extra) = Compression.GetComprInfo();
            tt = tt[0..^extra];
            var decoded = DecodeString(q, tt, lc, dict2, dict, dict1, dict3);
            Console.WriteLine(decoded);
            Console.WriteLine(text.Equals(decoded));

            // Console.WriteLine(CodeString(4, "ABEDA", dict2, dict));
           // Console.WriteLine(DecodeString(6, "100011101001", 'A', dict2, dict, dict1, dict3));
            
        }


        static string CodeString(int q, string s, Dictionary<char, string[]> encoding, Dictionary<char, int[]> q_table)
        {

            if (s.Equals(""))
            {
                //return $"end:{q}";
                Console.WriteLine($"llast cond {q}");
                return $"";
            }

            return encoding.GetValueOrDefault(s[0])[q - 1] + CodeString(q_table.GetValueOrDefault(s[0])[q - 1], s[1..] ?? "", encoding, q_table);
        }


        static Dictionary<char, int[]> DefineAmountOfPos(char[] symbls, int[] prob)
        {
            var dict = new Dictionary<char, int[]>();
            var total = prob.Sum();
            for (var i = 0; i < symbls.Length; i++)
            {
                var counts = GenerateAmountsOfPos(total, prob[i]);
                dict.Add(symbls[i], counts);
            }
            return dict;
        }

        //поиск индекса предыдущего состояния
        static int FindIndex(char c, string s, int q, Dictionary<char, string[]> encoding, Dictionary<char, int[]> q_table, Dictionary<char, int[]> dict3, int startNum)
        {
            var codes = encoding.GetValueOrDefault(c);
            var ind = dict3.GetValueOrDefault(c)[0..(q - startNum)].Sum();
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

        //поиск предыдущего символа по выходному состоянию
        static char FindChar(int n, Dictionary<char, int[]> dict)
        {
            foreach(var e in dict)
            {
                if (e.Value.Contains(n))
                {
                    return e.Key;
                }
            }
            return '(';
        }

        static string DecodeString(int q, string text, char lastChar, Dictionary<char, string[]> encoding, Dictionary<char, int[]> q_table, Dictionary<char, int[]> q_arr, Dictionary<char, int[]> dict3)
        {
            if (text.Equals(""))
            {
                return $"";
            }
            var ind = FindIndex(lastChar, text, q, encoding, q_table, dict3, q_arr.GetValueOrDefault(lastChar)[0]);
            var prevCond = ind + 1;
            text = text[..^(encoding.GetValueOrDefault(lastChar)[ind]).Length];
           
            return DecodeString(prevCond, text, FindChar(prevCond, q_arr), encoding, q_table, q_arr, dict3) + lastChar;
        }


        //создает словарь "символ - массив из чисел", где каждое число - одно из состояний, соотв символу согласно частоте его появления
        static Dictionary<char, int[]> DefineCondForSymb(char[] arr, int[] prob)
        {
            var dict = new Dictionary<char, int[]>();
            var ind = 0;
            for(var i = 0; i < arr.Length; i++)
            {
                dict.Add(arr[i], GenerateArrOfCond(prob[i], ind));
                ind += prob[i];
            }
            return dict;
        }

        //создает словарь вида "символ - массив из чисел", где каждое число - выходное состояние, каждая позиция числа - входное состояние
        static Dictionary<char, int[]> DefineInputCondForSymb(char[] symbls, int[] prob)
        {
            var dict = new Dictionary<char, int[]>();
            var total = prob.Sum();
            var start_num = 1;
            for (var i = 0; i < symbls.Length; i++)
            {
                var counts = GenerateAmountsOfPos(total, prob[i]);
                /*Console.Write(arr[i]);
                foreach(var e in counts)
                {
                    Console.Write($"{e} ");
                }
                Console.WriteLine();*/
                var nums_arr = new int[total];
                var start_ind = 0;
               
                for(var j = 0; j < prob[i]; j++)
                {
                    //Console.WriteLine($"{arr[i]} {counts.Length} ");
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

        //создает кодировку - словарь вида "символ - массив из строк", где каждая строка - двоичный код символа (который зависит от входного состояния), каждая позиция числа - входное состояние
        static Dictionary<char, string[]> DefineCondCoding(char[] symbls, int[] prob, int total)
        {
            var res = new Dictionary<char, string[]>();
            for (var i = 0; i < symbls.Length; i++)
            {
               // Console.Write(arr[i] + ": ");
                //Console.WriteLine($"{e.Value.Length}");
                var coding = new string[0];
                var totals = GenerateAmountsOfPos(total, prob[i]);

                for (var j = 0; j < prob[i]; j++)
                {
                    foreach (var s in CreateCoding(totals[j]))
                    {
                        coding = coding.Append(s).ToArray();
                        //Console.WriteLine(s);
                    }
                }

                res.Add(symbls[i], coding);
            }
            /*foreach(var e in arr)
            {
                Console.Write(e + ": ");
                //Console.WriteLine($"{e.Value.Length}");
                var coding = new string[0];
                for(var i = 0; i < prob[]; i++)
                {
                    foreach (var s in CreateCoding(e.Value[i]))
                    {
                        coding = coding.Append(s).ToArray();
                        //Console.WriteLine(s);
                    }
                }
                
                res.Add(e.Key, coding);
            }*/
            return res;
        }


        static string[] CreateCoding(int total)
        {
            var res = new string[total];
            for(var i = 0; i < total; i++)
            {
                var s = "";
                var n = i;
                while (n > 0)
                {
                    s = (n % 2) + s;
                    n /= 2;
                }
                
                while(s.Length < Math.Round(Math.Floor(Math.Log2(total))))
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
        static int[] GenerateAmountsOfPos(int total, int count)
        {

            var res = new int[count];
            if (count == total)   //вариант, когда состояний столько же, сколько и позиций в таблице состояний
            {
                for(var i = 0; i < count; i++)
                {
                    res[i] = 1;
                }
            }
            else if (count > total / 2)  //вариант, когда состояний больше половины позиций в таблице состояний
            {
                res = GenerateAmountsOfPos(total / 2, total / 2).Concat(GenerateAmountsOfPos(total / 2, count - total / 2)).ToArray();
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
                var amount = (int) Math.Pow(2, Math.Round(Math.Floor(Math.Log2(total / count))));
                for(var j = 0; j < count; j++)
                {
                    res[j] = amount;
                }
                var last = total - count * amount;
                var i = count - 1;
                while (i > 0)
                {
                    if (IsPowOfTwo(amount + last / (count -i)))
                    {
                        for(var j = count - 1; j >= i; j--)
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
            for(var i = start+1; i < n+start+1; i++)
            {
                res = res.Append(i).ToArray();
            }
            return res;
        }

    }


    
}
