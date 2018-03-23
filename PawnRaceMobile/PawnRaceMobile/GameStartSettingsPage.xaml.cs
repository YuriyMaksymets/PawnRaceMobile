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
        private bool m_LocalMultiplayer;
        private char m_WhiteGap = 'A';
        private char m_BlackGap = 'A';
        private BoardPage m_BoardPage;

        public GameStartSettingsPage(BoardPage boardPage)
        {
            InitializeComponent();
            m_WhiteColorSelected = true;
            m_LocalMultiplayer = true;
            blackGapPicker.SelectedIndex = 0;
            whiteGapPicker.SelectedIndex = 0;
            m_BoardPage = boardPage;
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

        private void OnAiModeSelected(object sender, EventArgs e)
        {
            m_LocalMultiplayer = false;
            aiModeButton.IsEnabled = false;
            humanModeButton.IsEnabled = true;
        }

        private void OnHumanModeSelected(object sender, EventArgs e)
        {
            m_LocalMultiplayer = true;
            aiModeButton.IsEnabled = true;
            humanModeButton.IsEnabled = false;
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
        {
            m_BoardPage.SetUpGame(m_WhiteGap, m_BlackGap, m_WhiteColorSelected, m_LocalMultiplayer);
            await Navigation.PushAsync(m_BoardPage);
        }
    }
}