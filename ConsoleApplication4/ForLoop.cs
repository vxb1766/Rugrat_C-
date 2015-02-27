using System;
using System.Collections.Generic;

namespace edu.uta.cse.proggen.statements
{


	using Method = edu.uta.cse.proggen.classLevelElements.Method;
	using Operand = edu.uta.cse.proggen.nodes.Operand;
	using ClassGenerator = edu.uta.cse.proggen.packageLevelElements.ClassGenerator;
	using ProgGenUtil = edu.uta.cse.proggen.util.ProgGenUtil;

	/// <summary>
	/// This creates: for(int i=0; i<var; i++){ stmt..}
	/// @author Ishtiaque_Hussain
	/// 
	/// </summary>
	public class ForLoop : Statement
	{

		internal Operand @var = null;
		internal new Statement stmt = null;

		private string positiveInteger()
		{
			int? integer = new int?((new Random()).Next(ProgGenUtil.maxLoopControllerValue));
			int val = (int)integer;

			if (val < 0)
			{
				val = val * -1;
			}

			return val + "";
		}

		public ForLoop(Method method, List<ClassGenerator> classList)
		{
			// adding 2 lines per for loop
			method.Loc = method.Loc + 2;

			Random rand = new Random();
			int option = rand.Next(2);
			if (option == 0)
			{
				stmt = new IfStmtIfStmt(method, classList);
			}
			else
			{
				stmt = Statement.getRandomizedStatement(method, classList);

				// We don't want method calls in the for loop, allow print statements that has 'method' keyword in it
                while (stmt != null && stmt.ToString().Contains("method") && !stmt.ToString().Contains("Console.WriteLine"))
				{
					stmt = Statement.getRandomizedStatement(method, classList);
				}
			}
		}

		public override string ToString()
		{
			string output = "";
			output += "for(int i = 0; i < " + positiveInteger() + "; i++){\n ";
			output += stmt.ToString() + "}\n";
			return output;
		}
	}

}