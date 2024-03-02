namespace HardCodedAI {
    public static class MapUtils {
        
        public static Tile GetFirstAvailableTile(ref int[,] map, ref Tile[,] tiles) {
            if (map == null || tiles == null)
                return null;
            
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == (int)Enums.TileMode.Open)
                    {
                        // Mark the tile as in use
                        map[i, j] = (int)Enums.TileMode.InUse;
                        return tiles[i, j];
                    }
                }
            }

            return null;
        }
        
    }
}
