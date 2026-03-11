// time:2025/11/19/16
// path:E:\unityproject\DarkGod\Assets\Script\Battle\StateMgr.csStateMgr.cs
// func:
// www34


using System.Collections.Generic;
using UnityEngine;

public class StateMgr : MonoBehaviour
{
    private Dictionary<AniState,IState> fsm = new Dictionary<AniState, IState>();
    public void Init()
    {
        
        fsm.Add(AniState.Born,new StateBorn());
        fsm.Add(AniState.Idle,new StateIdle());
        fsm.Add(AniState.Move,new StateMove());
        fsm.Add(AniState.Attack,new StateAttack());
        fsm.Add(AniState.Die,new StateDie());
        fsm.Add(AniState.Hit,new StateHit());
        
        PECommon.Log("StateMgr Init.....");
                
    }

    public void ChangeState(EntityBase entity, AniState targetState, params object[] args)
    {
        if(entity.currentAniState==targetState) return;
        if (!fsm.ContainsKey(targetState)) return;
        if (entity.currentAniState != AniState.None)
        {
            fsm[entity.currentAniState].Exit(entity,args);
        }
        entity.currentAniState = targetState;
        fsm[targetState].Enter(entity,args);
        fsm[targetState].Process(entity,args);
    }
}