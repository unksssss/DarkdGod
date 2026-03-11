// time:2025/11/19/16
// path:E:\unityproject\DarkGod\Assets\Script\Battle\BattleMgr.csBattleMgr.cs
// func: 战斗管理器
// www34

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using NUnit.Framework.Constraints;
using PEProtocol;
using UnityEngine;

public class BattleMgr : MonoBehaviour
{
    private ResSvc resSvc=null;
    private AudioSvc audioSvc=null;
    
    
    private StateMgr stateMgr;
    private SkillMgr skillMgr;
    private MapMgr mapMgr;
    public PlayerEntity entitySelfPlayer;
    private MapCfg mapData;
    private Dictionary<string,MonsterEntity> monsterEntityDic=new Dictionary<string,MonsterEntity>();
    public Vector2 GetCurrentDir()
    {
        return BattleSys.Instance.GetCurrentDir();
    }

    public bool triggerCheck = true;
    
    public void Init(int mapid,Action cb=null)
    {
        resSvc=ResSvc.Instance;
        audioSvc=AudioSvc.instance;
        
        
        
        stateMgr =gameObject.AddComponent<StateMgr>();
        skillMgr = gameObject.AddComponent<SkillMgr>();
        //初始化各管理器
        stateMgr.Init();
        skillMgr.Init();
        //加载战场地图
        mapData=resSvc.GetMapCfg(mapid);
        resSvc.AsyncLoadScene(mapData.SceneName,()=>
        {
            //todo 初始化地图资源
            GameObject map=GameObject.FindGameObjectWithTag("MapRoot");
            mapMgr=map.GetComponent<MapMgr>();
            mapMgr.Init(this); 
            map.transform.localPosition=new Vector3(0,0,0);
            map.transform.localScale=new Vector3(1,1,1);
           
            Camera.main.transform.localPosition = mapData.mainCameraPos;
            Camera.main.transform.localEulerAngles = mapData.mainCameraRotation;
            

            LoadPlayer(mapData);
            entitySelfPlayer.currentAniState = AniState.Idle;
            
            ActiveCurrentMonsterWave();
            audioSvc.PlayBGMuic(Constants.BGHuangYe);
            if (cb != null)
            {
                cb();
            }
        });
    }
    public bool isPauseGame = false;
    public void Update()
    {
        foreach (var item in monsterEntityDic)
        {
            MonsterEntity monsterEntity=item.Value;
            monsterEntity.TickAILogic();
            
        }

        if (mapMgr != null)
        {
            if (triggerCheck&&monsterEntityDic.Count == 0)
            {
                bool isExist=mapMgr.SetNextTriggerOn();
                triggerCheck = false;
                if (!isExist)
                {
                    //todo 关卡结束，战斗胜利
                    EndBattle(true,entitySelfPlayer.Hp);
                }
            }
        }
    }

    public void EndBattle(bool isWin,int restHP)
    {
        audioSvc.StopBGMusic();
        BattleSys.Instance.EndBattle(isWin, restHP);
        isPauseGame = true;
    }
    private void LoadPlayer(MapCfg mapData)
    {
        GameObject player = resSvc.LoadPrefab(PathDefine.AssassinBattlePlayerPrefab);
        player.transform.position = mapData.playerBornPos;
        player.transform.localEulerAngles = mapData.playerBornRotation;
        player.transform.localScale=new Vector3(1,1,1);
        var playerdata = GameRoot.Instance.PlayerData;
        var battleProps=new BattleProps
        {
            hp=playerdata.hp,
            ad = playerdata.ad,
            ap = playerdata.ap,
            addef = playerdata.addef,
            apdef = playerdata.apdef,
            critical = playerdata.critical,
            dodge = playerdata.dodge,
            pierce = playerdata.pierce
        };
        
        entitySelfPlayer=new PlayerEntity
        {
            stateMgr = stateMgr,
            skillMgr=skillMgr,
            battleMgr = this,
            //todo
            Name = "Player"
        };
        entitySelfPlayer.SetBattleProps(battleProps);
        PlayerController  playerController=player.GetComponent<PlayerController>();
        playerController.Init();
        entitySelfPlayer.SetController(playerController);
    }
    
