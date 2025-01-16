using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class Progress
{
    public int done = 0;
    public int total = 0;
    public bool canceled = false;
}
internal class Program
{
    static private object _lock = new object();
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
                    lock (_lock)
                    {
                        progresses.Add(p);
                        AddTask(p, cts.Token);   
                    }
                }
                
                if(key == 'c')
                {
                    cts.Cancel();
                    Write();
                    cts.Dispose();
                    cts = new CancellationTokenSource();
                }
            }
        }
    }

    private static void AddTask(Progress p, CancellationToken ct)
    {
        var task = AnImportantTask(p, ct);
        lock ( _lock )
        {
            tasks.Add(task);
        }
        task.ContinueWith((task) => Finished(p, task));
        Write();
    }

    private static async Task Finished(Progress p, Task task)
    {
        await Task.Delay(5000);
        lock (_lock)
        {
            tasks.Remove(task);
            progresses.Remove(p);
        }
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
                p.done = i+1;
                ct.ThrowIfCancellationRequested();
                await Task.Delay(tickLength);
                Write();
            }
        } catch (OperationCanceledException){
            p.canceled = true;
        }
    }

    private static void Write()
    {
        Console.Clear();
        Console.WriteLine($"tasks {tasks.Count()}              ");
        lock (_lock)
        {
            foreach( var progress in progresses )
            {
                int width = 50;
                double fraction = (double)progress.done / (double)progress.total;
                var sb = new StringBuilder();
                sb.Append('=', (int)(fraction*width));
                sb.Append(' ', width-((int)(fraction*width)));
                Console.Write(sb.ToString());
                Console.WriteLine((progress.canceled ? "Canceled": $"{progress.done/progress.total*100}%"));
            }
        }
    }
}