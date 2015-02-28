using System;

namespace edu.uta.cse.proggen.operators
{

	using Operator = edu.uta.cse.proggen.nodes.Operator;

	/// <summary>
	/// Conjunct operators: &&, ||, &, |
    /// @author Team 6 - CSE6324 - Spring 2015
	/// 
	/// </summary>
	public class ConjunctOperator : Operator
	{
		private string symbol = null;

		public ConjunctOperator()
		{
			Random rand = new Random();
			int option = rand.Next(4);
			switch (option)
			{
				case 0:
					symbol = "&&";
					break;
				case 1:
					symbol = "||";
					break;
				case 2:
					symbol = "&&";
					break;
				case 3:
					symbol = "&&";
					break;
			}
		}

		public override string ToString()
		{
			return symbol;
		}
	}

}