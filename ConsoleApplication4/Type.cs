using System;
using System.Collections.Generic;

namespace edu.uta.cse.proggen.classLevelElements
{

    /// <summary>
    /// This class represents the type for every entity(class variables, local variables,
    /// parameters, return values) in the generated class.
    /// 
    /// @author balamurugan
    /// 
    /// </summary>
    public class Type : System.Object
    {
        /// <summary>
        /// The primitives enum class encapsulates the allowed primitives in RUGRAT which
        /// also includes OBJECT.
        /// 
        /// OBJECT can be any one of the generated types.
        /// 
        /// @author balamurugan
        /// 
        /// </summary>
        /// 

        //Veena : Testing Primitives 
        public enum Primitives
        {
            CHAR, BYTE, SHORT, INT, LONG, FLOAT, DOUBLE, STRING, OBJECT,OTHER
        };

        Primitives p;
        String name;

        /// Veena : Adding constructor.
        public Type(Primitives primitive, string name)
        {
            this.p = primitive;
            this.name = name;
        }

        

        public Primitives getType()
        {
            return p;
        }
        ///start of testing area



        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Type))
            {
                return false;
            }

            Type obj1 = (Type)obj;

            if (this.ToString() != obj1.ToString())
            {
                return false;
            }

            return true;
        }

        //Veena : this is never executed.
        public static Primitives reverseLookup(string primitive)
        {
            if (primitive.Equals("char", StringComparison.CurrentCultureIgnoreCase))
            {
                return Primitives.CHAR;
            }
            else if (primitive.Equals("byte", StringComparison.CurrentCultureIgnoreCase))
            {
                return Primitives.BYTE;
            }
            else if (primitive.Equals("short", StringComparison.CurrentCultureIgnoreCase))
            {
                return Primitives.SHORT;
            }
            else if (primitive.Equals("int", StringComparison.CurrentCultureIgnoreCase))
            {
                return Primitives.INT;
            }
            else if (primitive.Equals("long", StringComparison.CurrentCultureIgnoreCase))
            {
                return Primitives.LONG;
            }
            else if (primitive.Equals("float", StringComparison.CurrentCultureIgnoreCase))
            {
                return Primitives.FLOAT;
            }
            else if (primitive.Equals("double", StringComparison.CurrentCultureIgnoreCase))
            {
                return Primitives.DOUBLE;
            }
            else if (primitive.Equals("String"))
            {
                return Primitives.STRING;
            }
           
            else
            {
                return Primitives.OBJECT;
            }
        }


        public String toString()
        {
            switch (this.p)
            {
                case Primitives.CHAR:
                    return "char";

                case Primitives.BYTE:
                    return "byte";

                case Primitives.SHORT:
                    return "short";

                case Primitives.INT:
                    return "int";

                case Primitives.LONG:
                    return "long";

                case Primitives.FLOAT:
                    return "float";

                case Primitives.DOUBLE:
                    return "double";

                case Primitives.STRING:
                    return "String";

                case Primitives.OTHER:
                    return "Other";
           
                default:
                    return "Object";
            }
        }
    }


}


/// <summary>
/// End of testing block here
/// </summary>




///Veena : Since we are not supporting the object, we can remove this method called toString()
//public override ToString()
//{
//    //if its an object return the classname.
//    if (p.Equals(Primitives.OBJECT))
//    {
//        return name;
//    }

//    return p.ToString();
//}



///Veena : 
///Actually this is how enumaration should be implemented

public sealed class Type1
{

    private readonly String name;
    private readonly int value;

    public static readonly Type1 FORMS = new Type1(1, "FORMS");
    public static readonly Type1 WINDOWSAUTHENTICATION = new Type1(2, "WINDOWS");
    public static readonly Type1 SINGLESIGNON = new Type1(3, "SSN");

    private Type1(int value, String name)
    {
        this.name = name;
        this.value = value;
    }

    public override String ToString()
    {
        return name;
    }

}

