using FTN.Common;
using FTN.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;

namespace FTN.Services.NetworkModelService.WpfClient
{
    public partial class MainWindow : Window
    {
        private readonly ModelResourcesDesc modelResourcesDesc = new ModelResourcesDesc();

        public MainWindow()
        {
            InitializeComponent();
            FillComboBoxes();
        }

        private void FillComboBoxes()
        {
            var classModelCodes = Enum.GetValues(typeof(ModelCode))
                .Cast<ModelCode>()
                .Where(IsClassModelCode)
                .OrderBy(x => x.ToString())
                .ToList();

            ExtentTypeComboBox.ItemsSource = classModelCodes;
            ExtentTypeComboBox.SelectedItem = ModelCode.ACLINESEGMENT;

            var referenceModelCodes = Enum.GetValues(typeof(ModelCode))
                .Cast<ModelCode>()
                .Where(IsReferenceProperty)
                .OrderBy(x => x.ToString())
                .ToList();

            RelatedPropertyComboBox.ItemsSource = referenceModelCodes;
            RelatedPropertyComboBox.SelectedItem = ModelCode.TERMINAL_CONDEQ;

            RelatedTypeComboBox.ItemsSource = classModelCodes;
            RelatedTypeComboBox.SelectedItem = ModelCode.TERMINAL;
        }

        private static bool IsClassModelCode(ModelCode modelCode)
        {
            return ((long)modelCode & (long)ModelCodeMask.MASK_ATTRIBUTE_TYPE) == 0
                && ((long)modelCode & (long)ModelCodeMask.MASK_TYPE) != 0;
        }

        private static bool IsReferenceProperty(ModelCode modelCode)
        {
            return ((long)modelCode & (long)ModelCodeMask.MASK_ATTRIBUTE_TYPE) == 0x09;
        }

        private void GetValues_Click(object sender, RoutedEventArgs e)
        {
            ExecuteSafe(() =>
            {
                long gid = ParseLongValue(GetValuesGidTextBox.Text);
                var type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(gid);
                var properties = modelResourcesDesc.GetAllPropertyIds(type);

                using (var proxy = CreateProxy())
                {
                    var rd = proxy.GetValues(gid, properties);
                    PrintResult("GetValues", rd == null ? "Nema podataka za prosleđeni GID." : ResourceDescriptionToText(rd));
                }
            });
        }

        private void GetExtentValues_Click(object sender, RoutedEventArgs e)
        {
            ExecuteSafe(() =>
            {
                var modelCode = (ModelCode)ExtentTypeComboBox.SelectedItem;
                var properties = modelResourcesDesc.GetAllPropertyIds(modelCode);

                using (var proxy = CreateProxy())
                {
                    int iteratorId = proxy.GetExtentValues(modelCode, properties);
                    var rows = ReadIterator(proxy, iteratorId);
                    PrintResult("GetExtentValues", FormatResources(rows));
                }
            });
        }

        private void GetRelatedValues_Click(object sender, RoutedEventArgs e)
        {
            ExecuteSafe(() =>
            {
                long source = ParseLongValue(RelatedSourceTextBox.Text);
                var association = new Association(
                    (ModelCode)RelatedPropertyComboBox.SelectedItem,
                    (ModelCode)RelatedTypeComboBox.SelectedItem,
                    InverseCheckBox.IsChecked == true);

                var properties = new List<ModelCode>
                {
                    ModelCode.IDOBJ_MRID,
                    ModelCode.IDOBJ_NAME,
                    ModelCode.IDOBJ_DESCRIPTION
                };

                using (var proxy = CreateProxy())
                {
                    int iteratorId = proxy.GetRelatedValues(source, properties, association);
                    var rows = ReadIterator(proxy, iteratorId);
                    PrintResult("GetRelatedValues", FormatResources(rows));
                }
            });
        }

