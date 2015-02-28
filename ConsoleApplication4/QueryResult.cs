namespace edu.uta.cse.proggen.configurationParser
{

	using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;

	/// <summary>
	/// Represents the <Result> sub-element of <Query> in the query xml.
	/// 
    /// @author Team 6 - CSE6324 - Spring 2015
	/// 
	/// </summary>
	public class QueryResult
	{
		private string name;
		private int seqNumber;
		private Primitives type;

		public QueryResult(string name, int seqNumber, Primitives type) : base()
		{
			this.name = name;
			this.seqNumber = seqNumber;
			this.type = type;
		}

		public virtual string Name
		{
			get
			{
				return name;
			}
		}

		public virtual int SeqNumber
		{
			get
			{
				return seqNumber;
			}
		}

		public virtual Primitives Type
		{
			get
			{
				return type;
			}
		}
	}

}