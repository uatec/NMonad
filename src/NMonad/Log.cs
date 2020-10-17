using System;
using Newtonsoft.Json;

namespace NMonad
{
    public class Log
    {
        internal void Info(object p)
        {
            Console.WriteLine(JsonConvert.SerializeObject(p, Formatting.Indented));
        }

        internal void Error(object p)
        {
            Console.WriteLine(JsonConvert.SerializeObject(p,Formatting.Indented));
        }

        internal void Warn(object p)
        {
            Console.WriteLine(JsonConvert.SerializeObject(p,Formatting.Indented));
        }

        internal void Fatal(string v, Exception ex)
        {
            Console.WriteLine(JsonConvert.SerializeObject(v,Formatting.Indented));
        }
    }
}
