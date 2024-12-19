using Staple;

public class TerrainAsset : IStapleAsset, IGuidAsset
{
    public int width;
    public int height;
    public float scale = 1;

    [SerializeAsBase64]
    public float[] heightData;

    private int guidHash;
    private string guid;

    public int GuidHash => guidHash;

    public string Guid
    {
        get => guid;

        set
        {
            guid = value;

            guidHash = guid?.GetHashCode() ?? 0;
        }
    }

    public static object Create(string guid)
    {
        return Resources.Load<TerrainAsset>(guid);
    }
}
