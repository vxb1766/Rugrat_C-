using System;

//This entire thing is never called
namespace edu.uta.cse.proggen.expressions
{

	using Method = edu.uta.cse.proggen.classLevelElements.Method;
	using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;
	using Operand = edu.uta.cse.proggen.nodes.Operand;
	using BinaryOperator = edu.uta.cse.proggen.operators.BinaryOperator;

    /// <summary>
    /// 
    /// @author Team 6 - CSE6324 - Spring 2015
    /// 
    /// </summary>

	public class FieldBinOpField : Operand
	{
		private Operand leftOprnd = null;
		private BinaryOperator binOp = null;
		private Operand rightOprnd = null;

		public FieldBinOpField(Method method, Primitives primitive)
		{

			Random rand = new Random();
			int nested = rand.Next(100);

			//nested expression
			if (nested < method.AssociatedClass.Percent)
			{
				leftOprnd = new FieldBinOpField(method, primitive);
				binOp = new BinaryOperator(primitive);
				rightOprnd = new FieldBinOpField(method, primitive);
			}
			else
			{
				leftOprnd = FieldGenerator.getRandomField(method.AssociatedClass, primitive, method.Static);
				binOp = new BinaryOperator(primitive);
				rightOprnd = FieldGenerator.getRandomField(method.AssociatedClass, primitive, method.Static);
			}

			// removing expressions of the form: (i5-i5) to avoid {i19%(i5-i5)}expr.
			if (leftOprnd.ToString().Equals(rightOprnd.ToString()))
			{
                rightOprnd = new Literal(primitive, Int32.MaxValue);
			}

			// for division and modulo operations, keeping only literals in the right expr.
			// i5%i3 => i5%constantValue OR f2/f4 => f2/constantValue

            //if (binOp.ToString().Equals("/") || binOp.ToString().Equals("%"))
            //{
            //    do
            //    { //FIXME: only handles int for now.
            //        rightOprnd = new Literal(primitive, Int32.MaxValue);
            //    }while (rightOprnd.ToString().Contains("(0)")); //avoiding divide by (0)
            //}

		}

		public override string ToString()
		{
			return "(" + leftOprnd.ToString() + binOp.ToString() + rightOprnd.ToString() + ")";
		}

	}

}