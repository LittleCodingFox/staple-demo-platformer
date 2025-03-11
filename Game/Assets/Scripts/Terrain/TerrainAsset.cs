using Staple;

public class TerrainAsset : IStapleAsset, IGuidAsset
{
    public int width;
    public int height;
    public float scale = 1;

    [SerializeAsBase64]
    public float[] heightData;

    private readonly GuidHasher guidHasher = new();

    public GuidHasher Guid => guidHasher;

    public static object Create(string guid)
    {
        return Resources.Load<TerrainAsset>(guid);
    }
}
