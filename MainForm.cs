namespace KeyEventLogger;

// Main window. Key events are captured only while this form is active.
public sealed class MainForm : Form
{
    private readonly KeyLogger _logger = new();

    private readonly ListBox _logList = new()
    {
        Dock = DockStyle.Fill,
        Font = new Font("Consolas", 11f),
        HorizontalScrollbar = true
    };

    private readonly Button _startButton = new() { Text = "Start", AutoSize = true };
    private readonly Button _stopButton = new() { Text = "Stop", AutoSize = true };
    private readonly Button _clearButton = new() { Text = "Clear", AutoSize = true };
    private readonly Button _saveButton = new() { Text = "Save to .txt", AutoSize = true };

    private readonly Label _statusLabel = new()
    {
        AutoSize = true,
        Font = new Font("Segoe UI", 11f, FontStyle.Bold),
        Padding = new Padding(16, 4, 0, 0)
    };

    private readonly Label _statsLabel = new()
    {
        Dock = DockStyle.Bottom,
        Height = 28,
        Padding = new Padding(8, 6, 0, 0)
    };

    public MainForm()
    {
        Text = "Key Event Logger";
        ClientSize = new Size(760, 520);
        MinimumSize = new Size(560, 360);
        KeyPreview = true;

        BuildLayout();
        WireEvents();
        UpdateState();
    }

    private void BuildLayout()
    {
        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            Padding = new Padding(6)
        };
        toolbar.Controls.AddRange(new Control[]
        {
            _startButton, _stopButton, _clearButton, _saveButton, _statusLabel
        });

        // The fill control is added first so that the other panels dock around it.
        Controls.Add(_logList);
        Controls.Add(_statsLabel);
        Controls.Add(toolbar);
    }

    private void WireEvents()
    {
        KeyDown += (_, e) => _logger.Record("KeyDown", e.KeyCode, e.Modifiers);
        KeyUp += (_, e) => _logger.Record("KeyUp", e.KeyCode, e.Modifiers);
        _logger.EntryRecorded += OnEntryRecorded;

        _startButton.Click += (_, _) => { _logger.Start(); UpdateState(); };
        _stopButton.Click += (_, _) => { _logger.Stop(); UpdateState(); };
        _clearButton.Click += (_, _) => ClearLog();
        _saveButton.Click += (_, _) => SaveLog();
    }

    private void OnEntryRecorded(object? sender, KeyLogEntry entry)
    {
        _logList.Items.Add(entry.ToDisplayString());
        _logList.TopIndex = _logList.Items.Count - 1;
        UpdateState();
    }

    private void ClearLog()
    {
        _logger.Clear();
        _logList.Items.Clear();
        UpdateState();
    }

    private void SaveLog()
    {
        if (_logger.EntryCount == 0)
        {
            MessageBox.Show(this, "There are no records to save.", "Save log",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dialog = new SaveFileDialog
        {
            Filter = "Text files (*.txt)|*.txt",
            FileName = "key-events.txt"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            _logger.SaveToFile(dialog.FileName);
            MessageBox.Show(this, "Log saved successfully.", "Save log",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show(this, "The file could not be saved: " + ex.Message, "Save log",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void UpdateState()
    {
        _startButton.Enabled = !_logger.IsRecording;
        _stopButton.Enabled = _logger.IsRecording;
        _saveButton.Enabled = _logger.EntryCount > 0;

        _statusLabel.Text = _logger.IsRecording ? "\u25CF RECORDING" : "\u25CF STOPPED";
        _statusLabel.ForeColor = _logger.IsRecording ? Color.Firebrick : Color.Gray;

        _statsLabel.Text = $"Total key presses: {_logger.TotalKeyPresses}   |   " +
                           $"Most used key: {_logger.MostUsedKey ?? "-"}";

        // Return the focus to the list so that Space or Enter never triggers a button.
        ActiveControl = _logList;
    }
}
