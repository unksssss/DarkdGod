using MySql.Data.MySqlClient;
using PEProtocol;
using System;

public class DBMgr
    {   

    private static DBMgr instance = null;
    public static DBMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DBMgr();
            }
            return instance;
        }
    }
    private MySqlConnection connection;
    public void Init()
    {
        connection = new MySqlConnection("server=localhost;User Id= root;password=;Database=darkgod;Charset = utf8");
        connection.Open();
        PECommon.Log("DBMgr Init Done");
        //QueryPlayerData("llll","oooo");
    }
    public PlayerData QueryPlayerData(string acct, string pass)
    {
        bool isNew = true;
        PlayerData playerData = null;
        //TODO
        MySqlDataReader reader= null;
        try
        {
            MySqlCommand mySqlCommand = new MySqlCommand("select * from account where acct = @acct", connection);
            mySqlCommand.Parameters.AddWithValue("acct", acct);
            reader = mySqlCommand.ExecuteReader();  
            if (reader.Read())
            {
                isNew = false;
                string _pass = reader.GetString("pass");
                if (_pass.Equals(pass))
                {
                    //密码正确。返回玩家信息
                    playerData = new PlayerData
                    {
                        id = reader.GetInt32("id"),
                        name = reader.GetString("name"),
                        lv = reader.GetInt32("level"),
                        exp = reader.GetInt32("exp"),
                        power = reader.GetInt32("power"),
                        coin = reader.GetInt32("coin"),
                        diamond = reader.GetInt32("diamond"),
                        crystal = reader.GetInt32("crystal"),
                        hp = reader.GetInt32("hp"),
                        ad = reader.GetInt32("ad"),
                        ap = reader.GetInt32("ap"),
                        addef = reader.GetInt32("addef"),
                        apdef = reader.GetInt32("apdef"),
                        dodge = reader.GetInt32("dodge"),
                        pierce = reader.GetInt32("pierce"),
                        critical = reader.GetInt32("critical"),
                        
                        guideid = reader.GetInt32("guideid"),
                        time = reader.GetInt64("time"),
                        fuben=reader.GetInt32("fuben"),
                        //TOADD
                    };
                    #region Strong
                    /*strong: 1#2#2#3#5#3#
                    strong数组，下标表示pos,值为当前的星级
                    在数据库中以字符串存储，用#分割
                    */
                    string[] strongstrArr=reader.GetString("strong").Split('#');
                    int[] _strongArr = new int[6];
                    for (int i = 0; i <_strongArr.Length; i++)
                    {
                        if (strongstrArr[i] == "")
                        {
                            continue;
                        }

                        if (int.TryParse(strongstrArr[i], out int statlv))
                        {
                            _strongArr[i] = statlv;
                        }
                        else
                        {
                            PECommon.Log("Parse Strong Arr Error",LogType.Error);
                        }
                    }
                    playerData.strong = _strongArr;
                    #endregion

                    #region taskArr解析
                    //数据示意：1|1|0#2|1|0#3|1|0#4|1|0#5|1|0#6|1|0#
                    //任务id|任务进度|是否已领取，0为未领取，
                    string[] taskstrArr=reader.GetString("task").Split('#');
                    playerData.taskArr=new string[6];
                    for (int i = 0; i < taskstrArr.Length; i++)
                    {
                        if (taskstrArr[i] == "")
                        {
                            continue;
                        }
                        if (taskstrArr[i].Length >= 5)
                        {
                            playerData.taskArr[i]=taskstrArr[i];
                        }
                        else
                        {
                            throw new Exception(" Task Error");
                        }
                    }
                    #endregion
                    //TOADD
                }
            }
        }
        catch (System.Exception ex)
        {
            PECommon.Log("Query playerData by acct&pass Error" + ex, LogType.Error);

        }
        finally
        {
            if (reader != null)
            {
                reader.Close();
            }
            if (isNew)
            {
                //不存在账号信息，创建新的默认账号信息并返回
                playerData = new PlayerData
                {
                    id=-1,
                    name="",
                    lv=1,
                    exp=1,
                    power=150,
                    coin=5000,
                    diamond=500,
                    crystal=500,
                    
                    hp = 2000,
                    ad = 275,
                    ap = 265,
                    addef = 67,
                    apdef = 43,
                    dodge = 7,
                    pierce = 5,
                    critical = 2,
                    
                    guideid = 1001,
                    strong = new int[6],
                    time=TimerSvc.Instance.GetNowTime(),
                    taskArr = new string[6],
                    fuben=10001
                    //TOADD
                };
                //数据示意：1|0|0#2|0|0#3|0|0#4|0|0#5|0|0#6|0|0#
                //任务id|任务进度|是否已领取，0为未领取，
                for (int i = 0; i < playerData.taskArr.Length; i++)
                {
                    playerData.taskArr[i] = (i + 1) + "|0|0";
                }
                //为主键赋值
                playerData.id = InsertNewPlayerData(acct, pass, playerData);

            }
        }
        return playerData;
    }
    private int InsertNewPlayerData(string acct, string pass, PlayerData pd)
    {
        int id = -1;
        try
        {
            MySqlCommand cmd =
                new MySqlCommand("insert into account set acct=@acct,pass =@pass,name=@name,level=@level,exp=@exp,power=@power,coin=@coin,diamond=@diamond,crystal=@crystal," +
                "hp=@hp,ad=@ad,ap=@ap,addef=@addef,apdef=@apdef,dodge=@dodge,pierce=@pierce,critical=@critical,guideid=@guideid,strong=@strong,time=@time,task=@task,fuben=@fuben", connection);
            cmd.Parameters.AddWithValue("acct", acct);
            cmd.Parameters.AddWithValue("pass", pass);
            cmd.Parameters.AddWithValue("name", pd.name);
            cmd.Parameters.AddWithValue("level", pd.lv);
            cmd.Parameters.AddWithValue("exp", pd.exp);
            cmd.Parameters.AddWithValue("power", pd.power);
            cmd.Parameters.AddWithValue("coin", pd.coin);
            cmd.Parameters.AddWithValue("diamond", pd.diamond);
            cmd.Parameters.AddWithValue("crystal", pd.crystal);
            
            cmd.Parameters.AddWithValue("hp", pd.hp);
            cmd.Parameters.AddWithValue("ad", pd.ad);
            cmd.Parameters.AddWithValue("ap", pd.ap);
            cmd.Parameters.AddWithValue("addef", pd.addef);
            cmd.Parameters.AddWithValue("apdef", pd.apdef);
            cmd.Parameters.AddWithValue("dodge", pd.dodge);
            cmd.Parameters.AddWithValue("pierce", pd.pierce);
            cmd.Parameters.AddWithValue("critical", pd.critical);
            cmd.Parameters.AddWithValue("guideid", pd.guideid);
            string stronginfo = "";
            for (int i = 0; i < pd.strong.Length; i++)
            {
                stronginfo += pd.strong[i];
                stronginfo += "#";
            }
            cmd.Parameters.AddWithValue("strong", stronginfo);
            cmd.Parameters.AddWithValue("time", pd.time);
            string taskinfo = "";
            for (int i = 0; i < pd.taskArr.Length; i++)
            {
                taskinfo += pd.taskArr[i];
                taskinfo += "#";
            }
            cmd.Parameters.AddWithValue("task", taskinfo);
            cmd.Parameters.AddWithValue("fuben", pd.fuben);
            //TOADD
            cmd.ExecuteNonQuery();
            id = (int)cmd.LastInsertedId;
        }
        catch (System.Exception ex)
        {
            PECommon.Log("Insert playerData by acct&pass Error" + ex, LogType.Error);
        }
        return id;
    }
    public bool QueryNameData(string name)
    {
        bool isexist=false;
        MySqlDataReader reader = null;
        try
        {
            MySqlCommand cmd = new MySqlCommand("select * from account where name= @name", connection);
            cmd.Parameters.AddWithValue("name", name);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                isexist = true;
            }
        }
        catch (System.Exception ex)
        {
            PECommon.Log("Query name data Error" + ex, LogType.Error);

        }
        finally
        {
            if (reader != null)
            {
                reader.Close();
            }
        }
        return isexist;

    }
    public bool UpdataPlayerData(int id, PlayerData playerData)
    {
        try
        {
            MySqlCommand cmd = new MySqlCommand(
            "update account set name=@name,level=@level,exp=@exp,power=@power,coin=@coin,diamond=@diamond,crystal=@crystal,hp=@hp" +
            ",ad=@ad,ap=@ap,addef=@addef,apdef=@apdef,dodge=@dodge,pierce=@pierce,critical=@critical,guideid=@guideid," +
            "strong=@strong,time=@time,task=@task,fuben=@fuben where id =@id"
            
                ,connection);
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("name", playerData.name);
            cmd.Parameters.AddWithValue("level", playerData.lv);
            cmd.Parameters.AddWithValue("exp", playerData.exp);
            cmd.Parameters.AddWithValue("power", playerData.power);
            cmd.Parameters.AddWithValue("coin", playerData.coin);
            cmd.Parameters.AddWithValue("diamond", playerData.diamond);
            cmd.Parameters.AddWithValue("crystal", playerData.crystal);
            
            cmd.Parameters.AddWithValue("hp", playerData.hp);
            cmd.Parameters.AddWithValue("ad", playerData.ad);
            cmd.Parameters.AddWithValue("ap", playerData.ap);
            cmd.Parameters.AddWithValue("addef", playerData.addef);
            cmd.Parameters.AddWithValue("apdef", playerData.apdef);
            cmd.Parameters.AddWithValue("dodge", playerData.dodge);
            cmd.Parameters.AddWithValue("pierce", playerData.pierce);
            cmd.Parameters.AddWithValue("critical", playerData.critical);
            cmd.Parameters.AddWithValue("guideid", playerData.guideid);
            string stronginfo = "";
            for (int i = 0; i < playerData.strong.Length; i++)
            {
                stronginfo += playerData.strong[i];
                stronginfo += "#";
            }
            string taskinfo = "";
            for (int i = 0; i < playerData.taskArr.Length; i++)
            {
                taskinfo += playerData.taskArr[i];
                taskinfo += "#";
            }
            cmd.Parameters.AddWithValue("strong", stronginfo);
            cmd.Parameters.AddWithValue("time", playerData.time);
            cmd.Parameters.AddWithValue("task", taskinfo);
            cmd.Parameters.AddWithValue("fuben", playerData.fuben);
            //TOADD Others
            cmd.ExecuteNonQuery();
        }
        catch (System.Exception ex)
        {
            PECommon.Log("Updata Player Data Error" + ex, LogType.Error);
            return false;
        }
        return true;
    }
}

