using Staple;
using Staple.Internal;
using System;
using System.Collections.Generic;
using System.Numerics;

public class TerrainRenderSystem : RenderSystemBase
{
    private struct RenderInfo
    {
        public Entity entity;
        public TerrainAsset asset;
        public TerrainRenderer renderer;
        public Material material;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    private readonly Dictionary<int, ExpandableContainer<RenderInfo>> renderers = [];

    private Texture defaultTexture;

    private readonly Dictionary<Vector2Int, (Mesh.StandardVertex[], int[])> cachedTerrainSizes = [];

    public TerrainRenderSystem() : base(false, typeof(TerrainRenderer), typeof(GenericRenderQueue<TerrainRenderer>))
    {
    }

    public override IRenderQueue CreateRenderQueue() => new GenericRenderQueue<TerrainRenderer>();

    private void CacheTerrain(int width, int height)
    {
        if(cachedTerrainSizes.ContainsKey(new Vector2Int(width, height)))
        {
            return;
        }

        var newVertices = new Mesh.StandardVertex[width * height * 4];
        var indices = new List<int>();

        var vertexCounter = 0;

        for(var i = 0; i < newVertices.Length; i++)
        {
            newVertices[i].normal = new Vector3(0, 1, 0);
        }

        for(var y = -height / 2; y < height / 2 - 1; y++)
        {
            for(var x = -width / 2; x < width / 2 - 1; x++)
            {
                indices.AddRange([vertexCounter, vertexCounter + 1, vertexCounter + 2, vertexCounter + 2, vertexCounter + 3, vertexCounter]);

                newVertices[vertexCounter++].position = new Vector3(x, 0, y);
                newVertices[vertexCounter - 1].uv = Vector2.Zero;

                newVertices[vertexCounter++].position = new Vector3(x, 0, y + 1);
                newVertices[vertexCounter - 1].uv = new Vector2(0, 1);

                newVertices[vertexCounter++].position = new Vector3(x + 1, 0, y + 1);
                newVertices[vertexCounter - 1].uv = new Vector2(1, 1);

                newVertices[vertexCounter++].position = new Vector3(x + 1, 0, y);
                newVertices[vertexCounter - 1].uv = new Vector2(1, 0);
            }
        }

        cachedTerrainSizes.Add(new Vector2Int(width, height), (newVertices, indices.ToArray()));
    }

    private (Mesh.StandardVertex[], int[]) GetCache(int width, int height)
    {
        CacheTerrain(width, height);

        return cachedTerrainSizes.TryGetValue(new Vector2Int(width, height), out var value) ? value : ([], []);
    }

    public override void Startup()
    {
    }

    public override void Shutdown()
    {
    }

    public override void Prepare()
    {
        defaultTexture ??= Resources.Load<Texture>("Hidden/Textures/Sprites/DefaultSprite.png");

        foreach(var pair in renderers)
        {
            pair.Value.Clear();
        }
    }

    private void UpdateHeights(TerrainRenderer renderer)
    {
        var noise = new NoiseGenerator();

        noise.frequency = 0.01f;
        noise.fractalType = NoiseGenerator.FractalType.FBm;
        noise.fractalGain = 10;

        for (int y = 0, yIndex = 0; y < renderer.asset.height; y++, yIndex += renderer.asset.width)
        {
            for (var x = 0; x < renderer.asset.width; x++)
            {
                renderer.asset.heightData[x + yIndex] = noise.GetNoise(x, y);
            }
        }

        renderer.needsUpdate = false;
    }

    private void UpdateMesh(TerrainRenderer renderer, int[] indices)
    {
        float GetHeight(int x, int y)
        {
            return renderer.asset.heightData[x + y * renderer.asset.width];
        }

        var vertexCounter = 0;
        var positions = new Vector3[renderer.meshData.Length];

        for (var y = 0; y < renderer.asset.height - 1; y++)
        {
            for (var x = 0; x < renderer.asset.width - 1; x++)
            {
                void SetHeight(float height)
                {
                    renderer.meshData[vertexCounter].position.Y = height * renderer.asset.scale;
                    positions[vertexCounter] = renderer.meshData[vertexCounter].position;

                    vertexCounter++;
                }

                SetHeight(GetHeight(x, y));
                SetHeight(GetHeight(x, y + 1));
                SetHeight(GetHeight(x + 1, y + 1));
                SetHeight(GetHeight(x + 1, y));
            }
        }

        var normals = Mesh.GenerateNormals(positions, indices, true);

        if (normals.Length == renderer.meshData.Length)
        {
            for (var i = 0; i < renderer.meshData.Length; i++)
            {
                renderer.meshData[i].normal = normals[i];
            }
        }

        renderer.mesh.SetMeshData(renderer.meshData.AsSpan(), Mesh.StandardVertexLayout.Value);
    }

