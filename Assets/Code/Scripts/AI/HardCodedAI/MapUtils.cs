using UnityRandom = UnityEngine.Random;

namespace HardCodedAI {
    public static class MapUtils {
        
        public static Tile GetFirstAvailableTile(ref int[,] map, ref Tile[,] tiles) {
            if (map == null || tiles == null)
                return null;
            
            for (int i = 0; i < map.GetLength(0); i++) {
                for (int j = 0; j < map.GetLength(1); j++) {
                    if (map[i, j] == (int)Enums.TileMode.Open) {
                        // Mark the tile as in use
                        map[i, j] = (int)Enums.TileMode.InUse;
                        return tiles[i, j];
                    }
                }
            }

            return null;
        }
        
        public static Tile GetRandomAvailableTile(ref int[,] map, ref Tile[,] tiles) {
            if (map == null || tiles == null)
                return null;

            int randRowNum = UnityRandom.Range(0, map.GetLength(0));
            for (int i = randRowNum; i < map.GetLength(0) + randRowNum; i++) {
                int row = i % map.GetLength(0);

                int randColNum = UnityRandom.Range(0, map.GetLength(1));
                for (int j = randColNum; j < map.GetLength(1) + randColNum; j++) {
                    int col = j % map.GetLength(1);
                    
                    if (map[row, col] == (int)Enums.TileMode.Open) {
                        // Mark the tile as in use
                        map[row, col] = (int)Enums.TileMode.InUse;
                        return tiles[row, col];
                    }
                }
            }

            return null;
        }
        
    }
}
