// time:2025/12/05/14
// path:E:\unityproject\DarkGod\Assets\Script\Battle\FSM\StateBorn.csStateBorn.cs
// func:出生状态
// www34

    public class StateBorn : IState
    {
        public void Enter(EntityBase entity, params object[] args)
        {
                
        }

        public void Process(EntityBase entity, params object[] args)
        {
            //播放出生动画
            entity.SetAction(Constants.ActionBorn);
            TimerSvc.Instance.AddTimeTask(i =>
            {
                entity.SetAction(Constants.ActionDefault);
            }, 500);
        }

        public void Exit(EntityBase entity, params object[] args)
        {
            
        }
    }
