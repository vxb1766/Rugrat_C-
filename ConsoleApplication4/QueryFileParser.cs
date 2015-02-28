using System;
using System.Collections.Generic;
using System.Xml;

namespace edu.uta.cse.proggen.configurationParser
{



    //using Document = Document;
    //using Element = org.w3c.dom.Element;
    //using Node = org.w3c.dom.Node;
    //using NodeList = org.w3c.dom.NodeList;

	using Type = edu.uta.cse.proggen.classLevelElements.Type;
	using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;

	/// <summary>
	/// Parser class for the query file XML.
	/// 
    /// @author Team 6 - CSE6324 - Spring 2015
	/// 
	/// </summary>
	public class QueryFileParser
	{
        private static XmlDocument document = new XmlDocument();
		public static List<Query> queries = new List<Query>();

		/*
		 * parse and construct query objects in memory as soon as this class is loaded
		 * in the JVM.
		 */
		static QueryFileParser()
		{
			//DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
			try
			{
				//DocumentBuilder builder = factory.newDocumentBuilder();
                string filename = ConfigurationXMLParser.getProperty("queryFilename");
                document.Load(filename);
				//document = builder.parse(new File(ConfigurationXMLParser.getProperty("queryFilename")));
				parseQueryNodes();
			}
			catch (Exception e)
			{
				Console.WriteLine("Error parsing query XML. Continuing...");
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}

		private static XmlNode RootNode
		{
			get
			{
				return document.DocumentElement;
			}
		}

		public static void parseQueryNodes()
		{
			XmlNode root = RootNode;
			XmlNodeList queryNodes = root.ChildNodes;

            int noOfQueryNodes = queryNodes.Count;

			for (int i = 0; i < noOfQueryNodes; i++)
			{
                XmlNode node = queryNodes.Item(i);
				if (node is XmlElement)
				{
                    XmlElement queryNode = (XmlElement)node;
					string queryString = queryNode.GetAttribute("value");

					XmlNodeList resultNodes = queryNode.ChildNodes;
					int noOfResultNodes = resultNodes.Count;
					string name = "";
					int seqNumber = -1;
					Type.Primitives type;

					List<QueryResult> results = new List<QueryResult>();
					HashSet<Type.Primitives> typeSet = new HashSet<Type.Primitives>();
					for (int j = 0; j < noOfResultNodes; j++)
					{
						XmlNode resultNode = resultNodes.Item(j);
						if (resultNode is XmlElement)
						{
                            XmlElement resultElement = (XmlElement)resultNode;
							name = resultElement.GetAttribute("Name");
							seqNumber = Convert.ToInt32(resultElement.GetAttribute("SeqNumber"));
							type = Type.reverseLookup(resultElement.GetAttribute("Type"));

							typeSet.Add(type);
							QueryResult queryResult = new QueryResult(name, seqNumber, type);
							results.Add(queryResult);
						}
					}
					Query query = new Query(queryString, results, typeSet);
					queries.Add(query);
				}
			}
		}
	}

}