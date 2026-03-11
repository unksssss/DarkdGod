// time:2025/11/19/16
// path:E:\unityproject\DarkGod\Assets\Script\Battle\SkillMgr.csSkillMgr.cs
// func:
// www34


using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Random = System.Random;

public class SkillMgr : MonoBehaviour
{
    private ResSvc resSvc;
    private TimerSvc timerSvc;
    public void Init()
    {
        resSvc=ResSvc.Instance;
        timerSvc=TimerSvc.Instance;
        PECommon.Log("SkillMgr Init.....");
        
    }

    public void SkillAttack(EntityBase caster, int skillId)
    {
        caster.skillMoveCBLst.Clear();
        caster.skillActionCBLst.Clear();
        
        AttackEffect(caster,skillId);
        AttackDamage(caster,skillId);
    }
    /// <summary>
    ///  技能表现效果
    /// </summary>
    /// <param name="caster">
    /// </param>
    /// <param name="skillId"></param>
    public void AttackEffect(EntityBase caster,int skillId)
    {
        SkillCfg skillData = resSvc.GetSkillCfg(skillId);
        
        
        

        if (!skillData.isCollide)
        {
            //忽略玩家与怪物之间的刚体碰撞
            Physics.IgnoreLayerCollision(9,10);
            timerSvc.AddTimeTask(i =>
            { 
                Physics.IgnoreLayerCollision(9,10,false);
            },skillData.aniTime);
        }

        
        //进入不可阻挡的状态
        if (!skillData.isBreak)
        {
            caster.entityState = EntityState.UnStoppableState;
        }
        
        
        if (caster.entityType == EntityType.Player)
        {
            if (caster.GetCurrentDir() == Vector2.zero)
            {
                // 搜索最近的怪物
                Vector2 dir = caster.CalcTargetDir();
                //todo 
                caster.SetAtkRotate(dir);
            }
            else
            {
                //控制人物方向
                caster.SetAtkRotate(caster.GetCurrentDir(), true);
            }
        }
        
        
        
        caster.SetAction(skillData.skillAction);
        caster.SetFX(skillData.fx,skillData.aniTime);
        
        CalcSkillMove(caster,skillData);
        
        caster.canControl = false;
        caster.SetDir(Vector2.zero);
        
        caster.skEndCB=timerSvc.AddTimeTask(tid =>
        {
            caster.Idle();
        },skillData.aniTime );
            
    }
    /// <summary>
    /// 技能伤害效果
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="skillId"></param>
    public void AttackDamage(EntityBase caster, int skillId)
    {
        SkillCfg skillData = resSvc.GetSkillCfg(skillId);
        List<int> skillActionLst=skillData.skillActionList;
        int sum = 0;
        for (int i = 0; i < skillActionLst.Count; i++)
        {
            int index = i;
            var actionID = skillActionLst[i];
            SkillActionCfg skillActionData = resSvc.GetSkillActionCfg(actionID);
            sum += skillActionData.delayTime;
            if (sum > 0)
            {
                int actionid=timerSvc.AddTimeTask(tid =>
                {
                    if (caster != null)
                    {
                        SkillAction(caster, skillData,index); 
                        caster.RmvSkillActionCb(tid);
                    }
                }, sum);
                caster.skillActionCBLst.Add(actionid);
                
            }
            //瞬时触发
            else
            {
                SkillAction(caster, skillData, index);
            }
        }
    }
    //处理单个技能伤害点
    private void SkillAction(EntityBase caster,SkillCfg skillCfg,int index)
    {
        //获取场景怪物实体
        List<MonsterEntity> monsterLst = caster.battleMgr.GetMonsterEntityList();
        SkillActionCfg skillActionData = resSvc.GetSkillActionCfg(skillCfg.skillActionList[index]);
        int damage=skillCfg.skillDamageList[index];
        //区分目标
        if (caster.entityType == EntityType.Monster)
        {
            EntityBase target = BattleSys.Instance.battleMgr.entitySelfPlayer;
            if (target==null)
            {
                return;
            }
            if (InRange(caster.GetPos(), target.GetPos(), skillActionData.radius)
                &&InAngle(caster.GetTrans(),target.GetPos(), skillActionData.angle))
            {
                //计算伤害
                //CalcDamage(entityBase);
                CalcDamage(caster,target,skillCfg,damage);
            }
        }
        else if (caster.entityType == EntityType.Player)
        {
            foreach (var target in monsterLst)
            {
                if (InRange(caster.GetPos(), target.GetPos(), skillActionData.radius)
                    &&InAngle(caster.GetTrans(), target.GetPos(), skillActionData.angle))
                {
                    //计算伤害
                    //CalcDamage(entityBase);
                    CalcDamage(caster,target,skillCfg,damage);
                }
            }
        }
    }

