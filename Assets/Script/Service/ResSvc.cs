/****************************************************
    文件：ResSvc.cs
    作者：www34
    邮箱:
    日期：2025/10/10 20:59:3
    功能：资源服务
*****************************************************/

using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Xml;

public class ResSvc : MonoBehaviour
{
    public static ResSvc Instance = null;
    private void Awake()
    {
        Instance = this;
    }
    public void InitSvc()
    {
        LoadRDNameCfg(PathDefine.RDNameCfg);
        InitMonsterCfg(PathDefine.MonsterCfg);
        InitMapCfg(PathDefine.MapPathCfg);
        InitGuideCfg(PathDefine.GuidePathCfg);
        InitStrongCfg(PathDefine.StrongPathCfg);
        InitTaskCfg(PathDefine.TaskCfg);
        InitSkillCfg(PathDefine.SkillCfg);
        InitSkillMoveCfg(PathDefine.SkillMoveCfg);
        InitSkillActionCfg(PathDefine.SkillActionCfg);
        PECommon.Log("Init ResSvc Done....");
    }
    private Action actionPrg;
    public void AsyncLoadScene(string SceneName, Action Loaded)
        //loaded事件表示加载完场景后显示某些UI
    {

        GameRoot.Instance.loadingWnd.SetWindowState(true);
        AsyncOperation SceneAsync = SceneManager.LoadSceneAsync(SceneName);
        actionPrg = () =>
        {
            //获取异步加载进度
            float val = SceneAsync.progress;
            //同步设置进度
            GameRoot.Instance.loadingWnd.SetProgress(val);
            if (val == 1)
            {
                //当加载完成时，关闭加载界面
                if (Loaded != null)
                {
                    Loaded();
                }
                SceneAsync = null;
                actionPrg = null;
                GameRoot.Instance.loadingWnd.SetWindowState(false);
            }
        };
    }

    public void Update()
    {
        //每帧更新加载进度
        if (actionPrg != null)
        {
            actionPrg();
        }
    }
    private Dictionary<string, AudioClip> AuDic = new Dictionary<string, AudioClip>();
    public AudioClip LoadAudio(string path, bool cache = false)
    {
        AudioClip audioClip = null;
        if (!AuDic.TryGetValue(path, out audioClip))
        {
            audioClip = Resources.Load<AudioClip>(path);
            if (cache)
            {
                AuDic.Add(path, audioClip);
            }
        }
        return audioClip;
    }
    private Dictionary<string, GameObject> goDic = new Dictionary<string, GameObject>();
    public GameObject LoadPrefab(string path,bool cache=false)
    {
        GameObject prefab = null;
        if(!goDic.TryGetValue(path,out prefab))
        {
            prefab = Resources.Load<GameObject>(path);

            if(cache)
            {
                goDic.Add(path, prefab);

            }
        }
        GameObject go = null;
        if (prefab != null)
        {

            go = Instantiate(prefab);

        }
        return go;
    }
    private Dictionary<string, Sprite> spDic = new Dictionary<string, Sprite>();
    public Sprite LoadSprite(string path,bool iscache=false)
    {
        Sprite sp = null;
        if(!spDic.TryGetValue(path,out sp))
        {
            sp = Resources.Load<Sprite>(path);
            if(iscache)
            {
                spDic.Add(path, sp);
            }
        }
        return sp;
    }
    #region InitCfgs

    #region 随机名字

    private List<string> surnamelist = new List<string>();
    private List<string> manlist = new List<string>();
    private List<string> womanlist = new List<string>();

