/****************************************************
    文件：MainCitySys.cs
	作者：www34
    邮箱: 
    日期：2025/10/19 14:28:45
	功能：主城业务系统
*****************************************************/

using System;
using PEProtocol;
using UnityEngine;
using UnityEngine.AI;

public class MainCitySys : SystemRoot 
{
    public static MainCitySys Instance = null;
    public MainCityWnd mainCityWnd;
    public InfoWnd infoWnd;
    public GuideWnd guideWnd;
    public StrongWnd strongWnd;
    public ChatWnd chatWnd;
    public BuyWnd buyWnd;
    public TaskWnd taskWnd;
    private PlayerController playerController = null;
    private Transform charShowCamTrans;
    private AutoGuideCfg currentTaskData;
    private Transform[] npcPosTrans;
    private NavMeshAgent nav;
    public override void InitSys()
    {
        base.InitSys();

        Instance = this;

        PECommon.Log("MainCitySys Init...");
    }
    private void Update()
    {
        if(isNavGuiding)
        {
            playerController.CameraFollow();
            isArriveNavPos();
        }
    }
    public void EnterMainCity()
    {
        MapCfg mapData = resSvc.GetMapCfg(Constants.SceneMainCityId);
        resSvc.AsyncLoadScene(mapData.SceneName, () =>
        {
            PECommon.Log("Enter MainCity...");
            //加载主角
            LoadPlayer(mapData);
            
            //打开场景UI
            mainCityWnd.SetWindowState();
            
            GameRoot.Instance.GetComponent<AudioListener>().enabled = false;
            //播放主城背景音乐
            audioSvc.PlayBGMuic(Constants.BGLMainCity);
            //获取场景中npc的位置
            GameObject mapRoot = GameObject.FindGameObjectWithTag("MapRoot");
            MainCityMap mcm = mapRoot.GetComponent<MainCityMap>();
            npcPosTrans = mcm.npcPosTrans;
            // 设置人物展示相机,人物展示相机默认关闭，在打开信息界面时同步打开
            if(charShowCamTrans!=null)
            {
                charShowCamTrans.gameObject.SetActive(false);

            }
        });
    }
    private void LoadPlayer(MapCfg mapData)
    {
        GameObject player = resSvc.LoadPrefab(PathDefine.AssassinCityPlayerPrefab, true);
        player.transform.position = mapData.playerBornPos;
        player.transform.localEulerAngles = mapData.playerBornRotation;
        player.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        //相机初始化
        Camera.main.transform.position = mapData.mainCameraPos;
        Camera.main.transform.localEulerAngles = mapData.mainCameraRotation;
        playerController =player.GetComponent<PlayerController>();
        playerController.Init();
        nav = player.GetComponent<NavMeshAgent>();

    }
    public void SetMove(Vector2 dir)
    {
        StopNavTask();

        //设置动画
        if (dir == Vector2.zero)
        {
            playerController.SetBlend(Constants.BlendIdle);

        }
        else
        {
            playerController.SetBlend(Constants.BlendMove);
            
        }
        //设置人物运动
        playerController.Dir = dir;
    }
    #region Info Wnd
    public void OpenInfoWnd()
    {
        StopNavTask();

        if (charShowCamTrans == null)
        {
            charShowCamTrans = GameObject.FindGameObjectWithTag("CharShowCam").transform;
        }
        //设置人物相机相对位置
        charShowCamTrans.localPosition = playerController.transform.position + playerController.transform.forward * 3.8f+new Vector3(0,1.2f,0);
        charShowCamTrans.localEulerAngles = new Vector3(0, 180 + playerController.transform.localEulerAngles.y, 0);
        charShowCamTrans.localScale = Vector3.one;
        charShowCamTrans.gameObject.SetActive(true);

        infoWnd.SetWindowState();
    }
    public void CloseInfoWnd()
    {
        infoWnd.SetWindowState(false);
        if(charShowCamTrans!=null)
        {
            charShowCamTrans.gameObject.SetActive(false);

        }
    }
    private float playerStartRoate = 0;
    public void SetPlayerStartRoate()
    {
        playerStartRoate = playerController.transform.localEulerAngles.y;
    }
    public void SetPlayerShowRoate(float rote)
    {
        playerController.transform.localEulerAngles = new Vector3(0, playerStartRoate - rote*0.4f, 0);

    }
#endregion
    #region Guide
    private bool isNavGuiding = false;
    public void RunTask(AutoGuideCfg guideCfg)
    {
        if(guideCfg!=null)
        {
            currentTaskData = guideCfg;
        }
        //解析任务数据
        if(currentTaskData.npcID!=-1)
        {
            //寻找目标npc
            //player 与目标npc的距离
            float distance = Vector3.Distance(playerController.transform.position, npcPosTrans[guideCfg.npcID].position);
            if(distance<0.5f)
            {
                nav.isStopped = true;
                isNavGuiding = false;
                playerController.SetBlend(Constants.BlendIdle);
                nav.enabled = false;
                OpenGuideWnd();
            }
            else
            {
                nav.enabled = true;
                isNavGuiding = true;
                nav.speed = Constants.playerMoveSpeed;
                nav.SetDestination(npcPosTrans[guideCfg.npcID].position);
                playerController.SetBlend(Constants.BlendMove);

            }
        }
        else
        {
            //不需要npc
            //直接打开引导界面
            OpenGuideWnd();

        }

    }
    //判断是否到达目标点
    private void isArriveNavPos()
    {
        float distance = Vector3.Distance(playerController.transform.position, npcPosTrans[currentTaskData.npcID].position);
        if (distance < 0.5f)
        {
            nav.isStopped = true;
            isNavGuiding = false;
            playerController.SetBlend(Constants.BlendIdle);
            nav.enabled = false;
            OpenGuideWnd();
        }
    }
    //某些情况下需要停止导航
    private void StopNavTask()
    {
        if(isNavGuiding)
        {
            isNavGuiding = false;
            nav.isStopped = true;
            nav.enabled = false;
            playerController.SetBlend(Constants.BlendIdle);

        }
    }
    private void OpenGuideWnd()
    {
        //TODO 引导界面
        guideWnd.SetWindowState();


    }
    public AutoGuideCfg GetCurrentTaskData()
    {
        return currentTaskData;
    }
    public void RspGuide(GameMsg msg)
    {
        RspGuide data = msg.rspGuide;
        GameRoot.AddTips(Constants.SetTxtColor("任务奖励 金币+" + currentTaskData.coin + "经验+" + currentTaskData.exp,TxtColor.Blue));
        switch (currentTaskData.actID)
        {
            case 0:
                //与智者对话
                break;
            case 1:
                //进入副本
                EnterFuben();                
                break;
            case 2:
                //进入强化界面
                OpenStrongWnd();
                break;
            case 3:
                //TODO进入体力购买
                OpenBuyWnd(BuyType.Power);
                break;
            case 4:
                //进入金币锻造
                OpenBuyWnd(BuyType.Coin);
                break;
            case 5:
                //进入世界聊天
                OpenChatWnd();
                break;
        }
        //更新本地数据
        GameRoot.Instance.SetPlayerDataByGuide(data);
        //更新ui中的数据
        mainCityWnd.RefreshUI();

    }
    #endregion
    #region StrongWnd
    public void OpenStrongWnd()
    {
        StopNavTask();
        strongWnd.SetWindowState();
    }
    public void RspStrong(GameMsg gameMsg)
    {
        int prePower = PECommon.GetFightByProps(GameRoot.Instance.PlayerData);
        GameRoot.Instance.SetPlayerDataByStrong(gameMsg.rspStrong);
        int curtPower = PECommon.GetFightByProps(GameRoot.Instance.PlayerData);
        GameRoot.AddTips(Constants.SetTxtColor("提升战力"+(curtPower-prePower),TxtColor.Blue));
        strongWnd.UpdateUI();
        mainCityWnd.RefreshUI();
    }
    #endregion

