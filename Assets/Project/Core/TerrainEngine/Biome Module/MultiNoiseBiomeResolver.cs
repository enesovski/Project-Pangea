using UnityEngine;
using Archura.TerrainEngine.Climate;

namespace Archura.TerrainEngine.BiomeModule
{
    [CreateAssetMenu(menuName = "TerrainEngine/Resolvers/MultiNoiseBiomeResolver", fileName = "MultiNoiseBiomeResolver")]
    public class MultiNoiseBiomeResolver : BiomeResolverBase
    {
        [Tooltip("All biomes in the world. The resolver picks the closest match for each climate sample.")]
        public BiomeSettings[] biomes;

        [Range(0f, 0.2f)]
        public float blendRadius = 0.02f;

        [Header("Axis Weights")]
        [Tooltip("Importance multiplier for each climate axis during distance calculation. " +
                 "Higher weight = that axis has more influence on biome selection.")]
        [Range(0.1f, 5f)] public float temperatureWeight     = 1f;
        [Range(0.1f, 5f)] public float moistureWeight        = 1f;
        [Range(0.1f, 5f)] public float continentalnessWeight = 1f;
        [Range(0.1f, 5f)] public float erosionWeight         = 1f;
        [Range(0.1f, 5f)] public float weirdnessWeight       = 0.5f;

        public override BiomeBlendResult Resolve(ClimateData climate)
        {
            if (biomes == null || biomes.Length == 0)
                return default;

            float bestDist = float.MaxValue;
            int bestIdx = 0;

            for (int i = 0; i < biomes.Length; i++)
            {
                if (!biomes[i]) continue;
                float d = WeightedSquaredDistance(biomes[i], climate);
                if (d < bestDist)
                {
                    bestDist = d;
                    bestIdx  = i;
                }
            }

            if (blendRadius <= 0f || bestDist <= 0f)
                return new BiomeBlendResult(new WeightedBiome(biomes[bestIdx], 1f));

            float threshold = bestDist + blendRadius;

            BiomeSettings s0 = null, s1 = null, s2 = null, s3 = null;
            float d0 = float.MaxValue, d1 = float.MaxValue, d2 = float.MaxValue, d3 = float.MaxValue;
            int count = 0;

            for (int i = 0; i < biomes.Length; i++)
            {
                if (biomes[i] == null) continue;
                float d = WeightedSquaredDistance(biomes[i], climate);
                if (d > threshold) continue;

                // Insert into sorted top-4.
                if (d < d0)      { ShiftDown(ref s0, ref d0, ref s1, ref d1, ref s2, ref d2, ref s3, ref d3); s0 = biomes[i]; d0 = d; count = Mathf.Min(count + 1, 4); }
                else if (d < d1) { ShiftDown(ref s1, ref d1, ref s2, ref d2, ref s3, ref d3);                  s1 = biomes[i]; d1 = d; count = Mathf.Min(count + 1, 4); }
                else if (d < d2) { ShiftDown(ref s2, ref d2, ref s3, ref d3);                                   s2 = biomes[i]; d2 = d; count = Mathf.Min(count + 1, 4); }
                else if (d < d3) {                                                                               s3 = biomes[i]; d3 = d; count = Mathf.Min(count + 1, 4); }
            }

            if (count <= 1)
                return new BiomeBlendResult(new WeightedBiome(s0, 1f));

            const float eps = 1e-6f;
            float w0 = 1f / (d0 + eps);
            float w1 = count > 1 ? 1f / (d1 + eps) : 0f;
            float w2 = count > 2 ? 1f / (d2 + eps) : 0f;
            float w3 = count > 3 ? 1f / (d3 + eps) : 0f;

            float wSum = w0 + w1 + w2 + w3;
            w0 /= wSum; w1 /= wSum; w2 /= wSum; w3 /= wSum;

            return new BiomeBlendResult(
                new WeightedBiome(s0, w0),
                new WeightedBiome(s1 ?? s0, w1),
                new WeightedBiome(s2 ?? s0, w2),
                new WeightedBiome(s3 ?? s0, w3)
            );
        }

        private float WeightedSquaredDistance(BiomeSettings biome, ClimateData climate)
        {
            float dist = 0f;
            dist += biome.temperatureRange.SquaredDistance(climate.temperature)         * temperatureWeight;
            dist += biome.moistureRange.SquaredDistance(climate.moisture)               * moistureWeight;
            dist += biome.continentalnessRange.SquaredDistance(climate.continentalness) * continentalnessWeight;
            dist += biome.erosionRange.SquaredDistance(climate.erosion)                 * erosionWeight;
            dist += biome.weirdnessRange.SquaredDistance(climate.weirdness)             * weirdnessWeight;
            dist += biome.offset * biome.offset;
            return dist;
        }

        private static void ShiftDown(
            ref BiomeSettings a, ref float da,
            ref BiomeSettings b, ref float db,
            ref BiomeSettings c, ref float dc,
            ref BiomeSettings d, ref float dd)
        {
            d = c; dd = dc;
            c = b; dc = db;
            b = a; db = da;
        }

        private static void ShiftDown(
            ref BiomeSettings a, ref float da,
            ref BiomeSettings b, ref float db,
            ref BiomeSettings c, ref float dc)
        {
            c = b; dc = db;
            b = a; db = da;
        }

        private static void ShiftDown(
            ref BiomeSettings a, ref float da,
            ref BiomeSettings b, ref float db)
        {
            b = a; db = da;
        }

        public override bool Validate(out string errorMessage)
        {
            if (biomes == null || biomes.Length == 0)
            {
                errorMessage = $"{nameof(MultiNoiseBiomeResolver)}: No biomes assigned.";
                return false;
            }
            for (int i = 0; i < biomes.Length; i++)
            {
                if (biomes[i] == null)
                {
                    errorMessage = $"{nameof(MultiNoiseBiomeResolver)}: Biome at index [{i}] is null.";
                    return false;
                }
            }
            errorMessage = null;
            return true;
        }
    }
}