using UnityEngine;

namespace Archura.TerrainEngine.Noise
{
    [CreateAssetMenu(menuName = "TerrainEngine/NoiseSettings", fileName = "NoiseSettings")]
    public class NoiseSettings : ScriptableObject
    {
        public int seed;
        
        [Header("Base Settings")]
        public FastNoiseLite.NoiseType noiseType;
        public float frequency;

        [Header("Fractal Settings")]
        public FastNoiseLite.FractalType fractalType;
        [Range(1, 8)] public int octaves;
        public float lacunarity;
        public float gain;
        
        public NoiseSettings(int defaultSeed = 42)
        {
            seed = defaultSeed;
            noiseType = FastNoiseLite.NoiseType.OpenSimplex2;
            frequency = 0.01f;
            fractalType = FastNoiseLite.FractalType.FBm;
            octaves = 4;
            lacunarity = 2.0f;
            gain = 0.5f;
        }
    }
}