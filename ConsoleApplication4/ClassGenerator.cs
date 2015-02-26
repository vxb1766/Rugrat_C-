using System;
using System.Collections.Generic;
using System.Text;

namespace edu.uta.cse.proggen.packageLevelElements
{


	using Field = edu.uta.cse.proggen.classLevelElements.Field;
	using Method = edu.uta.cse.proggen.classLevelElements.Method;
	using MethodSignature = edu.uta.cse.proggen.classLevelElements.MethodSignature;
	using Type = edu.uta.cse.proggen.classLevelElements.Type;
	using Variable = edu.uta.cse.proggen.classLevelElements.Variable;
	using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;
	using ConfigurationXMLParser = edu.uta.cse.proggen.configurationParser.ConfigurationXMLParser;
	using Literal = edu.uta.cse.proggen.expressions.Literal;
	using ProgGenUtil = edu.uta.cse.proggen.util.ProgGenUtil;

	/// <summary>
	/// Represents a class in the generated application.
	/// 
	/// @author balamurugan
	/// 
	/// </summary>
	public class ClassGenerator
	{
		private string fileName = "";
		private int numOfVars = 0;
		private int percent = 0;
		private int loc = 0;
		private int nestedIfCounter = 0;
		private int maxNestedIfs = 30;
        private int maxAllowedMethodCalls = 5;//ProgGenUtil.maxNoOfMethodCalls;
		private int methCounter = 0;
		private int methCalledCounter = 0;
		private int locPerMethod = 0;
		private bool preGenerate = false;

		/// <summary>
		/// Holds actual class body </summary>
		private string program = "";
		private int numberOfMethods = 1;
       
        //Veena : milliseconds introduced coz date time helper is causing issue.
        static long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        private static Int32 randInput = Convert.ToInt32(2147483647);
        private Random rand = new Random(randInput);

		private HashSet<Field> usedFields = new HashSet<Field>();
		private List<Field> fields = new List<Field>();

		private ClassGenerator superClass = null;
		private HashSet<ClassGenerator> subClasses = new HashSet<ClassGenerator>();

		private List<Method> methodList = new List<Method>();
		private List<MethodSignature> overriddenMethods = new List<MethodSignature>();
		private List<MethodSignature> methodSignatures = new List<MethodSignature>();

		private List<InterfaceGenerator> interfaceList = new List<InterfaceGenerator>();
		private HashSet<Type.Primitives> returnTypeSet = new HashSet<Type.Primitives>();


		/// <param name="fileName"> </param>
		/// <param name="loc"> </param>
		/// <param name="superClass"> </param>
		public ClassGenerator(string fileName, int loc, ClassGenerator superClass)  //: base()
		{
			this.fileName = fileName;
			//Atleast one variable per class. Added in response to Ishtiaque's comment.
			int maxNoOfVars = ConfigurationXMLParser.getPropertyAsInt("maxNoOfClassFields");
			int minNoOfFields = ConfigurationXMLParser.getPropertyAsInt("minNoOfClassFields");

			// to avoid Illegal argument exception
			if (maxNoOfVars != 0)
			{
				this.numOfVars = (new Random()).Next(maxNoOfVars);
			}
			else
			{
				this.numOfVars = 0;
			}

			if (numOfVars < minNoOfFields)
			{
				numOfVars = minNoOfFields;
			}
			this.numberOfMethods = 0;
			this.loc = loc;
			this.percent = ConfigurationXMLParser.getPropertyAsInt("probability");
			this.maxNestedIfs = ConfigurationXMLParser.getPropertyAsInt("maxNestedIfs");
			this.maxAllowedMethodCalls = ConfigurationXMLParser.getPropertyAsInt("maxAllowedMethodCalls");
			this.superClass = superClass;
		}

