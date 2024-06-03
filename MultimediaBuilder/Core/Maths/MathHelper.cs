namespace MultimediaBuilder.Maths
{
    public static class MathHelper
    {
        private static float RadsMultiplyBy = 180 / MathF.PI;
        private static float DegsMultiplyBy = MathF.PI / 180;

        public static float RadToDeg(float radians)
        {
            return radians * RadsMultiplyBy;
        }

        public static float DegToRad(float degrees)
        {
            return degrees * DegsMultiplyBy;
        }
    }
}