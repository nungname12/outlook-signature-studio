using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Win32;

namespace SignatureStudio
{
    public sealed class SignatureProfile
    {
        public string TemplateName = "IT Helpdesk - Draft";
        public string Greeting = "Best regards,";
        public string SenderName = "Pattraporn Pirarat";
        public string Role = "QA Manager";
        public string Company = "Belton Industrial (Thailand) Ltd. - Rojana";
        public string Group = "Belton Technology Group";
        public string Phone = "(66) 2529 7300";
        public string Extension = "831-2980";
        public string Mobile = "(66) 8340 7526";
        public string Email = "pattraporn.p@beltontechnology.com";
        public string Website = "www.beltontechnology.com";
        public bool UseForNew = true;
        public bool UseForReply = true;
    }

    public sealed class MainWindow : Window
    {
        private readonly SignatureProfile profile = new SignatureProfile();
        private readonly Dictionary<string, TextBox> fields = new Dictionary<string, TextBox>();
        private readonly WebBrowser preview = new WebBrowser();
        private readonly TextBlock status = new TextBlock();
        private readonly TextBlock stepText = new TextBlock();
        private readonly CheckBox newMessages = new CheckBox();
        private readonly CheckBox replies = new CheckBox();
        private readonly StackPanel content = new StackPanel();
        private bool loadingValues;
        private readonly string signatureRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "Signatures");

        private static readonly Brush Navy = new SolidColorBrush(Color.FromRgb(20, 36, 66));
        private static readonly Brush Blue = new SolidColorBrush(Color.FromRgb(45, 108, 223));
        private static readonly Brush Ink = new SolidColorBrush(Color.FromRgb(23, 32, 51));
        private static readonly Brush Muted = new SolidColorBrush(Color.FromRgb(105, 117, 138));
        private static readonly Brush Line = new SolidColorBrush(Color.FromRgb(226, 231, 240));
        private static readonly Brush Wash = new SolidColorBrush(Color.FromRgb(244, 247, 251));

        public MainWindow()
        {
            Title = "Signature Studio - Outlook Manager";
            Width = 1180;
            Height = 760;
            MinWidth = 980;
            MinHeight = 650;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Background = Wash;
            FontFamily = new FontFamily("Segoe UI");
            BuildShell();
            LoadValues();
            UpdatePreview();
        }

