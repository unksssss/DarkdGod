/********************************************************************
    created:	2025/10/21
    created:	21:10:2025   22:35
    filename: 	E:\unityproject\DarkGod\Assets\Script\Common\BaseData.cs
    file path:	E:\unityproject\DarkGod\Assets\Script\Common
    file base:	BaseData
    file ext:	cs
    author:		Author

    purpose:	数据配置类
*********************************************************************/

using System.Collections.Generic;
using UnityEngine;
    
public class AutoGuideCfg:BaseData<AutoGuideCfg>
{
    //<npcID></npcID>
    //<dilogArr></dilogArr>
    //<actID></actID>
    //<coin></coin>
    //<exp></exp>
    public int npcID;//触发任务目标npc索引号
    public string dialogArr;
    public int actID;
    public int coin;
    public int exp;
}
public class StrongCfg:BaseData<StrongCfg>
{
    //< pos />
    //< starlv />
    //< addhp />
    //< addhurt />
    //< adddef />
    //< minlv />
    //< coin />
    //< crystal />
    public int pos;
    public int starlv;
    public int addhp;
    public int addhurt;
    public int adddef;
    public int minlv;
    public int coin; 
    public int crystal;
}
public class MapCfg : BaseData<MapCfg>
{
    public string MapName;
    public string SceneName;
    public int power;
    public Vector3 mainCameraPos;
    public Vector3 mainCameraRotation;
    public Vector3 playerBornPos;
    public Vector3 playerBornRotation;
    
    public List<MonsterData> monsterLst;
    
    public int coin;
    public int crystal;
    public int exp;
}

public class TaskRewardCfg : BaseData<TaskRewardCfg>
{
    public string taskname;
    public int count;
    public int exp;
    public int coin;
}

public class TaskRewardData : BaseData<TaskRewardData>
{
    public int prgs;
    public bool istaked;
}

public class SkillCfg : BaseData<SkillCfg>
{
    public string skillName;
    public int cdTime;
    public int aniTime;
    public int skillAction;
    public string fx;
    public bool isCombo;
    public bool isCollide;
    public bool isBreak;
    
    public DamageType damageType;
    public List<int> skillMoveList;
    public List<int> skillActionList;
    public List<int> skillDamageList;
}

public enum DamageType
{
    AD=1,
    AP=2
}
public class SkillMoveCfg : BaseData<SkillMoveCfg>
{
    public int delayTime;
    public int moveTime;
    public float moveDis;

}

public class SkillActionCfg : BaseData<SkillActionCfg>
{
    public int delayTime;
    public float radius;
    public int angle;
}
public class MonsterCfg : BaseData<MonsterCfg>
{
    public string mName;
    public MonsterType mType;
    public bool canStop;
    public string resPath;
    public int skillID;
    public float atkDis;
    public BattleProps battleProps;
}

public class MonsterData : BaseData<MonsterData>
{
    public int waveID;//批次
    public int monsterIndex;//序号
    public MonsterCfg Cfg;
    public Vector3 mBornPos;
    public Vector3 mBornRot;
    public int level;
}
public class BaseData<T>
{
    public int ID;
}

public class BattleProps
{
    
    public int hp;
    public int ad;
    public int ap;
    public int addef;
    public int apdef;
    public int dodge;
    public int pierce;
    public int critical;
}