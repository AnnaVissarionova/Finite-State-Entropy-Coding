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
            /* var dict = DefineCondForSymb(new char[] { 'A', 'B', 'C', 'D' }, new int[] { 3, 2, 1, 4 });
             foreach (var d in dict) {
                 foreach(var a in d.Value)
                 {
                     Console.Write($"{a} ");
                 }
                 Console.WriteLine();
             }*/

            /* var arr = GenerateArr(16, 13);
             foreach(var i in arr)
             {
                 Console.Write($"{i} ");
             }*/

            var dict = DefineInputCondForSymb(new char[] { 'A', 'B', 'C', 'D', 'E' }, new int[] { 6, 1, 3, 4, 2});
            foreach (var d in dict) {
                foreach(var a in d.Value)
                {
                    Console.Write($"{a} ");
                }
                Console.WriteLine();
            }
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
