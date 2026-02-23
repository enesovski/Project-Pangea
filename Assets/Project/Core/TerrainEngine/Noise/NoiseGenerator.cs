using UnityEngine;

namespace Archura.TerrainEngine.Noise
{
    public static class NoiseGenerator
    {
        public static float[,] GenerateNoiseMap(int width, int height, NoiseSettings settings)
        {
            float[,] noiseMap = new float[width, height];
            FastNoiseLite noise = new FastNoiseLite();
            
            noise.SetSeed(settings.seed);
            noise.SetNoiseType(settings.noiseType);
            noise.SetFrequency(settings.frequency);
            
            // Fractal & Octaves
            noise.SetFractalType(settings.fractalType);
            noise.SetFractalOctaves(settings.octaves);
            noise.SetFractalLacunarity(settings.lacunarity);
            noise.SetFractalGain(settings.gain);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float rawNoise = noise.GetNoise(x, y);
                    float normalizedNoise = (rawNoise + 1f) / 2f;
                    noiseMap[x, y] = Mathf.Clamp01(normalizedNoise);
                }
            }
            
            return noiseMap;
        }
          
    }
}