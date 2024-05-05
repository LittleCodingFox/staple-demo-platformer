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

    private Dictionary<Vector2Int, (TerrainVertex[], int[])> cachedTerrainSizes = new();

    private void CacheTerrain(int width, int height)
    {
        if(cachedTerrainSizes.ContainsKey(new Vector2Int(width, height)))
        {
            return;
        }

        var newVertices = new TerrainVertex[width * height * 6];
        var indices = Enumerable.Repeat(0, width * height * 6).Select((x, xIndex) => xIndex).ToArray();

        var vertexCounter = 0;

        for(var y = 0; y < height; y++)
        {
            for(var x = 0; x < width; x++)
            {
                newVertices[vertexCounter++].position = new Vector3(x, 0, y);
                newVertices[vertexCounter++].position = new Vector3(x, 0, y + 1);
                newVertices[vertexCounter++].position = new Vector3(x + 1, 0, y + 1);
                newVertices[vertexCounter++].position = new Vector3(x + 1, 0, y + 1);
                newVertices[vertexCounter++].position = new Vector3(x + 1, 0, y);
                newVertices[vertexCounter++].position = new Vector3(x, 0, y);
            }
        }

        cachedTerrainSizes.Add(new Vector2Int(width, height), (newVertices, indices));
    }

    private (TerrainVertex[], int[]) GetCache(int width, int height)
    {
        CacheTerrain(width, height);

        return cachedTerrainSizes.TryGetValue(new Vector2Int(width, height), out var value) ? value : ([], []);
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

            renderer.mesh.SetMeshData(data.Item1, new VertexLayoutBuilder()
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
                    Vertices = data.Item1.Select(x => x.position).ToArray(),
                    Indices = renderer.mesh.Indices,
                };

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
