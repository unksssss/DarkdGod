/****************************************************
    文件：FubenWnd.cs
    作者：www34
    邮箱:
    日期：2025/11/16 15:14:43
    功能：副本选择界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class FubenWnd : WindowRoot 
{
    #region UiDefine

    public Button[] btns;
    public Transform pointerTrans;
    
    #endregion
    PlayerData  playerData;
    protected override void InitWnd()
    {
        base.InitWnd();
        playerData=GameRoot.Instance.PlayerData;
        RefreshUi();
        
    }

    public void RefreshUi()
    {
        int fuben = playerData.fuben;
        for (int i = 0; i < btns.Length; i++)
        {
            if (i < fuben % 10000)
            {
                SetActive(btns[i].gameObject);
                if (i != fuben % 10000 - 1) continue;
                SetActive(btns[i].gameObject);
                pointerTrans.SetParent(btns[i].transform);
                pointerTrans.localPosition = new Vector3(25, 100, 0);
            }
            else
            {
                SetActive(btns[i].gameObject, false);
            }
        }
    }
    public void ClickCloseBtn()
    {
        audioSvc.PlayUIMusic(Constants.UIClickMusic);
        SetWindowState(false);
    }
    
    public void ClickTaskBtn(int fid)
    {
        audioSvc.PlayUIMusic(Constants.UIClickMusic);
        //SetWindowState(false);
        int power = resSvc.GetMapCfg(fid).power;
        if (playerData.power < power)
        {
            GameRoot.AddTips("体力不足");
            return;
        }
        //todo 发送网络消息
        netSvc.SendMsg(new GameMsg
        {
            cmd = (int)CMD.ReqFBFight,
            reqFBFight = new ReqFBFight
            {
                fid = fid
            }
        });
    }
}