        private void RunAdvancedQuery_Click(object sender, RoutedEventArgs e)
        {
            ExecuteSafe(() =>
            {
                using (var proxy = CreateProxy())
                {
                    var acLineProperties = modelResourcesDesc.GetAllPropertyIds(ModelCode.ACLINESEGMENT);
                    int extentIterator = proxy.GetExtentValues(ModelCode.ACLINESEGMENT, acLineProperties);
                    var acLineSegments = ReadIterator(proxy, extentIterator);

                    if (!acLineSegments.Any())
                    {
                        PrintResult("Napredni primer", "Nema ACLineSegment instanci u modelu.");
                        return;
                    }

                    long maxGid = acLineSegments.Max(x => x.Id);

                    var terminalProps = new List<ModelCode>
                    {
                        ModelCode.IDOBJ_NAME,
                        ModelCode.IDOBJ_MRID,
                        ModelCode.TERMINAL_CONNECTIVITYNODE
                    };

                    var association = new Association(ModelCode.TERMINAL_CONDEQ, ModelCode.TERMINAL, true);
                    int relatedIterator = proxy.GetRelatedValues(maxGid, terminalProps, association);
                    var terminals = ReadIterator(proxy, relatedIterator);

                    var sb = new StringBuilder();
                    sb.AppendLine($"ACLineSegment sa najvećim GID-om: 0x{maxGid:X16} ({maxGid})");
                    sb.AppendLine("Vezani Terminal-i:");
                    sb.AppendLine(FormatResources(terminals));

                    PrintResult("Napredni primer", sb.ToString());
                }
            });
        }

        private static NetworkModelGDAProxy CreateProxy()
        {
            var proxy = new NetworkModelGDAProxy("NetworkModelGDAEndpoint");
            proxy.Open();
            return proxy;
        }

        private static List<ResourceDescription> ReadIterator(NetworkModelGDAProxy proxy, int iteratorId)
        {
            var resources = new List<ResourceDescription>();
            const int chunkSize = 50;

            try
            {
                int left = proxy.IteratorResourcesLeft(iteratorId);

                while (left > 0)
                {
                    var chunk = proxy.IteratorNext(chunkSize, iteratorId);
                    resources.AddRange(chunk);
                    left = proxy.IteratorResourcesLeft(iteratorId);
                }
            }
            finally
            {
                proxy.IteratorClose(iteratorId);
            }

            return resources;
        }

        private static long ParseLongValue(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Vrednost nije uneta.");
            }

            input = input.Trim();

            if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                return long.Parse(input.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return long.Parse(input, CultureInfo.InvariantCulture);
        }

        private static string ResourceDescriptionToText(ResourceDescription rd)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"GID: 0x{rd.Id:X16} ({rd.Id})");

            foreach (var prop in rd.Properties)
            {
                sb.AppendLine($"  - {prop.Id}: {PropertyValueToText(prop)}");
            }

            return sb.ToString();
        }

        private static string FormatResources(List<ResourceDescription> resources)
        {
            if (resources == null || resources.Count == 0)
            {
                return "Nema rezultata.";
            }

            var sb = new StringBuilder();
            foreach (var rd in resources)
            {
                sb.Append(ResourceDescriptionToText(rd));
                sb.AppendLine();
            }

            sb.AppendLine($"Ukupno: {resources.Count}");
            return sb.ToString();
        }

        private static string PropertyValueToText(Property property)
        {
            switch (property.Type)
            {
                case PropertyType.Reference:
                    return $"0x{property.AsReference():X16}";
                case PropertyType.ReferenceVector:
                    return string.Join(", ", property.AsReferences().Select(x => $"0x{x:X16}"));
                case PropertyType.String:
                    return property.AsString();
                case PropertyType.Bool:
                    return property.AsBool().ToString();
                case PropertyType.Float:
                    return property.AsFloat().ToString(CultureInfo.InvariantCulture);
                case PropertyType.Int32:
                    return property.AsInt().ToString(CultureInfo.InvariantCulture);
                case PropertyType.Int64:
                    return property.AsLong().ToString(CultureInfo.InvariantCulture);
                case PropertyType.Enum:
                    return property.AsEnum().ToString(CultureInfo.InvariantCulture);
                default:
                    return property.ToString();
            }
        }

        private void PrintResult(string operation, string text)
        {
            OutputTextBox.Text = $"[{DateTime.Now:HH:mm:ss}] {operation}{Environment.NewLine}{text}";
        }

        private void ExecuteSafe(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                PrintResult("Greška", ex.ToString());
            }
        }
    }
}
