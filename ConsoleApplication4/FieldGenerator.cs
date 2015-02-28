using System;
using System.Collections.Generic;

namespace edu.uta.cse.proggen.expressions
{


	using Field = edu.uta.cse.proggen.classLevelElements.Field;
	using Method = edu.uta.cse.proggen.classLevelElements.Method;
	using Type = edu.uta.cse.proggen.classLevelElements.Type;
	using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;
	using Operand = edu.uta.cse.proggen.nodes.Operand;
	using ClassGenerator = edu.uta.cse.proggen.namespaceLevelElements.ClassGenerator;

	/// <summary>
	/// Class with utility methods for fetching fields from a generated class.
	/// 
    /// @author Team 6 - CSE6324 - Spring 2015
	/// 
	/// </summary>
	public class FieldGenerator
	{
		/// <summary>
		/// Returns a random field for a given class.
		/// </summary>
		/// <param name="generator"> </param>
		/// <param name="isStatic">
		/// @return </param>
		public static Field getRandomField(ClassGenerator generator, bool isStatic)
		{
			Field field;
			Random rand = new Random();

			// if no fields for this class is available
			if (generator.Fields.Count == 0)
			{
				return null;
			}

			field = generator.Fields[rand.Next(generator.Fields.Count)];
			int count = 5000;

			while (field.Static != isStatic && count > 0)
			{
				field = generator.Fields[rand.Next(generator.Fields.Count)];
				count--;
			}

			if (field.Static == isStatic && count > 0)
			{
				// adding to the used variable Set
				generator.UsedFields.Add(field);
				return field;
			}

			return null;
		}

		/// <summary>
		/// returns a random field of a given type for a generated class.
		/// </summary>
		/// <param name="generator"> </param>
		/// <param name="primitive"> </param>
		/// <param name="isStatic">
		/// @return </param>
		public static Operand getRandomField(ClassGenerator generator, Type.Primitives primitive, bool isStatic)
		{
			Field field;

			field = getField(generator.Fields, primitive, isStatic);

			if (field == null)
			{
				return new Literal(primitive,Int32.MaxValue);
			}

			generator.UsedFields.Add(field);
			return field;
		}

		private static Field getField(List<Field> fields, Type.Primitives primitive, bool isStatic)
		{
			List<Field> typedFieldList = new List<Field>();

			foreach (Field @var in fields)
			{
				if (@var.type.getType() == primitive && @var.Static == isStatic)
				{
					typedFieldList.Add(@var);
				}
			}

			if (typedFieldList.Count == 0)
			{
				return null;
			}

			int index = (new Random()).Next(typedFieldList.Count);
			return typedFieldList[index];
		}

		/// <summary>
		/// Returns a randomized object of a given type belonging
		/// to a generated class.
		/// </summary>
		/// <param name="generator"> </param>
		/// <param name="type">
		/// @return </param>
		public static Operand getRandomizedObjectForType(ClassGenerator generator, Type type)
		{
			List<Field> fieldList = generator.Fields;
			List<Field> typedFieldList = new List<Field>();

			foreach (Field field in fieldList)
			{
				if (field.Type.Equals(type))
				{
					typedFieldList.Add(field);
				}
			}

			if (typedFieldList.Count == 0)
			{
				return new Literal(Type.Primitives.OBJECT,Int32.MaxValue);
			}

			int index = (new Random()).Next(typedFieldList.Count);
			return typedFieldList[index];
		}
	}

}