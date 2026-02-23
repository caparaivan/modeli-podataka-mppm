using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FTN.Common;
using FTN.ServiceContracts;

namespace ModelLabsApp
{
    public class GdaReaderForm : Form
    {
        private readonly ModelResourcesDesc modelResourcesDesc = new ModelResourcesDesc();

        private TextBox textBoxGetValuesGid;
        private TextBox textBoxRelatedSourceGid;
        private TextBox textBoxRelatedProperty;
        private TextBox textBoxRelatedType;
        private CheckBox checkBoxRelatedInverse;
        private ComboBox comboBoxExtentType;
        private RichTextBox richTextBoxOutput;

        public GdaReaderForm()
        {
            InitializeUi();
        }

        private void InitializeUi()
        {
            Text = "GDA Reader";
            Width = 980;
            Height = 720;
            StartPosition = FormStartPosition.CenterParent;

            var mainPanel = new TableLayoutPanel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.ColumnCount = 1;
            mainPanel.RowCount = 4;
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Controls.Add(mainPanel);

            mainPanel.Controls.Add(CreateGetValuesGroup(), 0, 0);
            mainPanel.Controls.Add(CreateGetExtentGroup(), 0, 1);
            mainPanel.Controls.Add(CreateGetRelatedGroup(), 0, 2);

            richTextBoxOutput = new RichTextBox();
            richTextBoxOutput.Dock = DockStyle.Fill;
            richTextBoxOutput.ReadOnly = true;
            mainPanel.Controls.Add(richTextBoxOutput, 0, 3);
        }

        private GroupBox CreateGetValuesGroup()
        {
            var group = new GroupBox();
            group.Text = "GetValues";
            group.Dock = DockStyle.Top;
            group.AutoSize = true;

            var panel = new FlowLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.AutoSize = true;

            panel.Controls.Add(new Label { Text = "GID (hex/dec):", AutoSize = true, Margin = new Padding(10, 10, 3, 3) });
            textBoxGetValuesGid = new TextBox { Width = 220, Text = "0x" };
            panel.Controls.Add(textBoxGetValuesGid);

            var button = new Button { Text = "Read", AutoSize = true };
            button.Click += buttonGetValuesOnClick;
            panel.Controls.Add(button);

            group.Controls.Add(panel);
            return group;
        }

