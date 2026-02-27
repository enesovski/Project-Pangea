using UnityEngine;
using Archura.TerrainEngine.Climate;

namespace Archura.TerrainEngine.BiomeModule
{
    public abstract class BiomeResolverBase : ScriptableObject, IBiomeResolver
    {
        public abstract BiomeBlendResult Resolve(ClimateData climate);
        public abstract bool Validate(out string errorMessage);
    }
}