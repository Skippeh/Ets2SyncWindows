using System.Numerics;

namespace PrismLibrary.Sii.Parsing.Binary.DataTypes
{
    public struct Vector2
    {
        public float X;
        public float Y;
        
        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2(float value)
            : this(value, value)
        {
        }

        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.X + b.X, a.Y + b.Y);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.X - b.X, a.Y - b.Y);
        public static Vector2 operator *(Vector2 a, Vector2 b) => new Vector2(a.X * b.X, a.Y * b.Y);
        public static Vector2 operator *(Vector2 a, float b) => new Vector2(a.X * b, a.Y * b);
        public static Vector2 operator /(Vector2 a, Vector2 b) => new Vector2(a.X / b.X, a.Y / b.Y);
        public static Vector2 operator /(Vector2 a, float b) => new Vector2(a.X / b, a.Y / b);

        public override string ToString()
        {
            return $"{{{X}, {Y}}}";
        }
    }
}