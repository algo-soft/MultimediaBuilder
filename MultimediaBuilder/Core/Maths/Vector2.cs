namespace MultimediaBuilder.Maths
{
    public struct Vector2
    {
        public float X;
        public float Y;

        public Vector2(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public static Vector2 Zero
        {
            get { return new Vector2(0, 0); }
        }
    }
}