using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PENet;
using PEProtocol;

/********************************************************************
    created:	2025/10/15
    created:	15:10:2025   15:48
    filename: 	E:\unityproject\DarkGod\Server\Server\01Service\01NetSvc\NetSvc.cs
    file path:	E:\unityproject\DarkGod\Server\Server\01Service\01NetSvc
    file base:	NetSvc
    file ext:	cs
    author:		Author

    purpose:	网络服务
*********************************************************************/
public class MsgPack
{
    public ServerSession session;
    public GameMsg msg;

    public MsgPack(ServerSession session, GameMsg msg)
    {
        this.session = session;
        this.msg = msg;
    }
}

public class NetSvc
{
    private static NetSvc instance = null;

    public static NetSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NetSvc();
            }

            return instance;
        }
    }

    public void Init()
    {
        PESocket<ServerSession, GameMsg> server = new PESocket<ServerSession, GameMsg>();
        server.StartAsServer(SvrCfg.svrIP, SvrCfg.svrPort);
        PECommon.Log("NetSvr Init Done");
    }

    private Queue<MsgPack> GameMsgPackQue = new Queue<MsgPack>();
    private readonly string obj = "";

    public void AddMsgQue(MsgPack msgPack)
    {
        lock (obj)
        {
            GameMsgPackQue.Enqueue(msgPack);
        }
    }

    public void Update()
    {
        if (GameMsgPackQue.Count > 0)
        {
            PECommon.Log("PackCount: " + GameMsgPackQue.Count);
            lock (obj)
            {
                MsgPack msgPack = GameMsgPackQue.Dequeue();
                HandOutMsg(msgPack);
            }
        }
    }

    public void HandOutMsg(MsgPack msgPack)
    {
        switch ((CMD)msgPack.msg.cmd)
        {
            case CMD.ReqLogin:
                LoginSys.Instance.ReqLogin(msgPack);
                break;
            case CMD.ReqRename:
                LoginSys.Instance.ReqRename(msgPack);
                break;
            case CMD.RepGuide:
                GuideSystem.Instance.ReqGuide(msgPack);
                break;
            case CMD.ReqStrong:
                StrongSystem.Instance.ReqStrong(msgPack);
                break;
            case CMD.SendChat:
                ChatSystem.Instance.SendChat(msgPack);
                break;
            case CMD.ReqBuy:
                BuySystem.Instance.ReqBuy(msgPack);
                break;
            case CMD.ReqTaskReward:
                TaskSystem.Instance.ReqTaskReward(msgPack);
                break;
            case CMD.ReqFBFight:
                FuBenSystem.Instance.ReqFBFight(msgPack);
                break;
            case CMD.ReqEndFBFight:
                FuBenSystem.Instance.ReqEndFBFight(msgPack);
                break;
        }
    }
}