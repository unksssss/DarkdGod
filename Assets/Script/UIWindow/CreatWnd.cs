/****************************************************
    文件：CreatWnd.cs
	作者：www34
    邮箱: 
    日期：2025/10/13 16:5:38
	功能：创建角色窗口
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class CreatWnd : WindowRoot 
{
    public InputField inputField;
    public Button RDNameBtn;
    public Button EnterBtn;
    protected override void InitWnd()
    {
        base.InitWnd();
        RDNameBtn.onClick.AddListener(ClickRDNameBtn);
        EnterBtn.onClick.AddListener(ClickEnterBtn);
        inputField.text = resSvc.GetRDName(false);
        //ToDo
        //随机显示一个名字
    }
    public void ClickRDNameBtn()
    {
        AudioSvc.instance.PlayUIMusic(Constants.UIClickMusic);
        inputField.text = resSvc.GetRDName(false);
    }
    public void ClickEnterBtn()
    {
        AudioSvc.instance.PlayUIMusic(Constants.UIClickMusic);
        if (inputField.text != "")
        {
            //ToDo
            //发送名称给服务器，登录主城
            GameMsg gameMsg = new GameMsg
            {
                cmd = (int)CMD.ReqRename,
                reqRename = new ReqRename
                {
                    name = inputField.text
                }
            };
            netSvc.SendMsg(gameMsg);
        }
        else
        {
            GameRoot.AddTips("名字不能为空");
        }

    }
}