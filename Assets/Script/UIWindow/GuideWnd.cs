/****************************************************
    文件：GuideWnd.cs
	作者：www34
    邮箱: 
    日期：2025/10/27 16:10:14
	功能：引导对话窗口
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class GuideWnd : WindowRoot 
{
    public Text txtName;
    public Text txtTalk;
    public Image imgIcon;
    private PlayerData pd;
    private AutoGuideCfg currenntTaskData;
    private string[] dialogArr;
    private int index;
    protected override void InitWnd()
    {
        base.InitWnd();
        pd = GameRoot.Instance.PlayerData;
        currenntTaskData = MainCitySys.Instance.GetCurrentTaskData();
        dialogArr = currenntTaskData.dialogArr.Split('#');
        index = 1;
        SetTalk();

    }
    private void SetTalk()
    {
        string[] talkArr = dialogArr[index].Split('|');
        if(talkArr[0]=="0")
        {
            //玩家
            SetSprite(imgIcon,PathDefine.SelfIcon);
            SetText(txtName, pd.name);
            
        }
        else
        {
            //npc
            switch (currenntTaskData.npcID)
            {
                case 0:
                    SetSprite(imgIcon, PathDefine.WiseManIcon);
                    SetText(txtName, "智者");
                    break;
                case 1:
                    SetSprite(imgIcon, PathDefine.GeneralIcon);
                    SetText(txtName, "将军");
                    break;
                case 2:
                    SetSprite(imgIcon, PathDefine.ArtisanIcon);
                    SetText(txtName, "工匠");
                    break;
                case 3:
                    SetSprite(imgIcon, PathDefine.TraderIcon);
                    SetText(txtName, "商人");
                    break;
                default:
                    SetSprite(imgIcon, PathDefine.GuideIcon);
                    SetText(txtName, "小芸");
                    break;

            }

        }
        imgIcon.SetNativeSize();
        SetText(txtTalk, talkArr[1].Replace("$name", pd.name));

    }
    public void ClickNextBtn()
    {
        audioSvc.PlayUIMusic(Constants.UIClickMusic);
        index += 1;
        if(index==dialogArr.Length)
        {
            //TODO 向服务器发送任务完成的消息
            GameMsg gameMsg = new GameMsg
            {
                cmd = (int)CMD.RepGuide,
                reqGuide = new ReqGuide
                {
                    guideid = currenntTaskData.ID
                }
            };
            netSvc.SendMsg(gameMsg);

            //关闭对话窗口
            SetWindowState(false);

        }
        else
        {
        SetTalk();

        }

    }
}