using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PawnRaceMobile
{
    public partial class GameStartSettingsPage : ContentPage
    {
        private const string c_SelectedSide = "Selected side: ";
        private const string c_OpponentDecides = "Opponents decides, where the gaps are";
        private const string c_GapPositions = "Selected gaps are: ";

        private bool m_WhiteColorSelected;
        private char m_WhiteGap = 'A';
        private char m_BlackGap = 'A';

        public GameStartSettingsPage()
        {
            InitializeComponent();
            m_WhiteColorSelected = true;
            blackGapPicker.SelectedIndex = 0;
            whiteGapPicker.SelectedIndex = 0;
        }

        private void OnBlackColorSelected(object sender, EventArgs e)
        {
            m_WhiteColorSelected = false;
            whiteColorButton.IsEnabled = true;
            blackColorButton.IsEnabled = false;
            gapLayout.IsVisible = true;
            colorLabel.Text = c_SelectedSide + "Black";
            UpdateGapLabel();
        }

        private void OnWhiteColorSelected(object sender, EventArgs e)
        {
            m_WhiteColorSelected = true;
            whiteColorButton.IsEnabled = false;
            blackColorButton.IsEnabled = true;
            gapLayout.IsVisible = false;
            colorLabel.Text = c_SelectedSide + "White";
            gapLabel.Text = c_OpponentDecides;
        }

        private void BlackGapPicked(object sender, EventArgs e)
        {
            Picker picker = sender as Picker;
            m_BlackGap = char.Parse(picker.SelectedItem.ToString());
            UpdateGapLabel();
        }

        private void WhiteGapPicked(object sender, EventArgs e)
        {
            Picker picker = sender as Picker;
            m_WhiteGap = char.Parse(picker.SelectedItem.ToString());
            UpdateGapLabel();
        }

        private void UpdateGapLabel()
        {
            if (!m_WhiteColorSelected)
            {
                gapLabel.Text = c_GapPositions + m_WhiteGap + "(white) " + m_BlackGap + "(black)";
            }
        }

        private async void OnStartClicked(object sender, EventArgs e)
            => await Navigation
                .PushAsync(new BoardPage(m_WhiteGap, m_BlackGap, m_WhiteColorSelected));
    }
}