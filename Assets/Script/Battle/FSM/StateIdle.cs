// time:2025/11/22/15
// path:E:\unityproject\DarkGod\Assets\Script\Battle\FSM\StateIdle.csStateIdle.cs
// func:待机状态
// www34


using UnityEngine;

public class StateIdle:IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        //PECommon.Log("Enter Idle");
        entity.SetDir(Vector2.zero);
        entity.skEndCB = -1;
    }

    public void Process(EntityBase entity, params object[] args)
    {
        if (entity.nextSkillID != 0)
        {
            entity.Attack(entity.nextSkillID);
        }
        else
        {
            Vector2 currentDir=entity.GetCurrentDir();
            if (currentDir != Vector2.zero)
            {
                entity.SetDir(currentDir);
                entity.Move();
            }
            else
            {
                entity.canRlsSkill = true;
                entity.SetBlend(Constants.BlendIdle);
                //PECommon.Log("Process Idle");
            }
        }
    }

    public void Exit(EntityBase entity, params object[] args)
    {
        //PECommon.Log("Exit Idle");
            
    }
}