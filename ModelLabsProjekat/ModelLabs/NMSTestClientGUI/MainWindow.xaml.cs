using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using FTN.Common;
using TelventDMS.Services.NetworkModelService.TestClient.Tests;

namespace NMSTestClientGUI
{
    public partial class MainWindow : Window
    {
        private TestGda testGda;

        public MainWindow()
        {
            InitializeComponent();
            testGda = new TestGda();
        }

        // Логика за специфичан упит за највишу оцену
        // Додај ово дугме у свој XAML или позови ову методу из постојећег
        private void BtnQueryMaxGidSegments_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowResult(RtbClamps, "Извршавање напредног упита..."); // Користим RtbClamps као пример из твог XAML-а

                List<long> segments = testGda.GetSegmentsOfMaxGidImpedance();

                if (segments.Count == 0)
                {
                    ShowResult(RtbClamps, "Нису пронађене деонице за импедансу са Max GID-ом.");
                    return;
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"--- РЕЗУЛТАТИ УПИТА ЗА MAX GID ---");
                sb.AppendLine($"Пронађено деоница: {segments.Count}");
                sb.AppendLine();

                foreach (long gid in segments)
                {
                    sb.AppendLine(DescribeResource(gid));
                    sb.AppendLine("--------------------------------");
                }

                ShowResult(RtbClamps, sb.ToString());
            }
            catch (Exception ex)
            {
                ShowResult(RtbClamps, "ГРЕШКА: " + ex.Message);
            }
        }

        // Помоћне методе које твој XAML већ користи
        private void ShowResult(System.Windows.Controls.RichTextBox rtb, string text)
        {
            rtb.Document.Blocks.Clear();
            rtb.Document.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(text)));
        }

        private string DescribeResource(long gid)
        {
            var rd = testGda.GetValues(gid);
            if (rd == null) return $"GID: 0x{gid:X16} [Null]";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"GID: 0x{gid:X16}");
            var name = rd.GetProperty(ModelCode.IDOBJ_NAME);
            sb.AppendLine($"IME: {name?.AsString() ?? "Nema imena"}");
            return sb.ToString();
        }
    }
}