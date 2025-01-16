using System.Runtime.InteropServices;

internal class Program
{
    static private List<Task> tasks = new List<Task>();
    private static CancellationTokenSource cts = new CancellationTokenSource();

    private static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World! press t for new task");
        Console.WriteLine($"tasks {tasks.Count()}");
        while (true)
        {
            if(Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).KeyChar;
                if(key == 't')
                    AddTask(cts.Token);
                
                if(key == 'c')
                {
                    cts.Cancel();
                    cts.Dispose();
                    cts = new CancellationTokenSource();
                }
            }
        }
    }

    private static void AddTask(CancellationToken ct)
    {
        var task = AnImportantTask(ct);
        tasks.Add(task);
        task.ContinueWith((task) => Finished(task));
        Write();
    }

    private static void Finished(Task task)
    {
        tasks.Remove(task);
        Write();
    }

    private static async Task AnImportantTask(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try 
        {
            var ticks = Random.Shared.Next(1, 10);
            const int tickLength = 500;
            
            await Task.Delay(tickLength, ct);

        } catch {}
    }

    private static void Write()
    {
        Console.CursorTop = Console.CursorTop - 1;
        Console.WriteLine("");
        Console.CursorTop = Console.CursorTop - 1;
        Console.WriteLine($"tasks {tasks.Count()}              ");
    }
}