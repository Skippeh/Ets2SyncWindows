namespace PrismLibrary.Sii.Parsing.Binary.DataTypes
{
    public struct Vector3Int
    {
        public int X;
        public int Y;
        public int Z;

        public Vector3Int(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3Int operator +(Vector3Int a, Vector3Int b) => new Vector3Int(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3Int operator -(Vector3Int a, Vector3Int b) => new Vector3Int(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3Int operator *(Vector3Int a, Vector3Int b) => new Vector3Int(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        public static Vector3Int operator *(Vector3Int a, int b) => new Vector3Int(a.X * b, a.Y * b, a.Z * b);
        public static Vector3Int operator /(Vector3Int a, Vector3Int b) => new Vector3Int(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        public static Vector3Int operator /(Vector3Int a, int b) => new Vector3Int(a.X / b, a.Y / b, a.Z / b);

        public override string ToString()
        {
            return $"{{{X}, {Y}, {Z}}}";
        }
    }
}