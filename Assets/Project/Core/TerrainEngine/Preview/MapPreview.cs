using UnityEngine;
using Archura.TerrainEngine.Noise;

namespace Archura.TerrainEngine.Preview
{
    public class MapPreview : MonoBehaviour
    {
        [Header("Map Dimensions")]
        public int mapWidth = 256;
        public int mapHeight = 256;

        [Header("Settings")]
        public NoiseSettings noiseSettings;
        public bool autoUpdate = true;

        [Header("Output")]
        public Renderer textureRenderer;

        public void DrawMapInEditor()
        {
            if (!textureRenderer)
            {
                return;
            }

            float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(mapWidth, mapHeight, noiseSettings);
            
            Texture2D texture = new Texture2D(mapWidth, mapHeight);
            texture.filterMode = FilterMode.Point;
            
            Color[] colorMap = new Color[mapWidth * mapHeight];
            
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    colorMap[y * mapWidth + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
                }
            }

            texture.SetPixels(colorMap);
            texture.Apply();

            textureRenderer.sharedMaterial.mainTexture = texture;
        }
    }
}