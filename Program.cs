using System.Runtime.InteropServices;

public class Progress
{
    public int done = 0;
    public int total = 0;
}
internal class Program
{
    static private List<Task> tasks = new List<Task>();
    static private List<Progress> progresses = new List<Progress>();
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
                {
                    var p = new Progress();
                    progresses.Add(p);
                    AddTask(p, cts.Token);
                }
                
                if(key == 'c')
                {
                    cts.Cancel();
                    cts.Dispose();
                    cts = new CancellationTokenSource();
                }
            }
        }
    }

    private static void AddTask(Progress p, CancellationToken ct)
    {
        var task = AnImportantTask(p, ct);
        tasks.Add(task);
        task.ContinueWith((task) => Finished(p, task));
        Write();
    }

    private static void Finished(Progress p, Task task)
    {
        tasks.Remove(task);
        progresses.Remove(p);
        Write();
    }

    private static async Task AnImportantTask(Progress p, CancellationToken ct)
    {
        var ticks = Random.Shared.Next(1, 10);
        const int tickLength = 500;

        p.total = ticks;
        
        try
        {
            for(int i = 0; i < ticks; i++)
            {
                p.done = i;
                ct.ThrowIfCancellationRequested();
                await Task.Delay(tickLength);
            }
        } catch (OperationCanceledException){
            
        }
    }

    private static void Write()
    {
        Console.Clear();
        Console.WriteLine($"tasks {tasks.Count()}              ");
        foreach( var progress in progresses )
        {
            Console.WriteLine($"{progress.done}/{progress.total}");
        }
    }
}