namespace Tycho.Utils.SourceGenerator.Utils
{
    internal static class HashCode
    {
        public static int Combine(int h1, int h2)
        {
            unchecked
            {
                uint rol5 = ((uint)h1 << 5) | ((uint)h1 >> 27);
                return ((int)rol5 + h1) ^ h2;
            }
        }

        public static int Combine(int h1, int h2, int h3)
        {
            return Combine(Combine(h1, h2), h3);
        }

        public static int Combine(int h1, int h2, int h3, int h4)
        {
            return Combine(Combine(h1, h2), Combine(h3, h4));
        }

        public static int Combine(int h1, int h2, int h3, int h4, int h5)
        {
            return Combine(Combine(h1, h2, h3, h4), h5);
        }

        public static int Combine(int h1, int h2, int h3, int h4, int h5, int h6)
        {
            return Combine(Combine(h1, h2, h3), Combine(h4, h5, h6));
        }

        public static int Combine(int h1, int h2, int h3, int h4, int h5, int h6, int h7)
        {
            return Combine(Combine(h1, h2, h3, h4), Combine(h5, h6, h7));
        }

        public static int Combine(int h1, int h2, int h3, int h4, int h5, int h6, int h7, int h8)
        {
            return Combine(Combine(h1, h2, h3, h4), Combine(h5, h6, h7, h8));
        }
    }
}