        private void BuildShell()
        {
            var root = new Grid();
            root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(70) });
            root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(32) });

            var header = new Border { Background = Navy, Padding = new Thickness(25, 0, 25, 0) };
            var headerRow = new Grid();
            headerRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            var brand = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
            brand.Children.Add(new Border { Width = 34, Height = 34, CornerRadius = new CornerRadius(10), Background = new SolidColorBrush(Color.FromRgb(200, 230, 107)), Child = new TextBlock { Text = "S", Foreground = Navy, FontSize = 19, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } });
            var brandText = new StackPanel { Margin = new Thickness(11, 0, 0, 0) };
            brandText.Children.Add(new TextBlock { Text = "Signature Studio", Foreground = Brushes.White, FontSize = 16, FontWeight = FontWeights.Bold });
            brandText.Children.Add(new TextBlock { Text = "Portable Outlook signature manager", Foreground = new SolidColorBrush(Color.FromRgb(173, 186, 208)), FontSize = 11 });
            brand.Children.Add(brandText);
            headerRow.Children.Add(brand);
            var headButtons = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
            headButtons.Children.Add(HeaderButton("Open signature folder", OpenSignatureFolder));
            headButtons.Children.Add(HeaderButton("About", delegate { MessageBox.Show(this, "Signature Studio v1\nClassic Outlook 2010+ integration", "About", MessageBoxButton.OK, MessageBoxImage.Information); }));
            Grid.SetColumn(headButtons, 1);
            headerRow.Children.Add(headButtons);
            header.Child = headerRow;
            Grid.SetRow(header, 0);
            root.Children.Add(header);

            var body = new Grid { Margin = new Thickness(25, 24, 25, 15) };
            body.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(215) });
            body.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            body.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.05, GridUnitType.Star) });
            body.Children.Add(BuildSteps());
            var main = new Border { Background = Brushes.White, BorderBrush = Line, BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(15), Margin = new Thickness(15, 0, 8, 0) };
            main.Child = BuildMainContent();
            Grid.SetColumn(main, 1);
            body.Children.Add(main);
            var previewCard = new Border { Background = Brushes.White, BorderBrush = Line, BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(15), Margin = new Thickness(8, 0, 0, 0) };
            previewCard.Child = BuildPreviewContent();
            Grid.SetColumn(previewCard, 2);
            body.Children.Add(previewCard);
            Grid.SetRow(body, 1);
            root.Children.Add(body);

            var footer = new Border { BorderBrush = Line, BorderThickness = new Thickness(0, 1, 0, 0), Background = Brushes.White, Padding = new Thickness(25, 0, 25, 0) };
            footer.Child = status;
            status.VerticalAlignment = VerticalAlignment.Center;
            status.Foreground = Muted;
            status.FontSize = 11;
            status.Text = "Starter draft พร้อมแก้ไข - ข้อมูลจะเขียนเฉพาะโปรไฟล์ Windows ปัจจุบัน";
            Grid.SetRow(footer, 2);
            root.Children.Add(footer);
            Content = root;
        }

        private Button HeaderButton(string text, RoutedEventHandler click)
        {
            var b = new Button { Content = text, Margin = new Thickness(6, 0, 0, 0), Padding = new Thickness(10, 6, 10, 6), Background = Brushes.Transparent, Foreground = Brushes.White, BorderBrush = new SolidColorBrush(Color.FromRgb(61, 78, 112)), BorderThickness = new Thickness(1), FontSize = 11 };
            b.Click += click;
            return b;
        }

        private UIElement BuildSteps()
        {
            var panel = new Border { Background = Brushes.White, BorderBrush = Line, BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(15), Padding = new Thickness(18) };
            var stack = new StackPanel();
            stack.Children.Add(new TextBlock { Text = "SETUP PROGRESS", Foreground = Blue, FontSize = 10, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 12) });
            stack.Children.Add(Step("1", "Choose template", "เลือก Signature ที่ต้องการ", true));
            stack.Children.Add(Step("2", "Your details", "กรอกข้อมูลผู้ส่ง", true));
            stack.Children.Add(Step("3", "Set in Outlook", "ติดตั้งอัตโนมัติ", true));
            var info = new Border { Background = new SolidColorBrush(Color.FromRgb(247, 250, 255)), BorderBrush = new SolidColorBrush(Color.FromRgb(216, 227, 249)), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(10), Padding = new Thickness(11), Margin = new Thickness(0, 20, 0, 0) };
            var infoText = new TextBlock { TextWrapping = TextWrapping.Wrap, Foreground = Muted, FontSize = 11 };
            infoText.Inlines.Add(new Run("Target\n") { FontWeight = FontWeights.Bold, Foreground = Ink });
            infoText.Inlines.Add(new Run("Classic Outlook\n2010, 2013, 2016, 2019, 2021, Microsoft 365 Classic\n\n"));
            infoText.Inlines.Add(new Run("No administrator rights required") { FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(78, 124, 41)) });
            info.Child = infoText;
            stack.Children.Add(info);
            panel.Child = stack;
            return panel;
        }

        private UIElement Step(string number, string title, string sub, bool done)
        {
            var row = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 15) };
            row.Children.Add(new Border { Width = 27, Height = 27, CornerRadius = new CornerRadius(14), Background = new SolidColorBrush(Color.FromRgb(239, 248, 217)), BorderBrush = new SolidColorBrush(Color.FromRgb(215, 237, 182)), BorderThickness = new Thickness(1), Child = new TextBlock { Text = "✓", Foreground = new SolidColorBrush(Color.FromRgb(78, 124, 41)), FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } });
            var text = new StackPanel { Margin = new Thickness(10, 0, 0, 0) };
            text.Children.Add(new TextBlock { Text = title, Foreground = Ink, FontWeight = FontWeights.Bold, FontSize = 12 });
            text.Children.Add(new TextBlock { Text = sub, Foreground = Muted, FontSize = 10, TextWrapping = TextWrapping.Wrap, MaxWidth = 140 });
            row.Children.Add(text);
            return row;
        }

        private UIElement BuildMainContent()
        {
            var stack = new StackPanel();
            stack.Children.Add(Header("Your details", "เปิดมาพร้อม Starter Draft แล้ว แก้เฉพาะข้อมูลของ User ก่อนกด SET SIGNATURE"));
            var scroll = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Padding = new Thickness(22, 0, 22, 0) };
            content.Margin = new Thickness(0, 2, 0, 0);
            content.Children.Add(Field("Template name", "TemplateName", false));
            content.Children.Add(Field("คำขึ้นต้น", "Greeting", false));
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(12) });
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            AddPair(grid, 0, Field("ชื่อผู้ใช้งาน", "SenderName", false), Field("ตำแหน่ง / แผนก", "Role", false));
            AddPair(grid, 1, Field("บริษัท", "Company", false), Field("กลุ่มบริษัท", "Group", false));
            AddPair(grid, 2, Field("Direct Line", "Phone", false), Field("Extension", "Extension", false));
            AddPair(grid, 3, Field("Mobile", "Mobile", false), Field("Email", "Email", false));
            AddPair(grid, 4, Field("Website", "Website", false), new Border());
            content.Children.Add(grid);
            content.Children.Add(new Border { Height = 1, Background = Line, Margin = new Thickness(0, 18, 0, 14) });
            newMessages.Content = "Use for new messages";
            newMessages.IsChecked = true;
            newMessages.Foreground = Muted;
            newMessages.Margin = new Thickness(0, 0, 0, 9);
            newMessages.Checked += CheckChanged;
            newMessages.Unchecked += CheckChanged;
            replies.Content = "Use for replies & forwards";
            replies.IsChecked = true;
            replies.Foreground = Muted;
            replies.Checked += CheckChanged;
            replies.Unchecked += CheckChanged;
            content.Children.Add(newMessages);
            content.Children.Add(replies);
            scroll.Content = content;
            stack.Children.Add(scroll);
            var actions = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(22, 12, 22, 18) };
            actions.Children.Add(ActionButton("Reset", ResetValues, false));
            actions.Children.Add(ActionButton("Export HTML", ExportHtml, false));
            actions.Children.Add(ActionButton("SET SIGNATURE", SetSignature, true));
            stack.Children.Add(actions);
            return stack;
        }

        private TextBlock Header(string title, string sub)
        {
            var text = new TextBlock { Margin = new Thickness(22, 20, 22, 15) };
            text.Inlines.Add(new Run(title + "\n") { Foreground = Ink, FontSize = 18, FontWeight = FontWeights.Bold });
            text.Inlines.Add(new Run(sub) { Foreground = Muted, FontSize = 11 });
            return text;
        }

        private FrameworkElement Field(string label, string key, bool full)
        {
            var stack = new StackPanel { Margin = new Thickness(0, 0, 0, 12) };
            stack.Children.Add(new TextBlock { Text = label.ToUpperInvariant(), Foreground = Muted, FontSize = 10, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) });
            var box = new TextBox { Height = 30, Padding = new Thickness(8, 5, 8, 5), BorderBrush = Line, BorderThickness = new Thickness(1), Background = new SolidColorBrush(Color.FromRgb(252, 253, 255)), Foreground = Ink, FontSize = 11 };
            box.TextChanged += FieldChanged;
            fields[key] = box;
            stack.Children.Add(box);
            return stack;
        }

        private void AddPair(Grid grid, int row, FrameworkElement left, FrameworkElement right)
        {
            while (grid.RowDefinitions.Count <= row) grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Grid.SetRow(left, row); Grid.SetColumn(left, 0);
            Grid.SetRow(right, row); Grid.SetColumn(right, 2);
            grid.Children.Add(left); grid.Children.Add(right);
        }

        private Button ActionButton(string text, RoutedEventHandler click, bool primary)
        {
            var b = new Button { Content = text, Padding = new Thickness(13, 8, 13, 8), Margin = new Thickness(7, 0, 0, 0), FontSize = 11, FontWeight = FontWeights.Bold, Background = primary ? Blue : Brushes.White, Foreground = primary ? Brushes.White : Ink, BorderBrush = primary ? Blue : Line, BorderThickness = new Thickness(1) };
            b.Click += click;
            return b;
        }

        private UIElement BuildPreviewContent()
        {
            var stack = new StackPanel();
            var head = new Border { Padding = new Thickness(20, 20, 20, 15), BorderBrush = Line, BorderThickness = new Thickness(0, 0, 0, 1) };
            var title = new TextBlock();
            title.Inlines.Add(new Run("Live preview\n") { Foreground = Ink, FontSize = 16, FontWeight = FontWeights.Bold });
            title.Inlines.Add(new Run("ตัวอย่าง Signature ใน New message") { Foreground = Muted, FontSize = 11 });
            head.Child = title;
            stack.Children.Add(head);
            var previewBorder = new Border { Margin = new Thickness(20), Background = new SolidColorBrush(Color.FromRgb(244, 247, 251)), BorderBrush = Line, BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(10), Padding = new Thickness(12) };
            previewBorder.Child = preview;
            preview.HorizontalAlignment = HorizontalAlignment.Stretch;
            preview.VerticalAlignment = VerticalAlignment.Stretch;
            preview.MinHeight = 350;
            stack.Children.Add(previewBorder);
            var note = new TextBlock { Text = "Preview จะเปลี่ยนทันทีตามข้อมูลที่กรอก", Foreground = Muted, FontSize = 10, Margin = new Thickness(20, 0, 20, 20), TextWrapping = TextWrapping.Wrap };
            stack.Children.Add(note);
            return stack;
        }

        private void LoadValues()
        {
            loadingValues = true;
            fields["TemplateName"].Text = profile.TemplateName;
            fields["Greeting"].Text = profile.Greeting;
            fields["SenderName"].Text = profile.SenderName;
            fields["Role"].Text = profile.Role;
            fields["Company"].Text = profile.Company;
            fields["Group"].Text = profile.Group;
            fields["Phone"].Text = profile.Phone;
            fields["Extension"].Text = profile.Extension;
            fields["Mobile"].Text = profile.Mobile;
            fields["Email"].Text = profile.Email;
            fields["Website"].Text = profile.Website;
            newMessages.IsChecked = profile.UseForNew;
            replies.IsChecked = profile.UseForReply;
            loadingValues = false;
        }

        private void FieldChanged(object sender, TextChangedEventArgs e)
        {
            if (loadingValues || !IsInitialized) return;
            SyncProfile();
            UpdatePreview();
        }

        private void CheckChanged(object sender, RoutedEventArgs e)
        {
            profile.UseForNew = newMessages.IsChecked == true;
            profile.UseForReply = replies.IsChecked == true;
            UpdatePreview();
        }

        private void SyncProfile()
        {
            profile.TemplateName = fields["TemplateName"].Text.Trim();
            profile.Greeting = fields["Greeting"].Text.Trim();
            profile.SenderName = fields["SenderName"].Text.Trim();
            profile.Role = fields["Role"].Text.Trim();
            profile.Company = fields["Company"].Text.Trim();
            profile.Group = fields["Group"].Text.Trim();
            profile.Phone = fields["Phone"].Text.Trim();
            profile.Extension = fields["Extension"].Text.Trim();
            profile.Mobile = fields["Mobile"].Text.Trim();
            profile.Email = fields["Email"].Text.Trim();
            profile.Website = fields["Website"].Text.Trim();
        }

        private void UpdatePreview()
        {
            try { preview.NavigateToString(BuildPreviewHtml()); } catch { }
        }

        private string BuildPreviewHtml()
        {
            return "<html><head><meta http-equiv='X-UA-Compatible' content='IE=edge'></head><body style='margin:0;background:#f4f7fb;font-family:Arial,sans-serif;font-size:12px;color:#394459'><div style='background:white;border:1px solid #dbe2ec;border-radius:8px;padding:18px;min-height:270px'><div style='color:#8a95a8;font-size:11px;padding-bottom:18px'>To: customer@example.com<br>Subject: Service request follow-up</div><div style='font-family:Georgia,serif;font-size:13px;color:#334056'>Hello,<br><span style='display:block;margin-top:6px'>Thank you for contacting our support team.</span></div><div style='border-top:1px solid #cfd5df;margin-top:22px;padding-top:10px;line-height:1.6'>" + BuildSignatureHtml(false) + "</div></div></body></html>";
        }

        private string BuildSignatureDocumentHtml()
        {
            return "<!DOCTYPE html><html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8'><meta http-equiv='X-UA-Compatible' content='IE=edge'><title>" + Html(profile.TemplateName) + "</title></head><body style='margin:0;padding:0;background:#ffffff;font-family:Arial,sans-serif;font-size:12px;color:#394459'>" + BuildSignatureHtml(true) + "</body></html>";
        }

        private string BuildSignatureHtml(bool tableWrapper)
        {
            string direct = Join(" &nbsp;|&nbsp; ", profile.Phone.Length == 0 ? "" : "Direct Line: " + Html(profile.Phone), profile.Extension.Length == 0 ? "" : "Ext.: " + Html(profile.Extension));
            string mobile = profile.Mobile.Length == 0 ? "" : "Mobile: " + Html(profile.Mobile);
            string identity = Html(profile.SenderName);
            if (profile.Role.Length > 0) identity += " <span style='font-weight:400;color:#313a4b'>| " + Html(profile.Role) + "</span>";
            if (profile.Company.Length > 0) identity += " <span style='font-weight:400;color:#313a4b'>| " + Html(profile.Company) + "</span>";
            string email = profile.Email.Length == 0 ? "" : "Email: <a href='mailto:" + Html(profile.Email) + "' style='color:#2658b1;text-decoration:underline'>" + Html(profile.Email) + "</a>";
            string website = profile.Website.Length == 0 ? "" : "Website: <a href='" + Html(Url(profile.Website)) + "' style='color:#2658b1;text-decoration:underline'>" + Html(profile.Website) + "</a>";
            string body = "<div style='font-family:Georgia,serif;color:#334056;margin-bottom:3px'>" + Html(profile.Greeting) + "</div>";
            body += "<div style='font-size:15px;font-weight:800;color:#172541;white-space:normal'>" + identity + "</div>";
            if (profile.Group.Length > 0) body += "<div style='font-weight:600;color:#313a4b;margin-top:10px'>" + Html(profile.Group) + "</div>";
            body += "<div style='margin-top:10px'>" + direct;
            if (direct.Length > 0 && mobile.Length > 0) body += "<br>";
            body += mobile;
            if ((direct.Length > 0 || mobile.Length > 0) && email.Length > 0) body += "<br>";
            body += email;
            if ((direct.Length > 0 || mobile.Length > 0 || email.Length > 0) && website.Length > 0) body += "<br>";
            body += website + "</div>";
            // Keep the signature table-free. Outlook 2010 opens contextual Table Tools
            // whenever the caret lands inside a table-based signature.
            if (tableWrapper) return "<div style='font-family:Arial,sans-serif;font-size:12px;line-height:1.55;color:#394459;border-top:1px solid #cfd5df;padding-top:10px'>" + body + "</div>";
            return body;
        }

        private string Html(string value)
        {
            return (value ?? "").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&#39;");
        }

        private string Url(string value)
        {
            return value.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || value.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ? value : "https://" + value;
        }

        private string Join(string separator, params string[] values)
        {
            return string.Join(separator, values.Where(v => !string.IsNullOrEmpty(v)).ToArray());
        }

        private void ResetValues(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(this, "ล้างข้อมูลกลับเป็นค่าเริ่มต้นหรือไม่?", "Reset", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;
            var fresh = new SignatureProfile();
            CopyProfile(fresh);
            LoadValues();
            newMessages.IsChecked = profile.UseForNew;
            replies.IsChecked = profile.UseForReply;
            UpdatePreview();
            ShowStatus("รีเซ็ตข้อมูลแล้ว");
        }

        private void CopyProfile(SignatureProfile source)
        {
            profile.TemplateName = source.TemplateName; profile.Greeting = source.Greeting; profile.SenderName = source.SenderName; profile.Role = source.Role; profile.Company = source.Company; profile.Group = source.Group; profile.Phone = source.Phone; profile.Extension = source.Extension; profile.Mobile = source.Mobile; profile.Email = source.Email; profile.Website = source.Website; profile.UseForNew = source.UseForNew; profile.UseForReply = source.UseForReply;
        }

        private void SetSignature(object sender, RoutedEventArgs e)
        {
            try
            {
                SyncProfile();
                if (string.IsNullOrWhiteSpace(profile.TemplateName)) { MessageBox.Show(this, "กรุณาระบุชื่อ Template", "ข้อมูลไม่ครบ", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
                Directory.CreateDirectory(signatureRoot);
                string name = SafeName(profile.TemplateName);
                string backup = BackupExisting(name);
                string basePath = Path.Combine(signatureRoot, name);
                File.WriteAllText(basePath + ".htm", BuildSignatureDocumentHtml(), new UTF8Encoding(true));
                File.WriteAllText(basePath + ".txt", StripHtml(BuildSignatureHtml(false)), new UTF8Encoding(false));
                File.WriteAllText(basePath + ".rtf", "{\\rtf1\\ansi " + Rtf(StripHtml(BuildSignatureHtml(false))) + "}", Encoding.Default);
                var versions = DetectOfficeVersions();
                var preservedRibbon = new List<string>();
                foreach (string version in versions)
                {
                    if (version == "14.0")
                    {
                        // Outlook 2010 can hide the Signature command when an external
                        // tool writes NewSignature/ReplySignature. Keep the command
                        // visible and let Outlook own its default selection instead.
                        RemoveOutlook2010Defaults(version);
                        preservedRibbon.Add(version);
                    }
                    else SetDefaults(version, name);
                }
                var outlookRunning = Process.GetProcessesByName("OUTLOOK").Length > 0;
                string message = "ติดตั้ง Signature สำเร็จแล้ว\n\nชื่อ: " + name + "\nตำแหน่ง: " + signatureRoot + "\nOffice: " + string.Join(", ", versions.ToArray());
                if (preservedRibbon.Count > 0) message += "\nOutlook 2010: เก็บปุ่ม Signature ไว้ จึงต้องเลือก Default ใน Outlook เอง 1 ครั้ง";
                else message += "\nตั้งค่า: New messages + Replies/forwards";
                if (outlookRunning) message += "\n\nหมายเหตุ: Outlook กำลังเปิดอยู่ กรุณาปิดและเปิด Outlook ใหม่เพื่อโหลดค่าใหม่";
                if (!string.IsNullOrEmpty(backup)) message += "\nสำรองไฟล์เดิม: " + backup;
                ShowStatus("Set Signature สำเร็จ - พร้อมใช้ใน Classic Outlook");
                MessageBox.Show(this, message, "Set Signature สำเร็จ", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ShowStatus("ติดตั้งไม่สำเร็จ");
                MessageBox.Show(this, "ไม่สามารถติดตั้ง Signature ได้\n\n" + ex.Message, "เกิดข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<string> DetectOfficeVersions()
        {
            var versions = new List<string>();
            foreach (string version in new[] { "14.0", "15.0", "16.0" })
            {
                bool exists = false;
                string[] paths = { "Software\\Microsoft\\Office\\" + version + "\\Outlook", "Software\\Microsoft\\Office\\" + version + "\\Common\\General", "Software\\Microsoft\\Office\\" + version + "\\Common\\MailSettings", "Software\\Microsoft\\Office\\" + version, "Software\\WOW6432Node\\Microsoft\\Office\\" + version + "\\Outlook" };
                foreach (string path in paths)
                {
                    using (var key = Registry.CurrentUser.OpenSubKey(path)) if (key != null) exists = true;
                    using (var key = Registry.LocalMachine.OpenSubKey(path)) if (key != null) exists = true;
                }
                if (exists) versions.Add(version);
            }
            if (versions.Count == 0) versions.AddRange(new[] { "14.0", "15.0", "16.0" });
            return versions;
        }

        private void SetDefaults(string version, string name)
        {
            string generalPath = "Software\\Microsoft\\Office\\" + version + "\\Common\\General";
            using (var general = Registry.CurrentUser.CreateSubKey(generalPath))
            {
                if (general == null) throw new InvalidOperationException("ไม่สามารถเขียนตำแหน่ง Signature ของผู้ใช้ได้");
                general.SetValue("Signatures", "signatures", RegistryValueKind.String);
            }
            string path = "Software\\Microsoft\\Office\\" + version + "\\Common\\MailSettings";
            using (var key = Registry.CurrentUser.CreateSubKey(path))
            {
                if (key == null) throw new InvalidOperationException("ไม่สามารถเขียน Registry ของผู้ใช้ได้");
                // Outlook reads these defaults more reliably as REG_EXPAND_SZ.
                // Do not write DisableSignature/DisableSignatures: those are policy/UI controls.
                RemoveLegacyZeroFlag(key, "DisableSignatures");
                RemoveLegacyZeroFlag(key, "DisableSignature");
                key.SetValue("NewSignature", profile.UseForNew ? name : "", RegistryValueKind.ExpandString);
                key.SetValue("ReplySignature", profile.UseForReply ? name : "", RegistryValueKind.ExpandString);
            }
        }

        private void RemoveOutlook2010Defaults(string version)
        {
            string path = "Software\\Microsoft\\Office\\" + version + "\\Common\\MailSettings";
            using (var key = Registry.CurrentUser.OpenSubKey(path, true))
            {
                if (key == null) return;
                key.DeleteValue("NewSignature", false);
                key.DeleteValue("ReplySignature", false);
            }
        }

        private void RemoveLegacyZeroFlag(RegistryKey key, string valueName)
        {
            object value = key.GetValue(valueName, null, RegistryValueOptions.DoNotExpandEnvironmentNames);
            if (value is int && (int)value == 0) key.DeleteValue(valueName, false);
        }

        private string BackupExisting(string name)
        {
            var items = Directory.Exists(signatureRoot) ? Directory.GetFileSystemEntries(signatureRoot).Where(p => Path.GetFileName(p).StartsWith(name, StringComparison.OrdinalIgnoreCase)).ToArray() : new string[0];
            if (items.Length == 0) return "";
            string backup = Path.Combine(signatureRoot, "_SignatureStudioBackup", DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + name);
            Directory.CreateDirectory(backup);
            foreach (string item in items)
            {
                string dest = Path.Combine(backup, Path.GetFileName(item));
                if (Directory.Exists(item)) CopyDirectory(item, dest); else File.Copy(item, dest, true);
            }
            return backup;
        }

        private void CopyDirectory(string source, string destination)
        {
            Directory.CreateDirectory(destination);
            foreach (string file in Directory.GetFiles(source)) File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), true);
            foreach (string dir in Directory.GetDirectories(source)) CopyDirectory(dir, Path.Combine(destination, Path.GetFileName(dir)));
        }

        private string SafeName(string value)
        {
            string result = value.Trim();
            foreach (char invalid in Path.GetInvalidFileNameChars()) result = result.Replace(invalid.ToString(), "_");
            return result.Length == 0 ? "Signature" : result;
        }

        private string StripHtml(string html)
        {
            string text = Regex.Replace(html, "<br\\s*/?>", "\n", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "<[^>]+>", " ");
            return System.Net.WebUtility.HtmlDecode(Regex.Replace(text, "\\s+", " ")).Trim();
        }

        private string Rtf(string text)
        {
            return text.Replace("\\", "\\\\").Replace("{", "\\{").Replace("}", "\\}").Replace("\n", "\\par ");
        }

        private void ExportHtml(object sender, RoutedEventArgs e)
        {
            SyncProfile();
            var dialog = new SaveFileDialog { FileName = SafeName(profile.TemplateName) + ".html", Filter = "HTML file (*.html)|*.html|All files (*.*)|*.*" };
            if (dialog.ShowDialog() == true) { File.WriteAllText(dialog.FileName, BuildSignatureDocumentHtml(), new UTF8Encoding(true)); ShowStatus("Export HTML แล้ว"); }
        }

        private void OpenSignatureFolder(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(signatureRoot);
            Process.Start("explorer.exe", signatureRoot);
        }

        private void ShowStatus(string text)
        {
            status.Text = text;
            status.Foreground = text.Contains("สำเร็จ") ? new SolidColorBrush(Color.FromRgb(78, 124, 41)) : Muted;
        }
    }

    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            var app = new Application { ShutdownMode = ShutdownMode.OnMainWindowClose };
            app.Run(new MainWindow());
        }
    }
}
