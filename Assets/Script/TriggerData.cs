/****************************************************
    文件：TriggerData.cs
	作者：www34
    邮箱: 
    日期：2026/1/21 14:35:49
	功能：Nothing
*****************************************************/

using System;
using UnityEngine;

public class TriggerData : MonoBehaviour
{
	public MapMgr mapMgr;
	public int triggerWave;
	public void OnTriggerExit(Collider other)
	{
		if(!other.CompareTag("Player")) return;
		if(mapMgr==null) return;
		mapMgr.TriggerMonsterBorn(this,triggerWave);
	}
}