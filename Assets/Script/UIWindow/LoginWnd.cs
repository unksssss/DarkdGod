/****************************************************
    文件：LoginWnd.cs
	作者：www34
    邮箱: 
    日期：2025/10/11 15:25:11
	功能：登录界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using PEProtocol;
public class LoginWnd : WindowRoot
{
    public InputField player_account;
    public InputField player_password;
    public Button BTN_Enter;
    public Button BTN_Notice;

    protected override void  InitWnd()
    {
        base.InitWnd();
        if (PlayerPrefs.HasKey("account") && PlayerPrefs.HasKey("password"))
        {
            player_account.text = PlayerPrefs.GetString("account");
            player_password.text = PlayerPrefs.GetString("password");
        }
        else
        {
            player_account.text = "";
            player_password.text = "";
        }
        BTN_Enter.onClick.AddListener(ClickEnterBtn);
        BTN_Notice.onClick.AddListener(ClickNoticeBtn);
        
    }

    public void ClickEnterBtn()
    {
        audioSvc.PlayUIMusic(Constants.UILoginMusic);
        string _acct = player_account.text;
        string _passwd = player_password.text;
        if (_acct != "" && _passwd != "")
        {
            // 更新本地账号数据存储到本地
            PlayerPrefs.SetString("account", _acct);
            PlayerPrefs.SetString("password", _passwd);
            //ToDo 发送网络消息，请求登录
            GameMsg gameMsg = new GameMsg
            {
                cmd=(int)CMD.ReqLogin,
                reqLogin = new ReqLogin
                {
                    acct =_acct,
                    pass =_passwd
                }
            };
            netSvc.SendMsg(gameMsg);
        }
        else
        {
            GameRoot.AddTips("账号或密码为空");
        }
    }
    public void ClickNoticeBtn()
    {
        GameRoot.AddTips("Notice Buttton Click");
    }
    
}