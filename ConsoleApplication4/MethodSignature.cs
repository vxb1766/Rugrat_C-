using System.Collections.Generic;

namespace edu.uta.cse.proggen.classLevelElements
{


	using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;

	/// <summary>
	/// This class represents the signature of a generated method.
	/// 
	/// @author balamurugan
	/// 
	/// </summary>
	public class MethodSignature
	{
		private bool isStatic;
		private Primitives returnType;
		private string name;
		private List<Variable> parameterList;

		// To stop indirect recursion
		private HashSet<MethodSignature> callerMethods = new HashSet<MethodSignature>();

		public MethodSignature(bool isStatic, Primitives returnType, string name, List<Variable> parameterList) : base()
		{
			this.isStatic = isStatic;
			this.returnType = returnType;
			this.name = name;
			this.parameterList = parameterList;
		}

		/// <summary>
		/// returns the String form of the MethodSignature
		/// eg: int add(int x, int y)
		/// </summary>
		public override string ToString()
		{
			string str = "public virtual ";

			if (isStatic)
			{
				str += "static ";
			}

			str += returnType.ToString() + " " + name + "(";

			foreach (Variable parameter in parameterList)
			{
				str += parameter.FieldDeclaration + ", ";
			}

			str = str.Substring(0, str.Length - 2);
			str += ")";

			return str;
		}

		/* (non-Javadoc)
		 * @see java.lang.Object#equals(java.lang.Object)
		 */
		public override bool Equals(object obj)
		{
			/*
			 * not necessary to override hashcode as
			 * we are not persisting these objects in 
			 * hash based structures.
			 */
			if (obj == null)
			{
				return false;
			}

			if (!(obj is MethodSignature))
			{
				return false;
			}

			MethodSignature obj1 = (MethodSignature)obj;

			if (this.isStatic != obj1.isStatic)
			{
				return false;
			}

			if (this.returnType != obj1.returnType)
			{
				return false;
			}

			if (this.name != obj1.name)
			{
				return false;
			}

			int size = this.parameterList.Count;
			for (int i = 0; i < size; i++)
			{
				if (!this.parameterList[i].Equals(obj1.parameterList[i]))
				{
					return false;
				}
			}
			return true;
		}

		public virtual bool Static
		{
			get
			{
				return isStatic;
			}
		}

		public virtual Primitives ReturnType
		{
			get
			{
				return returnType;
			}
		}

		public virtual string Name
		{
			get
			{
				return name;
			}
		}

		public virtual List<Variable> ParameterList
		{
			get
			{
				return parameterList;
			}
		}

		public virtual HashSet<MethodSignature> CallerMethodSignature
		{
			get
			{
				return callerMethods;
			}
		}
	}

}