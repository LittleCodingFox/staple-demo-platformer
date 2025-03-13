using Staple;
using System.Numerics;

public class TerrainGenerator: IComponent
{
    public TerrainAsset asset;

    public NoiseGeneratorSettings noiseSettings;

    public Vector2 offset;

    public float heightScale = 5;
}
