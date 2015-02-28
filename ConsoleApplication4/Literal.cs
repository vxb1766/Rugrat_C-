using System;
using System.Text;

namespace edu.uta.cse.proggen.expressions
{

    using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;
	using Operand = edu.uta.cse.proggen.nodes.Operand;
	using ProgGenUtil = edu.uta.cse.proggen.util.ProgGenUtil;

	/// <summary>
	/// This returns a literal except 0 {}
    /// @author Team 6 - CSE6324 - Spring 2015
	/// 
	/// </summary>
	public class Literal : Operand
	{
		internal readonly char[] alphabets = new char[] {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};

		public Literal(Primitives primitive , int randomRange)
		{
			assignLiteralForType(primitive,randomRange);
		}

		private void assignLiteralForType(Primitives primitive , int randomRange)
		{
			Random random = new Random();

			switch (primitive.ToString())
			{
				case "CHAR":
					literal = "'" + (new char?(alphabets[random.Next(26)])).ToString() + "'";
					break;

				case "BYTE":
					byte[] bytes = new byte[100];
                    random.NextBytes(bytes);
					sbyte b = (sbyte)bytes[random.Next(99)];
					literal = "(byte)(" + (new sbyte?(b)).ToString() + ")";
					break;

				case "SHORT":
					short shortVal = (short)random.Next(32768);
					literal = "(short)(" + shortVal + ")";
					break;

				case "INT":
					literal = "(int)(" + ((new int?(random.Next(ProgGenUtil.integerMaxValue))).ToString()) + ")";
					break;

				case "LONG":
					//long literals can cause out-of-range exceptions. using compatible int instead
					literal = "(long)(" + (new long?(random.Next(ProgGenUtil.integerMaxValue))).ToString() + ")";
					break;

				case "FLOAT":
					literal = "(float)(" + (new float?((float)random.NextDouble())).ToString() + ")";
					break;

				case "DOUBLE":
					literal = "(double)(" + (new double?(random.NextDouble())).ToString() + ")";
					break;

				case "STRING":
					StringBuilder builder = new StringBuilder();
					int max = random.Next(100) + 2;
					for (int i = 0; i < max; i++)
					{
						builder.Append(alphabets[random.Next(26)]);
					}
					literal = "\"" + builder.ToString() + "\"";
					break;

                case "OTHER" :
                    literal = "(int)(" + "var"+(randomRange+1)+ ")";
                    //literal;

                    break;

				default:
                    literal = "null";
                  


				break;
			}
		}
	}

}