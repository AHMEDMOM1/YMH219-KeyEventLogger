using System;
using System.Windows.Forms;

namespace KeyEventLogger
{
    public class KeyEventRecord
    {
        public DateTime Timestamp { get; }
        public string EventType { get; }
        public string KeyName { get; }
        public int KeyValue { get; }
        public Keys Modifiers { get; }

        public KeyEventRecord(DateTime timestamp, string eventType, Keys keyCode, int keyValue, Keys modifiers)
        {
            Timestamp = timestamp;
            EventType = eventType;
            KeyName = keyCode.ToString();
            KeyValue = keyValue;
            Modifiers = modifiers;
        }

        public string ToDisplayLine()
        {
            string modText = Modifiers != Keys.None ? $" [Mod: {Modifiers}]" : string.Empty;
            return $"[{Timestamp:HH:mm:ss.fff}] {EventType,-7} | Tuş: {KeyName,-12} (Kod: {KeyValue}){modText}";
        }

        public override string ToString() => ToDisplayLine();
    }
}
