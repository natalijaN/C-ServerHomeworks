using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ServerInterfaces;
using ServerPlugins.SqlServer.CommandResponders;

namespace ServerPlugins.SqlServer.CommandResponders
{
    internal class TableObject : ICommandResponder
    {
        public string ConnectionString { get; private set; }
        public string TableName { get; private set; }
        public int ObjectId { get; set; }

        public TableObject(string connectionString, string tableName, int objectId)
        {
            ConnectionString = connectionString;
            TableName = tableName;
            ObjectId = objectId;
        }

        public async Task<Response> GetResponse()
        {
            using (var cnn = new SqlConnection(ConnectionString))
            {
                cnn.Open();
                using (var command = new SqlCommand($@"Select * from {TableName} where Id={ObjectId}", cnn))
                {
                    using (var dr = await command.ExecuteReaderAsync())
                    {
                        var schema = dr.GetColumnSchema();
                        var columnNames = schema.Select(cs => cs.ColumnName);
                        object[] values = new object[columnNames.Count()];

                        var results = new List<Dictionary<string, string>>();

                        while (dr.Read())
                        {
                            var columnCount = dr.GetValues(values);
                            var objectDictionary = new Dictionary<string, string>();
                            foreach (var column in schema)
                            {
                                objectDictionary.Add(column.ColumnName, dr.GetValue(column.ColumnOrdinal.Value).ToString());
                            }
                            results.Add(objectDictionary);
                        }

                        var body = SqlHelpers.GenerateJsonData(results);

                        return new Response
                        {
                            ContentType = ContentTypes.JsonApplication,
                            ResponseCode = ResponseCode.Ok,
                            Type = ResponseType.Text,
                            Body = body
                        };
                    }
                }
            }
        }
    }
}