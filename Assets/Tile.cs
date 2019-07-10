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
    public static readonly int square = 5;
    public static readonly int diamond = 4;

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
}