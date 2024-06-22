using Staple;
using System;

internal class TerrainRenderer : Renderable
{
    public TerrainAsset asset;
    public Material material;
    public bool needsUpdate = true;

    internal Mesh mesh;
    internal TerrainRenderSystem.TerrainVertex[] meshData = Array.Empty<TerrainRenderSystem.TerrainVertex>();
}
