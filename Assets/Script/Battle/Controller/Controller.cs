// time:2025/11/22/15
// path:E:\unityproject\DarkGod\Assets\Script\Battle\Controller\Controller.csController.cs
// func:表现实体控制器抽象基类
// www34


using System.Collections.Generic;
using UnityEngine;

public abstract class Controller:MonoBehaviour
{
    public Animator ani;
    public CharacterController controller;
    protected Transform cameraTrans;
    protected bool isMove=false;
    protected Vector2 dir = Vector2.zero;
    public Vector2 Dir
    {
        set
        {
            if (value == Vector2.zero)
            {
                isMove = false;
            }
            else
            {
                isMove = true;
            }
            dir = value;
        }
    }

    public void SetDetectCollisions(bool isdetect)
    {
        controller.detectCollisions = isdetect;
    }

    protected bool skillMove = false;
    protected float skillMoveSpeed = 0f;
    protected Dictionary<string,GameObject> FXDic=new Dictionary<string, GameObject>();
    protected TimerSvc timerSvc;

    public virtual void Init()
    {
        timerSvc=TimerSvc.Instance;
    }
    public virtual void SetBlend(float blend)
    {
        ani.SetFloat("Blend", blend);
    }

    
    public void SetAction(int action)
    {
        ani.SetInteger("Action", action);
    }

    public virtual void SetFX(string fx, int delay)
    {

    }

    public void SetSkillMoveState(bool move, float skillSpeed=0f)
    {
        skillMove = move;
        skillMoveSpeed = skillSpeed;
    }

    public virtual void SetAtkRotCam(Vector2 atkRot)
    {
        float angle = Vector2.SignedAngle(atkRot, new Vector2(0, 1))+cameraTrans.localEulerAngles.y;
        Vector3 eulerAngle = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngle;
    }
    public virtual void SetAtkRotLocal(Vector2 atkRot)
    {
        float angle = Vector2.SignedAngle(atkRot, new Vector2(0, 1));
        Vector3 eulerAngle = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngle;
    }
}