using System;

namespace Grid
{
    [Serializable]
    public class Vector2IntMinMax
    {
        public int MinX;
        public int MaxX;

        public Vector2IntMinMax(int minX, int maxX)
        {
            MinX = minX;
            MaxX = maxX;
        }
    }
}