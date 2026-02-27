using Archura.TerrainEngine.Climate;

namespace Archura.TerrainEngine.BiomeModule
{
    public interface IBiomeResolver
    {
        BiomeBlendResult Resolve(ClimateData climate);

        bool Validate(out string errorMessage);
    }
}