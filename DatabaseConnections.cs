using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Essential using statements 
using System.IO;
using System.Data.OleDb; // MS Access
using System.Data.SQLite; // SQLite
using MySql.Data.MySqlClient; // MySQL

namespace HarvestingFW.V_01
{
	internal class DatabaseConnections
	{
		public static void MSAccessConnection()
		{
			string dbPath = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.mdb").FirstOrDefault();
			using (OleDbConnection connection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dbPath + ";Jet OLEDB:Database Password=qwer@1234;Persist Security Info=True"))
			{
				connection.Open();
				OleDbDataReader rst = new OleDbCommand("SELECT * FROM [Table1] WHERE done = FALSE ORDER BY id", connection).ExecuteReader();
				while (rst.Read())
				{
					// Do your process here

					OleDbTransaction transaction = connection.BeginTransaction(); //for faster performance

					for (int i = 0; i < 100; i++) //multiple operations for transaction example (Replace with your HtmlNode loop)
					{
						OleDbCommand insertCmd = new OleDbCommand("INSERT INTO [Table2] ([Name], [Age]) VALUES (?,?)", connection, transaction);
						insertCmd.Parameters.AddWithValue("Name", "Eaint");
						insertCmd.Parameters.AddWithValue("Age", 23);
						insertCmd.ExecuteNonQuery();
					}

					OleDbCommand updateCmd = new OleDbCommand($"UPDATE [Table1] SET done = TRUE WHERE id = {rst["id"]}", connection, transaction);
					updateCmd.ExecuteNonQuery();

					// Instead of writing to the database 100 times, write once — after all 100 operations are ready
					transaction.Commit();
					Console.WriteLine($"Done!!! - {DateTime.Now}");
				}

				rst.Close();
				connection.Close(); connection.Dispose();
			}
		}
		public static void SQLiteConnection()
		{
			string dbPath = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.sqlite").FirstOrDefault();
			using (SQLiteConnection sqliteconn = new SQLiteConnection("Data Source=" + dbPath + ";Version=3;"))
			{
				sqliteconn.Open();
				SQLiteDataReader rst = new SQLiteCommand("SELECT * FROM [Table1] WHERE done = FALSE ORDER BY id", sqliteconn).ExecuteReader();
				while (rst.Read())
				{
					// Do your process here

					SQLiteTransaction transaction = sqliteconn.BeginTransaction(); //for faster performance

					for (int i = 0; i < 100; i++) //multiple operations for transaction example (Replace with your HtmlNode loop)
					{
						SQLiteCommand insertCmd = new SQLiteCommand("INSERT INTO [Table2] ([Name], [Age]) VALUES (?,?)", sqliteconn, transaction);
						insertCmd.Parameters.AddWithValue("Name", "Eaint");
						insertCmd.Parameters.AddWithValue("Age", 23);
						insertCmd.ExecuteNonQuery();
					}

					SQLiteCommand updateCmd = new SQLiteCommand($"UPDATE [Table1] SET done = TRUE WHERE id = {rst["id"]}", sqliteconn, transaction);
					updateCmd.ExecuteNonQuery();

					// Instead of writing to the database 100 times, write once — after all 100 operations are ready
					transaction.Commit();
					Console.WriteLine($"Done!!! - {DateTime.Now}");
				}

				rst.Close();
				sqliteconn.Close(); sqliteconn.Dispose();
			}
		}
		public static void MySQLConnection()
		{
			#region <Other Database> to <MySQL Database>
			// If u want to Read and Write from <Other Database> to <MySQL Database>
			string dbPath = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.sqlite").FirstOrDefault();
			SQLiteConnection sqliteconn = new SQLiteConnection("Data Source=" + dbPath + ";Version=3;");
			sqliteconn.Open();

			MySqlConnection mysqlconn = new MySqlConnection("Server=127.0.0.1;database=db1;uid=root;pwd=12345;AllowUserVariables=True;");
			mysqlconn.Open();

			SQLiteDataReader rst = new SQLiteCommand("SELECT * FROM table1 WHERE done = FALSE ORDER BY id", sqliteconn).ExecuteReader();
			while (rst.Read())
			{
				// Do your process here

				MySqlTransaction transaction = mysqlconn.BeginTransaction(); //for faster performance

				for (int i = 0; i < 100; i++) //multiple operations for transaction example (Replace with your HtmlNode loop)
				{
					MySqlCommand insertCmd = new MySqlCommand("INSERT INTO `table2` (`Name`, `Age`) VALUES (?,?)", mysqlconn, transaction);
					insertCmd.Parameters.AddWithValue("Name", "Eaint");
					insertCmd.Parameters.AddWithValue("Age", 23);
					insertCmd.ExecuteNonQuery();
				}

				SQLiteCommand updateCmd = new SQLiteCommand($"UPDATE [table1] SET done = TRUE WHERE id = {rst["id"]}", sqliteconn);
				updateCmd.ExecuteNonQuery();

				// Instead of writing to the database 100 times, write once — after all 100 operations are ready
				transaction.Commit();
				Console.WriteLine($"Done!!! - {DateTime.Now}");
			}

			rst.Close();
			mysqlconn.Close(); mysqlconn.Dispose();
			sqliteconn.Close(); sqliteconn.Dispose();
			#endregion

			#region <MySQL Database> to <MySQL Database>
			// If u want to Read and Write from <MySQL Database> to <MySQL Database>
			// MySQL blocks inserts/updates while DataReader is open so use a Second Connection to avoid that error
			MySqlConnection readerConn = new MySqlConnection("Server=127.0.0.1;database=db1;uid=root;pwd=12345;AllowUserVariables=True;");
			readerConn.Open();

			MySqlConnection writerConn = new MySqlConnection("Server=127.0.0.1;database=db1;uid=root;pwd=12345;AllowUserVariables=True;");
			writerConn.Open();

			MySqlDataReader recordSet = new MySqlCommand("SELECT * FROM `table1` WHERE done = FALSE ORDER BY id", readerConn).ExecuteReader();
			while (recordSet.Read())
			{
				// Do your process here

				MySqlTransaction transaction = writerConn.BeginTransaction(); //for faster performance

				for (int i = 0; i < 100; i++) //multiple operations for transaction example (Replace with your HtmlNode loop)
				{
					MySqlCommand insertCmd = new MySqlCommand("INSERT INTO `table2` (`Name`, `Age`) VALUES (?,?)", writerConn, transaction);
					insertCmd.Parameters.AddWithValue("Name", "Eaint");
					insertCmd.Parameters.AddWithValue("Age", 23);
					insertCmd.ExecuteNonQuery();
				}

				MySqlCommand updateCmd = new MySqlCommand($"UPDATE table1 SET done = TRUE WHERE id = @id", writerConn, transaction);
				updateCmd.Parameters.AddWithValue("@id", recordSet["id"]);
				updateCmd.ExecuteNonQuery();

				// Instead of writing to the database 100 times, write once — after all 100 operations are ready
				transaction.Commit();
				Console.WriteLine($"Done!!! - {DateTime.Now}");
			}

			recordSet.Close();
			readerConn.Close(); readerConn.Dispose();
			writerConn.Close(); writerConn.Dispose();
			#endregion
		}
	}
}