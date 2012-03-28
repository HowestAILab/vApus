----
link
----
http://gborg.postgresql.org/project/npgsql/projdisplay.php
http://pgfoundry.org/projects/npgsql

error in driver:
http://pgfoundry.org/tracker/index.php?func=detail&aid=1000575&group_id=1000140&atid=590

-----------
connection
-----------
string volgens connectionstrings.com :  "DRIVER={PostgreSQL};SERVER=ipaddress;port=5432;DATABASE=dbname;UID=username;PWD=password;"

string volgens postgre : Server=192.168.0.1;User Id=test;Password=pass;Database=Test";

To use Npgsql you should use the Npgsql namespace.

In C# you use the using statement. 
Later, you create the NpgsqlConnection object.

You can provide the following parameters in the connection string:

    * Server(IP or dns name)
    * Port
    * User Id
    * Password
    * Database

Where Database is optional and defaults to User Id and Port defaults to 5432.

The key-value pairs are semi-colon separated.

If there is any error in the connection string, an ArgumentException is thrown. There are two points where the connection string is checked: in the constructor and when it is assigned with the ConnectionString property.
For npgsql v. 0.1
FAQ created by: fxjr
Create Date: May 22, 2002
Last Modified: May 22, 2002
Code Example:

	

// in C#

using System;

using System.Data;

using Npgsql;



public class Test

{

     public static void Main(String[] args)

     {

         String connstring = "Server=192.168.0.1;User Id=test;Password=pass;Database=Test";

         NpgsqlConnection conn = new NpgsqlConnection(connstring);

         conn.Open();

         conn.Close();

     }

}

----------------------
update, insert, delete
----------------------
ing Npgsql;



public class TestInsertDeleteUpdate

{





    public static void Main(String[] args)

	{

		try

		{

			NpgsqlConnection conn = new NpgsqlConnection("Server=server;User ID=user;Password=passwd;Database=database");

			conn.Open();

			Console.WriteLine("Connection completed");

			

			NpgsqlCommand command = new NpgsqlCommand();

			command.CommandText = "insert into t1 values(10, 5)";

			command.Connection = conn;

			Int32 num_rows = command.ExecuteNonQuery();

			Console.WriteLine("{0} rows were added!", num_rows);

			

			command.CommandText = "delete from t1 where a = 10";

			num_rows = command.ExecuteNonQuery();

			Console.WriteLine("{0} rows were deleted!", num_rows);

			

					conn.Close();

			

		}

		catch(NpgsqlException e)

		{

			Console.WriteLine(e.ToString());

		}

	}

}	