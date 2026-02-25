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
