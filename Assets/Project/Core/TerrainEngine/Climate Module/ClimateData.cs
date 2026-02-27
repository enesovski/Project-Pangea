namespace Archura.TerrainEngine.Climate
{
    [System.Serializable]
    public struct ClimateData
    {
        public float temperature;
        public float moisture;
        public float continentalness;
        public float erosion;
        public float weirdness;

        public ClimateData(float temperature, float moisture)
        {
            this.temperature     = temperature;
            this.moisture        = moisture;
            this.continentalness = 0.5f;
            this.erosion = 0.5f;
            this.weirdness = 0f;
        }

        public ClimateData(float temperature, float moisture, float continentalness, float erosion, float weirdness = 0f)
        {
            this.temperature     = temperature;
            this.moisture        = moisture;
            this.continentalness = continentalness;
            this.erosion         = erosion;
            this.weirdness       = weirdness;
        }

        public override string ToString()
        {
            return $"Climate(T:{temperature:F2} M:{moisture:F2} C:{continentalness:F2} E:{erosion:F2} W:{weirdness:F2})";
        }
    }
}