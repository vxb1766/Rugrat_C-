using System;
using System.Linq;
using System.Collections.Generic;

namespace edu.uta.cse.proggen.statements
{


	using Method = edu.uta.cse.proggen.classLevelElements.Method;
	using Type = edu.uta.cse.proggen.classLevelElements.Type;
	using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;
	using AssignmentExpression = edu.uta.cse.proggen.expressions.AssignmentExpression;
	using VariableGenerator = edu.uta.cse.proggen.expressions.VariableGenerator;
	using Operand = edu.uta.cse.proggen.nodes.Operand;
	using ClassGenerator = edu.uta.cse.proggen.packageLevelElements.ClassGenerator;
	using ProgGenUtil = edu.uta.cse.proggen.util.ProgGenUtil;

	/// <summary>
	/// Statements: i1 = a+b;
	/// 
	/// @author Ishtiaque_Hussain
	/// 
	/// </summary>
	public class Statement
	{
		internal string stmt = "";

		public static Statement getRandomizedStatement(Method method, List<ClassGenerator> classList)
		{
			Statement stmt = new Statement();
			Random rand = new Random();
			int option = 0;

			option = rand.Next(100) % 3;
			switch (option)
			{
			case 0: // Assignment statement
				stmt.stmt = (new AssignmentExpression(method)).ToString();
				//FIXME: if there is no local variable available to assign, it will simply use print statement.
				// following line will add an extra line to the LOC
				method.Loc = method.Loc + 1;
				break;

			case 1: // Print statements: Max. 5 lines in a block
				stmt.stmt = (new PrintStatement(method)).ToString();
				break;

			case 2: // method calls: restrict it to MAX_ALLOWED_METH_CALL
				int methCalledCounterValue = method.MethodCallCounter + 1;
				method.MethodCallCounter = methCalledCounterValue;
				if (methCalledCounterValue < method.MaxAllowedMethodCalls)
				{
					if (ProgGenUtil.coinFlip())
					{
						stmt.stmt += ProgGenUtil.getMethodCall(method, classList);
					}
					else
					{
						//wire it to variables in scope
						Operand lhs;
						Random random = new Random();

						HashSet<Type.Primitives> validPrimitivesInScope = ProgGenUtil.getValidPrimitivesInScope(method);

						//Pick a type
                        Primitives[] primitivesArray = validPrimitivesInScope.ToArray();

						if (primitivesArray.Length == 0)
						{
							stmt.stmt += ProgGenUtil.getMethodCall(method, classList);
							break;
						}

						Type.Primitives selectedPrimitive = (Type.Primitives)primitivesArray[random.Next(primitivesArray.Length)];

						// Introducing any variable
						lhs = VariableGenerator.getRandomizedVariable(method, selectedPrimitive);

						string methodCall = ProgGenUtil.getMethodCallForReturnType(method, classList, new Type(selectedPrimitive, ""), lhs);
						stmt.stmt += methodCall + "\n";
					}
				}
				else
				{
					stmt.stmt = (new PrintStatement(method)).ToString();
				}
				break;

			default:
				stmt.stmt = (new IfStmtIfStmt(method, classList)).ToString();
			break;
			}

			return stmt;
		}

		public override string ToString()
		{
			return stmt;
		}
	}

}