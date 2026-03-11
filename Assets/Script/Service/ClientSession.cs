/****************************************************
    文件：ClientSession.cs
	作者：www34
    邮箱: 
    日期：2025/10/15 16:32:42
	功能：Nothing
*****************************************************/

using UnityEngine;
using PENet;
using PEProtocol;
public class ClientSession : PESession<GameMsg>
{
    protected override void OnConnected()
    {
        PECommon.Log("Server Connect");

    }
    protected override void OnReciveMsg(GameMsg msg)
    {
        PECommon.Log("Server Rsp:" + (CMD)msg.cmd);
        //将链接中的信息加载到本地
        NetSvc.instance.AddNetPkg(msg);
    }
    protected override void OnDisConnected()
    {
        PECommon.Log("Server DisConnect");

    }
}