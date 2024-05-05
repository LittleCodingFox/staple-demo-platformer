using Staple.Editor;

[CustomEditor(typeof(TerrainAsset))]
internal class TerrainAssetEditor : StapleAssetEditor
{
    public override void OnInspectorGUI()
    {
        var asset = (TerrainAsset)target;

        if (asset == null)
        {
            return;
        }

        if (asset.width <= 0)
        {
            asset.width = 1;
        }

        if (asset.height <= 0)
        {
            asset.height = 1;
        }

        if (asset.heightData == null || asset.heightData.Length != asset.width * asset.height)
        {
            asset.heightData = new float[asset.width * asset.height];
        }

        base.OnInspectorGUI();
    }
}
