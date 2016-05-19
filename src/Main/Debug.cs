using System;
using System.Collections.Generic;
using System.Text;

namespace Spritely
{
	public class Debug
	{
		static TraceForm m_form = new TraceForm();

		private static bool m_fTraceFormVisible = false;

		public static bool TraceFormVisible
		{
			get { return m_fTraceFormVisible; }
			set
			{
				m_fTraceFormVisible = value;
				if (!m_fTraceFormVisible)
					m_form.Hide();
				else
					m_form.Show();
			}
		}

		/// <summary>
		/// Id of the last trace message displayed.
		/// </summary>
		static int m_lastTraceId = 0;

		public static void ClearTrace()
		{
			m_form.Clear();
			m_lastTraceId = 0;
		}

		/// <summary>
		/// Write out a trace message.
		/// </summary>
		/// <param name="strFormat"></param>
		/// <param name="args"></param>
		public static void Trace(string strFormat, params object[] args)
		{
			string strMessage = String.Format(strFormat, args);
			m_form.AddTrace(strMessage);
			m_lastTraceId = 0;
		}

		/// <summary>
		/// Write a trace message but don't repeat the message if the id is the same as the last message.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="strFormat"></param>
		/// <param name="args"></param>
		public static void TraceNoRepeat(int id, string strFormat, params object[] args)
		{
			if (id == m_lastTraceId)
				return;
			string strMessage = String.Format(strFormat, args);
			m_form.AddTrace(strMessage);
			m_lastTraceId = id;
		}
	}
}
