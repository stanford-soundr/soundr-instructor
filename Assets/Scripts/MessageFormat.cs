using UnityEngine;

public enum MessageType
{
    Parameter,
    Acknowledge,
    TrackingData,
    AudioData,
    Stop,
    Error
}

public class RawMessage
{
    public string MessageType;
    public string Content;

    public static RawMessage FromJsonString(string jsonString)
    {
        return JsonUtility.FromJson<RawMessage>(jsonString);
    }

    public string ToJsonString()
    {
        return JsonUtility.ToJson(this);
    }
}

public class DecodedMessage
{
    public MessageType MessageType;
    public object Content;

    public static DecodedMessage FromRawMessage(RawMessage rawMessage)
    {
        MessageType messageType;
        object content;
        switch (rawMessage.MessageType)
        {
            case "PARAMETER":
                messageType = MessageType.Parameter;
                content = JsonUtility.FromJson<ParameterContent>(rawMessage.Content);
                break;
            case "ACKNOWLEDGE":
                messageType = MessageType.Acknowledge;
                content = JsonUtility.FromJson<AcknowledgeContent>(rawMessage.Content);
                break;
            case "TRACKING_DATA":
                messageType = MessageType.TrackingData;
                content = JsonUtility.FromJson<TrackingDataContent>(rawMessage.Content);
                break;
            case "AUDIO_DATA":
                messageType = MessageType.AudioData;
                content = JsonUtility.FromJson<AudioDataContent>(rawMessage.Content);
                break;
            case "STOP":
                messageType = MessageType.Stop;
                content = JsonUtility.FromJson<StopContent>(rawMessage.Content);
                break;
            case "ERROR":
                messageType = MessageType.Error;
                content = JsonUtility.FromJson<ErrorContent>(rawMessage.Content);
                break;
            default:
                messageType = MessageType.Error;
                content = ErrorContent.DecodeError;
                break;
        }

        return new DecodedMessage {MessageType = messageType, Content = content};
    }

    public static DecodedMessage FromJsonString(string jsonString)
    {
        return FromRawMessage(RawMessage.FromJsonString(jsonString));
    }

    public RawMessage ToRawMessage()
    {
        string messageType;
        string content;
        switch (MessageType)
        {
            case MessageType.Parameter:
                messageType = "PARAMETER";
                content = JsonUtility.ToJson(Content);
                break;
            case MessageType.Acknowledge:
                messageType = "ACKNOWLEDGE";
                content = JsonUtility.ToJson(Content);
                break;
            case MessageType.TrackingData:
                messageType = "TRACKING_DATA";
                content = JsonUtility.ToJson(Content);
                break;
            case MessageType.AudioData:
                messageType = "AUDIO_DATA";
                content = JsonUtility.ToJson(Content);
                break;
            case MessageType.Stop:
                messageType = "STOP";
                content = JsonUtility.ToJson(Content);
                break;
            case MessageType.Error:
                messageType = "ERROR";
                content = JsonUtility.ToJson(Content);
                break;
            default:
                messageType = "ERROR";
                content = JsonUtility.ToJson(ErrorContent.DecodeError);
                break;
        }

        return new RawMessage {MessageType = messageType, Content = content};
    }

    public string ToJsonString()
    {
        return ToRawMessage().ToJsonString();
    }

    public static DecodedMessage ErrorMessage(MessageError error)
    {
        const MessageType messageType = MessageType.Error;
        var content = new ErrorContent {ErrorCode = (int) error};
        return new DecodedMessage {MessageType = messageType, Content = content};
    }

    public static DecodedMessage AcknowledgeMessage()
    {
        const MessageType messageType = MessageType.Acknowledge;
        var content = new AcknowledgeContent {Timestamp = InstructorUtils.CurrentTimestamp()};
        return new DecodedMessage {MessageType = messageType, Content = content};
    }

    public static DecodedMessage TrackingDataMessage(double[] timestamps, Vector3[] positions, Quaternion[] quaternions)
    {
        const MessageType messageType = MessageType.TrackingData;
        var content = new TrackingDataContent
        {
            Timestamp = InstructorUtils.CurrentTimestamp(),
            DataTimeStamps = timestamps,
            DataPositions = positions,
            DataQuaternions = quaternions
        };
        return new DecodedMessage {MessageType = messageType, Content = content};
    }

    public static DecodedMessage AudioDataMessage(float[] audio)
    {
        const MessageType messageType = MessageType.AudioData;
        var content = new AudioDataContent
        {
            Timestamp = InstructorUtils.CurrentTimestamp(),
            DataAudio = audio
        };
        return new DecodedMessage {MessageType = messageType, Content = content};
    }

    public static DecodedMessage StopMessage(double timestamp)
    {
        const MessageType messageType = MessageType.Stop;
        var content = new StopContent
        {
            Timestamp = timestamp
        };
        return new DecodedMessage {MessageType = messageType, Content = content};
    }
}

public class ParameterContent
{
    public int UserId;
    public int TrialId;
}

public class AcknowledgeContent
{
    public double Timestamp;
}

public class TrackingDataContent
{
    public double Timestamp;
    public double[] DataTimeStamps;
    public Vector3[] DataPositions;
    public Quaternion[] DataQuaternions;
}

public class AudioDataContent
{
    public double Timestamp;
    public float[] DataAudio;
}

public class StopContent
{
    public double Timestamp;
    public bool Internal => Timestamp < 0;
}

public enum MessageError
{
    DecodeError = 0,
    NotReadyError = 1,
    ParseError = 2
}

public class ErrorContent
{
    public int ErrorCode;

    public static readonly ErrorContent DecodeError = new ErrorContent {ErrorCode = (int) MessageError.DecodeError};
}