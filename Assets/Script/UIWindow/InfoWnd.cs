/****************************************************
    文件：InfoWnd.cs
	作者：www34
    邮箱: 
    日期：2025/10/23 13:37:15
	功能：角色信息展示界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InfoWnd : WindowRoot 
{
    #region UI Dedine
    public RawImage imgCharShow;
    public Text txtinfo;
    public Text txtExp;
    public Image imgExpPrg;
    public Text txtPower;
    public Image imgPowerPrg;
    public Text txtJob;
    public Text txtFight;
    public Text txtHp;
    public Text txtHurt;
    public Text txtDef;

    public Button btnOpenDetail;
    public Button btnCloseDetail;
    //public Button btnDetail;
    public Transform transDetail;
    public Text dtxHp;
    public Text dtxAd;
    public Text dtxAp;
    public Text dtxAddef;
    public Text dtxApdef;
    public Text dtxdodge;
    public Text dtxpierce;
    public Text dtxcritical; 

    #endregion
    private Vector2 startPos;
    protected override void InitWnd()
    {
        base.InitWnd();
        
        RefreshUI();
        RegTouchEvts();
        SetActive(transDetail, false);

    }
    private void RegTouchEvts()
    {
        OnClickDown(imgCharShow.gameObject, (PointerEventData evt) =>
         {
             startPos = evt.position;
             MainCitySys.Instance.SetPlayerStartRoate();

         });
        OnDrag(imgCharShow.gameObject, (PointerEventData evt) =>
         {
             float rote = evt.position.x - startPos.x;
             MainCitySys.Instance.SetPlayerShowRoate(rote);
         });
    }
    private void RefreshUI()
    {
        PlayerData pd = GameRoot.Instance.PlayerData;
        SetText(txtinfo, pd.name + "LV." + pd.lv);
        SetText(txtExp, pd.exp + "/" + PECommon.GetExpUpValByLv(pd.lv));
        imgExpPrg.fillAmount = pd.exp *1.0f/ PECommon.GetExpUpValByLv(pd.lv)*1.0f;
        SetText(txtPower, pd.power);
        imgPowerPrg.fillAmount = pd.power*1.0f / PECommon.GetPowerLimit(pd)*1.0f;

        SetText(txtJob, "职业  暗夜刺客");
        SetText(txtFight,"战力  "+ PECommon.GetFightByProps(pd));
        SetText(txtHp,"生命  "+ pd.hp);
        SetText(txtHurt, "伤害  " + (pd.ad + pd.ap));
        SetText(txtDef, "防御  " + (pd.addef + pd.apdef));

        //TODO detail
        SetText(dtxHp, pd.hp);
        SetText(dtxAd, pd.ad);
        SetText(dtxAp, pd.ap);
        SetText(dtxAddef, pd.addef);
        SetText(dtxApdef, pd.apdef);
        SetText(dtxdodge, pd.dodge+"%");
        SetText(dtxpierce, pd.pierce + "%");
        SetText(dtxcritical, pd.critical + "%");
    }
    public void ClickCloseBtn()
    {
        audioSvc.PlayUIMusic(Constants.UIClickMusic);
        MainCitySys.Instance.CloseInfoWnd();
    }
    public void ClickDetailBtn()
    {
        audioSvc.PlayUIMusic(Constants.UIClickMusic);
        SetActive(transDetail);
    }
    public void ClickCloseDetailBtn()
    {
        audioSvc.PlayUIMusic(Constants.UIClickMusic);
        SetActive(transDetail, false);

    }
}