    private Random rd = new Random();
    private void CalcDamage(EntityBase caster,EntityBase target,SkillCfg skillCfg, int damage)
    {
        int dmgsum = damage;
        switch (skillCfg.damageType)
        {
            case DamageType.AD:
                //计算闪避
                int dodgeNUm = PETool.RDInt(1, 100, rd);
                if (dodgeNUm <= target.BattleProps.dodge)
                {
                    //PECommon.Log("闪避成功:"+dodgeNUm+"/"+target.BattleProps.dodge);
                    target.SetDodge();
                    return;
                }
                //计算属性加成
                dmgsum += caster.BattleProps.ad;
                //计算暴击
                int criticalNum = PETool.RDInt(1, 100, rd);
                if (criticalNum <= target.BattleProps.critical)
                {
                    float criticalRate=1+(PETool.RDInt(1, 100, rd)/100.0f);
                    dmgsum =(int)criticalRate*dmgsum;
                    target.SetCritical(dmgsum);
                }
                //计算穿甲
                int addef=(int)((1-caster.BattleProps.pierce/100.0f)*target.BattleProps.addef);
                dmgsum -= addef;
                break;
            case DamageType.AP:
                //计算属性加成
                dmgsum+=caster.BattleProps.ap;
                //计算魔抗
                dmgsum -= target.BattleProps.apdef;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        //dmgsum=dmgsum>0?dmgsum:0;
        if(dmgsum<=0) return;
        //todo 玩家血条修改
        target.SetHurt(dmgsum);
        
        if (dmgsum > target.Hp)
        {
            target.Hp = 0;
            target.Die();
            if (target.entityType == EntityType.Monster)
            {
                
                target.battleMgr.RmvMonster(target.Name);
            }
            else if (target.entityType == EntityType.Player)
            {
                target.battleMgr.EndBattle(false,0);
                target.battleMgr.entitySelfPlayer = null;
            }
          
        }
        else
        {
            target.Hp -= dmgsum;
            if (target.entityState == EntityState.None&&target.GetStop())
            {
                target.Hit();
            }
        }
        PECommon.Log("DMG:"+dmgsum);
    }
    private bool InRange(Vector3 from, Vector3 to, float range)
    {
        float dis=Vector3.Distance(from,to);
        if (dis <= range)
        {
            return true;
        }
        return false;
    }
    private bool InAngle(Transform trans, Vector3 to, float angle)
    {
        //todo 角度判断修正
        if (Mathf.Approximately(angle, 360))
        {
            return true;
        }

        Vector3 start=trans.transform.position;
        start.y = 0;
        to.y = 0;
        Vector3 dir=(to-start).normalized;
        float _angle = Vector3.Angle(trans.forward.normalized, dir);
        return _angle < angle / 2;
    }

    
    private void CalcSkillMove(EntityBase entity, SkillCfg skillData)
    {
        List<int> skillMoveList=skillData.skillMoveList;
        int sum = 0;
        foreach (var VARIABLE in skillMoveList)
        {
            SkillMoveCfg skillMoveData = resSvc.GetSkillMoveCfg(VARIABLE);
            float speed = skillMoveData.moveDis / (skillMoveData.moveTime / 1000f);
            sum += skillMoveData.delayTime;
            if (sum> 0)
            {
                int moveID = timerSvc.AddTimeTask(i =>
                {
                    entity.SetSkillMoveState(true,speed);
                    entity.RmvSkillMoveCb(i);
                },sum);
                entity.skillMoveCBLst.Add(moveID);
            }
            else
            {
                entity.SetSkillMoveState(true,speed);
            }
            sum+=skillMoveData.moveTime;
            int stopid=timerSvc.AddTimeTask(i =>
            {
                entity.SetSkillMoveState(false);
            },sum);
            entity.skillMoveCBLst.Add(stopid);
        }
    }

    
}