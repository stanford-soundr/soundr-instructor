using System.Collections.Generic;
using UnityEngine;

public class DataRecorder: MonoBehaviour
{
    public bool Recording;

    public List<double> HistoryTimestamps = new List<double>();
    public List<Vector3> HistoryPositions = new List<Vector3>();
    public List<Quaternion> HistoryQuaternions = new List<Quaternion>();

    public double CurrentTimestamp;
    public Vector3 CurrentPosition;
    public Quaternion CurrentQuaternion;
    
    public AudioClip MicrophoneClip;
    public int LastPosition;
    
    public MasterInstructor MasterInstructor;

    public void StartRecording()
    {
        Recording = true;
        MicrophoneClip = Microphone.Start(null, true, 100, 48000);
        while(Microphone.GetPosition(null) < 0) {}
        LastPosition = 0;
        HistoryTimestamps.Clear();
        HistoryPositions.Clear();
        HistoryQuaternions.Clear();
    }

    public void Update()
    {
        CurrentTimestamp = InstructorUtils.CurrentTimestamp();
        CurrentPosition = MasterInstructor.OriginAnchor.InverseTransformPoint(MasterInstructor.HeadsetAnchor.position);
        CurrentQuaternion = Quaternion.Inverse(MasterInstructor.OriginAnchor.rotation) * 
                            MasterInstructor.HeadsetAnchor.rotation;
        
        HistoryTimestamps.Add(CurrentTimestamp);
        HistoryPositions.Add(CurrentPosition);
        HistoryQuaternions.Add(CurrentQuaternion);
        
        if (Recording && HistoryTimestamps.Count == 72)
        {
            SendData();
        }
    }

    private void SendData()
    {
        var trackingMessage = DecodedMessage.TrackingDataMessage(
            HistoryTimestamps.ToArray(),
            HistoryPositions.ToArray(),
            HistoryQuaternions.ToArray());
        var newPosition = Microphone.GetPosition(null);
        var positionDiff = newPosition - LastPosition;
        Debug.Log($"Audio Length: {positionDiff}");
        if (positionDiff < 0)
        {
            positionDiff += MicrophoneClip.samples;
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            Debug.Log("Clip overwritten!");
        }
        if (positionDiff > 0)
        {
            var samples = new float[positionDiff * MicrophoneClip.channels];
            MicrophoneClip.GetData(samples, LastPosition);
            var audioMessage = DecodedMessage.AudioDataMessage(samples);
            MasterInstructor.InstructorServer.SendMessageToClient(audioMessage);
        }
        LastPosition = newPosition;
        MasterInstructor.InstructorServer.SendMessageToClient(trackingMessage);
        HistoryTimestamps.Clear();
        HistoryPositions.Clear();
        HistoryQuaternions.Clear();
    }
    
    public void StopRecording()
    {
        Recording = false;
        SendData();
        Microphone.End(null);
    }
}