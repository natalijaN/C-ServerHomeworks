using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ServerInterfaces;
using ServerPlugins.SqlServer.CommandResponders;

namespace ServerPlugins
{
    public class ErrorResponseGenerator : IResponseGenerator
    {
        public int Count { get; }

        public string DatabaseName { get; private set; }
        public string ConnectionString { get; private set; }

        public ErrorResponseGenerator(string databaseName, string connectionString)
        {
            DatabaseName = databaseName;
            ConnectionString = connectionString;
        }

        public async Task<Response> Generate(Request request, ILogger logger)
        {
            var response = await GetResponse();
            return response;
        }

        private async Task<Response> GetResponse( )
        {
            ICommandResponder responder = new Error();
            return await responder.GetResponse();
        }

        public bool IsInterested(Request request, ILogger logger)
        {
            var regex = new Regex($@"^sql\/{DatabaseName.ToLower()}*\/?[tables]*?$");
            return regex.IsMatch(request.Path) ? false : true;
        }
    }
}
