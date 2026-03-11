/****************************************************
    文件：Constants.cs
	作者：www34
    邮箱: 
    日期：2025/10/10 21:25:35
	功能：常量配置
*****************************************************/

using UnityEngine;
public enum TxtColor
{
    Red,
    Green,
    Blue,
    Yellow
}

public enum EntityType
{
    None,
    Player,
    Monster,
}

public enum EntityState
{
    None,
    UnStoppableState
}

public enum MonsterType
{
    None,
    Boss
}
public class Constants
{
    private const string ColorRed =     "<color=#FF0000FF>";
    private const string ColorGreen =   "<color=#00FF00FF>";
    private const string ColorBlue =    "<color=#00B4FFFF>";
    private const string ColorYellow =  "<color=#FF0000FF>";
    private const string ColorEnd =     "</color>";
    public static string SetTxtColor(string txt,TxtColor txtColor)
    {
        string result = "";
        switch (txtColor)
        {
            case TxtColor.Red:
                result = ColorRed + txt + ColorEnd;
                break;
            case TxtColor.Green:
                result = ColorGreen + txt + ColorEnd;
                break;
            case TxtColor.Blue:
                result = ColorBlue + txt + ColorEnd;
                break;
            case TxtColor.Yellow:
                result = ColorYellow + txt + ColorEnd;
                break;
        }
        return result;
    }
    public const string SceneLogin = "SceneLogin";
    //public const string SceneMainCity = "SceneMainCity";
    public const int SceneMainCityId = 10000;

    public const string BGLoginMusic = "bgLogin";
    public const string BGLMainCity = "bgMainCity";
    public const string BGHuangYe= "bgHuangYe";
    
    //玩家受击音效
    public const string AssassinHit = "assassin_Hit";
    
    //登录音效
    public const string UILoginMusic = "uiLoginBtn";
    //常规ui点击音效
    public const string UIClickMusic = "uiClickBtn";
    public const string UIExtenMusic = "uiExtenBtn";
    //
    public const string UIOpenPage = "uiOpenPage";
    public const string FBItemEnter = "fbitem";
    public const string FBLose = "fblose";
    public const string FBWin = "fbwin";
    
    //
    public const int WindowStandardWidth = 1920;
    public const int WindowStandardHeight = 1080;
    //摇杆点标准操作距离
    public const int ScreenOpDis = 90;
    //
    public const int playerMoveSpeed = 8;
    public const int MonsterMoveSpeed = 4;
    //运动平滑
    public const float AccelerSpeed = 5f;
    public const float UIAccelerSpeed = 0.2f;
    //混合参数
    public const float BlendMove = 1f;
    public const float BlendIdle = 0f;

    //Action 触发参数
    public const int ActionDefault = -1;
    public const int ActionBorn = 0;
    public const int ActionDie = 100;
    public const int ActionHit = 101;
    
    //连招有效间隔
    public const int ComboSpace = 500;
    
    
    public const int DieAniLength = 5000;
    //AutoGuideNPC
    public const int NPCWiseman = 0;
    public const int NPCGeneral = 1;
    public const int NPCArtisan = 2;
    public const int NPCTrader = 3;
    
    
}