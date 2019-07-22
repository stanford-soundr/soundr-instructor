using TMPro;
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
    
    public float PositionalValue;
    public float RotationalValue;

    public TextMeshProUGUI ReadingText;

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
        RotationalDistance = Mathf.Min(angle, 360 - angle) / 10;

        PositionalValue = PositionalDistance * 5f;
        RotationalValue = Mathf.Clamp01(Mathf.Pow(RotationalDistance, 10));
        ReadingText.enabled = PositionalValue < 0.5 && RotationalValue < 0.5;
        var totalValue = (PositionalValue + RotationalValue) / 2;
        
        var colorR = totalValue;
        var colorG = PositionalValue;
        var colorB = RotationalValue;
        _material.SetColor(Color, new Color(colorR, 1 - colorG, 1 - colorB));
    }
}