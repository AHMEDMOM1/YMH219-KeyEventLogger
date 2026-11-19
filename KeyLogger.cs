namespace KeyEventLogger;

// Holds the recorded events and the recording state. It has no UI code.
public sealed class KeyLogger
{
    private readonly List<KeyLogEntry> _entries = new();
    private readonly Dictionary<string, int> _keyCounts = new();

    public event EventHandler<KeyLogEntry>? EntryRecorded;

    public bool IsRecording { get; private set; }

    public int TotalKeyPresses { get; private set; }

    public int EntryCount => _entries.Count;

    public string? MostUsedKey =>
        _keyCounts.Count == 0 ? null : _keyCounts.MaxBy(pair => pair.Value).Key;

    public void Start() => IsRecording = true;

    public void Stop() => IsRecording = false;

    public void Record(string eventType, Keys keyCode, Keys modifiers)
    {
        if (!IsRecording)
        {
            return;
        }

        string keyName = modifiers == Keys.None
            ? keyCode.ToString()
            : modifiers.ToString().Replace(", ", "+") + "+" + keyCode;

        var entry = new KeyLogEntry(DateTime.Now, eventType, keyName);
        _entries.Add(entry);

        if (eventType == "KeyDown")
        {
            TotalKeyPresses++;
            string codeName = keyCode.ToString();
            _keyCounts[codeName] = _keyCounts.GetValueOrDefault(codeName) + 1;
        }

        EntryRecorded?.Invoke(this, entry);
    }

    public void Clear()
    {
        _entries.Clear();
        _keyCounts.Clear();
        TotalKeyPresses = 0;
    }

    public void SaveToFile(string path) =>
        File.WriteAllLines(path, _entries.Select(entry => entry.ToString()));
}
