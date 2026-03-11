// time:2025/11/22/15
// path:E:\unityproject\DarkGod\Assets\Script\Battle\Entity\PlayerEntity.csPlayerEntity.cs
// func:玩家逻辑实体
// www34


using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class PlayerEntity: EntityBase
{
    public PlayerEntity()
    {
        entityType=EntityType.Player;
    }
    public override Vector2 GetCurrentDir()
    {
        return battleMgr.GetCurrentDir();
    }

    public override Vector2 CalcTargetDir()
    {
        MonsterEntity target = GetClosedTarget();
        if (target != null)
        {
            Vector3 targetPos = target.GetPos();
            Vector3 self = GetPos();
            Vector2 dir=new Vector2(targetPos.x - self.x, targetPos.z - self.z);
            return dir.normalized;
        }

        return Vector2.zero;
    }

    private MonsterEntity GetClosedTarget()
    {
        MonsterEntity target = null;
        List<MonsterEntity> list = battleMgr.GetMonsterEntityList();
        if (list.Count == 0 || list == null)
        {
            return null;
        }

        Vector3 self = GetPos();
        float dis=0;
        for (int i = 0; i < list.Count; i++)
        {
            Vector3 targetPos=list[i].GetPos();
            if (i == 0)
            {
                dis = Vector3.Distance(self, targetPos);
                target=list[i];
            }
            float _dis = Vector3.Distance(self, targetPos);
            if (dis > _dis)
            {
                dis = _dis;
                target=list[i];
            }
        }
        return target;
    }

    public override void SetHPVal(int oldVal, int newVal)
    {
        BattleSys.Instance.playerControlWnd.SetPlayerHPBarVal(newVal);
    }

    public override void SetDodge()
    {
        GameRoot.Instance.dynamicWnd.SetSelfDodge();
    }
}