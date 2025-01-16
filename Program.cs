using System.Runtime.InteropServices;

internal class Program
{
    static private List<Task> tasks = new List<Task>();
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World! press t for new task");
        while (true)
        {
            if(Console.KeyAvailable && Console.ReadKey(true).KeyChar == 't')
            {
                Console.WriteLine($"tasks {tasks.Count(t => !t.IsCompleted)}");
                AddTask();
            }
        }
    }

    private static void AddTask()
    {
        tasks.Add(AnImportantTask());
        Clear();
    }

    private static async Task AnImportantTask()
    {
        await Task.Delay(Random.Shared.Next(500, 10000));
    }

    private static void Clear()
    {
        Console.CursorTop = Console.CursorTop - 1;
        Console.WriteLine();
        Console.CursorTop = Console.CursorTop - 1;
    }
}