    public void LoadMonsterByWaveID(int waveID)
    {
        for (int i = 0; i < mapData.monsterLst.Count; i++)
        {
            MonsterData monsterData = mapData.monsterLst[i];
            if (waveID == monsterData.waveID)
            {
                GameObject m = resSvc.LoadPrefab(monsterData.Cfg.resPath, true);
                m.transform.localPosition = monsterData.mBornPos;
                m.transform.localEulerAngles = monsterData.mBornRot;
                m.transform.localScale=Vector3.one;
                m.name="Wave"+monsterData.waveID+"_"+monsterData.monsterIndex;
                MonsterEntity monsterEntity=new MonsterEntity
                {
                    stateMgr = stateMgr,
                    skillMgr=skillMgr,
                    battleMgr = this,
                    
                };
                //初始属性
                monsterEntity.md=monsterData;
                monsterEntity.Name = m.name;
                //更新属性
                monsterEntity.SetBattleProps(monsterData.Cfg.battleProps);
                monsterEntityDic.Add(m.name, monsterEntity);
                if (monsterEntity.md.Cfg.mType == MonsterType.None)
                {
                    Transform hpRoot = m.transform.Find("hpRoot");
                    GameRoot.Instance.dynamicWnd.AddHPItemInfo(m.name,hpRoot,monsterEntity.Hp);
                    
                }
                else if (monsterEntity.md.Cfg.mType==MonsterType.Boss)
                {
                    BattleSys.Instance.playerControlWnd.SetBossHpBarState(true);
                }
                MonsterController monsterController=m.GetComponent<MonsterController>();
                monsterController.Init();
                monsterEntity.SetController(monsterController);
                m.SetActive(false);
            }
        }
    }

    public void ActiveCurrentMonsterWave()
    {
        TimerSvc.Instance.AddTimeTask(i =>
        {
            foreach (var item in monsterEntityDic)
            {
                item.Value.SetActive(true);
                item.Value.Born();
                TimerSvc.Instance.AddTimeTask(i1 =>
                {
                    //出生一秒后进入idle状态
                    item.Value.Idle();
                },1000);
            }
        }, 500);
    }
    public List<MonsterEntity> GetMonsterEntityList()
    {
        List<MonsterEntity> list=new List<MonsterEntity>();
        foreach (var item in monsterEntityDic)
        {
            list.Add(item.Value);
        }
        return list;
    }

    public void RmvMonster(string mName)
    {
        MonsterEntity me;
        if (monsterEntityDic.TryGetValue(mName, out me))
        {
            monsterEntityDic.Remove(mName);
            GameRoot.Instance.dynamicWnd.RmvHPItemInfo(mName);
        }
    }
    #region 技能释放与角色控制相关
    public void SetSelfPlayerMoveDir(Vector2 dir)
    {
        //设置玩家移动
        if(!entitySelfPlayer.canControl)
            return;
        
        if (dir == Vector2.zero)
        {
            entitySelfPlayer.Idle();
            entitySelfPlayer.SetDir(Vector2.zero);
        }
        else
        {
            entitySelfPlayer.Move();
            entitySelfPlayer.SetDir(dir);
        }
    }

    public void ReqReleaseSkill(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.NormalAttack:
                ReleaseNormalAttack();
                break;
            case SkillType.Skill1:
                ReleaseSkill1();
                break;
            case SkillType.Skill2:
                ReleaseSkill2();
                break;
            case SkillType.Skill3:
                ReleaseSkill3();
                break;
            default:
                throw new ArgumentOutOfRangeException("skillType", skillType, null);
        }
    }

    public double lastTime=0;
    public int[] comboArr = new int[] { 111, 112, 113, 114, 115 };
    public int comboIndex = 0;
    private void ReleaseNormalAttack()
    {
        PECommon.Log("Release Normal Attack");
        if (entitySelfPlayer.currentAniState == AniState.Attack)
        {
            //500ms以内进行连招
            double nowTime = TimerSvc.Instance.GetCurtTime();
            if (!(nowTime - lastTime < Constants.ComboSpace) || lastTime == 0) return;
            if (comboArr[comboIndex] != comboArr[comboArr.Length - 1])
            {
                comboIndex++;
                entitySelfPlayer.comboQueue.Enqueue(comboArr[comboIndex]);
                lastTime = nowTime;
            }
            else
            {
                ResetComboData();
            }
        }
        else if (entitySelfPlayer.currentAniState==AniState.Idle||entitySelfPlayer.currentAniState==AniState.Move)
        {
            comboIndex = 0;
            entitySelfPlayer.Attack(comboArr[comboIndex]);
            lastTime = TimerSvc.Instance.GetCurtTime();
        }
    }

    public void ResetComboData()
    {
        comboIndex = 0;
        lastTime = 0;
    }
    private void ReleaseSkill1()
    {
        //PECommon.Log("Release Skill 1");
        entitySelfPlayer.Attack(101);
    }
    private void ReleaseSkill2()
    {
        //PECommon.Log("Release Skill 2");
        entitySelfPlayer.Attack(102);
    }
    private void ReleaseSkill3()
    {
        //PECommon.Log("Release Skill 3");
        entitySelfPlayer.Attack(103);
    }

    public bool GetCanRlsSkill()
    {
        return entitySelfPlayer.canRlsSkill;
    }
    #endregion
    
}

public enum SkillType
{
    NormalAttack,
    Skill1,
    Skill2,
    Skill3,
}