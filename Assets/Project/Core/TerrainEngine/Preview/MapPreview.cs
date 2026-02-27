using Archura.TerrainEngine.Climate;
using Archura.TerrainEngine.BiomeModule;
using UnityEngine;

namespace Archura.TerrainEngine.Preview
{
    public class MapPreview : MonoBehaviour
    {
        [Header("Map Dimensions")]
        public int mapWidth  = 256;
        public int mapHeight = 256;
        public bool autoUpdate = true;

        [Header("Output")]
        public Renderer textureRenderer;

        [Header("Climate")]
        public ClimateNoiseSettings climateNoiseSettings;

        [Header("Biome Resolver")]
        public BiomeResolverBase biomeResolver;

        [Header("Visualization")]
        public DrawMode drawMode;

        public enum DrawMode
        {
            Biome,
            Temperature,
            Moisture,
            Continentalness,
            Erosion,
            Weirdness
        }

        public void DrawMapInEditor()
        {
            if (climateNoiseSettings == null)
            {
                Debug.LogWarning("[MapPreview] ClimateNoiseSettings is not assigned.");
                return;
            }

            if (drawMode == DrawMode.Biome)
            {
                if (!biomeResolver)
                {
                    Debug.LogWarning("[MapPreview] No biome resolver assigned.");
                    return;
                }
                if (!biomeResolver.Validate(out string error))
                {
                    Debug.LogWarning($"[MapPreview] Resolver validation failed: {error}");
                    return;
                }
            }

            ClimateData[,] climateMap = ClimateGenerator.GenerateClimateMap(
                mapWidth, mapHeight, climateNoiseSettings);

            Texture2D texture = new Texture2D(mapWidth, mapHeight);
            texture.filterMode = FilterMode.Point;
            Color[] colorMap = new Color[mapWidth * mapHeight];

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    ClimateData climate = climateMap[x, y];
                    colorMap[y * mapWidth + x] = SampleColor(climate);
                }
            }

            texture.SetPixels(colorMap);
            texture.Apply();

            if (textureRenderer)
                textureRenderer.sharedMaterial.mainTexture = texture;
        }

        private Color SampleColor(ClimateData climate)
        {
            switch (drawMode)
            {
                case DrawMode.Temperature:
                    return Color.Lerp(Color.blue, Color.red, climate.temperature);

                case DrawMode.Moisture:
                    return Color.Lerp(new Color(0.6f, 0.4f, 0.2f), new Color(0.1f, 0.4f, 0.9f), climate.moisture);

                case DrawMode.Continentalness:
                    if (climate.continentalness < 0.3f)
                        return Color.Lerp(new Color(0.05f, 0.05f, 0.4f), new Color(0.1f, 0.5f, 0.8f), climate.continentalness / 0.3f);
                    else
                        return Color.Lerp(new Color(0.1f, 0.5f, 0.8f), new Color(0.2f, 0.7f, 0.2f), (climate.continentalness - 0.3f) / 0.7f);

                case DrawMode.Erosion:
                    return Color.Lerp(new Color(0.4f, 0.25f, 0.1f), new Color(0.6f, 0.85f, 0.4f), climate.erosion);

                case DrawMode.Weirdness:
                    return Color.Lerp(Color.black, new Color(0.8f, 0.2f, 0.9f), climate.weirdness);

                case DrawMode.Biome:
                    BiomeBlendResult result = biomeResolver.Resolve(climate);
                    return result.BlendColors(b => b.debugColor);

                default:
                    return Color.magenta;
            }
        }
    }
}