		/// <summary>
		/// pre-generation determines the class members, variables and method signatures </summary>
		/// <param name="classList"> </param>
		/// <param name="preGeneratedClasses"> </param>
		public virtual void preGenerateForMethodSignature(List<ClassGenerator> classList, HashSet<string> preGeneratedClasses)
		{
			if (preGenerate)
			{
				//pre generation should happen only once.
				return;
			}
			// superclass should have its methods signature ready first
			if (this.SuperClass != null && (!preGeneratedClasses.Contains(this.SuperClass.FileName)))
			{
				this.SuperClass.preGenerateForMethodSignature(classList, preGeneratedClasses);
			}

			// add class fields
			//classList is used all the way down to "new Type()"
			generateClassFields(classList);

			Console.WriteLine("calculating number of methods...");
			//calculate number of methods
			calculateNumberOfMethods();

			Console.WriteLine("overriding interface methods...");
			//first override methods of implemented interfaces
			overrideInterfaceMethods(classList);

			Console.WriteLine("generating member method signatures...");
			generateMethodSignatures(classList);

			preGeneratedClasses.Add(fileName);
			preGenerate = true;
		}
        //Veena : Added this to test what was wrong.
        //public static void Main(string[] args)
       // {
       //     ClassGenerator test = new ClassGenerator("TP0", 100 / 5, null);
      //      Console.Write(test);
      //      Console.ReadLine();
       // }
		/// <summary>
		/// Generates the actual body or content of the class 
		/// and updates the set  'generatedClasses' Set of </summary>
		/// <param name="classList"> List of created class objects </param>
		/// <param name="generatedClasses"> Set of already generated class names </param>
		public virtual void generate(List<ClassGenerator> classList, HashSet<string> generatedClasses, HashSet<string> preGeneratedClasses)
		{
			program = "";

			if (this.SuperClass != null && (!generatedClasses.Contains(this.SuperClass.FileName)))
			{
				this.SuperClass.generate(classList, generatedClasses, preGeneratedClasses);
			}

			if (!preGenerate)
			{
				preGenerateForMethodSignature(classList, preGeneratedClasses);
			}

			// append package name
			appendPackageName();

			//append import statements
			if (ProgGenUtil.useQueries)
			{
				appendImportStatements();
			}

			Console.WriteLine("appending classname...");
			// append class name
			appendClassName();

			Console.WriteLine("Injecting contents...");
			//append injected contents from external file
			appendInjectedContents();

			Console.WriteLine("appending field names...");
			// append field names
			appendFieldNames();

			Console.WriteLine("generating class methods...");
			//generate methods
			generateMethods(classList);

			Console.WriteLine("writing main...");
			generateMain();

			//generate SingleEntry for CarFast
			Console.WriteLine("writing singleEntry.....");
			generateSingleEntry();

			Console.WriteLine("writing end of class...");
			//write end of class
			generateEndOfClass();

			generatedClasses.Add(this.FileName);
		}

		private void appendImportStatements()
		{
			program += "import java.sql.ResultSet;\n";
			program += "import java.util.Random;\n\n\n";
		}

		private void appendInjectedContents()
		{
			program += ProgGenUtil.InjectContents;
		}

		private void overrideInterfaceMethods(List<ClassGenerator> classList)
		{
			foreach (InterfaceGenerator interfaceGen in interfaceList)
			{
				foreach (MethodSignature signature in interfaceGen.MethodSignatures)
				{
					Type.Primitives returnType = signature.ReturnType;
					if (returnType != Type.Primitives.OBJECT)
					{
						returnTypeSet.Add(returnType);
					}
				}
				methodSignatures.AddRange(interfaceGen.MethodSignatures);
			}
		}

		private void generateMethods(List<ClassGenerator> classList)
		{
			foreach (MethodSignature signature in methodSignatures)
			{
				Method method = Method.getMethod(signature, this, classList, locPerMethod, this.maxNestedIfs, this.maxAllowedMethodCalls);
				program += method;
				methodList.Add(method);
			}
		}

		private void calculateNumberOfMethods()
		{
			int totalNumberOfMethods = 0;
			numberOfMethods = 0;

			//atleast two methods
			if (ProgGenUtil.maxNoOfMethodsPerClass < 2)
			{
				Console.WriteLine("Setting number of methods per class as 2.");
				numberOfMethods = 2;
			}
			else if (ProgGenUtil.maxNoOfMethodsPerClass == 2)
			{
				numberOfMethods = 2;
			}
			else
			{
				numberOfMethods = rand.Next(ProgGenUtil.maxNoOfMethodsPerClass - 2) + 2;
			}

			totalNumberOfMethods += numberOfMethods;
			//add the number of methods from the implemented interfaces.
			foreach (InterfaceGenerator interfaceGen in interfaceList)
			{
				totalNumberOfMethods += interfaceGen.NumOfMethods;
			}

			locPerMethod = this.loc / totalNumberOfMethods;
		}

		public virtual int MaxNestedIfs
		{
			get
			{
				return maxNestedIfs;
			}
			set
			{
				this.maxNestedIfs = value;
			}
		}


