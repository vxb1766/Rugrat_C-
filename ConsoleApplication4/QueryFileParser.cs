using System;
using System.Collections.Generic;

namespace edu.uta.cse.proggen.configurationParser
{



	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;

	using Type = edu.uta.cse.proggen.classLevelElements.Type;
	using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;

	/// <summary>
	/// Parser class for the query file XML.
	/// 
	/// @author balamurugan
	/// 
	/// </summary>
	public class QueryFileParser
	{
		private static Document document = null;
		public static List<Query> queries = new List<Query>();

		/*
		 * parse and construct query objects in memory as soon as this class is loaded
		 * in the JVM.
		 */
		static QueryFileParser()
		{
			DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
			try
			{
				DocumentBuilder builder = factory.newDocumentBuilder();
				document = builder.parse(new File(ConfigurationXMLParser.getProperty("queryFilename")));
				parseQueryNodes();
			}
			catch (Exception e)
			{
				Console.WriteLine("Error parsing query XML. Continuing...");
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}

		private static Node RootNode
		{
			get
			{
				return document.DocumentElement;
			}
		}

		public static void parseQueryNodes()
		{
			Node root = RootNode;
			NodeList queryNodes = root.ChildNodes;

			int noOfQueryNodes = queryNodes.Length;

			for (int i = 0; i < noOfQueryNodes; i++)
			{
				Node node = queryNodes.item(i);
				if (node is Element)
				{
					Element queryNode = (Element)node;
					string queryString = queryNode.getAttribute("value");

					NodeList resultNodes = queryNode.ChildNodes;
					int noOfResultNodes = resultNodes.Length;
					string name = "";
					int seqNumber = -1;
					Type.Primitives type;

					List<QueryResult> results = new List<QueryResult>();
					HashSet<Type.Primitives> typeSet = new HashSet<Type.Primitives>();
					for (int j = 0; j < noOfResultNodes; j++)
					{
						Node resultNode = resultNodes.item(j);
						if (resultNode is Element)
						{
							Element resultElement = (Element) resultNode;
							name = resultElement.getAttribute("Name");
							seqNumber = Convert.ToInt32(resultElement.getAttribute("SeqNumber"));
							type = Type.reverseLookup(resultElement.getAttribute("Type"));

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