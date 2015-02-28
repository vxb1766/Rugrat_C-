using System;

namespace edu.uta.cse.proggen.statements
{

	using Method = edu.uta.cse.proggen.classLevelElements.Method;

	/// <summary>
	/// Creates a print statement to be appended to the method body.
	/// 
    /// @author Team 6 - CSE6324 - Spring 2015
	/// 
	/// </summary>
	public class PrintStatement
	{
		private Method method;

		public PrintStatement(Method method)
		{
			this.method = method;
		}

		public override string ToString()
		{
			string stmt = "";
			Random rand = new Random();
			int option = rand.Next(5);

			for (int i = 0; i <= option; i++)
			{
                stmt = "Console.WriteLine(\"" + method.AssociatedClass.FileName + " - " + method.Name + "- LineInMethod: " + method.Loc + "\");" + "\n";
			}

			method.Loc = method.Loc + option + 1;
			return stmt;
		}
	}

}