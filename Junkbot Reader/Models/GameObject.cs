
namespace Junkbot_Reader.Models
{
    public struct Boint
    {
        public Boint(byte x, byte y)
        {
            X = x;
            Y = y;
        }

        public byte X { get; set; }

        public byte Y { get; set; }
    }

    public class GameObject
    {
        protected GameObject()
        {

        }

        public GameObject(string name, byte x, byte y)
        {
            this.Name = name;
            this.Position = new Boint(x, y);
        }

        public string Name { get; set; }

        public Boint? Position { get; set; }

        public int? ZIndex { get; set; }
    }

    public class Sprite : GameObject
    {
        public Sprite(string image, int zIndex = -50000)
        {
            Name = "Sprite";
            Image = image;
            ZIndex = zIndex;
        }

        public string Image { get; set; }
    }

    public class CharacterBot : GameObject
    {
        public CharacterBot(byte x, byte y)
        {
            Name = "CharacterBot";
            Position = new Boint(x, y);
        }
    }

    public class CharacterBin : GameObject
    {
        public CharacterBin(byte x, byte y)
        {
            Name = "CharacterBin";
            Position = new Boint(x, y);
        }
    }

    public class Brick : GameObject
    {
        public Brick(byte x, byte y, byte width, string color)
        {
            Name = "BrickNormal";
            Position = new Boint(x, y);
            Width = width;
            Color = color;
        }

        public byte Width { get; set; }

        public string Color { get; set; }
    }

    public class BrickPlate : GameObject
    {
        public bool IsOn { get; set; }
    }

    public class BrickPlateHot : BrickPlate
    {
        public BrickPlateHot(byte x, byte y, bool isOn)
        {
            Name = "BrickPlateHot";
            Position = new Boint(x, y);
            IsOn = isOn;
        }
    }

    public class BrickPlateFan : BrickPlate
    {
        public BrickPlateFan(byte x, byte y, bool isOn)
        {
            Name = "BrickPlateFan";
            Position = new Boint(x, y);
            IsOn = isOn;
        }
    }

    public struct Levels
    {
        public Levels(string next, string prev)
        {
            NextLevel = next;
            PreviousLevel = prev;
        }

        public string NextLevel { get; set; }
        public string PreviousLevel { get; set; }
    }

    public class LevelSequence : GameObject
    {

        public LevelSequence(string levelName, byte par, string next, string prev)
        {
            Name = "LevelSequence";
            Position = new Boint(43, 0);
            LevelName = levelName;
            Levels = new Levels(next, prev);
            Par = par;
        }

        public string LevelName { get; set; }
        public Levels Levels { get; set; }
        public byte Par { get; set; }
    }
}