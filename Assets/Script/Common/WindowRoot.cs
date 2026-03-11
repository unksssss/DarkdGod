/****************************************************
    文件：WindowRoot.cs
	作者：www34
    邮箱: 
    日期：2025/10/12 16:13:17
	功能：Ui窗口基类
*****************************************************/

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowRoot : MonoBehaviour 
{
    protected ResSvc resSvc = null;
    protected AudioSvc audioSvc=null;
    protected NetSvc netSvc = null;
    protected TimerSvc timerSvc = null;
    public void SetWindowState(bool isActive=true)
    {
        if (gameObject.activeSelf!=isActive)
        {
            SetActive(gameObject,isActive);
        }
        if(isActive)
        {
            InitWnd();
        }
        else
        {
            ClearWnd();
        }
    }

    public bool GetWindowState()
    {
        return gameObject.activeSelf;
    }
    protected  virtual void InitWnd()
    {
        resSvc = ResSvc.Instance;
        audioSvc = AudioSvc.instance;
        netSvc = NetSvc.instance;
        timerSvc = TimerSvc.Instance;
    }
    protected virtual void ClearWnd()
    {
        resSvc = null;
        audioSvc = null;
        netSvc = null;
        timerSvc = null;
    }

    protected Transform GetTrans(Transform parent, string name)
    {
        if (parent != null)
        {
            return parent.Find(name);
        }
        else
        {
            return transform.Find(name);
        }
    }
#region SetActive
    protected void SetActive(GameObject go,bool isActive=true)
    {
        go.SetActive(isActive);
    }
    protected void SetActive(Transform trans, bool state = true)
    { 
        trans.gameObject.SetActive(state);
    }
    protected void SetActive(RectTransform rectTrans, bool state = true)
    {
        rectTrans.gameObject.SetActive(state);
    }
    protected void SetActive(Image img, bool state = true) 
    {
        img.transform.gameObject.SetActive(state); 
    }
    protected void SetActive(Text txt, bool state = true)
    { 
        txt.transform.gameObject.SetActive(state); 
    }
#endregion
#region SetText
    protected void SetText(Text text,string txt="")
    {
        text.text = txt;
    }
    protected void SetText(Transform trans, int num = 0) 
    {
        SetText(trans.GetComponent<Text>(), num);
    }
    protected void SetText(Transform trans, string context = "") 
    { 
        SetText(trans.GetComponent<Text>(), context);
    }
    protected void SetText(Text txt, int num = 0) 
    { 
        SetText(txt, num.ToString());
    }
    #endregion
    #region Click Events
    protected T AddOrGetComponent<T>(GameObject go) where T : Component
    {
        T t = go.GetComponent<T>();
        if (t == null)
        {
            t = go.AddComponent<T>();
        }
        return t;
    }
    protected void OnClick(GameObject go, Action<object> cb,object args)
    {
        PEListener listener = AddOrGetComponent<PEListener>(go);
        listener.onClick = cb;
        listener.args = args;
    }
    protected void OnClickDown(GameObject go, Action<PointerEventData> cb)
    {
        PEListener listener = AddOrGetComponent<PEListener>(go);
        listener.onClickDown = cb;
    }
    protected void OnClickUp(GameObject go, Action<PointerEventData> cb)
    {
        PEListener listener = AddOrGetComponent<PEListener>(go);
        listener.onClickUp = cb;
    }
    protected void OnDrag(GameObject go, Action<PointerEventData> cb)
    {
        PEListener listener = AddOrGetComponent<PEListener>(go);
        listener.onDrag = cb;
    }
    #endregion
    #region SetSprite
    protected void SetSprite(Image img,string path)
    {
        Sprite sp = resSvc.LoadSprite(path, true);
        img.sprite = sp;
    }
    #endregion
}