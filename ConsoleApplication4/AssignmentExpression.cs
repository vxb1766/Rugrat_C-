using System;
using System.Collections.Generic;
using System.Text;

namespace edu.uta.cse.proggen.expressions
{


	using Field = edu.uta.cse.proggen.classLevelElements.Field;
	using Method = edu.uta.cse.proggen.classLevelElements.Method;
	using Primitives = edu.uta.cse.proggen.classLevelElements.Type.Primitives;
	using Query = edu.uta.cse.proggen.configurationParser.Query;
	using QueryFileParser = edu.uta.cse.proggen.configurationParser.QueryFileParser;
	using QueryResult = edu.uta.cse.proggen.configurationParser.QueryResult;
	using Expression = edu.uta.cse.proggen.nodes.Expression;
	using Operator = edu.uta.cse.proggen.nodes.Operator;
	using BinaryOperator = edu.uta.cse.proggen.operators.BinaryOperator;
	using PrintStatement = edu.uta.cse.proggen.statements.PrintStatement;
	using ProgGenUtil = edu.uta.cse.proggen.util.ProgGenUtil;

	/// <summary>
	/// var = NormanExpr BinOp NormalExpr
    /// @author Team 6 - CSE6324 - Spring 2015
	/// 
	/// </summary>
	public class AssignmentExpression : Expression
	{
		internal Field lhs;
		internal Expression expr = null;
		internal Expression leftExpr;
		internal Expression rightExpr;
		internal string output = "";

		public AssignmentExpression(Method method)
		{
			Random rand = new Random();

			//should we use field or variable?
			int OptionVarOrField = rand.Next(100) % 2;

			if (OptionVarOrField == 0)
			{
				// Introducing any variable
				lhs = VariableGenerator.getRandomizedVariable(method);
			}
			else
			{
				//Introducing any field
				lhs = FieldGenerator.getRandomField(method.AssociatedClass, method.Static);

				if (lhs == null)
				{
					//if no field is present, introduce variable
					lhs = VariableGenerator.getRandomizedVariable(method);
				}
			}

			// if no variable is present, just use print statements
			if (lhs == null)
			{
				output += (new PrintStatement(method)).ToString();
				return;
			}


            Primitives primitive = lhs.type.getType();

			//If the primitive is an object, invoke constructor
			if (primitive == Primitives.OBJECT)
			{
				output += lhs + " = new " + ProgGenUtil.getClassToConstruct(lhs.Type.ToString(), method.ClassList) + "();\n";
				return;
			}

			//Randomly choose to nest operation or not
			int option = rand.Next(100);
			if (option > method.AssociatedClass.Percent)
			{
				leftExpr = new NormalExpression(method, primitive);
				rightExpr = new NormalExpression(method, primitive);
				Operator binOp = new BinaryOperator(primitive);

				// Removing variable from right expr. if binOp is / or %
				// i6=(i3/2)%(i3-5)  => i6=(i3/2)%constant
                //if (binOp.ToString().Equals("/") || binOp.ToString().Equals("%"))
                //{
                //    do
                //    { //FIXME: only handles int for now.
                //        rightExpr = new Literal(primitive,Int32.MaxValue);
                //    }while (rightExpr.ToString().Contains("(0)")); //avoiding divide by (0)
                //}

				output += lhs + " = (" + lhs.Type + ")(" + leftExpr + binOp.ToString() + rightExpr + ");\n";
			}
			else
			{
				if (ProgGenUtil.useQueries)
				{
					if (ProgGenUtil.coinFlip())
					{
						expr = new NormalExpression(method, primitive);

						//we don't want assignments statements like this: i8 = i8
						while (lhs.Equals(expr.ToString()))
						{
                            expr = new NormalExpression(method, lhs.type.getType());
						}
						output += lhs + " = (" + lhs.Type + ")" + expr.ToString() + ";\n";
					}
					else
					{
						Console.WriteLine("Trying to fetch literal from database for : " + lhs.Type);
						StringBuilder builder = new StringBuilder();
						Query query = getQueryForType(primitive);
						if (query == null)
						{
							//no query results
                            output += lhs + " = (" + lhs.Type + ")" + new Literal(primitive, Int32.MaxValue) + ";\n";
							return;
						}

						builder.Append("try{");
						builder.Append("ResultSet rs = DBUtil.getDBUtil().executeQuery(");
						builder.Append("\"" + query.QueryString + "\"");
						builder.Append(");\n");

						builder.Append("rs.last();\n");
						builder.Append("int rowToSelect = new Random().nextInt(rs.getRow());\n");
						builder.Append("rs.first();\n");

						builder.Append("for(int rowToSelectCounter=0; rowToSelectCounter<rowToSelect;rowToSelectCounter++)");
						builder.Append("{ 	rs.next();	}\n");

						List<QueryResult> queryResultsForType = getColumnNumbersForType(query, primitive);
						if (queryResultsForType.Count == 0)
						{
							//no query results present for expected type.
                            output += lhs + " = (" + lhs.Type + ")" + new Literal(primitive, Int32.MaxValue) + ";\n";
							return;
						}

						int resultToSelect = (new Random()).Next(queryResultsForType.Count);
						QueryResult selectedResult = queryResultsForType[resultToSelect];

						string result = getValueFromResultSet(primitive, selectedResult);
						if (result == null)
						{
                            output += lhs + " = (" + lhs.Type + ")" + new Literal(primitive, Int32.MaxValue) + ";\n";
							return;
						}
						builder.Append(lhs + " = (" + lhs.Type + ")" + result + "\n\n");
						builder.Append("} catch(Exception e) { e.printStackTrace(); }\n\n");
                        builder.Append("Console.WriteLine(" + lhs + ");\n");
						output += builder.ToString();
                        
					}
				}
				else
				{
					expr = new NormalExpression(method, primitive);

					//we don't want assignments statements like this: i8 = i8
					while (lhs.Equals(expr.ToString()))
					{
                        expr = new NormalExpression(method, lhs.type.getType());
					}
					output += lhs + " = (" + lhs.Type + ")" + expr.ToString() + ";\n";
				}
			}
		}

