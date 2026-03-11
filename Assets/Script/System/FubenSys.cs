using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;


class FubenSys:SystemRoot
{
    
    public static FubenSys Instance = null;
    public FubenWnd fubenWnd;
    //PlayerData  playerData;
    public override void InitSys()
    {
        base.InitSys();
        Instance = this;
        //playerData = GameRoot.Instance.PlayerData;
        PECommon.Log("FubenSys Init...");
    }

    public void EnterFuben()
    {
        OpenFuBenWnd();

    }
    #region FuBenWnd
    public void OpenFuBenWnd()
    {
        fubenWnd.SetWindowState();
    }
    #endregion

    public void RspFBFight(GameMsg gameMsg)
    {
        RspFBFight data = gameMsg.rspFBFight;
        GameRoot.Instance.SetPlayerDataByFBFight(data);
        MainCitySys.Instance.mainCityWnd.SetWindowState(false);
        fubenWnd.SetWindowState(false);
        //
        BattleSys.Instance.StartBattle(data.fid);
        

    }
}