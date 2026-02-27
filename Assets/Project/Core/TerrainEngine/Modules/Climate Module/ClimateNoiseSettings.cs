using Archura.TerrainEngine.Noise;
using UnityEngine;

namespace Archura.TerrainEngine.Climate
{
    [CreateAssetMenu(menuName = "TerrainEngine/ClimateNoiseSettings", fileName = "ClimateNoiseSettings")]
    public class ClimateNoiseSettings : ScriptableObject
    {
        [Header("Temperature")]
        [Tooltip("Noise layer for the temperature axis.")]
        public NoiseSettings temperatureNoise;

        [Tooltip("How much latitude (north/south position) influences temperature. " +
                 "0 = pure noise, 1 = pure latitude gradient, 0.6 = recommended blend.")]
        [Range(0f, 1f)]
        public float latitudeInfluence = 0.6f;

        [Header("Moisture / Humidity")]
        [Tooltip("Noise layer for the moisture axis.")]
        public NoiseSettings moistureNoise;

        [Header("Continentalness")]
        [Tooltip("Low-frequency noise that carves oceans and defines coastlines. " +
                 "Use a very low frequency (e.g. 0.003) for large ocean masses.")]
        public NoiseSettings continentalnessNoise;

        [Header("Erosion")]
        [Tooltip("Noise that controls flat vs mountainous regions. " +
                 "High erosion = plains, low erosion = mountains.")]
        public NoiseSettings erosionNoise;

        [Header("Weirdness (Optional)")]
        [Tooltip("Optional noise axis for biome variants. Leave null to disable.")]
        public NoiseSettings weirdnessNoise;
    }
}