﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BrushFactory.Properties;

namespace BrushFactory.Gui
{
    internal partial class BrushFactoryPreferences : Form
    {
        private readonly IBrushFactorySettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrushFactoryPreferences" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <exception cref="ArgumentNullException"><paramref name="settings"/> is null.</exception>
        public BrushFactoryPreferences(IBrushFactorySettings settings)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            InitializeComponent();
            Icon = Resources.Icon;
            CenterToScreen();
        }

        #region Methods (overridden)
        /// <summary>
        /// Configures the drawing area and loads text localizations.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Sets the text and tooltips based on language.
            bttnCancel.Text = Localization.Strings.Cancel;
            bttnSave.Text = Localization.Strings.SavePreferences;
            chkbxLoadDefaultBrushes.Text = Localization.Strings.LoadDefaultBrushes;
            bttnAddFolder.Text = Localization.Strings.AddFolder;
            txtBrushLocations.Text = Localization.Strings.BrushLocations;
            tooltip.SetToolTip(bttnCancel, Localization.Strings.CancelTip);
            tooltip.SetToolTip(bttnSave, Localization.Strings.SavePreferencesTip);
            tooltip.SetToolTip(chkbxLoadDefaultBrushes, Localization.Strings.LoadDefaultBrushesTip);
            tooltip.SetToolTip(bttnAddFolder, Localization.Strings.AddFolderTip);
            tooltip.SetToolTip(txtbxBrushLocations, Localization.Strings.BrushLocationsTextboxTip);

            chkbxLoadDefaultBrushes.Checked = settings.UseDefaultBrushes;
            foreach (string item in settings.CustomBrushDirectories)
            {
                txtbxBrushLocations.AppendText(item + Environment.NewLine);
            }
        }
        #endregion

        #region Methods (not event handlers)
        /// <summary>
        /// Saves values to the registry from the gui.
        /// </summary>
        public void SaveSettings()
        {
            string[] values = txtbxBrushLocations.Text.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.RemoveEmptyEntries);

            settings.CustomBrushDirectories = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
            settings.UseDefaultBrushes = chkbxLoadDefaultBrushes.Checked;
        }
        #endregion

        #region Methods (event handlers)

        /// <summary>
        /// Allows the user to browse for a folder to add as a directory.
        /// </summary>
        private void bttnAddFolder_Click(object sender, EventArgs e)
        {
            //Opens a folder browser.
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.RootFolder = Environment.SpecialFolder.Desktop;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                //Appends the chosen directory to the textbox of directories.
                if (txtbxBrushLocations.Text != string.Empty)
                {
                    txtbxBrushLocations.AppendText(Environment.NewLine);
                }

                txtbxBrushLocations.AppendText(dlg.SelectedPath);
            }
        }

        /// <summary>
        /// Cancels and doesn't apply the preference changes.
        /// </summary>
        private void bttnCancel_Click(object sender, EventArgs e)
        {
            //Disables the button so it can't accidentally be called twice.
            bttnCancel.Enabled = false;

            Close();
        }

        /// <summary>
        /// Accepts and applies the preference changes.
        /// </summary>
        private void bttnSave_Click(object sender, EventArgs e)
        {
            //Disables the button so it can't accidentally be called twice.
            //Ensures settings will be saved.
            bttnSave.Enabled = false;

            SaveSettings();

            DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion
    }
}
