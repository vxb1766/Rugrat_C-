using System;

namespace edu.uta.cse.proggen.statements
{

	using Method = edu.uta.cse.proggen.classLevelElements.Method;

	/// <summary>
	/// Creates a print statement to be appended to the method body.
	/// 
	/// @author balamurugan
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
				stmt = "System.out.println(\"" + method.AssociatedClass.FileName + " - " + method.Name + "- LineInMethod: " + method.Loc + "\");" + "\n";
			}

			method.Loc = method.Loc + option + 1;
			return stmt;
		}
	}

}