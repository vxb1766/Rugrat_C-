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
	public class Type
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
		public sealed class Primitives
		{
			public static readonly Primitives CHAR, BYTE, SHORT, INT, LONG, FLOAT, DOUBLE, STRING, OBJECT = new Primitives("CHAR, BYTE, SHORT, INT, LONG, FLOAT, DOUBLE, STRING, OBJECT", InnerEnum.CHAR, BYTE, SHORT, INT, LONG, FLOAT, DOUBLE, STRING, OBJECT);

			private static readonly IList<Primitives> valueList = new List<Primitives>();

			static Primitives()
			{
				valueList.Add(CHAR, BYTE, SHORT, INT, LONG, FLOAT, DOUBLE, STRING, OBJECT);
			}

			public enum InnerEnum
			{
				CHAR, BYTE, SHORT, INT, LONG, FLOAT, DOUBLE, STRING, OBJECT
			}

			private readonly string nameValue;
			private readonly int ordinalValue;
			private readonly InnerEnum innerEnumValue;
			private static int nextOrdinal = 0;

			private Primitives(string name, InnerEnum innerEnum)
			{
				nameValue = name;
				ordinalValue = nextOrdinal++;
				innerEnumValue = innerEnum;
			}

			public override string ToString()
			{
				switch (this)
				{
					case CHAR:
						return "char";

					case BYTE:
						return "byte";

					case SHORT:
						return "short";

					case INT:
						return "int";

					case LONG:
						return "long";

					case FLOAT:
						return "float";

					case DOUBLE:
						return "double";

					case STRING:
						return "String";

					default:
						return "Object";
				}
			}

			public static IList<Primitives> values()
			{
				return valueList;
			}

			public InnerEnum InnerEnumValue()
			{
				return innerEnumValue;
			}

			public int ordinal()
			{
				return ordinalValue;
			}

			public static Primitives valueOf(string name)
			{
				foreach (Primitives enumInstance in Primitives.values())
				{
					if (enumInstance.nameValue == name)
					{
						return enumInstance;
					}
				}
				throw new System.ArgumentException(name);
			}
		}

		internal Primitives p;
		internal string name;

		public Type(Primitives primitive, string name)
		{
			this.p = primitive;
			this.name = name;
		}

		public virtual Primitives getType()
		{
			return p;
		}

		public override string ToString()
		{
			//if its an object return the classname.
			if (p.Equals(Primitives.OBJECT))
			{
				return name;
			}

			return p.ToString();
		}

		public override bool Equals(object obj)
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
	}

}