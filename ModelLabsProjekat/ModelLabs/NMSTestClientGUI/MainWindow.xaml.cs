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
                Enum.TryParse(TbAssociationProperty.Text?.Trim(), true, out ModelCode prop);
                Enum.TryParse(TbAssociationType.Text?.Trim(), true, out ModelCode type);

                // Prvi korak: Pronalaženje povezanih GID-ova 
                Association association = new Association(prop, type);
                List<long> relatedIds = testGda.GetRelatedValues(sourceGid, association);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"=== REZULTAT ASOCIJACIJE ({relatedIds.Count} resursa) ===");
                sb.AppendLine();

                // Drugi korak (za 60 poena): Za svaki nađeni GID, pročitaj detalje 
                foreach (long id in relatedIds)
                {
                    ResourceDescription rd = testGda.GetValues(id);
                    if (rd != null)
                    {
                        sb.AppendLine(DescribeResource(rd)); // Koristi našu Hex-ready metodu
                        sb.AppendLine(new string('=', 40));
                    }
                }
                ShowResult(RtbRelatedValues, sb.ToString());
            }
            catch (Exception ex) { ShowResult(RtbRelatedValues, "GDA GREŠKA: " + ex.Message); }
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

        private static string DescribeResource(ResourceDescription rd)
        {
            if (rd == null) return "[null resource]";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== DETALJI RESURSA ===");
            sb.AppendLine($"Global ID: 0x{rd.Id:X16}");
            sb.AppendLine(new string('-', 35));

            foreach (Property prop in rd.Properties)
            {
                string valueDisplay = string.Empty;

                // Provera tipa atributa za lepši ispis 
                if (prop.Type == PropertyType.ReferenceVector)
                {
                    // Ako je lista (npr. Terminali), ispisujemo sve ID-ove odvojene zarezom
                    List<long> gids = prop.AsReferences();
                    valueDisplay = gids.Count > 0
                       ? string.Join(", ", gids.Select(g => $"0x{g:X16}"))
                        : "Empty List";
                }
                else if (prop.Type == PropertyType.Reference || prop.Id == ModelCode.IDOBJ_GID)
                {
                    // Pojedinačne reference i GID uvek u Hex-u
                    valueDisplay = $"0x{prop.AsLong():X16}";
                }
                else
                {
                    // Ostali tipovi (float, string, bool)
                    valueDisplay = prop.GetValue()?.ToString() ?? "null";
                }

                sb.AppendLine($"{prop.Id,-35} : {valueDisplay}");
            }
            return sb.ToString();
        }

        private static void ShowResult(RichTextBox rtb, string text)
        {
            rtb.Document.Blocks.Clear();
            rtb.Document.Blocks.Add(new Paragraph(new Run(text ?? string.Empty)));
        }

        private void RtbGetValues_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
