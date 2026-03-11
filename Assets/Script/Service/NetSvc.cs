/****************************************************
    文件：NetSvc.cs
	作者：www34
    邮箱: 
    日期：2025/10/15 16:27:48
	功能：网络服务
*****************************************************/

using System;
using PENet;
using PEProtocol;
using System.Collections.Generic;
using UnityEngine;

public class NetSvc : MonoBehaviour
{
    public static NetSvc instance = null;
    PESocket<ClientSession, GameMsg> ClientSocket = null;
    private Queue<GameMsg> msgQue = new Queue<GameMsg>();
    private static readonly string obj = "lock";


    public void InitSvc()
    {
        instance = this;
        ClientSocket = new PESocket<ClientSession, GameMsg>();
        
        //日志接口
        ClientSocket.SetLog(true, (string msg, int level) =>
        {
            switch (level)
            {
                case 0:
                    msg = "Log:" + msg;
                    Debug.Log(msg);
                    break;
                case 2:
                    msg = "Error:" + msg;
                    Debug.LogError(msg);
                    break;
                case 1:
                    msg = "Warning:" + msg;
                    Debug.LogWarning(msg);
                    break;
                case 3:
                    msg = "Info:" + msg;
                    Debug.Log(msg);
                    break;
            }
            Debug.Log("Init NetSvc");


        });
        ClientSocket.StartAsClient(SvrCfg.svrIP, SvrCfg.svrPort);
    }
    public void SendMsg(GameMsg gameMsg)
    {
        if(ClientSocket!=null)
        {
            ClientSocket.session.SendMsg(gameMsg);
        }
        else
        {
            GameRoot.AddTips("网络连接异常");
            InitSvc();
        }
    }
    public void AddNetPkg(GameMsg msg)
    {
        lock (obj)
        {
            msgQue.Enqueue(msg);
        }
    }
    private void Update()
    {
        if(msgQue.Count>0)
        {
            lock(obj)
            {
                GameMsg gameMsg = msgQue.Dequeue();
                ProcessMsg(gameMsg);
            }
        }
    }
    private void ProcessMsg(GameMsg gameMsg)
    {
        if(gameMsg.err!=(int)ErrorCode.None)
        {
            switch((ErrorCode)gameMsg.err)
            {
                case ErrorCode.ServerDataError:
                    PECommon.Log("服务器数据异常", LogType.Error);
                    GameRoot.AddTips("客户端数据异常");
                    break;
                case ErrorCode.UpdataDBError:
                    PECommon.Log("数据库更新异常", LogType.Error);
                    GameRoot.AddTips("网络不稳定");
                    break;
                case ErrorCode.ClientDataError:
                    PECommon.Log("客户端数据异常", LogType.Error);
                    break;
                case ErrorCode.AcctIsOnline:
                    GameRoot.AddTips("账号已上线");
                    break;
                case ErrorCode.WrongPass:
                    GameRoot.AddTips("密码错误");
                    break;
                case ErrorCode.NameisExist:
                    GameRoot.AddTips("名字已存在");
                    break;
                case ErrorCode.LackLevel:
                    GameRoot.AddTips("等级不足");
                    break;
                case ErrorCode.LackCoin:
                    GameRoot.AddTips("金币不足");
                    break;
                case ErrorCode.LackCrystal:
                    GameRoot.AddTips("水晶不足");
                    break;
                case ErrorCode.LackDiamond:
                    GameRoot.AddTips("钻石不足");
                    break;
                case ErrorCode.LackPower:
                    GameRoot.AddTips("体力不足");
                    break;
                case ErrorCode.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return;
        }
        switch((CMD)gameMsg.cmd)
        {
            case CMD.RspLogin:
                LoginSys.Instance.RspLogin(gameMsg);
                break;
            case CMD.RspRename:
                LoginSys.Instance.RspRename(gameMsg);
                break;
            case CMD.RspGuide:
                MainCitySys.Instance.RspGuide(gameMsg);
                break;
            case CMD.RspStrong:
                MainCitySys.Instance.RspStrong(gameMsg);
                break;
            case CMD.PushChat:
                MainCitySys.Instance.PushChat(gameMsg);
                break;
            case CMD.RspBuy:
                MainCitySys.Instance.RspBuy(gameMsg);
                break;
            case CMD.PushPower:
                MainCitySys.Instance.PushPower(gameMsg);
                break;
            case CMD.RspTaskReward:
                MainCitySys.Instance.RspTaskReward(gameMsg);
                break;
            case CMD.PushTaskPrg:
                MainCitySys.Instance.PushTaskPrg(gameMsg);
                break;
            case CMD.RspFBFight:
                FubenSys.Instance.RspFBFight(gameMsg);
                break;
            case CMD.RspEndFBFight:
                BattleSys.Instance.RspEndFBFight(gameMsg);
                break;
        }
    }
}

