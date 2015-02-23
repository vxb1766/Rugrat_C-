using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using edu.uta.cse.proggen;




namespace edu.uta.cse.proggen.start.Start
{
    using Method = edu.uta.cse.proggen.classLevelElements.Method;
    using ConfigurationXMLParser = edu.uta.cse.proggen.configurationParser.ConfigurationXMLParser;
    using SingleEntryGenerator = edu.uta.cse.proggen.packageLevelElements.SingleEntryGenerator;
    using ProgGenUtil = edu.uta.cse.proggen.util.ProgGenUtil;
    using ClassGenerator = edu.uta.cse.proggen.packageLevelElements.ClassGenerator;
    using TreeOfSingleEntryGenerator = edu.uta.cse.proggen.packageLevelElements.TreeOfSingleEntryGenerator;
    using InterfaceGenerator = edu.uta.cse.proggen.packageLevelElements.InterfaceGenerator;
    using DBUtilGenerator = edu.uta.cse.proggen.packageLevelElements.DBUtilGenerator;

	/// <summary>
	/// Starting point of the ProgGen tool.
	/// 
	/// @author Ishtiaque_Hussain
	/// 
	/// </summary>
	public class Start
	{

		private static string pathToDir = "";

		public static string PathToDir
		{
			get
			{
				return pathToDir;
			}
		}

