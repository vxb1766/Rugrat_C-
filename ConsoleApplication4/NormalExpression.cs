using System;
using System.Collections.Generic;

namespace edu.uta.cse.proggen.expressions
{


	using Field = edu.uta.cse.proggen.classLevelElements.Field;
	using Method = edu.uta.cse.proggen.classLevelElements.Method;
	using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;
	using Expression = edu.uta.cse.proggen.nodes.Expression;

	/// <summary>
	/// normalExpr:= operand binOp operand | operand | methodCall
	/// 
    /// @author Team 6 - CSE6324 - Spring 2015
	/// 
	/// </summary>
	public class NormalExpression : Expression
	{
		private Expression subExpr = null;

		public NormalExpression(Method method, Primitives primitive)
		{
			HashSet<Primitives> fieldPrimitivesList = getFieldPrimitiveList(method.AssociatedClass.Fields, method.Static);

			if (fieldPrimitivesList.Contains(primitive))
			{
				Random rand = new Random();
				int option = rand.Next(100) % 5;
				switch (option)
				{
					case 4:
						subExpr = new FieldBinOpField(method, primitive);
						break;

					default:
						subExpr = new VariableBinOpVariable(method, primitive);
						break;
				}
			}
			else
			{
				subExpr = new VariableBinOpVariable(method, primitive);
			}
		}

		private HashSet<Primitives> getFieldPrimitiveList(List<Field> fieldList, bool isStatic)
		{
			HashSet<Primitives> primitivesSet = new HashSet<Primitives>();
			foreach (Field field in fieldList)
			{
				if (field.Static == isStatic)
				{
					primitivesSet.Add(field.type.getType());
				}
			}
			return primitivesSet;
		}

		public override string ToString()
		{
			return subExpr.ToString();
		}
	}

}