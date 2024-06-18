using Staple;
using Staple.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

internal class TerrainRenderSystem : IRenderSystem
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    private struct TerrainVertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 uv;
    }

    private class RenderInfo
    {
        public Entity entity;
        public TerrainAsset asset;
        public TerrainRenderer renderer;
        public Material material;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public ushort viewID;
    }

    private List<RenderInfo> renderers = new();

    private Dictionary<Vector2Int, (TerrainVertex[], int[], int[])> cachedTerrainSizes = new();

    private void CacheTerrain(int width, int height)
    {
        if(cachedTerrainSizes.ContainsKey(new Vector2Int(width, height)))
        {
            return;
        }

        var newVertices = new TerrainVertex[width * height * 6];
        var indices = new List<int>();
        var collisionIndices = new List<int>();

        var vertexCounter = 0;

        for(var i = 0; i < newVertices.Length; i++)
        {
            newVertices[i].normal = new Vector3(0, 1, 0);
        }

        for(var y = -height / 2; y < height / 2 - 1; y++)
        {
            for(var x = -width / 2; x < width / 2 - 1; x++)
            {
                indices.AddRange([vertexCounter + 3, vertexCounter + 2, vertexCounter + 1, vertexCounter + 1, vertexCounter, vertexCounter + 3]);
                collisionIndices.AddRange([vertexCounter, vertexCounter + 1, vertexCounter + 2, vertexCounter + 2, vertexCounter + 3, vertexCounter]);

                newVertices[vertexCounter++].position = new Vector3(x, 0, y);
                newVertices[vertexCounter++].position = new Vector3(x, 0, y + 1);
                newVertices[vertexCounter++].position = new Vector3(x + 1, 0, y + 1);
                newVertices[vertexCounter++].position = new Vector3(x + 1, 0, y);
            }
        }

        cachedTerrainSizes.Add(new Vector2Int(width, height), (newVertices, indices.ToArray(), collisionIndices.ToArray()));
    }

    private (TerrainVertex[], int[], int[]) GetCache(int width, int height)
    {
        CacheTerrain(width, height);

        return cachedTerrainSizes.TryGetValue(new Vector2Int(width, height), out var value) ? value : ([], [], []);
    }

    public void Destroy()
    {
    }

    public void Prepare()
    {
        renderers.Clear();
    }

    public void Preprocess(Entity entity, Transform transform, IComponent relatedComponent, Camera activeCamera, Transform activeCameraTransform)
    {
        if(relatedComponent is not TerrainRenderer renderer ||
            renderer.asset == null ||
            renderer.material == null ||
            renderer.material.Disposed ||
            renderer.asset.width <= 0 ||
            renderer.asset.height <= 0 ||
            renderer.asset.heightData == null ||
            renderer.asset.heightData.Length != renderer.asset.width * renderer.asset.height)
        {
            return;
        }

        if(renderer.mesh == null ||
            renderer.mesh.VertexCount != renderer.asset.width * renderer.asset.height * 6)
        {
            if(Platform.IsPlaying)
            {
                renderer.mesh?.Clear();
            }

            var data = GetCache(renderer.asset.width, renderer.asset.height);

            renderer.mesh = new Mesh()
            {
                MeshTopology = MeshTopology.Triangles,
                IndexFormat = MeshIndexFormat.UInt32,
            };

            if(Platform.IsEditor)
            {
                renderer.mesh.MarkDynamic();
            }

            var localMeshData = data.Item1;

            var noise = new NoiseGenerator();

            noise.frequency = 0.01f;
            noise.fractalType = NoiseGenerator.FractalType.FBm;
            noise.fractalGain = 10;

            float GetHeight(int x, int y)
            {
                return noise.GetNoise(x, y);
            }

            var vertexCounter = 0;
            var positions = new Vector3[localMeshData.Length];

            for (var y = 0; y < renderer.asset.height - 1; y++)
            {
                for (var x = 0; x < renderer.asset.width - 1; x++)
                {
                    void SetHeight(float height)
                    {
                        localMeshData[vertexCounter].position.Y = height * renderer.asset.heightScale;
                        localMeshData[vertexCounter].position.X *= renderer.asset.scale;
                        localMeshData[vertexCounter].position.Z *= renderer.asset.scale;
                        positions[vertexCounter] = localMeshData[vertexCounter].position;

                        vertexCounter++;
                    }

                    SetHeight(GetHeight(x, y));
                    SetHeight(GetHeight(x, y + 1));
                    SetHeight(GetHeight(x + 1, y + 1));
                    SetHeight(GetHeight(x + 1, y));
                }
            }

            var normals = Mesh.GenerateNormals(positions, data.Item2, true);

            if(normals.Length == localMeshData.Length)
            {
                for(var i = 0; i < localMeshData.Length; i++)
                {
                    localMeshData[i].normal = normals[i];
                }
            }

            renderer.mesh.SetMeshData(localMeshData, new VertexLayoutBuilder()
                .Add(Bgfx.bgfx.Attrib.Position, 3, Bgfx.bgfx.AttribType.Float)
                .Add(Bgfx.bgfx.Attrib.Normal, 3, Bgfx.bgfx.AttribType.Float)
                .Add(Bgfx.bgfx.Attrib.TexCoord0, 2, Bgfx.bgfx.AttribType.Float)
                .Build());

            renderer.mesh.Indices = data.Item2;

            renderer.mesh.UploadMeshData();

            if(entity.TryGetComponent<MeshCollider3D>(out var meshCollider))
            {
                meshCollider.mesh = new Mesh()
                {
                    MeshTopology = MeshTopology.Triangles,
                    IndexFormat = MeshIndexFormat.UInt32,
                    Vertices = localMeshData.Select(x => x.position).ToArray(),
                    Normals = normals,
                    Indices = data.Item3,
                };

                Log.Debug($"RECREATE BODY");

                Physics3D.Instance.RecreateBody(entity);
            }
        }
    }

    public void Process(Entity entity, Transform transform, IComponent relatedComponent, Camera activeCamera, Transform activeCameraTransform, ushort viewId)
    {
        if (relatedComponent is not TerrainRenderer renderer ||
            renderer.asset == null ||
            renderer.material == null ||
            renderer.material.Disposed ||
            renderer.asset.width <= 0 ||
            renderer.asset.height <= 0 ||
            renderer.asset.heightData == null ||
            renderer.asset.heightData.Length != renderer.asset.width * renderer.asset.height ||
            renderer.mesh == null)
        {
            return;
        }

        renderers.Add(new()
        {
            asset = renderer.asset,
            entity = entity,
            material = renderer.material,
            renderer = renderer,
            position = transform.Position,
            rotation = transform.Rotation,
            scale = transform.Scale,
            viewID = viewId,
        });
    }

    public Type RelatedComponent()
    {
        return typeof(TerrainRenderer);
    }

    public void Submit()
    {
        foreach(var renderer in renderers)
        {
            MeshRenderSystem.DrawMesh(renderer.renderer.mesh, renderer.position, renderer.rotation, renderer.scale, renderer.material, renderer.viewID);
        }
    }
}
