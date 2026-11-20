using System;
using System.Drawing;
using System.Windows.Forms;

namespace KeyEventLogger
{
    public class MainForm : Form
    {
        private readonly KeyLogManager _logManager = new();

        private Label _lblStatus = null!;
        private Button _btnStart = null!;
        private Button _btnStop = null!;
        private Button _btnClear = null!;
        private Button _btnSave = null!;
        private TextBox _txtTestInput = null!;
        private ListBox _lstEvents = null!;
        private Label _lblStats = null!;

        public MainForm()
        {
            InitializeComponents();
            UpdateUiState();
        }

        private void InitializeComponents()
        {
            Text = "YMH219 - Key Event Logger (Yalnızca Uygulama Penceresi)";
            Size = new Size(760, 540);
            MinimumSize = new Size(680, 480);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9.5f);
            KeyPreview = true;

            KeyDown += MainForm_KeyDown;
            KeyUp += MainForm_KeyUp;

            _lblStatus = new Label
            {
                Location = new Point(20, 16),
                Size = new Size(700, 30),
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _btnStart = new Button
            {
                Text = "Başlat",
                Location = new Point(20, 56),
                Size = new Size(110, 36),
                BackColor = Color.FromArgb(34, 139, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnStart.Click += (_, _) =>
            {
                _logManager.Start();
                UpdateUiState();
                _txtTestInput.Focus();
            };

            _btnStop = new Button
            {
                Text = "Durdur",
                Location = new Point(140, 56),
                Size = new Size(110, 36),
                BackColor = Color.FromArgb(192, 57, 43),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnStop.Click += (_, _) =>
            {
                _logManager.Stop();
                UpdateUiState();
            };

            _btnClear = new Button
            {
                Text = "Temizle",
                Location = new Point(260, 56),
                Size = new Size(110, 36),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnClear.Click += (_, _) =>
            {
                _logManager.Clear();
                _lstEvents.Items.Clear();
                _txtTestInput.Clear();
                UpdateStats();
            };

            _btnSave = new Button
            {
                Text = ".txt Kaydet",
                Location = new Point(380, 56),
                Size = new Size(130, 36),
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnSave.Click += BtnSave_Click;

            var lblInput = new Label
            {
                Text = "Test Yazım Alanı (Bu pencere odaktayken yazın):",
                Location = new Point(20, 106),
                Size = new Size(400, 22)
            };

            _txtTestInput = new TextBox
            {
                Location = new Point(20, 130),
                Size = new Size(700, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                PlaceholderText = "Tuş olaylarını test etmek için buraya yazın..."
            };

            var lblList = new Label
            {
                Text = "Yakalanan Tuş Olayları (KeyDown / KeyUp):",
                Location = new Point(20, 170),
                Size = new Size(400, 22)
            };

            _lstEvents = new ListBox
            {
                Location = new Point(20, 195),
                Size = new Size(700, 255),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Consolas", 9.5f),
                BackColor = Color.White
            };

            _lblStats = new Label
            {
                Location = new Point(20, 462),
                Size = new Size(700, 26),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(55, 65, 81),
                Text = "Toplam Olay: 0 | En çok basılanlar: -"
            };

            Controls.AddRange(new Control[]
            {
                _lblStatus, _btnStart, _btnStop, _btnClear, _btnSave,
                lblInput, _txtTestInput, lblList, _lstEvents, _lblStats
            });
        }

        private void MainForm_KeyDown(object? sender, KeyEventArgs e)
        {
            var record = _logManager.AddEvent("KeyDown", e.KeyCode, e.KeyValue, e.Modifiers);
            if (record != null)
            {
                AddRecordToList(record);
            }
        }

        private void MainForm_KeyUp(object? sender, KeyEventArgs e)
        {
            var record = _logManager.AddEvent("KeyUp", e.KeyCode, e.KeyValue, e.Modifiers);
            if (record != null)
            {
                AddRecordToList(record);
            }
        }

        private void AddRecordToList(KeyEventRecord record)
        {
            _lstEvents.Items.Add(record.ToDisplayLine());
            _lstEvents.TopIndex = _lstEvents.Items.Count - 1;
            UpdateStats();
        }

        private void UpdateUiState()
        {
            if (_logManager.IsLogging)
            {
                _lblStatus.Text = "● Kayıt Durumu: AKTİF (Dinleniyor)";
                _lblStatus.ForeColor = Color.FromArgb(22, 163, 74);
            }
            else
            {
                _lblStatus.Text = "● Kayıt Durumu: DURDURULDU";
                _lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
            }

            _btnStart.Enabled = !_logManager.IsLogging;
            _btnStop.Enabled = _logManager.IsLogging;
            UpdateStats();
        }

        private void UpdateStats()
        {
            _lblStats.Text = $"Toplam Olay: {_logManager.TotalEvents} | {_logManager.GetTopKeysSummary()}";
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "Metin Dosyası (*.txt)|*.txt",
                FileName = $"key_events_{DateTime.Now:yyyyMMdd_HHmmss}.txt",
                Title = "Tuş Olayı Kayıtlarını Kaydet"
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                _logManager.ExportToTxt(dialog.FileName);
                MessageBox.Show(
                    "Kayıtlar başarıyla .txt dosyasına aktarıldı.",
                    "Bilgi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
    }
}
