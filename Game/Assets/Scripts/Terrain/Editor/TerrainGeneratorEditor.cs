using Staple;
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
                generator.asset.heightData == null ||
                generator.asset.heightData.Length != generator.asset.width * generator.asset.height)
            {
                return;
            }

            var noiseGenerator = new NoiseGenerator();

            noiseGenerator.seed = generator.seed;

            noiseGenerator.frequency = generator.frequency;

            noiseGenerator.noiseType = generator.noiseType;

            noiseGenerator.rotationType3D = generator.rotationType3D;

            noiseGenerator.fractalType = generator.fractalType;

            noiseGenerator.fractalOctaves = generator.fractalOctaves;

            noiseGenerator.fractalLacunarity = generator.fractalLacunarity;

            noiseGenerator.fractalGain = generator.fractalGain;

            noiseGenerator.fractalWeightedStrength = generator.fractalWeightedStrength;

            noiseGenerator.fractalPingPongStrength = generator.fractalPingPongStrength;

            noiseGenerator.cellularDistanceFunction = generator.cellularDistanceFunction;

            noiseGenerator.cellularReturnType = generator.cellularReturnType;

            noiseGenerator.cellularJitter = generator.cellularJitter;

            noiseGenerator.domainWarpType = generator.domainWarpType;

            noiseGenerator.domainWarpAmp = generator.domainWarpAmp;

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
