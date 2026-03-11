// time:2025/11/10/19
// path:E:\unityproject\DarkGod\Server\Server\02System\06PowerSystem\PowerSystem.csPowerSystem.cs
// func:
// www34


using System.Collections.Generic;
using PEProtocol;

public class PowerSystem
{
    private static PowerSystem instance = null;
    public static PowerSystem Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new PowerSystem();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;
    private TimerSvc timerSvc = null;
    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        timerSvc = TimerSvc.Instance;
        TimerSvc.Instance.AddTimeTask(CalcuOnlinePowerAdd,PECommon.PowerAddSpace,PETimeUnit.Minute,0);
        PECommon.Log("PowerSystem Init Done");
    }

    private void CalcuOnlinePowerAdd(int tid)
    {
        GameMsg msg = new GameMsg()
        {
            cmd=(int)CMD.PushPower
        };
        msg.pushPower = new PushPower();
        //
        var onlineAcctDic = cacheSvc.OnlineSessionDic;
        foreach (var VARIABLE in onlineAcctDic)
        {
            PlayerData pd=VARIABLE.Value;
            ServerSession serverSession = VARIABLE.Key;
            int maxPower = PECommon.GetPowerLimit(pd);
            if (pd.power >= maxPower)
            {
                continue;
            }
            pd.time=timerSvc.GetNowTime();
            pd.power=pd.power+PECommon.PowerAddCount>maxPower ? maxPower : pd.power+PECommon.PowerAddCount;
            if (!cacheSvc.UpdataPlayerData(pd.id, pd))
            {
                msg.err = (int)ErrorCode.UpdataDBError;
            }
            else
            {
                msg.pushPower.power = pd.power;
            }
            serverSession.SendMsg(msg);
            
        }
    }

    public void CalcuOffLinePowerAdd(PlayerData pd)
    {
        int power = pd.power;
        int maxPower = PECommon.GetPowerLimit(pd);
        long now = timerSvc.GetNowTime();
        long milliseconds =now-pd.time;
        int addPower=(int)(milliseconds/(1000*60*PECommon.PowerAddSpace))*PECommon.PowerAddCount;
        if (addPower > 0&&pd.power < maxPower)
        {
            pd.power=power+addPower>maxPower ? maxPower : power+addPower;
        }
        if (pd.power != power)
        {
            cacheSvc.UpdataPlayerData(pd.id, pd);
        }
        PECommon.Log("prepower:"+power+"  currentPower:"+pd.power);
    }
}