﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Dapper;
using Rebus.Messages;

namespace Snoop.Client
{
    public class RebusService
    {
        public List<TableViewModel> GetValidTables(string connectionString)
        {
            var validTables = new List<TableViewModel>();
            using (var connection = new SqlConnection(FixConnectionStringFormat(connectionString)))
            {
                connection.Open();

                var tables = connection.Query<TableQueryModel>(@"
                    SELECT t.name AS TableName, s.name AS SchemaName FROM sys.tables t
                    INNER JOIN sys.schemas s ON t.schema_id = s.schema_id").ToList();

                foreach (var table in tables)
                {

                    // try to get a rowcount from each table, using columns that are in rebus queue tables
                    // this will throw an exception if the table doesn't have the needed columns
                    // there could potentially still be an issue if a table has those columns but are not from rebus and have different types
                    try
                    {
                        connection.Query(
                            $"SELECT id, priority, expiration, visible, headers, body FROM {table.GetQualifiedName()} WITH (NOLOCK)");

                        var rowCount = connection.ExecuteScalar<int>($@"SELECT COUNT(*) FROM {table.GetQualifiedName()} WITH (NOLOCK)");

                        validTables.Add(new TableViewModel(table.GetQualifiedName(), rowCount, connectionString));
                    }
                    catch (SqlException ex) when (ex.Message.IndexOf("Invalid column name", StringComparison.OrdinalIgnoreCase) != -1)
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
                using (var connection = new SqlConnection(FixConnectionStringFormat(connectionString)))
                {
                    connection.Open();

                    messages = connection.Query<MessageQueryModel>($"SELECT id, priority, visible, expiration, headers, body " +
                                                                   $"FROM {table} WITH (NOLOCK)" +
                                                                   $"ORDER BY visible").ToList();
                }

                var result = new List<MessageViewModel>();
                foreach (var message in messages)
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

        private MessageQueryModel LoadSingleMessage(string connectionString, string table, long id)
        {
            try
            {
                using (var connection = new SqlConnection(FixConnectionStringFormat(connectionString)))
                {
                    connection.Open();

                    var messages = connection.Query<MessageQueryModel>($"select id, priority, visible, expiration, headers, body from {table} where id = {id}").ToList();
                    return messages.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                //todo handle error
                return null;
            }
        }

        public void DeleteMessage(string connectionString, string table, long id)
        {
            try
            {
                using (var connection = new SqlConnection(FixConnectionStringFormat(connectionString)))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = $"delete from {table} where id = {id}";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                //todo handle error
            }
        }

        public void ReturnToSourceQueue(string connectionString, string errorTable, string sourceQueue, MessageViewModel message)
        {
            try
            {
                var loadedMessage = LoadSingleMessage(connectionString, errorTable, message.Id);
                using (var connection = new SqlConnection(FixConnectionStringFormat(connectionString)))
                {
                    connection.Open();
                    var sql = $"INSERT INTO {sourceQueue} (priority,expiration,visible,headers,body) values (@priority, @expiration, @visible, @headers, @body)";
                    var cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    cmd.Parameters.Add("headers", SqlDbType.VarBinary).Value = loadedMessage.Headers;
                    cmd.Parameters.Add("body", SqlDbType.VarBinary).Value = loadedMessage.Body;
                    cmd.Parameters.Add("expiration", SqlDbType.DateTimeOffset).Value = loadedMessage.Expiration;
                    cmd.Parameters.Add("visible", SqlDbType.DateTimeOffset).Value = loadedMessage.Visible;
                    cmd.Parameters.Add("priority", SqlDbType.Int).Value = loadedMessage.Priority;
                    cmd.ExecuteNonQuery();
                }
                DeleteMessage(connectionString, errorTable, message.Id);
            }
            catch (Exception e)
            {
                //todo handle error
            }
        }

        public void Purge(string connectionString, string tableName)
        {
            try
            {
                using (var connection = new SqlConnection(FixConnectionStringFormat(connectionString)))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = $"delete from {tableName}";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                //todo handle error
            }
        }

        private static string FixConnectionStringFormat(string connectionString)
        {
            return connectionString.Replace("//", "/").Replace(@"\\", @"\");
        }
    }
}
