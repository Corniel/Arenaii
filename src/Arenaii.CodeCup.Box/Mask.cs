namespace Arenaii.CodeCup.Box;

public static class Mask
{
    public static class Horizontal
    {
        /// <summary>binary 11.</summary>
        public const uint Shared = 0x3;

        /// <summary>binary 1111.</summary>
        public const uint Extended = 0xf;
    }
    public static class Vertical
    {
        /// <summary>binary 111.111.</summary>
        public const uint Shared = 0x3f;
        /// <summary>binary 1111.1111.</summary>
        public const uint Extended = 0xff;
    }
}
