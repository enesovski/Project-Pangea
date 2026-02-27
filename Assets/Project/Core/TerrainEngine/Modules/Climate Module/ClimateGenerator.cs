using Archura.TerrainEngine.Noise;
using UnityEngine;

namespace Archura.TerrainEngine.Climate
{
    public static class ClimateGenerator
    {
        public static ClimateData[,] GenerateClimateMap(int width, int height, ClimateNoiseSettings settings)
        {
            float[,] tempNoise   = NoiseGenerator.GenerateNoiseMap(width, height, settings.temperatureNoise);
            float[,] moistNoise  = NoiseGenerator.GenerateNoiseMap(width, height, settings.moistureNoise);
            float[,] contNoise   = NoiseGenerator.GenerateNoiseMap(width, height, settings.continentalnessNoise);
            float[,] erosNoise   = NoiseGenerator.GenerateNoiseMap(width, height, settings.erosionNoise);
            float[,] weirdNoise  = settings.weirdnessNoise != null
                ? NoiseGenerator.GenerateNoiseMap(width, height, settings.weirdnessNoise)
                : null;

            ClimateData[,] climateMap = new ClimateData[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float latitude        = (float)y / height;
                    float distFromEquator = Mathf.Abs(latitude - 0.5f) * 2f;
                    float baseTemp        = 1f - distFromEquator;

                    float finalTemp = Mathf.Clamp01(
                        baseTemp * settings.latitudeInfluence +
                        tempNoise[x, y] * (1f - settings.latitudeInfluence));

                    float finalMoist = moistNoise[x, y];
                    float finalCont  = contNoise[x, y];
                    float finalEros  = erosNoise[x, y];

                    float finalWeird = weirdNoise != null ? weirdNoise[x, y] : 0f;

                    climateMap[x, y] = new ClimateData(finalTemp, finalMoist, finalCont, finalEros, finalWeird);
                }
            }

            return climateMap;
        }
    }
}