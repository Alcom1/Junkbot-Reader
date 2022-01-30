
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

    public class CharacterBot : CharacterDir
    {
        public CharacterBot(byte x, byte y, bool isForward)
        {
            Name = "CharacterBot";
            Position = new Boint(x, y);
            IsForward = isForward;
        }
    }

    public class CharacterRBG : CharacterDir
    {
        public CharacterRBG(byte x, byte y, bool isForward)
        {
            Name = "CharacterRBG";
            Position = new Boint(x, y);
            IsForward = isForward;
        }
    }

    public class CharacterDir : GameObject
    {
        public bool IsForward { get; set; }
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

        public int? Circuit { get; set; }
    }

    public class BrickPlateButton : BrickPlate
    {
        public BrickPlateButton(byte x, byte y, string key, bool isOn)
        {
            Name = "BrickPlateButton";
            Position = new Boint(x, y);
            IsOn = isOn;
            int.TryParse(key.Replace("switch", ""), out int temp);
            Circuit = temp;
        }
    }

    public class BrickPlateHot : BrickPlate
    {
        public BrickPlateHot(byte x, byte y, string key, bool isOn)
        {
            Name = "BrickPlateHot";
            Position = new Boint(x, y);
            IsOn = isOn;
            int.TryParse(key.Replace("switch", ""), out int temp);
            Circuit = temp;
        }
    }

    public class BrickPlateFan : BrickPlate
    {
        public BrickPlateFan(byte x, byte y, string key, bool isOn)
        {
            Name = "BrickPlateFan";
            Position = new Boint(x, y);
            IsOn = isOn;
            int.TryParse(key.Replace("switch", ""), out int temp);
            Circuit = temp;
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