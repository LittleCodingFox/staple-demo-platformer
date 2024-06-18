using Staple;

public class TerrainAsset : IStapleAsset, IGuidAsset
{
    public int width;
    public int height;
    public float scale = 1;
    public float heightScale = 5;

    [SerializeAsBase64]
    public float[] heightData;

    public string Guid { get; set; }

    public static object Create(string guid)
    {
        return Resources.Load<TerrainAsset>(guid);
    }
}
