/****************************************************
    文件：TaskWnd.cs
	作者：www34
    邮箱: 
    日期：2025/11/12 19:33:3
	功能：任务奖励窗口
*****************************************************/

using System.Collections.Generic;
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class TaskWnd : WindowRoot
{
	public Transform scrollContent;
	private PlayerData playerdata=null;
	private List<TaskRewardData> taskDataList=new List<TaskRewardData>();
	protected override void InitWnd()
	{
		base.InitWnd();
		playerdata = GameRoot.Instance.PlayerData;
		RefreshUi();
	}
    protected override void ClearWnd()
    {
        base.ClearWnd();
        for(int i=0;i<scrollContent.childCount;i++)
        {
            Destroy(scrollContent.GetChild(i).gameObject);
        }
    }

    public void RefreshUi()
	{
        for (int i = 0; i < scrollContent.childCount; i++)
        {
            Destroy(scrollContent.GetChild(i).gameObject);
        }
        taskDataList.Clear();
		//重新赋值
        
		List<TaskRewardData> totakeList = new List<TaskRewardData>();
		List<TaskRewardData> todoList = new List<TaskRewardData>();
		List<TaskRewardData> doneList = new List<TaskRewardData>();
		for (int i = 0; i < playerdata.taskArr.Length; i++)
		{
			string[] taskinfo=playerdata.taskArr[i].Split('|');
			TaskRewardData taskData = new TaskRewardData()
			{
				ID = int.Parse(taskinfo[0]),
				prgs = int.Parse(taskinfo[1]),
				istaked = taskinfo[2].Equals("1")
			};
            if(taskData.prgs==resSvc.GetTaskCfg(taskData.ID).count&&!taskData.istaked)
            {
                totakeList.Add(taskData);

            }
			if (taskData.istaked)
			{
				doneList.Add(taskData);
			}
			else if(taskData.prgs < resSvc.GetTaskCfg(taskData.ID).count)
			{
				todoList.Add(taskData);
			}
		}
        taskDataList.AddRange(totakeList);
		taskDataList.AddRange(todoList);
		taskDataList.AddRange(doneList);
		for (int i = 0; i < taskDataList.Count; i++)
		{
			
			GameObject go = resSvc.LoadPrefab(PathDefine.TaskItemPrefab);
			go.transform.SetParent(scrollContent);
			go.transform.localPosition=Vector3.zero;
			go.transform.localScale=Vector3.one;
			go.name = "taskitem_" + i;

			TaskRewardCfg taskCfg = resSvc.GetTaskCfg(taskDataList[i].ID);
			SetText(GetTrans(go.transform,"txtname"),taskCfg.taskname);
			SetText(GetTrans(go.transform,"txtprg"),taskDataList[i].prgs+"/"+taskCfg.count);
			SetText(GetTrans(go.transform,"txtExp"),"奖励：     经验"+taskCfg.exp);
			SetText(GetTrans(go.transform,"txtCoin"),"金币"+taskCfg.coin);
			Image imgPrg=GetTrans(go.transform,"bgprg/prgVal").GetComponent<Image>();
			imgPrg.fillAmount = taskDataList[i].prgs * 1.0f /taskCfg.count*1.0f;
			
			Button btnTake=GetTrans(go.transform,"btnTake").GetComponent<Button>();
			btnTake.onClick.AddListener(() =>
			{
				ClickTakeBtn(go.name);
			});
			Transform tranComp=GetTrans(go.transform,"imgComp");
			if (taskDataList[i].istaked)
			{
				//已领取
				SetActive(tranComp);
				btnTake.interactable = false;
			}
			else
			{
				SetActive(tranComp,false);
				if (taskDataList[i].prgs == taskCfg.count)
				{
					//任务已完成，可领取
					btnTake.interactable = true;
				}
				else
				{
					//任务未完成，不可领取
					btnTake.interactable = false;
				}
			}
		}
		
	}
	public void ClickCloseBtn()
	{
        audioSvc.PlayUIMusic(Constants.UIClickMusic);

		SetWindowState(false);
	}

	public void ClickTakeBtn(string name)
	{
        string[] vs = name.Split('_');
        int index = int.Parse(vs[1]);
		Debug.Log("click taskitem_"+index);
        GameMsg gameMsg = new GameMsg
        {
            cmd = (int)CMD.ReqTaskReward,
            reqTaskReward=new ReqTaskReward
            {
                rid=taskDataList[index].ID
            }
        };
        netSvc.SendMsg(gameMsg);

        TaskRewardCfg rewardCfg = resSvc.GetTaskCfg(taskDataList[index].ID);
        int coin = rewardCfg.coin;
        int exp = rewardCfg.exp;
        GameRoot.AddTips("获得" + Constants.SetTxtColor("金币", TxtColor.Yellow) + coin+ "," + Constants.SetTxtColor("经验", TxtColor.Green) + exp + "!");
	}
}