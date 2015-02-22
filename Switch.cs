using System;
using System.Collections.Generic;

namespace edu.uta.cse.proggen.statements
{


	using Method = edu.uta.cse.proggen.classLevelElements.Method;
	using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;
	using VariableBinOpField = edu.uta.cse.proggen.expressions.VariableBinOpField;
	using VariableBinOpVariable = edu.uta.cse.proggen.expressions.VariableBinOpVariable;
	using Expression = edu.uta.cse.proggen.nodes.Expression;
	using ClassGenerator = edu.uta.cse.proggen.packageLevelElements.ClassGenerator;


	/// <summary>
	///  This creates the switch statement.
	/// @author Ishtiaque_Hussain
	/// 
	/// </summary>
	public class Switch : Statement
	{

		internal Expression operand = null;
		internal List<string> cases = new List<string>();

		public Switch(Method method, List<ClassGenerator> classList)
		{
			Random rand = new Random();
			//introducing fields in the switch statement
			if (rand.Next(2) == 0)
			{
				operand = new VariableBinOpField(method, Primitives.INT);
			}
			else
			{
				operand = new VariableBinOpVariable(method, Primitives.INT);
			}

			method.Loc = method.Loc + 2;

			int option = rand.Next(100) % 10; //switches will have maximum of 10 cases.

			//We don't want switch to have 0, 1 or only 2 cases
			if (option <= 2)
			{
				option += 2;
			}

			for (int i = 0; i < option ; i++)
			{
				cases.Add("case " + i + ":\n" + Statement.getRandomizedStatement(method, classList).ToString() + " break;\n");
				method.Loc = method.Loc + 2;
				//Start.loc_per_meth_counter += 2; // each case adds 2 lines 'case:' and 'break'
			}
			cases.Add("default :\n" + Statement.getRandomizedStatement(method, classList).ToString());
		}


		public override string ToString()
		{
			string output = "switch(" + operand.ToString() + "){\n";
			for (int i = 0; i < cases.Count; i++)
			{
				output += cases[i];
			}
			output += "}\n";

			return output;
		}
	}

}