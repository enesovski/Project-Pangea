using UnityEngine;
using Archura.TerrainEngine.Noise;

namespace Archura.TerrainEngine.BiomeModule
{
    [CreateAssetMenu(menuName = "TerrainEngine/GeologyProfile", fileName = "GeologyProfile")]
    public class GeologyProfile : ScriptableObject
    {
        [Header("Height Shape")]
        public AnimationCurve heightCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public float amplitude = 100f;
        [Header("Noise Parameters")]
        public NoiseSettings noiseOverride;
        [Range(0f, 200f)]
        public float domainWarpStrength = 30f;
        [Header("Erosion")]
        [Range(0f, 1f)]
        public float erosionStrength = 0.3f;

        [Range(0f, 90f)]
        public float talusAngle = 35f;

        public float EvaluateHeight(float rawNoise)
        {
            return heightCurve.Evaluate(Mathf.Clamp01(rawNoise)) * amplitude;
        }
    }
}