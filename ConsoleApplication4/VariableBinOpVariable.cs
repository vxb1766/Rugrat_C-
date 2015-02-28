using System;
using System.Text.RegularExpressions;
namespace edu.uta.cse.proggen.expressions
{

	using Method = edu.uta.cse.proggen.classLevelElements.Method;
	using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;
	using Expression = edu.uta.cse.proggen.nodes.Expression;
	using BinaryOperator = edu.uta.cse.proggen.operators.BinaryOperator;

    /// <summary>
    /// 
    /// @author Team 6 - CSE6324 - Spring 2015
    /// 
    /// </summary>

	public class VariableBinOpVariable : Expression
	{

		private Expression leftOprnd = null;
		private BinaryOperator binOp = null;
		private Expression rightOprnd = null;

		public VariableBinOpVariable(Method method, Primitives primitive)
		{

			Random rand = new Random();

			int nested = rand.Next(100);
			if (nested < method.AssociatedClass.Percent)
			{
				leftOprnd = new VariableBinOpVariable(method, primitive);
				binOp = new BinaryOperator(primitive);
				rightOprnd = new VariableBinOpVariable(method, primitive);
			}
			else
			{
				leftOprnd = VariableGenerator.getRandomizedVariable(method, primitive);
				binOp = new BinaryOperator(primitive);
				rightOprnd = VariableGenerator.getRandomizedVariable(method, primitive);
			}


			// removing expressions of the form: (i5-i5) to avoid {i19%(i5-i5)}expr.
			if (leftOprnd.ToString().Equals(rightOprnd.ToString()))
			{
                //rightOprnd = new Literal(primitive);
                //String test = rightOprnd.ToString().SubstringSpecial(rightOprnd.ToString().Length-1, rightOprnd.ToString().Length);
                //test = test;
                //Primitives t = Primitives.OTHER;
                //String testValue = t.ToString();

                string pattern = "[^0-9]+";
                string replacement = "";
                Regex rgx = new Regex(pattern);
                string test = rgx.Replace(rightOprnd.ToString(), replacement);
                Primitives t = Primitives.OTHER;

                rightOprnd = new Literal(t,Convert.ToInt32(test));
            }
            else
            {
                bool testfalse = false;
            }

			// for division and modulo operations, keeping only literals in the right expr.
			// i5%i3 => i5%constantValue OR f2/f4 => f2/constantValue
            bool test1 = binOp.ToString().Equals("/") || binOp.ToString().Equals("%");
            //if (binOp.ToString().Equals("/") || binOp.ToString().Equals("%"))
            //{
            //    do
            //    { //FIXME: only handles int for now.
            //        rightOprnd = new Literal(primitive,Int32.MaxValue);
            //        rightOprnd.ToString().EndsWith("0");


            //    }while ( rightOprnd.ToString().EndsWith("0")); //avoiding divide by (0)
            //}

		}

		public override string ToString()
		{
			return "(" + leftOprnd.ToString() + binOp.ToString() + rightOprnd.ToString() + ")";
		}
	}

}