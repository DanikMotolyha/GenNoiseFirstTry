using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IslMapGenerator
{
    public static float[,] GenerateIslMap(int mapHeight , int mapWidth)
    {
        float[,] map = new float[mapWidth, mapHeight];

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                float x = i / (float) mapWidth * 2 - 1;
                float y = j / (float) mapHeight * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = FuncIsland(value);
            }
        }

        return map;

    }
    static float FuncIsland(float value)
    {
        float a = 2;
        float b = 4;
        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}
