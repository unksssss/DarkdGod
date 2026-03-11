// time:2025/11/06/15
// path:E:\unityproject\DarkGod\Server\Server\02System\ChatSystem\ChatSystem.csChatSystem.cs
// func: 聊天系统
// www34


using System;
using System.Collections.Generic;
using PEProtocol;

public class ChatSystem
{
    private static ChatSystem instance = null;
    public static ChatSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ChatSystem();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;
    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        PECommon.Log("LoginSys Init Done");
    }

    public void SendChat(MsgPack msgPack)
    {
        SendChat data = msgPack.msg.sendChat;
        PlayerData playerData=cacheSvc.GetPlayerDataBySession(msgPack.session);
        TaskSystem.Instance.CalcuTaskPrg(playerData,6);
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.PushChat,
            pushChat = new PushChat()
            {
                name = playerData.name,
                chat = data.chat,
            }
        };
        //提前序列化消息包，优化服务端性能
        Byte[] bytes = PENet.PETool.PackNetMsg(msg);
                
        //广播聊天信息给所有的在线用户
        List<ServerSession> list = cacheSvc.GetOnlineServerSessions();
        foreach (var VARIABLE in list)
        {
            VARIABLE.SendMsg(bytes);
        }

    }
}