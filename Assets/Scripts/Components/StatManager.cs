using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class StatData
{
    public string statName;
    public float maxValue;
}

public class StatManager : MonoBehaviour
{
    [SerializeField] private List<StatData> statDataList = new();
    private readonly Dictionary<string, float> statValues = new();

    private void Awake()
    {
        InitializeStats();
    }

    private void InitializeStats()
    {
        foreach (var data in statDataList)
        {
            statValues[data.statName] = data.maxValue;
        }
    }

    public void ModifyStat(string statName, float amount)
    {
        if (statValues.ContainsKey(statName))
        {
            statValues[statName] += amount;
            ClampStat(statName);
        }
        else
        {
            LogStat(statName);
        }
    }

    public void SetStat(string statName, float value)
    {
        if (statValues.ContainsKey(statName))
        {
            statValues[statName] = value;
            ClampStat(statName);
        }
        else
        {
            LogStat(statName);
        }
    }

    public float GetStat(string statName)
    {
        if (statValues.ContainsKey(statName))
        {
            return statValues[statName];
        }
        else
        {
            LogStat(statName);
            return 0;
        }
    }

    public float GetMaxStat(string statName)
    {
        foreach (var data in statDataList)
        {
            if (data.statName == statName)
            {
                return data.maxValue;
            }
        }
        LogStat(statName);
        return 0;
    }

    private void ClampStat(string statName)
    {
        if (statValues.ContainsKey(statName))
        {
            float maxValue = GetMaxStat(statName);
            statValues[statName] = Mathf.Clamp(statValues[statName], 0f, maxValue);
        }
    }

    private void LogStat(string statName)
    {
        Debug.LogWarning("Stat with name " + statName + " does not exist.");
    }
}
