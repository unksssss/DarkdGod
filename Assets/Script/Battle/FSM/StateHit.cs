// time:2025/12/06/15
// path:E:\unityproject\DarkGod\Assets\Script\Battle\FSM\StateHit.csStateHit.cs
// func:受击状态
// www34


using UnityEngine;

public class StateHit:IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        //受击时禁止技能移动
        entity.SetSkillMoveState(false);
        entity.SetDir(Vector2.zero);
        
        entity.RmvSkillCb();
    }

    public void Process(EntityBase entity, params object[] args)
    {
        //停止移动
        if (entity.entityType == EntityType.Player)
        {
            entity.canRlsSkill = false;
        }
        
        entity.SetDir(Vector2.zero);
        entity.SetAction(Constants.ActionHit);
        AudioSource audioSource = entity.GetAudioSource();
        if (entity.entityType == EntityType.Player)
        {
            AudioSvc.instance.PlayCharacterAudio(Constants.AssassinHit, audioSource);
        }
        
        
        TimerSvc.Instance.AddTimeTask(i =>
        {
            entity.SetAction(Constants.ActionDefault);
            entity.Idle();
        }, (int)(GetHitAniLength(entity) * 1000));
    }

    public void Exit(EntityBase entity, params object[] args)
    {
            
    }

    private float GetHitAniLength(EntityBase entity)
    {
        AnimationClip[] clips=entity.GetAnimationClips();
        for (int i = 0; i < clips.Length; i++)
        {
            string aniName=clips[i].name;
            if (aniName.Contains("hit") ||
                aniName.Contains("Hit") ||
                aniName.Contains("HIT") )
            {
                return clips[i].length;
            }
        }
        return 1;
    }
    
}