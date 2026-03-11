/****************************************************
    文件：LoginSys.cs
	作者：www34
    邮箱: 
    日期：2025/10/10 20:59:50
	功能：登录系统
*****************************************************/

using PEProtocol;
using UnityEngine;

public class LoginSys : SystemRoot 
{
    public  static LoginSys Instance=null;
    public LoginWnd LoginWnd;
    public CreatWnd creatWnd;
    public override void InitSys()
    {
        base.InitSys();
        Instance = this;
        PECommon.Log("LoginSys Init...");
    }
    /// <summary> 
    /// 进入登入界面
    /// </summary>
    public void EnterLogin()
    {
        //ToDo
        //异步加载登录场景
        resSvc.AsyncLoadScene(Constants.SceneLogin, () =>
        {
            LoginWnd.SetWindowState();
            audioSvc.PlayBGMuic(Constants.BGLoginMusic,true);
            GameRoot.AddTips("加载完成");
        });

        //并显示加载的进度

        //加载完成后打开注册登录界面
    }
    public void RspLogin(GameMsg gameMsg)
    {
        GameRoot.AddTips("登录成功");
        //玩家数据保存本地
        GameRoot.Instance.SetPlayerData(gameMsg.rspLogin);
        //当玩家信息为空时，进入注册界面
        if(gameMsg.rspLogin.playerData.name=="")
        {
            creatWnd.SetWindowState(true);

        }
        else
        {
            //当有玩家信息时，直接进入主城
            MainCitySys.Instance.EnterMainCity();

        }
        //关闭登录界面
        LoginWnd.SetWindowState(false);
        //打开创建角色界面
    }
    public void RspRename(GameMsg gameMsg)
    {
        //更新本地玩家信息
        GameRoot.Instance.SetPlayerName(gameMsg.rspRename.name);
        //TODO
        //打开主城
        MainCitySys.Instance.EnterMainCity();
        //关闭创建角色界面
        creatWnd.SetWindowState(false);

    }

}