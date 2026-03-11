using PEProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/********************************************************************
	created:	2025/10/16
	created:	16:10:2025   18:52
	filename: 	E:\unityproject\DarkGod\Server\Server\03Cache\CacheSvc.cs
	file path:	E:\unityproject\DarkGod\Server\Server\03Cache
	file base:	CacheSvc
	file ext:	cs
	author:		Author
	
	purpose:	本地缓存层
*********************************************************************/

public class CacheSvc
{
    private DBMgr dBMgr;
    private static CacheSvc instance = null;
    public static CacheSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CacheSvc();
            }
            return instance;
        }
    }
    public void Init()
    {
        dBMgr = DBMgr.Instance;
        PECommon.Log("CacheSvc Init Done");

    }
    private Dictionary<string, ServerSession> onlineAcctDic = new Dictionary<string, ServerSession>();
    private Dictionary<ServerSession, PlayerData> onlineSessionDic = new Dictionary<ServerSession, PlayerData>();

    public Dictionary<ServerSession, PlayerData> OnlineSessionDic => onlineSessionDic;
    
    public bool IsAcctOnline(string acct)
    {
        return onlineAcctDic.ContainsKey(acct);
    }

    public List<ServerSession> GetOnlineServerSessions()
    {
        List<ServerSession> list = new List<ServerSession>();
        foreach (var VARIABLE in onlineSessionDic)
        {
            list.Add(VARIABLE.Key);
        }
        return list;
    }
    /// <summary>
    /// 根据账号密码获取玩家信息，
    /// 密码错误返回空
    /// 账号不存在，默认创建新账号
    /// </summary>
    public PlayerData GetPlayerData(string acct, string pass)
    {
        //TODO
        //从数据库中拿到玩家数据
        return dBMgr.QueryPlayerData(acct, pass);
    }

    public ServerSession GetServerSession(int pd_id)
    {
        ServerSession serverSession = null;
        foreach (var VARIABLE in onlineSessionDic)
        {
            if (pd_id != VARIABLE.Value.id) continue;
            serverSession = VARIABLE.Key;
            break;
        }
        return serverSession;
    }
    /// <summary>
    /// 账号上线，缓存本地
    /// </summary>
    public void AcctOnline(string acct, ServerSession serverSession, PlayerData playerData)
    {
        onlineAcctDic.Add(acct, serverSession);
        onlineSessionDic.Add(serverSession, playerData);

    }
    public bool IsNameExist(string name)
    {
        return dBMgr.QueryNameData(name);
    }
    //从缓存层获取在先玩家的数据
    public PlayerData GetPlayerDataBySession(ServerSession serverSession)
    {
        if (onlineSessionDic.TryGetValue(serverSession, out PlayerData playerdata))
        {
            return playerdata;
        }
        else
        {
            return null;
        }
    }
    public bool UpdataPlayerData(int id, PlayerData playerdata)
    {
        dBMgr.UpdataPlayerData(id, playerdata);

        return true;
    }
    public void AcctOffLine(ServerSession session)
    {
        foreach (var item in onlineAcctDic)
        {
            if (item.Value == session)
            {
                onlineAcctDic.Remove(item.Key);
                break;
            }
        }
        bool succ= onlineSessionDic.Remove(session);
        PECommon.Log("Session ID:" + session.sessionID + "offline result" + succ);

    }
}

 