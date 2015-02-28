using System;
using System.Collections.Generic;
using System.Xml;

namespace edu.uta.cse.proggen.configurationParser
{


    using XMLException = System.Xml.XmlException;
    using IOException = System.IO.IOException;
    //using Document = org.w3c.dom.Document;
    //using Node = org.w3c.dom.Node;
    //using NodeList = org.w3c.dom.NodeList;
    //using SAXException = org.xml.sax.SAXException;

	using Start = edu.uta.cse.proggen.start.Start;

	/// <summary>
	/// Parser class to get information from config.xml.
	///  * 
    /// @author Team 6 - CSE6324 - Spring 2015
	/// 
	/// </summary>

	/*
	 * Parses config.xml and puts all the key-value pairs in 'properties
	 * HashMap. For AllowedTypes, puts the key-value pairs in 'typeList' LinkedHashSet.
	 * It has also public methods to access these maps. 
	 */
	public class ConfigurationXMLParser
	{

		private static XmlDocument document = null;
		private static Dictionary<string, string> properties = new Dictionary<string, string>();
        private static HashSet<string> typeList = new HashSet<string>();

		//read information from the XML as soon as the class is loaded into the JVM
		static ConfigurationXMLParser()
		{
			//DocumentBuilderFactory documentBuilderFactory = DocumentBuilderFactory.newInstance();
			try
			{
				//DocumentBuilder documentBuilder = documentBuilderFactory.newDocumentBuilder();
                string filename = (Start.Start.PathToDir + "config.xml");
                document = new XmlDocument();
				document.Load(filename);
				try
				{
					parseProperties();
				}
				catch (Exception e)
				{
					Console.WriteLine("Error parsing properties from XML!");
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
					Environment.Exit(1);
				}
			}
			catch (XMLException e)
			{
				Console.WriteLine("error parsing XML configuration!");
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Environment.Exit(1);
			}
            //catch (SAXException e)
            //{
            //    Console.WriteLine("error parsing XML configuration!");
            //    Console.WriteLine(e.ToString());
            //    Console.Write(e.StackTrace);
            //    Environment.Exit(1);
            //}
            catch (IOException e)
            {
                Console.WriteLine("error processing XML configuration file!");
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
                Environment.Exit(1);
            }
		}

		private static XmlNode RootNode
		{
			get
			{
				return document.DocumentElement;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void parseProperties() throws Exception
		public static void parseProperties()
		{
            XmlNode root = RootNode;
            XmlNodeList propertyNodes = root.ChildNodes;

			int numberOfPropertyNodes = propertyNodes.Count;

			for (int i = 0; i < numberOfPropertyNodes; i++)
			{
                XmlNode node = propertyNodes.Item(i);
                string name = node.Name;
				if (name.Equals("allowedTypes"))
				{
					parseAllowedTypes(node);
					continue;
				}
                string value = node.InnerText;
				properties[name] = value;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void parseAllowedTypes(org.w3c.dom.Node node) throws Exception
        private static void parseAllowedTypes(XmlNode node)
		{
			if (!node.Name.Equals("allowedTypes"))
			{
				throw new Exception("Invalid node allowedTypes");
			}

			XmlNodeList typeNodes = node.ChildNodes;
			int noOfTypes = typeNodes.Count;

			for (int i = 0; i < noOfTypes; i++)
			{
				string str = typeNodes.Item(i).InnerText.Trim();
				if (str.Equals(""))
				{
					continue;
				}
				typeList.Add(str);
			}
		}

		public static HashSet<string> TypeList
		{
			get
			{
				return typeList;
			}
		}

		public static string getProperty(string property)
		{
			return properties[property];
		}

		/// <summary>
		/// returns an int value or -1 in case of error
		/// </summary>
		/// <param name="property"> - to be fetched </param>
		/// <returns> int value </returns>
		public static int getPropertyAsInt(string property)
		{
			try
			{
				return int.Parse(getProperty(property));
			}
			catch (System.FormatException)
			{
				Console.WriteLine("Error converting value to int for property : " + property);
				Environment.Exit(1);
			}
			return -1;
		}

	}

}