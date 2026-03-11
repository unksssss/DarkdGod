// time:2025/11/12/21
// path:E:\unityproject\DarkGod\Server\Server\02System\07TaskSystem\TaskSystem.csTaskSystem.cs
// func:任务奖励系统
// www34


using PEProtocol;

public class TaskSystem
{
    private static TaskSystem instance = null;
    public static TaskSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TaskSystem();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;
    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        PECommon.Log("TaskSystem Init Done");
    }

    public void ReqTaskReward(MsgPack msgPack)
    {
        ReqTaskReward data = msgPack.msg.reqTaskReward;
        
        GameMsg msg = new GameMsg()
        {
            cmd = (int)CMD.RspTaskReward,
        };
        PlayerData playerData = cacheSvc.GetPlayerDataBySession(msgPack.session);
        TaskRewardCfg trc = CfgSvc.Instance.GetTaskCfg(data.rid);
        TaskRewardData trd = CalcTaskRewardData(playerData, data.rid);
        if (trd.prgs == trc.count && !trd.istaked)
        {
            playerData.coin += trc.coin;
            PECommon.CalcExp(playerData,trc.exp);
            trd.istaked = true;
            // 任务奖励已领取，更新数据库信息与玩家信息
            UpdatePDTaskArr(playerData,trd);
            if (!cacheSvc.UpdataPlayerData(playerData.id, playerData))
            {
                msg.err = (int)ErrorCode.UpdataDBError;
            }
            else
            {
                msg.rspTaskReward = new RspTaskReward
                {
                    coin = playerData.coin,
                    exp = playerData.exp,
                    lv = playerData.lv,
                    taskArr = playerData.taskArr
                };
            }
        }
        else
        {
            msg.err = (int)ErrorCode.ClientDataError;
        }
        msgPack.session.SendMsg(msg);
    }

    private TaskRewardData CalcTaskRewardData(PlayerData playerData,int rid)
    {
        TaskRewardData trd = null;
        for (int i = 0; i < playerData.taskArr.Length; i++)
        {
            string[] taskinfo=playerData.taskArr[i].Split('|');
            if (int.Parse(taskinfo[0]) == rid)
            {
                trd = new TaskRewardData
                {
                    ID = rid,
                    prgs = int.Parse(taskinfo[1]),
                    istaked = taskinfo[2].Equals("1")
                };
                break;
            }
        }
        return trd;
    }

    public void UpdatePDTaskArr(PlayerData playerData, TaskRewardData trd)
    {
        string result=trd.ID+"|"+trd.prgs+"|"+(trd.istaked?"1":"0");
        
        for (int i = 0; i < playerData.taskArr.Length; i++)
        {
            string[] taskinfo = playerData.taskArr[i].Split('|');
            if (int.Parse(taskinfo[0]) == trd.ID)
            {
                playerData.taskArr[i] = result;
                break;
            }
        }
    }

    public void CalcuTaskPrg(PlayerData playerData,int taskId)
    {
        var trd=CalcTaskRewardData(playerData,taskId);
        var trc=CfgSvc.Instance.GetTaskCfg(taskId);
        if (trd.prgs >= trc.count) return;
        trd.prgs += 1;
        UpdatePDTaskArr(playerData, trd);
        var session = cacheSvc.GetServerSession(playerData.id);
        session?.SendMsg(new GameMsg()
        {
            cmd = (int)CMD.PushTaskPrg,
            pushTaskPrg = new PushTaskPrg
            {
                taskArr = playerData.taskArr
            }
        });
    }
    public PushTaskPrg GetTaskPrg(PlayerData playerData,int taskId)
    {
        var trd=CalcTaskRewardData(playerData,taskId);
        var trc=CfgSvc.Instance.GetTaskCfg(taskId);
        if (trd.prgs >= trc.count) return null;
        trd.prgs += 1;
        UpdatePDTaskArr(playerData, trd);
        return new PushTaskPrg
        {
            taskArr = playerData.taskArr
        };
    }
}