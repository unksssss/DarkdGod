/****************************************************
    文件：BuyWnd.cs
	作者：www34
    邮箱: 
    日期：2025/11/8 15:24:40
	功能：通用购买交易窗口
*****************************************************/

using System;
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;


public class BuyWnd : WindowRoot
{
	public Text txtinfo;
	public Button btnSure;
	private BuyType buyType;

	public void SetBuyType(BuyType buyType)
	{
		this.buyType = buyType;
	}
	protected override void InitWnd()
	{
		base.InitWnd();
		btnSure.interactable = true;
		RefreshUi();
	}

	private void RefreshUi()
	{
		switch (buyType)
		{
			case BuyType.Power:
				txtinfo.text = "是否花费" + Constants.SetTxtColor("10钻石", TxtColor.Blue) + "购买" +
				          Constants.SetTxtColor("100体力", TxtColor.Green)+"？";
				break;
			case BuyType.Coin:
				txtinfo.text = "是否花费" + Constants.SetTxtColor("10钻石", TxtColor.Blue) + "购买" +
				               Constants.SetTxtColor("1000金币", TxtColor.Yellow)+"？";
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public void ClickSureBtn()
	{
		audioSvc.PlayUIMusic(Constants.UIClickMusic);
		//todo 发送网络消息
		GameMsg msg = new GameMsg()
		{
			cmd = (int)CMD.ReqBuy,
			reqBuy = new ReqBuy()
			{
				buyType = buyType,
				cost = 10
			}
		};
		netSvc.SendMsg(msg);
		btnSure.interactable = false;
	}
	public void ClickCloseBtn()
	{
		audioSvc.PlayUIMusic(Constants.UIClickMusic);
		SetWindowState(false);
	}
}