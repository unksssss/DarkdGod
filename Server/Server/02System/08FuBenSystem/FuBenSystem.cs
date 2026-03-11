// time:2025/11/17/19
// path:E:\unityproject\DarkGod\Server\Server\02System\08FuBenSystem\FuBenSystem.csFuBenSystem.cs
// func:副本战斗业务
// www34


using PEProtocol;

public class FuBenSystem
{
    private static FuBenSystem instance = null;
    public static FuBenSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new FuBenSystem();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;
    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        PECommon.Log("FuBenSystem Init Done");
    }

    public void ReqFBFight(MsgPack msgPack)
    {
        ReqFBFight data = msgPack.msg.reqFBFight;
        GameMsg msg = new GameMsg()
        {
            cmd = (int)CMD.RspFBFight,
        };
        PlayerData playerData = cacheSvc.GetPlayerDataBySession(msgPack.session);
        int power = CfgSvc.Instance.GetMapCfg(data.fid).power;
        if (data.fid > playerData.fuben)
        {
            msg.err = (int)ErrorCode.ClientDataError;
        }
        else if (playerData.power<power)
        {
            msg.err = (int)ErrorCode.LackPower;
        }
        else
        {
            playerData.power -= power;
            if (cacheSvc.UpdataPlayerData(playerData.id, playerData))
            {
                msg.rspFBFight=new RspFBFight
                {
                    fid = data.fid,
                    power = playerData.power
                };
            }
            else
            {
                msg.err = (int)ErrorCode.UpdataDBError;
            }
        }        
        msgPack.session.SendMsg(msg);
    }

    public void ReqEndFBFight(MsgPack msgPack)
    {
        ReqFBFightEnd data = msgPack.msg.reqFBFightEnd;
        GameMsg msg = new GameMsg()
        {
            cmd = (int)CMD.RspEndFBFight,
        };
        //校验战斗是否合法
        if (data.isWin)
        {
            if (data.costTime > 0 && data.restHP > 0)
            {
                //根据副本id获取相应奖励
                MapCfg reward=CfgSvc.Instance.GetMapCfg(data.fid);
                PlayerData pd=cacheSvc.GetPlayerDataBySession(msgPack.session);

                pd.coin += reward.coin;
                pd.crystal+=reward.crystal;
                PECommon.CalcExp(pd,reward.exp);
                if (pd.fuben == data.fid)
                {
                    pd.fuben += 1;
                }

                if (!cacheSvc.UpdataPlayerData(pd.id, pd))
                {
                    msg.err=(int)ErrorCode.UpdataDBError;
                }
                else
                {
                    RspFBFightEnd rspFbFightEnd = new RspFBFightEnd()
                    {
                        isWin = data.isWin,
                        fid = data.fid,
                        costTime = data.costTime,
                        restHP = data.restHP,
                        coin=pd.coin,
                        lv=pd.lv,
                        crystal=pd.crystal,
                        exp=pd.exp,
                        fuben = pd.fuben,
                    };
                    msg.rspFBFightEnd = rspFbFightEnd;
                }
            }
        }
        else
        {
            msg.err = (int)ErrorCode.ClientDataError;
        }
        msgPack.session.SendMsg(msg);
    }
}