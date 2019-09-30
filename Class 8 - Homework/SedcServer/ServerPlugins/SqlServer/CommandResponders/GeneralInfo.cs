using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using ServerInterfaces;

namespace ServerPlugins.SqlServer.CommandResponders
{
    public class GeneralInfo: ICommandResponder
    {
        private string ConnectionString { get; set; }

        public GeneralInfo(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public async Task<Response> GetResponse()
        {
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                try
                {
                    cnn.Open();
                    var command = new SqlCommand("select @@version", cnn);
                    var result = (await command.ExecuteScalarAsync()).ToString();
                    var body = $@"{{""version"":""{result}""}}";
                    return new Response
                    {
                        ContentType = ContentTypes.JsonApplication,
                        ResponseCode = ResponseCode.Ok,
                        Type = ResponseType.Text,
                        Body = body
                    };
                }
                catch   (Exception ex)
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
