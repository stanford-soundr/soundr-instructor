using UnityEngine;

public class ConeManager: MonoBehaviour
{
    // ReSharper disable once InconsistentNaming
    public Material _material;

    public Transform HeadTransform;

    public float PositionalDistance;
    public float RotationalDistance;

    public float TotalDistance;
    private static readonly int Color = Shader.PropertyToID("_GridColor");

    private void Start()
    {
        _material = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        var positionalDifference = (transform.position - HeadTransform.position);
        positionalDifference.y = 0;
        PositionalDistance = positionalDifference.magnitude;
        (transform.rotation * Quaternion.Inverse(HeadTransform.rotation)).ToAngleAxis(out var angle, out _);
        RotationalDistance = Mathf.Min(angle, 360 - angle);

        TotalDistance = RotationalDistance / 10 + PositionalDistance;

        var colorR = Mathf.Clamp01(Mathf.Pow(TotalDistance, 10));
        var colorG = Mathf.Clamp01(Mathf.Pow(PositionalDistance, 10));
        var colorB = Mathf.Clamp01(Mathf.Pow(RotationalDistance / 10, 10));
        _material.SetColor(Color, new Color(colorR, 1 - colorG, 1 - colorB));
    }
}