using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace edu.uta.cse.proggen.util
{


    using Field = edu.uta.cse.proggen.classLevelElements.Field;
    using Method = edu.uta.cse.proggen.classLevelElements.Method;
    using MethodSignature = edu.uta.cse.proggen.classLevelElements.MethodSignature;
    using Type = edu.uta.cse.proggen.classLevelElements.Type;
    using Variable = edu.uta.cse.proggen.classLevelElements.Variable;
    using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;
    using ConfigurationXMLParser = edu.uta.cse.proggen.configurationParser.ConfigurationXMLParser;
    using FieldGenerator = edu.uta.cse.proggen.expressions.FieldGenerator;
    using Literal = edu.uta.cse.proggen.expressions.Literal;
    using VariableGenerator = edu.uta.cse.proggen.expressions.VariableGenerator;
    using Operand = edu.uta.cse.proggen.nodes.Operand;
    using ClassGenerator = edu.uta.cse.proggen.packageLevelElements.ClassGenerator;
    using PrintStatement = edu.uta.cse.proggen.statements.PrintStatement;

    /// <summary>
    /// Class containing utility APIs.
    /// 
    /// @author balamurugan
    /// 
    /// </summary>
    public class ProgGenUtil
    {
        public static readonly int maxNoOfParameters = ConfigurationXMLParser.getPropertyAsInt("maxNoOfParametersPerMethod");
        public static readonly int minNoOfParameters = ConfigurationXMLParser.getPropertyAsInt("minNoOfParametersPerMethod");
        public static readonly int maxNoOfMethodsPerClass = ConfigurationXMLParser.getPropertyAsInt("maxNoOfMethodsPerClass");
        public static readonly int maxNoOfMethodCalls = ConfigurationXMLParser.getPropertyAsInt("maxAllowedMethodCalls");
        public static readonly int maxRecursionDepth = ConfigurationXMLParser.getPropertyAsInt("maxRecursionDepth");
        public static readonly int recursionProbability = ConfigurationXMLParser.getPropertyAsInt("recursionProbability");
        public static readonly int integerMaxValue = ConfigurationXMLParser.getPropertyAsInt("intMaxValue");

        //minInheritanceDepth
        public static readonly int minInheritanceDepth = ConfigurationXMLParser.getPropertyAsInt("minInheritanceDepth");

        public static readonly List<string> allowedTypes;
        public static readonly Dictionary<string, Type.Primitives> primitivesMap = new Dictionary<string, Type.Primitives>();
        public static readonly int maxLoopControllerValue = ConfigurationXMLParser.getPropertyAsInt("maxValueForLoop"); //1000;
        public static readonly string dbUserName = ConfigurationXMLParser.getProperty("dbUsername");
        public static readonly string password = ConfigurationXMLParser.getProperty("password");
        public static readonly string db = ConfigurationXMLParser.getProperty("dbName");
        public static readonly string driver = ConfigurationXMLParser.getProperty("dbDriver");
        public static readonly string dbUrl = "jdbc:mysql:///" + db;
        public static bool useQueries = Convert.ToBoolean(ConfigurationXMLParser.getProperty("useQueries"));

        //Using arrays as class field
        public static string allowArray = ConfigurationXMLParser.getProperty("allowArray");
        public enum CallType { localWithoutRecursionLimit, localWithRecursionLimit, crossClassWithoutRecursionLimit, crossClassWithRecursionLimit };

        /// <summary>
        /// Determines the call type for the method calls generated within
        /// a method body.
        /// 
        /// @author balamurugan
        /// </summary>
        /// 

        //Commented by veena
        //public sealed class CallType
        //{
        //    public static readonly CallType localWithoutRecursionLimit, localWithRecursionLimit, crossClassWithoutRecursionLimit, crossClassWithRecursionLimit = new CallType("localWithoutRecursionLimit, localWithRecursionLimit, crossClassWithoutRecursionLimit, crossClassWithRecursionLimit", InnerEnum.localWithoutRecursionLimit, localWithRecursionLimit, crossClassWithoutRecursionLimit, crossClassWithRecursionLimit);

        //    private static readonly IList<CallType> valueList = new List<CallType>();

        //    static CallType()
        //    {

        //        valueList.Add(localWithoutRecursionLimit, localWithRecursionLimit, crossClassWithoutRecursionLimit, crossClassWithRecursionLimit);
        //    }

        //    public enum InnerEnum
        //    {
        //        localWithoutRecursionLimit, localWithRecursionLimit, crossClassWithoutRecursionLimit, crossClassWithRecursionLimit
        //    }

        //    private readonly string nameValue;
        //    private readonly int ordinalValue;
        //    private readonly InnerEnum innerEnumValue;
        //    private static int nextOrdinal = 0;

        //    private CallType(string name, InnerEnum innerEnum)
        //    {
        //        nameValue = name;
        //        ordinalValue = nextOrdinal++;
        //        innerEnumValue = innerEnum;
        //    }

        //    public static IList<CallType> values()
        //    {
        //        return valueList;
        //    }

        //    public InnerEnum InnerEnumValue()
        //    {
        //        return innerEnumValue;
        //    }

        //    public int ordinal()
        //    {
        //        return ordinalValue;
        //    }

        //    public override string ToString()
        //    {
        //        return nameValue;
        //    }

        //    public static CallType valueOf(string name)
        //    {
        //        foreach (CallType enumInstance in CallType.values())
        //        {
        //            if (enumInstance.nameValue == name)
        //            {
        //                return enumInstance;
        //            }
        //        }
        //        throw new System.ArgumentException(name);
        //    }
        //}
        public static CallType methodCallType;

        private static int maximumArraySize = 2;
        private static string injectContents = "";

        //read config information needed for code generation.
        static ProgGenUtil()
        {
            allowedTypes = AllowedTypesAsList;
            primitivesMap["char"] = Type.Primitives.CHAR;
            primitivesMap["byte"] = Type.Primitives.BYTE;
            primitivesMap["short"] = Type.Primitives.SHORT;
            primitivesMap["int"] = Type.Primitives.INT;
            primitivesMap["long"] = Type.Primitives.LONG;
            primitivesMap["float"] = Type.Primitives.FLOAT;
            primitivesMap["double"] = Type.Primitives.DOUBLE;
            primitivesMap["String"] = Type.Primitives.STRING;
            primitivesMap["Object"] = Type.Primitives.OBJECT;
            primitivesMap["Other"] = Type.Primitives.OTHER;
            readInjectContents();
            maximumArraySize = ConfigurationXMLParser.getPropertyAsInt("maximumArraySize");
            if (ConfigurationXMLParser.getProperty("useQueries").Equals("true", StringComparison.InvariantCultureIgnoreCase))
            {
                useQueries = true;
            }

            //if (ConfigurationXMLParser.getProperty("useQueries").equalsIgnoreCase("true"))
            //{
            //    useQueries = true;
            //}

            string callType = ConfigurationXMLParser.getProperty("callType");
            if (callType.Equals("MCO2_2", StringComparison.CurrentCultureIgnoreCase))
            {
                methodCallType = CallType.crossClassWithoutRecursionLimit;
            }
            else if (callType.Equals("MCO2_1", StringComparison.CurrentCultureIgnoreCase))
            {
                methodCallType = CallType.crossClassWithRecursionLimit;
            }
            else if (callType.Equals("MCO1_1", StringComparison.CurrentCultureIgnoreCase))
            {
                methodCallType = CallType.localWithRecursionLimit;
            }
            else
            {
                //Original: methodCallType = CallType.localWithoutRecursionLimit;
                //Veena : Changed it to 
                methodCallType = CallType.localWithRecursionLimit;
            }
        }

        /// <summary>
        /// returns the user-configured allowed types.
        /// 
        /// @return
        /// </summary>
        private static List<string> AllowedTypesAsList
        {
            get
            {
                List<string> allowedTypesList = new List<string>();
                HashSet<string> allowedTypes = ConfigurationXMLParser.TypeList;
                object[] array = allowedTypes.ToArray();
                foreach (object o in array)
                {
                    allowedTypesList.Add((string)o);
                }
                return allowedTypesList;
            }
        }

        /// <summary>
        /// read the contents to be injected into every generated class.
        /// </summary>
        private static void readInjectContents()
        {
            string injectFileName = ConfigurationXMLParser.getProperty("injectFilename");

            if (!File.Exists(injectFileName))
            {
                Console.WriteLine("Unable to locate Inject File. Skipping content injection!");
            }

            //Veena : Unreachable code. I tested it twice in eclipse. it never comes here.
            // Hence commenting the try block
            try
            {
                //StringBuilder buffer = new StringBuilder();
                //System.IO.StreamReader reader = new System.IO.StreamReader(injectFileName);
                //System.IO.StreamReader buffReader = new System.IO.StreamReader(reader);
                //string str = "";
                //while ((str = buffReader.ReadLine()) != null)
                //{
                //    buffer.Append(str);
                //    buffer.Append("\n");
                //}
                //injectContents = buffer.ToString();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Unable to locate Inject File. Skipping content injection!");
                injectContents = "";
            }
            catch (IOException)
            {
                Console.WriteLine("Unable to read Inject File. Skipping content injection!");
                injectContents = "";
            }
        }

        public static string InjectContents
        {
            get
            {
                return injectContents;
            }
        }

        public static List<string> AllowedTypes
        {
            get
            {
                return allowedTypes;
            }
        }

        public static int RandomArraySize
        {
            get
            {
                if (maximumArraySize < 2)
                {
                    Console.WriteLine("Array should be atleast of size 2! Config.xml has value: " + maximumArraySize);
                    Console.WriteLine("Setting array size to 2");
                    return 2;
                }
                else if (maximumArraySize == 2)
                {
                    return maximumArraySize;
                }
                //array size should be atleast 2
                return (new Random()).Next(maximumArraySize - 2) + 2;
            }
        }

        public static Type.Primitives RandomizedPrimitive
        {
            get
            {

                List<string> typeList = AllowedTypes;
                if (typeList.Count == 0)
                {
                    Console.WriteLine("No type specified in config.xml!");
                    Environment.Exit(1);
                }

                string primitiveString = typeList[(new Random()).Next(typeList.Count)];

                return primitivesMap[primitiveString];
            }

        }



        public static int getRandomIntInRange(int range)
        {
            return (new Random()).Next(range);
        }

        /// <summary>
        /// method to return a boolean value based on a coin flip.
        /// 
        /// @return
        /// </summary>
        public static bool coinFlip()
        {
            if (((new Random()).Next()) % 2 == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Method to reverse lookup a class by name in a given list.
        /// </summary>
        /// <param name="classList"> </param>
        /// <param name="classname">
        /// @return </param>
        private static ClassGenerator getClassByName(List<ClassGenerator> classList, string classname)
        {
            foreach (ClassGenerator classGenerator in classList)
            {
                if (classGenerator.FileName.Equals(classname))
                {
                    return classGenerator;
                }
            }
            return null;
        }

        /// <summary>
        /// Method to fetch fields which are of OBJECT primitive type.
        /// </summary>
        /// <param name="variableList">
        /// @return </param>
        //JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
        //ORIGINAL LINE: private static java.util.ArrayList<? extends edu.uta.cse.proggen.classLevelElements.Field> getObjects(java.util.ArrayList<? extends edu.uta.cse.proggen.classLevelElements.Field> variableList)
        //JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
        //ORIGINAL LINE: private static java.util.ArrayList<? extends edu.uta.cse.proggen.classLevelElements.Field> getObjects(java.util.ArrayList<? extends edu.uta.cse.proggen.classLevelElements.Field> variableList)
        //static void Cat<T> (IList<T> sources)



        //Veena : Again unreachable Code. 
        // THis is if Objects are being created. Since we are not doing inheritence/Objects, it's Unreacheble.
        //So dont waste time converting it. Get a Life :P
        //private static List<T> getObjects(List<T> variableList) where ? : edu.uta.cse.proggen.classLevelElements.Field where T1 : edu.uta.cse.proggen.classLevelElements.Field
        //{
        //    List<Field> objList = new List<Field>();

        //    foreach (Field @var in variableList)
        //    {
        //        if (@var.Type.Type == Type.Primitives.OBJECT)
        //        {
        //            objList.Add(@var);
        //        }
        //    }
        //    return objList;
        //}

        private static string getParametersForList(List<Variable> parameterList, Method method)
        {
            string parameters = "";
            foreach (Variable @var in parameterList)
            {
                if (@var.Name.Equals("recursionCounter"))
                {
                    parameters += "recursionCounter,";
                    continue;
                }

                Operand operand;
                Type.Primitives primitive = @var.Type.getType();//.Type.Type;
                int optionVariableOrField = (new Random()).Next(1);
                if (optionVariableOrField == 0)
                {

                    operand = VariableGenerator.getRandomizedVariable(method, primitive);

                }
                else
                {

                    operand = FieldGenerator.getRandomField(method.AssociatedClass, primitive, method.Static);

                }
                parameters += operand + ",";
            }
            parameters = parameters.Substring(0, parameters.Length - 1);
            return parameters;
        }

        private static MethodSignature getMethodToBeInvoked(List<MethodSignature> methodList, bool isStatic, Type returnType, MethodSignature callingMethod)
        {
            List<MethodSignature> list = new List<MethodSignature>();


            foreach (MethodSignature methodSignature in methodList)
            {
                if (methodSignature.ReturnType.Equals(returnType.getType()) && !methodSignature.Equals(callingMethod))
                {
                    list.Add(methodSignature);
                }
            }

            if (isStatic)
            {
                List<MethodSignature> staticSignatures = new List<MethodSignature>();
                foreach (MethodSignature methodSignature in list)
                {
                    if (methodSignature.Static)
                    {
                        staticSignatures.Add(methodSignature);
                    }
                }

                if (staticSignatures.Count == 0)
                {
                    return null;
                }
                return staticSignatures[(new Random()).Next(staticSignatures.Count)];
            }

            if (list.Count == 0)
            {
                return null;
            }

            return list[(new Random()).Next(list.Count)];
        }

        private static MethodSignature getMethodToBeInvoked(List<MethodSignature> methodList, bool isStatic, MethodSignature callingMethod)
        {
            if (!isStatic)
            {
                MethodSignature methodSignature = methodList[(new Random()).Next(methodList.Count)];
                int counter = 300;

                while (methodSignature.Equals(callingMethod) && counter > 0)
                {
                    methodSignature = methodList[(new Random()).Next(methodList.Count)];
                }

                if (counter > 0 && !methodSignature.Equals(callingMethod))
                {
                    return methodSignature;
                }
                return null;
            }
            else
            {
                List<MethodSignature> staticMethods = new List<MethodSignature>();
                foreach (MethodSignature method in methodList)
                {
                    if (method.Static && !method.Equals(callingMethod))
                    {
                        staticMethods.Add(method);
                    }
                }

                if (staticMethods.Count == 0)
                {
                    return null;
                }

                return staticMethods[(new Random()).Next(staticMethods.Count)];
            }
        }

        /// <summary>
        /// Return a method call statement based on a given primitive.
        /// </summary>
        /// <param name="method"> </param>
        /// <param name="classList"> </param>
        /// <param name="returnType"> </param>
        /// <param name="lhs">
        /// @return </param>
        public static string getMethodCallForReturnType(Method method, List<ClassGenerator> classList, Type returnType, Operand lhs)
        {
            string stmt = "";

            if (ProgGenUtil.methodCallType == CallType.localWithoutRecursionLimit || ProgGenUtil.methodCallType == CallType.localWithRecursionLimit)
            {
                //only local method calls.
                List<MethodSignature> methodList = method.AssociatedClass.MethodSignatures;
                if (methodList.Count < 1)
                {
                    return lhs + " = " + (new Literal(returnType.getType(), Int32.MaxValue)).ToString() + ";";
                }

                MethodSignature methodToBeInvoked = getMethodToBeInvoked(methodList, method.Static, returnType, method.MethodSignature);


                if (methodToBeInvoked == null)
                {
                    return lhs + " = " + (new Literal(returnType.getType(), Int32.MaxValue)).ToString() + ";";
                }

                //Check if indirect recursion is allowed:
                if (ConfigurationXMLParser.getProperty("allowIndirectRecursion").ToLower().Equals("no"))
                {
                    try
                    {
                        string[] tok = methodToBeInvoked.Name.ToLower().Split("method", true);
                        int calleeMethodID = int.Parse(tok[1]);

                        string[] tok2 = method.MethodSignature.Name.ToLower().Split("method", true);
                        int callerMethodID = int.Parse(tok2[1]);

                        if (callerMethodID >= calleeMethodID)
                        {
                            return lhs + " = " + (new Literal(returnType.getType(), Int32.MaxValue)).ToString() + ";";
                        }
                    }
                    catch (System.FormatException e)
                    {
                        Console.WriteLine(e.ToString());
                        Console.Write(e.StackTrace);
                    }
                }


                List<Variable> parameterList = methodToBeInvoked.ParameterList;
                string parameters = "(";
                parameters += getParametersForList(parameterList, method);
                parameters += ")";

                stmt += lhs + " = " + methodToBeInvoked.Name + parameters + ";";
                method.CalledMethods.Add(methodToBeInvoked);
                method.CalledMethodsWithClassName.Add(method.AssociatedClass.FileName + "." + methodToBeInvoked.Name);
                method.Loc = method.Loc + 1;
                return stmt;
            }
            else
            {
                //Veena : tested in eclipse. Never comes here Since we are not handling objects.
                //cross-class method calls.
                //Random random = new Random();
                //List<Field> objList = new List<Field>();

                //objList.AddRange(getObjects(method.AssociatedClass.Fields));
                //objList.AddRange(getObjects(method.ParameterList));

                //if (objList.Count != 0)
                //{
                //    ArrayList list = getClassByMethodReturnType(objList, returnType.getType(), classList);
                //    if (list == null)
                //    {
                //        return lhs + " = " + (new Literal(returnType.getType())).ToString() + ";";
                //    }

                //    Field variable = (Field)list[0];
                //    ClassGenerator classObj = (ClassGenerator)list[1];

                //    if (classObj != null)
                //    {
                //        List<MethodSignature> signatures = classObj.getMethodSignatures(returnType);
                //        if (signatures.Count == 0)
                //        {
                //            return lhs + " = " + (new Literal(returnType.getType())).ToString() + ";";
                //        }

                //        MethodSignature signature = signatures[random.Next(signatures.Count)];
                //        string varString = variable.ToString();
                //        string methodCall = signature.Name + "(" + getParametersForList(signature.ParameterList, method) + ");\n";

                //        if (!signature.Static)
                //        {
                //            if (method.Static && !variable.Static)
                //            {
                //                if (!variable.Name.StartsWith("var", StringComparison.Ordinal))
                //                {
                //                    stmt += "classObj.";
                //                }
                //            }
                //            stmt += varString + " = new " + variable.Type + "();\n";
                //        }

                //        stmt += lhs + " = ";

                //        if (method.Static && !variable.Static && !variable.Name.StartsWith("var", StringComparison.Ordinal) && !signature.Static)
                //        {
                //            stmt += "classObj.";
                //        }

                //        if (signature.Static)
                //        {
                //            stmt += classObj.FileName + "." + methodCall;
                //        }
                //        else
                //        {
                //            stmt += varString + "." + methodCall;
                //        }
                //        method.Loc = method.Loc + 1;
                //        method.CalledMethods.Add(signature);
                //        method.CalledMethodsWithClassName.Add(variable.Type + "." + method.Name);
                //        return stmt;
                //    }
                //}
            }
            return lhs + " = " + (new Literal(returnType.getType(), Int32.MaxValue)).ToString() + ";";
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") private static java.util.ArrayList getClassByMethodReturnType(java.util.ArrayList<edu.uta.cse.proggen.classLevelElements.Field> objList, edu.uta.cse.proggen.classLevelElements.Type.Primitives returnType, java.util.ArrayList<edu.uta.cse.proggen.packageLevelElements.ClassGenerator> classList)
        private static ArrayList getClassByMethodReturnType(List<Field> objList, Type.Primitives returnType, List<ClassGenerator> classList)
        {
            Field field;
            int counter = 500;
            Random random = new Random();

            field = objList[random.Next(objList.Count)];
            ClassGenerator classObj = getClassByName(classList, field.Type.ToString());

            while (counter > 0 && !classObj.ReturnTypeSet.Contains(returnType))
            {
                field = objList[random.Next(objList.Count)];
                classObj = getClassByName(classList, field.Type.ToString());
                counter--;
            }

            if (counter > 0 && classObj.ReturnTypeSet.Contains(returnType))
            {
                ArrayList list = new ArrayList();
                list.Add(field);
                list.Add(classObj);
                return list;
            }

            return null;
        }

        /// <summary>
        /// get a random method call.
        /// </summary>
        /// <param name="method"> </param>
        /// <param name="classList">
        /// @return </param>
        public static string getMethodCall(Method method, List<ClassGenerator> classList)
        {
            string stmt = "";
            if (ProgGenUtil.methodCallType == CallType.localWithoutRecursionLimit || ProgGenUtil.methodCallType == CallType.localWithRecursionLimit)
            {
                //only local method calls.
                List<MethodSignature> methodList = method.AssociatedClass.MethodSignatures;
                if (methodList.Count < 1)
                {
                    stmt = (new PrintStatement(method)).ToString();
                    return stmt;
                }

                MethodSignature methodToBeInvoked = getMethodToBeInvoked(methodList, method.Static, method.MethodSignature);

                if (methodToBeInvoked == null)
                {
                    stmt = (new PrintStatement(method)).ToString();
                    return stmt;
                }

                // Check if indirect recursion is allowed:          
                if (ConfigurationXMLParser.getProperty("allowIndirectRecursion").ToLower().Equals("no"))
                {
                    //Methods are always named ClassNameM/methodNUMBER
                    string[] tok = methodToBeInvoked.Name.ToLower().Split("method", true);
                    int calleeMethodID = int.Parse(tok[1]);

                    string[] tok2 = method.MethodSignature.Name.ToLower().Split("method", true);
                    int callerMethodID = int.Parse(tok2[1]);

                    // callerID should be lower than calleeID
                    if (callerMethodID >= calleeMethodID)
                    {
                        stmt = (new PrintStatement(method)).ToString();
                        return stmt;
                    }
                }




                List<Variable> parameterList = methodToBeInvoked.ParameterList;
                string parameters = "(";
                parameters += getParametersForList(parameterList, method);
                parameters += ")";

                stmt += methodToBeInvoked.Name + parameters + ";";
                method.CalledMethods.Add(methodToBeInvoked);
                method.CalledMethodsWithClassName.Add(method.AssociatedClass.FileName + "." + methodToBeInvoked.Name);
                method.Loc = method.Loc + 1;
                return stmt;
            }
            else
            {
                //cross-class method calls.
                //Veena : tested in eclipse. Never comes here Since we are not handling objects.
                //Random random = new Random();
                //List<Field> objList = new List<Field>();

                //objList.AddRange(getObjects(method.AssociatedClass.Fields));
                //objList.AddRange(getObjects(method.ParameterList));

                //if (objList.Count != 0)
                //{
                //    Field variable = objList[random.Next(objList.Count)];
                //    ClassGenerator classObj = getClassByName(classList, variable.Type.ToString());
                //    if (classObj != null)
                //    {
                //        string varString = variable.ToString();
                //        List<MethodSignature> signatures = classObj.MethodSignatures;
                //        MethodSignature signature = signatures[random.Next(signatures.Count)];
                //        string methodCall = signature.Name + "(" + getParametersForList(signature.ParameterList, method) + ");\n";

                //        if (!signature.Static)
                //        {
                //            if (method.Static && !variable.Static)
                //            {
                //                if (!variable.Name.StartsWith("var", StringComparison.Ordinal))
                //                {
                //                    stmt += "classObj.";
                //                }
                //            }
                //            stmt += varString + " = new " + variable.Type + "();\n";
                //        }

                //        if (method.Static && !variable.Static && !variable.Name.StartsWith("var", StringComparison.Ordinal) && !signature.Static)
                //        {
                //            stmt += "classObj.";
                //        }

                //        if (signature.Static)
                //        {
                //            stmt += classObj.FileName + "." + methodCall;
                //        }
                //        else
                //        {
                //            stmt += varString + "." + methodCall;
                //        }
                //        method.Loc = method.Loc + 1;
                //        method.CalledMethods.Add(signature);
                //        method.CalledMethodsWithClassName.Add(variable.Type + "." + signature.Name);
                //        return stmt;
                //    }
                //}
            }
            return stmt;
        }


        /// 
        /// <param name="inheritanceDepth">: Randomly chosen inheritance depth </param>
        /// <param name="numberOfClasses">: User defined total number of classes </param>
        /// <param name="usedIntegers">: So far used integers
        /// @return: a unique integer list of size inheritanceDepth </param>
        public static List<int?> getRandomList(int inheritanceDepth, int numberOfClasses, IList<int?> usedIntegers) // e.g., 1, 2, 5, 7 -  e.g., 10 -  e.g., 3
        { // may return a list: {3, 4, 9}
            HashSet<int?> set = new HashSet<int?>();

            Random random = new Random();

            while (set.Count < inheritanceDepth)
            {
                int next = random.Next(numberOfClasses);
                if (!usedIntegers.Contains(next))
                {
                    set.Add(next);
                    usedIntegers.Add(next);
                }
            }

            List<int?> intList = new List<int?>();
            foreach (int num in set)
            {
                intList.Add(num);
            }
            return intList;
        }

        //this is test - delete it


        /// 
        /// <returns> Any of the { CHAR, BYTE, SHORT, INT, LONG, FLOAT, DOUBLE, STRING, OBJECT } types </returns>
        //public static Type.Primitives RandomizedPrimitive
        //{
        //    get
        //    {
        //        List<string> typeList = AllowedTypes;
        //        if (typeList.Count == 0)
        //        {
        //            Console.WriteLine("No type specified in config.xml!");
        //            Environment.Exit(1);
        //        }

        //        string primitiveString = typeList[(new Random()).Next(typeList.Count)];

        //        return primitivesMap[primitiveString];
        //    }
        //}

        public static Type.Primitives getRandomizedPrimitive()
        {
             List<string> typeList = AllowedTypes;
                if (typeList.Count == 0)
                {
                    Console.WriteLine("No type specified in config.xml!");
                    Environment.Exit(1);
                }

                string primitiveString = typeList[(new Random()).Next(typeList.Count)];

                return primitivesMap[primitiveString];
            
        }
        public static Type.Primitives RandomizedPrimitiveForOperands
        {
            get
            {
                //returns any Primitive except Object
                Type.Primitives primitive = ProgGenUtil.getRandomizedPrimitive();

                while (primitive == Type.Primitives.OBJECT)
                {
                    primitive = RandomizedPrimitive;
                }

                return primitive;
            }
        }

        /// 
        /// <param name="noOfInheritanceChains"> : User defined desired number of inheritance chains </param>
        /// <param name="inheritanceDepth"> : Maximum allowed inheritance depth </param>
        /// <param name="range">: Total number of classes </param>
        /// <returns> Something like this: {<3,5,1>,<7,2,6>} random but unique list of integers  </returns>
        public static List<List<int?>> getInheritanceList(int noOfInheritanceChains, int inheritanceDepth, int range) // number of classes
        {
            //Ishtiaque: this is probably already checked
            //Bala: This is a static method which can be reused for Interfaces
            //Even if the main stub is replaced with a GUI, this check will be needed.
            if (range < (noOfInheritanceChains * inheritanceDepth))
            {
                Console.WriteLine("getInheritanceList:: Invalid range");
                return null;
            }
            List<List<int?>> inheritanceList = new List<List<int?>>();

            List<int?> usedList = new List<int?>();

            for (int i = 0; i < noOfInheritanceChains; i++)
            {
                Random random = new Random();

                //we don't want a 0 depth inheritance chain.
                int randomizedInheritanceDepth = random.Next(inheritanceDepth) + 1;

                // considering MinInheritanceDepth;  Min. InheritanceDepth should be less than equal to Max.,=> this is checked in GUI
                if (randomizedInheritanceDepth < minInheritanceDepth)
                {
                    randomizedInheritanceDepth = minInheritanceDepth;
                }



                inheritanceList.Add(ProgGenUtil.getRandomList(randomizedInheritanceDepth, range, usedList));
            }
            return inheritanceList;
        }

        public static Type.Primitives getRandomizedPrimitiveForBooleanExpression(HashSet<Type.Primitives> primitiveSet)
        {
                
            Primitives[] primitiveArray = primitiveSet.ToArray();

            int index = (new Random()).Next(primitiveArray.Length);
                 return (Type.Primitives)primitiveArray[index];
        }

        public static HashSet<Type.Primitives> getPrimitivesOfVariables(Method method)
        {
            HashSet<Type.Primitives> primitiveSet = new HashSet<Type.Primitives>();
            List<Variable> parameterList = method.ParameterList;

            foreach (Variable @var in parameterList)
            {
                //ignore the recursionCounter
                if (@var.Name.Equals("recursionCounter"))
                {
                    continue;
                }

                Type.Primitives primitive = @var.Type.getType();
                //we don't want expressions based on Object type
                if (!(primitive == Type.Primitives.OBJECT))
                {
                    primitiveSet.Add(primitive);
                }
            }
            return primitiveSet;
        }

        public static string getClassToConstruct(string classname, List<ClassGenerator> classList)
        {
            ClassGenerator lhsClass = getClassByName(classList, classname);
            if (lhsClass == null)
            {
                return classname;
            }

            if (ProgGenUtil.coinFlip())
            {
                //return the same class
                return classname;
            }

            //else pick one of its subclasses to return
            HashSet<ClassGenerator> directKnownSubClasses = lhsClass.SubClasses;

            if (directKnownSubClasses.Count == 0)
            {
                //no subclasses
                return classname;
            }

            //Using linked hash set for predictable iteration order
            HashSet<ClassGenerator> knownSubClasses = new HashSet<ClassGenerator>(directKnownSubClasses);

            foreach (ClassGenerator generator in knownSubClasses)
            {
                knownSubClasses.Add(generator);
                HashSet<ClassGenerator> subclasses = generator.SubClasses;
                foreach (ClassGenerator subclass in subclasses)
                {
                    knownSubClasses.Add(subclass);
                }
            }

            object[] subClassArray = knownSubClasses.ToArray();
            ClassGenerator chosenOne = (ClassGenerator)subClassArray[(new Random()).Next(subClassArray.Length)];
            return chosenOne.FileName;
        }

        public static HashSet<Type.Primitives> getValidPrimitivesInScope(Method method)
        {
            HashSet<Type.Primitives> validPrimitivesInScope = new HashSet<Type.Primitives>();

            List<Variable> variableList = method.ParameterList;
            foreach (Variable @var in variableList)
            {
                if (!@var.Name.Equals("recursionCounter"))
                {
                    validPrimitivesInScope.Add(@var.Type.getType());
                }
            }

            if (validPrimitivesInScope.Contains(Type.Primitives.OBJECT))
            {
                validPrimitivesInScope.Remove(Type.Primitives.OBJECT);
            }

            return validPrimitivesInScope;
        }
    }

}