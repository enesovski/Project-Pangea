using Archura.TerrainEngine.Climate;
using UnityEngine;

namespace Archura.TerrainEngine.BiomeModule
{
    [CreateAssetMenu(menuName = "TerrainEngine/BiomeSettings", fileName = "BiomeSettings")]
    public class BiomeSettings : ScriptableObject
    {
        [Header("Biome Properties")]
        public string biomeName = "New Biome";

        [Header("Climate Noises")]
        [Tooltip("0 = freezing, 1 = hot.")]
        public ClimateParameterRange temperatureRange = new ClimateParameterRange(0f, 1f);

        [Tooltip("0 = arid, 1 = soaking wet.")]
        public ClimateParameterRange moistureRange = new ClimateParameterRange(0f, 1f);

        [Tooltip("Low = ocean, mid = coast, high = deep inland.")]
        public ClimateParameterRange continentalnessRange = new ClimateParameterRange(0f, 1f);

        [Tooltip("Low = mountains, high = flat eroded plains.")]
        public ClimateParameterRange erosionRange = new ClimateParameterRange(0f, 1f);

        [Tooltip("0 = normal, 1 = rare/mutated")]
        public ClimateParameterRange weirdnessRange = new ClimateParameterRange(0f, 1f);

        [Header("Biome Priority")]
        [Tooltip("Offset penalty added to this biome's distance. Higher = less likely to be selected. " +
                 "Use small values (0–0.1) to make common biomes slightly preferred, " +
                 "or higher values (0.2+) for rare biomes that only appear in very specific conditions.")]
        [Range(0f, 1f)]
        public float offset = 0f;

        [Header("Geology")]
        public GeologyProfile geologyProfile;

        [Header("Debug")]
        public Color debugColor = Color.white;


        public float SquaredDistanceTo(ClimateData climate)
        {
            float dist = 0f;
            dist += temperatureRange.SquaredDistance(climate.temperature);
            dist += moistureRange.SquaredDistance(climate.moisture);
            dist += continentalnessRange.SquaredDistance(climate.continentalness);
            dist += erosionRange.SquaredDistance(climate.erosion);
            dist += weirdnessRange.SquaredDistance(climate.weirdness);
            dist += offset * offset; 
            return dist;
        }
        public bool ContainsClimate(float temperature, float moisture)
        {
            return temperatureRange.Contains(temperature) && moistureRange.Contains(moisture);
        }

        public bool ContainsClimate(ClimateData climate)
        {
            return temperatureRange.Contains(climate.temperature)
                && moistureRange.Contains(climate.moisture)
                && continentalnessRange.Contains(climate.continentalness)
                && erosionRange.Contains(climate.erosion)
                && weirdnessRange.Contains(climate.weirdness);
        }
    }
}