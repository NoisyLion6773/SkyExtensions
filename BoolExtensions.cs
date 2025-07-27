namespace SkyExtensions;

public static class BoolExtensions
{
    public static void Toggle(this ref bool var)
    {
        var = !var;
    }
}
