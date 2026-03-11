// time:2025/12/01/14
// path:E:\unityproject\DarkGod\Assets\Script\Battle\Controller\MonsterController.csMonsterController.cs
// func:怪物表现实体角色控制器类
// www34

using System;
using UnityEngine;

public class MonsterController:Controller
{
    private void Update()
    {
        if (isMove)
        {
            SetDir();
            SetMove();
        }
    }
    private void SetDir()
    {
        float angle = Vector2.SignedAngle(dir, new Vector2(0, 1));
        Vector3 eulerAngle = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngle;
    }
    private void SetMove()
    {
        controller.Move(transform.forward * (Time.deltaTime * Constants.MonsterMoveSpeed));
        //模拟重力
        controller.Move(Vector3.down * (Time.deltaTime * Constants.MonsterMoveSpeed));
    }
}