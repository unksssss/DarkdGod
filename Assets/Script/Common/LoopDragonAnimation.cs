/****************************************************
    文件：LoopDragonAnimation.cs
	作者：www34
    邮箱: 
    日期：2025/10/12 17:33:20
	功能：飞龙循环
*****************************************************/

using UnityEngine;

public class LoopDragonAnimation : MonoBehaviour 
{
    private Animation animation;
    private void Awake()
    {
        animation = GetComponent<Animation>();

    }
    private void Start()
    {
        if(animation!=null)
        {
            InvokeRepeating("PlayeDragonAni", 0, 20);
        }
    }
    private void PlayeDragonAni()
    {
        if(animation!=null)
        {
            animation.Play();
        }
    }
}