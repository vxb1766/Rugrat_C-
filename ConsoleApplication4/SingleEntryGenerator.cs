using System;
using System.Collections.Generic;
using System.Text;

namespace edu.uta.cse.proggen.packageLevelElements
{

	using ConfigurationXMLParser = edu.uta.cse.proggen.configurationParser.ConfigurationXMLParser;
	using ProgGenUtil = edu.uta.cse.proggen.util.ProgGenUtil;

	public class SingleEntryGenerator
	{

		private string program = "";
		private List<ClassGenerator> listOfClasses;
		private int noOfMethCalls = 0;
		private readonly double methCallLimit = 150;

		public SingleEntryGenerator(List<ClassGenerator> list)
		{
			listOfClasses = list;
			double totalClass = listOfClasses.Count;
			double temp = Math.Ceiling(totalClass / methCallLimit);
			noOfMethCalls = (int) temp;

			generateSingleEntryClass();
		}


		private void appendPackageName()
		{
			//program += "package com.accenture.lab.carfast.test;\n\n\n";
            //Srujana: using System import necessary for all C# programs
            program += "using System;\n\n\n";
            //Srujana: C# packages are called namespaces
            program += "namespace com.accenture.lab.carfast.test{\n\n\n";
		}
		private void appendClassName()
		{
			program += "public class " + ConfigurationXMLParser.getProperty("classNamePrefix") + "Start {\n";
		}

		private void appendFieldNames()
		{
			for (int i = 0; i < ProgGenUtil.maxNoOfParameters; i++)
			{
				program += "private static int f" + i + ";\n";
			}
			program += "\n\n";
		}

		private void appendSubMethod()
		{

			StringBuilder fields = new StringBuilder();
			for (int i = 0; i < ProgGenUtil.maxNoOfParameters; i++)
			{
				fields.Append("f" + i + ",");
			}
			string f = fields.ToString();
			f = f.Substring(0, fields.Length - 1);

	//		for(ClassGenerator cls : listOfClasses){
	//			program += cls.getFileName()+"." + "singleEntry(" + f + ");\n";
	//		}	
			int indx = 0;
			int totalClass = listOfClasses.Count;
			for (int i = 0; i < noOfMethCalls; i++)
			{
				program += "private static void subEntryMethod" + i + "(){\n";
				for (int k = 0; (k < methCallLimit && indx < totalClass); indx++, k++)
				{
					program += listOfClasses[indx].FileName + "." + "singleEntry(" + f + ");\n";
				}
				program += "}\n\n";
			}

			program += "\n\n";

		}

		private void appendEntryMethod()
		{
			program += "public static void entryMethod(";

			StringBuilder param = new StringBuilder();
			for (int i = 0; i < ProgGenUtil.maxNoOfParameters; i++)
			{
				param.Append("int i" + i + ",");
			}

			string st = param.ToString();
			st = st.Substring(0, st.Length - 1);
			st += "){\n";

			program += st;

			for (int i = 0; i < ProgGenUtil.maxNoOfParameters; i++)
			{
				program += "f" + i + " = " + "i" + i + ";\n";
			}

			program += "\n\n";

       
			for (int i = 0; i < noOfMethCalls; i++)
			{
				program += "subEntryMethod" + i + "();\n";
			}

			program += "\n}\n";
		}

		private void endClass()
		{
			program += "}";
		}

		private void appendMainMethod()
		{
			program += "public static void main(String[] args){\n entryMethod(";

			StringBuilder str = new StringBuilder();
			for (int i = 0; i < ProgGenUtil.maxNoOfParameters; i++)
			{
				str.Append("Integer.parseInt(args[" + i + "]),");
			}
			string s = str.ToString();
			s = s.Substring(0, str.Length - 1);
			s += ");\n}";

			program += s;

		}

		public override string ToString()
		{
			return program;
		}

		public virtual void generateSingleEntryClass()
		{
			appendPackageName();
			appendClassName();
			appendFieldNames();
			appendSubMethod();
			appendEntryMethod();
			appendMainMethod();
			endClass();

		}


	}

}