        public static void Main(string[] args)
        {
            /* For ant script: specify 'config.xml' file and output directory */

            if (args.Length == 1)
            {
                pathToDir = args[0] + System.IO.Path.PathSeparator;
            }


            /* List of generated class objects: ClassGenerators */
            List<ClassGenerator> list = new List<ClassGenerator>();
            //	List<InterfaceGenerator> interfaceList = new List<InterfaceGenerator>();
            int numberOfClasses = 0;
            int maxInheritanceDepth = 0;
            int noOfInheritanceChains = 0;
            int noOfInterfaces = 0;
            int maxInterfacesToImplement = 0;

            /* Set of generated classes, it's updated in ClassGenerator.generate() */
            HashSet<string> generatedClasses = new HashSet<string>();
            HashSet<string> preGeneratedClasses = new HashSet<string>();

            try
            {
                string className = ConfigurationXMLParser.getProperty("classNamePrefix");
                int totalLoc = ConfigurationXMLParser.getPropertyAsInt("totalLOC");

                numberOfClasses = ConfigurationXMLParser.getPropertyAsInt("noOfClasses");
                maxInheritanceDepth = ConfigurationXMLParser.getPropertyAsInt("maxInheritanceDepth"); // e.g. 3
                noOfInheritanceChains = ConfigurationXMLParser.getPropertyAsInt("noOfInheritanceChains"); // 2 => "A-B-C" ; "E-F-G"
                noOfInterfaces = ConfigurationXMLParser.getPropertyAsInt("noOfInterfaces");
                maxInterfacesToImplement = ConfigurationXMLParser.getPropertyAsInt("maxInterfacesToImplement");

                if (numberOfClasses < (maxInheritanceDepth * noOfInheritanceChains))
                {
                    Console.WriteLine("Insufficent number of classes. Should be atleast: " + maxInheritanceDepth * noOfInheritanceChains);
                    Environment.Exit(1);
                }

                HashSet<string> classList = new HashSet<string>();

                for (int i = 0; i < numberOfClasses; i++)
                {

                    classList.Add(className + i);
                }
                //E.g., {[2,5,6], [0,1,4]}
                List<List<int>> inheritanceHierarchies = new List<List<int>>();

                //inheritanceHierarchies = ProgGenUtil.getInheritanceList(noOfInheritanceChains, maxInheritanceDepth, numberOfClasses);

                for (int i = 0; i < numberOfClasses; i++)
                {
                    //Ishtiaque: All classes have equal number of variables, methods, etc. Should we change it?	
                    // classes are like A1, A2, etc where A=<UserDefinedName> 
                    //Bala: All such cases are handled in the ClassGenerator. It generates arbitrary number of
                    // fields, methods. Only constraint is it should override all the methods of the interfaces
                    // it implements.

                    ClassGenerator test = new ClassGenerator(className + i, totalLoc / numberOfClasses, null);
                    list.Add(test);

                }
                string path = @"C:\Users\VeenaBalasubramanya\Desktop\Adv_Se\rugrat\TestPrograms\com\accenture\lab\carfast\test";
                //Directory directory = Directory.CreateDirectory(path);
                DirectoryInfo directory = Directory.CreateDirectory(path);
                //System.IO.FileStream fs = System.IO.File.Create(pathString);


                if (!directory.Exists)
                {
                    //Console.WriteLine(directory.mkdirs());
                    directory = System.IO.Directory.CreateDirectory(path);
                    Console.WriteLine(directory);
                }
                /*
                                    for (int i = 0; i < noOfInterfaces; i++)
                                    {
                                        InterfaceGenerator generator = new InterfaceGenerator(className + "Interface" + i, list);
                                        interfaceList.Add(generator);
                                        writeToFile(generator);
                                    }

                                    //establishClassRelationships(inheritanceHierarchies, list);

                                    //establishClassInterfaceRelationships(interfaceList, list);
                 */
            }
            catch (System.FormatException e)
            {
                Console.WriteLine("Please enter integer values for arguments that expect integers!!!");
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
                Environment.Exit(1);
            }

            //do pre-generation for classes
            //pre-generation determines the class members variables and method signatures
            foreach (ClassGenerator generator in list)
            {
                generator.preGenerateForMethodSignature(list, preGeneratedClasses);
            }

            foreach (ClassGenerator generator in list)
            {
                //Ishtiaque: How can 'generatedClasses' contain any of the ClassGenerator objects from the list? 
                //Bala: classes are generated semi-recursively. The classes will invoke class generation on the
                //super class. The current class will be generated only after all its ancestor classes are generated.
                //We do not want to regenerate the ancestor classses and make stale the information used by its sub-classes
                //based on the earlier version.
                if (!generatedClasses.Contains(generator.FileName))
                {
                    //call generate to construct the class contents
                    generator.generate(list, generatedClasses, preGeneratedClasses);
                }
                writeToFile(generator);
            }

            //generate DBUtil only if useQueries is TRUE
            //if (ProgGenUtil.useQueries)
            //{
            //    DBUtilGenerator dbUtilGenerator = new DBUtilGenerator();
            //    writeToFile(dbUtilGenerator);
            //}

            /* writing SingleEntry class */

            //SingleEntryGenerator singleEntryGen = new SingleEntryGenerator(list);
            //String className = ConfigurationXMLParser.getProperty("classNamePrefix")+"Start";
            //write(className, singleEntryGen.toString());

            TreeOfSingleEntryGenerator treeSingleEntryGen = new TreeOfSingleEntryGenerator(list, pathToDir);
            treeSingleEntryGen.generateTreeOfSingleEntryClass();

            //write the reachability matrix

            if (ConfigurationXMLParser.getProperty("doReachabilityMatrix").Equals("no"))
            {
                return;
            }


            List<Method> methodListAll = new List<Method>();
            foreach (ClassGenerator generator in list)
            {
                methodListAll.AddRange(generator.MethodList);
            }

            StringBuilder builder = new StringBuilder();
            builder.Append("Name, ");

            foreach (Method method in methodListAll)
            {
                builder.Append(method.AssociatedClass.FileName + "." + method.Name);
                builder.Append(", ");
            }
            builder.Append("\n");

            foreach (Method method in methodListAll)
            {
                builder.Append(method.AssociatedClass.FileName + "." + method.Name);
                builder.Append(", ");
                foreach (Method calledMethod in methodListAll)
                {

                    if (method.CalledMethodsWithClassName.Contains(calledMethod.AssociatedClass.FileName + "." + calledMethod.Name))
                    {
                        builder.Append("1, ");
                    }
                    else
                    {
                        builder.Append("0, ");
                    }
                }
                builder.Append("\n");
            }
            writeReachabilityMatrix(builder.ToString());
        }

