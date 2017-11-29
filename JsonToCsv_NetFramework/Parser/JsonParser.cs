using System;
//using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace JsonToCsv.Parser
{

    public class JsonParser
    {

        /// <summary>
        /// Parses a json array of type T to a 
        /// .NET List<T>
        /// </summary>
        /// <typeparam name="T">Typeof record</typeparam>
        /// <param name="bytes">Bytestream of data</param>
        /// <returns></returns>
        public static List<T> Parse<T>(Stream bytes)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var resName = args.Name + ".dll";
                var thisAssembly = Assembly.GetExecutingAssembly();
                using (var input = thisAssembly.GetManifestResourceStream(resName))
                {
                    return input != null
                         ? Assembly.Load(StreamToBytes(input))
                         : null;
                }
            };
            var serialiser = new Newtonsoft.Json.JsonSerializer();

            using (var sr = new StreamReader(bytes))
            using (var jsonReader = new Newtonsoft.Json.JsonTextReader(sr))
            {
                return serialiser.Deserialize<List<T>>(jsonReader);
            }
        }


        static byte[] StreamToBytes(Stream input)
        {
            var capacity = input.CanSeek ? (int)input.Length : 0;
            using (var output = new MemoryStream(capacity))
            {
                int readLength;
                var buffer = new byte[4096];

                do
                {
                    readLength = input.Read(buffer, 0, buffer.Length);
                    output.Write(buffer, 0, readLength);
                }
                while (readLength != 0);

                return output.ToArray();
            }
        }
    }
}
