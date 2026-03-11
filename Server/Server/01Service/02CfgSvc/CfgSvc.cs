// time:2025/10/27/17
// path:E:\unityproject\DarkGod\Server\Server\01Service\02CfgSvc\CfgSvc.csCfgSvc.cs
// func: 读取配置文件服务
// www34


using System;
using System.Collections.Generic;
using System.Xml;
using MySql.Data;

public class CfgSvc
{
    private static CfgSvc instance = null;
    public static CfgSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CfgSvc();
            }
            return instance;
        }
    }

    public void Init()
    {
        InitGuideCfg();
        InitStrongCfg();
        InitTaskCfg();
        InitMapCfg();
        PECommon.Log("CfgSvc Init Done");
    }
    
    #region 自动引导配置
    private Dictionary<int, GuideCfg> guideCfgDic = new Dictionary<int, GuideCfg>();
    private void InitGuideCfg()
    { 
            XmlDocument doc = new XmlDocument();
            doc.Load(@"E:\unityproject\DarkGod\Assets\Resources\ResCfgs\guide.xml");
            XmlNodeList xmlNodeList = doc.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlElement ele = xmlNodeList[i] as XmlElement;
                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                GuideCfg guideCfg = new GuideCfg
                {
                    ID = ID,
                };

                foreach (XmlElement xmlElement in xmlNodeList[i].ChildNodes)
                {
                    switch (xmlElement.Name)
                    {
                        case "coin":
                            guideCfg.coin =int.Parse(xmlElement.InnerText);
                            break;
                        case "exp":
                            guideCfg.exp = int.Parse(xmlElement.InnerText);
                            break;
                    }
                }
                guideCfgDic.Add(ID, guideCfg);
            }
            PECommon.Log("GuideCfg Init Done");
            
    }
    public GuideCfg GetGuideCfg(int id)
    {
        GuideCfg guideCfg = null;
        if(guideCfgDic.TryGetValue(id,out guideCfg))
        {
            return guideCfg;
        }
        return null ;
    }
    #endregion
    #region 任务奖励配置
    private Dictionary<int, TaskRewardCfg> _taskRewardCfgsDic = new Dictionary<int, TaskRewardCfg>();
    private void InitTaskCfg()
    { 
        XmlDocument doc = new XmlDocument();
        doc.Load(@"E:\unityproject\DarkGod\Assets\Resources\ResCfgs\taskreward.xml");
        XmlNodeList xmlNodeList = doc.SelectSingleNode("root").ChildNodes;
        for (int i = 0; i < xmlNodeList.Count; i++)
        {
            XmlElement ele = xmlNodeList[i] as XmlElement;
            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
            TaskRewardCfg taskRewardCfg = new TaskRewardCfg()
            {
                ID = ID,
            };

            foreach (XmlElement xmlElement in xmlNodeList[i].ChildNodes)
            {
                switch (xmlElement.Name)
                {
                    case "coin":
                        taskRewardCfg.coin =int.Parse(xmlElement.InnerText);
                        break;
                    case "exp":
                        taskRewardCfg.exp = int.Parse(xmlElement.InnerText);
                        break;
                    case "count":
                        taskRewardCfg.count = int.Parse(xmlElement.InnerText);
                        break;
                }
            }
            _taskRewardCfgsDic.Add(ID, taskRewardCfg);
        }
        PECommon.Log("TaskRewardCfg Init Done");
    }
    public TaskRewardCfg GetTaskCfg(int id)
    {
        TaskRewardCfg taskRewardCfg = null;
        if(_taskRewardCfgsDic.TryGetValue(id,out taskRewardCfg))
        {
            return taskRewardCfg;
        }
        return null ;
    }
    #endregion
    #region 升级强化配置
    private Dictionary<int, Dictionary<int, StrongCfg>> strongCfgDic = new Dictionary<int, Dictionary<int, StrongCfg>>();
    private void InitStrongCfg()
    {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"E:\unityproject\DarkGod\Assets\Resources\ResCfgs\strong.xml");
            XmlNodeList xmlNodeList = doc.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlElement ele = xmlNodeList[i] as XmlElement;
                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                StrongCfg strongCfg = new StrongCfg
                {
                    ID = ID,
                };

                foreach (XmlElement xmlElement in xmlNodeList[i].ChildNodes)
                {
                    int val = int.Parse(xmlElement.InnerText);

                    switch (xmlElement.Name)
                    {
                        case "pos":
                            strongCfg.pos = val;
                            break;
                        case "starlv":
                            strongCfg.starlv = val;
                            break;
                        case "addhp":
                            strongCfg.addhp = val;
                            break;
                        case "addhurt":
                            strongCfg.addhurt = val;
                            break;
                        case "adddef":
                            strongCfg.adddef = val;
                            break;
                        case "minlv":
                            strongCfg.minlv = val;
                            break;
                        case "coin":
                            strongCfg.coin = val;
                            break;
                        case "crystal":
                            strongCfg.crystal = val;
                            break;
                    }
                }
                Dictionary<int, StrongCfg> dic = null;
                if(strongCfgDic.TryGetValue(strongCfg.pos,out dic))
                {
                    dic.Add(strongCfg.starlv, strongCfg);
                }
                else
                {
                    dic = new Dictionary<int, StrongCfg>();
                    dic.Add(strongCfg.starlv, strongCfg);
                    strongCfgDic.Add(strongCfg.pos, dic);

                }
            }
            PECommon.Log("StrongCfg Init Done");
        
    }
    public StrongCfg GetStrongCfg(int pos,int starlv)
    {
        StrongCfg strongCfg = null;
        Dictionary<int, StrongCfg> dic = null;

        if(strongCfgDic.TryGetValue(pos,out dic))
        {
            if(dic.TryGetValue(starlv,out strongCfg))
            {
                return strongCfg;
            }
        }
        return null;
    }
    

    #endregion

    #region 地图配置
    private Dictionary<int, MapCfg> _mapCfgs = new Dictionary<int, MapCfg>();

    public void InitMapCfg()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(@"E:\unityproject\DarkGod\Assets\Resources\ResCfgs\map.xml");
        XmlNodeList xmlNodeList = doc.SelectSingleNode("root").ChildNodes;
        for (int i = 0; i < xmlNodeList.Count; i++)
        {
            XmlElement ele = xmlNodeList[i] as XmlElement;
            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
            MapCfg mc = new MapCfg
            {
                ID = ID
            };

            foreach (XmlElement xmlElement in xmlNodeList[i].ChildNodes)
            {
                switch (xmlElement.Name)
                {
                    case "power":
                        mc.power=int.Parse(xmlElement.InnerText);
                        break;
                    case "coin":
                        mc.coin=int.Parse(xmlElement.InnerText);
                        break;
                    case "crystal":
                        mc.crystal=int.Parse(xmlElement.InnerText);
                        break;
                    case "exp":
                        mc.exp=int.Parse(xmlElement.InnerText);
                        break;
                    
                }
            }
            _mapCfgs.Add(ID, mc);
        }
        PECommon.Log("MapCfg Init Done");
    }
    public MapCfg GetMapCfg(int id)
    {
        MapCfg mapCfg = null;
        Dictionary<int, MapCfg> dic = null;
        return _mapCfgs.TryGetValue(id,out mapCfg) ? mapCfg : null;
    }

    #endregion
}

public class GuideCfg:BaseData<GuideCfg>
{
    //<npcID></npcID>
    //<dilogArr></dilogArr>
    //<actID></actID>
    //<coin></coin>
    //<exp></exp>
    public int coin;
    public int exp;
}
public class StrongCfg:BaseData<StrongCfg>
{
    //< pos />
    //< starlv />
    //< addhp />
    //< addhurt />
    //< adddef />
    //< minlv />
    //< coin />
    //< crystal />
    public int pos;
    public int starlv;
    public int addhp;
    public int addhurt;
    public int adddef;
    public int minlv;
    public int coin;
    public int crystal;
}
public class TaskRewardCfg : BaseData<TaskRewardCfg>
{
    public int count;
    public int exp;
    public int coin;
}

public class MapCfg : BaseData<MapCfg>
{
    public int power;
    public int coin;
    public int crystal;
    public int exp;
}
public class TaskRewardData : BaseData<TaskRewardData>
{
    public int prgs;
    public bool istaked;
}
public class BaseData<T>
{
    public int ID;
}
