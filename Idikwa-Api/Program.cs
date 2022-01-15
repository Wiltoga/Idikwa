using Newtonsoft.Json;
using System.Diagnostics;
using System.IO.Pipes;

public class Program
{
    public static void Main(string[] args)
    {
        var pipe = new NamedPipeClientStream("Idikwa-Api");
        try
        {
            pipe.Connect(3000);
        }
        catch (TimeoutException)
        {
            Console.WriteLine("error:Idikwa not started");
            return;
        }
        var reader = new BinaryReader(pipe);
        var writer = new BinaryWriter(pipe);
        try
        {
            writer.Write(JsonConvert.SerializeObject(args));
            Console.WriteLine(reader.ReadString());
        }
        catch (Exception)
        {
            Console.WriteLine("error:Idikwa closed");
        }
        pipe.Dispose();
    }
}