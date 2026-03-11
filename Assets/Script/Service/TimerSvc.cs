// time:2025/11/10/16
// path:E:\unityproject\DarkGod\Assets\Script\Service\TimerSvc.csTimerSvc.cs
// func:
// www34


using System;

public class TimerSvc:SystemRoot
{
    public static TimerSvc Instance=null;
    private PETimer pt;
    public void InitSvc()
    {
        Instance = this;
        pt = new PETimer();
        //设置日志输出
        pt.SetLog((string msg) =>
        {
            PECommon.Log(msg);
        });
        PECommon.Log("Init TimerSvc");
    }

    private void Update()
    {
        pt.Update();
    }

    public int AddTimeTask(Action<int> callback, double delay, PETimeUnit timeUnit=PETimeUnit.Millisecond, int count=1)
    {
        return pt.AddTimeTask(callback, delay, timeUnit, count);
    }

    public double GetCurtTime()
    {
        return pt.GetMillisecondsTime();

    }

    public void DeleteTimeTask(int tid)
    {
        pt.DeleteTimeTask(tid);
    }
}