		private string getValueFromResultSet(Primitives primitive, QueryResult result)
		{
			bool fetchUsingSeqNumber = ProgGenUtil.coinFlip();
			if (primitive == Primitives.INT)
			{
				if (fetchUsingSeqNumber)
				{
					return "rs.getInt(" + result.SeqNumber + ");\n";
				}
				return "rs.getInt(\"" + result.Name + "\");\n";
			}
			else if (primitive == Primitives.BYTE)
			{
				if (fetchUsingSeqNumber)
				{
					return "rs.getByte(" + result.SeqNumber + ");\n";
				}
				return "rs.getByte(\"" + result.Name + "\");\n";
			}
			else if (primitive == Primitives.CHAR)
			{
				if (fetchUsingSeqNumber)
				{
					return "rs.getString(" + result.SeqNumber + ");\n";
				}
				return "rs.getString(\"" + result.Name + "\");\n";
			}
			else if (primitive == Primitives.DOUBLE)
			{
				if (fetchUsingSeqNumber)
				{
					return "rs.getDouble(" + result.SeqNumber + ");\n";
				}
				return "rs.getDouble(\"" + result.Name + "\");\n";
			}
			else if (primitive == Primitives.FLOAT)
			{
				if (fetchUsingSeqNumber)
				{
					return "rs.getFloat(" + result.SeqNumber + ");\n";
				}
				return "rs.getFloat(\"" + result.Name + "\");\n";
			}
			else if (primitive == Primitives.LONG)
			{
				if (fetchUsingSeqNumber)
				{
					return "rs.getLong(" + result.SeqNumber + ");\n";
				}
				return "rs.getLong(\"" + result.Name + "\");\n";
			}
			else if (primitive == Primitives.SHORT)
			{
				if (fetchUsingSeqNumber)
				{
					return "rs.getShort(" + result.SeqNumber + ");\n";
				}
				return "rs.getShort(\"" + result.Name + "\");\n";
			}
			else if (primitive == Primitives.STRING)
			{
				if (fetchUsingSeqNumber)
				{
					return "rs.getString(" + result.SeqNumber + ");\n";
				}
				return "rs.getString(\"" + result.Name + "\");\n";
			}
			return null;
		}

		private List<QueryResult> getColumnNumbersForType(Query query, Primitives primitive)
		{
			List<QueryResult> list = new List<QueryResult>();

			List<QueryResult> results = query.Results;

			foreach (QueryResult result in results)
			{
				if (result.Type.Equals(primitive))
				{
					list.Add(result);
				}
			}
			return list;
		}

		private Query getQueryForType(Primitives primitive)
		{
			List<Query> queryList = QueryFileParser.queries;
			int noOfQueries = queryList.Count;
			Random random = new Random();

			int index = random.Next(noOfQueries);
			int count = 100;
			Query query = queryList[index];

            while (!query.ResultTypes.Contains(primitive) && count > 0)
			{
				query = queryList[random.Next(noOfQueries)];
				count--;
			}

			if (count == 0)
			{
				//no query was found with expected type in its results.
				return null;
			}

			return query;
		}

		public override string ToString()
		{
			return output;
		}
	}

}