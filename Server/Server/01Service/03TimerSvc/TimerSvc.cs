// time:2025/11/10/17
// path:E:\unityproject\DarkGod\Server\Server\01Service\03TimerSvc\TimerSvc.csTimerSvc.cs
// func:服务端计时器
// www34


using System;
using System.Collections.Generic;

public class TimerSvc
{
    class TaskPack
    {
        public int tid;
        public Action<int> cb;
        public TaskPack(int tid, Action<int> cb)
        {
            this.tid = tid;
            this.cb = cb;
        }
    }
    private PETimer pt;
    private Queue<TaskPack> taskQue = new Queue<TaskPack>();
    private static readonly string taskQueLock = "taskQueLock";
    private static TimerSvc instance = null;
    public static TimerSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TimerSvc();
            }
            return instance;
        }
    }
    public void Init()
    {
        pt = new PETimer(100);
        pt.SetLog(s =>
        {
            PECommon.Log(s);
        });
        pt.SetHandle((cb, tid) =>
        {
            if (cb != null)
            {
                lock (taskQueLock)
                {
                    taskQue.Enqueue(new TaskPack(tid, cb));
                }
            }
        });
        PECommon.Log("TimerSvc Init Done");

    }
    public int AddTimeTask(Action<int> callback, double delay, PETimeUnit timeUnit=PETimeUnit.Millisecond, int count=1)
    {
        return pt.AddTimeTask(callback, delay, timeUnit, count);
    }

    public long GetNowTime()
    {
        return (long)pt.GetMillisecondsTime();
    }
    public void Update()
    {
        while (taskQue.Count > 0)
        {
            TaskPack taskPack = null;
            lock (taskQueLock)
            {
                taskPack = taskQue.Dequeue();
            }

            taskPack?.cb(taskPack.tid);

        }
    }
}