    private void LoadRDNameCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml file:" + path + " not load correctly");
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);
            XmlNodeList xmlNodeList = doc.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlElement ele = xmlNodeList[i] as XmlElement;
                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                //int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                foreach (XmlElement xmlElement in xmlNodeList[i].ChildNodes)
                {
                    switch (xmlElement.Name)
                    {
                        case "surname":
                            surnamelist.Add(xmlElement.InnerText);
                            break;
                        case "man":
                            manlist.Add(xmlElement.InnerText);
                            break;
                        case "woman":
                            womanlist.Add(xmlElement.InnerText);
                            break;
                    }
                }
            }
        }
    }
    public string GetRDName(bool isman)
    {
        //Random random = new Random();
        string name = surnamelist[PETool.RDInt(0, surnamelist.Count)];
        if (isman)
        {
            name += manlist[PETool.RDInt(0, manlist.Count - 1)];
        }
        else
        {
            name += womanlist[PETool.RDInt(0, womanlist.Count - 1)];
        }
        return name;
    }

    #endregion

    #region 地图配置
    private Dictionary<int,MapCfg> mapCfgDic = new Dictionary<int, MapCfg>();
    
    private void InitMapCfg(string  path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml file:" + path + " not load correctly");
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);
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
                    ID = ID,
                    monsterLst = new List<MonsterData>(),
                   

                };
                
                foreach (XmlElement xmlElement in xmlNodeList[i].ChildNodes)
                {
                    switch (xmlElement.Name)
                    {
                        case "mapName":
                            mc.MapName = xmlElement.InnerText;
                            break;
                        case "sceneName":
                            mc.SceneName = xmlElement.InnerText;
                            break;
                        case "power":
                            mc.power=int.Parse(xmlElement.InnerText);
                            break;
                        case "mainCamPos":
                        {
                            string[] values = xmlElement.InnerText.Split(',');
                            mc.mainCameraPos = new Vector3(float.Parse(values[0]), float.Parse(values[1]),
                                float.Parse(values[2]));
                        }
                            break;
                        case "mainCamRote":
                        {
                            string[] values = xmlElement.InnerText.Split(',');
                            mc.mainCameraRotation = new Vector3(float.Parse(values[0]), float.Parse(values[1]),
                                float.Parse(values[2]));
                        }
                            break;
                        case "playerBornPos":
                        {
                            string[] values = xmlElement.InnerText.Split(',');
                            mc.playerBornPos = new Vector3(float.Parse(values[0]), float.Parse(values[1]),
                                float.Parse(values[2]));
                        }
                            break;
                        case "playerBornRote":
                        {
                            string[] values = xmlElement.InnerText.Split(',');
                            mc.playerBornRotation = new Vector3(float.Parse(values[0]), float.Parse(values[1]),
                                float.Parse(values[2]));
                        }
                           
                            break;
                        case "monsterLst":
                        {
                            string[] strArr=xmlElement.InnerText.Split('#');
                            for (int waveIndex = 1; waveIndex < strArr.Length; waveIndex++)
                            {
                                string[] tempArr = strArr[waveIndex].Split('|');
                                for (int j = 1; j < tempArr.Length; j++)
                                {
                                    string[] arr=tempArr[j].Split(',');
                                    MonsterData monsterData = new MonsterData()
                                    {
                                        ID = int.Parse(arr[0]),
                                        waveID = waveIndex,
                                        monsterIndex = j,
                                        mBornPos = new Vector3(float.Parse(arr[1]), float.Parse(arr[2]), float.Parse(arr[3])),
                                        mBornRot = new Vector3(0, float.Parse(arr[4]), 0),
                                        level = int.Parse(arr[5]),
                                        Cfg = GetMonsterCfg(int.Parse(arr[0]))
                                    };
                                    mc.monsterLst.Add(monsterData);
                                }
                                
                            }
                            
                        }
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
                mapCfgDic.Add(ID,mc);
            }
        }
    }
    
    public MapCfg GetMapCfg(int id)
    {
        MapCfg mapCfg = null;
        if(mapCfgDic.TryGetValue(id,out mapCfg))
        {
            return mapCfg;
        }
        else
        {
            return null;
        }
    }

    #endregion
    
    #region 任务奖励配置
    private Dictionary<int, TaskRewardCfg> taskCfgDic = new Dictionary<int, TaskRewardCfg>();
    private void InitTaskCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml file:" + path + " not load correctly");
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);
            XmlNodeList xmlNodeList = doc.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlElement ele = xmlNodeList[i] as XmlElement;
                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                TaskRewardCfg taskCfg = new TaskRewardCfg
                {
                    ID = ID,
                };
                foreach (XmlElement xmlElement in xmlNodeList[i].ChildNodes)
                {
                    switch (xmlElement.Name)
                    {
                        case "count":
                            taskCfg.count = int.Parse(xmlElement.InnerText);
                            break;
                        case "coin":
                            taskCfg.coin = int.Parse(xmlElement.InnerText);
                            break;
                        case "exp":
                            taskCfg.exp = int.Parse(xmlElement.InnerText);
                            break;
                        case "taskName":
                            taskCfg.taskname = xmlElement.InnerText;
                            break;
                    }
                }
                taskCfgDic.Add(ID, taskCfg);
            }
        }
    }
    public TaskRewardCfg GetTaskCfg(int id)
    {
        TaskRewardCfg taskCfg = null;
        if(taskCfgDic.TryGetValue(id,out taskCfg))
        {
            return taskCfg;
        }
        return null ;
    }
    #endregion
    
    #region 强化系统配置
    //strongCfgDic:<pos,<starlv,strongCfg>>
    private Dictionary<int, Dictionary<int, StrongCfg>> strongCfgDic = new Dictionary<int, Dictionary<int, StrongCfg>>();
    private void InitStrongCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml file:" + path + " not load correctly");
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);
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
        }
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
    public enum PropertyType
    {
        Hp,
        Hurt,
        Defense
    }
    //获取当前星级的属性累加值，也就是装备当前的属性
    public int GetPropSumByStarLv(int pos, int starLv,PropertyType type)
    {
        Dictionary<int, StrongCfg> dic;
        int valve = 0;
        if(strongCfgDic.TryGetValue(pos,out dic))
        {
            for(int i=0;i<=starLv;i++)
            {
                StrongCfg data;
                if(dic.TryGetValue(i,out data))
                {
                    switch(type)
                    {
                        case PropertyType.Hp:
                            valve += data.addhp;
                            break;
                        case PropertyType.Hurt:
                            valve += data.addhurt;
                            break;
                        case PropertyType.Defense:
                            valve += data.adddef;
                            break;
                    }
                }
            }
        }
        return valve;
    }
    #endregion
    #region 自动引导配置
    private Dictionary<int, AutoGuideCfg> guideCfgDic = new Dictionary<int, AutoGuideCfg>();
    private void InitGuideCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml file:" + path + " not load correctly");
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);
            XmlNodeList xmlNodeList = doc.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlElement ele = xmlNodeList[i] as XmlElement;
                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                AutoGuideCfg guideCfg = new AutoGuideCfg
                {
                    ID = ID,
                };

                foreach (XmlElement xmlElement in xmlNodeList[i].ChildNodes)
                {
                    switch (xmlElement.Name)
                    {
                        case "npcID":
                            guideCfg.npcID =int.Parse(xmlElement.InnerText);
                            break;
                        case "dilogArr":
                            guideCfg.dialogArr = xmlElement.InnerText;
                            break;
                        case "actID":
                            guideCfg.actID = int.Parse(xmlElement.InnerText);
                            break;
                        case "coin":
                            guideCfg.coin = int.Parse(xmlElement.InnerText);
                            break;
                        case "exp":
                            guideCfg.exp = int.Parse(xmlElement.InnerText);
                            break;

                    }
                }
                guideCfgDic.Add(ID, guideCfg);
            }
        }
    }
    public AutoGuideCfg GetGuideCfg(int id)
    {
        AutoGuideCfg autoGuideCfg = null;
        if(guideCfgDic.TryGetValue(id,out autoGuideCfg))
        {
            return autoGuideCfg;
        }
        return null ;
    }
    #endregion


    #region 技能配置
    private Dictionary<int,SkillCfg>  skillCfgDic = new Dictionary<int, SkillCfg>();
    public void InitSkillCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml file:" + path + " not load correctly");
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);
            XmlNodeList xmlNodeList = doc.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlElement ele = xmlNodeList[i] as XmlElement;
                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                SkillCfg skillCfg = new SkillCfg
                {
                    ID = ID,
                    skillMoveList = new List<int>(),
                    skillActionList = new List<int>(),
                    skillDamageList = new List<int>()
                };

                foreach (XmlElement xmlElement in xmlNodeList[i].ChildNodes)
                {
                    switch (xmlElement.Name)
                    {
                        case "skillName":
                            skillCfg.skillName = xmlElement.InnerText;
                            break;
                        case "cdTime":
                            skillCfg.cdTime = int.Parse(xmlElement.InnerText);
                            break;
                        case "skillTime":
                            skillCfg.aniTime=int.Parse(xmlElement.InnerText);
                            break;
                        case "aniAction":
                            skillCfg.skillAction=int.Parse(xmlElement.InnerText);
                            break;
                        case "fx":
                            skillCfg.fx=xmlElement.InnerText;
                            break;
                        case "isCombo":
                            skillCfg.isCombo = xmlElement.InnerText.Equals("1");
                            break;
                        case "isCollide":
                            skillCfg.isCollide = xmlElement.InnerText.Equals("1");
                            break;
                        case "isBreak":
                            skillCfg.isBreak = xmlElement.InnerText.Equals("1");
                            break;
                        case "dmgType":
                        {
                            int type=int.Parse(xmlElement.InnerText);
                            skillCfg.damageType = (DamageType)type;
                        }break;
                        case "skillMoveLst":
                            string[] moveList=xmlElement.InnerText.Split('|');
                            for (int j = 0; j < moveList.Length; j++)
                            {
                                if (!moveList[j].Equals(""))
                                {
                                    skillCfg.skillMoveList.Add(int.Parse(moveList[j]));
                                }
                            }
                            break;
                        case "skillActionLst":
                            string[] actionList=xmlElement.InnerText.Split('|');
                            for (int j = 0; j < actionList.Length; j++)
                            {
                                if (!actionList[j].Equals(""))
                                {
                                    skillCfg.skillActionList.Add(int.Parse(actionList[j]));
                                }
                            }
                            break;
                        case "skillDamageLst":
                            string[] damageList=xmlElement.InnerText.Split('|');
                            for (int j = 0; j < damageList.Length; j++)
                            {
                                if (!damageList[j].Equals(""))
                                {
                                    skillCfg.skillDamageList.Add(int.Parse(damageList[j]));
                                }
                            }
                            break;
                    }
                }
                skillCfgDic.Add(ID, skillCfg);
            }
        }
    }

    public SkillCfg GetSkillCfg(int id)
    {
        SkillCfg skillCfg = null;
        if(skillCfgDic.TryGetValue(id,out skillCfg))
        {
            return skillCfg;
        }
        return null;
    }
    
    
    #endregion

    #region 技能移动配置

    private Dictionary<int,SkillMoveCfg>  skillMoveCfgDic = new Dictionary<int, SkillMoveCfg>();
    public void InitSkillMoveCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml file:" + path + " not load correctly");
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);
            XmlNodeList xmlNodeList = doc.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlElement ele = xmlNodeList[i] as XmlElement;
                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                SkillMoveCfg skillMoveCfg = new SkillMoveCfg
                {
                    ID = ID,
                };

                foreach (XmlElement xmlElement in xmlNodeList[i].ChildNodes)
                {
                    switch (xmlElement.Name)
                    {
                        case "delayTime":
                            skillMoveCfg.delayTime = int.Parse(xmlElement.InnerText);
                            break;
                        case "moveTime":
                            skillMoveCfg.moveTime = int.Parse(xmlElement.InnerText);
                            break;
                        case "moveDis":
                            skillMoveCfg.moveDis = float.Parse(xmlElement.InnerText);
                            break;
                    }
                }
                skillMoveCfgDic.Add(ID, skillMoveCfg);
            }
        }
    }

    public SkillMoveCfg GetSkillMoveCfg(int id)
    {
        SkillMoveCfg skillMoveCfg = null;
        if(skillMoveCfgDic.TryGetValue(id,out skillMoveCfg))
        {
            return skillMoveCfg;
        }
        return null;
    }

    #endregion
    #region 技能伤害点配置

    private Dictionary<int,SkillActionCfg>  skillActionCfgDic = new Dictionary<int, SkillActionCfg>();

    private void InitSkillActionCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml file:" + path + " not load correctly");
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);
            XmlNodeList xmlNodeList = doc.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlElement ele = xmlNodeList[i] as XmlElement;
                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                SkillActionCfg skillActionCfg = new SkillActionCfg
                {
                    ID = ID,
                };

                foreach (XmlElement xmlElement in xmlNodeList[i].ChildNodes)
                {
                    switch (xmlElement.Name)
                    {
                        case "delayTime":
                            skillActionCfg.delayTime = int.Parse(xmlElement.InnerText);
                            break;
                        case "radius":
                            skillActionCfg.radius = float.Parse(xmlElement.InnerText);
                            break;
                        case "angle":
                            skillActionCfg.angle = int.Parse(xmlElement.InnerText);
                            break;
                    }
                }
                skillActionCfgDic.Add(ID, skillActionCfg);
            }
        }
    }

    public SkillActionCfg GetSkillActionCfg(int id)
    {
        SkillActionCfg skillActionCfg = null;
        return skillActionCfgDic.TryGetValue(id,out skillActionCfg) ? skillActionCfg : null;
    }

    #endregion
    #region 怪物配置

    
    private Dictionary<int,MonsterCfg>  monsterCfgDic = new Dictionary<int, MonsterCfg>();
    public void InitMonsterCfg(string path)
    {
        
        
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml file:" + path + " not load correctly");
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);
            XmlNodeList xmlNodeList = doc.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlElement ele = xmlNodeList[i] as XmlElement;
                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                MonsterCfg monsterCfg = new MonsterCfg()
                {
                    ID = ID,
                    battleProps = new BattleProps()
                };

                foreach (XmlElement xmlElement in xmlNodeList[i].ChildNodes)
                {
                    switch (xmlElement.Name)
                    {
                        case "mName":
                            monsterCfg.mName = xmlElement.InnerText;
                            break;
                        case "mType":
                            if (xmlElement.InnerText == "1")
                            {
                                monsterCfg.mType = MonsterType.None;
                            }
                            else if (xmlElement.InnerText == "2")
                            {
                                monsterCfg.mType = MonsterType.Boss;
                            }
                            break;
                        case "isStop":
                            monsterCfg.canStop = int.Parse(xmlElement.InnerText) == 1;
                            break;
                        case "resPath":
                            monsterCfg.resPath = xmlElement.InnerText;
                            break;
                        case "skillID":
                            monsterCfg.skillID = int.Parse(xmlElement.InnerText);
                            break;
                        case "atkDis":
                            monsterCfg.atkDis = float.Parse(xmlElement.InnerText);
                            break;
                        case "hp":
                            monsterCfg.battleProps.hp = int.Parse(xmlElement.InnerText);
                            break;
                        case "ad":
                            monsterCfg.battleProps.ad = int.Parse(xmlElement.InnerText);
                            break;
                        case "ap":
                            monsterCfg.battleProps.ap = int.Parse(xmlElement.InnerText);
                            break;
                        case "addef":
                            monsterCfg.battleProps.addef = int.Parse(xmlElement.InnerText);
                            break;
                        case "apdef":
                            monsterCfg.battleProps.apdef = int.Parse(xmlElement.InnerText);
                            break;
                        case "dodge":
                            monsterCfg.battleProps.dodge = int.Parse(xmlElement.InnerText);
                            break;
                        case "pierce":
                            monsterCfg.battleProps.pierce = int.Parse(xmlElement.InnerText);
                            break;
                        case "critical":
                            monsterCfg.battleProps.critical = int.Parse(xmlElement.InnerText);
                            break;
                    }
                }
                monsterCfgDic.Add(ID, monsterCfg);
            }
        }
    }

    public MonsterCfg GetMonsterCfg(int id)
    {
        MonsterCfg monsterCfg = null;
        if(monsterCfgDic.TryGetValue(id,out monsterCfg))
        {
            return monsterCfg;
        }
        return null;
    }

    #endregion
    #endregion

    public void ResetSkillMoveData()
    {
        skillCfgDic.Clear();
        skillMoveCfgDic.Clear();
        InitSkillCfg(PathDefine.SkillCfg);
        InitSkillMoveCfg(PathDefine.SkillMoveCfg);
    }
}