    #region ChatWnd

    public void OpenChatWnd()
    {
        chatWnd.SetWindowState();
    }
    public void PushChat(GameMsg gameMsg)
    {
        
        chatWnd.AddChat(gameMsg.pushChat.name,gameMsg.pushChat.chat);
    }

    #endregion

    #region BuyWnd

    public void OpenBuyWnd(BuyType buyType)
    {
        StopNavTask();
        buyWnd.SetBuyType(buyType);
        buyWnd.SetWindowState();
    }
    public void RspBuy(GameMsg gameMsg)
    {
        RspBuy data = gameMsg.rspBuy;
        GameRoot.Instance.SetPlayerDataByBuy(data);
        GameRoot.AddTips("购买成功");
        mainCityWnd.RefreshUI();
        buyWnd.SetWindowState(false);

        if (gameMsg.pushTaskPrg == null) return;
        PushTaskPrg(gameMsg);
    }

    #endregion

    public void PushPower(GameMsg gameMsg)
    {
        PushPower data = gameMsg.pushPower;
        GameRoot.Instance.SetPlayerDataByPushPower(data);
        if (mainCityWnd.GetWindowState())
        {
            mainCityWnd.RefreshUI();
        }
        
    }
#region TaskWnd

    public void OpenTaskWnd()
    {
        taskWnd.SetWindowState();
    }
    public void RspTaskReward(GameMsg gameMsg)
    {
        RspTaskReward data = gameMsg.rspTaskReward;
        GameRoot.Instance.SetPlayerDataByTaskReward(data);
        if(taskWnd.GetWindowState())
        {
            taskWnd.RefreshUi();
        }
        mainCityWnd.RefreshUI();

    }
    public void PushTaskPrg(GameMsg gameMsg)
    {
        PushTaskPrg data = gameMsg.pushTaskPrg;
        GameRoot.Instance.SetPlayerDataByPushTaskPrg(data);
        if (taskWnd.GetWindowState())
        {
            taskWnd.RefreshUi();
        }

    }
#endregion
    public void EnterFuben()
    {
        StopNavTask();
        FubenSys.Instance.EnterFuben();
    }
}