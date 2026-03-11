// time:2025/11/03/14
// path:E:\unityproject\DarkGod\Server\Server\02System\03StrongSystem\StrongSystem.csStrongSystem.cs
// func:强化系统
// www34

using System.Data.Common;
using PEProtocol;

public class StrongSystem
{
    private static StrongSystem instance = null;
    public static StrongSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new StrongSystem();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;
    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        PECommon.Log("StrongSystem Init");
        
    }

    public void ReqStrong(MsgPack msgPack)
    {
        ReqStrong data = msgPack.msg.reqStrong;
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspStrong
        };
        PlayerData playerData = cacheSvc.GetPlayerDataBySession(msgPack.session);
        int curtStarLv = playerData.strong[data.pos];
        StrongCfg nextSd=CfgSvc.Instance.GetStrongCfg(data.pos,curtStarLv+1);
        //条件判断
        if (playerData.lv < nextSd.minlv)
        {
            msg.err = (int)ErrorCode.LackLevel;
        }
        else if (playerData.coin<nextSd.coin)
        {
            msg.err = (int)ErrorCode.LackCoin;
        }
        else if(playerData.crystal<nextSd.crystal)
        {
            msg.err = (int)ErrorCode.LackCrystal;
        }
        else
        {
            //任务数据更新
            TaskSystem.Instance.CalcuTaskPrg(playerData,3);
            //资源扣除
            playerData.coin-=nextSd.coin;
            playerData.crystal-=nextSd.crystal;
            playerData.strong[data.pos] += 1;
            //属性增益
            playerData.hp += nextSd.addhp;
            playerData.ad += nextSd.addhurt;
            playerData.ap += nextSd.addhurt;
            playerData.addef += nextSd.adddef;
            playerData.apdef += nextSd.adddef;
        }
        //更新数据库
        if (!cacheSvc.UpdataPlayerData(playerData.id, playerData))
        {
            msg.err = (int)ErrorCode.UpdataDBError;
        }
        else
        {
            msg.rspStrong = new RspStrong
            {
                coin = playerData.coin,
                crystal = playerData.crystal,
                hp = playerData.hp,
                ad = playerData.ad,
                ap = playerData.ap,
                addef = playerData.addef,
                apdef = playerData.apdef,
                strongArr = playerData.strong
                
            };
            msgPack.session.SendMsg(msg);
        }
    }
}
