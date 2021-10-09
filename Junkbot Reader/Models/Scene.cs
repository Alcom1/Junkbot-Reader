using System.Collections.Generic;

namespace Junkbot_Reader.Models
{
    public struct Scene
    {
        public Scene(string name, string tag, string need, int zIndex)
        {
            Name = name;
            Tag = tag;
            Need = new string[] { need };
            ZIndex = zIndex;
        }

        public string Name { get; set; }

        public string Tag { get; set; }

        public string[] Need { get; set; }

        public int ZIndex { get; set; }

    }

    public class SceneSingle
    {
        public SceneSingle(List<GameObject> gameObjects, string tag)
        {
            this.Scene = new Scene("Level", tag, "LevelInterface", 0);
            this.GameObjects = gameObjects.ToArray();
        }

        public Scene Scene { get; set; }

        public GameObject[] GameObjects { get; set; }
    }
}
