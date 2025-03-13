using Staple.Editor;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.Button("Generate", "Generate", () =>
        {
            if(target is not TerrainGenerator generator ||
                generator.asset == null ||
                generator.noiseSettings == null)
            {
                return;
            }

            generator.asset.heightData = new float[generator.asset.width * generator.asset.height];

            var noiseGenerator = generator.noiseSettings.MakeGenerator();

            for (int y = 0, yIndex = 0; y < generator.asset.height; y++, yIndex += generator.asset.width)
            {
                for (var x = 0; x < generator.asset.width; x++)
                {
                    generator.asset.heightData[x + yIndex] = noiseGenerator.GetNoise(x + generator.offset.X, y + generator.offset.Y) * generator.heightScale;
                }
            }

            EditorUtils.SaveAsset(generator.asset);
        });
    }
}
