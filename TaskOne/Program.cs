using Microsoft.Extensions.Configuration;
using TaskOne;

var configuration = new ConfigurationBuilder()
     .SetBasePath(Directory.GetCurrentDirectory())
     .AddJsonFile($"appsettings.json").Build();

DirectoryWorkClass directoryWorkClass = new DirectoryWorkClass()
{
    SourceFolder = configuration.GetSection("path:folderA").Value,
    ResultFolder = configuration.GetSection("path:folderB").Value
};
MainFunction mainFunction = new MainFunction() { directoryWorkClass = directoryWorkClass };

CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
CancellationToken token = cancelTokenSource.Token;

Task task = new Task(() =>
{
    if (token.IsCancellationRequested)
        token.ThrowIfCancellationRequested();
    else
    {
        mainFunction.Start(ref cancelTokenSource);
    }

}, token);

string stateProgramm = "waiting start";
while (true)
{
    Console.Clear();
    Console.WriteLine("Choose an option:");
    Console.WriteLine("1) start");
    Console.WriteLine("2) stop");
    Console.WriteLine("3) reset");
    Console.WriteLine("Program status: " + stateProgramm);
    Console.Write("\r\nSelect an option: ");

    switch (Console.ReadLine())
    {
        case "1":
            if (task.Status == TaskStatus.Canceled)
            {
                Console.WriteLine("Program was stopped recently. Press Enter and then Reset program");
                Console.ReadLine();
                break;
            }
            else if (task.Status == TaskStatus.Running)
            {
                Console.WriteLine("Program has already ran. You can stop and reset. Press Enter and select option");
                Console.ReadLine();
                break;
            }
            else if (task.Status == TaskStatus.RanToCompletion)
            {
                ReCreateTask();
                task.Start();
                stateProgramm = "start";
                break;
            }
            else
            {
                task.Start();
                stateProgramm = "start";
                break;
            }
        case "2":
            cancelTokenSource.Cancel();
            stateProgramm = "stop";
            break;
        case "3":
            if (task.Status == TaskStatus.Running)
            {
                Console.WriteLine("Before Reset you should stop program. Press Enter and then Stop program");
                Console.ReadLine();
            }
            else
            {
                ReCreateTask();
                task.Start();
                stateProgramm = "reset";
            }
            break;
        default:
            break;
    }
}
void ReCreateTask()
{
    cancelTokenSource = new CancellationTokenSource();
    token = cancelTokenSource.Token;
    task = new Task(() =>
    {
        if (token.IsCancellationRequested)
            token.ThrowIfCancellationRequested();
        else
        {
            mainFunction.Start(ref cancelTokenSource);
        }

    }, token);
}