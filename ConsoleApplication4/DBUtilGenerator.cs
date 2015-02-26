using System.Text;

namespace edu.uta.cse.proggen.packageLevelElements
{

	using ProgGenUtil = edu.uta.cse.proggen.util.ProgGenUtil;

	/// <summary>
	/// Class that generates the contents of DBUtil in the generated application.
	/// 
	/// @author balamurugan
	/// 
	/// </summary>
	public class DBUtilGenerator
	{
		private string packageString = "namespace com.accenture.lab.carfast.test\n{\n\n";
		private string importString = "import java.sql.Connection;\nimport java.sql.DriverManager;\nimport java.sql.PreparedStatement;\n" + "import java.sql.ResultSet;\nimport java.sql.SQLException;\nimport java.util.Properties;\n\n\n\n";

		private string classnameString = "public class DBUtil{\n";
		private string membersString = "private static	DBUtil	dbUtil;\nprivate Connection	connection;\n";
		internal string constructorString = "//singleton implementation for database resources.\n" + "private DBUtil(){\n" + "try{\n" + "Class.forName(\"" + ProgGenUtil.driver + "\").newInstance();\n" + "Properties props = new Properties();\n" + "props.put(\"user\",\"" + ProgGenUtil.dbUserName + "\");\n" + "props.put(\"password\",\"" + ProgGenUtil.password + "\");\n" + "connection = DriverManager.getConnection(\"" + ProgGenUtil.dbUrl + "\", props);\n}\n" + "catch (Exception e) {\n" + "e.printStackTrace(); System.exit(1);\n}\n}\n\n";

		private string methodsString = "public static DBUtil getDBUtil(){\n" + "if(dbUtil == null)	{\n" + "dbUtil = new DBUtil();\n}\n" + "return dbUtil;\n }\n\n" + "public ResultSet executeQuery(String sql){\n" + "try {\n" + "PreparedStatement ps = connection.prepareStatement(sql);\n" + "return ps.executeQuery();\n}\n" + "catch (SQLException e) {\n" + "e.printStackTrace();\n" + "return null;\n}\n}\n\n";

		private string endOfClassString = "\n}\n}\n";

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(packageString);
			builder.Append(importString);
			builder.Append(classnameString);
			builder.Append(membersString);
			builder.Append(constructorString);
			builder.Append(methodsString);
			builder.Append(endOfClassString);

			return builder.ToString();
		}
	}

}