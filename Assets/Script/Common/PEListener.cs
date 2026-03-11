/****************************************************
    文件：PEListener.cs
	作者：www34
    邮箱: 
    日期：2025/10/20 19:48:34
	功能：UI事件监听插件
*****************************************************/

using System;
using UnityEngine;
using UnityEngine.EventSystems;
public class PEListener : MonoBehaviour ,IPointerClickHandler,IPointerDownHandler,IPointerUpHandler,IDragHandler
{
    public Action<object> onClick = null;
    public Action<PointerEventData> onClickDown = null;
    public Action<PointerEventData> onClickUp = null;
    public Action<PointerEventData> onDrag = null;
    /// <summary>
    /// 定义args，用于标注点击事件在出差时的被注册物体信息
    /// 例如，在strongwnd中，对posBtnTrans的子物体注册OnPointerClick时有子物体的标识i
    /// 在注册点击事件时，i传给onClick()的args，并在onClick()中赋值给listener.args保存在listner对象中，
    /// 然后通过回调函数把listner中的args也就是当前子物体的下标通过事件onClick传到匿名函数中，
    /// 匿名函数在调用ClickPosItem()时即可调用匿名函数的参数args，输出点击物体的下标
    /// </summary>
    public object args;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null)
        {
            onClick(args);
        }
    }
    public void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (onClickDown != null)
        {
            onClickDown(eventData);
        }
        
    }

    public void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if(onClickUp != null)
        {
            onClickUp(eventData);
        }
    }

    public void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (onDrag != null)
        {
            onDrag(eventData);
        }
    }

}