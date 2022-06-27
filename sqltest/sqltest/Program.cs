using System;
using Microsoft.Data.SqlClient;
using System.Text;
using System.Data;


namespace sqltest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "<Replcae with your server name>.database.windows.net";
                builder.UserID = "<Replcae with your User ID>";
                builder.Password = "<Replcae with your password>";
                builder.InitialCatalog = "<Replcae with your databse name>";


                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                //String sql = "SELECT name, collation_name FROM sys.databases" Select count(*) from [dbo].[tTable1] where description like 'inserted%';;
                //using (SqlCommand command = new SqlCommand(sql, connection))
                //{
                //    connection.Open();
                //    using (SqlDataReader reader = command.ExecuteReader())
                //    {
                //        while (reader.Read())
                //        {
                //            Console.WriteLine("{0} {1}", reader.GetString(0), reader.GetString(1));
                //        }
                //    }
                //}
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    string sql = "Insert into[dbo].[tTable1] values(@id, @value, @description)";

                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (SqlCommand cmd = new SqlCommand(sql, connection, transaction))
                            {
                                for (int count = 0; count <= 10000; count = count + 1)
                                {
                                    cmd.Parameters.Add(new SqlParameter(
                                        "@id", SqlDbType.VarChar)).Value = count;
                                    cmd.Parameters.Add(new SqlParameter(
                                        "@value", SqlDbType.VarChar)).Value = "value" + count;
                                    cmd.Parameters.Add(new SqlParameter(
                                        "@description", SqlDbType.VarChar)).Value = "inserted from VS2022";

                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear(); //　泣く泣くこの書き方。。。
                                }
                                transaction.Commit();
                                Console.WriteLine("\nTransction Committed\n");
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.ReadLine();
        }
    }
}