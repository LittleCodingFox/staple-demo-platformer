using Staple;

public class TerrainRenderer : Renderable, IComponentDisposable
{
    public TerrainAsset asset;
    public Material material;
    public bool needsUpdate = true;

    internal Mesh mesh;
    internal Mesh.StandardVertex[] meshData = [];

    public void DisposeComponent()
    {
        mesh?.Destroy();
    }
}
