using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/********************************************************************
	created:	2025/10/13
	created:	13:10:2025   21:24
	filename: 	E:\unityproject\DarkGod\Assets\Script\Common\PETool.cs
	file path:	E:\unityproject\DarkGod\Assets\Script\Common
	file base:	PETool
	file ext:	cs
	author:		
	
	purpose:	工具类
*********************************************************************/

     public class PETool
    {
		public static int RDInt(int min,int max,Random r=null)
		{
		if (r == null)
		{
			r=new Random();
		}
		int val=r.Next(min,max+1);
		return val;
		}
    }

