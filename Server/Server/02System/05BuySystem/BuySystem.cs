// time:2025/11/08/16
// path:E:\unityproject\DarkGod\Server\Server\02System\05BuySystem\BuySystem.csBuySystem.cs
// func:
// www34

using System;
using PEProtocol;

public class BuySystem
{
    private static BuySystem instance = null;
    public static BuySystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BuySystem();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;
    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        PECommon.Log("BuySystem Init Done");
    }

    public void ReqBuy(MsgPack msgPack)
    {
        
        ReqBuy data = msgPack.msg.reqBuy;
        GameMsg msg = new GameMsg()
        {
            cmd = (int)CMD.RspBuy,
        };
        PlayerData playerData = cacheSvc.GetPlayerDataBySession(msgPack.session);
        if (data.cost > playerData.diamond)
        {
            msg.err = (int)ErrorCode.LackDiamond;
        }
        else 
        {
            PushTaskPrg pushTaskPrg=null;
            playerData.diamond -= data.cost;
            switch (data.buyType)
            {
                case BuyType.Power:
                    playerData.power += 100;
                    pushTaskPrg=TaskSystem.Instance.GetTaskPrg(playerData,4);
                    break;
                case BuyType.Coin:
                    playerData.coin += 1000;
                    pushTaskPrg=TaskSystem.Instance.GetTaskPrg(playerData,5);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //更新数据库
            if (!cacheSvc.UpdataPlayerData(playerData.id, playerData))
            {
                msg.err = (int)ErrorCode.UpdataDBError;
            }
            else
            {
                msg.rspBuy = new RspBuy()
                {
                    buyType = data.buyType,
                    coin = playerData.coin,
                    power = playerData.power,
                    diamond = playerData.diamond,
                };
                msg.pushTaskPrg = pushTaskPrg;
            }
        }
        msgPack.session.SendMsg(msg);
    }
    
}
