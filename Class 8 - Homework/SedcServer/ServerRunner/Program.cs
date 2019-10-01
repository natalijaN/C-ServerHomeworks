using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using PngResponseGeneratorLib;
using ServerCore;
using ServerInterfaces;
using ServerPlugins;
using ServerPlugins.SqlServer;

namespace ServerRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var server = new WebServer())
            {
                server
                    .UseResponseGenerator<PngResponseGenerator>()
                    .UseResponseGenerator<PostMethodResponseGenerator>()
                    .UseResponsePostProcessor<NotFoundPostProcessor>()
                    .UseResponseGenerator(new SqlServerResponseGenerator("SEDC", "Server=.\\SQLExpress;Database=SEDC;Trusted_Connection=True;"))
                    .UseResponseGenerator(new ErrorResponseGenerator("SEDC", "Server=.\\SQLExpress;Database=SEDC;Trusted_Connection=True;"))
                    .UseResponseGenerator(new StaticResponseGenerator(@"D:\web development\trycode"));

                var result = server.Run();
                result.Wait();
            }
        }
    }
}
