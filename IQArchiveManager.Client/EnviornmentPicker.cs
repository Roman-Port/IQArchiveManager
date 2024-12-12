using IQArchiveManager.Client.Components;
using IQArchiveManager.Client.RdsModes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IQArchiveManager.Client
{
    public partial class EnviornmentPicker : Form
    {
        public EnviornmentPicker()
        {
            InitializeComponent();
        }

        private List<ConfigOption> options = new List<ConfigOption>();
        private ConfigOption optionDbPath;
        private ConfigOption optionIqaPath;
        private ConfigOption optionEditPath;
        private ConfigOption optionMovePath;

        private JObject config;

        private const string CONFIG_FILENAME = "enviornment.json";

        private void EnviornmentPicker_Load(object sender, EventArgs e)
        {
            //Create options
            optionDbPath = HelperAddOption(new ConfigOption(comboBox0, "db", true));
            optionIqaPath = HelperAddOption(new ConfigOption(comboBox1, "iqa", false));
            optionEditPath = HelperAddOption(new ConfigOption(comboBox2, "edit", false));
            optionMovePath = HelperAddOption(new ConfigOption(comboBox3, "move", false));

            //Load config from file
            if (File.Exists(CONFIG_FILENAME))
                config = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(CONFIG_FILENAME));
            else
                config = new JObject();

            //Init all
            foreach (var o in options)
            {
                o.Init(config);
                o.OnSelectionChanged += DropdownChanged;
            }

            //Update status
            UpdateStatus();
        }

        private void DropdownChanged(object sender, EventArgs e)
        {
            UpdateStatus();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            //Save options
            File.WriteAllText(CONFIG_FILENAME, JsonConvert.SerializeObject(config, Formatting.Indented));

            //Create enviornment
            IQEnviornment env = new IQEnviornment(optionIqaPath.SelectedValue, optionDbPath.SelectedValue, optionEditPath.SelectedValue, optionMovePath.SelectedValue);

            //Apply
            RdsPatchNoDelimiters.BRANDING_REMOVAL = Brandings;

            //Switch to main menu
            Hide();
            new MainMenu(env).ShowDialog();
            Show();
        }

        private void UpdateStatus()
        {
            bool ok = true;
            foreach (var o in options)
                ok = ok && o.SelectedValue != null;
            btnApply.Enabled = ok;
        }

        private ConfigOption HelperAddOption(ConfigOption option)
        {
            options.Add(option);
            return option;
        }

        class ConfigOption
        {
            public ConfigOption(ComboBox box, string key, bool isFile)
            {
                this.box = box;
                this.key = key;
                this.isFile = isFile;
            }

            public void Init(JObject rootCfg)
            {
                //Validate JSON objects
                this.rootCfg = rootCfg;
                if (!rootCfg.ContainsKey(key))
                    rootCfg.Add(key, new JObject());
                if (!Config.ContainsKey("paths"))
                    Config.Add("paths", new JArray());
                if (!Config.ContainsKey("index"))
                    Config.Add("index", -1);

                //Update dropbox
                RefreshDropdown();

                //Bind events
                box.SelectedValueChanged += Box_SelectedValueChanged;
            }

            private void RefreshDropdown()
            {
                //Suspend
                box.SuspendLayout();

                //Clear
                box.Items.Clear();

                //Add all saved paths
                foreach (var p in SavedPaths)
                    box.Items.Add((string)p);

                //Add "add" button
                box.Items.Add("[Add New Path...]");

                //Set selected
                box.SelectedIndex = SelectedIndex;

                //Resume
                box.ResumeLayout();
            }

            private void Box_SelectedValueChanged(object sender, EventArgs e)
            {
                //Check if we're not adding a new path
                if (box.SelectedIndex < SavedPaths.Count)
                {
                    SelectedIndex = box.SelectedIndex;
                } else
                {
                    //Open picker
                    if (isFile)
                    {
                        SaveFileDialog fd = new SaveFileDialog();
                        fd.Filter = "All Files (*.*)|*.*";
                        if (fd.ShowDialog() == DialogResult.OK)
                        {
                            SavedPaths.Add(fd.FileName);
                            SelectedIndex = box.SelectedIndex;
                            RefreshDropdown();
                        }
                        else
                        {
                            SelectedIndex = -1;
                            RefreshDropdown();
                        }
                    }
                    else
                    {
                        FolderBrowserDialog fd = new FolderBrowserDialog();
                        if (fd.ShowDialog() == DialogResult.OK)
                        {
                            SavedPaths.Add(fd.SelectedPath + Path.DirectorySeparatorChar);
                            SelectedIndex = box.SelectedIndex;
                            RefreshDropdown();
                        }
                        else
                        {
                            SelectedIndex = -1;
                            RefreshDropdown();
                        }
                    }
                }

                //Fire event
                OnSelectionChanged?.Invoke(sender, e);
            }

            private JObject Config { get => (JObject)rootCfg[key]; }
            private JArray SavedPaths { get => (JArray)Config["paths"]; }
            private int SelectedIndex { get => (int)Config["index"]; set => Config["index"] = value; }


            public string SelectedValue { get => SelectedIndex == -1 ? null : (string)SavedPaths[SelectedIndex]; }
            public event EventHandler OnSelectionChanged;


            private ComboBox box;
            private JObject rootCfg;
            private string key;
            private bool isFile;
        }

        private string[] Brandings
        {
            get
            {
                if (config.TryGetValue("brandings", out JToken tok))
                {
                    return tok.ToObject<string[]>();
                } else
                {
                    return new string[0];
                }
            }
        }

        private void btnEditStationBrandings_Click(object sender, EventArgs e)
        {
            StationBrandingsEditor f = new StationBrandingsEditor();
            f.Brandings = Brandings;
            f.ShowDialog();
            config["brandings"] = JArray.FromObject(f.Brandings);
        }
    }
}
