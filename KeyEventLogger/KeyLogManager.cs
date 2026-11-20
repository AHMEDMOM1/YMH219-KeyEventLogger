using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KeyEventLogger
{
    public class KeyLogManager
    {
        private readonly List<KeyEventRecord> _records = new();
        private readonly Dictionary<string, int> _keyCounts = new(StringComparer.OrdinalIgnoreCase);

        public bool IsLogging { get; private set; }
        public int TotalEvents => _records.Count;
        public IReadOnlyList<KeyEventRecord> Records => _records.AsReadOnly();

        public void Start()
        {
            IsLogging = true;
        }

        public void Stop()
        {
            IsLogging = false;
        }

        public void Clear()
        {
            _records.Clear();
            _keyCounts.Clear();
        }

        public KeyEventRecord? AddEvent(string eventType, Keys keyCode, int keyValue, Keys modifiers)
        {
            if (!IsLogging)
            {
                return null;
            }

            var record = new KeyEventRecord(DateTime.Now, eventType, keyCode, keyValue, modifiers);
            _records.Add(record);

            if (string.Equals(eventType, "KeyDown", StringComparison.OrdinalIgnoreCase))
            {
                _keyCounts[record.KeyName] = _keyCounts.GetValueOrDefault(record.KeyName) + 1;
            }

            return record;
        }

        public string GetTopKeysSummary(int topCount = 3)
        {
            if (_keyCounts.Count == 0)
            {
                return "En çok basılanlar: -";
            }

            var topKeys = _keyCounts
                .OrderByDescending(kvp => kvp.Value)
                .ThenBy(kvp => kvp.Key)
                .Take(topCount)
                .Select(kvp => $"{kvp.Key} ({kvp.Value})");

            return "En çok basılanlar: " + string.Join(", ", topKeys);
        }

        public void ExportToTxt(string filePath)
        {
            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
            writer.WriteLine("=== YMH219 Key Event Logger Kayıtları ===");
            writer.WriteLine($"Oluşturma Tarihi: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            writer.WriteLine($"Toplam Olay Sayısı: {TotalEvents}");
            writer.WriteLine(GetTopKeysSummary());
            writer.WriteLine(new string('-', 60));

            foreach (var record in _records)
            {
                writer.WriteLine(record.ToDisplayLine());
            }
        }
    }
}
