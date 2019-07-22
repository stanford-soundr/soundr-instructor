using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TMPro;
using UnityEngine;

[Serializable]
public enum InstructorState
{
    Start,
    CreateGround,
    CreateWall,
    CreateMicrophone,
    CreateWidth,
    CreateDepth,
    MeasureHeight,
    DoExperiment,
    End
}

[Serializable]
public struct VerticalWallParam
{
    public Vector3 Point1;
    public Vector3 Point2;
}

public class MasterInstructor: MonoBehaviour
{
    public float SnapGrid;
        
    public VerticalGridGenerator VerticalWallTemplate;
    public GroundGridGenerator GroundTemplate;
    public GameObject MicrophoneTemplate;
    public Transform LeftControllerAnchor;
    public Transform RightControllerAnchor;
    public Transform HeadsetAnchor;
    public ExperimentManager ExperimentManager;
    public Transform Guider;
    public Transform HeadGuider;
    public TextMeshProUGUI GuiderText;
    public TextMeshProUGUI MessageText;
    public ConeManager ConeManager;

    public int UserId;
    public int TrialId;
    // ReSharper disable once InconsistentNaming
    public InstructorState _state;
    // ReSharper disable once MemberCanBePrivate.Global
    public InstructorState State
    {
        get => _state;
        set
        {
            if (_state != value)
            {
                FinishState(_state);
                StartState(value);
            }
            _state = value;
        }
    }
        
    public int ExperimentCount;

    public float GroundParam;
    public GroundGridGenerator Ground;

    public VerticalWallParam WallParam;
    public VerticalGridGenerator Wall;

    public Vector3 MicrophonePosition;
    public Transform Microphone;

    public Vector2 RoomSize;
    public VerticalGridGenerator LeftWall;
    public VerticalGridGenerator RightWall;
    public VerticalGridGenerator BackWall;

    public float PersonHeightParam;

