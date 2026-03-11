using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/********************************************************************
    created:	2025/10/15
    created:	15:10:2025   15:48
    filename: 	E:\unityproject\DarkGod\Server\Server\00Common\ServerRoot.cs
    file path:	E:\unityproject\DarkGod\Server\Server\00Common
    file base:	ServerRoot
    file ext:	cs
    author:		www34

    purpose:	服务器初始化
*********************************************************************/

public class ServerRoot
{
    private static ServerRoot instance = null;
    public static ServerRoot Instance
    {
        get
        {
            if(instance==null)
            {
                instance = new ServerRoot();
            }
            return instance;
        }
    }
    public void Init()
    {
        //数据层 TODO
        DBMgr.Instance.Init();
        //服务层
        NetSvc.Instance.Init();
        CacheSvc.Instance.Init();
        CfgSvc.Instance.Init();
        TimerSvc.Instance.Init();
        //业务层
        LoginSys.Instance.Init();
        GuideSystem.Instance.Init();
        StrongSystem.Instance.Init();
        ChatSystem.Instance.Init();
        BuySystem.Instance.Init();
        PowerSystem.Instance.Init();
        TaskSystem.Instance.Init();
        FuBenSystem.Instance.Init();
        //
    }
    public void Update()
    {
        NetSvc.Instance.Update();
        TimerSvc.Instance.Update();
    }
    private int Sessionid = 0;
    public int GetSessionId()
    {
        if (Sessionid == int.MaxValue)
        {
            Sessionid = 0;
        }
        return Sessionid += 1;
    }
}