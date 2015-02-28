using System;

namespace edu.uta.cse.proggen.operators
{

	using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;
	using Operator = edu.uta.cse.proggen.nodes.Operator;

	/// <summary>
	/// Binary Operator : +, -, *, /, %
    /// @author Team 6 - CSE6324 - Spring 2015
	/// 
	/// </summary>

	public class BinaryOperator : Operator
	{

		private string symbol = null;

		public BinaryOperator(Primitives primitive)
		{

			if (primitive == Primitives.STRING)
			{
				symbol = "+";
			}
			else
			{
				Random rand = new Random();
				int option = rand.Next(8);
				switch (option)
				{
					case 0:
						symbol = "%";
						break;
					case 1:
						symbol = "-";
						break;
					case 2:
						symbol = "*";
						break;
					case 3:
						symbol = "+";
						break;
					case 4:
						symbol = "-";
						break;
					case 5:
						symbol = "*";
						break;
					case 6:
						symbol = "+";
						break;
					case 7:
						symbol = "/";
						break;
				}
			}
		}

		public override string ToString()
		{
			return symbol;
		}

	}

}