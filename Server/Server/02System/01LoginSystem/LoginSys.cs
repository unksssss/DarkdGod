using PENet;
using PEProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/********************************************************************
    created:	2025/10/15
    created:	15:10:2025   15:56
    filename: 	E:\unityproject\DarkGod\Server\Server\02System\01LoginSystem\LoginSys.cs
    file path:	E:\unityproject\DarkGod\Server\Server\02System\01LoginSystem
    file base:	LoginSys
    file ext:	cs
    author:		Author

    purpose:	登录系统
*********************************************************************/

public class LoginSys
{
    private static LoginSys instance = null;
    public static LoginSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LoginSys();
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
    public void ReqLogin(MsgPack msgPack)
    {
        ReqLogin reqLoginData = msgPack.msg.reqLogin;
        //当前账号是否已上线
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspLogin,
            
        };
        // 已上线：返回错误信息
        if(cacheSvc.IsAcctOnline(reqLoginData.acct))
        {
            //已上线，不允许重复上线返回错误信息
            msg.err = (int)ErrorCode.AcctIsOnline;
        }
        //未上线：
        else
        {
            //账号是否存在
            PlayerData _playerData = cacheSvc.GetPlayerData(reqLoginData.acct, reqLoginData.pass);
            if(_playerData==null)
            {
                //存在，密码错误未获取到玩家信息
                msg.err = (int)ErrorCode.WrongPass;
            }
            else
            {
                //密码正确，向客户端返回玩家信息
                PowerSystem.Instance.CalcuOffLinePowerAdd(_playerData);
                RspLogin rspLogin = new RspLogin
                {
                    playerData= _playerData,
                };
                msg.rspLogin= rspLogin;
                //缓存
                cacheSvc.AcctOnline(reqLoginData.acct, msgPack.session, _playerData);
            }
            //存在 检测密码
            //不存在 创建默认账号密码
            //回应服务器

        }
        //回应客户端
        msgPack.session.SendMsg(msg);
    }
    public void ReqRename(MsgPack msgPack)
    {
        ReqRename reqRename = msgPack.msg.reqRename;
        //向客户端发送的消息
        GameMsg gameMsg = new GameMsg()
        {
            cmd=(int)CMD.RspRename
        };
        //名字是否已存在
        if (cacheSvc.IsNameExist(reqRename.name))
        {
            // 存在，返回错误码
            gameMsg.err = (int)ErrorCode.NameisExist;
        }
        else
        {
            //不存在，更新缓存层和数据库，再返回给客户端
            PlayerData playerData = cacheSvc.GetPlayerDataBySession(msgPack.session);
            playerData.name = reqRename.name;
            if (!cacheSvc.UpdataPlayerData(playerData.id, playerData))
            {
                //如果更新数据库出错
                gameMsg.err = (int)ErrorCode.UpdataDBError;
            }
            else
            {
                gameMsg.rspRename = new RspRename
                {
                    name=reqRename.name,
                };
            }
        }
        msgPack.session.SendMsg(gameMsg);

    }
    public void CleanOffLine(ServerSession session)
    {
        PlayerData playerData = cacheSvc.GetPlayerDataBySession(session);
        if (playerData != null)
        {
            playerData.time = TimerSvc.Instance.GetNowTime();
            if (!cacheSvc.UpdataPlayerData(playerData.id, playerData))
            {
                PECommon.Log("Update offline time error",LogType.Error);
            }
            cacheSvc.AcctOffLine(session);
        }
    }
}