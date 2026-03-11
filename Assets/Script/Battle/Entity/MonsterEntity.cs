// time:2025/12/01/14
// path:E:\unityproject\DarkGod\Assets\Script\Battle\Entity\MonsterEntity.csMonsterEntity.cs
// func:怪物逻辑实体
// www34


using  UnityEngine;
using UnityEngineInternal;

public class MonsterEntity:EntityBase
{
    private float checkTime = 2;
    private float checkCount = 0;
    private float atkTime = 2;
    private float atkCount = 0;
    public MonsterData md;
    private bool runAI=true;

    public MonsterEntity()
    {
        entityType=EntityType.Monster;
    }
    public override void SetBattleProps(BattleProps battleProps)
    {
        int level = md.level;
        var p = new BattleProps()
        {
            ad = battleProps.ad * level,
            addef = battleProps.addef * level,
            ap = battleProps.ap * level,
            apdef = battleProps.apdef * level,
            critical = battleProps.critical * level,
            dodge = battleProps.dodge * level,
            hp = battleProps.hp * level,
            pierce = battleProps.pierce * level,
        };
        BattleProps = p;
        Hp = p.hp;
    }

    public override void TickAILogic()
    {
        if(!runAI) return;
        
        
        
        if (currentAniState != AniState.Idle && currentAniState != AniState.Move) return;
        //暂停怪物ai
        if (battleMgr.isPauseGame)
        {
            Idle();
            return;
        }
        
        float deltaTime=Time.deltaTime;
        
        
        checkCount+=deltaTime;
        if (checkCount<checkTime) return;

        Vector2 dir = CalcTargetDir();
        Debug.Log("AI Check");
        //判断是否在攻击范围内
        if (!InAtkRange())
        {
            //不在，设置移动方向，并进入移动状态
            SetDir(dir);
            Move();
        }
        else
        {
            //在,停止移动
            SetDir(Vector2.zero);
            //判断攻击间隔
            atkCount += checkCount;
            if (atkCount >= atkTime)
            {
                //达到，转向并进入攻击
                SetAtkRotate(dir);
                Attack(md.Cfg.skillID);
                atkCount = 0;
            }
            else
            {
                //未达到攻击时间，进入idle状态等待
                Idle();
            }

            checkCount = 0;
            checkTime = PETool.RDInt(1, 5) * 1.0f / 10;
        }

    }

    public override Vector2 CalcTargetDir()
    {
        PlayerEntity target = battleMgr.entitySelfPlayer;
        if (target == null || target.currentAniState == AniState.Die)
        {
            runAI = false;
            return Vector2.zero;
        }

        Vector3 targetPos = target.GetPos();
        Vector3 selfPos = GetPos();
        return new Vector2(targetPos.x-selfPos.x,targetPos.z-selfPos.z).normalized;
        
    }

    private bool InAtkRange()
    {
        PlayerEntity target = battleMgr.entitySelfPlayer;
        if (target == null || target.currentAniState == AniState.Die)
        {
            runAI = false;
            return false;
        }
        Vector3 targetPos = target.GetPos();
        Vector3 selfPos = GetPos();
        targetPos.y = 0;
        selfPos.y = 0;
        float dis = Vector3.Distance(targetPos, selfPos);
        return dis <= md.Cfg.atkDis;
    }

    public override bool GetStop()
    {
        if (md.Cfg.canStop)
        {
            if (currentSkillCfg != null)
            {
                return currentSkillCfg.isBreak;
            }

            return true;
        }

        return false;
    }

    public override void SetHPVal(int oldVal, int newVal)
    {
        if (md.Cfg.mType == MonsterType.Boss)
        {
            BattleSys.Instance.playerControlWnd.SetBossHPBarVal(oldVal,newVal,BattleProps.hp);
        }
        else
        {
            base.SetHPVal(oldVal, newVal);
        }
    }
}