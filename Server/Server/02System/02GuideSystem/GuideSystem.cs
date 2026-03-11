// time:
// 2025102717
// path:
// E:\unityproject\DarkGod\Server\Server\02System\02GuideSystem\GuideSystem.csGuideSystem.cs
// func: 服务端引导系统


using System.Threading.Tasks;
using PEProtocol;

public class GuideSystem
{
    private static GuideSystem instance = null;
    private CfgSvc cfgSvc=null;
    public static GuideSystem Instance
    {
        get { return instance ?? (instance = new GuideSystem()); }
    }
    private CacheSvc cacheSvc = null;
    public void Init()
    {
        cacheSvc=CacheSvc.Instance;
        cfgSvc=CfgSvc.Instance;
        PECommon.Log("GuideSystem Init");
    }

    public void ReqGuide(MsgPack msgPack)
    {
        ReqGuide data = msgPack.msg.reqGuide;
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspGuide
        };
        PlayerData playerData = cacheSvc.GetPlayerDataBySession(msgPack.session);
        GuideCfg guideCfg = cfgSvc.GetGuideCfg(playerData.guideid);
        
        if (playerData.guideid == data.guideid)
        {
            if (playerData.guideid == 1001)
            {
                TaskSystem.Instance.CalcuTaskPrg(playerData,1);
            }
            //更新任务id
            playerData.guideid += 1;
            //更新玩家数据  读取数据文件
            playerData.coin += guideCfg.coin;
            //处理经验
            PECommon.CalcExp(playerData,guideCfg.exp);
            if (!cacheSvc.UpdataPlayerData(playerData.id, playerData))
            {
                //更新失败
                msg.err = (int)ErrorCode.UpdataDBError;
            }
            else
            {
                //更新成功，将更新返回客户端
                msg.rspGuide = new RspGuide
                {
                    guideid = playerData.guideid,
                    coin = playerData.coin,
                    exp = playerData.exp,
                    lv = playerData.lv
                };
            }
        }
        else
        {
            //客户端与服务端任务数据不同步
            //返回错误码
            msg.err = (int)ErrorCode.ServerDataError;
        }
        msgPack.session.SendMsg(msg);
        //更新玩家数据
    }

    
}
