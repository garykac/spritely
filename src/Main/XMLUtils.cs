using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Spritely
{
	public class XMLUtils
	{
		public static bool HasXMLAttribute(XmlNode n, string strAttr)
		{
			XmlAttribute attr = n.Attributes.GetNamedItem(strAttr) as XmlAttribute;
			return attr != null;
		}

		public static string GetXMLAttribute(XmlNode n, string strAttr)
		{
			XmlAttribute attr = n.Attributes.GetNamedItem(strAttr) as XmlAttribute;
			if (attr != null)
				return attr.Value;
			return "";
		}

		public static int GetXMLIntegerAttribute(XmlNode n, string strAttr)
		{
			XmlAttribute attr = n.Attributes.GetNamedItem(strAttr) as XmlAttribute;
			if (attr != null)
				return ParseInteger(attr.Value);
			return 0;
		}

		public static int ParseInteger(string str)
		{
			int nVal = 0;
			try
			{
				nVal = Int32.Parse(str);
			}
			catch
			{
			}
			return nVal;
		}

		public static int ParseHexInteger(string str)
		{
			int nVal = 0;
			try
			{
				nVal = Int32.Parse(str, System.Globalization.NumberStyles.HexNumber);
			}
			catch
			{
			}
			return nVal;
		}

	}

	#region Tests

	[TestFixture]
	public class XMLUtils_Test
	{
		XmlDocument m_xd;
		XmlElement m_node;

		[TestFixtureSetUp]
		public void FixtureInit()
		{
			m_xd = new XmlDocument();
			m_node = m_xd.CreateElement("node");
			m_node.SetAttribute("attr", "23");
		}

		[Test]
		public void Test_HasXMLAttribute()
		{
			Assert.IsTrue(XMLUtils.HasXMLAttribute(m_node, "attr"));
			Assert.IsFalse(XMLUtils.HasXMLAttribute(m_node, "no_attr"));
		}

		[Test]
		public void Test_GetXMLAttribute()
		{
			Assert.AreEqual("23", XMLUtils.GetXMLAttribute(m_node, "attr"));
			Assert.AreEqual("", XMLUtils.GetXMLAttribute(m_node, "no_attr"));
		}

		[Test]
		public void Test_GetXMLIntegerAttribute()
		{
			Assert.AreEqual(23, XMLUtils.GetXMLIntegerAttribute(m_node, "attr"));
			Assert.AreEqual(0, XMLUtils.GetXMLIntegerAttribute(m_node, "no_attr"));
		}

		[Test]
		public void Test_ParseInteger()
		{
			Assert.AreEqual(2, XMLUtils.ParseInteger("2"));
			Assert.AreEqual(0, XMLUtils.ParseInteger(""));
			Assert.AreEqual(0, XMLUtils.ParseInteger("fred"));
		}

		[Test]
		public void Test_ParseHexInteger()
		{
			Assert.AreEqual(2, XMLUtils.ParseHexInteger("2"));
			Assert.AreEqual(16, XMLUtils.ParseHexInteger("10"));
			Assert.AreEqual(10, XMLUtils.ParseHexInteger("a"));
			Assert.AreEqual(10, XMLUtils.ParseHexInteger("A"));
			Assert.AreEqual(255, XMLUtils.ParseHexInteger("fF"));
			Assert.AreEqual(0, XMLUtils.ParseHexInteger(""));
			Assert.AreEqual(0, XMLUtils.ParseHexInteger("fred"));
		}

	}

	#endregion
}
