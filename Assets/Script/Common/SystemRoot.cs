/****************************************************
    文件：SystemRoot.cs
	作者：www34
    邮箱: 
    日期：2025/10/12 17:22:6
	功能：Nothing
*****************************************************/

using UnityEngine;

public class SystemRoot : MonoBehaviour 
{
    protected AudioSvc audioSvc;
    protected ResSvc resSvc;
    protected NetSvc netSvc;
    protected TimerSvc timerSvc;
    public virtual void InitSys()
    {
        audioSvc = AudioSvc.instance;
        resSvc = ResSvc.Instance;
        netSvc = NetSvc.instance;
        timerSvc = TimerSvc.Instance;
    }
}