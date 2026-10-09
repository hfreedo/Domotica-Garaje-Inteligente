using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using QRCoder;

namespace PanelGaraje;

public sealed class MainForm : Form
{
    private readonly TextBox _portBox = new() { Text = "8773" };
    private readonly TextBox _pinBox = new() { ReadOnly = true };
    private readonly TextBox _localUrlBox = new() { ReadOnly = true };
    private readonly TextBox _lanUrlBox = new() { ReadOnly = true };
    private readonly TextBox _ngrokUrlBox = new() { ReadOnly = true };
    private readonly TextBox _ngrokTokenBox = new() { UseSystemPasswordChar = true };
    private readonly TextBox _logBox = new() { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical };
    private readonly TextBox _demoLogBox = new() { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical };
    private readonly PictureBox _lanQr = new() { SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle };
    private readonly PictureBox _ngrokQr = new() { SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle };
    private readonly Label _statusLabel = new() { Text = "Servidor detenido", AutoSize = true };
    private readonly Label _scrollHintLabel = new() { Text = "Desplace esta columna para ver registro y controles online", AutoSize = false };

    private Process? _serverProcess;
    private Process? _ngrokProcess;
    private string _projectRoot = "";
    private string _pin = "";

    public MainForm()
    {
        Text = "Panel del Garaje";
        Size = new Size(1200, 780);
        MinimumSize = new Size(1050, 700);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9F);
        Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) ?? Icon;

        _projectRoot = FindProjectRoot();
        _pin = GeneratePin();
        _pinBox.Text = _pin;

        BuildLayout();
        RefreshUrls();
        Log("Panel del Garaje listo.");
        Log($"Proyecto: {_projectRoot}");
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        StopNgrok();
        StopServer();
        base.OnFormClosing(e);
    }

    private void BuildLayout()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(14),
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
        Controls.Add(root);

        var leftShell = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(0, 0, 12, 0),
        };
        leftShell.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        leftShell.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        leftShell.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        root.Controls.Add(leftShell, 0, 0);

        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 1,
            RowCount = 2,
            Margin = new Padding(0, 0, 0, 8),
        };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        var title = new Label
        {
            Text = "Panel del Garaje",
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 2),
        };
        var subtitle = new Label
        {
            Text = "Servidor local, acceso remoto y registro operativo",
            AutoSize = true,
            ForeColor = Color.DimGray,
            Margin = new Padding(2, 0, 0, 0),
        };
        header.Controls.Add(title, 0, 0);
        header.Controls.Add(subtitle, 0, 1);
        leftShell.Controls.Add(header, 0, 0);

        var tabs = new TabControl
        {
            Dock = DockStyle.Fill,
            Padding = new Point(12, 6),
            HotTrack = true,
        };
        leftShell.Controls.Add(tabs, 0, 1);

        var localTab = new TabPage("Modo local") { Padding = new Padding(8) };
        var demoTab = new TabPage("Modo demo") { Padding = new Padding(8) };
        tabs.TabPages.Add(localTab);
        tabs.TabPages.Add(demoTab);

        var localContent = ScrollableStack();
        localTab.Controls.Add(localContent.Scroll);
        localContent.Stack.Controls.Add(AutoGroup("Servidor", ServerPanel()));
        localContent.Stack.Controls.Add(AutoGroup("Acceso LAN / Hotspot", LanPanel()));
        localContent.Stack.Controls.Add(AutoGroup("Acceso online con Ngrok", NgrokPanel()));
        localContent.Stack.Controls.Add(AutoGroup("Registro", LogPanel()));

        var demoContent = ScrollableStack();
        demoTab.Controls.Add(demoContent.Scroll);
        demoContent.Stack.Controls.Add(AutoGroup("Prueba sin Arduino", DemoPanel()));
        demoContent.Stack.Controls.Add(AutoGroup("Registro", DemoLogPanel()));

        _scrollHintLabel.Dock = DockStyle.Fill;
        _scrollHintLabel.TextAlign = ContentAlignment.MiddleLeft;
        _scrollHintLabel.ForeColor = Color.FromArgb(72, 72, 72);
        _scrollHintLabel.BackColor = Color.FromArgb(244, 247, 248);
        _scrollHintLabel.Padding = new Padding(10, 0, 0, 0);
        leftShell.Controls.Add(_scrollHintLabel, 0, 2);

        var right = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(0),
        };
        right.RowStyles.Add(new RowStyle(SizeType.Percent, 52));
        right.RowStyles.Add(new RowStyle(SizeType.Percent, 48));
        root.Controls.Add(right, 1, 0);

        right.Controls.Add(QrPanel("QR LAN / Hotspot", _lanQr, _lanUrlBox, CopyLan, OpenLan), 0, 0);
        right.Controls.Add(QrPanel("QR Online / Ngrok", _ngrokQr, _ngrokUrlBox, CopyNgrok, OpenNgrok), 0, 1);
        SetQrPlaceholder(_ngrokQr, "Online pendiente");
    }

    private static (Panel Scroll, TableLayoutPanel Stack) ScrollableStack()
    {
        var scroll = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            AutoScrollMargin = new Size(0, 18),
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(8, 8, 10, 8),
        };
        var stack = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
        };
        stack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        scroll.Controls.Add(stack);
        return (scroll, stack);
    }

    private Control ServerPanel()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            ColumnCount = 2,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(2, 6, 2, 2),
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 96));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        panel.Controls.Add(new Label { Text = "Puerto", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        _portBox.Dock = DockStyle.Fill;
        panel.Controls.Add(_portBox, 1, 0);

        panel.Controls.Add(new Label { Text = "PIN", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
        _pinBox.Dock = DockStyle.Fill;
        panel.Controls.Add(_pinBox, 1, 1);

        _statusLabel.AutoSize = false;
        _statusLabel.Dock = DockStyle.Fill;
        _statusLabel.Height = 28;
        _statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        _statusLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        _statusLabel.Margin = new Padding(4, 10, 4, 4);
        panel.Controls.Add(_statusLabel, 0, 2);
        panel.SetColumnSpan(_statusLabel, 2);

        var buttons = ButtonGrid(
            ("Iniciar local", async (_, _) => await StartServer(false)),
            ("Iniciar LAN / Hotspot", async (_, _) => await StartServer(true)),
            ("Detener servidor", (_, _) => StopServer()),
            ("Regenerar PIN", (_, _) => RegeneratePin()),
            ("Copiar PIN", (_, _) => Clipboard.SetText(_pinBox.Text)),
            ("Abrir local", (_, _) => OpenUrl(_localUrlBox.Text))
        );
        panel.Controls.Add(buttons, 0, 3);
        panel.SetColumnSpan(buttons, 2);
        return panel;
    }

    private Control LanPanel()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            ColumnCount = 2,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(2, 6, 2, 2),
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 96));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        panel.Controls.Add(new Label { Text = "URL local", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        _localUrlBox.Dock = DockStyle.Fill;
        panel.Controls.Add(_localUrlBox, 1, 0);
        panel.Controls.Add(new Label { Text = "URL LAN", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
        _lanUrlBox.Dock = DockStyle.Fill;
        panel.Controls.Add(_lanUrlBox, 1, 1);
        var note = new Label
        {
            Text = "Use LAN/Hotspot cuando el celular y la PC estén en la misma red. El celular pedirá el PIN temporal mostrado arriba.",
            Dock = DockStyle.Top,
            AutoSize = true,
            MaximumSize = new Size(520, 0),
            ForeColor = Color.DimGray,
            Margin = new Padding(4, 8, 4, 0),
        };
        panel.Controls.Add(note, 0, 2);
        panel.SetColumnSpan(note, 2);
        return panel;
    }

    private Control NgrokPanel()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            ColumnCount = 2,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(2, 6, 2, 2),
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 96));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        panel.Controls.Add(new Label { Text = "Authtoken", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        _ngrokTokenBox.Dock = DockStyle.Fill;
        panel.Controls.Add(_ngrokTokenBox, 1, 0);
        var buttons = ButtonGrid(
            ("Guardar token", (_, _) => ConfigureNgrokToken()),
            ("Publicar online", (_, _) => StartNgrok()),
            ("Detener Ngrok", (_, _) => StopNgrok()),
            ("Buscar ngrok", (_, _) => Log(FindNgrok() ?? "No se encontró ngrok.exe."))
        );
        panel.Controls.Add(buttons, 0, 1);
        panel.SetColumnSpan(buttons, 2);
        return panel;
    }

    private Control LogPanel()
    {
        _logBox.Height = 210;
        _logBox.Dock = DockStyle.Fill;
        _logBox.Font = new Font("Consolas", 9F);
        return _logBox;
    }

    private Control DemoPanel()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            ColumnCount = 1,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(2, 6, 2, 2),
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        var note = new Label
        {
            Text = "Use este modo para presentar o revisar la interfaz sin Arduino conectado. El panel inicia el servidor local, activa el modo demo en el dashboard y abre la interfaz.",
            Dock = DockStyle.Top,
            AutoSize = true,
            MaximumSize = new Size(520, 0),
            ForeColor = Color.DimGray,
            Margin = new Padding(4, 0, 4, 8),
        };
        panel.Controls.Add(note, 0, 0);

        var buttons = ButtonGrid(
            ("Iniciar demo", async (_, _) => await StartDemoMode()),
            ("Detener demo", async (_, _) => await SetDemoMode(false)),
            ("Abrir dashboard", (_, _) => OpenUrl(_localUrlBox.Text)),
            ("Detener servidor", (_, _) => StopServer())
        );
        panel.Controls.Add(buttons, 0, 1);
        return panel;
    }

    private Control DemoLogPanel()
    {
        _demoLogBox.Height = 330;
        _demoLogBox.Dock = DockStyle.Fill;
        _demoLogBox.Font = new Font("Consolas", 9F);
        return _demoLogBox;
    }

    private Control QrPanel(string title, PictureBox qr, TextBox urlBox, Action copyAction, Action openAction)
    {
        var box = FillGroup(title, new Panel());
        var panel = (Panel)box.Controls[0];
        panel.Dock = DockStyle.Fill;

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        panel.Controls.Add(layout);

        qr.Dock = DockStyle.Fill;
        qr.MinimumSize = new Size(240, 220);
        qr.BackColor = Color.White;
        layout.Controls.Add(qr, 0, 0);
        urlBox.Dock = DockStyle.Fill;
        urlBox.Margin = new Padding(4, 6, 4, 4);
        layout.Controls.Add(urlBox, 0, 1);

        var buttons = ButtonGrid(
            ("Copiar URL", (_, _) => copyAction()),
            ("Abrir", (_, _) => openAction())
        );
        buttons.Margin = new Padding(0, 2, 0, 0);
        layout.Controls.Add(buttons, 0, 2);
        return box;
    }

    private static GroupBox AutoGroup(string title, Control content)
    {
        var group = new GroupBox
        {
            Text = title,
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(12, 20, 12, 12),
            Margin = new Padding(0, 0, 0, 12),
        };
        content.Dock = DockStyle.Top;
        group.Controls.Add(content);
        return group;
    }

    private static GroupBox FillGroup(string title, Control content)
    {
        var group = new GroupBox
        {
            Text = title,
            Dock = DockStyle.Fill,
            Padding = new Padding(12, 20, 12, 12),
            Margin = new Padding(0, 0, 0, 12),
        };
        content.Dock = DockStyle.Fill;
        group.Controls.Add(content);
        return group;
    }

    private static TableLayoutPanel ButtonGrid(params (string Text, EventHandler Handler)[] items)
    {
        var panel = new TableLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Dock = DockStyle.Top,
            ColumnCount = 2,
            Margin = new Padding(0, 8, 0, 0),
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        var rows = (int)Math.Ceiling(items.Length / 2.0);
        panel.RowCount = rows;
        for (var row = 0; row < rows; row++)
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));

        for (var i = 0; i < items.Length; i++)
        {
            var button = Button(items[i].Text, items[i].Handler);
            panel.Controls.Add(button, i % 2, i / 2);
        }
        return panel;
    }

    private static Button Button(string text, EventHandler onClick)
    {
        var button = new Button
        {
            Text = text,
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            MinimumSize = new Size(0, 36),
            Margin = new Padding(4),
            TextAlign = ContentAlignment.MiddleCenter,
        };
        button.Click += onClick;
        return button;
    }

    private async Task StartServer(bool lanMode)
    {
        if (_serverProcess is { HasExited: false })
        {
            Log("El servidor ya esta activo.");
            return;
        }

        if (!int.TryParse(_portBox.Text, out var port) || port <= 0 || port > 65535)
        {
            MessageBox.Show("Puerto invalido.", "Panel del Garaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var backendExe = FindBackendExecutable();
        var serverPath = Path.Combine(_projectRoot, "interfaz_garaje", "server.py");
        if (backendExe is null && !File.Exists(serverPath))
        {
            MessageBox.Show($"No se encontró GarajeServidor.exe ni server.py en {serverPath}", "Panel del Garaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var host = lanMode ? "0.0.0.0" : "127.0.0.1";
        var args = $"--host {host} --port {port} --no-browser";
        ProcessStartInfo psi;
        if (backendExe is not null)
        {
            psi = new ProcessStartInfo(backendExe, args)
            {
                WorkingDirectory = Path.GetDirectoryName(backendExe)!,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };
        }
        else
        {
            var venvPython = Path.Combine(_projectRoot, ".venv", "Scripts", "python.exe");
            psi = new ProcessStartInfo(File.Exists(venvPython) ? venvPython : "python", $"\"{serverPath}\" {args}")
            {
                WorkingDirectory = Path.GetDirectoryName(serverPath)!,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };
        }

        try
        {
            Log($"Backend: {(backendExe is not null ? backendExe : serverPath)}");
            Log($"Host: {host} | Puerto: {port}");
            using (var listener = new TcpListener(IPAddress.Loopback, port))
            {
                try { listener.Start(); } catch { throw new InvalidOperationException("Puerto ocupado. Elija otro puerto; no se reutiliza un servidor ajeno."); }
                finally { listener.Stop(); }
            }
            psi.Environment["GARAGE_PIN"] = _pin;
            _serverProcess = StartLoggedProcess(psi, "servidor");
            _portBox.ReadOnly = true;
            var ready = false;
            using (var health = new HttpClient { Timeout = TimeSpan.FromSeconds(1) })
            {
                for (var attempt=0; attempt<30; attempt++)
                {
                    if (_serverProcess is null || _serverProcess.HasExited) break;
                    try {
                        var response = await health.GetStringAsync($"http://127.0.0.1:{port}/health");
                        using var info=JsonDocument.Parse(response);
                        if (info.RootElement.GetProperty("service").GetString()=="garage") {ready=true;break;}
                    } catch { }
                    await Task.Delay(300);
                }
            }
            if (!ready) { StopServer(); throw new InvalidOperationException("El servidor no confirmó que está listo. Revise el registro."); }
            SetStatus(lanMode ? "Servidor LAN activo" : "Servidor local activo", Color.SeaGreen);
            RefreshUrls();
            Log(lanMode ? "Servidor iniciado en modo LAN/Hotspot." : "Servidor iniciado en modo local.");
            Log($"URL local sin PIN: {_localUrlBox.Text}");
            Log($"URL LAN: {_lanUrlBox.Text}");
            Log("Acceso local permitido sin PIN. Acceso desde otros equipos requiere el PIN mostrado en el panel.");
        }
        catch (Exception ex)
        {
            SetStatus("Error al iniciar servidor", Color.Firebrick);
            Log($"Error al iniciar servidor: {ex.Message}");
            MessageBox.Show(ex.Message, "No se pudo iniciar el servidor", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void StopServer()
    {
        StopNgrok();
        _portBox.ReadOnly = false;
        if (_serverProcess is null || _serverProcess.HasExited)
        {
            _serverProcess = null;
            SetStatus("Servidor detenido", Color.Firebrick);
            Log("No hay servidor activo para detener.");
            return;
        }
        StopProcess(_serverProcess, "servidor");
        _serverProcess = null;
        SetStatus("Servidor detenido", Color.Firebrick);
    }

    private void ConfigureNgrokToken()
    {
        var token = _ngrokTokenBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(token))
        {
            Log("Authtoken Ngrok vacío. Se omitió la configuración.");
            return;
        }
        var ngrok = FindNgrok();
        if (ngrok is null)
        {
            Log("No se encontró ngrok.exe. No se puede configurar el authtoken.");
            MessageBox.Show("No se encontró ngrok.exe. Colóquelo en support/ngrok/ngrok.exe o agréguelo al PATH.", "Ngrok", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        var psi = new ProcessStartInfo(ngrok)
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };
        psi.ArgumentList.Add("config"); psi.ArgumentList.Add("add-authtoken"); psi.ArgumentList.Add(token);
        using var process = Process.Start(psi);
        if (process is null || !process.WaitForExit(8000)) { Log("Ngrok no confirmó la configuración."); return; }
        _ngrokTokenBox.Clear();
        Log(process.ExitCode == 0 ? "Authtoken Ngrok configurado." : "No se pudo configurar el authtoken Ngrok.");
    }

    private async void StartNgrok()
    {
        if (_serverProcess is not { HasExited: false })
        {
            await StartServer(false);
            await Task.Delay(800);
        }

        if (_ngrokProcess is { HasExited: false })
        {
            Log("Ngrok ya está activo.");
            return;
        }

        var ngrok = FindNgrok();
        if (ngrok is null)
        {
            Log("No se encontró ngrok.exe. Colóquelo en support/ngrok/ngrok.exe o agréguelo al PATH.");
            MessageBox.Show("No se encontró ngrok.exe. Descárguelo desde ngrok.com y colóquelo en support/ngrok/ngrok.exe.", "Ngrok", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_serverProcess is null || _serverProcess.HasExited) { Log("Servidor no disponible."); return; }
        var port = int.Parse(_portBox.Text);
        var psi = new ProcessStartInfo(ngrok, $"http http://127.0.0.1:{port} --log=stdout")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };

        _ngrokProcess = StartLoggedProcess(psi, "ngrok");
        Log("Esperando URL pública de Ngrok...");
        var publicUrl = await WaitForNgrokUrl();
        if (!string.IsNullOrWhiteSpace(publicUrl))
        {
            _ngrokUrlBox.Text = publicUrl;
            RenderQr(_ngrokUrlBox.Text, _ngrokQr);
            Log($"Ngrok publicado: {_ngrokUrlBox.Text}");
            Log("El acceso online solicitará el PIN temporal mostrado en el panel.");
        }
        else
        {
            Log("No se pudo detectar la URL de Ngrok. Revise el registro.");
        }
    }

    private void StopNgrok()
    {
        StopProcess(_ngrokProcess, "ngrok");
        _ngrokProcess = null;
        _ngrokUrlBox.Text = "";
        SetQrPlaceholder(_ngrokQr, "Online pendiente");
    }

    private Process StartLoggedProcess(ProcessStartInfo psi, string name)
    {
        var process = new Process { StartInfo = psi, EnableRaisingEvents = true };
        process.OutputDataReceived += (_, e) => { if (name != "ngrok" && !string.IsNullOrWhiteSpace(e.Data)) Log($"[{name}] {e.Data}"); };
        process.ErrorDataReceived += (_, e) => { if (!string.IsNullOrWhiteSpace(e.Data)) Log($"[{name}] {e.Data}"); };
        process.Exited += (_, _) =>
        {
            var exitText = "";
            try { exitText = $" Código de salida: {process.ExitCode}."; } catch { }
            Log($"[{name}] proceso finalizado.{exitText}");
            if (name == "servidor" && ReferenceEquals(_serverProcess, process))
            {
                _serverProcess = null;
                SetStatus("Servidor detenido", Color.Firebrick);
            }
            if (name == "ngrok" && ReferenceEquals(_ngrokProcess, process))
            {
                _ngrokProcess = null;
            }
        };
        Log($"Iniciando {name}...");
        if (!process.Start()) throw new InvalidOperationException($"No se pudo iniciar {name}.");
        Log($"[{name}] PID {process.Id} iniciado.");
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        return process;
    }

    private void StopProcess(Process? process, string name)
    {
        if (process is null) return;
        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
                process.WaitForExit(3000);
                Log($"{name} detenido.");
            }
        }
        catch (Exception ex)
        {
            Log($"No se pudo detener {name}: {ex.Message}");
        }
    }

    private async Task<string?> WaitForNgrokUrl()
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
        for (var i = 0; i < 20; i++)
        {
            await Task.Delay(500);
            try
            {
                var json = await client.GetStringAsync("http://127.0.0.1:4040/api/tunnels");
                using var doc = JsonDocument.Parse(json);
                foreach (var tunnel in doc.RootElement.GetProperty("tunnels").EnumerateArray())
                {
                    if (!tunnel.TryGetProperty("config", out var config) || !config.TryGetProperty("addr", out var address) ||
                        address.GetString()?.TrimEnd('/') != $"http://127.0.0.1:{_portBox.Text}") continue;
                    if (tunnel.TryGetProperty("public_url", out var urlElement))
                    {
                        var url = urlElement.GetString();
                        if (!string.IsNullOrWhiteSpace(url) && url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                            return url;
                    }
                }
            }
            catch
            {
                // Ngrok puede tardar unos segundos en levantar su API local.
            }
        }
        return null;
    }

    private void RefreshUrls()
    {
        var port = int.TryParse(_portBox.Text, out var parsed) ? parsed : 8773;
        _localUrlBox.Text = $"http://127.0.0.1:{port}";
        var ip = GetLanAddress() ?? "IP-DE-LA-PC";
        _lanUrlBox.Text = $"http://{ip}:{port}";
        RenderQr(_lanUrlBox.Text, _lanQr);
    }

    private async Task StartDemoMode()
    {
        if (_serverProcess is not { HasExited: false })
        {
            await StartServer(false);
            await Task.Delay(900);
        }

        if (_serverProcess is not { HasExited: false })
        {
            Log("No se pudo activar demo porque el servidor no está activo.");
            return;
        }

        await SetDemoMode(true);
        OpenUrl(_localUrlBox.Text);
    }

    private async Task SetDemoMode(bool enabled)
    {
        RefreshUrls();
        var endpoint = $"{_localUrlBox.Text.TrimEnd('/')}/api/demo";
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
            client.DefaultRequestHeaders.Add("X-Garage-Request", "1");
            using var content = new StringContent($"{{\"enabled\":{enabled.ToString().ToLowerInvariant()}}}", Encoding.UTF8, "application/json");
            using var response = await client.PostAsync(endpoint, content);
            if (response.IsSuccessStatusCode)
            {
                Log(enabled ? "Modo demo activado." : "Modo demo desactivado.");
            }
            else
            {
                Log($"No se pudo cambiar el modo demo. HTTP {(int)response.StatusCode}.");
            }
        }
        catch (Exception ex)
        {
            Log($"Error al cambiar modo demo: {ex.Message}");
        }
    }

    private void RegeneratePin()
    {
        if (_serverProcess is { HasExited: false })
        {
            Log("No se regeneró el PIN porque el servidor está activo. Detenga el servidor y vuelva a intentarlo.");
            MessageBox.Show("Detenga el servidor antes de regenerar el PIN. Así evitamos que el panel muestre un PIN distinto al que está usando el backend.", "Panel del Garaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        _pin = GeneratePin();
        _pinBox.Text = _pin;
        RefreshUrls();
        if (!string.IsNullOrWhiteSpace(_ngrokUrlBox.Text))
            RenderQr(_ngrokUrlBox.Text, _ngrokQr);
        Log("PIN regenerado. Se aplicará al iniciar nuevamente el servidor.");
    }

    private static string GeneratePin()
    {
        return System.Security.Cryptography.RandomNumberGenerator.GetInt32(10000000, 100000000).ToString();
    }

    private void RenderQr(string text, PictureBox pictureBox)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        var png = new PngByteQRCode(data);
        var bytes = png.GetGraphic(8);
        using var stream = new MemoryStream(bytes);
        pictureBox.Image?.Dispose();
        using var decoded = Image.FromStream(stream);
        pictureBox.Image = new Bitmap(decoded);
    }

    private static void SetQrPlaceholder(PictureBox pictureBox, string text)
    {
        var width = Math.Max(320, pictureBox.Width > 0 ? pictureBox.Width : 320);
        var height = Math.Max(240, pictureBox.Height > 0 ? pictureBox.Height : 240);
        var bitmap = new Bitmap(width, height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.White);
        using var borderPen = new Pen(Color.FromArgb(210, 218, 220), 2);
        using var textBrush = new SolidBrush(Color.FromArgb(88, 98, 102));
        using var titleFont = new Font("Segoe UI", 14F, FontStyle.Bold);
        using var bodyFont = new Font("Segoe UI", 9F);
        graphics.DrawRectangle(borderPen, 12, 12, width - 24, height - 24);
        var titleSize = graphics.MeasureString(text, titleFont);
        graphics.DrawString(text, titleFont, textBrush, (width - titleSize.Width) / 2, (height / 2) - 24);
        const string body = "Genere una URL para mostrar el QR.";
        var bodySize = graphics.MeasureString(body, bodyFont);
        graphics.DrawString(body, bodyFont, textBrush, (width - bodySize.Width) / 2, (height / 2) + 8);
        pictureBox.Image?.Dispose();
        pictureBox.Image = bitmap;
    }

    private static string? GetLanAddress()
    {
        var candidates = NetworkInterface.GetAllNetworkInterfaces()
            .Where(n => n.OperationalStatus == OperationalStatus.Up)
            .SelectMany(n => n.GetIPProperties().UnicastAddresses)
            .Where(a => a.Address.AddressFamily == AddressFamily.InterNetwork)
            .Select(a => a.Address.ToString())
            .Where(ip => !IPAddress.IsLoopback(IPAddress.Parse(ip)) && !ip.StartsWith("169.254."))
            .OrderByDescending(ip => ip.StartsWith("192.168.137."))
            .ThenByDescending(ip => ip.StartsWith("192.168."))
            .ThenByDescending(ip => ip.StartsWith("10."))
            .ToList();
        return candidates.FirstOrDefault();
    }

    private string? FindNgrok()
    {
        var candidates = new[]
        {
            Path.Combine(_projectRoot, "support", "ngrok", "ngrok.exe"),
            Path.Combine(_projectRoot, "ngrok.exe"),
        };
        foreach (var candidate in candidates)
        {
            if (File.Exists(candidate)) return candidate;
        }

        var path = Environment.GetEnvironmentVariable("PATH") ?? "";
        foreach (var dir in path.Split(Path.PathSeparator))
        {
            try
            {
                var candidate = Path.Combine(dir.Trim(), "ngrok.exe");
                if (File.Exists(candidate)) return candidate;
            }
            catch
            {
                // Ignorar entradas PATH invalidas.
            }
        }
        return null;
    }

    private string? FindBackendExecutable()
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "GarajeServidor.exe"),
            Path.Combine(_projectRoot, "GarajeServidor.exe"),
            Path.Combine(_projectRoot, "interfaz_garaje", "dist", "GarajeServidor", "GarajeServidor.exe"),
            Path.Combine(_projectRoot, "release_public", "GarajeServidor_Portable_v1.0.0", "GarajeServidor.exe"),
        };
        return candidates.FirstOrDefault(File.Exists);
    }

    private string FindProjectRoot()
    {
        if (File.Exists(Path.Combine(AppContext.BaseDirectory, "GarajeServidor.exe"))) return AppContext.BaseDirectory;
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "interfaz_garaje", "server.py")))
                return current.FullName;
            current = current.Parent;
        }
        current = new DirectoryInfo(Environment.CurrentDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "interfaz_garaje", "server.py")))
                return current.FullName;
            current = current.Parent;
        }
        return Environment.CurrentDirectory;
    }

    private void CopyLan() => Clipboard.SetText(_lanUrlBox.Text);
    private void CopyNgrok()
    {
        if (!string.IsNullOrWhiteSpace(_ngrokUrlBox.Text)) Clipboard.SetText(_ngrokUrlBox.Text);
    }

    private void OpenLan() => OpenUrl(_lanUrlBox.Text);
    private void OpenNgrok() => OpenUrl(_ngrokUrlBox.Text);

    private static void OpenUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return;
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }

    private void Log(string message)
    {
        if (InvokeRequired)
        {
            try { BeginInvoke(() => Log(message)); } catch { }
            return;
        }
        var line = $"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}";
        _logBox.AppendText(line);
        _demoLogBox.AppendText(line);
    }

    private void SetStatus(string message, Color color)
    {
        if (IsDisposed) return;
        if (InvokeRequired)
        {
            try { BeginInvoke(() => SetStatus(message, color)); } catch { }
            return;
        }
        if (message == "Servidor detenido") _portBox.ReadOnly = false;
        _statusLabel.Text = message;
        _statusLabel.ForeColor = color;
    }
}
