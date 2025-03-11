using Staple;

internal class TerrainRenderer : Renderable, IComponentDisposable
{
    public TerrainAsset asset;
    public Material material;
    public bool needsUpdate = true;

    internal Mesh mesh;
    internal TerrainRenderSystem.TerrainVertex[] meshData = [];

    public void DisposeComponent()
    {
        mesh?.Destroy();
    }
}
