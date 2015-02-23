using System;
using System.Collections.Generic;
using System.Text;

namespace edu.uta.cse.proggen.packageLevelElements
{


	using ConfigurationXMLParser = edu.uta.cse.proggen.configurationParser.ConfigurationXMLParser;
	using ProgGenUtil = edu.uta.cse.proggen.util.ProgGenUtil;

	/// <summary>
	/// Creates Single entry classes in a tree structure.
	/// For X number of generated classes, it will create Y number of SingleEntry class
	/// such that Y = X/150. That is, each of Y classes will call 150 SingleEntry methods
	/// of generated classes; this is the leaf level.
	/// 
	/// Then at the higher level, Z = Y/150, number of classes will call 150 each.
	/// 
	///  So, the tree depth is: Log150(X) =  Log(X) / Log(150);
	/// 
	/// @author Ishtiaque Hussain {ishtiaque.hussain@mavs.uta.edu}
	/// 
	/// </summary>

	public class TreeOfSingleEntryGenerator
	{

		private List<ClassGenerator> listOfClasses;
		private readonly double methCallLimit = 150;
		private readonly string DirPath;
		private readonly string formalParam;
		private readonly string argument;

		private int LEVEL;

		public TreeOfSingleEntryGenerator(List<ClassGenerator> list, string pathToDir)
		{
			listOfClasses = list;
			DirPath = pathToDir;
			LEVEL = (int) Math.Ceiling(Math.Log10(listOfClasses.Count) / Math.Log10(methCallLimit));
			formalParam = formalParamBuilder();
			argument = argBuilder();
		}


		/// <summary>
		///  Tree of entry Classes	  
		/// </summary>
		public virtual void generateTreeOfSingleEntryClass()
		{
			Console.WriteLine("Writing SingleEntry classes...");
			createClass(0, listOfClasses.Count);
			Console.WriteLine("Successfully written!!");
		}

		/// <summary>
		/// Creates String of {int i0, int i1, int i2, ...} </summary>
		/// <returns> int i0, int i1, int i2, int i3, ... </returns>
		private string formalParamBuilder()
		{

			StringBuilder param = new StringBuilder();
			for (int j = 0; j < ProgGenUtil.maxNoOfParameters; j++)
			{
				param.Append("int i" + j + ",");
			}

			string st = param.ToString();
			st = st.Substring(0, st.Length - 1);

			return st;
		}

		/// 
		/// <returns> argument of a method, e.g., f1, f2, f3, f4, f5, f6 </returns>

		private string argBuilder()
		{
			StringBuilder fields = new StringBuilder();
			for (int j = 0; j < ProgGenUtil.maxNoOfParameters; j++)
			{
				fields.Append("f" + j + ",");
			}
			string f = fields.ToString();
			f = f.Substring(0, fields.Length - 1);

			return f;

		}

