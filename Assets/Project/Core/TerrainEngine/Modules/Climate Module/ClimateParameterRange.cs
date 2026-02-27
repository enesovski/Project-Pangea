using System;
using UnityEngine;

namespace Archura.TerrainEngine.Climate
{
    [Serializable]
    public struct ClimateParameterRange
    {
        [Range(0f, 1f)] public float min;
        [Range(0f, 1f)] public float max;

        public ClimateParameterRange(float min, float max)
        {
            this.min = Mathf.Clamp01(min);
            this.max = Mathf.Clamp01(max);
        }
        
        public float Center => (min + max) * 0.5f;
        
        public bool Contains(float value) => value >= min && value <= max;
        
        public float SquaredDistance(float value)
        {
            if (value < min)
            {
                float d = min - value;
                return d * d;
            }
            if (value > max)
            {
                float d = value - max;
                return d * d;
            }
            return 0f;
        }

        public static ClimateParameterRange Full => new ClimateParameterRange(0f, 1f);

        public override string ToString() => $"[{min:F2}–{max:F2}]";
    }
}