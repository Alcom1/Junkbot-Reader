using Junkbot_Reader.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Junkbot_Reader
{
    public static class Program
    {
        public const string basepath = @"C:\Users\Alcom\Desktop\Dev\Not Git\Junkbot\fff";

        public static void Main(string[] args)
        {
            var scenes = new List<SceneSingle>();
            var max = 61;
            var counter = 0;

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

                    var types   = String.Join(",", contents.ExtractAll("types")).Split(',');
                    var colors  = contents.Extract("colors").Split(',');
                    var parts   = 
                        String.Join(",", contents.ExtractAll("parts"))
                        .Split(',')
                        .Select(s => s.Split(';'))
                        .Select(s => 
                        {
                            return new RetroObject()
                            {
                                X = Byte.Parse(s[0]),
                                Y = Byte.Parse(s[1]),
                                Type =  types[int.Parse(s[2]) - 1],
                                Color = colors[int.Parse(s[3]) - 1],
                                State = s[4],
                                What = Byte.Parse(s[5]),
                                Key = ""
                            };
                        }).ToList();

                    var gameObjects = parts.Select(x => ConvertRetroToGame(x)).ToList();

                    //Remove Nulls!
                    gameObjects.RemoveAll(o => o == null);

                    //Move to match coordinates
                    gameObjects.ForEach(o => 
                    { 
                        if (o != null && 
                            o.Position.HasValue) 
                        {
                            o.Position = new Boint((byte)(o.Position.Value.X - 1), (byte)(o.Position.Value.Y + 1));
                        } 
                    });

                    //LOL!
                    if (counter == 1)
                    {
                        gameObjects.RemoveAll(o =>
                            o.Position.Value.X == 22 &&
                            o.Position.Value.Y == 6);

                        gameObjects.Where(o =>
                            o.Position.Value.X == 19 &&
                            o.Position.Value.Y == 6).ToList().ForEach(o => ((Brick)o).Width = 4);
                    }

                    //
                    gameObjects.Add(new Sprite("background_level"));
                    gameObjects.Add(new LevelSequence(
                        contents.Extract("title"),
                        byte.Parse(contents.Extract("par")),
                        $"LEVEL_{mod(counter + 1, max):00}",
                        $"LEVEL_{mod(counter - 1, max):00}"));

                    scenes.Add(new SceneSingle(gameObjects, $"LEVEL_{counter:00}"));
                    counter++;
                }
            }

            var qq = SerializeObject(scenes).Remove(0, 1);

            Console.ReadKey();
        }

        public static int mod(int val, int length)
        {
            return (val + length) % length;
        }

        public static string SerializeObject(Object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });
        }

        public static GameObject ConvertRetroToGame(RetroObject retro)
        {
            if (new string[]
                {
                    //"HAZ_SLICKFIRE",
                    "haz_walker",
                    //"HAZ_SLICKPIPE",
                    //"HAZ_SLICKFAN",
                    //"haz_slickJump",
                    "HAZ_CLIMBER",
                    //"BRICK_SLICKJUMP",
                    "HAZ_DUMBFLOAT",
                    "HAZ_SLICKSWITCH",
                    "HAZ_FLOAT",
                    //"HAZ_SLICKSHIELD",

                    //UNDERCOVER
                    "HAZ_SLICKCRATE",
                    "HAZ_SLICKLASER_L",
                    "haz_slickLaser_R",
                    "HAZ_SLICKTELEPORT",
                    "SCAREDY",
                    "BRICK_SLICKSHIELD"
                }.Contains(retro.Type))
            {
                return null;
            }
            if (String.Equals(retro.Type, "BRICK_SLICKJUMP"))
            {
                return new GameObject("BrickJumpMove", retro.X, retro.Y);
            }
            if (String.Equals(retro.Type, "haz_slickJump"))
            {
                return new GameObject("BrickJump", retro.X, retro.Y);
            }
            if (String.Equals(retro.Type, "HAZ_SLICKPIPE"))
            {
                return new GameObject("BrickPipe", retro.X, retro.Y);
            }
            if (String.Equals(retro.Type, "HAZ_SLICKSHIELD"))
            {
                return new GameObject("BrickSuper", retro.X, retro.Y);
            }
            if (String.Equals(retro.Type, "HAZ_SLICKFIRE"))
            {
                return new BrickPlateHot(retro.X, retro.Y, String.Equals(retro.State, "on"));
            }
            if (String.Equals(retro.Type, "HAZ_SLICKFAN"))
            {
                return new BrickPlateFan(retro.X, retro.Y, new String[] { "on", "none" }.Contains(retro.State));
            }
            if (retro.Type.StartsWith("BRICK"))
            {
                return new Brick(
                    retro.X, 
                    retro.Y, 
                    Byte.Parse(retro.Type.Split('_')[1]), 
                    String.Equals(retro.Color, "GRAY") ? null : retro.Color.ToLower());
            }
            if (String.Equals(retro.Type, "MINIFIG"))
            {
                return new CharacterBot((byte)(retro.X + 1), retro.Y);
            }
            if (String.Equals(retro.Type, "flag"))
            {
                return new CharacterBin((byte)(retro.X + 1), retro.Y);
            }

            return null;
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
