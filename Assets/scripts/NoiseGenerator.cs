using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColorMap, Mesh, IsleMap, ClosedMap};
    public DrawMode drawMode;
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;
    
    public bool autoUpdate;
    public enum BorderStyle { ClosedMap, IsleMap, Classic };
    public BorderStyle borderStyle;
    public int seed;
    public TerrainType[] regions;
    
    public float meshGeightMultiplier;
    public AnimationCurve meshHeightCurve;

    private float[,] borderMap;

    [Range(1,100)]
    public int sizeSeeds;
    public void BorderMap()
    {
        borderMap = IslMapGenerator.GenerateIslMap(mapHeight, mapWidth); 
    }
    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity);
        BorderMap();
        Color[] colorMap = new Color[mapWidth*mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if(borderStyle == BorderStyle.IsleMap)
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - borderMap[x, y]);
                else if(borderStyle == BorderStyle.ClosedMap)
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] + borderMap[x, y]);

                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                    if(currentHeight <= regions[i].height)
                    {
                        colorMap[y * mapWidth + x] = regions[i].color;
                        break;
                    }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MashGenerator.GenerateTerrainMesh(noiseMap, meshGeightMultiplier,meshHeightCurve), TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));   
        }
        else if (drawMode == DrawMode.IsleMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(IslMapGenerator.GenerateIslMap(mapWidth, mapHeight)));
        }
        else if (drawMode == DrawMode.ClosedMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(IslMapGenerator.GenerateIslMap(mapWidth, mapHeight)));
        }

        #region Check (Проверка на входящие данные)

        if (mapWidth < 1) mapWidth = 1;
        if (mapHeight < 1) mapHeight = 1;
        if (lacunarity < 1) lacunarity = 1;
        if (octaves < 0) octaves = 0;
        if (noiseScale < 1) noiseScale = 1;
        if (seed < 0) seed = 0;

        #endregion Validate
    }
    public IEnumerator GenerateMapToN()
    {
        borderStyle = BorderStyle.Classic;
        for (int i = 0; i < sizeSeeds; i++)
        {
            seed = i;
            GenerateMap();
            yield return new WaitForSeconds(1f);
        }
        seed = 0;

        borderStyle = BorderStyle.IsleMap;
        for (int i = 0; i < sizeSeeds; i++)
        {
            seed = i;
            GenerateMap();
            yield return new WaitForSeconds(1f);
        }

        seed = 0;
        borderStyle = BorderStyle.ClosedMap;
        for (int i = 0; i < sizeSeeds; i++)
        {
            seed = i;
            GenerateMap();
            yield return new WaitForSeconds(1f);
        }
    }
    public void Update()
    {
        StartCoroutine(GenerateMapToN());
    }
}

[System.Serializable]
public struct TerrainType 
{
    public string name;
    [Range(0, 1)]
    public float height;
    public Color color;
}
