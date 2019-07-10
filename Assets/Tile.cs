class Tile
{
    public static readonly int Green = 0;
    public static readonly int Blue = 1;
    public static readonly int Purple = 2;
    public static readonly int Orange = 3;
    public static readonly int Yellow = 4;
    public static readonly int Red = 5;

    public static readonly int star4 = 0;
    public static readonly int star8 = 1;
    public static readonly int circle = 2;
    public static readonly int cross = 3;
    public static readonly int diamond = 4;
    public static readonly int square = 5;

    public static readonly int empty = -1;

    public int color;
    public int shape;

    public Tile(int empty)
    {
        color = empty;
        shape = empty;
    }

    public Tile(int color, int shape)
    {
        this.color = color;
        this.shape = shape;
    }

    public bool IsEmpty()
    {
        return color == empty || shape == empty;
    }

    public string GetColorName()
    {
        switch(color)
        {
            case -1: return "Empty";
            case 0: return "Green";
            case 1: return "Blue";
            case 2: return "Purple";
            case 3: return "Orange";
            case 4: return "Yellow";
            case 5: return "Red";
        }

        return "";
    }

    public string GetShapeName()
    {
        switch(shape)
        {
            case -1: return "Empty";
            case 0: return "4-pointed Star";
            case 1: return "8-pointed Star";
            case 2: return "Circle";
            case 3: return "Cross";
            case 4: return "Diamond";
            case 5: return "Square";
        }

        return "";
    }
}