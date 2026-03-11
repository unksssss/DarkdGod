// time:2025/11/19/16
// path:E:\unityproject\DarkGod\Assets\Script\Battle\MapMgr.csMapMgr.cs
// func:
// www34


using System.CodeDom;
using UnityEngine;

public class MapMgr : MonoBehaviour
{
    private BattleMgr battleMgr;
    public TriggerData[]  triggerDataLst;
    private int waveID = 1;
    public void Init(BattleMgr battle)
    {
        battleMgr = battle;
        battleMgr.LoadMonsterByWaveID(waveID);
        PECommon.Log("MapMgr Init.....");
    }


    public void TriggerMonsterBorn(TriggerData trigger,int triggerWave)
    {
        if(battleMgr==null) return;
        BoxCollider co = trigger.GetComponent<BoxCollider>();
        co.isTrigger = false;
        battleMgr.LoadMonsterByWaveID(triggerWave);
        battleMgr.ActiveCurrentMonsterWave();
        battleMgr.triggerCheck = true;
    }

    public bool SetNextTriggerOn()
    {
        waveID++;
        foreach (var triggerData in triggerDataLst)
        {
            if (triggerData.triggerWave == waveID)
            {
                BoxCollider co = triggerData.GetComponent<BoxCollider>();
                co.isTrigger = true;
                return true;
            }
        }
        return false;
    }
}