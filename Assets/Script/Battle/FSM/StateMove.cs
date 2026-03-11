// time:2025/11/22/15
// path:E:\unityproject\DarkGod\Assets\Script\Battle\FSM\StateMove.csStateMove.cs
// func:移动状态
// www34


    public class StateMove:IState
    {
        public void Enter(EntityBase entity, params object[] args)
        {
            //PECommon.Log("Enter Move");
            
            
        }

        public void Process(EntityBase entity, params object[] args)
        {
            //PECommon.Log("Process Move");
            entity.SetBlend(Constants.BlendMove);
        }

        public void Exit(EntityBase entity, params object[] args)
        {
            //PECommon.Log("Exit Move");
            
        }
    }
