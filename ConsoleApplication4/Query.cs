using System.Collections.Generic;

namespace edu.uta.cse.proggen.configurationParser
{


	using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;

	/// <summary>
	/// This class represents a <query> element in the QueryFile fed into
	/// RUGRAT.
	/// 
    /// @author Team 6 - CSE6324 - Spring 2015
	/// 
	/// </summary>
	public class Query
	{
		private string queryString;
		private List<QueryResult> results;
		private HashSet<Primitives> resultTypes;

		public Query(string queryString, List<QueryResult> results, HashSet<Primitives> resultTypes) : base()
		{
			this.queryString = queryString;
			this.results = results;
			this.resultTypes = resultTypes;
		}

		public virtual string QueryString
		{
			get
			{
				return queryString;
			}
		}

		public virtual List<QueryResult> Results
		{
			get
			{
				return results;
			}
		}

		public virtual HashSet<Primitives> ResultTypes
		{
			get
			{
				return resultTypes;
			}
		}
	}

}