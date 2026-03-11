/****************************************************
    文件：StrongWnd.cs
	作者：www34
    邮箱: 
    日期：2025/10/28 19:4:59
	功能：强化界面
*****************************************************/

using System;
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class StrongWnd : WindowRoot 
{
    #region Ui Define
    public Button btnClose;
    public Transform posBtnTrans;


    public Image imgCurtPos;
    public Text txtStarLv;
    public Transform starLvGrp;
    public Text txtHp1;
    public Text txtHurt1;
    public Text txtDef1;
    public Text txtHp2;
    public Text txtHurt2;
    public Text txtDef2;
    public Image UpArr1;
    public Image UpArr2;
    public Image UpArr3;

    public Transform transCostInfo;
    public Text txtNeedLv;
    public Text txtNeedCoin;
    public Text txtNeedCrystal;

    public Text txtCoin;
    #endregion
    
    private Image[] images;
    //当前点击图片的下标
    private int currentIndex;
    private PlayerData playerData;
    //private StrongCfg data;
    private StrongCfg nextStrongData;
    
    protected override void InitWnd()
    {
        base.InitWnd();
        images = new Image[6];
        playerData = GameRoot.Instance.PlayerData;
        RegClickEvts();

        ClickPosItem(0);

    }
    private void RegClickEvts()
    {
        for(int i=0;i<posBtnTrans.childCount;i++)
        {
            Image image = posBtnTrans.GetChild(i).GetComponent<Image>();
            OnClick(image.gameObject, (object args) =>
             {
                 audioSvc.PlayUIMusic(Constants.UIClickMusic);
                 ClickPosItem((int)args);
             },i);
            images[i] = image;
        }
    }

    private void ClickPosItem(int index)
    {
        PECommon.Log("Click Item"+index);
        currentIndex = index;
        for(int i=0;i<images.Length;i++)
        {
            Transform trans = images[i].transform;
            if(i==currentIndex)
            {
                //当前图片为点击到的图片
                //设置箭头
                SetSprite(images[i], PathDefine.ArrowBG);
                trans.localPosition = new Vector3(10, trans.localPosition.y, 0);
                trans.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 110);

            }
            else
            {
                SetSprite(images[i], PathDefine.PlantBG);
                trans.localPosition = new Vector3(0, trans.localPosition.y, 0);
                trans.GetComponent<RectTransform>().sizeDelta = new Vector2(263, 85);

            }
        }
        RefreshItem();

    }
    private void RefreshItem()
    {
        switch(currentIndex)
        {
            case 0:
                SetSprite(imgCurtPos, PathDefine.ItemToukui);
                break;
            case 1:
                SetSprite(imgCurtPos, PathDefine.ItemBody);
                break;
            case 2:
                SetSprite(imgCurtPos, PathDefine.ItemYaobu);
                break;
            case 3:
                SetSprite(imgCurtPos, PathDefine.ItemHand);
                break;
            case 4:
                SetSprite(imgCurtPos, PathDefine.ItemLeg);
                break;
            case 5:
                SetSprite(imgCurtPos, PathDefine.ItemFoot);
                break;
        }
        int curtStarLv = playerData.strong[currentIndex];
        SetText(txtStarLv, curtStarLv + "星级");
        //设置星级图标
        for(int i=0;i<starLvGrp.childCount;i++)
        {
            Image image = starLvGrp.GetChild(i).GetComponent<Image>();
            if(i<curtStarLv)
            {
                SetSprite(image, PathDefine.ItemStar2);

            }
            else
            {
                SetSprite(image, PathDefine.ItemStar1);

            }
        }
        //设置属性词条
        /*
        */
        SetText(txtHp1,"生命+"+resSvc.GetPropSumByStarLv(currentIndex, curtStarLv, ResSvc.PropertyType.Hp));
        SetText(txtHurt1, "伤害+" + resSvc.GetPropSumByStarLv(currentIndex, curtStarLv, ResSvc.PropertyType.Hurt));
        SetText(txtDef1, "防御+" + resSvc.GetPropSumByStarLv(currentIndex, curtStarLv, ResSvc.PropertyType.Defense));
        if(curtStarLv==10)
        {
            //满级
            SetActive(txtHp2,false);
            SetActive(txtHurt2,false);
            SetActive(txtDef2,false);
            SetActive(UpArr1,false);
            SetActive(UpArr2,false);
            SetActive(UpArr3,false);
            SetActive(transCostInfo,false);
            
        }
        else
        {
            SetActive(txtHp2);
            SetActive(txtHurt2);
            SetActive(txtDef2);
            SetActive(UpArr1);
            SetActive(UpArr2);
            SetActive(UpArr3);
            SetActive(transCostInfo);
            SetText(txtHp2, "生命+" + resSvc.GetPropSumByStarLv(currentIndex, curtStarLv + 1, ResSvc.PropertyType.Hp));
            SetText(txtHurt2, "伤害+" + resSvc.GetPropSumByStarLv(currentIndex, curtStarLv + 1, ResSvc.PropertyType.Hurt));
            SetText(txtDef2, "防御+" + resSvc.GetPropSumByStarLv(currentIndex, curtStarLv + 1, ResSvc.PropertyType.Defense));
            //data = resSvc.GetStrongCfg(currentIndex, curtStarLv);
            nextStrongData=resSvc.GetStrongCfg(currentIndex, curtStarLv+1);
            //设置需要等级
            SetText(txtNeedLv, "需要等级：" + nextStrongData.minlv);
            //设置需要金币和钻石
            SetText(txtNeedCoin,"需要金币：      "+ nextStrongData.coin);
            SetText(txtNeedCrystal,+ nextStrongData.crystal+"/"+playerData.crystal);
        }
        //TODO 水晶字段
        SetText(txtCoin, playerData.coin);
    }
    public void ClickCloseBtn()
    {

        audioSvc.PlayUIMusic(Constants.UIClickMusic);

        SetWindowState(false);
    }

    public void ClickStrongBtn()
    {
        audioSvc.PlayUIMusic(Constants.UIClickMusic);
        if (playerData.strong[currentIndex] == 10)
        {
            GameRoot.AddTips("已满级");
            return;
        }
        if (playerData.lv < nextStrongData.minlv)
        {
            GameRoot.AddTips("等级不足");
            return;
        }
        if (playerData.coin < nextStrongData.coin)
        {
            GameRoot.AddTips("金币不足");
            return;
        }

        if (playerData.lv < nextStrongData.minlv)
        {
            GameRoot.AddTips("水晶不足");
            return;
        }
        netSvc.SendMsg(new GameMsg
        {
            cmd = (int)CMD.ReqStrong,
            reqStrong = new ReqStrong
            {
                pos = currentIndex
            }
        });
        
    }
    
    public void UpdateUI()
    {
        audioSvc.PlayUIMusic(Constants.FBItemEnter);
        ClickPosItem(currentIndex);
    }
}