		public virtual List<MethodSignature> MethodSignatures
		{
			get
			{
				return methodSignatures;
			}
		}

		public virtual HashSet<Type.Primitives> ReturnTypeSet
		{
			get
			{
				return returnTypeSet;
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


		public virtual int MethCounter
		{
			get
			{
				return methCounter;
			}
			set
			{
				this.methCounter = value;
			}
		}


		public virtual ClassGenerator SuperClass
		{
			get
			{
				return superClass;
			}
			set
			{
				this.superClass = value;
			}
		}


		public virtual List<InterfaceGenerator> InterfaceList
		{
			get
			{
				return interfaceList;
			}
			set
			{
				this.interfaceList = value;
			}
		}


		public virtual int Percent
		{
			get
			{
				return percent;
			}
		}

		public virtual List<Method> MethodList
		{
			get
			{
				return this.methodList;
			}
			set
			{
				this.methodList = value;
			}
		}


		public virtual HashSet<ClassGenerator> SubClasses
		{
			get
			{
				return subClasses;
			}
		}

		public virtual int MethCalledCounter
		{
			get
			{
				return methCalledCounter;
			}
			set
			{
				this.methCalledCounter = value;
			}
		}


		public virtual HashSet<Field> UsedFields
		{
			get
			{
				return usedFields;
			}
			set
			{
				this.usedFields = value;
			}
		}


		public virtual List<Field> Fields
		{
			get
			{
				return fields;
			}
			set
			{
				this.fields = value;
			}
		}


		public override string ToString()
		{
			return program;
		}

		private void generateMain()
		{
			program += "\npublic static void main(string[] args){\n";
			program += this.fileName + " obj = new " + this.fileName + "();\n";
			foreach (Method method in this.methodList)
			{
				StringBuilder builder = new StringBuilder();

				MethodSignature signature = method.MethodSignature;
				if (!signature.Static)
				{
					builder.Append("obj.");
				}

				builder.Append(signature.Name + "(");

				foreach (Variable variable in signature.ParameterList)
				{
					Type type = variable.Type;
					if (type.getType() == Type.Primitives.OBJECT)
					{
						builder.Append("new " + type.ToString() + "()");
						builder.Append(",");
					}
					else
					{
						if (variable.Name.Equals("recursionCounter"))
						{
							builder.Append((new Random()).Next(ProgGenUtil.maxRecursionDepth));
						}
						else
						{
							builder.Append(new Literal(type.getType()));
						}
						builder.Append(",");
					}
				}
				string str = builder.ToString();
				str = str.Substring(0, str.Length - 1);
				str += ");\n";

				program += str;
			}
			program += "}\n";
		}


		//Single Entry point for CarFast
		//TODO: support other parameter types
		private void generateSingleEntry()
		{
			program += "\npublic static void singleEntry(";

			StringBuilder param = new StringBuilder();

			for (int i = 0; i < ProgGenUtil.maxNoOfParameters; i++)
			{
				param.Append("int i" + i + ",");
			}

			string st = param.ToString();
			st = st.Substring(0, st.Length - 1);
			st += "){\n";

			program += st;


			program += this.fileName + " obj = new " + this.fileName + "();\n";
			foreach (Method method in this.methodList)
			{
				HashSet<int?> set = new HashSet<int?>();
				StringBuilder builder = new StringBuilder();

				MethodSignature signature = method.MethodSignature;
				if (!signature.Static)
				{
					builder.Append("obj.");
				}

				builder.Append(signature.Name + "(");

				foreach (Variable variable in signature.ParameterList)
				{
					Type type = variable.Type;
					if (type.getType() == Type.Primitives.OBJECT)
					{
						builder.Append("new " + type.ToString() + "()");
						builder.Append(",");
					}
					else
					{
						if (variable.Name.Equals("recursionCounter"))
						{
							builder.Append((new Random()).Next(ProgGenUtil.maxRecursionDepth));
						}
						else
						{
                            if (type.getType() != Type.Primitives.INT)
							{
                                builder.Append(new Literal(type.getType()));
							}
							else
							{
								bool addedToSet = false;
								do
								{
									int @var = (new Random()).Next(ProgGenUtil.maxNoOfParameters);
									addedToSet = set.Add(@var);
									if (addedToSet)
									{
										builder.Append("i" + @var);
									}
								}while (!addedToSet);

							}
						}
						builder.Append(",");
					}
				}
				string str = builder.ToString();
				str = str.Substring(0, str.Length - 1);
				str += ");\n";

				program += str;
			}
			program += "}\n";
		}

		private void generateEndOfClass()
		{
			// closing brace of the class
			program += "\n}";
            //closing brace of namespace
            program += "\n}";
		}

		private MethodSignature overRiddenMethod(List<ClassGenerator> classList, int loc)
		{
			List<MethodSignature> superClassMethodSignatures = this.superClass.methodSignatures;
			MethodSignature methodToOverride = superClassMethodSignatures[rand.Next(superClassMethodSignatures.Count)];
			int counter = this.superClass.numberOfMethods;

			while ((overriddenMethods.Contains(methodToOverride) || methodSignatures.Contains(methodToOverride)) && counter > 0)
			{
				methodToOverride = superClassMethodSignatures[rand.Next(superClassMethodSignatures.Count)];
				counter--;
			}

			if (counter == 0)
			{
				Console.WriteLine("overriddenMethod()::returning null due to lack of methods in super class.");
				return null;
			}

			overriddenMethods.Add(methodToOverride);
			Type.Primitives returnType = methodToOverride.ReturnType;
			if (returnType != Type.Primitives.OBJECT)
			{
				returnTypeSet.Add(returnType);
			}
			return methodToOverride;
		}


		private void generateMethodSignatures(List<ClassGenerator> classList)
		{
			for (int i = 0; i < numberOfMethods; i++)
			{
				if (this.superClass != null)
				{
					//flip a coin(1 out of 3) to decide for method override
					int choiceOverride = rand.Next(3);
					if (choiceOverride == 0)
					{
						MethodSignature overridenMethod = overRiddenMethod(classList, locPerMethod);
						if (overridenMethod != null)
						{
							methodSignatures.Add(overridenMethod);
							continue;
						}
						//else fallthrough and generate normal class method
					}
				}

				//Basically creating MethodSignature
				Method method = Method.generateMethod(this, ProgGenUtil.maxNoOfParameters, classList, this.fileName + "method" + i, locPerMethod, this.maxNestedIfs, this.maxAllowedMethodCalls, false);

				methodSignatures.Add(method.MethodSignature);
				Type.Primitives returnType = method.ReturnType;
				if (returnType != Type.Primitives.OBJECT)
				{
					returnTypeSet.Add(returnType);
				}
			}
		}

		/// <summary>
		/// Gives the corresponding class name </summary>
		/// <returns> Name of the Class </returns>
		public virtual string FileName
		{
			get
			{
				return fileName;
			}
		}

		private void appendFieldNames()
		{
			for (int i = 0; i < fields.Count; i++)
			{
				program += fields[i].FieldDeclaration + ";\n";
			}
			program += "\n\n";
		}

		private void appendClassName()
		{
			program += "public class " + fileName;

			if (this.superClass != null)
			{
				program += " extends " + superClass.FileName;
				superClass.addSubClass(this);
			}

			if (this.interfaceList.Count > 0)
			{
				program += " implements ";
				foreach (InterfaceGenerator interfaceGen in interfaceList)
				{
					Console.WriteLine("implementing interface : " + interfaceGen.Name);
					program += interfaceGen.Name + ", ";
				}

				program = program.Substring(0, program.Length - 2);
			}

			program += " {\n";
		}

		private void addSubClass(ClassGenerator classGenerator)
		{
			this.subClasses.Add(classGenerator);
		}

		private void appendPackageName()
		{
			//program += "package com.accenture.lab.carfast.test;\n\n\n";
            //Srujana: using System import necessary for all C# programs
            program += "using System;\n\n\n";
            //Srujana: C# packages are called namespaces
            program += "namespace com.accenture.lab.carfast.test{\n\n\n";

		}

		private void generateClassFields(List<ClassGenerator> classList)
		{
			for (int i = 0; i < numOfVars; i++)
			{
				fields.Add(Field.generateField("f" + i, classList));
			}
		}

		public virtual int NestedIfCounter
		{
			set
			{
				this.nestedIfCounter = value;
			}
			get
			{
				return nestedIfCounter;
			}
		}


		public virtual List<MethodSignature> getMethodSignatures(Type returnType)
		{
			List<MethodSignature> list = new List<MethodSignature>();
			foreach (MethodSignature signature in methodSignatures)
			{
                if (signature.ReturnType.Equals(returnType.getType()))
				{
					list.Add(signature);
				}
			}
			return list;
		}
	}

}