using UnityEngine;

public class GroundGridGenerator : MonoBehaviour
{
    public Vector3[] Vertices = new Vector3[4];
    private readonly Vector2[] _uv = new Vector2[4];
    private readonly int[] _triangles = {0, 1, 2, 1, 2, 3, 3, 2, 1, 2, 1, 0};
    private MeshFilter _meshFilter;

    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        var mesh = new Mesh();
        _meshFilter.mesh = mesh;
        mesh.vertices = Vertices;
        mesh.uv = _uv;
        mesh.triangles = _triangles;
    }

    // Update is called once per frame
    private void Update()
    {
        Vertices[1].y = Vertices[0].y;
        Vertices[3].y = Vertices[0].y;
        Vertices[2] = Vertices[3] + (Vertices[0] - Vertices[1]);
        _uv[0] = Vector2.zero;
        _uv[1] = Vector2.right * (Vertices[1] - Vertices[0]).magnitude;
        _uv[2] = Vector2.down * (Vertices[2] - Vertices[0]).magnitude;
        _uv[3] = _uv[1] + _uv[2];
        var mesh = _meshFilter.mesh;
        mesh.Clear();
        mesh.vertices = Vertices;
        mesh.uv = _uv;
        mesh.triangles = _triangles;
    }
}