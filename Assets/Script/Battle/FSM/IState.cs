// time:2025/11/22/15
// path:E:\unityproject\DarkGod\Assets\Script\Battle\FSM\IState.csIState.cs
// func:状态接口
// www34


public interface IState
{
    void Enter(EntityBase entity,params object[] args);
    void Process(EntityBase entity,params object[] args);
    void Exit(EntityBase entity,params object[] args);
}

public enum AniState
{
    None,
    Born,
    Idle,
    Move,
    Attack,
    Die,
    Hit
}