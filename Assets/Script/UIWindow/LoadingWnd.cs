/****************************************************
    文件：LoadingWnd.cs
	作者：www34
    邮箱: 
    日期：2025/10/10 21:35:36
	功能：加载进度界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

public class LoadingWnd :WindowRoot 
{
    public Text txtTips;
    public Image imgFg;
    public Image imgPoint;
    public Text txtLoading;
    private float fgWidth;
    protected override void InitWnd()
    {
        base.InitWnd();
        //获取进度条的长度
        fgWidth = imgFg.GetComponent<RectTransform>().sizeDelta.x;
        SetText(txtTips, "这是一条游戏Tips");
        SetText(txtLoading, "0");
        imgFg.fillAmount = 0;
        imgPoint.transform.localPosition = new Vector3(-545f, 0, 0);
    }
    public void SetProgress(float progress)
    {
        SetText(txtLoading, (int)(progress * 100) + "%");
        imgFg.fillAmount= progress;
        //当前进度条的位置
        float position = fgWidth * progress - 545f;
        imgPoint.GetComponent<RectTransform>().anchoredPosition = new Vector2(position, 0);
    }

}