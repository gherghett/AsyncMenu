using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


internal class Program
{
    static private List<Task> tasks = new List<Task>();
    static private List<Progress> progresses = new List<Progress>();
    private static CancellationTokenSource cts = new CancellationTokenSource();
    static private object _lock = new object();
    private static bool update = false; //flag to signal a new Write to screen is needeed
    private const int millisecondsPerFrame = 100;

    private static Task Main(string[] args)
    {
        Write(); 

        var timer = new Stopwatch(); //
        timer.Start();

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
                    update = true;
                    cts.Dispose();
                    cts = new CancellationTokenSource();
                }
            }

            if(update && timer.ElapsedMilliseconds > millisecondsPerFrame)
            {
                Write();
                update = false;
                timer.Restart();
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
        update = true;
    }

    private static async Task Finished(Progress p, Task task)
    {
        await Task.Delay(5000);
        lock (_lock)
        {
            tasks.Remove(task);
            progresses.Remove(p);
        }
        update = true;
    }

    private static async Task AnImportantTask(Progress p, CancellationToken ct)
    {
        var ticks = Random.Shared.Next(10, 100);
        const int tickLength = 50;

        p.total = ticks;
        
        try
        {
            for(int i = 0; i < ticks; i++)
            {
                p.done = i+1;
                ct.ThrowIfCancellationRequested();
                await Task.Delay(tickLength);
                update = true;
            }
        } catch (OperationCanceledException){
            p.canceled = true;
        }
    }

    private static void Write()
    {
        Console.Clear();
        Console.WriteLine("[T] to start new task, [C] to cancel all tasks");
        lock (_lock)
        {
            Console.WriteLine($"tasks {tasks.Count()}              ");
            foreach( var progress in progresses )
            {
                int width = 50;
                double fraction = (double)progress.done / (double)progress.total;
                var sb = new StringBuilder();
                sb.Append('=', (int)(fraction*width));
                sb.Append(' ', width-((int)(fraction*width)));
                Console.Write(sb.ToString());
                Console.WriteLine((progress.canceled ? "Canceled": $"{Math.Round(fraction*100)}%"));
            }
        }
    }
}