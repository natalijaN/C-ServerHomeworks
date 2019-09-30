using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServerInterfaces;

namespace ServerPlugins.SqlServer.CommandResponders
{
    class TableList: ICommandResponder
    {
        private string ConnectionString { get; set; }

        public TableList(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public async Task<Response> GetResponse()
        {
            using (var cnn = new SqlConnection(ConnectionString))
            {
                try
                {
                    cnn.Open();
                    using (var command = new SqlCommand("select name from sys.objects where type = 'U' and name != 'sysdiagrams'", cnn))
                    {
                        using (var dr = await command.ExecuteReaderAsync())
                        {
                            var tableNames = new List<string>();
                            while (dr.Read())
                            {
                                tableNames.Add(dr.GetString(0));
                            }
                            var tableNamesStr = JsonConvert.SerializeObject(tableNames);
                            var body = $@"Table Names: {tableNamesStr}";

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
                catch (Exception ex)
                {
                    var body = $@"Can not connect to database. Connection string ""{ConnectionString}"" is wrong or Database server is offline!";
                    return new Response
                    {
                        ContentType = ContentTypes.JsonApplication,
                        ResponseCode = ResponseCode.ServiceUnavailable,
                        Type = ResponseType.Text,
                        Body = body
                    };
                    throw new Exception(ex.Message);
                }            
            }
        }
    }
}
