using Junkbot_Reader.Models;
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
            var scenes = new List<SceneSingle>();

            foreach (var file in Directory.GetFiles(basepath))
            {
                var content = File.ReadAllText(file);
                var indexHeader = content.IndexOf("[info]");

                if (indexHeader > 0)
                {
                    var contents = content.Replace("\n", "").Split('\r');

                    var title       = contents.Extract("title");

                    //var par         = contents.Extract("par");
                    //var hint        = contents.Extract("hint");
                    //var backdrop    = contents.Extract("backdrop");
                    //var decals      = contents.Extract("decals");
                    //var size        = contents.Extract("size");
                    //var spacing     = contents.Extract("spacing");
                    //var scale       = contents.Extract("scale");

                    var types   = contents.Extract("types").Split(',');
                    var colors  = contents.Extract("colors").Split(',');
                    var parts   = 
                        String.Join(",", contents.ExtractAll("parts"))
                        .Split(',')
                        .Select(s => s.Split(';'))
                        .Select(s => 
                        {
                            var typeIndex  = int.Parse(s[2]) - 1;
                            var colorIndex = int.Parse(s[3]) - 1;
                            return new RetroObject()
                            {
                                X = Byte.Parse(s[0]),
                                Y = Byte.Parse(s[1]),
                                Type =  typeIndex < types.Length ? types[typeIndex] : s[2],
                                Color = colors[colorIndex],
                                State = s[4],
                                What = Byte.Parse(s[5]),
                                Key = ""
                            };
                        }).ToList();

                    Console.WriteLine(title);

                    //var gameObjects = new List<GameObject>();

                    //gameObjects.Add(new Sprite("background_level"));

                    //var scene = new SceneSingle(gameObjects);
                }
            }

            Console.ReadKey();
        }

        public static string Extract(this string[] contents, string pattern)
        {
            return contents.FirstOrDefault(x => x.StartsWith(pattern)).Split('=')[1];
        }

        public static string[] ExtractAll(this string[] contents, string pattern)
        {
            return contents.Where(x => x.StartsWith(pattern)).Select(x => x.Split('=')[1]).ToArray();
        }
    }
}
