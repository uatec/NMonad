using System.Drawing;

namespace NMonad
{
    public class RectangleBuilder
    {

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public static implicit operator Rectangle(RectangleBuilder builder)
        {
            return new Rectangle(builder.X, builder.Y, builder.Width, builder.Height);
        }
    }
}