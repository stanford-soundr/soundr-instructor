using System.Collections.Generic;
using UnityEngine;

public class ConeGenerator : MonoBehaviour
{
    public float Fov = 30f;
    public float StartDistance = 0.5f;
    public float Distance = 1f;
    public int VerticesCount = 4;
    
    private void Start()
    {
        var positions = new List<Vector3>();
        var uvs = new List<Vector2>();
        var triangles = new List<int>();

        var anglePerIndex = 360f / VerticesCount;
        var vPerIndex = 1f / (VerticesCount - 1);
        for (var i = 0; i < VerticesCount; i++)
        {
            var angle = anglePerIndex * (i + 0.5f);
            var axis = Vector3.forward;
            var rotation = Quaternion.AngleAxis(angle, axis);
            var position = Vector3.forward * StartDistance + rotation * (Mathf.Tan(Fov / 2f / 180f * 3.14f) * StartDistance * Vector3.up);
            positions.Add(position);

            var u = StartDistance;
            var v = i * vPerIndex;
            var uv = new Vector2(u, v);
            uvs.Add(uv);

            var thisIndex = i;
            var nextIndex = (i + 1) % VerticesCount;
            var farIndex = nextIndex + VerticesCount;
            triangles.AddRange(new []{farIndex, thisIndex, nextIndex, farIndex, nextIndex, thisIndex});
        }
        for (var i = 0; i < VerticesCount; i++)
        {
            var angle = anglePerIndex * (i + 0.5f);
            var axis = Vector3.forward;
            var rotation = Quaternion.AngleAxis(angle, axis);
            var position = Vector3.forward * Distance + rotation * (Mathf.Tan(Fov / 2f / 180f * 3.14f) * Distance * Vector3.up);
            positions.Add(position);

            var u = Distance;
            var v = i * vPerIndex;
            var uv = new Vector2(u, v);
            uvs.Add(uv);

            var thisIndex = i + VerticesCount;
            var nextIndex = (i + 1) % VerticesCount + VerticesCount;
            var nearIndex = i;
            triangles.AddRange(new []{nearIndex, thisIndex, nextIndex, nearIndex, nextIndex, thisIndex});
        }
        
        var meshFilter = GetComponent<MeshFilter>();
        var mesh = new Mesh();
        meshFilter.mesh = mesh;
        mesh.vertices = positions.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();
    }
}
