using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PENet;
using PEProtocol;
/********************************************************************
	created:	2025/10/15
	created:	15:10:2025   16:11
	filename: 	E:\unityproject\DarkGod\Server\Server\01Service\01NetSvc\ServerSession.cs
	file path:	E:\unityproject\DarkGod\Server\Server\01Service\01NetSvc
	file base:	ServerSession
	file ext:	cs
	author:		Author
	
	purpose:	网络会话链接
*********************************************************************/

  public  class ServerSession : PESession<GameMsg>
    {
    public int sessionID = 0;
    protected override void OnConnected()
    {
        sessionID = ServerRoot.Instance.GetSessionId();
        PECommon.Log("SessionID:"+sessionID+" Client Connect");
        
    }
    protected override void OnReciveMsg(GameMsg msg)
    {
        PECommon.Log("SessionID:" + sessionID + "  RcvPack CMD: " +((CMD)msg.cmd).ToString());
        NetSvc.Instance.AddMsgQue(new MsgPack(this,msg));

    }
    protected override void OnDisConnected()
    {
        //清除缓存
        LoginSys.Instance.CleanOffLine(this);
        PECommon.Log("SessionID:" + sessionID + "Client DisConnect");
    }
}

