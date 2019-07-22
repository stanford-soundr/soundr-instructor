using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ExperimentManager: MonoBehaviour
{
    public struct Setting
    {
        public float PersonHeight;
        public float RoomWidth;
        public float RoomDepth;
        public int UserId;
        public int TrialId;
    }

    [Serializable]
    public struct Step
    {
        public Vector3 Position;
        public Quaternion Direction;
        public string Text;
    }
    
    private Setting _setting;

    public Step[] Steps;
    
    private const int WidthBracketCount = 8;
    private const int DepthBracketCount = 8;
    private const int DirectionCount = 4;

    private const float WidthClearance = 0.5f;
    private const float DepthClearance = 0.5f;

    public void GenerateExperiment(Setting setting)
    {
        Random.InitState(setting.TrialId * 100 + setting.UserId);
        _setting = setting;
        
        var widthBracketSize = (_setting.RoomWidth - WidthClearance) / WidthBracketCount;
        var depthBracketSize = (_setting.RoomDepth - DepthClearance) / DepthBracketCount;
        const float directionBracketSize = 360f / DirectionCount;

        var steps = new List<Step>();
        for (var widthBracket = 0; widthBracket < WidthBracketCount; widthBracket++)
        {
            var widthBracketStart = widthBracketSize * widthBracket;
            for (var depthBracket = 0; depthBracket < DepthBracketCount; depthBracket++)
            {
                var depthBracketStart = depthBracketSize * depthBracket;
                for (var directionBracket = 0; directionBracket < DirectionCount; directionBracket++)
                {
                    var directionBracketStart = directionBracketSize * directionBracket;
                    
                    var x = widthBracketStart + widthBracketSize * Random.value + WidthClearance / 2f;
                    var z = depthBracketStart + depthBracketSize * Random.value + DepthClearance / 2f;
                    var y = _setting.PersonHeight - 0.05f;
                    var position = new Vector3(x, y, z);

                    var rY = directionBracketStart + directionBracketSize * Random.value;
                    var rX = -60 + 120 * Random.value;
                    const float rZ = 0f;
                    var direction = Quaternion.Euler(rX, rY, rZ);

                    var text = ReadText.Scripts[Random.Range(0, ReadText.Scripts.Length)];
                    
                    steps.Add(new Step{Position = position, Direction = direction, Text = text});
                }
            }
        }

        var rearrangedSteps = new List<Step>();

        const int fold = 4;

        for (var i = 0; i < fold; i++)
        {
            for (var j = i; j < steps.Count; j += fold)
            {
                rearrangedSteps.Add(steps[j]);
            }
        }

        Steps = rearrangedSteps.ToArray();
    }
}