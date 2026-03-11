/****************************************************
    文件：PlayerController.cs
	作者：www34
    邮箱: 
    日期：2025/10/21 18:33:53
	功能：表现实体角色控制器
*****************************************************/

using UnityEngine;

public class PlayerController : Controller 
{
    //public Animator ani;

    #region FX

    public GameObject daggerSkill1FX;
    public GameObject daggerSkill2FX;
    public GameObject daggerSkill3FX;
    
    public GameObject daggerAttack1FX;
    public GameObject daggerAttack2FX;
    public GameObject daggerAttack3FX;
    public GameObject daggerAttack4FX;
    public GameObject daggerAttack5FX;

    #endregion
    
    
    public Vector3 cameraOffSet;
    
    private float tarBlend;
    private float currentBlend;
    
    public override void Init()
    {
        base.Init();
        if (Camera.main != null) cameraTrans = Camera.main.transform;
        cameraOffSet = transform.position - cameraTrans.position;
        if (daggerSkill1FX != null) 
        {
            FXDic.Add(daggerSkill1FX.name, daggerSkill1FX);
        }
        if (daggerSkill2FX != null) 
        {
            FXDic.Add(daggerSkill2FX.name, daggerSkill2FX);
        }
        if (daggerSkill3FX != null) 
        {
            FXDic.Add(daggerSkill3FX.name, daggerSkill3FX);
        }
        if (daggerAttack1FX != null) 
        {
            FXDic.Add(daggerAttack1FX.name, daggerAttack1FX);
        }
        if (daggerAttack2FX != null) 
        {
            FXDic.Add(daggerAttack2FX.name, daggerAttack2FX);
        }
        if (daggerAttack3FX != null) 
        {
            FXDic.Add(daggerAttack3FX.name, daggerAttack3FX);
        }
        if (daggerAttack4FX != null) 
        {
            FXDic.Add(daggerAttack4FX.name, daggerAttack4FX);
        }
        if (daggerAttack5FX != null) 
        {
            FXDic.Add(daggerAttack5FX.name, daggerAttack5FX);
        }
    }
    private void Update()
    {
        #region Input
        /*
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
        */
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

        if (!skillMove) return;
        SetSkillMove();
        CameraFollow();
    }
    private void SetMove()
    {
        controller.Move(transform.forward * (Time.deltaTime * Constants.playerMoveSpeed));
    }
    private void SetSkillMove()
    {
        controller.Move(transform.forward * (Time.deltaTime * skillMoveSpeed));
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
    
    public override void SetBlend(float blend)
    {
        tarBlend = blend;
    }

    public override void SetFX(string fx, int delay)
    {
        GameObject go = null;
        if (FXDic.TryGetValue(fx, out go))
        {
            go.SetActive(true);
            timerSvc.AddTimeTask(i =>
            {
                go.SetActive(false);
            }, delay, PETimeUnit.Millisecond, 1);
        }

    }
}