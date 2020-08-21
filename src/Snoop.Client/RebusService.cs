using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace Snoop.Client
{
    public class ValidTableQueryModel
    {
        public long Id { get; set; }
        public byte[] Headers { get; set; }
        public byte[] Body { get; set; }
        public int Count { get; set; }
    }

    public class RebusService
    {
        public List<TableViewModel> GetValidTables(string connectionString)
        {
            var validTables = new List<TableViewModel>();
            using (var connection = new SqlConnection(connectionString.Replace("//", "/")))
            {
                connection.Open();

                var tables = connection.Query<TableQueryModel>(@"
                SELECT t.name AS TableName, s.name AS SchemaName FROM sys.tables t
                INNER JOIN sys.schemas s ON t.schema_id = s.schema_id").ToList();

                foreach (var table in tables)
                {
                    //try to get a rowcount from each table, using columns that are in rebus queue tables
                    //this will throw an exception if the table doesn't have the needed columns
                    //there could potentially still be an issue if a table has those columns but are not from rebus and have different types
                    try
                    {
                        var rowCount = connection.ExecuteScalar<int>($@"WITH cte as (SELECT id,
                        priority,
                        expiration,
                        visible,
                        headers,
                        body FROM {table.GetQualifiedName()}) SELECT COUNT(*) FROM cte");
                        validTables.Add(new TableViewModel(table.GetQualifiedName(), rowCount));
                    }
                    catch 
                    {
                        //any exception caught here is probably because the table doesn't have the right columns, so we can't use it
                        //ignore the exception
                    }
                }
            }

            return validTables;
        }

        public List<MessageViewModel> GetMessages(string connectionString, string table)
        {
            try
            {
                List<MessageQueryModel> messages;
                using (var connection = new SqlConnection(connectionString.Replace("//", "/")))
                {
                    connection.Open();

                    messages = connection.Query<MessageQueryModel>($"select id, headers, body from {table} with (nolock)").ToList();
                }

                var result = new List<MessageViewModel>();
                foreach(var message in messages)
                {
                    result.Add(RebusMessageParser.ParseMessage(message));
                }

                return result.Where(x => x != null).ToList(); //todo handle errors - ParseMessage returns null if it fails
            }
            catch (Exception e)
            {
                //todo handle error
                return new List<MessageViewModel>();
            }
        }

        public void DeleteMessage(string connectionString, string table, long id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString.Replace("//", "/")))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = $"delete from {table} where id = {id}";
                }
            }
            catch (Exception e)
            {
                //todo handle error
            }
        }
    }
}