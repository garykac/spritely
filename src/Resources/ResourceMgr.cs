using System;
using System.Collections.Generic;
using System.Drawing;
using System.Resources;
using System.Text;

namespace Spritely
{
	public static class ResourceMgr
	{
		static ResourceManager m_rm = new ResourceManager("Spritely.Resources.Resources",
								System.Reflection.Assembly.GetExecutingAssembly());

		public static string GetString(string strName)
		{
			return m_rm.GetString(strName);
		}

		public static Bitmap GetBitmap(string strName)
		{
			return (Bitmap)m_rm.GetObject(strName);
		}
	}
}
