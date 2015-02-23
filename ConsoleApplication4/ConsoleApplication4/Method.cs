using System;
using System.Collections.Generic;
using System.Text;

namespace edu.uta.cse.proggen.classLevelElements
{


	using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;
	using FieldGenerator = edu.uta.cse.proggen.expressions.FieldGenerator;
	using Literal = edu.uta.cse.proggen.expressions.Literal;
	using VariableGenerator = edu.uta.cse.proggen.expressions.VariableGenerator;
	using Operand = edu.uta.cse.proggen.nodes.Operand;
	using ClassGenerator = edu.uta.cse.proggen.packageLevelElements.ClassGenerator;
	using ForLoop = edu.uta.cse.proggen.statements.ForLoop;
	using IfElse = edu.uta.cse.proggen.statements.IfElse;
	using IfStmtIfStmt = edu.uta.cse.proggen.statements.IfStmtIfStmt;
	using Switch = edu.uta.cse.proggen.statements.Switch;
	using ProgGenUtil = edu.uta.cse.proggen.util.ProgGenUtil;
	using CallType = edu.uta.cse.proggen.util.ProgGenUtil.CallType;

	/// <summary>
	/// This class represents a method in the generated class.
	/// 
	/// @author balamurugan
	/// 
	/// </summary>
	public class Method
	{
		private bool isStatic;
		private int numberOfParams;
		private ClassGenerator associatedClass;
		private List<ClassGenerator> classList;
		private string name;
		private int loc;
		private int locPerMethod;
		private Primitives returnType = ProgGenUtil.RandomizedPrimitive;

		private List<Variable> parameterList = new List<Variable>();
		private HashSet<Variable> usedParameterList = new HashSet<Variable>();

		private string methodBody;
		private string returnStatement;
		private int maxNestedIfs;
		private int nestedIfCounter;
		private int maxAllowedMethodCalls;
		private int methodCallCounter;
		private MethodSignature methodSignature;
		private HashSet<MethodSignature> calledMethods = new HashSet<MethodSignature>();
		private HashSet<string> calledMethodsWithClassName = new HashSet<string>();



		/// <summary>
		/// Private constructor to create the method.
		/// </summary>
		/// <param name="signature"> : signature of the method to be constructed </param>
		/// <param name="associatedClass"> : class to which the method belongs </param>
		/// <param name="classList"> : list of generated types </param>
		/// <param name="locPerMethod"> : lines of code for this method </param>
		/// <param name="maxNestedIfs"> : upper limit on the nested ifs. </param>
		/// <param name="maxAllowedMethodCalls"> : upper limit on the method calls. </param>
		private Method(MethodSignature signature, ClassGenerator associatedClass, List<ClassGenerator> classList, int locPerMethod, int maxNestedIfs, int maxAllowedMethodCalls)
		{
			Console.WriteLine("Constructing method..." + signature.Name);
			this.isStatic = signature.Static;
			this.numberOfParams = signature.ParameterList.Count;
			this.associatedClass = associatedClass;
			this.classList = classList;
			this.name = signature.Name;
			this.loc = 0;
			this.nestedIfCounter = 0;
			this.locPerMethod = locPerMethod;
			this.maxNestedIfs = maxNestedIfs;
			this.maxAllowedMethodCalls = maxAllowedMethodCalls;
			this.methodCallCounter = 0;

			this.methodSignature = signature;
			this.parameterList = signature.ParameterList;
			this.returnType = signature.ReturnType;
			generateMethodBody();
			generateReturnStatement();
		}

		/// <summary>
		/// Static API to get a generated method.
		/// </summary>
		/// <param name="signature"> </param>
		/// <param name="associatedClass"> </param>
		/// <param name="classList"> </param>
		/// <param name="loc"> </param>
		/// <param name="maxNestedIfs"> </param>
		/// <param name="maxMethodCalls">
		/// @return </param>
		public static Method getMethod(MethodSignature signature, ClassGenerator associatedClass, List<ClassGenerator> classList, int loc, int maxNestedIfs, int maxMethodCalls)
		{
			 return new Method(signature, associatedClass, classList, loc, maxNestedIfs, maxMethodCalls);
		}

		public virtual Primitives ReturnType
		{
			get
			{
				return returnType;
			}
		}

		/// <summary>
		/// Constructor that creates a method with/without
		/// generating the method body based on the generateMethodBody
		/// parameter.
		/// </summary>
		/// <param name="isStatic"> </param>
		/// <param name="numberOfParams"> </param>
		/// <param name="associatedClass"> </param>
		/// <param name="classList"> </param>
		/// <param name="name"> </param>
		/// <param name="locPerMethod"> </param>
		/// <param name="maxNestedIfs"> </param>
		/// <param name="maxAllowedMethodCalls"> </param>
		/// <param name="generateMethodBody"> </param>
		private Method(bool isStatic, int numberOfParams, ClassGenerator associatedClass, List<ClassGenerator> classList, string name, int locPerMethod, int maxNestedIfs, int maxAllowedMethodCalls, bool generateMethodBody)
		{
			Console.WriteLine("Constructing method..." + name);
			this.isStatic = isStatic;
			this.numberOfParams = numberOfParams;
			this.associatedClass = associatedClass;
			this.classList = classList;
			this.name = name;
			this.loc = 0;
			this.nestedIfCounter = 0;
			this.locPerMethod = locPerMethod;
			this.maxNestedIfs = maxNestedIfs;
			this.maxAllowedMethodCalls = maxAllowedMethodCalls;
			this.methodCallCounter = 0;

			generateParams();

			this.methodSignature = new MethodSignature(this.isStatic, this.returnType, this.name, this.parameterList);

			if (generateMethodBody)
			{
				generateMethodBody();
				generateReturnStatement();
			}
		}

