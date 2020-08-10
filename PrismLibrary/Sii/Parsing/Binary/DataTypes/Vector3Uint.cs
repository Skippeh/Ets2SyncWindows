namespace PrismLibrary.Sii.Parsing.Binary.DataTypes
{
    public struct Vector3Uint
    {
        public uint X;
        public uint Y;
        public uint Z;

        public Vector3Uint(uint x, uint y, uint z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3Uint operator +(Vector3Uint a, Vector3Uint b) => new Vector3Uint(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3Uint operator -(Vector3Uint a, Vector3Uint b) => new Vector3Uint(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3Uint operator *(Vector3Uint a, Vector3Uint b) => new Vector3Uint(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        public static Vector3Uint operator *(Vector3Uint a, uint b) => new Vector3Uint(a.X * b, a.Y * b, a.Z * b);
        public static Vector3Uint operator /(Vector3Uint a, Vector3Uint b) => new Vector3Uint(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        public static Vector3Uint operator /(Vector3Uint a, uint b) => new Vector3Uint(a.X / b, a.Y / b, a.Z / b);

        public override string ToString()
        {
            return $"{{{X}, {Y}, {Z}}}";
        }
    }
}