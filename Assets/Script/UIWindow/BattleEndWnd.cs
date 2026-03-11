/****************************************************
    文件：BattleEndWnd.cs
	作者：www34
    邮箱: 
    日期：2026/2/4 15:54:54
	功能：战斗结束界面
*****************************************************/

using System;
using UnityEngine;
using UnityEngine.UI;

public class BattleEndWnd : WindowRoot 
{
	#region UIDefine

	public Transform rewardTrans;
	public Button btnClose;
	public Button btnExit;
	public Button btnSure;


	public Text txtTime;
	public Text txtRestHP;
	public Text txtReward;
	#endregion
	private FBEndType fbEndType=FBEndType.None;
	public Animation aniReward;
	protected override void InitWnd()
	{
		base.InitWnd();
		RefreshUI();
	}

	private void RefreshUI()
	{
		BattleSys.Instance.SetSelfPlayerMoveDir(Vector2.zero);
		switch (fbEndType)
		{
			case FBEndType.None:
				break;
			case FBEndType.Pause:
				SetActive(rewardTrans,false);
				SetActive(btnExit.gameObject);
				SetActive(btnClose.gameObject);
				break;
			case FBEndType.Win:
				SetActive(rewardTrans,false);
				SetActive(btnExit.gameObject,false);
				SetActive(btnClose.gameObject,false);
				MapCfg mc = resSvc.GetMapCfg(fid);
				int min = costTime / 60;
				int sec = costTime % 60;
				int coin =mc.coin;
				int exp=mc.exp;
				int crystal=mc.crystal;
				SetText(txtTime,"通关时间："+min+":"+sec);
				SetText(txtRestHP,"剩余血量："+restHP);
				SetText(txtReward,"关卡奖励："
				                  +Constants.SetTxtColor(coin+"金币",TxtColor.Yellow)
				                  +Constants.SetTxtColor(exp+"经验",TxtColor.Green)
				                  +Constants.SetTxtColor(crystal+"水晶",TxtColor.Blue));
				
				timerSvc.AddTimeTask(tid =>
				{
					SetActive(rewardTrans,true);
					aniReward.Play();
				},500);
				break;
			case FBEndType.Lose:
				SetActive(rewardTrans,false);
				SetActive(btnExit.gameObject);
				SetActive(btnClose.gameObject,false);
				audioSvc.PlayUIMusic(Constants.FBLose);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public void ClickCloseBtn()
	{
		audioSvc.PlayUIMusic(Constants.UIClickMusic);
		BattleSys.Instance.battleMgr.isPauseGame = false;
	}

	public void ClickExitBtn()
	{
		audioSvc.PlayUIMusic(Constants.UIClickMusic);
		//进入主城，销毁当前战斗
		//todo
		MainCitySys.Instance.EnterMainCity();
		BattleSys.Instance.DestroyBattle();
		SetWindowState(false);
	}

	public void ClickSureBtn()
	{
		audioSvc.PlayUIMusic(Constants.UIClickMusic);
		//进入主城，销毁当前战斗
		//todo
		//打开副本界面
		MainCitySys.Instance.EnterMainCity();
		BattleSys.Instance.DestroyBattle();
		SetWindowState(false);
		FubenSys.Instance.EnterFuben();
	}
	public void SetWndType(FBEndType fbEndType)
	{
		this.fbEndType = fbEndType;
	}

	private int fid;
	private int restHP;
	private int costTime;
	public void SetBattleEndData(int fid,int restHp,int costTime)
	{
		this.fid = fid;
		this.restHP = restHp;
		this.costTime = costTime;

	}
}

public enum FBEndType
{
	None,
	Pause,
	Win,
	Lose
}