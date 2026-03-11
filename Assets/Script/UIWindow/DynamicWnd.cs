/****************************************************
    文件：DynamicWnd.cs
    作者：www34
    邮箱:
    日期：2025/10/12 19:10:7
    功能：动态Ui元素界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class DynamicWnd : WindowRoot 
{
    public Animation tipsAnim;
    public Animation SelfDodgeAni;
    public Text txtTips;
    public Transform HPItemRoot;
    private Queue<string> tipsQue = new Queue<string>();
    private Dictionary<string,ItemEntityHP> itemDic = new Dictionary<string,ItemEntityHP>();
    private bool isTipsShow=false;
    protected override void InitWnd()
    {
        base.InitWnd();
        SetActive(gameObject, true);
    }
    public void AddTips(string tips)
    {
        lock (tipsQue)
        {
            tipsQue.Enqueue(tips);
        }
    }
    private void Update()
    {
        if(tipsQue.Count>0&&!isTipsShow)
        {
            lock(tipsQue)
            {
                SetTips(tipsQue.Dequeue());
                isTipsShow=true;
            }
        }
    }
    private void SetTips(string txt)
    {
        SetActive(txtTips);
        SetText(txtTips, txt);
        AnimationClip clip = tipsAnim.GetClip("Anima_TxtTips");
        tipsAnim.Play();
        //延时关闭激活 动画播放完成后关闭tips弹窗
        StartCoroutine(AnimPlayOver(clip.length, () =>
        {
            isTipsShow= false;
            SetActive(txtTips, false);
        }));
    }
    private IEnumerator AnimPlayOver(float sec,Action action)
    {
        yield return new WaitForSeconds(sec);   
        if(action != null)
        {
            action();
        }
    }

    public void AddHPItemInfo(string mName,Transform trans, int HP)
    {
        ItemEntityHP item = null;
        if (itemDic.TryGetValue(mName, out item))
        {
            return;
        }
        GameObject go = resSvc.LoadPrefab(PathDefine.HPItemPrefab,true);
        go.transform.SetParent(HPItemRoot);
        go.transform.localPosition = new Vector3(-1000, 0, 0);
        ItemEntityHP ieh = go.GetComponent<ItemEntityHP>();
        ieh.InitItemHp(trans,HP);
        itemDic.Add(mName, ieh);
    }

    public void RmvHPItemInfo(string mName)
    {
        ItemEntityHP item = null;
        if (!itemDic.TryGetValue(mName, out item)) return;
        Destroy(item.gameObject);
        itemDic.Remove(mName);
    }

    public void RmvAllHPItemInfo()
    {
        foreach (var itemEntityHp in itemDic)
        {
            Destroy(itemEntityHp.Value.gameObject);
        }
        itemDic.Clear();
    }
    public void SetDodge(string mName)
    {
        ItemEntityHP item = null;
        if (itemDic.TryGetValue(mName, out item))
        {
            item.SetDodge();
        }
    }
    public void SetHurt(string mName,int hurt)
    {
        ItemEntityHP item = null;
        if (!itemDic.TryGetValue(mName, out item)) return;
        item.SetHurt(hurt);
    }
    public void SetCritical(string mName,int crit)
    {
        ItemEntityHP item = null;
        if (itemDic.TryGetValue(mName, out item))
        {
            item.SetCritical(crit);
        }
    }
    public void SetHPVal(string mName,int oldVal, int newVal)
    {
        ItemEntityHP item = null;
        if (itemDic.TryGetValue(mName, out item))
        {
            item.SetHPVal(oldVal, newVal);
        }
    }

    public void SetSelfDodge()
    {
        SelfDodgeAni.Stop();
        SelfDodgeAni.Play();
    }
}