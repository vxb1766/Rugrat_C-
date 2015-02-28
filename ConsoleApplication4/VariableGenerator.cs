using System;
using System.Collections.Generic;

namespace edu.uta.cse.proggen.expressions
{


	using Method = edu.uta.cse.proggen.classLevelElements.Method;
	using Type = edu.uta.cse.proggen.classLevelElements.Type;
	using Variable = edu.uta.cse.proggen.classLevelElements.Variable;
	using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;
	using Operand = edu.uta.cse.proggen.nodes.Operand;
	using ProgGenUtil = edu.uta.cse.proggen.util.ProgGenUtil;
	using CallType = edu.uta.cse.proggen.util.ProgGenUtil.CallType;

	/// <summary>
	/// Class with utility methods to fetch a variable from a generated class
	/// based on various criteria.
	/// 
    /// @author Team 6 - CSE6324 - Spring 2015
	/// 
	/// </summary>
	public class VariableGenerator
	{
		public static Variable getRandomizedVariable(Method method)
		{
			Variable variable;
			List<Variable> parameterList = method.ParameterList;
			if (ProgGenUtil.methodCallType == ProgGenUtil.CallType.crossClassWithRecursionLimit || ProgGenUtil.methodCallType == ProgGenUtil.CallType.localWithRecursionLimit)
			{
				//ignore the first parameter: 'recursionCounter'
				// if 'recursionCounter' is the only parameter, return null (nextInt(0) throws exception)
				if (parameterList.Count == 1)
				{
					return null;
				}

				variable = parameterList[(new Random()).Next(parameterList.Count - 1) + 1];
			}
			else
			{
				if (parameterList.Count == 0)
				{
					return null;
				}
				variable = parameterList[(new Random()).Next(parameterList.Count)];
			}

			method.UsedParameterList.Add(variable); // adding to the used variable Set
			return variable;
		}

		public static Operand getRandomizedVariable(Method method, Type.Primitives primitive)
		{
			Variable variable;
			variable = getVariable(method.ParameterList, primitive);

			if (variable == null)
			{
				return new Literal(primitive,Int32.MaxValue);
			}

			method.UsedParameterList.Add(variable);
			return variable;
		}

		private static Variable getVariable(List<Variable> variables, Type.Primitives primitive)
		{
			List<Variable> typedVariableList = new List<Variable>();

			foreach (Variable @var in variables)
			{
				if (@var.Name.Equals("recursionCounter"))
				{
					continue;
				}

				if (@var.Type.getType() == primitive)
				{
					typedVariableList.Add(@var);
				}
			}

			if (typedVariableList.Count == 0)
			{
				return null;
			}

			int index = (new Random()).Next(typedVariableList.Count);
			return typedVariableList[index];
		}

		public static Operand getRandomizedObjectForType(Method method, Type type)
		{
			List<Variable> variableList = method.ParameterList;
			List<Variable> typedVariableList = new List<Variable>();
			foreach (Variable @var in variableList)
			{
				if (@var.Type.Equals(type))
				{
					typedVariableList.Add(@var);
				}
			}

			if (typedVariableList.Count == 0)
			{
                return new Literal(Type.Primitives.OBJECT, Int32.MaxValue);
			}

			int index = (new Random()).Next(typedVariableList.Count);
			return typedVariableList[index];
		}
	}

}