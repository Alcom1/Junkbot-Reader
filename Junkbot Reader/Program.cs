using Junkbot_Reader.Models;
using Junkbot_Reader.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Junkbot_Reader
{
    public static class Program
    {
        private const string HELP = "--help";

        public static void Main(string[] args)
        {
            if (ValidateArgs(args))
            {
                Process(args[0], args[1]);
            }

            Console.ReadKey();
            return;
        }

        private static bool ValidateArgs(string[] args)
        {
            switch (args.Length)
            {
                case 0:
                    Console.WriteLine(Resources.Error0);
                    return false;

                case 1:
                    if (args[0] == HELP)
                    {
                        Console.WriteLine(Resources.Help);
                    }
                    else
                    {
                        Console.WriteLine(Resources.Error1);
                    }
                    return false;

                default:
                    return true;
            }
        }

        private static void Process(string sourcePath, string targetFile, int max = 61)
        {
            var scenes = new List<SceneSingle>();
            var counter = 0;

            foreach (var file in Directory.GetFiles(sourcePath))
            {
                Console.Write($"Reading {Path.GetFileName(file)} - ");

                var content = File.ReadAllText(file);
                var indexHeader = content.IndexOf("[info]");

                if (indexHeader > 0)
                {
                    var contents = content.Replace("\n", "").Split('\r');

                    //var par         = contents.Extract("par");
                    //var hint        = contents.Extract("hint");
                    //var backdrop    = contents.Extract("backdrop");
                    //var decals      = contents.Extract("decals");
                    //var size        = contents.Extract("size");
                    //var spacing     = contents.Extract("spacing");
                    //var scale       = contents.Extract("scale");

                    var title = contents.Extract("title");
                    Console.WriteLine(title);
                    var types = String.Join(",", contents.ExtractAll("types")).Split(',');
                    var colors = contents.Extract("colors").Split(',');
                    var retroObjects =
                        String.Join(",", contents.ExtractAll("parts"))
                        .Split(',')
                        .Select(s => s.Split(';'))
                        .Select(s =>
                        {
                            return new RetroObject()
                            {
                                X = Byte.Parse(s[0]),
                                Y = Byte.Parse(s[1]),
                                Type = types[int.Parse(s[2]) - 1],
                                Color = colors[int.Parse(s[3]) - 1],
                                State = s[4],
                                What = Byte.Parse(s[5]),
                                Key = s.Length > 6 ? s[6] : ""
                            };
                        }).ToList();

                    //Convert all parts/retroObjects to game objects
                    var gameObjects = retroObjects.Select(x => ConvertRetroToGame(x)).ToList();

                    //Remove null game objects!
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

                    //That one weird brick.
                    if (counter == 1)
                    {
                        //Remove weird brick
                        gameObjects.RemoveAll(o =>
                            o.Position.Value.X == 22 &&
                            o.Position.Value.Y == 6);

                        //Extend brick next to it to fill its spot
                        gameObjects.Where(o =>
                            o.Position.Value.X == 19 &&
                            o.Position.Value.Y == 6).ToList().ForEach(o => ((Brick)o).Width = 4);
                    }

                    //Add additional game objects.
                    gameObjects.Add(new Sprite("background_level"));
                    gameObjects.Add(new LevelSequence(
                        title,
                        byte.Parse(contents.Extract("par")),
                        $"LEVEL_{Mod(counter + 1, max):00}",
                        $"LEVEL_{Mod(counter - 1, max):00}"));

                    scenes.Add(new SceneSingle(gameObjects, $"LEVEL_{counter:00}"));
                    counter++;
                }
                else
                {
                    Console.WriteLine("Invalid fff file! Skipping...");
                }
            }

            File.WriteAllText(targetFile, SerializeScenes(scenes));

            Console.Write($"\nSucessfully wrote levels to {Path.GetFileName(targetFile)}");
        }

        private static int Mod(int val, int length)
        {
            return (val + length) % length;
        }

        private static string SerializeScenes(List<SceneSingle> scenes)
        {
            var sceneJson = JArray.Parse(Resources.AdditionalScenes);

            sceneJson.Merge(
                JArray.FromObject(
                    scenes,
                    JsonSerializer.Create(new JsonSerializerSettings
                    {
                        ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = new CamelCaseNamingStrategy()
                        },
                        NullValueHandling = NullValueHandling.Ignore
                    })));

            return sceneJson.ToString(Formatting.Indented);
        }

        private static GameObject ConvertRetroToGame(RetroObject retro)
        {
            if (new string[]
                {
                    //"HAZ_SLICKFIRE",
                    //"haz_walker",
                    //"HAZ_SLICKPIPE",
                    //"HAZ_SLICKFAN",
                    //"haz_slickJump",
                    "HAZ_CLIMBER",
                    //"BRICK_SLICKJUMP",
                    "HAZ_DUMBFLOAT",
                    //"HAZ_SLICKSWITCH",
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
            if (String.Equals(retro.Type, "haz_walker"))
            {
                return new CharacterRBG(
                    (byte)(retro.X + 1),
                    retro.Y,
                    String.Equals(retro.State, "Walk_R", StringComparison.InvariantCultureIgnoreCase));
            }
            if (String.Equals(retro.Type, "HAZ_SLICKSWITCH"))
            {
                return new BrickPlateButton(retro.X, retro.Y, retro.Key, String.Equals(retro.State, "on"));
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
                return new BrickPlateHot(retro.X, retro.Y, retro.Key, String.Equals(retro.State, "on"));
            }
            if (String.Equals(retro.Type, "HAZ_SLICKFAN"))
            {
                return new BrickPlateFan(retro.X, retro.Y, retro.Key, new String[] { "on", "none" }.Contains(retro.State));
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
                return new CharacterBot(
                    (byte)(retro.X + 1), 
                    retro.Y,
                    String.Equals(retro.State, "Walk_R", StringComparison.InvariantCultureIgnoreCase));
            }
            if (String.Equals(retro.Type, "flag"))
            {
                return new CharacterBin((byte)(retro.X + 1), retro.Y);
            }

            return null;
        }

        private static string Extract(this string[] contents, string pattern)
        {
            return contents.FirstOrDefault(x => x.StartsWith(pattern)).Split('=')[1];
        }

        private static string[] ExtractAll(this string[] contents, string pattern)
        {
            return contents.Where(x => x.StartsWith(pattern)).Select(x => x.Split('=')[1]).ToArray();
        }
    }
}
