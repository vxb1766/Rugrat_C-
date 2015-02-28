using System;

namespace edu.uta.cse.proggen.expressions
{

	using Method = edu.uta.cse.proggen.classLevelElements.Method;
	using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;
	using Expression = edu.uta.cse.proggen.nodes.Expression;
	using Operator = edu.uta.cse.proggen.nodes.Operator;
	using BooleanOperator = edu.uta.cse.proggen.operators.BooleanOperator;
	using ConjunctOperator = edu.uta.cse.proggen.operators.ConjunctOperator;
	using ProgGenUtil = edu.uta.cse.proggen.util.ProgGenUtil;

	/// 
	/// <summary>
	/// boolExpr := expr boolOp expr | boolExpr conjOp boolExpr
    /// @author Team 6 - CSE6324 - Spring 2015
	/// 
	/// </summary>

	public class BooleanExpression : Expression
	{

		private Expression leftExpr = null;
		private Operator op = null;
		private Expression rightExpr = null;

		public BooleanExpression(Method method, Primitives primitive)
		{
			if (primitive == null)
			{
				return;
			}

			if (!ProgGenUtil.getPrimitivesOfVariables(method).Contains(primitive))
			{
				return;
			}

			Random rand = new Random();
			int option = rand.Next(100);

			if (option < method.AssociatedClass.Percent)
			{
				leftExpr = new BooleanExpression(method, primitive);
				op = new ConjunctOperator();
				rightExpr = new BooleanExpression(method, primitive);
			}
			else
			{
				leftExpr = new NormalExpression(method, primitive);
				op = new BooleanOperator(primitive);
				rightExpr = new NormalExpression(method, primitive);

				//we also don't want if(i8 != i8)
				while (leftExpr.ToString().Equals(rightExpr.ToString()))
				{
					rightExpr = new NormalExpression(method, primitive);
				}
			}
		}

		public override string ToString()
		{
			if (leftExpr == null || rightExpr == null || op == null)
			{
				return null;
			}
			return "(" + leftExpr.ToString() + op.ToString() + rightExpr.ToString() + ")";
		}
	}

}