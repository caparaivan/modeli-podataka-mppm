using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using FTN.Common;
using TelventDMS.Services.NetworkModelService.TestClient.Tests;

namespace NMSTestClientGUI
{
    public partial class MainWindow : Window
    {
        private readonly TestGda testGda;

        public MainWindow()
        {
            InitializeComponent();
            testGda = new TestGda();
        }

        private void BtnGetValues_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                long gid = ParseGid(TbGetValuesGid.Text);
                ResourceDescription rd = testGda.GetValues(gid);

                if (rd == null)
                {
                    ShowResult(RtbGetValues, $"No resource found for GID 0x{gid:X16}.");
                    return;
                }

                ShowResult(RtbGetValues, DescribeResource(rd));
            }
            catch (Exception ex)
            {
                ShowResult(RtbGetValues, "ERROR: " + ex.Message);
            }
        }

        private void BtnGetExtent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Enum.TryParse(TbExtentModelCode.Text?.Trim(), true, out ModelCode modelCode))
                {
                    throw new ArgumentException("Invalid ModelCode value.");
                }

                List<long> ids = testGda.GetExtentValues(modelCode);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Extent for {modelCode}: {ids.Count} resource(s)");

                foreach (long id in ids)
                {
                    sb.AppendLine($"- 0x{id:X16}");
                }

                ShowResult(RtbExtentValues, sb.ToString());
            }
            catch (Exception ex)
            {
                ShowResult(RtbExtentValues, "ERROR: " + ex.Message);
            }
        }

        private void BtnGetRelated_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                long sourceGid = ParseGid(TbSourceGid.Text);

                if (!Enum.TryParse(TbAssociationProperty.Text?.Trim(), true, out ModelCode associationProperty))
                {
                    throw new ArgumentException("Invalid association property ModelCode.");
                }

                ModelCode associationType = 0;
                string associationTypeText = TbAssociationType.Text?.Trim();
                if (!string.IsNullOrEmpty(associationTypeText) && !Enum.TryParse(associationTypeText, true, out associationType))
                {
                    throw new ArgumentException("Invalid association type ModelCode.");
                }

                Association association = new Association(associationProperty, associationType);
                List<long> ids = testGda.GetRelatedValues(sourceGid, association);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Related values count: {ids.Count}");
                foreach (long id in ids)
                {
                    sb.AppendLine($"- 0x{id:X16}");
                }

                ShowResult(RtbRelatedValues, sb.ToString());
            }
            catch (Exception ex)
            {
                ShowResult(RtbRelatedValues, "ERROR: " + ex.Message);
            }
        }

        private void BtnQueryMaxGidSegments_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<long> segments = testGda.GetSegmentsOfMaxGidImpedance();

                if (segments.Count == 0)
                {
                    ShowResult(RtbClamps, "Nisu pronađene deonice za impedansu sa maksimalnim GID-om.");
                    return;
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("--- REZULTATI UPITA ZA MAX GID ---");
                sb.AppendLine($"Pronađeno deonica: {segments.Count}");
                sb.AppendLine();

                foreach (long gid in segments)
                {
                    ResourceDescription rd = testGda.GetValues(gid);
                    sb.AppendLine(DescribeResource(rd));
                    sb.AppendLine("--------------------------------");
                }

                ShowResult(RtbClamps, sb.ToString());
            }
            catch (Exception ex)
            {
                ShowResult(RtbClamps, "ERROR: " + ex.Message);
            }
        }

        private void BtnGetClamps_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                long acLineGid = ParseGid(TbAclineGid.Text);
                Association association = new Association(ModelCode.CONDEQ_TERMINALS, ModelCode.TERMINAL);
                List<long> terminalIds = testGda.GetRelatedValues(acLineGid, association);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Napomena: u trenutnom modelu ne postoji poseban CLAMP tip, pa se prikazuju povezani TERMINAL objekti.");
                sb.AppendLine($"ACLineSegment 0x{acLineGid:X16} -> povezani terminali: {terminalIds.Count}");

                foreach (long terminalId in terminalIds)
                {
                    sb.AppendLine($"- 0x{terminalId:X16}");
                }

                ShowResult(RtbClamps, sb.ToString());
            }
            catch (Exception ex)
            {
                ShowResult(RtbClamps, "ERROR: " + ex.Message);
            }
        }

        private void BtnClampMinLength_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                long acLineGid = ParseGid(TbAclineGid.Text);
                Association association = new Association(ModelCode.CONDEQ_TERMINALS, ModelCode.TERMINAL);
                List<long> terminalIds = testGda.GetRelatedValues(acLineGid, association);

                if (terminalIds.Count == 0)
                {
                    ShowResult(RtbClamps, "Nema povezanih terminala za zadati ACLineSegment.");
                    return;
                }

                long selected = terminalIds.Min();
                ShowResult(RtbClamps,
                    "Napomena: za model trenutno nije dostupna 'dužina clamp-a'.\n" +
                    "Prikazan je terminal sa najmanjim GID-om kao zamena:\n" +
                    $"0x{selected:X16}");
            }
            catch (Exception ex)
            {
                ShowResult(RtbClamps, "ERROR: " + ex.Message);
            }
        }

        private void BtnGetTerminals_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<long> terminalIds = testGda.GetExtentValues(ModelCode.TERMINAL);
                ShowResult(RtbTerminals, BuildTerminalListReport("Svi terminali", terminalIds));
            }
            catch (Exception ex)
            {
                ShowResult(RtbTerminals, "ERROR: " + ex.Message);
            }
        }

        private void BtnDisconnectedTerminals_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<long> terminalIds = testGda.GetExtentValues(ModelCode.TERMINAL);
                List<long> disconnected = new List<long>();

                foreach (long id in terminalIds)
                {
                    ResourceDescription rd = testGda.GetValues(id);
                    Property cn = rd?.GetProperty(ModelCode.TERMINAL_CONNECTIVITYNODE);
                    if (cn == null || cn.AsReference() == 0)
                    {
                        disconnected.Add(id);
                    }
                }

                ShowResult(RtbTerminals, BuildTerminalListReport("Diskonektovani terminali", disconnected));
            }
            catch (Exception ex)
            {
                ShowResult(RtbTerminals, "ERROR: " + ex.Message);
            }
        }

        private void BtnConnectedTerminals_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<long> terminalIds = testGda.GetExtentValues(ModelCode.TERMINAL);
                List<long> connected = new List<long>();

                foreach (long id in terminalIds)
                {
                    ResourceDescription rd = testGda.GetValues(id);
                    Property cn = rd?.GetProperty(ModelCode.TERMINAL_CONNECTIVITYNODE);
                    if (cn != null && cn.AsReference() != 0)
                    {
                        connected.Add(id);
                    }
                }

                ShowResult(RtbTerminals, BuildTerminalListReport("Konektovani terminali", connected));
            }
            catch (Exception ex)
            {
                ShowResult(RtbTerminals, "ERROR: " + ex.Message);
            }
        }

        private static long ParseGid(string raw)
        {
            string text = (raw ?? string.Empty).Trim();

            if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                return Convert.ToInt64(text.Substring(2), 16);
            }

            return long.Parse(text);
        }

        private static string BuildTerminalListReport(string title, List<long> ids)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{title}: {ids.Count}");
            foreach (long id in ids)
            {
                sb.AppendLine($"- 0x{id:X16}");
            }

            return sb.ToString();
        }

        private static string DescribeResource(ResourceDescription rd)
        {
            if (rd == null)
            {
                return "[null resource]";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"GID: 0x{rd.Id:X16}");

            Property name = rd.GetProperty(ModelCode.IDOBJ_NAME);
            if (name != null)
            {
                sb.AppendLine($"IME: {name.AsString()}");
            }

            return sb.ToString();
        }

        private static void ShowResult(RichTextBox rtb, string text)
        {
            rtb.Document.Blocks.Clear();
            rtb.Document.Blocks.Add(new Paragraph(new Run(text ?? string.Empty)));
        }
    }
}