		/// <summary>
		/// Generates an appropriate return statement for the Method.
		/// </summary>
		private void generateReturnStatement()
		{
			StringBuilder builder = new StringBuilder("return ");
			builder.Append("(");
			builder.Append(this.returnType);
			builder.Append(")");

			int choiceVarOrLiteral = (new Random()).Next(1);
			Operand operand;

			if (choiceVarOrLiteral == 0)
			{
				operand = VariableGenerator.getRandomizedVariable(this, this.returnType);
			}
			else
			{
				operand = new Literal(this.returnType);
			}

			builder.Append(operand);
			builder.Append(";\n");
			this.returnStatement = builder.ToString();
		}

		private void generateMethodBody()
		{
			string output = "";
			Random rand = new Random();

			if (loc < locPerMethod)
			{
				//for the case of indirect recursion.
				//this helps to stop the chain of method calls within
				//the configured limit.
				if (ProgGenUtil.methodCallType == ProgGenUtil.CallType.crossClassWithRecursionLimit || ProgGenUtil.methodCallType == ProgGenUtil.CallType.localWithRecursionLimit)
				{
					output += generateBaseCaseForRecursion();
				}

				if (rand.Next(100) < ProgGenUtil.recursionProbability)
				{
					loc += 1;
					//method to include recursive calls.
					output += RecursiveMethodCall;
				}

				//introduce a instance of the current class
				if (ProgGenUtil.methodCallType == ProgGenUtil.CallType.crossClassWithoutRecursionLimit || ProgGenUtil.methodCallType == ProgGenUtil.CallType.crossClassWithRecursionLimit)
				{
					string classname = AssociatedClass.FileName;
					output += classname + " classObj = new " + classname + "();\n";
					loc += 1;
				}
			}

			while (loc < locPerMethod)
			{
					int option = 0;
					if (ProgGenUtil.AllowedTypes.contains("int"))
					{
						if (ProgGenUtil.getValidPrimitivesInScope(this).contains(Primitives.INT))
						{
							option = rand.Next(4);
						}
						else
						{
							//can contain for-loops but not switch statements
							option = rand.Next(3);
						}
					}
					else
					{
						option = rand.Next(2);
					}

					switch (option)
					{
						case 0:
							output += (new IfStmtIfStmt(this, classList)).ToString();
							break;
						case 1:
							output += (new IfElse(this, classList)).ToString();
							break;
							/*
							 * cases after this not valid if INT is not 
							 * a user-specified type.
							 */
						case 2:
							output += (new ForLoop(this, classList)).ToString();
							break;
						case 3:
							output += (new Switch(this, classList)).ToString();
							break;
					}
			}
				this.methodBody = output;
		}

		private string generateBaseCaseForRecursion()
		{
			//generate a return statement for the base case
			generateReturnStatement();

			StringBuilder builder = new StringBuilder("if(! (recursionCounter > 0 && recursionCounter < " + ProgGenUtil.maxRecursionDepth + ") )");
			builder.Append("\n{\n");
			builder.Append(this.returnStatement);
			builder.Append("\n}\n");
			builder.Append("else\n{\n recursionCounter--; \n}\n");
			//add 4 to lines of count.
			loc += 4;
			return builder.ToString();
		}

		private string RecursiveMethodCall
		{
			get
			{
				Console.WriteLine("Inside recursive method call for : " + methodSignature);
				string parameters = "(";
    
				foreach (Variable @var in parameterList)
				{
					if (@var.Name.Equals("recursionCounter"))
					{
						parameters += "recursionCounter,";
						continue;
					}
    
					Operand operand;
					Primitives primitive = @var.Type.Type;
					int optionVariableOrField = (new Random()).Next(1);
					if (optionVariableOrField == 0)
					{
						if (primitive == Primitives.OBJECT)
						{
							operand = VariableGenerator.getRandomizedObjectForType(this, @var.Type);
						}
						else
						{
							operand = VariableGenerator.getRandomizedVariable(this, primitive);
						}
					}
					else
					{
						if (primitive == Primitives.OBJECT)
						{
							operand = FieldGenerator.getRandomizedObjectForType(this.AssociatedClass, @var.Type);
						}
						else
						{
							operand = FieldGenerator.getRandomField(this.AssociatedClass, primitive, this.Static);
						}
					}
					parameters += operand + ",";
				}
    
				parameters = parameters.Substring(0, parameters.Length - 1);
				parameters += ")";
    
				return this.Name + parameters + ";\n\n";
			}
		}

