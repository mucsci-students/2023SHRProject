using UnityEngine;

public static class Utils
{
    public static void Print2DArray(int[,] array)
    {
        for (var i = 0; i < array.GetLength(0); i++)
        {
            var message = "Row " + i + ": ";
            for (var j = 0; j < array.GetLength(1); j++) {
                message += array[i, j] + " ";
            }
            Debug.Log(message);
        }
    }
}
