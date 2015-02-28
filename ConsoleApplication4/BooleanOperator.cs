using System;

namespace edu.uta.cse.proggen.operators
{

	using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;
	using Operator = edu.uta.cse.proggen.nodes.Operator;
	/// <summary>
	/// Boolean Operators : ==, !=, >, >=, <, <=
    /// Team 6 - CSE6324 - Spring 2015
	/// 
	/// </summary>

	public class BooleanOperator : Operator
	{

		private string symbol = null;

		public BooleanOperator(Primitives primitive)
		{
			Random rand = new Random();
			int option;
			if (primitive.Equals(Primitives.STRING) || primitive.Equals(Primitives.OBJECT))
			{
				option = rand.Next(2);
			}
			else
			{
				option = rand.Next(6);
			}

			switch (option)
			{
				case 0:
					symbol = "!=";
					break;
				case 1:
					symbol = "==";
					break;
				case 2:
					symbol = ">=";
					break;
				case 3:
					symbol = ">";
					break;
				case 4:
					symbol = "<";
					break;
				case 5:
					symbol = "<=";
					break;
			}
		}

		public override string ToString()
		{
			return symbol;
		}
	}

}