/****************************************************
    文件：ItemEntityHP.cs
	作者：www34
    邮箱: 
    日期：2025/12/6 19:32:22
	功能：血条物体
*****************************************************/

using System;
using NUnit.Framework.Internal.Commands;
using UnityEngine;
using UnityEngine.UI;

public class ItemEntityHP : MonoBehaviour
{
	#region UIDefine

	public Image imgHPGray;
	public Image imgHPRed;
	
	
	public Animation criticalAni;
	public Text txtCritical;
	
	
	public Animation dodgeAni;
	public Text txtDodge;
	
	public Animation HPAni;
	public Text txtHP;

	#endregion

	private int HPValve;
	private Transform tranRoot;
	private RectTransform rectTran;
	private float ScreemScale=1.0f*Constants.WindowStandardHeight/Screen.height;
	private void Update()
	{
		Vector3 screenPoint =Camera.main.WorldToScreenPoint(tranRoot.position);
		rectTran.position = screenPoint*ScreemScale;

		if (currentPrg == targetPrg) return;
		UpdateMixBlend();
		imgHPGray.fillAmount = currentPrg;
	}

	private void UpdateMixBlend()
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
	
	public void SetCritical(int critical)
	{
		criticalAni.Stop();
		txtCritical.text = "暴击" + critical;
		criticalAni.Play();
	}

	public void SetDodge()
	{
		dodgeAni.Stop();
		txtDodge.text="闪避";
		dodgeAni.Play();
	}

	public void SetHurt(int hurt)
	{
		HPAni.Stop();
		txtHP.text="-" + hurt;
		HPAni.Play();
		
	}

	public void InitItemHp(Transform trans,int hp)
	{
		rectTran = GetComponent<RectTransform>();
		tranRoot=trans;
		HPValve = hp;
		imgHPRed.fillAmount = 1;
		imgHPGray.fillAmount = 1;
	}

	private float currentPrg;
	private float targetPrg;
	public void SetHPVal(int oldVal, int newVal)
	{
		currentPrg=oldVal*1.0f/HPValve;
		targetPrg=newVal*1.0f/HPValve;
		imgHPRed.fillAmount = targetPrg;
	}
}