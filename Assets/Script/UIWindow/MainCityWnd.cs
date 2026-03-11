/****************************************************
    文件：MainCityWnd.cs
	作者：www34
    邮箱: 
    日期：2025/10/19 14:26:59
	功能：主城ui窗口
*****************************************************/

using PEProtocol;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainCityWnd : WindowRoot
{
    //屏幕缩放比
    private float globalRate;
    #region UIDefine
    public Image imgTouch;
    public Image imgDirbg;
    public Image imgDirPoint;

    public Button menuBtn;
    public Animation menuAni;
    public Text txtFight;
    public Text txtPower;
    public Image imgPowerPrg;
    public Text txtLevel;
    public Text txtName;
    public Text txtExpPrg;
    public Transform exptransform;
    public Button btnGuide;
    public Button btnChat;
    #endregion
    private bool menuState = true;
    private Vector2 startPos;
    private Vector2 defaultPos;
    private float pointerDis;
    private AutoGuideCfg currentTaskData;
    private void Update()
    {
        RefreshUI();

    }
    #region MainFunc
    protected override void InitWnd()
    {
        base.InitWnd();
        defaultPos = imgDirbg.transform.position;
        SetActive(imgDirPoint, false);
        RegisterTouchEvts();

        RefreshUI();
        pointerDis = Constants.ScreenOpDis / globalRate;

    }
    public void RefreshUI()
    {
        PlayerData playerData = GameRoot.Instance.PlayerData;
        SetText(txtFight, PECommon.GetFightByProps(playerData));
        SetText(txtPower, "体力" + playerData.power + "/" + PECommon.GetPowerLimit(playerData));
        imgPowerPrg.fillAmount = playerData.power * 1.0f / PECommon.GetPowerLimit(playerData);
        SetText(txtLevel, playerData.lv);
        SetText(txtName, playerData.name);
        //expPrg
        #region 经验条
        int expPrgVal =(int)(playerData.exp *1.0f/ PECommon.GetExpUpValByLv(playerData.lv)*100);
        SetText(txtExpPrg, expPrgVal + "%");
        int index = expPrgVal / 10;

        GridLayoutGroup gridLayoutGroup = exptransform.gameObject.GetComponent<GridLayoutGroup>();
        globalRate = 1.0f * Constants.WindowStandardHeight / Screen.height;
        float screenWidth = globalRate * Screen.width;
        float width = (screenWidth - 186) / 10;
        gridLayoutGroup.cellSize = new Vector2(width, 7);

        for(int i=0;i<exptransform.childCount;i++)
        {
            Image image = exptransform.GetChild(i).GetComponent<Image>();
            if (i < index)
            {
                image.fillAmount = 1;
            }
            else if (i == index)
            {
                image.fillAmount = expPrgVal % 10 * 1.0f / 10;
            }
            else
            {
                image.fillAmount = 0;
            }
        }
        #endregion
        #region 摇杆
        pointerDis = Constants.ScreenOpDis / globalRate;
        #endregion
        #region AutoGuideTask

        currentTaskData = resSvc.GetGuideCfg(playerData.guideid);
        if (currentTaskData != null)
        {
            SetGuideBtnIcon(currentTaskData.npcID);

        }
        else
        {
            SetGuideBtnIcon(-1);

        }
        #endregion
    }
    //任务系统图标更新
    private void SetGuideBtnIcon(int npcID)
    {
        string spPath = "";
        Image img = btnGuide.GetComponent<Image>();
        switch (npcID)
        {
            case Constants.NPCWiseman:
                spPath = PathDefine.WiseManHead;
                break;
            case Constants.NPCGeneral:
                spPath = PathDefine.GeneralHead;
                break;
            case Constants.NPCArtisan:
                spPath = PathDefine.ArtisanHead;
                break;
            case Constants.NPCTrader:
                spPath = PathDefine.TraderHead;
                break;
            default:
                spPath = PathDefine.TaskHead;
                break;

        }
        SetSprite(img, spPath);

    }
    #endregion
    #region ClickEvent
    public void ClickFubenBtn()
    {
        audioSvc.PlayUIMusic(Constants.UIOpenPage);
        MainCitySys.Instance.EnterFuben();

    }
    public void ClickBuyPowerBtn()
    {
        audioSvc.PlayUIMusic(Constants.UIOpenPage);
        MainCitySys.Instance.OpenBuyWnd(BuyType.Power);
    }
    public void ClickMKCoinBtn()
    {
        audioSvc.PlayUIMusic(Constants.UIOpenPage);
        MainCitySys.Instance.OpenBuyWnd(BuyType.Coin);
    }
    public void ClickChatBtn()
    {
        audioSvc.PlayUIMusic(Constants.UIOpenPage);
        MainCitySys.Instance.OpenChatWnd();
    }
    public void ClickGuideBtn()
    {
        audioSvc.PlayUIMusic(Constants.UIClickMusic);
        if (currentTaskData != null)
        {
            MainCitySys.Instance.RunTask(currentTaskData);

        }
        else
        {
            GameRoot.AddTips("更多引导任务，正在开发");
        }
    }
    public void ClickBtnMenu()
    {
        audioSvc.PlayUIMusic(Constants.UIExtenMusic);
        menuState = !menuState;
        AnimationClip clip = null;
        if (menuState)
        {
            clip = menuAni.GetClip("OpenMCMenu");

        }
        else
        {
            clip = menuAni.GetClip("CloseMCMenu");

        }
        menuAni.Play(clip.name);
    }
    public void ClickHeadBtn()
    {
        audioSvc.PlayUIMusic(Constants.UIOpenPage); 
        MainCitySys.Instance.OpenInfoWnd();

    }
    public void ClickStrongBtn()
    {
        audioSvc.PlayUIMusic(Constants.UIOpenPage);
        MainCitySys.Instance.OpenStrongWnd();

    }

    public void ClickTaskBtn()
    {
        audioSvc.PlayUIMusic(Constants.UIOpenPage);
        MainCitySys.Instance.OpenTaskWnd();

    }
    public void RegisterTouchEvts()
    {
        OnClickDown(imgTouch.gameObject, (PointerEventData evt) =>
        {
            startPos = evt.position;
            SetActive(imgDirPoint);
            imgDirbg.transform.position = evt.position;
        });
        OnClickUp(imgTouch.gameObject, (PointerEventData evt) =>
        {
            imgDirbg.transform.position = defaultPos;
            SetActive(imgDirPoint, false);
            imgDirPoint.transform.localPosition = Vector2.zero;
            MainCitySys.Instance.SetMove(Vector2.zero);
        });
        OnDrag(imgTouch.gameObject, (PointerEventData evt) =>
        {
            //计算方向
            Vector2 dir = evt.position - startPos;
            //取长度
            float len = dir.magnitude;

           //限制范围
            if(len> pointerDis)
            {
                Vector2 clampDir = Vector2.ClampMagnitude(dir, pointerDis);
                imgDirPoint.transform.position = startPos + clampDir;
            }
            else
            {
                imgDirPoint.transform.position = evt.position;
            }
            MainCitySys.Instance.SetMove(dir.normalized);
        });
    }
    #endregion


}