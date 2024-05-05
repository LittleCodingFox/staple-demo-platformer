using Staple;

public class TerrainAsset : IStapleAsset, IGuidAsset
{
    public int width;
    public int height;

    [SerializeAsBase64]
    public float[] heightData;

    public string Guid { get; set; }

    public static object Create(string guid)
    {
        return Resources.Load<TerrainAsset>(guid);
    }
}
