// time:2025/11/22/15
// path:E:\unityproject\DarkGod\Assets\Script\Battle\Entity\EntityBase.csEntityBase.cs
// func:逻辑实体基类
// www34


using System.Collections;
using System.Collections.Generic;
using System.Net.Configuration;
using UnityEngine;

public abstract class EntityBase
{

    public EntityType entityType = EntityType.None;
    public AniState currentAniState = AniState.None;
    public EntityState entityState = EntityState.None;
    
    
    public BattleMgr battleMgr=null;
    public StateMgr stateMgr =null;
    public SkillMgr skillMgr = null;
    protected Controller controller=null;

    public bool canControl = true;
    public bool canRlsSkill = true;
    private int hp;
    private string name;

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public int Hp
    {
        get { return hp; }
        set
        {
            // 通知ui层刷新
            SetHPVal(hp,value);
            //PECommon.Log("Hp Chang:"+hp+"to"+value);
            hp = value;
        }
        
    }
    public Queue<int> comboQueue = new Queue<int>();
    public int nextSkillID=0;
    public SkillCfg currentSkillCfg=null;
    public BattleProps BattleProps { get; protected set; }
    
    
    
    public List<int> skillMoveCBLst=new List<int>();
    public List<int> skillActionCBLst=new List<int>();

    public int skEndCB = -1;
    public virtual void SetBattleProps(BattleProps battleProps)
    {
        Hp=battleProps.hp;
        BattleProps = battleProps;
    }
    
    public void Born()
    {
        stateMgr.ChangeState(this,AniState.Born);
    }
    
    public void Move()
    {
        stateMgr.ChangeState(this,AniState.Move);
    }

    public void Idle()
    {
        stateMgr.ChangeState(this,AniState.Idle);
    }

    public virtual void TickAILogic()
    {
        
    }
    public void Attack(int skillId)
    {
        stateMgr.ChangeState(this,AniState.Attack,skillId);
    }
    public void Die()
    {
        stateMgr.ChangeState(this,AniState.Die);
    }
    public void Hit()
    {
        stateMgr.ChangeState(this,AniState.Hit);
    }

    public virtual void SetActive(bool active = true)
    {
        if (controller != null)
        {
            controller.gameObject.SetActive(active);
        }
    }

    public virtual void SetController(Controller ctrl)
    {
        controller = ctrl;
    }
    public virtual void SetBlend(float blend)
    {
        if (!ReferenceEquals(controller, null))
        {
            controller.SetBlend(blend);
        }
    }

    public virtual void SetDir(Vector2 dir)
    {
        if (controller!=null)
        {
            controller.Dir = dir;
            
        }
    }

    public virtual void SetAtkRotate(Vector2 atkRot,bool offset=false)
    {
        if (controller == null) return;
        if (offset)
        {
            controller.SetAtkRotCam(atkRot);
        }
        else
        {
            controller.SetAtkRotLocal(atkRot);
        }
    }
    public virtual void SetAction(int action)
    {
        if (controller != null)
        {
            controller.SetAction(action);
        }
    }

    public virtual void SetColliderState(bool state)
    {
        
    }
    public virtual void SetFX(string fx, int delay)
    {
        if (controller != null)
        {
            controller.SetFX(fx, delay);
        }
    }
    public virtual void SetSkillMoveState(bool move, float skillSpeed=0f)
    {
        if (controller != null)
        {
            controller.SetSkillMoveState(move, skillSpeed);
        }
    }

    #region 战斗信息显示

    public virtual void SetDodge()
    {
        if (controller == null)
            return;
        GameRoot.Instance.dynamicWnd.SetDodge(Name);
    }
    public virtual void SetCritical(int crit)
    {
        if (controller == null)
            return;
        GameRoot.Instance.dynamicWnd.SetCritical(Name,crit);
    }
    public virtual void SetHurt(int hurt)
    {
        if (controller == null)
            return;
        GameRoot.Instance.dynamicWnd.SetHurt(Name,hurt);
    }
    public virtual void SetHPVal(int oldVal, int newVal)
    {
        if (controller == null)
            return;
        GameRoot.Instance.dynamicWnd.SetHPVal(Name,oldVal,newVal);
        
    }

    #endregion
    
    public virtual void SkillAttack(int skillId)
    {
        skillMgr.SkillAttack(this,skillId);
    }
    

    public virtual Vector2 GetCurrentDir()
    {
        return Vector2.zero;
    }

    public virtual Vector3 GetPos()
    {
        return controller.transform.position;
    }

    public virtual Transform GetTrans()
    {
        return controller.transform;
    }

    public virtual bool GetStop()
    {
        return true;
    }
    public virtual AnimationClip[] GetAnimationClips()
    {
        return controller != null ? controller.ani.runtimeAnimatorController.animationClips : null;
    }

    public AudioSource GetAudioSource()
    {
        return controller!=null ? controller.GetComponent<AudioSource>() : null;
    }

    public virtual Vector2 CalcTargetDir()
    {
        return Vector2.zero;
    }
    public void ExitCurtSkill()
    {
        canControl = true;
        
        
        if(currentSkillCfg==null) return;
        if (!currentSkillCfg.isBreak)
        {
            entityState = EntityState.None;
        }
        
        
        if (currentSkillCfg.isCombo)
        {
            canControl = true;
            nextSkillID = comboQueue.Count > 0 ? comboQueue.Dequeue() : 0;
        }

        currentSkillCfg = null;
        SetAction(Constants.ActionDefault);
    }

    public void RmvSkillMoveCb(int tid)
    {
        int index = -1;
        for (int i = 0; i < skillMoveCBLst.Count; i++)
        {
            if (skillMoveCBLst[i] == tid)
            {
                index = i;
                break;
            }
        }
        if (index != -1)
        {
            skillMoveCBLst.RemoveAt(index);
        }
    }
    public void RmvSkillActionCb(int tid)
    {
        int index = -1;
        for (int i = 0; i < skillActionCBLst.Count; i++)
        {
            if (skillActionCBLst[i] == tid)
            {
                index = i;
                break;
            }
        }
        if (index != -1)
        {
            skillActionCBLst.RemoveAt(index);
        }
    }

    public void RmvSkillCb()
    {
        //Hit 状态清理所有未完成的伤害和位移计算
        foreach (var tid in skillMoveCBLst)
        {
            TimerSvc.Instance.DeleteTimeTask(tid);
        }
        foreach (var tid in skillActionCBLst)
        {
            TimerSvc.Instance.DeleteTimeTask(tid);
        }
        //攻击被中断时，清空技能结束回调
        if (skEndCB != -1)
        {
            TimerSvc.Instance.DeleteTimeTask(skEndCB);
            skEndCB = -1;
        }
        skillActionCBLst.Clear();
        skillMoveCBLst.Clear();
        //清空连招ID
        if (nextSkillID != 0 || comboQueue.Count > 0)
        {
            nextSkillID = 0;
            comboQueue.Clear();
        }
        battleMgr.ResetComboData();
    }
}