/****************************************************
    文件：PlayerControlWnd.cs
	作者：www34
    邮箱: 
    日期：2025/11/22 14:26:24
	功能：玩家控制界面
*****************************************************/

using System;
using System.Security.AccessControl;
using PEProtocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerControlWnd : WindowRoot 
{
	private float globalRate;
	#region UIDefine

	public Image imgTouch;
	public Image imgDirbg;
	public Image imgDirPoint;

	
	public Text txtLevel;
	public Text txtName;
	public Text txtExpPrg;
	public Transform exptransform;
	#endregion

	public Text txtPlayerHP;
	public Image imgPlayerHP;
	private bool menuState = true;
	private Vector2 startPos;
	private Vector2 defaultPos;
	private float pointerDis;
	public Vector2 currentDir;
	private int playerHPSum;
	protected override void InitWnd()
	{
		base.InitWnd();
		playerHPSum = GameRoot.Instance.PlayerData.hp;
		SetText(txtPlayerHP, playerHPSum+"/"+playerHPSum);
		imgPlayerHP.fillAmount = 1;
		defaultPos = imgDirbg.transform.position;
		SetActive(imgDirPoint, false);
		SetBossHpBarState(false);
		RegisterTouchEvts();
		
		RefreshUI();
		pointerDis = Constants.ScreenOpDis / globalRate;
		Sk1CDTime = resSvc.GetSkillCfg(101).cdTime / 1000.0f;
		Sk2CDTime = resSvc.GetSkillCfg(102).cdTime / 1000.0f;
		Sk3CDTime = resSvc.GetSkillCfg(103).cdTime / 1000.0f;
		
		PECommon.Log("PlayerControlWnd Init....");
		
	}

	//Test 
	private void Update()
	{
		//Test
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			ClickSkill1();
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			ClickSkill2();
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			ClickSkill3();
		}
		if (Input.GetKeyDown(KeyCode.A))
		{
			ClickNormalAttack();
		}

		#region skill1CD

		float delta = Time.deltaTime;
		if (isSk1CD)
		{
			sk1FillVal+=delta;
			if (sk1FillVal >= Sk1CDTime)
			{
				sk1FillVal = 0;
				isSk1CD = false;
				SetActive(imgSk1CD,false);
				SetActive(txtSk1CD,false);
			}
			else
			{
				imgSk1CD.fillAmount = 1 - sk1FillVal / Sk1CDTime;
			}
			sk1NumCount+=delta;
			if (sk1NumCount >= 1)
			{
				sk1NumCount -= 1;
				sk1CDVal -= 1;
				SetText(txtSk1CD,sk1CDVal);
				
			}
		}

		#endregion
		#region skill2CD

		if (isSk2CD)
		{
			sk2FillVal+=delta;
			if (sk2FillVal >= Sk2CDTime)
			{
				sk2FillVal = 0;
				isSk2CD = false;
				SetActive(imgSk2CD,false);
				SetActive(txtSk2CD,false);
			}
			else
			{
				imgSk2CD.fillAmount = 1 - sk2FillVal / Sk2CDTime;
			}
			sk2NumCount+=delta;
			if (sk2NumCount >= 1)
			{
				sk2NumCount -= 1;
				sk2CDVal -= 1;
				SetText(txtSk2CD,sk2CDVal);
				
			}
		}

		#endregion
		#region skill3CD

		if (isSk3CD)
		{
			sk3FillVal+=delta;
			if (sk3FillVal >= Sk3CDTime)
			{
				sk3FillVal = 0;
				isSk3CD = false;
				SetActive(imgSk3CD,false);
				SetActive(txtSk3CD,false);
			}
			else
			{
				imgSk3CD.fillAmount = 1 - sk3FillVal / Sk3CDTime;
			}
			sk3NumCount+=delta;
			if (sk3NumCount >= 1)
			{
				sk3NumCount -= 1;
				sk3CDVal -= 1;
				SetText(txtSk3CD,sk3CDVal);
				
			}
		}

		#endregion

		if (transBossHPBar.gameObject.activeSelf)
		{
			BlendBossHp();
			imgYellow.fillAmount = currentPrg;
		}
	}

	public void RefreshUI()
	{
		PlayerData playerData = GameRoot.Instance.PlayerData;
		SetText(txtLevel, playerData.lv);
		SetText(txtName, playerData.name);
		//expPrg
		#region 经验条
		int expPrgVal =(int)(playerData.exp *1.0f/ PECommon.GetExpUpValByLv(playerData.lv)*100);
		SetText(txtExpPrg, expPrgVal + "%");
		int index = expPrgVal / 10;

		GridLayoutGroup gridLayoutGroup = exptransform.gameObject.GetComponent<GridLayoutGroup>();
		globalRate = 1.0f * Constants.WindowStandardHeight / Screen.height;
		float screenWidth = globalRate * Screen.width;
		float width = (screenWidth - 186) / 10;
		gridLayoutGroup.cellSize = new Vector2(width, 7);

		for(int i=0;i<exptransform.childCount;i++)
		{
			Image image = exptransform.GetChild(i).GetComponent<Image>();
			if (i < index)
			{
				image.fillAmount = 1;
			}
			else if (i == index)
			{
				image.fillAmount = expPrgVal % 10 * 1.0f / 10;
			}
			else
			{
				image.fillAmount = 0;
			}
		}
		#endregion
		#region 摇杆
		pointerDis = Constants.ScreenOpDis / globalRate;
		#endregion
	}
	public void RegisterTouchEvts()
	{
		OnClickDown(imgTouch.gameObject, (PointerEventData evt) =>
		{
			startPos = evt.position;
			SetActive(imgDirPoint);
			imgDirbg.transform.position = evt.position;
		});
		OnClickUp(imgTouch.gameObject, (PointerEventData evt) =>
		{
			imgDirbg.transform.position = defaultPos;
			SetActive(imgDirPoint, false);
			imgDirPoint.transform.localPosition = Vector2.zero;
			//MainCitySys.Instance.SetMove(Vector2.zero);
			currentDir=Vector2.zero;
			BattleSys.Instance.SetSelfPlayerMoveDir(currentDir);
		});
		OnDrag(imgTouch.gameObject, (PointerEventData evt) =>
		{
			//计算方向
			Vector2 dir = evt.position - startPos;
			//取长度
			float len = dir.magnitude;

			//限制范围
			if(len> pointerDis)
			{
				Vector2 clampDir = Vector2.ClampMagnitude(dir, pointerDis);
				imgDirPoint.transform.position = startPos + clampDir;
			}
			else
			{
				imgDirPoint.transform.position = evt.position;
			}

			currentDir = dir.normalized;
			BattleSys.Instance.SetSelfPlayerMoveDir(currentDir);
		});
	}
	public void ClickNormalAttack()
	{
		BattleSys.Instance.ReqReleaseSkill(SkillType.NormalAttack);
	}

	#region skill1

	public Image imgSk1CD;
	public Text txtSk1CD;
	private bool isSk1CD = false;
	private float Sk1CDTime;
	private int sk1CDVal;
	private float sk1FillVal = 0;
	private float sk1NumCount = 0;
	public void ClickSkill1()
	{
		if (isSk1CD || !BattleSys.Instance.battleMgr.GetCanRlsSkill()) return;
		BattleSys.Instance.ReqReleaseSkill(SkillType.Skill1);
		isSk1CD = true;
		SetActive(imgSk1CD);
		SetActive(txtSk1CD);
		imgSk1CD.fillAmount = 1;
		sk1CDVal = (int)Sk1CDTime;
		SetText(txtSk1CD,sk1CDVal);
	}
	#endregion

	#region skill2
	public Image imgSk2CD;
	public Text txtSk2CD;
	private bool isSk2CD = false;
	private float Sk2CDTime;
	private int sk2CDVal;
	private float sk2FillVal = 0;
	private float sk2NumCount = 0;
	public void ClickSkill2()
	{
		if (isSk2CD || !BattleSys.Instance.battleMgr.GetCanRlsSkill()) return;
		BattleSys.Instance.ReqReleaseSkill(SkillType.Skill2);
		isSk2CD = true;
		SetActive(imgSk2CD);
		SetActive(txtSk2CD);
		imgSk2CD.fillAmount = 1;
		sk2CDVal = (int)Sk2CDTime;
		SetText(txtSk2CD,sk2CDVal);
	}
	#endregion
	
	#region skill3
	public Image imgSk3CD;
	public Text txtSk3CD;
	private bool isSk3CD = false;
	private float Sk3CDTime;
	private int sk3CDVal;
	private float sk3FillVal = 0;
	private float sk3NumCount = 0;
	
	public void ClickSkill3()
	{
		if (isSk3CD || !BattleSys.Instance.battleMgr.GetCanRlsSkill()) return;
		BattleSys.Instance.ReqReleaseSkill(SkillType.Skill3);
		isSk3CD = true;
		SetActive(imgSk3CD);
		SetActive(txtSk3CD);
		imgSk3CD.fillAmount = 1;
		sk3CDVal = (int)Sk3CDTime;
		SetText(txtSk3CD,sk3CDVal);
	}
	#endregion

	/// <summary>
	/// Test Reset Data
	/// </summary>
	//public void ResetDataBtn()
	//{
	//	ResSvc.Instance.ResetSkillMoveData();
	//}

	public void ClickHeadBtn()
	{
		BattleSys.Instance.SetBattleEndWndState(FBEndType.Pause,true);
		BattleSys.Instance.battleMgr.isPauseGame = true;
	}
	public void SetPlayerHPBarVal(int val)
	{
		SetText(txtPlayerHP,val+"/"+playerHPSum);
		imgPlayerHP.fillAmount = val*1.0f/playerHPSum;
	}

	public Transform transBossHPBar;
	public Image imgRed;
	public Image imgYellow;
	public Text txtBoss;
	private float currentPrg = 1;
	private float targetPrg = 1;
	public void SetBossHPBarVal(int oldVal, int newVal, int sumVal)
	{
		currentPrg = oldVal * 1.0f / sumVal;
		targetPrg = newVal * 1.0f / sumVal;
		imgRed.fillAmount = targetPrg;
		
	}

	private void BlendBossHp()
	{
		if (Mathf.Abs(currentPrg - targetPrg) < Constants.UIAccelerSpeed * Time.deltaTime)
		{
			currentPrg = targetPrg;
		}
		else if (currentPrg>targetPrg)
		{
			currentPrg-=Constants.UIAccelerSpeed*Time.deltaTime;
		}
		else
		{
			currentPrg+=Constants.UIAccelerSpeed*Time.deltaTime;
		}
	}
	public void SetBossHpBarState(bool state = true,float prg=1)
	{
		SetActive(transBossHPBar,state);
		imgRed.fillAmount = prg;
		imgYellow.fillAmount = prg;
	}
}