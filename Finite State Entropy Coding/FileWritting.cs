using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Finite_State_Entropy_Coding
{
    public class FileWritting
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

        public static void Compress(string bits, int q, string outpath_name)
        {
            var len = bits.Length;
            var extra = len % 8 == 0 ? 0 : 8 - len % 8;

            try
            {
                using (BinaryWriter writer = new BinaryWriter(new FileStream(outpath_name + ".dat", FileMode.Create), Encoding.ASCII))
                using (BinaryWriter writer2 = new BinaryWriter(new FileStream(outpath_name + "_info.dat", FileMode.Create), Encoding.ASCII))
                {
                    for (var i = 0; i < len / 8; i++)
                    {
                        byte arr = Convert.ToByte(bits.Substring(8 * i, 8), 2);

                        writer.Write(arr);
                    }
                    if(extra > 0)
                    {
                        var s = bits.Substring(8 * (len / 8));

                        for (var i = 0; i < extra; i++)
                        {
                            s = s + "0";
                        }
                        writer.Write(Convert.ToByte(s, 2));
                    }

                  
                    writer2.Write(q);
                    writer2.Write(extra);
                    


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static string Decompress(string outpath_name)
        {
            var s = "";
            try
            {
                using (BinaryReader reader = new BinaryReader(new FileStream(outpath_name + ".dat", FileMode.Open), Encoding.ASCII))
                {
                    while (reader.PeekChar() != -1)
                    {
                        var b = reader.ReadByte();
                        var ss = Convert.ToString(b, 2);
                        var len = ss.Length;
                        for(var i = 0; i < (8 - len); i++)
                        {
                            ss = "0" + ss;
                        }
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

        public static (int, int) GetComprInfo(string outpath_name)
        {
            (int q, char lastChar, int extra) = (-1, '(', -1);
            try
            {
                var fs = new FileStream(outpath_name + "_info.dat", FileMode.Open);
                using (BinaryReader reader = new BinaryReader(fs, Encoding.ASCII))
                {
                    fs.Seek(0, SeekOrigin.Begin);
                    q = reader.ReadInt32();
                    extra = reader.ReadInt32();
                }
                return (q, extra);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (q, extra);
        }

    }
}
