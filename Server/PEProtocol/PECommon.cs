using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/********************************************************************
	created:	2025/10/15
	created:	15:10:2025   19:11
	filename: 	E:\unityproject\DarkGod\Server\PEProtocol\PECommon.cs
	file path:	E:\unityproject\DarkGod\Server\PEProtocol
	file base:	PECommon
	file ext:	cs
	author:		Author
	
	purpose:	客户端服务端共用工具类
*********************************************************************/
using PENet;
using PEProtocol;
public enum LogType
{
    Log=0,
    Warning=1,
    Error=2,
    Info=3
}
public class PECommon
{
    public static void Log(string text="",LogType type=LogType.Log)
    {
    LogLevel level = (LogLevel)type;
    PETool.LogMsg(text, level);
    }
    public static int GetFightByProps(PlayerData playerData)
    {
    return playerData.lv * 100 + playerData.ad + playerData.ap + playerData.addef + playerData.apdef;
    }
    public static int GetPowerLimit(PlayerData playerData)
    {
        return ((playerData.lv - 1) / 10) * 150 + 150;
    }

    public static int GetExpUpValByLv(int lv)
    {
	    return lv * 100 *lv;
    }

    public const double PowerAddSpace = 5;//间隔5分钟
    public const int PowerAddCount = 2;//增加2点体力
    public static void CalcExp(PlayerData playerData, int addExp)
    {
	    int curtLv = playerData.lv;
	    int curtExp = playerData.exp;
	    int addRestExp = addExp;
	    while (true)
	    {
		    int upNeedExp = GetExpUpValByLv(curtLv) - curtExp;
		    if (addRestExp >= upNeedExp)
		    {
			    curtLv += 1;
			    curtExp = 0;
			    addRestExp-=upNeedExp;
		    }
		    else
		    {
			    playerData.lv=curtLv;
			    playerData.exp = curtExp + addRestExp;
			    break;
		    }
	    }
    }
}

