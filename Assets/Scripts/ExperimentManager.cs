using System;
using System.Collections.Generic;
using System.Linq;
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
    
    static private void shuffle<T>(T[] array)
    {
        for (var i = 0; i < array.Length; i++ )
        {
            var tmp = array[i];
            var r = Random.Range(i, array.Length);
            array[i] = array[r];
            array[r] = tmp;
        }
    }

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
                var directionChoices = Enumerable.Range(0, DirectionCount).ToArray();
                shuffle(directionChoices);
                foreach (var directionBracket in directionChoices)
                {
                    var directionBracketStart = directionBracketSize * directionBracket;
                    
                    var x = widthBracketStart + widthBracketSize * Random.value + WidthClearance / 2f
                            - _setting.RoomWidth / 2f;
                    var z = depthBracketStart + depthBracketSize * Random.value + DepthClearance / 2f;
                    var y = _setting.PersonHeight - 0.05f;
                    var position = new Vector3(x, y, z);

                    var rY = directionBracketStart + directionBracketSize * Random.value;
                    var rX = -60 + 120 * Random.value;
                    const float rZ = 0f;
                    var direction = Quaternion.Euler(rX, rY, rZ);
                    
                    steps.Add(new Step{
                        Position = position, 
                        Direction = direction, 
                        Text = ""
                    });
                }
            }
        }

        var rearrangedSteps = new List<Step>();

        const int fold = 4;

        for (var i = 0; i < fold; i++)
        {
            for (var j = i; j < steps.Count; j += fold)
            {
                var position = steps[j].Position;
                var direction = steps[j].Direction;
                rearrangedSteps.Add(new Step{
                    Position = position, 
                    Direction = direction, 
                    Text = ReadText.Scripts[Random.Range(0, ReadText.Scripts.Length)]
                });
                rearrangedSteps.Add(new Step{
                    Position = position, 
                    Direction = direction, 
                    Text = ReadText.Scripts[Random.Range(0, ReadText.Scripts.Length)]
                });
                rearrangedSteps.Add(new Step{
                    Position = position, 
                    Direction = direction, 
                    Text = ReadText.Scripts[Random.Range(0, ReadText.Scripts.Length)]
                });
                rearrangedSteps.Add(new Step{
                    Position = position, 
                    Direction = direction, 
                    Text = ReadText.Scripts[Random.Range(0, ReadText.Scripts.Length)]
                });
            }
        }

        Steps = rearrangedSteps.ToArray();
    }
}