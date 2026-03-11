// time:2025/11/24/14
// path:E:\unityproject\DarkGod\Assets\Script\Battle\FSM\IStateAttack.csIStateAttack.cs
// func:攻击状态
// www34


    public class StateAttack:IState
    {
        public void Enter(EntityBase entity, params object[] args)
        {
            //PECommon.Log("Enter Attack State");
            entity.currentSkillCfg=ResSvc.Instance.GetSkillCfg((int) args[0]);
        }

        public void Process(EntityBase entity, params object[] args)
        {
            //entity.SetAction(1);
            entity.SkillAttack((int)args[0]);
            if (entity.entityType == EntityType.Player)
            {
                entity.canRlsSkill = false;
            }
            //PECommon.Log("Proccess Attack State");
        }

        public void Exit(EntityBase entity, params object[] args)
        {
            entity.ExitCurtSkill();
            //PECommon.Log("Exit Attack State");
        }
    }
