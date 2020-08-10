namespace PrismLibrary.Sii.Parsing.Binary.DataTypes
{
    public struct Vector4
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static Vector4 operator +(Vector4 a, Vector4 b) => new Vector4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        public static Vector4 operator -(Vector4 a, Vector4 b) => new Vector4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        public static Vector4 operator *(Vector4 a, Vector4 b) => new Vector4(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W - b.W);
        public static Vector4 operator *(Vector4 a, float b) => new Vector4(a.X * b, a.Y * b, a.Z * b, a.W * b);
        public static Vector4 operator /(Vector4 a, Vector4 b) => new Vector4(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);
        public static Vector4 operator /(Vector4 a, float b) => new Vector4(a.X / b, a.Y / b, a.Z / b, a.W / b);

        public override string ToString()
        {
            return $"{{{X}, {Y}, {Z}, {W}}}";
        }
    }
}