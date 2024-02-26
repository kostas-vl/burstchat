namespace BurstChat.Signal.Models;

public class Payload<T>
    where T : notnull
{
    public string SignalGroup { get; set; }

    public T Content { get; set; }

    public Payload(string signalGroup, T content)
    {
        SignalGroup = signalGroup;
        Content = content;
    }
}
