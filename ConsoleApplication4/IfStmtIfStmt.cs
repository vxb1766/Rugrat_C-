using System;
using System.Collections.Generic;

namespace edu.uta.cse.proggen.statements
{


	using Method = edu.uta.cse.proggen.classLevelElements.Method;
	using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;
	using BooleanExpression = edu.uta.cse.proggen.expressions.BooleanExpression;
	using Expression = edu.uta.cse.proggen.nodes.Expression;
	using ClassGenerator = edu.uta.cse.proggen.namespaceLevelElements.ClassGenerator;
	using ProgGenUtil = edu.uta.cse.proggen.util.ProgGenUtil;

	/// <summary>
	/// Creates :
	/// if(cond1){
	///     stmt
	///     stmt
	///     ...
	///     if(cond2){
	///       stmt
	///       stmt
	///       ....
	///     }
	///  }
    /// @author Team 6 - CSE6324 - Spring 2015
	/// 
	/// </summary>
	public class IfStmtIfStmt : Statement
	{

		private Expression cond = null;
		private string output = "";
		private string body = "";
		private Method method;


		public IfStmtIfStmt(Method method, List<ClassGenerator> classList)
		{
			this.method = method;

			HashSet<Primitives> primitiveSet = ProgGenUtil.getPrimitivesOfVariables(method);
			cond = new BooleanExpression(method, ProgGenUtil.getRandomizedPrimitiveForBooleanExpression(primitiveSet));

			//adding two extra lines for each nested loop
			method.Loc = method.Loc + 2;

			Random rand = new Random();
			int option = rand.Next(100);
			//We want more nested if's but not more than the MAX.
			if ((option < method.AssociatedClass.Percent + 40) && (method.NestedIfCounter < method.MaxNestedIfs))
			{
				// counts the nested ifs
				method.NestedIfCounter = method.NestedIfCounter + 1;
				//Start.nestedIf_counter++; 
				body += (new IfStmtIfStmt(method, classList)).ToString();
			}
			else
			{
				body = Statement.getRandomizedStatement(method, classList).ToString();
			}
		}

		public override string ToString()
		{
			if (cond.ToString() == null)
			{
				//unable to construct a variable based expression
				return (new PrintStatement(method)).ToString();
			}

			output = "if( " + cond.ToString() + "){\n";
			output += body + "}\n";
			return output;
		}
	}

}