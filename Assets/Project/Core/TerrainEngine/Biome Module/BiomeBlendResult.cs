using System;
using UnityEngine;

namespace Archura.TerrainEngine.BiomeModule
{
    [Serializable]
    public struct WeightedBiome
    {
        public BiomeSettings biome;
        public float weight;

        public WeightedBiome(BiomeSettings biome, float weight)
        {
            this.biome  = biome;
            this.weight = weight;
        }
    }

    public readonly struct BiomeBlendResult
    {
        
        //maximum 4 biomes can blend, because of grid structure
        private readonly WeightedBiome _e0;
        private readonly WeightedBiome _e1;
        private readonly WeightedBiome _e2;
        private readonly WeightedBiome _e3;
        private readonly int _count;

        public int Count => _count;

        public BiomeSettings DominantBiome
        {
            get
            {
                var best = _e0;
                if (_count > 1 && _e1.weight > best.weight) best = _e1;
                if (_count > 2 && _e2.weight > best.weight) best = _e2;
                if (_count > 3 && _e3.weight > best.weight) best = _e3;
                return best.biome;
            }
        }

        public BiomeBlendResult(WeightedBiome e0)
        {
            _e0 = e0; _e1 = default; _e2 = default; _e3 = default;
            _count = 1;
        }

        public BiomeBlendResult(WeightedBiome e0, WeightedBiome e1, WeightedBiome e2, WeightedBiome e3)
        {
            _e0 = e0; _e1 = e1; _e2 = e2; _e3 = e3;
            _count = 4;
        }

        public float BlendFloats(Func<BiomeSettings, float> selector)
        {
            float value = _e0.weight * selector(_e0.biome);
            if (_count > 1) value += _e1.weight * selector(_e1.biome);
            if (_count > 2) value += _e2.weight * selector(_e2.biome);
            if (_count > 3) value += _e3.weight * selector(_e3.biome);
            return value;
        }

        public Color BlendColors(Func<BiomeSettings, Color> selector)
        {
            Color value = _e0.weight * selector(_e0.biome);
            if (_count > 1) value += _e1.weight * selector(_e1.biome);
            if (_count > 2) value += _e2.weight * selector(_e2.biome);
            if (_count > 3) value += _e3.weight * selector(_e3.biome);
            return value;
        }

        public void ForEach(Action<WeightedBiome> action)
        {
            action(_e0);
            if (_count > 1) action(_e1);
            if (_count > 2) action(_e2);
            if (_count > 3) action(_e3);
        }
    }
}