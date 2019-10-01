using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerInterfaces;

namespace ServerPlugins
{
    public class StaticResponseGenerator : IResponseGenerator
    {
        public int Count { get; }
        public string FolderName { get; private set; }
        public string FolderPath { get; private set; }

        public StaticResponseGenerator (string folderName)
        {
            FolderName = Path.GetFileName(folderName);
            FolderPath = Path.GetFullPath(folderName);
        }

        public async Task<Response> Generate(Request request, ILogger logger)
        {
            var path = string.Join(Path.DirectorySeparatorChar, request.Path.Split("/").Skip(2));
            var fullPath = Path.Combine(FolderPath, path);
          
            if (!File.Exists(fullPath))
            {
                var defaultHtml = $@"<div>
 <h2>Default Page</h2>
<div>Your file is not found in the folder - ""{FolderName}""</div>
</div>";
                byte[] defaultBytes = Encoding.ASCII.GetBytes(defaultHtml);
                return new Response
                {
                    Bytes = defaultBytes,
                    Type = ResponseType.Binary,
                    ContentType = ContentTypes.GetContentType(fullPath)
                };
            } else
            {
                var bytes = await File.ReadAllBytesAsync(fullPath);
                var footer = $"<div style='bottom: 0;position: absolute;'>Total size of the file is: {bytes.Count()} bytes.</div>";
                byte[] footerBytes = Encoding.ASCII.GetBytes(footer);
                var completeBytes = Combine(bytes, footerBytes);

                return new Response
                {
                    Bytes = completeBytes,
                    Type = ResponseType.Binary,
                    ContentType = ContentTypes.GetContentType(fullPath)

                };
            }
        }

        public bool IsInterested(Request request, ILogger logger)
        {
            var path = $"static/{FolderName}";
            return request.Path.StartsWith(path, StringComparison.InvariantCultureIgnoreCase);
        }

        public static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] combinedArrays = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, combinedArrays, 0, first.Length);
            Buffer.BlockCopy(second, 0, combinedArrays, first.Length, second.Length);
            return combinedArrays;
        }
    }
}
