using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

[assembly: AssemblyTitle("Jackdaw Flag Patcher")]
[assembly: AssemblyProduct("Jackdaw Flag Patcher")]
[assembly: AssemblyDescription("Standalone Jackdaw pirate flag texture patcher")]
[assembly: AssemblyCopyright("Copyright © 2026 Fishes")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

namespace JackdawFlagPatcher
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    internal sealed class MainForm : Form
    {
        private readonly TextBox gamePath = new TextBox();
        private readonly TextBox pngPath = new TextBox();
        private readonly TextBox log = new TextBox();
        private readonly Button chooseGame = new Button();
        private readonly Button choosePng = new Button();
        private readonly Button apply = new Button();
        private readonly Button revert = new Button();
        private readonly Panel header = new Panel();
        private readonly Panel accentBar = new Panel();
        private readonly Label title = new Label();
        private readonly Label subtitle = new Label();
        private readonly Label gameLabel = new Label();
        private readonly Label pngLabel = new Label();
        private readonly Label imageHint = new Label();
        private readonly Label activityLabel = new Label();
        private bool darkTheme;

        public MainForm()
        {
            Text = "Jackdaw Flag Patcher  ·  v1.0";
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            ClientSize = new Size(760, 550);
            MinimumSize = new Size(700, 540);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9F);
            AutoScaleMode = AutoScaleMode.Dpi;

            header.Dock = DockStyle.Top;
            header.Height = 102;
            accentBar.Dock = DockStyle.Left;
            accentBar.Width = 6;

            var emblem = new PictureBox
            {
                Image = Icon.ToBitmap(),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(28, 23),
                Size = new Size(54, 54)
            };
            title.Text = "Jackdaw Flag Patcher";
            title.Font = new Font("Segoe UI Semibold", 17F);
            title.AutoSize = true;
            title.Location = new Point(96, 20);
            subtitle.Text = "A focused, reversible texture patcher for the Jackdaw's pirate flag";
            subtitle.Font = new Font("Segoe UI", 9.5F);
            subtitle.AutoSize = true;
            subtitle.Location = new Point(99, 58);
            header.Controls.AddRange(new Control[] { accentBar, emblem, title, subtitle });

            gameLabel.Text = "BLACK FLAG INSTALLATION";
            gameLabel.Font = new Font("Segoe UI Semibold", 8.5F);
            gameLabel.AutoSize = true;
            gameLabel.Location = new Point(28, 126);
            gamePath.Location = new Point(28, 150);
            gamePath.Size = new Size(596, 26);
            gamePath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            chooseGame.Text = "Browse…";
            chooseGame.Location = new Point(636, 147);
            chooseGame.Size = new Size(96, 32);
            chooseGame.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chooseGame.Click += ChooseGame_Click;

            pngLabel.Text = "REPLACEMENT PNG";
            pngLabel.Font = new Font("Segoe UI Semibold", 8.5F);
            pngLabel.AutoSize = true;
            pngLabel.Location = new Point(28, 205);
            pngPath.Location = new Point(28, 229);
            pngPath.Size = new Size(596, 26);
            pngPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            choosePng.Text = "Choose…";
            choosePng.Location = new Point(636, 226);
            choosePng.Size = new Size(96, 32);
            choosePng.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            choosePng.Click += ChoosePng_Click;

            imageHint.Text = "1024 × 512 RGBA  •  Keep artwork 8–16 transparent pixels away from every image edge";
            imageHint.AutoSize = true;
            imageHint.Location = new Point(30, 265);

            apply.Text = "Apply replacement";
            apply.Font = new Font("Segoe UI Semibold", 9.5F);
            apply.Location = new Point(28, 307);
            apply.Size = new Size(156, 40);
            apply.Click += async delegate { await RunActionAsync(false); };
            revert.Text = "Restore original";
            revert.Font = new Font("Segoe UI Semibold", 9.5F);
            revert.Location = new Point(195, 307);
            revert.Size = new Size(145, 40);
            revert.Click += async delegate { await RunActionAsync(true); };

            activityLabel.Text = "ACTIVITY";
            activityLabel.Font = new Font("Segoe UI Semibold", 8.5F);
            activityLabel.AutoSize = true;
            activityLabel.Location = new Point(28, 377);
            log.Location = new Point(28, 401);
            log.Size = new Size(704, 121);
            log.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            log.Multiline = true;
            log.ReadOnly = true;
            log.ScrollBars = ScrollBars.Vertical;
            log.BorderStyle = BorderStyle.FixedSingle;

            Controls.AddRange(new Control[]
            {
                header, gameLabel, gamePath, chooseGame, pngLabel, pngPath, choosePng,
                imageHint, apply, revert, activityLabel, log
            });

            darkTheme = IsSystemDarkTheme();
            ApplyTheme();
            Microsoft.Win32.SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
            FormClosed += delegate { Microsoft.Win32.SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged; };

            Shown += delegate
            {
                var detected = Patcher.FindGameDirectory();
                if (detected != null)
                {
                    gamePath.Text = detected;
                    WriteLog("Black Flag installation detected.");
                }
                else
                {
                    WriteLog("Installation not detected. Choose the folder containing ACBlackFlag.exe or AC4BFSP.exe.");
                }
            };
        }

        private void ChooseGame_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Choose the Assassin's Creed IV Black Flag installation folder";
                dialog.SelectedPath = gamePath.Text;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                    gamePath.Text = dialog.SelectedPath;
            }
        }

        private void ChoosePng_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Choose a 1024 × 512 replacement flag";
                dialog.Filter = "PNG images (*.png)|*.png";
                if (dialog.ShowDialog(this) == DialogResult.OK)
                    pngPath.Text = dialog.FileName;
            }
        }

        private async Task RunActionAsync(bool restore)
        {
            SetBusy(true);
            try
            {
                string result;
                if (restore)
                {
                    result = await Task.Run(() => Patcher.Restore(gamePath.Text));
                }
                else
                {
                    result = await Task.Run(() => Patcher.Apply(gamePath.Text, pngPath.Text));
                }
                WriteLog(result);
                MessageBox.Show(this, result, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                WriteLog("Error: " + ex.Message);
                MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void SetBusy(bool busy)
        {
            chooseGame.Enabled = choosePng.Enabled = apply.Enabled = revert.Enabled = !busy;
            UseWaitCursor = busy;
        }

        private void WriteLog(string text)
        {
            log.AppendText(DateTime.Now.ToString("HH:mm:ss") + "  " + text + Environment.NewLine);
        }

        private void SystemEvents_UserPreferenceChanged(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e)
        {
            if (IsDisposed)
                return;
            BeginInvoke((MethodInvoker)delegate
            {
                darkTheme = IsSystemDarkTheme();
                ApplyTheme();
            });
        }

        private static bool IsSystemDarkTheme()
        {
            if (SystemInformation.HighContrast)
                return false;
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    var value = key == null ? null : key.GetValue("AppsUseLightTheme");
                    return value is int && (int)value == 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private void ApplyTheme()
        {
            if (SystemInformation.HighContrast)
            {
                BackColor = SystemColors.Window;
                ForeColor = SystemColors.WindowText;
                return;
            }

            var background = darkTheme ? Color.FromArgb(22, 25, 29) : Color.FromArgb(245, 247, 249);
            var surface = darkTheme ? Color.FromArgb(31, 36, 42) : Color.White;
            var input = darkTheme ? Color.FromArgb(41, 47, 54) : Color.White;
            var foreground = darkTheme ? Color.FromArgb(242, 244, 247) : Color.FromArgb(29, 35, 42);
            var muted = darkTheme ? Color.FromArgb(171, 180, 190) : Color.FromArgb(91, 101, 112);
            var accent = Color.FromArgb(211, 166, 70);
            var secondary = darkTheme ? Color.FromArgb(53, 60, 68) : Color.FromArgb(226, 230, 235);

            BackColor = background;
            ForeColor = foreground;
            header.BackColor = surface;
            accentBar.BackColor = accent;
            title.ForeColor = foreground;
            subtitle.ForeColor = muted;
            gameLabel.ForeColor = pngLabel.ForeColor = activityLabel.ForeColor = muted;
            imageHint.ForeColor = muted;

            foreach (var box in new[] { gamePath, pngPath, log })
            {
                box.BackColor = input;
                box.ForeColor = foreground;
            }

            StyleButton(chooseGame, secondary, foreground);
            StyleButton(choosePng, secondary, foreground);
            StyleButton(revert, secondary, foreground);
            StyleButton(apply, accent, Color.FromArgb(24, 24, 24));
            ApplyDarkTitleBar();
            Invalidate(true);
        }

        private static void StyleButton(Button button, Color background, Color foreground)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = background;
            button.ForeColor = foreground;
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ApplyDarkTitleBar();
        }

        private void ApplyDarkTitleBar()
        {
            if (!IsHandleCreated || SystemInformation.HighContrast)
                return;
            var enabled = darkTheme ? 1 : 0;
            try
            {
                if (DwmSetWindowAttribute(Handle, 20, ref enabled, sizeof(int)) != 0)
                    DwmSetWindowAttribute(Handle, 19, ref enabled, sizeof(int));
            }
            catch { }
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(
            IntPtr window, int attribute, ref int value, int valueSize);
    }

    internal static class Patcher
    {
        private const ulong TargetFileId = 0x218240C6D66UL;
        private const uint TextureMapType = 0x85C817C3U;
        private const int ExpectedWidth = 1024;
        private const int ExpectedHeight = 512;
        private const int ExpectedBc7Bytes = 524288;
        private const string VanillaEntrySha256 = "91801CBF0E308C450F813C6D0B2A93ACA09C12DDD3B7792910906A8FFA2D0680";
        private const string PrefixSha256 = "5CDC6E73C06AFC4FB33BAE5E01CB5770736A2AF2868692FE1CE073C6E3393940";
        private static readonly byte[] BmsMagic = { 0x33, 0xAA, 0xFB, 0x57, 0x99, 0xFA, 0x04, 0x10 };
        private static readonly byte[] CompressionInfo = { 0x08, 0x00, 0x00, 0x04, 0x80 };

        // Texture metadata for the single Jackdaw pirate-flag resource. No game artwork is embedded.
#if false
        private const string ResourcePrefixBase64 =
            "AwBmbQwkGAIAADECAAAAAAAAAABnPwwkGAIAAMEAAAAAAAAAAABgPwwkGAIAAEgBCAAAAAAAAADDF8iFBAIAACAAAABHeUMKGWZUiih5+xkCp7NK/vBoMYqWi7PP0HPy8P1Q4gBmbQwkGAIAAMMXyIUBASXSfe1UAgAABAAAAAFZ/wokGAIAAAEn8AcMBgIAAAHaDHVGGgIAAAFnPwwkGAIAAAMAAAAAAAAAAAAAAAAAAPj7AAAAAHRfuZIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADIAAEAAAEBAAEAAAABAQAAAAAAAQAAAAAAAAAAAAEBAAAAAAAAAAAAAQAAAAMAAAAAAAAAAAEA+PsAAAAAoaizOwIA+PsAAAAA9kSKvmZtDCQYAgAAAQAAAAMA+PsAAAAAJ5Y+jgUehM0CAAAAAAAAAAEAAAAQAAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAEAAAAFHoTNAAAAAAAADQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIA/AAAAgD8BAAAAAAQA+PsAAAAAjlNBygAAAIA/AACAPwAAgD8AAIA/AACAPwAAgD8AAIA/AAAAAAAHAAAAxUDa4wAAAAAAAA0AAAAAANE6PbEAAAAAAAAKAAAAAAA1pTKyAAAAAAAACgAAAAAAleQiPgAAAAAAAAoAAAAAAFTMEowAAAAAAAAKAAAAAACyGlzpAAAAAAAACgAAAAAAabogpAAAAAAAAAoAAAAAAClcDz6e2Ak+ElUEPgAAgD8AAAAAAACAP9ejMD8AAAAAAACAPzMzcz9wZg7XjwAAACUAAABHeUMKGWZUitDR+hkCp7NKKHmL0Ines0r+8GgxipaLs5G1c6KTAGc/DCQYAgAAcGYO1wEBYD8MJBgCAAADAAAAAAAAAAADAAAAAAAAAAADAAAAAAAAAAADAAAAAAAAAAADAAAAAAAAAAADAAAAAAAAAAADAAAAAAAAAAADAAAAAAAAAAADAAAAAAAAAAADAAAAAAAAAAADAAAAAAAAAABXAAAAF+m3og8BCAAsAAAAR3lDChlmVIrQ0foZAqezSih5i9CJ3rNK/vBoMYqWi7Mu2nKiGdb7mwEPExEAYD8MJBgCAAAX6beiAQAEAAAAAgAAAQAAAAEAAAAKAAAAAQAAAAEAAAABAAAAAAAAAAAAAAAAAAAAAAAAVwAAAAAA+PsAAAAAELVShgMAAAAAAQD4+wAAAAAQtVKGAwAAAAACAPj7AAAAABC1UoYDAAAAAAMA+PsAAAAAELVShgMAAAAAAAQA+PsAAAAA6X8jEwFgPwwkGAIAAAMAAABnAgAAAAQAAAACAAABAAAAAQAAAAEAAAAKAAAAAQAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAAAAQAAAAAAAAAIAAAAAAAACAAAAAABAAAAAgAAFAAAAAEAAAAAAAAAAAAAAAAAAAAAAAgAAAAIAA==";

#endif
        private const string VerifiedResourcePrefixBase64 =
            "AwBmbQwkGAIAADECAAAAAAAAAABnPwwkGAIAAMEAAAAAAAAAAABgPwwkGAIAAEgBCAAAAAAAAADDF8iFBAIAACAAAABHeUMKGWZU" +
            "iih5+xkCp7NK/vBoMYqWi7PP0HPy8P1Q4gBmbQwkGAIAAMMXyIUBASXSfe1UAgAABAAAAAFZ/wokGAIAAAEn8AcMBgIAAAHaDHVG" +
            "GgIAAAFnPwwkGAIAAAMAAAAAAAAAAAAAAAAAAPj7AAAAAHRfuZIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADIAAEAAAEBAAEAAAAB" +
            "AQAAAAAAAQAAAAAAAAAAAAEBAAAAAAAAAAAAAQAAAAMAAAAAAAAAAAEA+PsAAAAAoaizOwIA+PsAAAAA9kSKvmZtDCQYAgAAAQAA" +
            "AAMA+PsAAAAAJ5Y+jgUehM0CAAAAAAAAAAEAAAAQAAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAEAAAAFHoTNAAAAAAAA" +
            "DQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIA/AAAAgD8BAAAAAAQA+PsAAAAAjlNBygAAAIA/AACAPwAAgD8AAIA/AACAPwAA" +
            "gD8AAIA/AAAAAAAHAAAAxUDa4wAAAAAAAA0AAAAAANE6PbEAAAAAAAAKAAAAAAA1pTKyAAAAAAAACgAAAAAAleQiPgAAAAAAAAoA" +
            "AAAAAFTMEowAAAAAAAAKAAAAAACyGlzpAAAAAAAACgAAAAAAabogpAAAAAAAAAoAAAAAAClcDz6e2Ak+ElUEPgAAgD8AAAAAAACA" +
            "P9ejMD8AAAAAAACAPzMzcz9wZg7XjwAAACUAAABHeUMKGWZUitDR+hkCp7NKKHmL0Ines0r+8GgxipaLs5G1c6KTAGc/DCQYAgAA" +
            "cGYO1wEBYD8MJBgCAAADAAAAAAAAAAADAAAAAAAAAAADAAAAAAAAAAADAAAAAAAAAAADAAAAAAAAAAADAAAAAAAAAAADAAAAAAAA" +
            "AAADAAAAAAAAAAADAAAAAAAAAAADAAAAAAAAAAADAAAAAAAAAAADAAAAAAAAAAADAAAAAAAAAABXAAAAF+m3og8BCAAsAAAAR3lD" +
            "ChlmVIrQ0foZAqezSih5i9CJ3rNK/vBoMYqWi7Mu2nKiGdb7mwEPExEAYD8MJBgCAAAX6beiAQAEAAAAAgAAAQAAAAEAAAAKAAAA" +
            "AQAAAAEAAAABAAAAAAAAAAAAAAAAAAAAAAAAVwAAAAAA+PsAAAAAELVShgMAAAAAAQD4+wAAAAAQtVKGAwAAAAACAPj7AAAAABC1" +
            "UoYDAAAAAAMA+PsAAAAAELVShgMAAAAAAAQA+PsAAAAA6X8jEwFgPwwkGAIAAAMAAABnAgAAAAQAAAACAAABAAAAAQAAAAEAAAAK" +
            "AAAAAQAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAAAAQAAAAAAAAAIAAAAAAAACAAAAAABAAAAAgAAFAAAAAEAAAAAAAAAAAAA" +
            "AAAAAAAAAAgAAAAIAA==";

        public static string FindGameDirectory()
        {
            foreach (var candidate in RegistryInstallDirectories())
                if (IsGameDirectory(candidate))
                    return candidate;

            var candidates = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Ubisoft", "Ubisoft Game Launcher", "games", "Assassin's Creed IV Black Flag"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "steamapps", "common", "Assassin's Creed IV Black Flag"),
                @"C:\Program Files (x86)\Steam\steamapps\common\Assassin's Creed IV Black Flag",
                @"C:\Program Files (x86)\Ubisoft\Ubisoft Game Launcher\games\Assassin's Creed IV Black Flag"
            };

            foreach (var candidate in candidates.Distinct(StringComparer.OrdinalIgnoreCase))
                if (IsGameDirectory(candidate))
                    return candidate;

            foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady))
            {
                var candidate = Path.Combine(drive.RootDirectory.FullName, "SteamLibrary", "steamapps", "common", "Assassin's Creed IV Black Flag");
                if (IsGameDirectory(candidate))
                    return candidate;
            }
            return null;
        }

        private static string[] RegistryInstallDirectories()
        {
            var found = new System.Collections.Generic.List<string>();
            foreach (var hive in new[] { RegistryHive.LocalMachine, RegistryHive.CurrentUser })
            foreach (var view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
            {
                try
                {
                    using (var baseKey = RegistryKey.OpenBaseKey(hive, view))
                    {
                        AddUninstallLocations(baseKey, found);
                        AddUbisoftLocations(baseKey, found);
                        AddSteamLocations(baseKey, found);
                    }
                }
                catch { }
            }
            return found.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        }

        private static void AddUninstallLocations(RegistryKey baseKey, System.Collections.Generic.List<string> found)
        {
            using (var uninstall = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
            {
                if (uninstall == null)
                    return;
                foreach (var subkeyName in uninstall.GetSubKeyNames())
                {
                    using (var item = uninstall.OpenSubKey(subkeyName))
                    {
                        if (item == null)
                            continue;
                        var displayName = item.GetValue("DisplayName") as string;
                        var isKnownSteamApp = subkeyName.Equals("Steam App 3751950", StringComparison.OrdinalIgnoreCase) ||
                                              subkeyName.Equals("Steam App 242050", StringComparison.OrdinalIgnoreCase);
                        if (!isKnownSteamApp && !LooksLikeBlackFlag(displayName))
                            continue;
                        AddDirectoryValue(found, item.GetValue("InstallLocation"));
                        AddDirectoryFromCommand(found, item.GetValue("DisplayIcon"));
                        AddDirectoryFromCommand(found, item.GetValue("UninstallString"));
                    }
                }
            }
        }

        private static void AddUbisoftLocations(RegistryKey baseKey, System.Collections.Generic.List<string> found)
        {
            using (var installs = baseKey.OpenSubKey(@"SOFTWARE\Ubisoft\Launcher\Installs"))
            {
                if (installs == null)
                    return;
                foreach (var subkeyName in installs.GetSubKeyNames())
                using (var item = installs.OpenSubKey(subkeyName))
                {
                    if (item == null)
                        continue;
                    AddDirectoryValue(found, item.GetValue("InstallDir"));
                    AddDirectoryValue(found, item.GetValue("InstallLocation"));
                }
            }
        }

        private static void AddSteamLocations(RegistryKey baseKey, System.Collections.Generic.List<string> found)
        {
            using (var steam = baseKey.OpenSubKey(@"SOFTWARE\Valve\Steam"))
            {
                if (steam == null)
                    return;
                var root = steam.GetValue("InstallPath") as string ?? steam.GetValue("SteamPath") as string;
                if (string.IsNullOrWhiteSpace(root))
                    return;
                foreach (var library in SteamLibraries(root))
                {
                    AddSteamManifestInstall(found, library, "3751950");
                    AddSteamManifestInstall(found, library, "242050");
                }
            }
        }

        private static string[] SteamLibraries(string steamRoot)
        {
            var libraries = new System.Collections.Generic.List<string> { steamRoot };
            var vdf = Path.Combine(steamRoot, "steamapps", "libraryfolders.vdf");
            if (File.Exists(vdf))
            {
                foreach (var line in File.ReadAllLines(vdf))
                {
                    var match = Regex.Match(line, "\"path\"\\s+\"([^\"]+)\"", RegexOptions.IgnoreCase);
                    if (match.Success)
                        libraries.Add(match.Groups[1].Value.Replace(@"\\", @"\"));
                }
            }
            return libraries.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        }

        private static void AddSteamManifestInstall(System.Collections.Generic.List<string> found, string library, string appId)
        {
            var manifest = Path.Combine(library, "steamapps", "appmanifest_" + appId + ".acf");
            if (!File.Exists(manifest))
                return;
            foreach (var line in File.ReadAllLines(manifest))
            {
                var match = Regex.Match(line, "\"installdir\"\\s+\"([^\"]+)\"", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    found.Add(Path.Combine(library, "steamapps", "common", match.Groups[1].Value.Replace(@"\\", @"\")));
                    return;
                }
            }
        }

        private static bool LooksLikeBlackFlag(string value)
        {
            return !string.IsNullOrWhiteSpace(value) &&
                   value.IndexOf("Assassin", StringComparison.OrdinalIgnoreCase) >= 0 &&
                   value.IndexOf("Black Flag", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static void AddDirectoryValue(System.Collections.Generic.List<string> found, object value)
        {
            var path = value as string;
            if (!string.IsNullOrWhiteSpace(path))
                found.Add(Environment.ExpandEnvironmentVariables(path.Trim().Trim('"')).TrimEnd('\\', '/'));
        }

        private static void AddDirectoryFromCommand(System.Collections.Generic.List<string> found, object value)
        {
            var command = value as string;
            if (string.IsNullOrWhiteSpace(command))
                return;
            command = Environment.ExpandEnvironmentVariables(command.Trim());
            string executable;
            if (command.StartsWith("\""))
            {
                var end = command.IndexOf('"', 1);
                executable = end > 1 ? command.Substring(1, end - 1) : command.Trim('"');
            }
            else
            {
                var end = command.IndexOf(' ');
                executable = end > 0 ? command.Substring(0, end) : command;
            }
            if (File.Exists(executable))
                found.Add(Path.GetDirectoryName(executable));
        }

        public static string Apply(string selectedGameDirectory, string selectedPng)
        {
            EnsureGameClosed();
            var gameDirectory = NormalizeGameDirectory(selectedGameDirectory);
            var archivePath = Path.Combine(gameDirectory, "DataPC_boot.forge");
            ValidatePng(selectedPng);

            var prefix = Convert.FromBase64String(VerifiedResourcePrefixBase64);
            if (prefix.Length != 1138 || !Hash(prefix).Equals(PrefixSha256, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("The built-in resource definition failed its integrity check.");

            var entry = LocateEntry(archivePath);
            var currentHash = HashRange(archivePath, entry.Offset, entry.Length);
            var backupPath = BackupPathFor(archivePath);
            var backup = File.Exists(backupPath) ? ReadBackup(backupPath) : null;

            if (backup == null)
            {
                if (!currentHash.Equals(VanillaEntrySha256, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException(
                        "The Jackdaw flag is not in its original state. Restore it with the tool that last changed it, then run this patcher again.");
                backup = new Backup(archivePath, entry.Offset, entry.Length, currentHash);
                WriteBackup(backupPath, backup);
            }
            else
            {
                backup.ValidateFor(archivePath);
                if (!currentHash.Equals(VanillaEntrySha256, StringComparison.OrdinalIgnoreCase) &&
                    !IsOurUncompressedEntry(archivePath, entry))
                    throw new InvalidOperationException("The current flag entry was changed by another mod. Restore the original before continuing.");
            }

            var bc7 = ConvertPngToBc7(selectedPng);
            var resource = new byte[prefix.Length + bc7.Length];
            Buffer.BlockCopy(prefix, 0, resource, 0, prefix.Length);
            Buffer.BlockCopy(bc7, 0, resource, prefix.Length, bc7.Length);
            var packed = BuildUncompressedBms(resource);

            using (var stream = new FileStream(archivePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                stream.Seek(0, SeekOrigin.End);
                var newOffset = checked((ulong)stream.Position);
                stream.Write(packed, 0, packed.Length);
                stream.Flush(true);

                var writtenHash = HashRange(stream, (long)newOffset, packed.Length);
                var expectedHash = Hash(packed);
                if (!writtenHash.Equals(expectedHash, StringComparison.OrdinalIgnoreCase))
                    throw new IOException("The appended flag data did not pass verification. The game index was not changed.");

                try
                {
                    stream.Position = entry.RowPosition;
                    writer.Write(newOffset);
                    writer.Write(TargetFileId);
                    writer.Write((uint)packed.Length);
                    writer.Write(entry.Type);
                    stream.Flush(true);
                }
                catch
                {
                    stream.Position = entry.RowPosition;
                    writer.Write(entry.Offset);
                    writer.Write(TargetFileId);
                    writer.Write(entry.Length);
                    writer.Write(entry.Type);
                    stream.Flush(true);
                    throw;
                }
            }

            return "Jackdaw flag applied. Your original index entry is backed up for restoration.";
        }

        public static string Restore(string selectedGameDirectory)
        {
            EnsureGameClosed();
            var gameDirectory = NormalizeGameDirectory(selectedGameDirectory);
            var archivePath = Path.Combine(gameDirectory, "DataPC_boot.forge");
            var backupPath = BackupPathFor(archivePath);
            if (!File.Exists(backupPath))
                throw new FileNotFoundException("No backup made by Jackdaw Flag Patcher was found.");

            var backup = ReadBackup(backupPath);
            backup.ValidateFor(archivePath);
            var originalHash = HashRange(archivePath, backup.Offset, backup.Length);
            if (!originalHash.Equals(backup.Sha256, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("The backed-up original data no longer matches this archive. Nothing was changed.");

            var entry = LocateEntry(archivePath);
            using (var stream = new FileStream(archivePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                stream.Position = entry.RowPosition;
                writer.Write(backup.Offset);
                writer.Write(TargetFileId);
                writer.Write(backup.Length);
                writer.Write(entry.Type);
                stream.Flush(true);
            }
            File.Delete(backupPath);
            return "Original Jackdaw flag restored. Appended mod data was left harmlessly unreferenced.";
        }

        private static string NormalizeGameDirectory(string selected)
        {
            if (string.IsNullOrWhiteSpace(selected))
                throw new DirectoryNotFoundException("Choose the Black Flag installation folder.");
            var full = Path.GetFullPath(selected.Trim());
            if (!IsGameDirectory(full))
                throw new DirectoryNotFoundException(
                    "That folder does not contain DataPC_boot.forge and a recognized Black Flag executable (ACBlackFlag.exe or AC4BFSP.exe).");
            return full;
        }

        private static bool IsGameDirectory(string directory)
        {
            return !string.IsNullOrWhiteSpace(directory) &&
                   (File.Exists(Path.Combine(directory, "ACBlackFlag.exe")) ||
                    File.Exists(Path.Combine(directory, "AC4BFSP.exe"))) &&
                   File.Exists(Path.Combine(directory, "DataPC_boot.forge"));
        }

        private static void ValidatePng(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                throw new FileNotFoundException("Choose a replacement PNG.");
            if (!Path.GetExtension(path).Equals(".png", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("The replacement must be a PNG.");
            using (var image = Image.FromFile(path))
            {
                if (image.RawFormat.Guid != System.Drawing.Imaging.ImageFormat.Png.Guid)
                    throw new InvalidDataException("The selected file is not a valid PNG.");
                if (image.Width != ExpectedWidth || image.Height != ExpectedHeight)
                    throw new InvalidDataException("The replacement must be exactly 1024 × 512 pixels.");
            }
        }

        private static byte[] ConvertPngToBc7(string pngPath)
        {
            var texconv = Environment.GetEnvironmentVariable("JFP_TEXCONV_PATH");
            if (string.IsNullOrWhiteSpace(texconv))
                texconv = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "texconv.exe");
            if (!File.Exists(texconv))
                throw new FileNotFoundException("texconv.exe is missing. Re-extract the complete patcher archive.");

            var temp = Path.Combine(Path.GetTempPath(), "JackdawFlagPatcher-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(temp);
            try
            {
                var input = Path.Combine(temp, "flag.png");
                File.Copy(pngPath, input);
                var info = new ProcessStartInfo
                {
                    FileName = texconv,
                    Arguments = "-nologo -y -m 1 -f BC7_UNORM --ignore-srgb -w 1024 -h 512 -o \"" + temp + "\" \"" + input + "\"",
                    WorkingDirectory = temp,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                string output;
                using (var process = Process.Start(info))
                {
                    output = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                        throw new InvalidOperationException("Texture conversion failed: " + output.Trim());
                }

                var dds = Path.Combine(temp, "flag.DDS");
                if (!File.Exists(dds))
                    dds = Path.Combine(temp, "flag.dds");
                return ReadBc7Dds(dds);
            }
            finally
            {
                try { Directory.Delete(temp, true); } catch { }
            }
        }

        private static byte[] ReadBc7Dds(string path)
        {
            var bytes = File.ReadAllBytes(path);
            if (bytes.Length < 148 + ExpectedBc7Bytes ||
                bytes[0] != (byte)'D' || bytes[1] != (byte)'D' || bytes[2] != (byte)'S' || bytes[3] != (byte)' ')
                throw new InvalidDataException("The converter produced an invalid DDS file.");
            if (BitConverter.ToInt32(bytes, 12) != ExpectedHeight ||
                BitConverter.ToInt32(bytes, 16) != ExpectedWidth ||
                Encoding.ASCII.GetString(bytes, 84, 4) != "DX10" ||
                BitConverter.ToInt32(bytes, 128) != 98)
                throw new InvalidDataException("The converter did not produce the expected 1024 × 512 BC7 UNORM texture.");
            var pixels = new byte[ExpectedBc7Bytes];
            Buffer.BlockCopy(bytes, 148, pixels, 0, pixels.Length);
            return pixels;
        }

        private static Entry LocateEntry(string archivePath)
        {
            using (var stream = new FileStream(archivePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new BinaryReader(stream))
            {
                if (Encoding.ASCII.GetString(reader.ReadBytes(8)) != "scimitar")
                    throw new InvalidDataException("DataPC_boot.forge has an unrecognized header.");
                stream.Position = 13;
                var headerOffset = reader.ReadInt64();
                if (headerOffset < 0 || headerOffset > stream.Length - 12)
                    throw new InvalidDataException("The forge header index is invalid.");
                stream.Position = headerOffset;
                var count = reader.ReadUInt32();
                var tableOffset = reader.ReadUInt64();
                if (count == 0 || count > 10000000 || tableOffset > (ulong)stream.Length)
                    throw new InvalidDataException("The forge resource index is invalid.");

                stream.Position = checked((long)tableOffset);
                for (uint i = 0; i < count; i++)
                {
                    var rowPosition = stream.Position;
                    var offset = reader.ReadUInt64();
                    var fileId = reader.ReadUInt64();
                    var length = reader.ReadUInt32();
                    var type = reader.ReadUInt32();
                    if (fileId == TargetFileId && type == TextureMapType)
                    {
                        if (offset > (ulong)stream.Length || length > stream.Length - (long)offset)
                            throw new InvalidDataException("The Jackdaw flag entry points outside the archive.");
                        return new Entry(rowPosition, offset, length, type);
                    }
                }
            }
            throw new InvalidDataException("The Jackdaw pirate-flag resource was not found. This game build may be unsupported.");
        }

        private static byte[] BuildUncompressedBms(byte[] resource)
        {
            using (var output = new MemoryStream())
            using (var writer = new BinaryWriter(output))
            {
                WriteBmsBlock(writer, new[] { Slice(resource, 0, 56) });
                WriteBmsBlock(writer, new[]
                {
                    Slice(resource, 56, 262144),
                    Slice(resource, 262200, 262144),
                    Slice(resource, 524344, 1082)
                });
                return output.ToArray();
            }
        }

        private static void WriteBmsBlock(BinaryWriter writer, byte[][] pieces)
        {
            writer.Write(BmsMagic);
            writer.Write((ushort)3);
            writer.Write(CompressionInfo);
            writer.Write((uint)pieces.Length);
            foreach (var piece in pieces)
            {
                writer.Write((uint)piece.Length);
                writer.Write((uint)piece.Length);
            }
            foreach (var piece in pieces)
            {
                writer.Write(Adler32SeedZero(piece));
                writer.Write(piece);
            }
        }

        private static uint Adler32SeedZero(byte[] data)
        {
            const uint modulus = 65521;
            uint a = 0;
            uint b = 0;
            foreach (var value in data)
            {
                a = (a + value) % modulus;
                b = (b + a) % modulus;
            }
            return (b << 16) | a;
        }

        private static byte[] Slice(byte[] source, int offset, int count)
        {
            var result = new byte[count];
            Buffer.BlockCopy(source, offset, result, 0, count);
            return result;
        }

        private static bool IsOurUncompressedEntry(string archivePath, Entry entry)
        {
            if (entry.Length != 525512)
                return false;
            try
            {
                using (var stream = new FileStream(archivePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var reader = new BinaryReader(stream))
                {
                    stream.Position = (long)entry.Offset;
                    if (!reader.ReadBytes(8).SequenceEqual(BmsMagic) || reader.ReadUInt16() != 3 ||
                        !reader.ReadBytes(5).SequenceEqual(CompressionInfo) || reader.ReadUInt32() != 1 ||
                        reader.ReadUInt32() != 56 || reader.ReadUInt32() != 56)
                        return false;
                    reader.ReadUInt32();
                    var first = reader.ReadBytes(56);

                    if (!reader.ReadBytes(8).SequenceEqual(BmsMagic) || reader.ReadUInt16() != 3 ||
                        !reader.ReadBytes(5).SequenceEqual(CompressionInfo) || reader.ReadUInt32() != 3)
                        return false;
                    var sizes = new[] { 262144, 262144, 1082 };
                    for (var i = 0; i < sizes.Length; i++)
                        if (reader.ReadUInt32() != sizes[i] || reader.ReadUInt32() != sizes[i])
                            return false;

                    using (var prefix = new MemoryStream())
                    {
                        prefix.Write(first, 0, first.Length);
                        foreach (var size in sizes)
                        {
                            reader.ReadUInt32();
                            var take = Math.Min(size, 1138 - (int)prefix.Length);
                            if (take > 0)
                                prefix.Write(reader.ReadBytes(take), 0, take);
                            stream.Position += size - take;
                        }
                        return prefix.Length == 1138 &&
                               Hash(prefix.ToArray()).Equals(PrefixSha256, StringComparison.OrdinalIgnoreCase);
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private static void EnsureGameClosed()
        {
            if (Environment.GetEnvironmentVariable("JFP_TEST_MODE") == "1")
                return;
            if (Process.GetProcessesByName("AC4BFSP").Length != 0 ||
                Process.GetProcessesByName("ACBlackFlag").Length != 0)
                throw new InvalidOperationException("Close Assassin's Creed IV Black Flag before patching.");
        }

        private static string BackupPathFor(string archivePath)
        {
            var directory = Environment.GetEnvironmentVariable("JFP_BACKUP_DIR");
            if (string.IsNullOrWhiteSpace(directory))
                directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Jackdaw Flag Patcher Backup");
            var name = Hash(Encoding.UTF8.GetBytes(Path.GetFullPath(archivePath).ToUpperInvariant())).Substring(0, 16) + ".jfpbackup";
            return Path.Combine(directory, name);
        }

        private static void WriteBackup(string path, Backup backup)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var temp = path + ".tmp";
            using (var stream = new FileStream(temp, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new BinaryWriter(stream, Encoding.UTF8))
            {
                writer.Write(Encoding.ASCII.GetBytes("JFP1"));
                writer.Write(backup.ArchivePath);
                writer.Write(backup.Offset);
                writer.Write(backup.Length);
                writer.Write(backup.Sha256);
                stream.Flush(true);
            }
            if (File.Exists(path))
                File.Delete(temp);
            else
                File.Move(temp, path);
        }

        private static Backup ReadBackup(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new BinaryReader(stream, Encoding.UTF8))
            {
                if (Encoding.ASCII.GetString(reader.ReadBytes(4)) != "JFP1")
                    throw new InvalidDataException("The patcher backup is invalid.");
                return new Backup(reader.ReadString(), reader.ReadUInt64(), reader.ReadUInt32(), reader.ReadString());
            }
        }

        private static string Hash(byte[] bytes)
        {
            using (var sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "");
        }

        private static string HashRange(string path, ulong offset, uint length)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                return HashRange(stream, checked((long)offset), checked((int)length));
        }

        private static string HashRange(Stream stream, long offset, int length)
        {
            var originalPosition = stream.Position;
            try
            {
                stream.Position = offset;
                using (var sha = SHA256.Create())
                {
                    var buffer = new byte[65536];
                    var remaining = length;
                    while (remaining > 0)
                    {
                        var read = stream.Read(buffer, 0, Math.Min(buffer.Length, remaining));
                        if (read == 0)
                            throw new EndOfStreamException();
                        sha.TransformBlock(buffer, 0, read, null, 0);
                        remaining -= read;
                    }
                    sha.TransformFinalBlock(new byte[0], 0, 0);
                    return BitConverter.ToString(sha.Hash).Replace("-", "");
                }
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }

        private sealed class Entry
        {
            public readonly long RowPosition;
            public readonly ulong Offset;
            public readonly uint Length;
            public readonly uint Type;
            public Entry(long rowPosition, ulong offset, uint length, uint type)
            {
                RowPosition = rowPosition;
                Offset = offset;
                Length = length;
                Type = type;
            }
        }

        private sealed class Backup
        {
            public readonly string ArchivePath;
            public readonly ulong Offset;
            public readonly uint Length;
            public readonly string Sha256;
            public Backup(string archivePath, ulong offset, uint length, string sha256)
            {
                ArchivePath = Path.GetFullPath(archivePath);
                Offset = offset;
                Length = length;
                Sha256 = sha256;
            }
            public void ValidateFor(string archivePath)
            {
                if (!ArchivePath.Equals(Path.GetFullPath(archivePath), StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("The backup belongs to a different Black Flag installation.");
            }
        }
    }
}
