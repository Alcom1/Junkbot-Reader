using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Junkbot_Reader
{
    public static class Program
    {
        public const string basepath = "C:\\Users\\Alcom\\Desktop\\fff2";

        public static void Main(string[] args)
        {
            var count = 0;
            foreach (var file in Directory.GetFiles(basepath))
            {
                var content = File.ReadAllText(file);
                var indexHeader = content.IndexOf("[info]");

                if (indexHeader > 0)
                {
                    var contents = content.Replace("\n", "").Split('\r');

                    //var title       = contents.Extract("title");
                    //var par         = contents.Extract("par");
                    //var hint        = contents.Extract("hint");
                    //var backdrop    = contents.Extract("backdrop");
                    //var decals      = contents.Extract("decals");
                    //var size        = contents.Extract("size");
                    //var spacing     = contents.Extract("spacing");
                    //var scale       = contents.Extract("scale");
                    //var types       = contents.Extract("types");
                    //var colors      = contents.Extract("colors");
                    //var parts =     "parts=" + String.Join(",", contents.ExtractAll("parts")).Replace("parts=", "");

                    if (!Directory.Exists(basepath + "\\output\\"))
                    {
                        Directory.CreateDirectory(basepath + "\\output\\");
                    }

                    File.WriteAllText(
                        String.Join(
                            "",
                            new string[]
                            {
                                basepath,
                                "\\output\\",
                                count.ToString("00"),
                                "_",
                                String.Join("", contents.Extract("title").Split('=')[1].Split(Path.GetInvalidFileNameChars())),
                                ".txt"
                            }),
                        String.Join(
                            "\n",
                            new string[] 
                            {
                                "[info]",
                                contents.Extract("title"),
                                contents.Extract("par"),
                                contents.Extract("hint"),
                                "\n",

                                "[background]",
                                contents.Extract("backdrop"),
                                contents.Extract("decals"),
                                "\n",

                                "[playfield]",
                                contents.Extract("size"),
                                contents.Extract("spacing"),
                                contents.Extract("scale"),
                                "\n",

                                "[partslist]",
                                contents.Extract("types"),
                                contents.Extract("colors"),
                                "parts=\n\t" + String.Join(",", contents.ExtractAll("parts")).Replace("parts=", "").Replace(",", ",\n\t") }));

                    count++;
                }
            }

            Console.ReadKey();
        }

        public static string Extract(this string[] contents, string pattern)
        {
            return contents.FirstOrDefault(x => x.StartsWith(pattern));
        }

        public static string[] ExtractAll(this string[] contents, string pattern)
        {
            return contents.Where(x => x.StartsWith(pattern)).ToArray();
        }
    }
}
