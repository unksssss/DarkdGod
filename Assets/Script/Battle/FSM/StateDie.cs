// time:2025/12/05/14
// path:E:\unityproject\DarkGod\Assets\Script\Battle\FSM\StateDie.csStateDie.cs
// func:
// www34


using UnityEngineInternal;

public class StateDie:IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.RmvSkillCb();
    }

    public void Process(EntityBase entity, params object[] args)
    {
        entity.SetAction(Constants.ActionDie);
        if (entity.entityType == EntityType.Monster)
        {
            entity.SetColliderState(false);
            TimerSvc.Instance.AddTimeTask(i =>
            {
                entity.SetActive(false);
            }, Constants.DieAniLength);
        }
    }

    public void Exit(EntityBase entity, params object[] args)
    {
            
    }
}