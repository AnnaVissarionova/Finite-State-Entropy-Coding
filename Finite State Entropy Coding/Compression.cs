using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Finite_State_Entropy_Coding
{
    public class Compression
    {

        public static string ReadTextFile(string path)
        {
            try
            {
                return File.ReadAllLines(path).Aggregate((x, y) => y = x + y);
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message);
            }
            return "error";
        }

        public static void Compress(string bits, int q, char lastChar)
        {
            var len = bits.Length;
            //Console.WriteLine(bits);
            var extra = len % 8 == 0 ? 0 : 8 - len % 8;
            //Console.WriteLine(extra);

            try
            {
                using (BinaryWriter writer = new BinaryWriter(new FileStream("tt.dat", FileMode.Create), Encoding.ASCII))
                using (BinaryWriter writer2 = new BinaryWriter(new FileStream("info.dat", FileMode.Create), Encoding.ASCII))
                {
                    for (var i = 0; i < len / 8; i++)
                    {
                        //Console.WriteLine(bits.Substring(8 * i, 8));
                        byte arr = Convert.ToByte(bits.Substring(8 * i, 8), 2);
                        //Console.WriteLine($"{arr} byte");

                        writer.Write(arr);
                    }
                    if(extra > 0)
                    {
                       // Console.WriteLine($"subs = {8 * (len / 8)}");
                        var s = bits.Substring(8 * (len / 8));
                        //Console.WriteLine($"s = {s}");

                        for (var i = 0; i < extra; i++)
                        {
                            s = s + "0";
                        }
                        //Console.WriteLine(extra);
                        //Console.WriteLine($"s = {s}");
                        writer.Write(Convert.ToByte(s, 2));
                    }
                   /* else
                    {
                        var s = bits.Substring(8 * (len / 8) - 8);
                        writer.Write(Convert.ToByte(s, 2));
                    }*/

                    Console.WriteLine("gfff");
                    writer2.Write(q);
                    writer2.Write(lastChar);
                    writer2.Write(extra);
                    


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static string Decompress()
        {
            var s = "";
            try
            {
                using (BinaryReader reader = new BinaryReader(new FileStream("tt.dat", FileMode.Open), Encoding.ASCII))
                {
                    while (reader.PeekChar() != -1)
                    {
                        var b = reader.ReadByte();
                        //Console.WriteLine(Convert.ToString(b, 2));
                        var ss = Convert.ToString(b, 2);
                        //Console.WriteLine(ss.Length);
                        var len = ss.Length;
                        for(var i = 0; i < (8 - len); i++)
                        {
                            ss = "0" + ss;
                            Console.WriteLine(ss);

                        }
                        //Console.WriteLine("res : " + ss);
                       // Console.WriteLine();
                        s = s + ss;
                    }
                }
                return s;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return s;
        }

        public static (int, char, int) GetComprInfo()
        {
            (int q, char lastChar, int extra) = (-1, '(', -1);
            try
            {
                var fs = new FileStream("info.dat", FileMode.Open);
                using (BinaryReader reader = new BinaryReader(fs, Encoding.ASCII))
                {
                    fs.Seek(0, SeekOrigin.Begin);
                    q = reader.ReadInt32();
                    //Console.WriteLine(q);
                    lastChar = reader.ReadChar();
                    //Console.WriteLine(lastChar);
                    extra = reader.ReadInt32();
                    //Console.WriteLine(extra);



                }
                return (q, lastChar, extra);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return(q, lastChar, extra);
        }

    }
}
