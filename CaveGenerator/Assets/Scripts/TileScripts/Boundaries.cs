namespace TileScripts
{
    public static class Boundaries
    {
        public static bool InBoundaries(int x, int y, int height, int width)
        {
            return x >= 0 && 
                   y >= 0 && 
                   x < width && 
                   y < height;
        }
        
        public static bool IsWall(int x, int y, int height, int width, int borderSize) 
        {
            return x <= borderSize || 
                   y <= borderSize || 
                   x >= width - borderSize || 
                   y >= height - borderSize;
        }
    }
}