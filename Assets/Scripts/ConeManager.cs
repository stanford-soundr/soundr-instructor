using TMPro;
using UnityEngine;

public class ConeManager: MonoBehaviour
{
    // ReSharper disable once InconsistentNaming
    public Material _material;

    public Transform HeadTransform;

    public float PositionalDistance;
    public float RotationalDistance;

    private static readonly int Color = Shader.PropertyToID("_GridColor");
    
    public float PositionalValue;
    public float RotationalValue;

    public TextMeshProUGUI ReadingText;

    public LineRenderer GuideLine;

    private void Start()
    {
        _material = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        var headPosition = HeadTransform.position;
        var headRotation = HeadTransform.rotation;

        var position = transform.position;
        var positionalDifference = (position - headPosition);
        positionalDifference.y = 0;
        PositionalDistance = positionalDifference.magnitude;
        (transform.rotation * Quaternion.Inverse(headRotation)).ToAngleAxis(out var angle, out _);
        RotationalDistance = Mathf.Min(angle, 360 - angle) / 10;

        PositionalValue = PositionalDistance * 5f;
        RotationalValue = Mathf.Clamp01(Mathf.Pow(RotationalDistance, 10));
        var closeEnough = PositionalValue < 0.5 && RotationalValue < 0.5;
        var totalValue = (PositionalValue + RotationalValue) / 2;
        
        ReadingText.enabled = closeEnough;
        GuideLine.positionCount = 2;
        GuideLine.SetPosition(0, headPosition + headRotation * (0.2f * Vector3.forward));
        GuideLine.SetPosition(1, position);
        GuideLine.enabled = !closeEnough;
        
        var colorR = totalValue;
        var colorG = PositionalValue;
        var colorB = RotationalValue;
        _material.SetColor(Color, new Color(colorR, 1 - colorG, 1 - colorB));
    }
}