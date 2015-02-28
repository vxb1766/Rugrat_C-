using System.Collections.Generic;

namespace edu.uta.cse.proggen.statements
{


	using Method = edu.uta.cse.proggen.classLevelElements.Method;
	using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;
	using BooleanExpression = edu.uta.cse.proggen.expressions.BooleanExpression;
	using ClassGenerator = edu.uta.cse.proggen.namespaceLevelElements.ClassGenerator;
	using ProgGenUtil = edu.uta.cse.proggen.util.ProgGenUtil;

	/// <summary>
	/// It's if(e){
	/// 		...stmt
	/// 		}else {stmt..}
    /// @author Team 6 - CSE6324 - Spring 2015
	/// 
	/// </summary>
	public class IfElse : Statement
	{

		private BooleanExpression boolExpr = null;
		private Statement thenStmt = null;
		private Statement elseStmt = null;
		private Method method;

		public IfElse(Method method, List<ClassGenerator> classList)
		{
			this.method = method;
			method.Loc = method.Loc + 4;
			// 'if' and 'else' themselves contribute 4 lines in the loc
			HashSet<Primitives> primitiveSet = ProgGenUtil.getPrimitivesOfVariables(method);
			boolExpr = new BooleanExpression(method, ProgGenUtil.getRandomizedPrimitiveForBooleanExpression(primitiveSet));
			thenStmt = Statement.getRandomizedStatement(method, classList);
			elseStmt = Statement.getRandomizedStatement(method, classList);
		}

		public override string ToString()
		{
			if (boolExpr.ToString() == null)
			{
				//failed to construct a variable based expression
				return (new PrintStatement(method)).ToString();
			}
			string output = "if" + "(" + boolExpr.ToString() + "){" + "\n";
			output = output + thenStmt.ToString() + "}\n" + "else{\n " + elseStmt.ToString() + "}\n";
			return output;
		}
	}

}