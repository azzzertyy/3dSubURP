using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class StatRegenData
{
    public string statName;
    public float regenDelay;
    public float regenAmount;
}

public class StatRegen : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StatManager statManager;
    [Header("Data Fields")]
    [SerializeField] private List<StatRegenData> statRegenData;

    private Dictionary<string, float> regenTimers = new Dictionary<string, float>();

    private void Start()
    {
        foreach (var data in statRegenData)
        {
            regenTimers[data.statName] = 0f;
            StartCoroutine(HandleRegen(data));
        }
    }

    private IEnumerator HandleRegen(StatRegenData data)
    {
        while (true)
        {
            yield return new WaitForSeconds(data.regenDelay);

            string stat = data.statName;
            if (statManager.GetStat(stat) + data.regenAmount > statManager.GetMaxStat(stat))
            {
                statManager.SetStat(stat, statManager.GetMaxStat(stat));
            }
            else
            {
                statManager.ModifyStat(stat, data.regenAmount);
            }
        }
    }
}
