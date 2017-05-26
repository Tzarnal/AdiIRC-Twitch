using System;
using System.Windows.Forms;

namespace Twitch___AdiIRC
{
    public partial class SettingsForm : Form
    {
        private Settings _settings;

        public SettingsForm(Settings settings)
        {
            _settings = settings;

            InitializeComponent();

            settingsBox.Items.Add("Show Timeouts/Bans", _settings.ShowTimeouts);
            settingsBox.Items.Add("Show Cheers", _settings.ShowCheers);
            settingsBox.Items.Add("Show (Re)Subscription Notification",_settings.ShowSubs);
            settingsBox.Items.Add("Show Badges", _settings.ShowBadges);
            settingsBox.Items.Add("Tab Inserts @ Before Names", _settings.AutoComplete);
        }

        private void UpdateSettingsFromSettingsBox()
        {
            for (var i = 0; i < settingsBox.Items.Count; i++)
            {
                object o = settingsBox.Items[i];

                switch (o.ToString())
                {
                    case "Show Cheers":
                        _settings.ShowCheers = settingsBox.GetItemChecked(i);
                        break;

                    case "Show Timeouts/Bans":
                        _settings.ShowTimeouts = settingsBox.GetItemChecked(i);
                        break;

                    case "Show (Re)Subscription Notification":
                        _settings.ShowSubs = settingsBox.GetItemChecked(i);
                        break;

                    case "Show Badges":
                        _settings.ShowBadges = settingsBox.GetItemChecked(i);
                        break;

                    case "Tab Inserts @ Before Names":
                        _settings.AutoComplete = settingsBox.GetItemChecked(i);
                        break;
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateSettingsFromSettingsBox();
            _settings.Save();
            Hide();
        }
    }
}
