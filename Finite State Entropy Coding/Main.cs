using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finite_State_Entropy_Coding
{
    internal class main
    {

        static void Main(string[] args)
        {
            

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
           // var dict1 = DefineCondForSymb(new char[] { 'A', 'B', 'C', 'D', 'E' }, new int[] { 6, 1, 3, 4, 2 });
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
           /* var ind = 8;
            Console.WriteLine(dict2.GetValueOrDefault('C')[ind-1]);*/

            /*  var arr = CreateCoding(8);
              foreach(var s in arr)
              {
                  Console.WriteLine(s);
              }*/

            var s = "ww";
            //Console.WriteLine(s[2..].Equals(""));

            Console.WriteLine(CodeString(4, "ABEDA", dict2, dict));
        }


        static string CodeString(int q, string s, Dictionary<char, string[]> encoding, Dictionary<char, int[]> q_table)
        {
            if (s.Equals(""))
            {
                return "";
            }
            
            return encoding.GetValueOrDefault(s[0])[q - 1] + CodeString(q_table.GetValueOrDefault(s[0])[q-1], s[1..] ?? "", encoding, q_table);
        }


        static Dictionary<char, int[]> DefineCondForSymb(char[] arr, int[] prob)
        {
            var dict = new Dictionary<char, int[]>();
            var ind = 0;
            for(var i = 0; i < arr.Length; i++)
            {
                dict.Add(arr[i], GenereteArr(prob[i], ind));
                ind += prob[i];
            }
            return dict;
        }

        //создает словарь вида "символ - массив из чисел", где каждое число - выходное состояние, каждая позиция числа - входное состояние
        static Dictionary<char, int[]> DefineInputCondForSymb(char[] arr, int[] prob)
        {
            var dict = new Dictionary<char, int[]>();
            var sum = prob.Sum();
            var start_num = 1;
            for (var i = 0; i < arr.Length; i++)
            {
                var counts = GenerateArr(sum, prob[i]);
                /*Console.Write(arr[i]);
                foreach(var e in counts)
                {
                    Console.Write($"{e} ");
                }
                Console.WriteLine();*/
                var nums_arr = new int[sum];
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
                dict.Add(arr[i], nums_arr);
                start_num += prob[i];
            }
            return dict;
        }

        //создает кодировку - словарь вида "символ - массив из строк", где каждая строка - двоичный код символа (который зависит от входного состояния), каждая позиция числа - входное состояние
        static Dictionary<char, string[]> DefineCondCoding(char[] arr, int[] prob, int total)
        {
            var res = new Dictionary<char, string[]>();
            for (var i = 0; i < arr.Length; i++)
            {
               // Console.Write(arr[i] + ": ");
                //Console.WriteLine($"{e.Value.Length}");
                var coding = new string[0];
                var totals = GenerateArr(total, prob[i]);

                for (var j = 0; j < prob[i]; j++)
                {
                    foreach (var s in CreateCoding(totals[j]))
                    {
                        coding = coding.Append(s).ToArray();
                        //Console.WriteLine(s);
                    }
                }

                res.Add(arr[i], coding);
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


       /* static string CreateCoding(int total, int pos)
        {
            var s = pos > 0 ? "" : "0";
            var n = pos - 1;
            while (n > 0)
            {
                s = s + (n % 2);
                n /= 2;
            }
            return s;
        }*/

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


        static int[] GenerateArr(int total, int count)
        {

            var res = new int[count];
            if (count == total)
            {
                for(var i = 0; i < count; i++)
                {
                    res[i] = 1;
                }
            }
            else if (count > total / 2)
            {
                res = GenerateArr(total / 2, total / 2).Concat(GenerateArr(total / 2, count - total / 2)).ToArray();
            }
            else if ((total % count == 0) && (IsPowOfTwo(total / count)))
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
        static int[] GenereteArr(int n, int start)
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
