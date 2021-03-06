﻿//#######################################################################
//Diversity Mobile Synchronization
//Project Homepage:  http://www.diversitymobile.net
//Copyright (C) 2011  Tobias Schneider, Lehrstuhl Angewndte Informatik IV, Universität Bayreuth
//
//This program is free software; you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation; either version 2 of the License, or
//(at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License along
//with this program; if not, write to the Free Software Foundation, Inc.,
//51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
//#######################################################################
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Diversity_Synchronization;
using Diversity_Synchronization.Options.Language;
using Diversity_Synchronization.Options;

namespace Diversity_Synchronization_Gui.Options
{
    /// <summary>
    /// Interaktionslogik für SearchOptionsWindow.xaml
    /// </summary>
    public partial class SearchOptionsWindow : Window
    {
        public SearchOptionsWindow()
        {
            InitializeComponent();

            RefreshLanguage();
            UpdateSearchOptionsView();
        }

        private void RefreshLanguage()
        {
            ILanguage lf = OptionsAccess.Language;

            this.Title = lf.getLanguageString(801);
            this.menuItemMenu.Header = lf.getLanguageString(802);
            this.menuItemSaveAndClose.Header = lf.getLanguageString(803);
            this.menuItemCancel.Header = lf.getLanguageString(804);
            this.menuItemHelp.Header = lf.getLanguageString(805);
            this.textPageCaption.Content = lf.getLanguageString(821);
            this.textPageDescription.Text = lf.getLanguageString(822);
            this.groupBoxGeneralSettings.Header = lf.getLanguageString(823);
            this.labelTruncateDataItems.Content = lf.getLanguageString(824);
            this.checkBoxTruncateDataItems.Content = lf.getLanguageString(825);
            this.labelIncludeMultimedia.Content = lf.getLanguageString(826);
            this.checkBoxIncludeMultimedia.Content = lf.getLanguageString(827);
            this.labelMaxItems.Content = lf.getLanguageString(828);
            this.buttonSave.Content = lf.getLanguageString(851);
            this.buttonCancel.Content = lf.getLanguageString(852);
        }

        /**
         * Aktualisiert die Optionen in der Ansicht
         */
        private void UpdateSearchOptionsView()
        {
            SearchOptions so = OptionsAccess.SearchOptions;

            this.checkBoxTruncateDataItems.IsChecked = so.TruncateDataItems;
            this.checkBoxIncludeMultimedia.IsChecked = so.IncludeMultimedia;
            this.textBoxMaxItems.Text = Convert.ToString(so.MaxItems);
        }

        /**
         * Speichert die Optionen ab
         */
        private void SaveSearchOptions()
        {
            SearchOptions so = OptionsAccess.SearchOptions;

            if (this.checkBoxTruncateDataItems.IsChecked == null || this.checkBoxTruncateDataItems.IsChecked == false)
            {
                so.TruncateDataItems = false;
            }
            else
            {
                so.TruncateDataItems = true;
            }
            if (this.checkBoxIncludeMultimedia.IsChecked == null || this.checkBoxIncludeMultimedia.IsChecked == false)
            {
                so.IncludeMultimedia = false;
            }
            else
            {
                so.IncludeMultimedia = true;
            }
            so.MaxItems = Convert.ToInt32(this.textBoxMaxItems.Text);
            OptionsAccess.SearchOptions = so;

            DialogResult = true;
        }

        #region Button Events

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveSearchOptions();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #endregion

        #region Menu Events

        private void menuItemSaveAndClose_Click(object sender, RoutedEventArgs e)
        {
            SaveSearchOptions();
        }

        private void menuItemCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void menuItemHelp_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }

        #endregion
    }
}
