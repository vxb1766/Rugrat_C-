using System.Collections.Generic;

namespace edu.uta.cse.proggen.classLevelElements
{

	using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;
	using ClassGenerator = edu.uta.cse.proggen.namespaceLevelElements.ClassGenerator;

	/// <summary>
	/// Represents a variable in the generated class.
	/// 
    /// @author Team 6 - CSE6324 - Spring 2015
	/// 
	/// </summary>
	public class Variable : Field
	{
		protected internal Variable(string name, List<ClassGenerator> classList) : base(name, classList)
		{

			//variables are not arrays
			this.isArray = false;

			//variables are not static
			this.isStatic = false;
		}

		protected internal Variable(string name, Primitives primitive) : base(name, primitive)
		{

			//variables are not arrays
			this.isArray = false;

			//variables are not static
			this.isStatic = false;
		}

		public static Variable generateVariable(string name, List<ClassGenerator> classList)
		{
			return new Variable(name, classList);
		}

		public static Variable generateRecursionCounterVariable()
		{
			return new Variable("recursionCounter", Primitives.INT);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			if (!base.Equals(obj))
			{
				return false;
			}

			return true;
		}
	}

}