        private GroupBox CreateGetExtentGroup()
        {
            var group = new GroupBox();
            group.Text = "GetExtentValues";
            group.Dock = DockStyle.Top;
            group.AutoSize = true;

            var panel = new FlowLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.AutoSize = true;

            panel.Controls.Add(new Label { Text = "Type:", AutoSize = true, Margin = new Padding(10, 10, 3, 3) });
            comboBoxExtentType = new ComboBox { Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
            comboBoxExtentType.DataSource = Enum.GetValues(typeof(DMSType));
            panel.Controls.Add(comboBoxExtentType);

            var button = new Button { Text = "Read", AutoSize = true };
            button.Click += buttonGetExtentOnClick;
            panel.Controls.Add(button);

            group.Controls.Add(panel);
            return group;
        }

        private GroupBox CreateGetRelatedGroup()
        {
            var group = new GroupBox();
            group.Text = "GetRelatedValues";
            group.Dock = DockStyle.Top;
            group.AutoSize = true;

            var panel = new FlowLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.AutoSize = true;

            panel.Controls.Add(new Label { Text = "Source GID:", AutoSize = true, Margin = new Padding(10, 10, 3, 3) });
            textBoxRelatedSourceGid = new TextBox { Width = 180, Text = "0x" };
            panel.Controls.Add(textBoxRelatedSourceGid);

            panel.Controls.Add(new Label { Text = "PropertyId:", AutoSize = true, Margin = new Padding(10, 10, 3, 3) });
            textBoxRelatedProperty = new TextBox { Width = 180, Text = "TERMINAL_CONDEQ" };
            panel.Controls.Add(textBoxRelatedProperty);

            panel.Controls.Add(new Label { Text = "Type:", AutoSize = true, Margin = new Padding(10, 10, 3, 3) });
            textBoxRelatedType = new TextBox { Width = 180, Text = "TERMINAL" };
            panel.Controls.Add(textBoxRelatedType);

            checkBoxRelatedInverse = new CheckBox { Text = "Inverse", AutoSize = true, Margin = new Padding(10, 8, 3, 3) };
            panel.Controls.Add(checkBoxRelatedInverse);

            var button = new Button { Text = "Read", AutoSize = true };
            button.Click += buttonGetRelatedOnClick;
            panel.Controls.Add(button);

            group.Controls.Add(panel);
            return group;
        }

        private void buttonGetValuesOnClick(object sender, EventArgs e)
        {
            try
            {
                long gid = ParseLongValue(textBoxGetValuesGid.Text);
                short type = ModelCodeHelper.ExtractTypeFromGlobalId(gid);
                List<ModelCode> properties = modelResourcesDesc.GetAllPropertyIds((DMSType)type);

                using (NetworkModelGDAProxy proxy = CreateAndOpenProxy())
                {
                    ResourceDescription rd = proxy.GetValues(gid, properties);
                    richTextBoxOutput.Text = FormatResourceDescriptions(new List<ResourceDescription> { rd });
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void buttonGetExtentOnClick(object sender, EventArgs e)
        {
            int iteratorId = 0;
            try
            {
                DMSType selectedType = (DMSType)comboBoxExtentType.SelectedItem;
                ModelCode modelCode = modelResourcesDesc.GetModelCodeFromType(selectedType);
                List<ModelCode> properties = modelResourcesDesc.GetAllPropertyIds(selectedType);
                var resourceDescriptions = new List<ResourceDescription>();

                using (NetworkModelGDAProxy proxy = CreateAndOpenProxy())
                {
                    iteratorId = proxy.GetExtentValues(modelCode, properties);
                    int resourcesLeft = proxy.IteratorResourcesLeft(iteratorId);

                    while (resourcesLeft > 0)
                    {
                        resourceDescriptions.AddRange(proxy.IteratorNext(50, iteratorId));
                        resourcesLeft = proxy.IteratorResourcesLeft(iteratorId);
                    }

                    proxy.IteratorClose(iteratorId);
                    iteratorId = 0;
                }

                richTextBoxOutput.Text = FormatResourceDescriptions(resourceDescriptions);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void buttonGetRelatedOnClick(object sender, EventArgs e)
        {
            int iteratorId = 0;
            try
            {
                long sourceGid = ParseLongValue(textBoxRelatedSourceGid.Text);
                ModelCode propertyId = ParseModelCode(textBoxRelatedProperty.Text);
                ModelCode type = ParseModelCode(textBoxRelatedType.Text);
                var association = new Association(propertyId, type, checkBoxRelatedInverse.Checked);

                var properties = new List<ModelCode>
                {
                    ModelCode.IDOBJ_GID,
                    ModelCode.IDOBJ_NAME,
                    ModelCode.IDOBJ_MRID,
                    ModelCode.IDOBJ_DESCRIPTION
                };
                var resourceDescriptions = new List<ResourceDescription>();

                using (NetworkModelGDAProxy proxy = CreateAndOpenProxy())
                {
                    iteratorId = proxy.GetRelatedValues(sourceGid, properties, association);
                    int resourcesLeft = proxy.IteratorResourcesLeft(iteratorId);

                    while (resourcesLeft > 0)
                    {
                        resourceDescriptions.AddRange(proxy.IteratorNext(50, iteratorId));
                        resourcesLeft = proxy.IteratorResourcesLeft(iteratorId);
                    }

                    proxy.IteratorClose(iteratorId);
                    iteratorId = 0;
                }

                richTextBoxOutput.Text = FormatResourceDescriptions(resourceDescriptions);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private static NetworkModelGDAProxy CreateAndOpenProxy()
        {
            NetworkModelGDAProxy proxy = new NetworkModelGDAProxy("NetworkModelGDAEndpoint");
            proxy.Open();
            return proxy;
        }

        private static long ParseLongValue(string input)
        {
            string value = input.Trim();
            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                return Convert.ToInt64(value.Substring(2), 16);
            }

            return Convert.ToInt64(value);
        }

        private static ModelCode ParseModelCode(string input)
        {
            string value = input.Trim();
            ModelCode modelCode;

            if (ModelCodeHelper.GetModelCodeFromString(value, out modelCode))
            {
                return modelCode;
            }

            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                return (ModelCode)long.Parse(value.Substring(2), System.Globalization.NumberStyles.HexNumber);
            }

            return (ModelCode)long.Parse(value);
        }

        private static string FormatResourceDescriptions(List<ResourceDescription> descriptions)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Resources count: {descriptions.Count}");
            sb.AppendLine(new string('-', 80));

            foreach (ResourceDescription rd in descriptions)
            {
                DMSType dmsType = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(rd.Id);
                sb.AppendLine($"GID: 0x{rd.Id:x16} ({dmsType})");

                foreach (Property property in rd.Properties)
                {
                    sb.AppendLine($"  {property.Id} = {FormatPropertyValue(property)}");
                }

                sb.AppendLine(new string('-', 80));
            }

            return sb.ToString();
        }

        private static string FormatPropertyValue(Property property)
        {
            if (property.Type == PropertyType.Reference)
            {
                return $"0x{property.AsReference():x16}";
            }

            if (property.Type == PropertyType.ReferenceVector || property.Type == PropertyType.Int64Vector)
            {
                List<long> values = property.AsLongs();
                if (values == null || values.Count == 0)
                {
                    return "[]";
                }

                var sb = new StringBuilder("[");
                for (int i = 0; i < values.Count; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }

                    sb.AppendFormat("0x{0:x16}", values[i]);
                }

                sb.Append(']');
                return sb.ToString();
            }

            return property.ToString();
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show(this, ex.Message, "GDA read error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
