/****************************************************
    文件：GameRoot.cs
    作者：www34
    邮箱:
    日期：2025/10/10 20:57:20
    功能：游戏启动入口
*****************************************************/

using System;
using PEProtocol;
using UnityEngine;

public class GameRoot : MonoBehaviour
{
    public static GameRoot Instance;
    public LoadingWnd loadingWnd;
    public DynamicWnd dynamicWnd;
    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this);
        PECommon.Log("GameStart.....");

        ClearWnd();

        Init();
    }
    private void ClearWnd()
    {
        Transform canvas = transform.Find("Canvas");
        for (int i = 0; i < canvas.childCount; i++)
        {
            canvas.GetChild(i).gameObject.SetActive(false);
        }
        //dynamicWnd.gameObject.SetActive(true);
    }
    private void Init()
    {
        //服务模块初始化
        NetSvc netSvc = GetComponent<NetSvc>();
        netSvc.InitSvc();
        ResSvc res = GetComponent<ResSvc>();
        res.InitSvc();
        AudioSvc audioSvc = GetComponent<AudioSvc>();
        audioSvc.InitSvc();
        TimerSvc timerSvc = GetComponent<TimerSvc>();
        timerSvc.InitSvc();
        
        //业务模块初始化
        LoginSys login = GetComponent<LoginSys>();
        login.InitSys();
        MainCitySys mainCitySys = GetComponent<MainCitySys>();
        mainCitySys.InitSys();
        FubenSys fubenSys = GetComponent<FubenSys>();
        fubenSys.InitSys();
        BattleSys battleSys = GetComponent<BattleSys>();
        battleSys.InitSys();
        
        
        dynamicWnd.SetWindowState();
        //进入登录场景并加载对应UI
        login.EnterLogin();
        //AddTips("开始游戏");
        //AddTips("开始游戏1");
    }
    public static void AddTips(string text)
    {
        Instance.dynamicWnd.AddTips(text);
    }
    private PlayerData playerData = null;
    public PlayerData PlayerData
    {
        get
        {
            return playerData;
        }
    }
    public void SetPlayerData(RspLogin rspLogin)
    {
        playerData = rspLogin.playerData;
    }
    public void SetPlayerName(string name)
    {
        playerData.name = name;
    }
    public void SetPlayerDataByGuide(RspGuide data)
    {
        playerData.coin = data.coin;
        playerData.exp = data.exp;
        playerData.lv = data.lv;
        playerData.guideid = data.guideid;
    }

    public void SetPlayerDataByStrong( RspStrong data)
    {
        playerData.coin = data.coin;
        playerData.crystal=data.crystal;
        playerData.strong = data.strongArr;
        playerData.ad=data.ad;
        playerData.ap=data.ap;
        playerData.addef=data.addef;
        playerData.apdef=data.apdef;
    }

    public void SetPlayerDataByBuy(RspBuy data)
    {
        playerData.diamond=data.diamond;
        playerData.coin=data.coin;
        playerData.power=data.power;
    }

    public void SetPlayerDataByPushPower(PushPower data)
    {
        playerData.power=data.power;
    }
    public void SetPlayerDataByTaskReward(RspTaskReward data)
    {
        playerData.coin = data.coin;
        playerData.exp = data.exp;
        playerData.lv = data.lv;
        playerData.taskArr = data.taskArr;
    }
    public void SetPlayerDataByPushTaskPrg(PushTaskPrg data)
    {
        playerData.taskArr = data.taskArr;
    }

    public void SetPlayerDataByFBFight(RspFBFight data)
    {
        playerData.power=data.power;
        
    }

    public void SetPlayerDataByFBFightEnd(RspFBFightEnd data)
    {
        playerData.coin=data.coin;
        playerData.exp=data.exp;
        playerData.lv=data.lv;
        playerData.crystal=data.crystal;
        playerData.fuben=data.fuben;
    }
    
}