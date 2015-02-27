using System;

namespace edu.uta.cse.proggen.expressions
{

	using Method = edu.uta.cse.proggen.classLevelElements.Method;
	using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;
	using Expression = edu.uta.cse.proggen.nodes.Expression;
	using BinaryOperator = edu.uta.cse.proggen.operators.BinaryOperator;

	public class VariableBinOpField : Expression
	{
		private Expression leftOprnd = null;
		private BinaryOperator binOp = null;
		private Expression rightOprnd = null;

		public VariableBinOpField(Method method, Primitives primitive)
		{

			Random rand = new Random();


			int option = rand.Next(100) % 2;

			int nested = 0;

			switch (option)
			{
			case 0: //var binOp field
				nested = rand.Next(10);
                //if (nested < method.AssociatedClass.Percent)
                //{
                //    leftOprnd = new VariableBinOpField(method, primitive);
                //    binOp = new BinaryOperator(primitive);
                //    rightOprnd = new VariableBinOpField(method, primitive);
                //}
                //else
				{
					leftOprnd = VariableGenerator.getRandomizedVariable(method, primitive);
					binOp = new BinaryOperator(primitive);
					rightOprnd = FieldGenerator.getRandomField(method.AssociatedClass, primitive, method.Static);
				}
				break;

			case 1: // field binOp var
				nested = rand.Next(100);
                //if (nested < method.AssociatedClass.Percent)
                //{
                //    leftOprnd = new VariableBinOpField(method, primitive);
                //    binOp = new BinaryOperator(primitive);
                //    rightOprnd = new VariableBinOpField(method, primitive);
                //}
                //else
				{
					leftOprnd = VariableGenerator.getRandomizedVariable(method, primitive);
					binOp = new BinaryOperator(primitive);
					rightOprnd = FieldGenerator.getRandomField(method.AssociatedClass, primitive, method.Static);
				}
				break;
			}

			// for division and modulo operations, keeping only literals in the right expr.
			// i5%i3 => i5%constantValue OR f2/f4 => f2/constantValue

			if (binOp.ToString().Equals("/") || binOp.ToString().Equals("%"))
			{
					do
					{ //FIXME: only handles int for now.
						rightOprnd = new Literal(primitive);
					}while (rightOprnd.ToString().Contains("(0)")); //avoiding divide by (0)
			}

		}

		public override string ToString()
		{
			return "(" + leftOprnd.ToString() + binOp.ToString() + rightOprnd.ToString() + ")";
		}

	}

}