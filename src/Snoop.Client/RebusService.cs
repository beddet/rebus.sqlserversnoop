using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace Snoop.Client
{
    public class RebusService
    {
        public List<TableViewModel> GetTables(string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var tables = connection.Query<string>("select name from sys.tables").ToList();
                return tables.Select(x => new TableViewModel(x)).ToList();
            }
        }

        public List<MessageViewModel> GetMessages(string connectionString, string table)
        {
            try
            {
                List<MessageQueryModel> messages;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    messages = connection.Query<MessageQueryModel>($"select id, headers, body from {table}").ToList();
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
    }
}