		private void createClass(int level, int target)
		{
			if (level == 0)
			{

				/*
				 * create 'loop' number of classes where each class calls 150 or
				 * less T.SingleEntry() methods in total 'target' number of
				 * T.SingleEntry() methods name these classes TStart_Llevel_X
				 */


				int count = 0;
				int loop = (int) Math.Ceiling((double) target / 150);

				for (int i = 0; i < loop; i++)
				{

					try
					{
						File file = new File(DirPath + "TestPrograms" + File.separator + "com" + File.separator + "accenture" + File.separator + "lab" + File.separator + "carfast" + File.separator + "test" + File.separator + "TStart_L" + level + "_" + i + ".java");


						System.IO.StreamWriter fileWriter = new System.IO.StreamWriter(file);
						System.IO.StreamWriter writer = new System.IO.StreamWriter(fileWriter);

						StringBuilder output = new StringBuilder();

						output.Append("package com.accenture.lab.carfast.test;\n\n");
						output.Append("public class TStart_L" + level + "_" + i + "{\n");
						for (int k = 0; k < ProgGenUtil.maxNoOfParameters; k++)
						{
							output.Append("private static int f" + k + ";\n");
						}

						output.Append("\n\n");
						output.Append("public static void entryMethod(");
						//int i0, int i1, int i2, int i3, int i4, int i5, int i6){\n");					
						output.Append(formalParam + "){\n");

						for (int k = 0; k < ProgGenUtil.maxNoOfParameters; k++)
						{
							output.Append("f" + k + " = " + "i" + k + ";\n");
						}


						for (int j = 0; j < methCallLimit && count < target; j++, count++)
						{
							// call Tj.SingleEntry();
	//						output.append("FiveMLOC"+count+".singleEntry(f0,f1,f2,f3,f4,f5,f6);\n");
							output.Append(ConfigurationXMLParser.getProperty("classNamePrefix") + count + ".singleEntry(" + argument + ");\n");
						}

						output.Append("}\n}");

						string @out = output.ToString();
	//					System.out.println("Writing L0 level entry classes.");

						writer.WriteByte(@out);
						writer.Close();


					}
					catch (Exception e)
					{
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}
				}

				createClass(level + 1, loop);

			}
			else
			{
				if (level == LEVEL)
				{
					// create FiveMLOCStart.java class that will call
					// FiveMLOCStart_L(prevLevel)_0.entryMethod();				
					try
					{
						File file = new File(DirPath + "TestPrograms" + File.separator + "com" + File.separator + "accenture" + File.separator + "lab" + File.separator + "carfast" + File.separator + "test" + File.separator + ConfigurationXMLParser.getProperty("classNamePrefix") + "Start" + ".java");

	//					File file = new File("./FiveMLOCStart.java");
						System.IO.StreamWriter fileWriter = new System.IO.StreamWriter(file);
						System.IO.StreamWriter writer = new System.IO.StreamWriter(fileWriter);

						StringBuilder output = new StringBuilder();

						output.Append("package com.accenture.lab.carfast.test;\n\n");
	//					output.append("public class FiveMLOCStart {\n");
						output.Append("public class " + ConfigurationXMLParser.getProperty("classNamePrefix") + "Start {\n");
						for (int k = 0; k < ProgGenUtil.maxNoOfParameters; k++)
						{
							output.Append("private static int f" + k + ";\n");
						}

						output.Append("\n\n");

						output.Append("public static void entryMethod(");
						//int i0, int i1, int i2, int i3, int i4, int i5, int i6){\n");


						output.Append(formalParam + "){\n");


						for (int k = 0; k < ProgGenUtil.maxNoOfParameters; k++)
						{
							output.Append("f" + k + " = " + "i" + k + ";\n");
						}

						output.Append("TStart_L" + (level - 1) + "_0.entryMethod(" + argument + ");\n}\n\n");

						output.Append("public static void main(String[] args){\n entryMethod(");

						StringBuilder str = new StringBuilder();
						for (int i = 0; i < ProgGenUtil.maxNoOfParameters; i++)
						{
							str.Append("Integer.parseInt(args[" + i + "]),");
						}
						string s = str.ToString();
						s = s.Substring(0, str.Length - 1);
						s += ");\n}";

						output.Append(s);

						output.Append("\n}");

						string @out = output.ToString();
	//					System.out.println("Writing the 'Start' class.");

						writer.WriteByte(@out);
						writer.Close();

					}
					catch (Exception e)
					{
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}

					return;

				}
				else
				{

					int count = 0;
					int loop = (int) Math.Ceiling((double) target / 150);

					for (int i = 0; i < loop; i++)
					{
						/*
						 * create 'loop' number of classes,where each calls 150 or
						 * less TStart_L(prevlevel)_X.entryMethod() methods; in
						 * total 'target' number of calls.
						 * 
						 * Name each class TStart_Llevel_X
						 */

						try
						{

							File file = new File(DirPath + "TestPrograms" + File.separator + "com" + File.separator + "accenture" + File.separator + "lab" + File.separator + "carfast" + File.separator + "test" + File.separator + "TStart_L" + level + "_" + i + ".java");

	//						File file = new File("./TStart_L"+level+"_"+i+".java");
							System.IO.StreamWriter fileWriter = new System.IO.StreamWriter(file);
							System.IO.StreamWriter writer = new System.IO.StreamWriter(fileWriter);

							StringBuilder output = new StringBuilder();

							output.Append("package com.accenture.lab.carfast.test;\n\n");
							output.Append("public class TStart_L" + level + "_" + i + "{\n");
							for (int k = 0; k < ProgGenUtil.maxNoOfParameters; k++)
							{
								output.Append("private static int f" + k + ";\n");
							}

							output.Append("\n\n");

							output.Append("public static void entryMethod(");
							//int i0, int i1, int i2, int i3, int i4, int i5, int i6){\n");						

							output.Append(formalParam + "){\n");

							for (int k = 0; k < ProgGenUtil.maxNoOfParameters; k++)
							{
								output.Append("f" + k + " = " + "i" + k + ";\n");
							}

							for (int j = 0; j < methCallLimit && count < target; j++, count++)
							{
								// Call TStart_L(prevlevel)_X.entryMethod()
								output.Append("TStart_L" + (level - 1) + "_" + count + ".entryMethod(" + argument + ");\n");
							}

							output.Append("}\n}");


							string @out = output.ToString();
	//						System.out.println("Writing mid level classes.");

							writer.WriteByte(@out);
							writer.Close();

						}
						catch (Exception e)
						{
							Console.WriteLine(e.ToString());
							Console.Write(e.StackTrace);
						}
					}

					createClass(level + 1, loop);

				}
			}
		}

	}

}