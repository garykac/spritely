using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class UnitTestForm : Form
	{
		public UnitTestForm()
		{
			InitializeComponent();
		}

		private void UnitTestForm_Load(object sender, EventArgs e)
		{
			RunTests();
		}

		private bool HasAttrib(Type tBase, Type tAttrib)
		{
			return tBase.GetCustomAttributes(tAttrib, false).Length != 0;
		}

		private bool HasAttrib(MethodInfo m, Type tAttrib)
		{
			return m.GetCustomAttributes(tAttrib, false).Length != 0;
		}

		public void RunTests()
		{
			int nTests = 0;
			int nFailedTests = 0;
			lbResults.Items.Clear();

			foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (HasAttrib(t, typeof(TestFixtureAttribute)))
				{
					// Gather test info.
					MethodInfo mFixtureSetup = null;
					MethodInfo mFixtureTearDown = null;
					MethodInfo mSetup = null;
					MethodInfo mTearDown = null;
					List<MethodInfo> mTests = new List<MethodInfo>();
					foreach (MethodInfo m in t.GetMethods())
					{
						if (HasAttrib(m, typeof(TestFixtureSetUpAttribute)))
							mFixtureSetup = m;

						if (HasAttrib(m, typeof(TestFixtureTearDownAttribute)))
							mFixtureTearDown = m;

						if (HasAttrib(m, typeof(SetUpAttribute)))
							mSetup = m;

						if (HasAttrib(m, typeof(TearDownAttribute)))
							mTearDown = m;

						if (HasAttrib(m, typeof(TestAttribute)))
							mTests.Add(m);
					}

					// Run tests
					object obj = Activator.CreateInstance(t);

					if (mFixtureSetup != null)
						mFixtureSetup.Invoke(obj, null);

					foreach (MethodInfo m in mTests)
					{
						nTests++;
						bool fFailed = false;
						string strTestInfo = String.Format("{0} :: {1}...", t.Name, m.Name);
						lbResults.Items.Add(strTestInfo);
						lbResults.SelectedIndex = lbResults.Items.Count - 1;

						if (mSetup != null)
							mSetup.Invoke(obj, null);

						try
						{
							m.Invoke(obj, null);
						}
						catch (Exception)
						{
							fFailed = true;
						}

						if (mTearDown != null)
							mTearDown.Invoke(obj, null);

						lbResults.Items.RemoveAt(lbResults.Items.Count-1);
						if (fFailed)
						{
							nFailedTests++;
							lbResults.Items.Add(String.Format("{0}   FAILED", strTestInfo));
						}
						else
						{
							lbResults.Items.Add(String.Format("{0}   Success", strTestInfo));
						}
						lbResults.SelectedIndex = lbResults.Items.Count - 1;
					}

					if (mFixtureTearDown != null)
						mFixtureTearDown.Invoke(obj, null);
				}
			}

			lbResults.Items.Add(String.Format("{0} tests run. {1} failed", nTests, nFailedTests));
			lbResults.SelectedIndex = lbResults.Items.Count - 1;
		}

	}
}
