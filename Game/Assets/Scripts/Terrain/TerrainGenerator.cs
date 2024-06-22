using Staple;
using System.Numerics;
using static Staple.NoiseGenerator;

public class TerrainGenerator: IComponent
{
    public TerrainAsset asset;

    public Vector2 offset;

    public float heightScale = 5;

    public int seed = 1337;

    public float frequency = 0.01f;

    public NoiseType noiseType = NoiseType.OpenSimplex2;

    public RotationType3D rotationType3D = RotationType3D.None;

    public FractalType fractalType = FractalType.None;

    public int fractalOctaves = 3;

    public float fractalLacunarity = 2f;

    public float fractalGain = 0.5f;

    public float fractalWeightedStrength = 0f;

    public float fractalPingPongStrength = 2f;

    public CellularDistanceFunction cellularDistanceFunction = CellularDistanceFunction.EuclideanSq;

    public CellularReturnType cellularReturnType = CellularReturnType.Distance;

    public float cellularJitter = 1f;

    public DomainWarpType domainWarpType = DomainWarpType.OpenSimplex2;

    public float domainWarpAmp = 1f;
}
