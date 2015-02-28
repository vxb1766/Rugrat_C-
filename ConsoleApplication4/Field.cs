using System;
using System.Collections.Generic;

namespace edu.uta.cse.proggen.classLevelElements
{


	using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;
	using Operand = edu.uta.cse.proggen.nodes.Operand;
	using ClassGenerator = edu.uta.cse.proggen.namespaceLevelElements.ClassGenerator;
	using ProgGenUtil = edu.uta.cse.proggen.util.ProgGenUtil;

	/// <summary>
	/// This class represents a class variable in the generated class
	/// 
    /// @author Team 6 - CSE6324 - Spring 2015
	/// 
	/// </summary>
	public class Field : Operand
	{
		protected internal Type type;
		protected internal string name;
		protected internal bool isStatic;
		protected internal bool isArray;
		protected internal int arraySize;

		/* (non-Javadoc)
		 * @see java.lang.Object#equals(java.lang.Object)
		 */
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			if (!(obj is Field))
			{
				return false;
			}

			Field obj1 = (Field)obj;

			if (!this.type.Equals(obj1.type))
			{
				return false;
			}

			if (!this.name.Equals(obj1.name))
			{
				return false;
			}

			if (this.isStatic != obj1.isStatic)
			{
				return false;
			}

			if (this.isArray != obj1.isArray)
			{
				return false;
			}

			if (this.arraySize != obj1.arraySize)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// creates a random field with the given name. the field can be 
		/// of a user-specified type or one of the generated types.
		/// </summary>
		/// <param name="name"> - of the field </param>
		/// <param name="classList"> - list of generated types </param>
		protected internal Field(string name, List<ClassGenerator> classList)
		{
			this.name = name;
			this.arraySize = -1;
            Primitives primitive = ProgGenUtil.RandomizedPrimitive;
			string classname = "";

			if (primitive == Primitives.OBJECT)
			{
				int randomIndex = (new Random()).Next(classList.Count);
				classname = classList[randomIndex].FileName;
			}

			type = new Type(primitive, classname);
			randomizeIsStatic();
			randomizeIsArray();
		}

		/// <summary>
		/// Constructs a field of the specified primitive type </summary>
		/// <param name="name"> : of the field </param>
		/// <param name="primitive"> : type of the field </param>
		protected internal Field(string name, Primitives primitive)
		{
			this.name = name;
			this.type = new Type(primitive, "");
		}

		private void randomizeIsStatic()
		{
			isStatic = ProgGenUtil.coinFlip();
		}

		private void randomizeIsArray()
		{
			if (ProgGenUtil.allowArray.Equals("yes"))
			{
				isArray = ProgGenUtil.coinFlip();
			}
			else
			{
				isArray = false;
			}
		}

		/// <summary>
		/// Randomly selects the type of the field from { CHAR, BYTE, SHORT, INT, LONG, FLOAT, DOUBLE, STRING, OBJECT }.
		/// If it's OBJECT type; only then it selects one class from the 'classList' as the field's type. </summary>
		/// <param name="name"> Name of the field. </param>
		/// <param name="classList"> Helper list containing all the declared classes. </param>
		/// <returns> A field of primitive type or of a declared class type. </returns>
		public static Field generateField(string name, List<ClassGenerator> classList)
		{
			return new Field(name, classList);
		}

		public virtual Type Type
		{
			get
			{
				return type;
			}
		}

		public virtual bool Static
		{
			get
			{
				return isStatic;
			}
		}

		/// <summary>
		/// returns the field declaration.
		/// 
		/// eg: int i;
		///     char str[] = new char[26];
		/// </summary>
		/// <returns> : the field declaration String </returns>
		public virtual string FieldDeclaration
		{
			get
			{
				string str = "";
    
				if (isStatic)
				{
					str += "static ";
				}
    
				if (!isArray)
				{
					str += type.getType().ToString() + " " + name;
				}
				else
				{
					this.arraySize = ProgGenUtil.RandomArraySize;
                    str += type.getType().ToString() + "[] " + name + "= new " + type.getType().ToString() + "[" + this.arraySize + "]";
				}
    
				return str;
			}
		}

		/// <summary>
		/// Returns field name.
		/// e.g.:
		/// Array type: a[5]
		/// Non array type: a 
		/// </summary>
		public override string ToString()
		{
			this.literal = "";

			if (!isArray)
			{
				literal += name;
			}
			else
			{
				int arrayIndex = ProgGenUtil.getRandomIntInRange(arraySize);
				literal += name + "[" + arrayIndex + "]";
			}

			return literal;
		}

		public virtual int ArraySize
		{
			get
			{
				return arraySize;
			}
		}

		public virtual string Name
		{
			get
			{
				return name;
			}
		}
	}

}