using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using McMaster.Extensions.CommandLineUtils;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;

namespace RestAPI
{
    [HelpOption("--hlp")]
    [Subcommand(
        typeof(Lista),
        typeof(Adda),
        typeof(Updatea),
        typeof(Deletea),
        typeof(Donea),
        typeof(Undonea),
        typeof(Cleara)
    )]

    class Program
    {
        public static Task<int> Main(string[] args)
        {
            return CommandLineApplication.ExecuteAsync<Program>(args);
        }
    }

    [Command(Description = "Command to Get the Todo List", Name = "list")]
    class Lista
    {

        public async Task OnExecuteAsync()
        {
            var client = new HttpClient();
            var result = await client.GetStringAsync("http://localhost:3000/todo");
            var lisa = JsonConvert.DeserializeObject<List<Todo>>(result);

            foreach (var k in lisa)
            {
                string done = null;
                if (k.Done)
                {
                    done = "(DONE)";
                }
                Console.WriteLine($"{k.id}. {k.Activity} {done}");
            }

        }
    }

    [Command(Description = "Command to Post Tasks", Name = "add")]
    class Adda
    {
        [Argument(0)]
        public string text { get; set; }

        public async Task OnExecuteAsync()
        {
            var client = new HttpClient();
            var Isi = new Todo() { Activity = text, Done = false };
            var data = new StringContent(JsonConvert.SerializeObject(Isi), Encoding.UTF8, "application/json");
            await client.PostAsync("http://localhost:3000/todo", data);
        }
    }

    [Command(Description = "Command to Patch Tasks", Name = "update")]
    class Updatea
    {

        [Argument(0)]
        public string idNum { get; set; }
        [Argument(1)]
        public string text { get; set; }

        public async Task OnExecuteAsync()
        {
            var client = new HttpClient();
            var Isi = new Todo() { id = Convert.ToInt32(idNum), Activity = text };

            var data = new StringContent(JsonConvert.SerializeObject(Isi), Encoding.UTF8, "application/json");
            await client.PatchAsync($"http://localhost:3000/todo/{idNum}", data);
        }
    }

    [Command(Description = "Command to Delete Tasks", Name = "del")]
    class Deletea
    {
        [Argument(0)]
        public string idNum { get; set; }

        public async Task OnExecuteAsync()
        {
            var client = new HttpClient();
            var Isi = new { id = Convert.ToInt32(idNum) };
            await client.DeleteAsync($"http://localhost:3000/todo/{idNum}");
        }
    }

    [Command(Description = "Command to Set Task to Completed", Name = "done")]
    class Donea
    {

        [Argument(0)]
        public string idNum { get; set; }

        public async Task OnExecuteAsync()
        {
            var client = new HttpClient();
            var Isi = new { id = Convert.ToInt32(idNum), done = true };

            var data = new StringContent(JsonConvert.SerializeObject(Isi), Encoding.UTF8, "application/json");
            await client.PatchAsync($"http://localhost:3000/todo/{idNum}", data);
        }
    }

    [Command(Description = "Command to Set Task to Uncompleted", Name = "undone")]
    class Undonea
    {

        [Argument(0)]
        public string idNum { get; set; }

        public async Task OnExecuteAsync()
        {
            var client = new HttpClient();
            var Isi = new { id = Convert.ToInt32(idNum), done = false };

            var data = new StringContent(JsonConvert.SerializeObject(Isi), Encoding.UTF8, "application/json");
            await client.PatchAsync($"http://localhost:3000/todo/{idNum}", data);
        }
    }

    [Command(Description = "Command to Clear all Task", Name = "clear")]
    class Cleara
    {

        public async Task OnExecuteAsync()
        {

            var prompt = Prompt.GetYesNo("Are you sure to clear all task?", false, ConsoleColor.Red);

            var client = new HttpClient();

            if (prompt)
            {
                var listTasks = await client.GetStringAsync("http://localhost:3000/todo");
                var lisa = JsonConvert.DeserializeObject<List<Todo>>(listTasks);
                var lisaID = new List<int>();

                foreach (var k in lisa)
                {
                    lisaID.Add(k.id);
                }
                foreach (var i in lisaID)
                {
                    var result = await client.DeleteAsync($"http://localhost:3000/todo/{i}");
                }
            }
        }
    }

    public class Todo
    {
        public int id { get; set; }
        public string Activity { get; set; }
        public bool Done { get; set; }
    }

    public class TodoList
    {
        public List<Todo> Todolist { get; set; }
    }
}
