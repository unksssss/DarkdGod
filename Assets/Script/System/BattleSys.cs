// time:2025/11/19/15
// path:E:\unityproject\DarkGod\Assets\Script\System\BattleSys.csBattleSys.cs
// func:战斗业务系统
// www34


using PEProtocol;
using UnityEngine;

public class BattleSys : SystemRoot
{
    public static BattleSys Instance = null;
    public BattleMgr battleMgr;
    public PlayerControlWnd playerControlWnd;
    public BattleEndWnd battleEndWnd;
    private int fid;
    private double startTime;
    public override void InitSys()
    {
        base.InitSys();
        Instance = this;
        //playerData = GameRoot.Instance.PlayerData;
        PECommon.Log("BattleSys Init...");
    }

    public void StartBattle(int mapid)
    {  
        fid=mapid;
        GameObject go=new GameObject();
        go.name = "BattleRoot";
        go.transform.SetParent(GameRoot.Instance.transform);
        battleMgr = go.AddComponent<BattleMgr>();
        battleMgr.Init(mapid,()=>
        {
            startTime = timerSvc.GetCurtTime();
        });
        
        playerControlWnd.SetWindowState();
    }

    public void EndBattle(bool isWin, int restHP)
    {
        playerControlWnd.SetWindowState(false);
        GameRoot.Instance.dynamicWnd.RmvAllHPItemInfo();
        if (isWin)
        {
            double endTime = timerSvc.GetCurtTime();
            //发送结算信息
            GameMsg gameMsg= new GameMsg
            {
                cmd = (int)CMD.ReqEndFBFight,
                reqFBFightEnd = new ReqFBFightEnd
                {
                    isWin = isWin,
                    fid = fid,
                    restHP = restHP,
                    costTime = (int)((endTime - startTime)/1000)
                }

            };
            NetSvc.instance.SendMsg(gameMsg);
        }
        else
        {
            SetBattleEndWndState(FBEndType.Lose, true);
        }
    }
    public void SetSelfPlayerMoveDir(Vector2 dir)
    {
        battleMgr.SetSelfPlayerMoveDir(dir);
        //PECommon.Log(dir.ToString());
    }
    public void ReqReleaseSkill(SkillType skillType)
    {
        battleMgr.ReqReleaseSkill(skillType);
    }

    public Vector2 GetCurrentDir()
    {
        return playerControlWnd.currentDir;
    }

    public void SetBattleEndWndState(FBEndType fbEndType,bool isActive = true)
    {
        battleEndWnd.SetWndType(fbEndType);
        battleEndWnd.SetWindowState(isActive);
    }
    
    public void DestroyBattle()
    {
        playerControlWnd.SetWindowState(false);
        SetBattleEndWndState(FBEndType.None,false);
        GameRoot.Instance.dynamicWnd.RmvAllHPItemInfo();
        Destroy(battleMgr.gameObject);
        
    }

    public void RspEndFBFight(GameMsg gameMsg)
    {
        RspFBFightEnd data=gameMsg.rspFBFightEnd;
        GameRoot.Instance.SetPlayerDataByFBFightEnd(data);
        battleEndWnd.SetBattleEndData(data.fid,data.restHP,data.costTime);
        SetBattleEndWndState(FBEndType.Win);
    }
}