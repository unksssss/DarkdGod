/****************************************************
    文件：ChatWnd.cs
	作者：www34
    邮箱: 
    日期：2025/11/6 14:33:18
	功能：聊天界面
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public enum ChatType
{
	World,
	Guild,
	Friend,
}

public class ChatWnd : WindowRoot 
{
	#region UIDefine
	public InputField inputField;
	public Image imgWorld;
	public Image imgGuild;
	public Image imgFriend;
	public Text txtChat;
	#endregion
	private ChatType type;
	private List<String> chatList=new List<String>();
	private bool canSend = true;
	protected override void InitWnd()
	{
		base.InitWnd();
		type=ChatType.World;
		RefreshUi();
	}

	public void AddChat(string name, string chat)
	{
		chatList.Add(Constants.SetTxtColor(name+"：",TxtColor.Blue)+chat);
		if (chatList.Count > 12)
		{
			chatList.RemoveAt(0);
		}
		if (GetWindowState())
		{
			RefreshUi();
		}
	}
	
	private void RefreshUi()
	{
		string chatMsg="";
		switch (type)
		{
			case ChatType.World:
				for (int i = 0; i < chatList.Count; i++)
				{
					chatMsg+=chatList[i]+"\n";
				}
				SetText(txtChat,chatMsg);
				SetSprite(imgWorld,PathDefine.btnType1);
				SetSprite(imgGuild,PathDefine.btnType2);
				SetSprite(imgFriend,PathDefine.btnType2);
				break;
			case ChatType.Guild:
				SetText(txtChat,"尚未加入工会");
				SetSprite(imgWorld,PathDefine.btnType2);
				SetSprite(imgGuild,PathDefine.btnType1);
				SetSprite(imgFriend,PathDefine.btnType2);
				break;
			case ChatType.Friend:
				SetText(txtChat,"暂无好友信息");
				SetSprite(imgWorld,PathDefine.btnType2);
				SetSprite(imgGuild,PathDefine.btnType2);
				SetSprite(imgFriend,PathDefine.btnType1);
				break;
		}
	}

	public void ClickSendBtn()
	{
		if (!canSend)
		{
			GameRoot.AddTips("消息每5秒才能发送一次");
			return;
		}
		if (inputField.text == null && inputField.text == "" && inputField.text == " ")
		{
			GameRoot.AddTips("消息不能为空");
			return;
		}

		if (inputField.text.Length > 12)
		{
			GameRoot.AddTips("输入信息不能超过12个字");
			return;
		}
		//发送网络消息到服务器
		GameMsg msg = new GameMsg
		{
			cmd = (int)CMD.SendChat,
			sendChat =new SendChat
			{
				chat=inputField.text
			}
		};
		inputField.text = "";
		netSvc.SendMsg(msg);
		canSend = false;
		timerSvc.AddTimeTask((i =>
		{
			canSend = true;
		}), 5, PETimeUnit.Second);
	}
	public void ClickCloseBtn()
	{
		audioSvc.PlayUIMusic(Constants.UIClickMusic);
		SetWindowState(false);
		type=ChatType.World;
	}
	public void ClickWorldBtn()
	{
		audioSvc.PlayUIMusic(Constants.UIClickMusic);
		type = ChatType.World;
		RefreshUi();
	}
	public void ClickGuildBtn()
	{
		audioSvc.PlayUIMusic(Constants.UIClickMusic);
		type = ChatType.Guild;
		RefreshUi();
		
	}
	public void ClickFriendBtn()
	{
		audioSvc.PlayUIMusic(Constants.UIClickMusic);
		type = ChatType.Friend;
		RefreshUi();
		
	}
}