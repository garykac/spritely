using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Spritely
{
	public partial class TutorialTestForm : Form
	{
		private Document m_doc = null;

		public TutorialTestForm(ProjectMainForm parent)
		{
			InitializeComponent();
		}

		private void UnitTestForm_Load(object sender, EventArgs e)
		{
			foreach (string filepath in Directory.GetFiles(Options.Debug_TutorialRawPath))
			{
				string filename = Path.GetFileName(filepath);
				if (!filename.StartsWith("_"))
					clbTests.Items.Add(filename);
			}
			cbGBA.Checked = true;
			cbNDS.Checked = true;
		}

		private void bSelectAll_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < clbTests.Items.Count; i++)
				clbTests.SetItemChecked(i, true);
		}

		private void bRun_Click(object sender, EventArgs e)
		{
			RunTests();
		}

		private void Print(string strMessage)
		{
			lbResults.Items.Add(strMessage);
			lbResults.SelectedIndex = lbResults.Items.Count - 1;
		}

		private void Print(string strFormat, params object[] args)
		{
			string strMessage = String.Format(strFormat, args);
			lbResults.Items.Add(strMessage);
			lbResults.SelectedIndex = lbResults.Items.Count - 1;
		}

		string CalcPath(string str1, string str2)
		{
			return String.Format("{0}{1}{2}", str1, Path.DirectorySeparatorChar.ToString(), str2);
		}

		string CalcPath(string[] path)
		{
			return String.Join(Path.DirectorySeparatorChar.ToString(), path);
		}

		void ClearDir(string strDir)
		{
			// Delete directory if it exists.
			if (Directory.Exists(strDir))
			{
				// Directory delete can fail if the directory is open in a window, so
				// we don't want to fail in this case.
				try
				{
					Directory.Delete(strDir, true);
				}
				catch { }
			}
			if (!Directory.Exists(strDir))
				Directory.CreateDirectory(strDir);
		}

		/// <summary>
		/// Recursively copy a directory.
		/// </summary>
		/// <param name="strSourceDir"></param>
		/// <param name="strTargetDir"></param>
		void CopyDir(string strSourceDir, string strTargetDir)
		{
			ClearDir(strTargetDir);

			foreach (string file in Directory.GetFiles(strSourceDir))
				File.Copy(file, CalcPath(strTargetDir, Path.GetFileName(file)), true);

			foreach (string dir in Directory.GetDirectories(strSourceDir))
			{
				string[] path = dir.Split(Path.DirectorySeparatorChar);
				CopyDir(dir, CalcPath(strTargetDir, path[path.Length-1]));
			}
		}

		private string m_strTutorialName = "unknown";
		private string m_strProjectName = "game";
		private string m_strAuthor = "";
		private bool m_fIncludeFileLinks = false;

		void RunTests()
		{
			if (!System.IO.Directory.Exists(Options.Debug_TutorialPath))
			{
				Print("Unable to find tutorials in {0}.", Options.Debug_TutorialPath);
				return;
			}

			if (!System.IO.Directory.Exists(Options.Debug_TutorialRawPath))
			{
				Print("No tutorials to process in {0}.", Options.Debug_TutorialRawPath);
				return;
			}

			int nTuts = 0;
			int nGbaTests = 0;
			int nBadGbaTests = 0;
			int nNdsTests = 0;
			int nBadNdsTests = 0;
			lbResults.Items.Clear();

			foreach (string filename in clbTests.CheckedItems)
			{
				nTuts++;
				RunTutorial(filename, ref nGbaTests, ref nBadGbaTests, ref nNdsTests, ref nBadNdsTests);
			}

			Print("{0} tutorials processed:", nTuts);
			if (nGbaTests == 0)
				Print("  {0} GBA tests run", nGbaTests);
			else
				Print("  {0} GBA tests run -- {1} failed", nGbaTests, nBadGbaTests);
			if (nNdsTests == 0)
				Print("  {0} NDS tests run", nNdsTests);
			else
				Print("  {0} NDS tests run -- {1} failed", nNdsTests, nBadNdsTests);
		}

		void RunTutorial(string filename, ref int nGbaTests, ref int nBadGbaTests, ref int nNdsTests, ref int nBadNdsTests)
		{
			Print("Processing {0}", filename);

			string strFilename = CalcPath(Options.Debug_TutorialRawPath, filename);
			string strTempHtmlOutput = CalcPath(Options.Debug_TutorialPath, "tut.html");
			m_strTutorialName = "unknown";
			m_strProjectName = "game";
			m_strAuthor = "";
			m_fIncludeFileLinks = false;

			try
			{
				using (TextWriter tw = new StreamWriter(strTempHtmlOutput))
				{
					if (cbGBA.Checked)
					{
						using (TextReader tr = new StreamReader(strFilename, Encoding.UTF8))
						{
							nGbaTests++;
							if (!RunTutorial2(tr, tw, false))
								nBadGbaTests++;
						}
					}
					if (cbNDS.Checked)
					{
						using (TextReader tr = new StreamReader(strFilename, Encoding.UTF8))
						{
							nNdsTests++;
							if (!RunTutorial2(tr, !cbGBA.Checked ? tw : null, true))
								nBadNdsTests++;
						}
					}

					if (m_fIncludeFileLinks)
						WriteFileLinks(tw);

					WriteTutorialFooter(tw);
				}
			}
			catch (Exception ex)
			{
				Print("Unable to process tutorial file {0}", ex.Message);
			}

			// Copy temp file over original and delete temp.
			string strHtmlOutput = CalcPath(Options.Debug_TutorialPath, m_strTutorialName + ".html");
			File.Copy(strTempHtmlOutput, strHtmlOutput, true);
			File.Delete(strTempHtmlOutput);
		}

		/// <summary>
		/// Really run the tutorial for either GBA or NDS.
		/// </summary>
		/// <param name="tr"></param>
		/// <param name="tw">The output TextWriter. Null if we shouldn't write output.</param>
		/// <param name="fNDS"></param>
		/// <returns></returns>
		private bool RunTutorial2(TextReader tr, TextWriter tw, bool fNDS)
		{
			// Make a new document and initialize it.
			m_doc = new Document(null);
			m_doc.InitializeEmptyDocument();

			string strWorkDir = "test_" + (fNDS ? "nds" : "gba");
			string strSourceFile = "";
			string strTitle = "";

			ClearDir(CalcPath(Options.Debug_TutorialPath, strWorkDir));

			bool fGatherLines = false;
			List<string> Lines = new List<string>();

			Match m;
			int nStep = 1;
			int nFind = 0;
			int nUpdate = 0;
			int nTutorial = 1;	// Used when processing the index.

			string strLine;
			while ((strLine = tr.ReadLine()) != null)
			{
				if (strLine.StartsWith("#"))
					continue;

				// This is used when debugging the tutorial to stop the tutorial at
				// a certain point.
				if (strLine == "STOP")
					return false;

				m = Regex.Match(strLine, @"^NAME\s+(.+)$");
				if (m.Success)
				{
					m_strTutorialName = m.Groups[1].Value;
					continue;
				}

				m = Regex.Match(strLine, @"^TITLE\s+(.+)$");
				if (m.Success)
				{
					strTitle = m.Groups[1].Value;
					WriteTutorialHeader(tw, strTitle);
					continue;
				}

				if (strLine == "VERIFIED")
				{
					WriteCompatibility(tw);
					continue;
				}

				m = Regex.Match(strLine, @"^CATEGORY\s+(.+)$");
				if (m.Success)
				{
					continue;
				}

				m = Regex.Match(strLine, @"^AUTHOR\s+(.+)$");
				if (m.Success)
				{
					m_strAuthor = m.Groups[1].Value;
					continue;
				}

				m = Regex.Match(strLine, @"^PROJECT_NAME\s+(.+)$");
				if (m.Success)
				{
					m_strProjectName = m.Groups[1].Value;
					continue;
				}

				m = Regex.Match(strLine, @"^IMAGE\s+(.+)$");
				if (m.Success)
				{
					WriteImage(tw, m.Groups[1].Value);
					continue;
				}

				m = Regex.Match(strLine, @"^STEP\s+(.+)$");
				if (m.Success)
				{
					WriteStep(tw, nStep, m.Groups[1].Value);
					nStep++;
					continue;
				}

				m = Regex.Match(strLine, @"^FINISH$");
				if (m.Success)
				{
					WriteFinish(tw);
					continue;
				}

				m = Regex.Match(strLine, @"^FILE_LINKS$");
				if (m.Success)
				{
					m_fIncludeFileLinks = true;
					continue;
				}

				// Tutorial link from within a tutorial page.
				m = Regex.Match(strLine, @"^TUTORIAL_LINK\s+(\w+)\s+(.+)$");
				if (m.Success)
				{
					WriteTutorialLink(tw, m.Groups[1].Value, m.Groups[2].Value);
					continue;
				}

				// Tutorial entry on index page.
				m = Regex.Match(strLine, "^TUTORIAL_INDEX_ENTRY\\s+\"(.+)\"\\s+\"(.+)\"\\s+\"(.+)\"$");
				if (m.Success)
				{
					WriteTutorialIndexEntry(tw, nTutorial, m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value);
					nTutorial++;
					continue;
				}

				m = Regex.Match(strLine, "^TUTORIAL_INDEX_SETUP_ENTRY\\s+\"(.+)\"\\s+\"(.+)\"\\s+\"(.+)\"$");
				if (m.Success)
				{
					WriteTutorialIndexSetupEntry(tw, m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value);
					continue;
				}

				m = Regex.Match(strLine, "^TUTORIAL_INDEX_SPACE$");
				if (m.Success)
				{
					WriteTutorialIndexSpace(tw);
					continue;
				}

				m = Regex.Match(strLine, @"^FIND\s+(.+)$");
				if (m.Success)
				{
					strSourceFile = m.Groups[1].Value;
					fGatherLines = true;
					Lines.Clear();
					nFind++;
					continue;
				}

				if (strLine == "END_FIND")
				{
					int nStartLine, nLines;
					if (!FindLines(strWorkDir, strSourceFile, Lines, out nStartLine, out nLines))
					{
						Print("Failed to match FIND #{0} in {1} at line {2}", nFind, strSourceFile, nLines);
						return false;
					}
					WriteSourceLines(tw, strSourceFile, nStartLine, nStartLine + nLines - 1, Lines, false);
					fGatherLines = false;
					continue;
				}

				m = Regex.Match(strLine, @"^UPDATE\s+(.+)$");
				if (m.Success)
				{
					strSourceFile = m.Groups[1].Value;
					fGatherLines = true;
					Lines.Clear();
					nUpdate++;
					continue;
				}

				if (strLine == "END_UPDATE")
				{
					int nStartLine, nLines;
					if (!UpdateLines(strWorkDir, strSourceFile, Lines, out nStartLine, out nLines))
					{
						Print("Failed to match UPDATE #{0} in {1}", nUpdate, strSourceFile);
						return false;
					}
					WriteSourceLines(tw, strSourceFile, nStartLine, nStartLine + nLines - 1, Lines, true);
					fGatherLines = false;
					continue;
				}

				m = Regex.Match(strLine, @"^SPRITELY_ACTION\s+(.+)$");
				if (m.Success)
				{
					string strCommand = m.Groups[1].Value;
					if (!HandleSpritelyAction(strWorkDir, strCommand, fNDS))
					{
						Print("Failed to process {0} ({1})", strCommand, fNDS ? "nds" : "gba");
						return false;
					}
					continue;
				}

				if (strLine == "VERIFY_BUILD")
				{
					if (!VerifyBuild(strWorkDir))
					{
						Print("Build verify failed!");
						return false;
					}
					continue;
				}

				if (fGatherLines)
					Lines.Add(strLine);
				else if (tw != null)
					tw.WriteLine(strLine);
			}

			return true;
		}

		void WriteTutorialHeader(TextWriter tw, string strTitle)
		{
			if (tw == null)
				return;
			tw.WriteLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\"");
			tw.WriteLine("\"http://www.w3.org/TR/html4/loose.dtd\">");
			tw.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
			tw.WriteLine("");
			tw.WriteLine("<head>");
			tw.WriteLine(String.Format("<title>{0}</title>", strTitle));
			tw.WriteLine("<link href=\"css/tutorial.css\" type=\"text/css\" rel=\"stylesheet\" />");
			tw.WriteLine("<script type=\"text/javascript\" src=\"css/precodeformat.js\"></script>");
			tw.WriteLine("</head>");
			tw.WriteLine("");
			tw.WriteLine("<body onload=\"PreCodeFormat()\">");
			tw.WriteLine("<div id=\"content\">");
			tw.WriteLine(String.Format("<h1>{0}</h1>", strTitle));
		}

		void WriteTutorialFooter(TextWriter tw)
		{
			if (tw == null)
				return;
			tw.WriteLine("");
			tw.WriteLine("<div id=\"footer_bkgd\"><div id=\"footer\">");
			tw.WriteLine("<p>{0}</p>", m_strAuthor);
			tw.WriteLine("</div></div>");
			tw.WriteLine("");
			tw.WriteLine("</div>");
			WriteTrackingCode(tw);
			tw.WriteLine("</body>");
			tw.WriteLine("</html>");
		}

		/// <summary>
		/// Write Analytics Tracking code on tutorials so that we can track usage.
		/// </summary>
		/// <param name="tw"></param>
		void WriteTrackingCode(TextWriter tw)
		{
			if (tw == null)
				return;
			tw.WriteLine("");
			tw.WriteLine("<script type=\"text/javascript\">");
			tw.WriteLine("var gaJsHost = ((\"https:\" == document.location.protocol) ? \"https://ssl.\" : \"http://www.\");");
			tw.WriteLine("document.write(unescape(\"%3Cscript src='\" + gaJsHost + \"google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E\"));");
			tw.WriteLine("</script>");
			tw.WriteLine("<script type=\"text/javascript\">");
			tw.WriteLine("try {");
			tw.WriteLine("var pageTracker = _gat._getTracker(\"UA-1163903-2\");");
			tw.WriteLine("pageTracker._trackPageview();");
			tw.WriteLine("} catch(err) {}</script>");
			tw.WriteLine("");
		}

		void WriteCompatibility(TextWriter tw)
		{
			if (tw == null)
				return;
			tw.WriteLine("<p class=\"alert\">");
			tw.WriteLine("This tutorial has been tested with");
			tw.WriteLine(String.Format("<a href=\"http://www.devkitpro.org\">devkitARM release {0}</a>",
					Options.Debug_DevkitArmVersion));
			tw.WriteLine(String.Format("and <a href=\"http://code.google.com/p/spritely\">Spritely version {0}</a>",
					Options.VersionString));
			tw.WriteLine("and verified to work for both GBA and NDS projects.");
			tw.WriteLine("</p>");
		}

		void WriteImage(TextWriter tw, string strImageName)
		{
			if (tw == null)
				return;
			string strImagePath = CalcPath(m_strTutorialName, strImageName);
			if (!File.Exists(CalcPath(Options.Debug_TutorialPath, strImagePath)))
				Print("WARNING - Image {0} not found!", strImagePath);
			tw.WriteLine(String.Format("<p><img src=\"{0}/{1}\" /></p>", m_strTutorialName, strImageName));
		}

		void WriteStep(TextWriter tw, int nStep, string strStepName)
		{
			if (tw == null)
				return;
			tw.WriteLine("<h2>Step {0} : {1}</h2>", nStep, strStepName);
		}

		void WriteFinish(TextWriter tw)
		{
			if (tw == null)
				return;
			tw.WriteLine("<h2>Finished!</h2>");
		}

		void WriteTutorialIndexSetupEntry(TextWriter tw, string strCategory, string strFile, string strTitle)
		{
			if (tw == null)
				return;
			tw.WriteLine("<tr>");
			tw.WriteLine("<td style=\"border: 1px solid #aaa; padding: 5px;\"> </td>");
			tw.WriteLine("<td style=\"border: 1px solid #aaa; padding: 5px;\"> {0} </td>", strCategory);
			tw.WriteLine("<td style=\"border: 1px solid #aaa; padding: 5px;\"> <strong><a href=\"{0}.html\">{1}</a></strong> </td>", strFile, strTitle);
			tw.WriteLine("</tr>");
		}

		void WriteTutorialIndexEntry(TextWriter tw, int nTutorial, string strCategory, string strFile, string strTitle)
		{
			if (tw == null)
				return;
			tw.WriteLine("<tr>");
			tw.WriteLine("<td style=\"border: 1px solid #aaa; padding: 5px;\"> Tutorial {0} </td>", nTutorial);
			tw.WriteLine("<td style=\"border: 1px solid #aaa; padding: 5px;\"> {0} </td>", strCategory);
			if (strFile == "???")
				tw.WriteLine("<td style=\"border: 1px solid #aaa; padding: 5px;\"> {0} </td>", strTitle);
			else
				tw.WriteLine("<td style=\"border: 1px solid #aaa; padding: 5px;\"> <strong><a href=\"{0}.html\">{1}</a></strong> </td>", strFile, strTitle);
			tw.WriteLine("</tr>");
		}

		void WriteTutorialIndexSpace(TextWriter tw)
		{
			if (tw == null)
				return;
			tw.WriteLine("<tr>");
			tw.WriteLine("<td style=\"padding: 5px;\"> </td>");
			tw.WriteLine("<td style=\"padding: 5px;\"> </td>");
			tw.WriteLine("<td style=\"padding: 5px;\"> </td>");
			tw.WriteLine("</tr>");
		}

		void WriteTutorialLink(TextWriter tw, string strTutorial, string strTitle)
		{
			if (tw == null)
				return;
			tw.WriteLine(String.Format("<a href=\"{0}.html\">{1}</a>", strTutorial, strTitle));
		}

		void WriteFileLinks(TextWriter tw)
		{
			if (tw == null)
				return;
			tw.WriteLine("<div id=\"filelink_bkgd\"><div id=\"filelink\">");
			tw.WriteLine("<h1>Links to completed project</h1>");
			WriteFileLinks2(tw, "GBA", "gba");
			WriteFileLinks2(tw, "NDS", "nds");
			tw.WriteLine("</div></div>");
		}

		void WriteFileLinks2(TextWriter tw, string strTitle, string strPlatform)
		{
			if (tw == null)
				return;
			string strSourcePath = CalcPath(new string[] { Options.Debug_TutorialPath, "test_" + strPlatform });
			string strTargetPath = CalcPath(new string[] { Options.Debug_TutorialPath, m_strTutorialName, strPlatform });
			if (!Directory.Exists(strTargetPath))
				Directory.CreateDirectory(strTargetPath);

			tw.WriteLine("<p>{0}:</p>", strTitle);
			tw.WriteLine("<ul>");
			WriteSourceFileLink(tw, "game_state.h", strPlatform, strSourcePath, strTargetPath);
			WriteSourceFileLink(tw, "game_state.cpp", strPlatform, strSourcePath, strTargetPath);
			WriteTargetFileLink(tw, strPlatform, strSourcePath, strTargetPath);
			tw.WriteLine("</ul>");
		}

		void WriteSourceFileLink(TextWriter tw, string strFileName, string strPlatform, string strSourcePath, string strTargetPath)
		{
			if (tw == null)
				return;
			File.Copy(CalcPath(new string[] { strSourcePath, "source", strFileName }),
				CalcPath(strTargetPath, strFileName), true);
			tw.WriteLine(String.Format("<li><a href=\"{0}/{1}/{2}\">{3}</a></li>", m_strTutorialName, strPlatform, strFileName, strFileName));
		}

		void WriteTargetFileLink(TextWriter tw, string strPlatform, string strSourcePath, string strTargetPath)
		{
			if (tw == null)
				return;
			string strTarget = m_strProjectName + "." + strPlatform;
			File.Copy(CalcPath(strSourcePath, "test_"+strPlatform+"."+strPlatform),
				CalcPath(strTargetPath, strTarget), true);
			tw.WriteLine(String.Format("<li><a href=\"{0}/{1}/{2}\">{3}</a></li>", m_strTutorialName, strPlatform, strTarget, strTarget));
		}

		void WriteSourceLines(TextWriter tw, string strFilename, int nStartLine, int nEndLine,
			List<string> Lines, bool fUpdate)
		{
			if (tw == null)
				return;

			tw.Write(String.Format("<p class=\"filename\"><code><b>{0}</b></code>&nbsp;&nbsp;&mdash;&nbsp;&nbsp;",
				strFilename));
			if (nStartLine == nEndLine)
				tw.WriteLine(String.Format("Line {0}:</p>", nStartLine));
			else
				tw.WriteLine(String.Format("Lines {0} - {1}:</p>", nStartLine, nEndLine));
			tw.WriteLine("<pre class=\"code\">");

			foreach (string line in Lines)
			{
				string strPrefix = line.Substring(0, 1);
				string strCode = FixupHtml(line.Substring(1));
				if (strPrefix == ".")
				{
					if (fUpdate)
					{
						// The disabled tag must come after any leading tabs
						// to ensure proper tab processing.
						string strTabs = "";
						while (strCode.Length > 1 && strCode[0] == '\t')
						{
							strTabs += strCode[0];
							strCode = strCode.Substring(1);
						}
						// Make sure strTabs and str are not both blank.
						// IE6/7 won't render them correctly.
						if (strTabs == "" && strCode == "")
							strTabs = "\t";
						tw.WriteLine(strTabs + "<disabled/>" + strCode);
					}
					else
						tw.WriteLine(strCode);
				}
				else if (strPrefix == "+")
					tw.WriteLine("<mark type=\"plus\"/>" + strCode);
				else if (strPrefix == "x")
					tw.WriteLine("<mark type=\"cross\"/>" + strCode);
				else if (strPrefix == ">" || strPrefix == "*")
					tw.WriteLine("<mark type=\"arrow\"/>" + strCode);
				//else starts with "-", which we don't display
			}

			tw.WriteLine("</pre>");
		}

		/// <summary>
		/// Fixup punct marks in the code so that they display correctly in HTML.
		/// </summary>
		/// <param name="strCode"></param>
		/// <returns></returns>
		string FixupHtml(string strCode)
		{
			strCode = strCode.Replace("&", "&amp;");
			strCode = strCode.Replace("<", "&lt;");
			strCode = strCode.Replace(">", "&gt;");
			return strCode;
		}

		bool FindLines(string strProjectName, string strFilename, List<string> Lines, out int nSourceStartLine, out int nSourceNumLines)
		{
			nSourceStartLine = 0;
			nSourceNumLines = 0;

			bool fScan = true;
			bool fForceMatch = false;
	
			int nLineNum = 0;
			int nLineMax = Lines.Count;
			int nSourceLine = 0;
	
			string strPath = CalcPath(new string[] { Options.Debug_TutorialPath, strProjectName, "source", strFilename });
			try
			{
				using (TextReader tr = new StreamReader(strPath, Encoding.UTF8))
				{
					string strLine;
					while ((strLine = tr.ReadLine()) != null)
					{
						strLine = strLine.TrimEnd();
						nSourceLine++;
						string strPrefix = Lines[nLineNum].Substring(0,1);
						string strMatchLine = Lines[nLineNum].Substring(1);
						if (strPrefix == "@")
						{
							// Set breakpoint at next line to break when "@" line is encoutered.
							nLineNum++;
							continue;
						}
						if ("+-x><".Contains(strPrefix))
						{
							Print("Error - '{0}' is an invalid prefix in a FIND block", strPrefix);
							return false;
						}
						if (fScan && strLine == strMatchLine)
						{
							fScan = false;
							fForceMatch = true;
							nSourceStartLine = nSourceLine;
							nLineNum++;
							if (nLineNum >= nLineMax)
							{
								nSourceNumLines = 1;
								return true;
							}
							continue;
						}
						if (fForceMatch)
						{
							if (strLine != strMatchLine)
							{
								Print("Error - ({0}) '{1}' != ({2}) '{3}'", nSourceLine, strLine, nLineNum, strMatchLine);
								nSourceNumLines = nSourceLine - nSourceStartLine + 1;
								return false;
							}
							else
							{
								nLineNum++;
								if (nLineNum >= nLineMax)
								{
									nSourceNumLines = nSourceLine - nSourceStartLine + 1;
									return true;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Print("Unable to load file {0} - {1}", strPath, ex.Message);
				return false;
			}

			Print("Error - ({0}) '{1}' != ({2}) '{3}'", nSourceLine, "EOF", nLineNum, nLineNum < nLineMax ? Lines[nLineNum].Substring(1) : "???");
			nSourceNumLines = nSourceLine - nSourceStartLine + 1;
			return false;
		}

		bool UpdateLines(string strProjectName, string strFilename, List<string> Lines, out int nSourceStartLine, out int nSourceNumLines)
		{
			nSourceStartLine = 0;
			nSourceNumLines = 0;

			bool fScan = true;
			bool fForceMatch = false;
			bool fSuccess = false;

			int nLineNum = 0;
			int nLineMax = Lines.Count;
			int nSourceLine = 0;
			int nInsertedLines = 0;
			int nDeletedLines = 0;

			// Lines (if any) that we need to insert before the match.
			List<string> PreInsertLines = new List<string>();
			// Gather lines that need to be inserted before the match
			while (Lines[nLineNum].Substring(0,1) == "+")
			{
				PreInsertLines.Add(Lines[nLineNum].Substring(1));
				nLineNum++;
			}

			string strTempfile = "temp.out";
			string strSourcePath = CalcPath(new string[] { Options.Debug_TutorialPath, strProjectName, "source", strFilename });
			string strTempPath = CalcPath(new string[] { Options.Debug_TutorialPath, strProjectName, strTempfile });

			try
			{
				using (TextReader tr = new StreamReader(strSourcePath, Encoding.UTF8))
				{
					using (TextWriter tw = new StreamWriter(strTempPath))
					{
						string strLine;
						while ((strLine = tr.ReadLine()) != null)
						{
							nSourceLine++;

							string strPrefix = "";
							string strMatchLine = "";
							if (nLineNum < nLineMax)
							{
								strPrefix = Lines[nLineNum].Substring(0, 1);
								strMatchLine = Lines[nLineNum].Substring(1);
							}
							if (strPrefix == "@")
							{
								// Set breakpoint at next line to break when "@" line is encoutered.
								nLineNum++;
								continue;
							}

							strLine = strLine.TrimEnd();
							if (fScan && strLine == strMatchLine)
							{
								fScan = false;
								fForceMatch = true;
								nSourceStartLine = nSourceLine;
								nLineNum++;
								if (strPrefix == "-" || strPrefix == "x")
									continue;

								// Insert any lines before the match.
								foreach (string s in PreInsertLines)
								{
									tw.WriteLine(s);
									nInsertedLines++;
								}
								PreInsertLines.Clear();
							}
							else if (fForceMatch)
							{
								// Handle deletes
								if (strPrefix == "-" || strPrefix == "x")
								{
									if (strLine == strMatchLine)
									{
										nLineNum++;
										continue;
									}
									else
									{
										Print("Error in UPDATE starting with '{0}'", Lines[0]);
										Print("   (#{0}) '{1}' != (#{2}) '{3}'", nSourceLine, strLine, nLineNum, strMatchLine);
										return false;
									}
								}

								// Insert any lines.
								while (strPrefix == "+")
								{
									tw.WriteLine(strMatchLine);
									nInsertedLines++;
									nLineNum++;

									if (nLineNum < nLineMax)
									{
										strPrefix = Lines[nLineNum].Substring(0, 1);
										strMatchLine = Lines[nLineNum].Substring(1);
									}
									else
									{
										strPrefix = "";
										strMatchLine = "";
									}
								}

								// Handle line updates ('<' line followed by '>' line).
								if (strPrefix == "<" && strLine == strMatchLine)
								{
									nLineNum++;
									tw.WriteLine(Lines[nLineNum].Substring(1));
									nLineNum++;
									continue;
								}

								if (nLineNum >= nLineMax)
								{
									nSourceNumLines = nSourceLine - nSourceStartLine;
									fForceMatch = false;
									fSuccess = true;
								}
								else
								{
									// Match line.
									if (strLine != strMatchLine)
									{
										Print("Error in UPDATE starting with '{0}'", Lines[0]);
										Print("   (#{0}) '{1}' != (#{2}) '{3}'", nSourceLine, strLine, nLineNum, strMatchLine);
										return false;
									}
									else
										nLineNum++;
								}
							}
							tw.WriteLine(strLine);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Print("Unable to process file {0}", ex.Message);
				return false;
			}

			// Check for match at end of file.
			if (!fSuccess && nLineNum >= nLineMax)
			{
				nSourceNumLines = nSourceLine - nSourceStartLine;
				fSuccess = true;
			}

			nSourceNumLines += nInsertedLines;
			nSourceNumLines -= nDeletedLines;

			if (fSuccess)
			{
				// Copy temp file over original and delete temp.
				File.Copy(strTempPath, strSourcePath, true);
				File.Delete(strTempPath);
			}
			else
			{
				Print("Failed to match UPDATE starting with {0}", Lines[0]);
			}

			return fSuccess;
		}

		bool VerifyBuild(string strProjectName)
		{
			string strDir = CalcPath(Options.Debug_TutorialPath, strProjectName);

			// Cleanup previous binaries.
			string[] gbaFiles = Directory.GetFiles(strDir, "*.gba");
			foreach (string file in gbaFiles)
				File.Delete(file);
			string[] ndsFiles = Directory.GetFiles(strDir, "*.nds");
			foreach (string file in ndsFiles)
				File.Delete(file);

			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = "make";
			psi.Arguments = "-B";	// Force complete rebuild.
			psi.WorkingDirectory = strDir;
			//psi.CreateNoWindow = false;
			//psi.WindowStyle = ProcessWindowStyle.Hidden;
			//psi.UseShellExecute = false;
			//psi.RedirectStandardOutput = true;
			using (Process p = Process.Start(psi))
			{
				p.WaitForExit();
			}

			// Did we successfully create a new binary?
			if (Directory.GetFiles(strDir, "*.gba").Length != 0
				|| Directory.GetFiles(strDir, "*.nds").Length != 0)
				return true;

			return false;
		}

		#region Sample sprites

		private void DrawSampleSprite(Sprite s, int width, int height, int[,] data)
		{
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					s.SetPixel(x, y, data[y, x]);
				}
			}
		}

		private int[,] m_Ball = new int[,] {
			{0,0,0,0,0,0,0,0,},
			{0,0,0,0,0,0,0,0,},
			{0,0,0,1,1,0,0,0,},
			{0,0,1,1,1,1,0,0,},
			{0,0,1,1,1,1,0,0,},
			{0,0,0,1,1,0,0,0,},
			{0,0,0,0,0,0,0,0,},
			{0,0,0,0,0,0,0,0,},
		};

		/// <summary>
		/// Draw a sample sprite for use in the tutorials.
		/// </summary>
		/// <param name="s"></param>
		private void DrawSample1x1Sprite(Sprite s, int id)
		{
			DrawSampleSprite(s, 8, 8, m_Ball);
		}

		/// <summary>
		/// 2x2 Blobber facing right.
		/// </summary>
		private int[,] m_Blobber0 = new int[,] {
			{0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,},
			{0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,},
			{0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,},
			{0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,},
			{0,0,0,0, 3,3,3,3, 3,3,3,3, 0,0,0,0,},
			{0,0,0,3, 3,7,7,7, 7,7,7,3, 3,0,0,0,},
			{0,0,0,3, 7,7,7,7, 7,7,7,7, 3,0,0,0,},
			{0,0,0,3, 7,7,7,3, 7,7,3,7, 3,0,0,0,},
			{0,0,0,3, 7,7,7,7, 7,7,7,7, 3,0,0,0,},
			{0,0,0,3, 7,7,7,7, 7,7,7,7, 3,0,0,0,},
			{0,0,0,3, 7,7,7,3, 3,3,3,7, 3,0,0,0,},
			{0,0,0,3, 7,7,7,7, 7,7,7,7, 3,0,0,0,},
			{0,0,0,3, 7,7,7,7, 7,7,7,7, 3,0,0,0,},
			{0,0,0,3, 7,7,7,7, 7,7,7,7, 3,0,0,0,},
			{0,0,0,3, 7,7,7,3, 3,3,3,7, 3,0,0,0,},
			{0,0,0,3, 3,3,3,3, 0,0,3,3, 3,0,0,0,},
		};

		/// <summary>
		/// 2x2 Blobber facing forward.
		/// </summary>
		private int[,] m_Blobber1 = new int[,] {
			{0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,},
			{0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,},
			{0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,},
			{0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,},
			{0,0,0,0, 3,3,3,3, 3,3,3,3, 0,0,0,0,},
			{0,0,0,3, 3,7,7,7, 7,7,7,3, 3,0,0,0,},
			{0,0,0,3, 7,7,7,7, 7,7,7,7, 3,0,0,0,},
			{0,0,0,3, 7,7,3,7, 7,3,7,7, 3,0,0,0,},
			{0,0,0,3, 7,7,7,7, 7,7,7,7, 3,0,0,0,},
			{0,0,0,3, 7,7,7,7, 7,7,7,7, 3,0,0,0,},
			{0,0,0,3, 7,7,3,3, 3,3,7,7, 3,0,0,0,},
			{0,0,0,3, 7,7,7,7, 7,7,7,7, 3,0,0,0,},
			{0,0,0,3, 7,7,7,7, 7,7,7,7, 3,0,0,0,},
			{0,0,0,3, 7,7,7,7, 7,7,7,7, 3,0,0,0,},
			{0,0,0,3, 7,7,3,3, 3,3,7,7, 3,0,0,0,},
			{0,0,0,3, 3,3,3,7, 7,3,3,3, 3,0,0,0,},
		};

		/// <summary>
		/// 2x2 Blobber facing left.
		/// </summary>
		private int[,] m_Blobber2 = new int[,] {
			{0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,},
			{0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,},
			{0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,},
			{0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,},
			{0,0,0,0, 3,3,3,3, 3,3,3,3, 0,0,0,0,},
			{0,0,0,3, 3,7,7,7, 7,7,7,3, 3,0,0,0,},
			{0,0,0,3, 7,7,7,7, 7,7,7,7, 3,0,0,0,},
			{0,0,0,3, 7,3,7,7, 3,7,7,7, 3,0,0,0,},
			{0,0,0,3, 7,7,7,7, 7,7,7,7, 3,0,0,0,},
			{0,0,0,3, 7,7,7,7, 7,7,7,7, 3,0,0,0,},
			{0,0,0,3, 7,3,3,3, 3,7,7,7, 3,0,0,0,},
			{0,0,0,3, 7,7,7,7, 7,7,7,7, 3,0,0,0,},
			{0,0,0,3, 7,7,7,7, 7,7,7,7, 3,0,0,0,},
			{0,0,0,3, 7,7,7,7, 7,7,7,7, 3,0,0,0,},
			{0,0,0,3, 7,3,3,3, 3,7,7,7, 3,0,0,0,},
			{0,0,0,3, 3,3,7,7, 3,3,3,3, 3,0,0,0,},
		};

		/// <summary>
		/// Draw a sample sprite for use in the tutorials.
		/// </summary>
		/// <param name="s"></param>
		private void DrawSample2x2Sprite(Sprite s, int id)
		{
			if (id == 2)
				DrawSampleSprite(s, 16, 16, m_Blobber2);
			else if (id == 1)
				DrawSampleSprite(s, 16, 16, m_Blobber1);
			else
				DrawSampleSprite(s, 16, 16, m_Blobber0);
		}

		/// <summary>
		/// 4x1 Bat0
		/// </summary>
		private int[,] m_Bat0 = new int[,] {
			{0,0,0,0,0,0,0,1, 1,1,0,0,0,0,0,0, 0,0,0,0,0,0,1,1, 1,0,0,0,0,0,0,0,},
			{0,0,0,0,0,0,1,4, 4,4,1,1,0,0,1,1, 1,1,0,0,1,1,4,4, 4,1,0,0,0,0,0,0,},
			{0,0,0,0,0,1,4,4, 4,4,4,4,1,1,4,4, 4,4,1,1,4,4,4,4, 4,4,1,0,0,0,0,0,},
			{0,0,0,0,1,4,4,1, 1,1,4,4,1,4,1,4, 4,1,4,1,4,4,1,1, 1,4,4,1,0,0,0,0,},
			{0,0,0,0,1,1,1,0, 0,0,1,1,1,4,4,4, 4,4,4,1,1,1,0,0, 0,1,1,1,0,0,0,0,},
			{0,0,0,0,0,0,0,0, 0,0,0,0,0,1,4,4, 4,4,1,0,0,0,0,0, 0,0,0,0,0,0,0,0,},
			{0,0,0,0,0,0,0,0, 0,0,0,0,0,0,1,1, 1,1,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,},
			{0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,},
		};

		/// <summary>
		/// 4x1 Bat1
		/// </summary>
		private int[,] m_Bat1 = new int[,] {
			{0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,},
			{0,0,0,0,0,0,0,0, 0,0,0,0,0,0,1,1, 1,1,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,},
			{0,0,0,0,0,0,0,1, 1,1,1,1,1,1,0,0, 0,0,1,1,1,1,1,1, 1,0,0,0,0,0,0,0,},
			{0,0,0,0,0,1,1,4, 4,4,4,4,1,4,1,4, 4,1,4,1,4,4,4,4, 4,1,1,0,0,0,0,0,},
			{0,0,0,0,1,4,4,4, 4,1,1,1,1,4,4,4, 4,4,4,1,1,1,1,4, 4,4,4,1,0,0,0,0,},
			{0,0,0,0,1,4,1,1, 1,0,0,0,0,1,4,4, 4,4,1,0,0,0,0,1, 1,1,4,1,0,0,0,0,},
			{0,0,0,0,1,1,0,0, 0,0,0,0,0,0,1,1, 1,1,0,0,0,0,0,0, 0,0,1,1,0,0,0,0,},
			{0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,},
		};

		/// <summary>
		/// Draw a sample sprite for use in the tutorials.
		/// </summary>
		/// <param name="s"></param>
		private void DrawSample4x1Sprite(Sprite s, int id)
		{
			if (id == 1)
				DrawSampleSprite(s, 32, 8, m_Bat1);
			else
				DrawSampleSprite(s, 32, 8, m_Bat0);
		}

		#endregion

		private bool HandleSpritelyAction(string strProjectName, string strCommand, bool fNDS)
		{
			Match m;

			if (strCommand == "delete_sprite")
			{
				Spriteset ss = m_doc.Spritesets.Current;
				ss.RemoveSelectedSprite(null);
				return true;
			}
			m = Regex.Match(strCommand, "^add_sprite\\s+(\\d)x(\\d)\\s+(.+)$");
			if (m.Success)
			{
				int width = Int32.Parse(m.Groups[1].Value);
				int height = Int32.Parse(m.Groups[2].Value);
				string name = m.Groups[3].Value;
				Spriteset ss = m_doc.Spritesets.Current;
				ss.AddSprite(width, height, name, -1, "", 0, null);
				return true;
			}
			m = Regex.Match(strCommand, "^rename_sprite\\s+(.+)$");
			if (m.Success)
			{
				Sprite s = m_doc.Spritesets.Current.CurrentSprite;
				s.Name = m.Groups[1].Value;
				return true;
			}
			m = Regex.Match(strCommand, "^select_color\\s+(\\d+)$");
			if (m.Success)
			{
				int color = Int32.Parse(m.Groups[1].Value);
				Spriteset ss = m_doc.Spritesets.Current;
				// TODO: don't assume palette16
				Palette16 p16 = ss.Palette as Palette16;
				p16.GetCurrentSubpalette().CurrentColor = color;
				return true;
			}
			m = Regex.Match(strCommand, "^fill_sprite\\s+(\\d),(\\d)$");
			if (m.Success)
			{
				int x = Int32.Parse(m.Groups[1].Value);
				int y = Int32.Parse(m.Groups[2].Value);
				Sprite s = m_doc.Spritesets.Current.CurrentSprite;
				s.FloodFillClick(x,y);
				return true;
			}
			m = Regex.Match(strCommand, "^draw_sample_1x1_sprite\\s+(\\d)$");
			if (m.Success)
			{
				int id = Int32.Parse(m.Groups[1].Value);
				Sprite s = m_doc.Spritesets.Current.CurrentSprite;
				DrawSample1x1Sprite(s, id);
				return true;
			}
			m = Regex.Match(strCommand, "^draw_sample_2x2_sprite\\s+(\\d)$");
			if (m.Success)
			{
				int id = Int32.Parse(m.Groups[1].Value);
				Sprite s = m_doc.Spritesets.Current.CurrentSprite;
				DrawSample2x2Sprite(s, id);
				return true;
			}
			m = Regex.Match(strCommand, "^draw_sample_4x1_sprite\\s+(\\d)$");
			if (m.Success)
			{
				int id = Int32.Parse(m.Groups[1].Value);
				Sprite s = m_doc.Spritesets.Current.CurrentSprite;
				DrawSample4x1Sprite(s, id);
				return true;
			}
			m = Regex.Match(strCommand, "^import_bgimage\\s+(.+)$");
			if (m.Success)
			{
				string strImageFile = CalcPath(Options.Debug_TutorialRawDataPath, m.Groups[1].Value);
				System.Drawing.Bitmap b = new System.Drawing.Bitmap(strImageFile);
				m_doc.BackgroundImages.AddBgImage("Welcome", -1, "", b);
				return true;
			}
			m = Regex.Match(strCommand, "^add_bgsprite\\s+(\\d)x(\\d)\\s+(.+)$");
			if (m.Success)
			{
				int width = Int32.Parse(m.Groups[1].Value);
				int height = Int32.Parse(m.Groups[2].Value);
				string name = m.Groups[3].Value;
				Spriteset ss = m_doc.BackgroundSpritesets.Current;
				ss.AddSprite(width, height, name, -1, "", 0, null);
				return true;
			}
			m = Regex.Match(strCommand, "^select_bgcolor\\s+(\\d+)$");
			if (m.Success)
			{
				int color = Int32.Parse(m.Groups[1].Value);
				Spriteset ss = m_doc.BackgroundSpritesets.Current;
				ss.Palette.GetCurrentSubpalette().CurrentColor = color;
				return true;
			}
			m = Regex.Match(strCommand, "^fill_bgsprite\\s+(\\d),(\\d)$");
			if (m.Success)
			{
				int x = Int32.Parse(m.Groups[1].Value);
				int y = Int32.Parse(m.Groups[2].Value);
				Sprite s = m_doc.BackgroundSpritesets.Current.CurrentSprite;
				s.FloodFillClick(x, y);
				return true;
			}
			m = Regex.Match(strCommand, "^fill_background\\s+(\\d+),(\\d+)\\s+(\\d+),(\\d+)$");
			if (m.Success)
			{
				int x0 = Int32.Parse(m.Groups[1].Value);
				int y0 = Int32.Parse(m.Groups[2].Value);
				int x1 = Int32.Parse(m.Groups[3].Value);
				int y1 = Int32.Parse(m.Groups[4].Value);
				Sprite s = m_doc.BackgroundSpritesets.Current.CurrentSprite;
				Map map = m_doc.BackgroundMaps.CurrentMap;
				for (int x = x0; x <= x1; x++)
					for (int y=y0; y<= y1; y++)
						map.SetTile(x, y, s.FirstTileId, 0);
				return true;
			}
			m = Regex.Match(strCommand, "^export (.+)$");
			if (m.Success)
			{
				string strDir = CalcPath(Options.Debug_TutorialPath, strProjectName);
				if (!Directory.Exists(strDir))
					Directory.CreateDirectory(strDir);
				string strParam = m.Groups[1].Value;
				if (strParam == "full")
					return m_doc.Filer.Export(strDir, true, fNDS, false);
				if (strParam == "sprites")
					return m_doc.Filer.Export(strDir, false, fNDS, false);
			}

			Print("Unknown command: {0}", strCommand);
			return false;
		}

	}
}