		private static void writeReachabilityMatrix(string matrix)
		{
			System.IO.FileStream fos = null;
            StreamWriter outstream = new StreamWriter("C:\\log.txt", true) ;

			try
			{
				System.IO.File.WriteAllText(@"C:\Users\VeenaBalasubramanya\Desktop\Adv_Se\rugrat\TestPrograms\com\accenture\lab\carfast\test\ReachabilityMatrix.csv", matrix);
				Console.WriteLine("Writing reachability matrix...");
               	outstream.WriteLine(matrix.GetBytes());
			}
			catch (Exception e)
			{
				Console.WriteLine("Error writing reachability matrix!");
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			finally
			{
				try
				{
					if (outstream != null)
					{
                        outstream.Flush();
						outstream.Close();
					}
				}
				catch (IOException e)
				{
					Console.WriteLine("Error closing output filestream");
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
					Environment.Exit(1);
				}
			}
		}

		private static void writeToFile(DBUtilGenerator generator)
		{
			if (generator == null)
			{
				Console.WriteLine("DBUtil generator null");
				return;
			}
			write("DBUtil", generator.ToString());
		}

		private static void writeToFile(ClassGenerator generator)
		{
			if (generator == null)
			{
				return;
			}
			string filename = generator.FileName;
			write(filename, generator.ToString());
		}

		private static void writeToFile(InterfaceGenerator generator)
		{
			if (generator == null)
			{
				return;
			}
			write(generator.Name, generator.ToString());
		}

		private static void write(string filename, string contents)
		{
			System.IO.FileStream fos = null;
            String path = @"C:\Users\VeenaBalasubramanya\Desktop\Adv_Se\rugrat\TestPrograms\com\accenture\lab\carfast\test\"+filename;
            

			try
			{
//                fos = new System.IO.FileStream(pathToDir + "TestPrograms" + Path.PathSeparator + "com" + Path.PathSeparator + "accenture" + Path.PathSeparator + "lab" + Path.PathSeparator + "carfast" + Path.PathSeparator + "test" + Path.PathSeparator + filename + ".java", System.IO.FileMode.Create, System.IO.FileAccess.Write);

	//			outstream = new BufferedOutputStream(fos);
                
                
                System.IO.File.WriteAllText(path, contents);

				// To let the user know RUGRAT is working...
                Console.WriteLine("I am from Rugrat c#");
				Console.WriteLine("Writing to file...." + filename);
				
				// Successfully written
				Console.WriteLine("Successfully written.");
			}
			catch (FileNotFoundException e)
			{
				Console.WriteLine("Error writing out class to .java file : " + filename);
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Environment.Exit(1);
			}
			catch (IOException e)
			{
				Console.WriteLine("Error writing out class to .java file : " + filename);
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Environment.Exit(1);
			}
			finally
			{
				try
				{
						/*if (outstream != null)
						{
							outstream.flush();
							outstream.close();
						}*/
				}
					catch (IOException e)
					{
					Console.WriteLine("Error closing output filestream");
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
					Environment.Exit(1);
					}
			}
		}

		private static void establishClassInterfaceRelationships(List<InterfaceGenerator> interfaceList, List<ClassGenerator> list)
		{
			int numberOfInterfaces = interfaceList.Count;
			int maxInterfacesToImplement = ConfigurationXMLParser.getPropertyAsInt("maxInterfacesToImplement");
			if (numberOfInterfaces == 0)
			{
				Console.WriteLine("No interfaces generated!");
				return;
			}

			foreach (ClassGenerator generator in list)
			{
				List<InterfaceGenerator> generatorInterfaceList = generator.InterfaceList;
				Random random = new Random();

				//A class can implement 0 or more interfaces.
				int numberOfInterfacesToImplement = random.Next(maxInterfacesToImplement);

				for (int j = 0; j < numberOfInterfacesToImplement; j++)
				{
					InterfaceGenerator interfaceGenerator = interfaceList[random.Next(interfaceList.Count)];
					if (generatorInterfaceList.Contains(interfaceGenerator))
					{
						continue;
					}
					generatorInterfaceList.Add(interfaceGenerator);
				}
			}
		}

		/*
		 * First class has no superclass.
		 * E.g., A-B-C
		 * A has no superclass. But A is superclass of B,...
		 */

		private static void establishClassRelationships(List<List<int?>> inheritanceHierarchies, List<ClassGenerator> classes) // E.g., <{2,3,7}, {4, 5, 1},...>
		{
			foreach (List<int?> hierarchy in inheritanceHierarchies)
			{
				int superClassIndex = -1;
				foreach (int index in hierarchy)
				{
					if (superClassIndex == -1)
					{
						superClassIndex = index;
						continue;
					}

					classes[index].SuperClass = classes[superClassIndex];
					superClassIndex = index;
				}
			}
		}
	}

}