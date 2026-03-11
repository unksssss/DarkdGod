/****************************************************
    文件：TestPlayer.cs
    作者：www34
    邮箱:
    日期：2025/11/24 14:19:24
    功能：Nothing
*****************************************************/

using System;
using System.Collections;
using UnityEngine;

public class TestPlayer : Controller 
{
    public CharacterController controller;

    public GameObject daggerSkill1FX;
    private Transform cameraTrans;
    private Vector3 cameraOffSet;
    
    private float tarBlend;
    private float currentBlend;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        if (Camera.main != null) cameraTrans = Camera.main.transform;
        cameraOffSet = transform.position - cameraTrans.position;
    }
    private void Update()
    {
        #region Input
        
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 _dir = new Vector2(h, v).normalized;
        if(_dir!=Vector2.zero)
        {
            Dir = _dir;
            SetBlend(Constants.BlendMove);

        }
        else
        {
            Dir = Vector2.zero;
            SetBlend(Constants.BlendIdle);
        }
        #endregion
        if(currentBlend!=tarBlend)
        {
            UpdateMixedBlend();

        }
        //设置移动
        if(isMove){
            SetMove();

            //设置方向
            SetDir();

            //相机跟随\
            CameraFollow();
        }

    }
    private void SetMove()
    {
        controller.Move(transform.forward * (Time.deltaTime * Constants.playerMoveSpeed));
    }
    private void SetDir()
    {
        float angle = Vector2.SignedAngle(dir, new Vector2(0, 1))+cameraTrans.localEulerAngles.y;
        Vector3 eulerAngle = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngle;
    }
    public void CameraFollow()
    {
        if (cameraTrans != null)
        {
            cameraTrans.position= transform.position - cameraOffSet;
        }
    }
    public override void SetBlend(float blend)
    {
        tarBlend = blend;
    }
    private void UpdateMixedBlend()
    {
        if (Mathf.Abs(currentBlend - tarBlend) < Constants.AccelerSpeed * Time.deltaTime)
        {
            currentBlend = tarBlend;
        }
        else if (currentBlend > tarBlend)
        {
            currentBlend -= Constants.AccelerSpeed * Time.deltaTime;
        }
        else
        {
            currentBlend += Constants.AccelerSpeed * Time.deltaTime;
        }
        ani.SetFloat("Blend", currentBlend);
    }

    public void ClickSkill1Btn()
    {
        ani.SetInteger("Action", 1);
        daggerSkill1FX.SetActive(true);
        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.9f);
        ani.SetInteger("Action", -1);
        daggerSkill1FX.SetActive(false);
        
    }
    
}