    private void UpdateMeshBounds(TerrainRenderer renderer)
    {
        var points = new Vector3[renderer.meshData.Length];

        for(var i = 0; i < renderer.meshData.Length; i++)
        {
            points[i] = renderer.meshData[i].position;
        }

        renderer.localBounds = AABB.CreateFromPoints(points);
    }

    public override void Preprocess(IRenderQueue renderQueue)
    {
        if(renderQueue is not GenericRenderQueue<TerrainRenderer> queue)
        {
            return;
        }

        var items = queue.Items;

        foreach(var entry in items)
        {
            if (entry.component is not TerrainRenderer renderer ||
                renderer.enabled == false ||
                renderer.forceRenderingOff ||
                renderer.asset == null ||
                renderer.materials == null ||
                renderer.materials.Count != 1 ||
                renderer.asset.width <= 0 ||
                renderer.asset.height <= 0 ||
                renderer.asset.heightData == null ||
                renderer.asset.heightData.Length != renderer.asset.width * renderer.asset.height)
            {
                continue;
            }

            if (renderer.mesh == null ||
                renderer.mesh.VertexCount != renderer.asset.width * renderer.asset.height * 4)
            {
                if (Platform.IsPlaying)
                {
                    renderer.mesh?.Clear();
                }

                renderer.mesh ??= new Mesh()
                {
                    MeshTopology = MeshTopology.Triangles,
                    IndexFormat = MeshIndexFormat.UInt32,
                };

                if (Platform.IsEditor)
                {
                    renderer.mesh.MarkDynamic();
                }

                var data = GetCache(renderer.asset.width, renderer.asset.height);

                renderer.meshData = new Mesh.StandardVertex[data.Item1.Length];

                Array.Copy(data.Item1, renderer.meshData, data.Item1.Length);

                for (var j = 0; j < renderer.meshData.Length; j++)
                {
                    renderer.meshData[j].position.X *= renderer.asset.scale;
                    renderer.meshData[j].position.Z *= renderer.asset.scale;
                }

                UpdateMesh(renderer, data.Item2);

                renderer.mesh.Indices = data.Item2;

                UpdateMeshBounds(renderer);

                if (entry.entity.TryGetComponent<HeightMapCollider3D>(out var collider))
                {
                    collider.heights = renderer.asset.heightData;
                    collider.offset = new Vector3(-0.5f * renderer.asset.width * renderer.asset.scale, 0, -0.5f * renderer.asset.height * renderer.asset.scale);
                    collider.scale = Vector3.One * renderer.asset.scale;

                    Physics3D.Instance.RecreateBody(entry.entity);
                }
            }
            else if (renderer.needsUpdate)
            {
                renderer.needsUpdate = false;

                var data = GetCache(renderer.asset.width, renderer.asset.height);

                UpdateMesh(renderer, data.Item2);

                UpdateMeshBounds(renderer);
            }

            renderer.bounds = new(renderer.localBounds.center * entry.transform.Scale + entry.transform.Position,
                renderer.localBounds.size * entry.transform.Scale);
        }
    }

    public override void Process(IRenderQueue renderQueue, Camera activeCamera, Transform activeCameraTransform, int renderIndex)
    {
        if (renderQueue is not GenericRenderQueue<TerrainRenderer> queue)
        {
            return;
        }

        if (!renderers.TryGetValue(renderIndex, out var renderData))
        {
            renderData = new();

            renderers.AddOrSetKey(renderIndex, renderData);
        }

        var items = queue.Items;

        foreach (var entry in items)
        {
            if (entry.component is not TerrainRenderer renderer ||
                renderer.enabled == false ||
                renderer.forceRenderingOff ||
                renderer.asset == null ||
                renderer.materials == null ||
                renderer.materials.Count != 1 ||
                renderer.asset.width <= 0 ||
                renderer.asset.height <= 0 ||
                renderer.asset.heightData == null ||
                renderer.asset.heightData.Length != renderer.asset.width * renderer.asset.height ||
                renderer.mesh == null ||
                !IsValidMaterial(renderer.materials[0], renderIndex))
            {
                continue;
            }

            renderData.Add(new()
            {
                asset = renderer.asset,
                entity = entry.entity,
                material = renderer.materials[0],
                renderer = renderer,
                position = entry.transform.Position,
                rotation = entry.transform.Rotation,
                scale = entry.transform.Scale,
            });
        }
    }

    public override void Submit()
    {
        foreach(var (renderIndex, queue) in renderers)
        {
            var length = queue.Length;

            var items = queue.Contents;

            for (var i = 0; i < length; i++)
            {
                var renderer = items[i];

                if (renderer.renderer == null)
                {
                    continue;
                }

                MeshRenderSystem.RenderMesh(renderer.renderer.mesh, renderer.position, renderer.rotation, renderer.scale, renderer.material,
                    MaterialLighting.Lit);
            }
        }
    }
}
