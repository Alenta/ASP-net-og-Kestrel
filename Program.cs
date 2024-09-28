using System.IO;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Text.Json.Nodes;
namespace Oppgave;
internal class Program
{
    private static void Main(string[] args)
    {
        //Set up builder for webapp
        var builder = WebApplication.CreateBuilder(args);
        //Build the builder
        var app = builder.Build();
        UpdateCatfacts();

        //Middleware - UseDefault: enables default filemapping through wwwroot folder
        app.UseDefaultFiles();
        //Use static allows server to get associated files through the wwwroot folder
        app.UseStaticFiles();

        //API-endpoint to check server health
        app.MapGet("/health", () => "Server status: OK");
        app.MapGet("/catfact.txt", () => {
            string txt = File.ReadAllText("catfact.txt");
            return txt;
        });
        app.MapGet("/newfact", () =>{
            UpdateCatfacts();
        });

        //Start the server
        app.Run();
    }
    public static async void UpdateCatfacts(){
        await APIGet();
    }
    static readonly HttpClient client = new();
    public static async Task APIGet(){
        Serializer serializer = new();
        try
        {
            Catfact? catfact = await client.GetFromJsonAsync<Catfact>("https://cat-fact.herokuapp.com/facts/random?animal_type=cat&amount=1");

            Console.WriteLine("Response from cat-fact.herokuapp.com:");
            if(catfact != null){
                string[] catfacts = {catfact.Text};
                serializer.WriteToFile("catfact.txt",catfacts);
            }
            
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
        }

    }
}

public class Catfact()
{
    public string? Text {get; set;}
}
public class Serializer: ISerializer{
    public bool WriteToFile(string path, string[] textToWrite){
        try 
        {
            //Pass the file path and file name to the StreamWriter constructor
            StreamWriter sw = new StreamWriter(path);
            //Foreach item in texttowrite
            foreach (var item in textToWrite)
            {
                //Writeline the item to the textfile
                sw.WriteLine(item);
                Console.WriteLine($"Wrote {item} to {sw}");

            }
            //Close the stream
            sw.Close();
        }
        catch(Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
            return false;
        }
        return true;
    }

}

interface ISerializer{
    bool WriteToFile(string path, string[] text);

}