    private void FinishState(InstructorState finishedState)
    {
        switch (finishedState)
        {
            case InstructorState.Start:
                break;
            case InstructorState.CreateGround:
                break;
            case InstructorState.CreateWall:
                break;
            case InstructorState.CreateMicrophone:
                break;
            case InstructorState.CreateWidth:
                break;
            case InstructorState.CreateDepth:
                break;
            case InstructorState.MeasureHeight:
                break;
            case InstructorState.DoExperiment:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(finishedState), finishedState, null);
        }
    }

    private void StartState(InstructorState startingState)
    {
        switch (startingState)
        {
            case InstructorState.Start:
                foreach (Transform child in transform) {
                    Destroy(child.gameObject);
                }
                break;
            case InstructorState.CreateGround:
                Ground = Instantiate(GroundTemplate, transform);
                MessageText.text =
                    "Please use the controller ring to touch the ground and click A or X on the controller.";
                break;
            case InstructorState.CreateWall:
                Wall = Instantiate(VerticalWallTemplate, transform);
                MessageText.text =
                    "Please use both controllers to create the wall for the microphone " +
                    "and click A or X on the controller.";
                break;
            case InstructorState.CreateMicrophone:
                var rotation = RotationFromWallParam();
                Microphone = Instantiate(MicrophoneTemplate, Vector3.zero, rotation, transform).transform;
                MessageText.text =
                    "Please use the right controller ring to touch the position of the microphone " +
                    "and click A or X on the controller.";
                break;
            case InstructorState.CreateWidth:
                Ground.transform.position = Vector3.zero;
                RoomSize = new Vector2(2, 2);
                LeftWall = Instantiate(VerticalWallTemplate, transform);
                RightWall = Instantiate(VerticalWallTemplate, transform);
                BackWall = Instantiate(VerticalWallTemplate, transform);
                break;
            case InstructorState.CreateDepth:
                break;
            case InstructorState.MeasureHeight:
                MessageText.text =
                    "Measuring you height... Please stand up right and keep still.";
                break;
            case InstructorState.DoExperiment:
                MessageText.text = "";
                var roomSetting = new ExperimentManager.Setting
                {
                    RoomWidth = RoomSize.x,
                    RoomDepth = RoomSize.y,
                    PersonHeight = PersonHeightParam,
                    UserId = UserId,
                    TrialId = TrialId,
                };
                ExperimentManager.GenerateExperiment(roomSetting);
                LoadExperiment();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(startingState), startingState, null);
        }
    }

    private void UpdateWallWithParam()
    {
        var point1 = WallParam.Point1;
        var point2 = WallParam.Point2;
        point1.y = GroundParam;
        point2.y = GroundParam + 2;
        var pointDiff = point1 - point2;
        pointDiff.y = 0;
        point1 += pointDiff;
        point2 -= pointDiff;
        Wall.Vertices[0] = point1;
        Wall.Vertices[3] = point2;
    }
        
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private Quaternion RotationFromWallParam()
    {
        var v1 = WallParam.Point1;
        var v2 = WallParam.Point2;
        var v1v2 = v2 - v1;
        var rotation = Quaternion.AngleAxis(180 - Mathf.Atan2(v1v2.z, v1v2.x) / Mathf.PI * 180, Vector3.up);
        return rotation;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private void UpdateMicrophonePosition()
    {
        var v3 = RightControllerAnchor.position;
        var v1 = WallParam.Point1;
        var v2 = WallParam.Point2;
        v1.y = v3.y;
        v2.y = v3.y;
        var u_v1v2 = (v2 - v1).normalized;
        var v1v3 = v3 - v1;
        var d_v1v4 = Vector3.Dot(v1v3, u_v1v2);
        var v1v4 = d_v1v4 * u_v1v2;
        var v4 = v1 + v1v4;
        v4.y = SnapToGrid(v4.y);
        MicrophonePosition = v4;
    }

    private float SnapToGrid(float input)
    {
        var output = input;
        if (SnapGrid > Mathf.Epsilon)
        {
            var count = Mathf.Round(input / SnapGrid);
            output = count * SnapGrid;
        }
        return output;
    }

    private void UpdateRoomWidth()
    {
        var anchorPositions = new[] {LeftControllerAnchor.position, RightControllerAnchor.position};
        var anchorMicrophonePositions = anchorPositions.Select(pos => Microphone.InverseTransformPoint(pos));
        var anchorX = anchorMicrophonePositions.Select(pos => Mathf.Abs(pos.x)).ToArray();
        var maxAnchorX = anchorX[0] > anchorX[1] ? anchorX[0] : anchorX[1];
        var snappedMaxAnchorX = SnapToGrid(maxAnchorX);
        RoomSize.x = snappedMaxAnchorX * 2;
        MessageText.text = $"Room width: {RoomSize.x}m";
    }

    private void UpdateRoomDepth()
    {
        var anchorPositions = new[] {LeftControllerAnchor.position, RightControllerAnchor.position};
        var anchorMicrophonePositions = anchorPositions.Select(pos => Microphone.InverseTransformPoint(pos));
        var anchorZ = anchorMicrophonePositions.Select(pos => Mathf.Abs(pos.z)).ToArray();
        var maxAnchorZ = anchorZ[0] > anchorZ[1] ? anchorZ[0] : anchorZ[1];
        var snappedMaxAnchorZ = SnapToGrid(maxAnchorZ);
        RoomSize.y = snappedMaxAnchorZ;
        MessageText.text = $"Room depth: {RoomSize.y}m";
    }

    private void UpdateWallsPosition()
    {
        var rawPositions = new[]
        {
            new Vector3(RoomSize.x / 2, 2, 0),
            new Vector3(-RoomSize.x / 2, 2, 0),
            new Vector3(RoomSize.x / 2, 2, RoomSize.y),
            new Vector3(-RoomSize.x / 2, 2, RoomSize.y),
            new Vector3(RoomSize.x / 2, 0, 0),
            new Vector3(-RoomSize.x / 2, 0, 0),
            new Vector3(RoomSize.x / 2, 0, RoomSize.y),
            new Vector3(-RoomSize.x / 2, 0, RoomSize.y),
        };
        var positions = rawPositions.Select(pos => Microphone.TransformPoint(pos) +
                                                   Vector3.up * (GroundParam - Microphone.position.y)).ToArray();
        Wall.Vertices[0] = positions[0];
        Wall.Vertices[3] = positions[5];
        RightWall.Vertices[0] = positions[1];
        RightWall.Vertices[3] = positions[7];
        LeftWall.Vertices[0] = positions[2];
        LeftWall.Vertices[3] = positions[4];
        BackWall.Vertices[0] = positions[3];
        BackWall.Vertices[3] = positions[6];
        Ground.Vertices[0] = positions[4];
        Ground.Vertices[1] = positions[6];
        Ground.Vertices[3] = positions[7];
    }

    private void LoadExperiment()
    {
        Guider.gameObject.SetActive(true);
        HeadGuider.gameObject.SetActive(true);
        var experimentStep = ExperimentManager.Steps[ExperimentCount];

        Guider.position = Microphone.TransformPoint(experimentStep.Position) + Vector3.up * (GroundParam - Microphone.position.y);
        Guider.rotation = Microphone.rotation * experimentStep.Direction;
        GuiderText.text = experimentStep.Text;
    }
        
    private void Update()
    {
        switch (State)
        {
            case InstructorState.Start:
                State = InstructorState.CreateGround;
                break;
            case InstructorState.CreateGround:
                var groundAnchor = LeftControllerAnchor.position.y < RightControllerAnchor.position.y ?
                    LeftControllerAnchor : RightControllerAnchor;
                var groundAnchorPosition = groundAnchor.transform.position;
                Ground.transform.position = groundAnchorPosition;
                GroundParam = groundAnchorPosition.y;
                if (OVRInput.GetUp(OVRInput.RawButton.A) || OVRInput.GetUp(OVRInput.RawButton.X))
                {
                    State = InstructorState.CreateWall;
                }
                break;
            case InstructorState.CreateWall:
                WallParam.Point1 = LeftControllerAnchor.position;
                WallParam.Point2 = RightControllerAnchor.position;
                UpdateWallWithParam();
                if (OVRInput.GetUp(OVRInput.RawButton.A) || OVRInput.GetUp(OVRInput.RawButton.X))
                {
                    State = InstructorState.CreateMicrophone;
                }
                break;
            case InstructorState.CreateMicrophone:
                UpdateMicrophonePosition();
                Microphone.position = MicrophonePosition;
                if (OVRInput.GetUp(OVRInput.RawButton.A) || OVRInput.GetUp(OVRInput.RawButton.X))
                {
                    State = InstructorState.CreateWidth;
                }
                break;
            case InstructorState.CreateWidth:
                UpdateRoomWidth();
                UpdateWallsPosition();
                if (OVRInput.GetUp(OVRInput.RawButton.A) || OVRInput.GetUp(OVRInput.RawButton.X))
                {
                    State = InstructorState.CreateDepth;
                }
                break;
            case InstructorState.CreateDepth:
                UpdateRoomDepth();
                UpdateWallsPosition();
                if (OVRInput.GetUp(OVRInput.RawButton.A) || OVRInput.GetUp(OVRInput.RawButton.X))
                {
                    State = InstructorState.MeasureHeight;
                }
                break;
            case InstructorState.MeasureHeight:
                PersonHeightParam = HeadsetAnchor.position.y - GroundParam;
                if (OVRInput.GetUp(OVRInput.RawButton.A) || OVRInput.GetUp(OVRInput.RawButton.X))
                {
                    State = InstructorState.DoExperiment;
                }
                break;
            case InstructorState.DoExperiment:
                if (ConeManager.PositionalValue > 0.5)
                {
                    MessageText.text = "Please move your head closer to the start of the cone.";
                }
                else if (ConeManager.RotationalValue > 0.5)
                {
                    MessageText.text = "Please rotate your head closer to align with the cone.";
                }
                else
                {
                    MessageText.text = "";
                }
                if (OVRInput.GetUp(OVRInput.RawButton.A) || OVRInput.GetUp(OVRInput.RawButton.X))
                {
                    ExperimentCount += 1;
                    if (ExperimentCount < ExperimentManager.Steps.Length)
                    {
                        LoadExperiment();
                    }
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}