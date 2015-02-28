using System;
using System.Collections.Generic;
using System.Text;

namespace edu.uta.cse.proggen.namespaceLevelElements
{


	using Method = edu.uta.cse.proggen.classLevelElements.Method;
	using MethodSignature = edu.uta.cse.proggen.classLevelElements.MethodSignature;
	using ConfigurationXMLParser = edu.uta.cse.proggen.configurationParser.ConfigurationXMLParser;
	using ProgGenUtil = edu.uta.cse.proggen.util.ProgGenUtil;


	/// <summary>
	/// Class to generate an interface.
	/// 
	/// Note: No inheritance among interfaces.
	/// 
    /// @author Team 6 - CSE6324 - Spring 2015
	/// 
	/// </summary>
	public class InterfaceGenerator
	{
		private List<MethodSignature> methodSignatures = new List<MethodSignature>();
		private string name;
		private int numOfMethods;

		public InterfaceGenerator(string name, List<ClassGenerator> classList)
		{
			this.name = name;

			//we need atleast one method per interface. not a marker interface.
			int maxNumOfMethods = ConfigurationXMLParser.getPropertyAsInt("maxNoOfMethodsPerInterface");
			int numOfMethods;
			if (maxNumOfMethods < 1)
			{
				numOfMethods = 1;
			}
			else
			{
				numOfMethods = (new Random()).Next(maxNumOfMethods);
			}

			if (numOfMethods == 0)
			{
				numOfMethods = 1;
			}
			this.numOfMethods = numOfMethods;

			for (int i = 0; i < numOfMethods; i++)
			{
				//use this method only to extract the signature and discard them
				Method method = Method.generateMethodForInterface(ProgGenUtil.maxNoOfParameters, classList, name + "Method" + i);
				methodSignatures.Add(method.MethodSignature);
			}
		}

		public virtual List<MethodSignature> MethodSignatures
		{
			get
			{
				return methodSignatures;
			}
		}

		public virtual int NumOfMethods
		{
			get
			{
				return numOfMethods;
			}
		}

		public virtual string Name
		{
			get
			{
				return name;
			}
		}

		public virtual string NamespaceName
		{
			get
			{
				return "namespace com.accenture.lab.carfast.test;\n\n\n";
			}
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder(NamespaceName);
			builder.Append("public interface");
			builder.Append(" ");
			builder.Append(name);
			builder.Append("\n{\n");
			foreach (MethodSignature signature in methodSignatures)
			{
				builder.Append(signature);
				builder.Append(";\n");
			}
			builder.Append("}");
			return builder.ToString();
		}
	}

}