namespace edu.uta.cse.proggen.nodes
{

	/// <summary>
	/// Represents an operand node in a stochastic parse tree.
	/// 
	/// @author balamurugan
	/// 
	/// </summary>
	public class Operand : Expression
	{
		protected internal string literal = "";

		public override string ToString()
		{
			return literal;
		}
	}

}