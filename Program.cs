using System.Runtime.InteropServices;

internal class Program
{
    static private List<Task> tasks = new List<Task>();
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World! press t for new task");
        Console.WriteLine($"tasks {tasks.Count()}");
        while (true)
        {
            if(Console.KeyAvailable && Console.ReadKey(true).KeyChar == 't')
            {
                AddTask();
            }
        }
    }

    private static void AddTask()
    {
        var task = AnImportantTask();
        tasks.Add(task);
        task.ContinueWith((task) => Finished(task));
        Clear();
    }

    private static void Finished(Task task)
    {
        tasks.Remove(task);
        Clear();
    }

    private static async Task AnImportantTask()
    {
        await Task.Delay(Random.Shared.Next(50, 1000));
    }

    private static void Clear()
    {
        Console.CursorTop = Console.CursorTop - 1;
        Console.WriteLine($"tasks {tasks.Count()}");
    }
}