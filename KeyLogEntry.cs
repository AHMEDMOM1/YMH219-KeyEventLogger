namespace KeyEventLogger;

// Immutable description of a single keyboard event.
public sealed record KeyLogEntry(DateTime Timestamp, string EventType, string KeyName)
{
    // Short format used in the on-screen list.
    public string ToDisplayString() =>
        $"{Timestamp:HH:mm:ss.fff} | {EventType,-7} | {KeyName}";

    // Full format used when the log is saved to a text file.
    public override string ToString() =>
        $"{Timestamp:yyyy-MM-dd HH:mm:ss.fff} | {EventType,-7} | {KeyName}";
}