		public virtual HashSet<string> CalledMethodsWithClassName
		{
			get
			{
				return calledMethodsWithClassName;
			}
		}

		public virtual List<Variable> ParameterList
		{
			get
			{
				return parameterList;
			}
		}

		public virtual ClassGenerator AssociatedClass
		{
			get
			{
				return associatedClass;
			}
		}

		public virtual HashSet<Variable> UsedParameterList
		{
			get
			{
				return usedParameterList;
			}
		}

		/// <summary>
		/// Generate randomized parameters for this method.
		/// </summary>
		private void generateParams()
		{
			if (ProgGenUtil.methodCallType == ProgGenUtil.CallType.crossClassWithRecursionLimit || ProgGenUtil.methodCallType == ProgGenUtil.CallType.localWithRecursionLimit)
			{
				parameterList.Add(Variable.generateRecursionCounterVariable());
				for (int i = 1; i < numberOfParams; i++)
				{ //classList is passed because: if Variable's type is Object, then to select type
					parameterList.Add(Variable.generateVariable("var" + i, classList));
				}
			}
			else
			{
				for (int i = 0; i < numberOfParams; i++)
				{
					parameterList.Add(Variable.generateVariable("var" + i, classList));
				}
			}
		}

		public virtual int Loc
		{
			get
			{
				return loc;
			}
			set
			{
				this.loc = value;
			}
		}


		public virtual MethodSignature MethodSignature
		{
			get
			{
				return methodSignature;
			}
		}

		public virtual string Name
		{
			get
			{
				return name;
			}
		}

		public virtual int MethodCallCounter
		{
			get
			{
				return methodCallCounter;
			}
			set
			{
				this.methodCallCounter = value;
			}
		}


		public virtual int NestedIfCounter
		{
			get
			{
				return nestedIfCounter;
			}
			set
			{
				this.nestedIfCounter = value;
			}
		}


		public virtual bool Static
		{
			get
			{
				return isStatic;
			}
		}

		public virtual int MaxAllowedMethodCalls
		{
			get
			{
				return maxAllowedMethodCalls;
			}
			set
			{
				this.maxAllowedMethodCalls = value;
			}
		}


		public virtual List<ClassGenerator> ClassList
		{
			get
			{
				return classList;
			}
		}

		public virtual int MaxNestedIfs
		{
			get
			{
				return maxNestedIfs;
			}
		}

		/// <summary>
		/// API to generate a method for a class.
		/// </summary>
		/// <param name="generator"> </param>
		/// <param name="maxNumberOfParams"> </param>
		/// <param name="classList"> </param>
		/// <param name="name"> </param>
		/// <param name="maxLocPerMethod"> </param>
		/// <param name="maxNestedIfs"> </param>
		/// <param name="maxAllowedMethodCalls"> </param>
		/// <param name="generateMethodBody">
		/// @return </param>
		public static Method generateMethod(ClassGenerator generator, int maxNumberOfParams, List<ClassGenerator> classList, string name, int maxLocPerMethod, int maxNestedIfs, int maxAllowedMethodCalls, bool generateMethodBody)
		{
			bool isStatic = ProgGenUtil.coinFlip();

			//we need minimum of two parameters
			int numberOfParams = ProgGenUtil.getRandomIntInRange(maxNumberOfParams);

	//		if(numberOfParams<2)
			if (numberOfParams < ProgGenUtil.minNoOfParameters)
			{
				numberOfParams = ProgGenUtil.minNoOfParameters;
			}

			return new Method(isStatic, numberOfParams, generator, classList, name, maxLocPerMethod, maxNestedIfs, maxAllowedMethodCalls, generateMethodBody);
		}

		public override string ToString()
		{
			string str = "";

			str += this.methodSignature;
			str += "{\n " + methodBody + returnStatement + "\n}\n\n";

			return str;
		}

		/// <summary>
		/// API to generate methods for an interface. Generated methods will not have 
		/// a body generated. Such methods are used only for extracting signatures.
		/// </summary>
		/// <param name="maxNoOfParameters"> </param>
		/// <param name="classList"> </param>
		/// <param name="name">
		/// @return </param>
		public static Method generateMethodForInterface(int maxNoOfParameters, List<ClassGenerator> classList, string name)
		{
			int numberOfParams = ProgGenUtil.getRandomIntInRange(maxNoOfParameters);

	//		if(numberOfParams < 2)
			if (numberOfParams < ProgGenUtil.minNoOfParameters)
			{
				numberOfParams = ProgGenUtil.minNoOfParameters;
			}
			//TODO: So, interface methods will have at least 2 parameters, but why?
			return new Method(false, numberOfParams, null, classList, name, 0, 0, 0, false);
		}

		public virtual HashSet<MethodSignature> CalledMethods
		{
			get
			{
				return calledMethods;
			}
		}

	}
}