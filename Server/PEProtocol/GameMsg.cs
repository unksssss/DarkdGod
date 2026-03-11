using System;
using PENet;
/********************************************************************
    created:	2025/10/15
    created:	15:10:2025   16:03
    filename: 	E:\unityproject\DarkGod\Server\PEProtocol\Class1.cs
    file path:	E:\unityproject\DarkGod\Server\PEProtocol
    file base:	Class1
    file ext:	cs
    author:		Author

    purpose:	网络通信协议,数据包
*********************************************************************/
namespace PEProtocol
{
    [Serializable]
    public class GameMsg : PEMsg
    {
        public PushChat pushChat=null;
        public ReqBuy reqBuy=null;
        public ReqGuide reqGuide=null;
        public ReqLogin reqLogin=null;
        public ReqRename reqRename = null;
        public ReqStrong reqStrong=null;
        public RspBuy rspBuy=null;
        public RspGuide rspGuide=null;
        public RspLogin rspLogin=null;
        public RspRename rspRename=null;
        public RspStrong rspStrong=null;
        public SendChat sendChat=null;
        public PushPower pushPower=null;
        
        public ReqTaskReward reqTaskReward=null;
        public RspTaskReward rspTaskReward=null;
        public PushTaskPrg pushTaskPrg=null;
        public ReqFBFight reqFBFight=null;
        public RspFBFight rspFBFight=null;
        public ReqFBFightEnd reqFBFightEnd=null;
        public RspFBFightEnd rspFBFightEnd=null;
        public string text=null;
    }
    /// <summary>
    /// CMD:数据类型
    /// </summary>
    public enum CMD
    {
        None=0,
        ReqLogin=101,
        RspLogin=102,


        ReqRename=103,
        RspRename=104,
        
        
        RepGuide=201,
        RspGuide=202,
        
        ReqStrong=203,
        RspStrong=204,
        
        SendChat=205,
        PushChat=206,
        
        ReqBuy=207,
        RspBuy=208,
        
        PushPower=209,
        
        ReqTaskReward=210,
        RspTaskReward=211,
        PushTaskPrg=212,
        
        
        ReqFBFight=301, 
        RspFBFight=302,
        
        ReqEndFBFight=303,
        RspEndFBFight=304,
    }
    [Serializable]
    public class PlayerData
    {
        public int ad;
        public int addef;//物理防御
        public int ap;
        public int apdef;//法术防御
        public int coin;
        public int critical;//暴击概率
        public int crystal;
        public int diamond;
        public int dodge;//闪避概率
        public int exp;

        public int guideid;

        public int hp;
        public int id;
        public int lv;
        public string name;
        public int pierce;//穿透比率

        public int power;

        public int[] strong;

        public long time;

        public string[] taskArr;

        public int fuben;
        //TODO
    }

    #region 登录相关
    [Serializable]
    public class ReqLogin
    {
        public string acct;
        public string pass;
    }
    [Serializable]
    public class ReqRename
    {
        public string name;
    }

    [Serializable]
    public class ReqStrong
    {
        public int pos;
    }

    [Serializable]
    public class RspStrong
    {
        public int ad;
        public int addef;
        public int ap;
        public int apdef;
        public int coin;
        public int crystal;
        public int hp;
        public int[] strongArr;
    }
    [Serializable]
    public class RspLogin
    {
        //向客户端回应玩家数据
        public PlayerData playerData;
        //TODO
    }
    [Serializable]
    public class RspRename
    {
        public string name;
    }
    #endregion
    #region 引导相关
    [Serializable] 
    public class ReqGuide
    {
        public int guideid;
    }
    [Serializable]
    public class RspGuide
    {
        public int coin;
        public int exp;
        public int guideid;
        public int lv;
    }

    #endregion

    #region 聊天相关
    [Serializable]
    public class SendChat
    {
        public string chat;
    }
    [Serializable]
    public class PushChat
    {
        public string chat;
        public string name;
    }
        
    #endregion

    #region 资源交易
        
    public enum BuyType
    {
        Power,
        Coin
    }
    [Serializable]
    public class ReqBuy
    {
        public BuyType buyType;
        public int cost;
    }

    [Serializable]
    public class RspBuy
    {
        public BuyType buyType;
        public int coin;
        public int diamond;
        public int power;
    }
    #endregion

    #region 体力增长
    [Serializable]
    public class PushPower
    {
        public int power;
    }

    #endregion

    #region 任务奖励

    [Serializable]
    public class ReqTaskReward
    {
        public int rid;
    }
    [Serializable]
    public class RspTaskReward
    {
        public int coin;
        public int exp;
        public int lv;
        public string[]  taskArr;
    }

    [Serializable]
    public class PushTaskPrg
    {
        public string[]  taskArr;
    }
    #endregion

    #region 副本战斗
    [Serializable]
    public class ReqFBFight
    {
        public int fid;
    }
    [Serializable]
    public class RspFBFight
    {
        public int fid;
        public int power;
        
    }

    [Serializable]
    public class ReqFBFightEnd
    {
        public int fid;
        public bool isWin;
        public int restHP;
        public int costTime;
    }

    [Serializable]
    public class RspFBFightEnd
    {
        public int fid;
        public bool isWin;
        public int restHP;
        public int costTime;
        //副本奖励
        public int coin;
        public int lv;
        public int exp;
        public int crystal;
        public int fuben;

    }
    #endregion
    public enum ErrorCode
    {
        None=0,//没有错误
        ServerDataError,//服务端数据异常
        UpdataDBError,//更新数据库出错
        ClientDataError,//客户端数据异常
        AcctIsOnline,//账号已上线
        WrongPass,//密码错误
        NameisExist,//名字已存在
            
        LackLevel,
        LackCoin,
        LackCrystal,
        LackDiamond,
        LackPower
    }
    public class SvrCfg
    {
        public const string svrIP = "127.0.0.1";
        public const int svrPort = 17666;
    }
    
}