using System;
using System.Threading;
using System.Threading.Tasks;

public static class TaskUtil
{
    public static CancellationToken RefreshToken(ref CancellationTokenSource tokenSource)
    {
        tokenSource?.Cancel();
        tokenSource?.Dispose();
        tokenSource = new CancellationTokenSource();
        return tokenSource.Token;
    }

    public static CancellationToken RefreshToken(this CancellationTokenSource tokenSource)
    {
        return RefreshToken(ref tokenSource);
    }

    public static async Task WaitAsync(this Task task, 
        TimeSpan timeout, 
        CancellationToken cancellationToken)
    {
        var delay = Task.Delay(timeout, cancellationToken);
        var completed = await Task.WhenAny(task, delay);
    
        if (completed == delay)
            throw new TimeoutException();
    
        await task; // Propagate exceptions
    }
}
