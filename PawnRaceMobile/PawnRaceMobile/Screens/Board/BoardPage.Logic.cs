using PawnRaceMobile.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace PawnRaceMobile
{
    public partial class BoardPage : ContentPage
    {
        private bool m_ControlEnabled;
        private Square m_Destination;
        private GameManager m_GameManager;
        private bool m_LocalMultiplayer;
        private Square m_Source;
        private HumanPlayer m_User;

        public BoardPage(bool userPlaysWhite, bool localMultiplayer)
        {
            m_LocalMultiplayer = localMultiplayer;
            m_ControlEnabled = m_BoardRotated = userPlaysWhite;

            InitializeComponent();
            InitializeBackground();
            SetNavBar();
            SetButtons();

            if (!localMultiplayer && userPlaysWhite)
            {
                HideStartButton();
                char[] gaps = SelectGaps();
                m_GapIndecies = gaps.Select(x => (x - 'a')).ToArray();
                Restart();
                DisplayEndgameAlert();
            }
            else
            {
                RenderGapSelectionPawns();
            }
        }

        public void SetUpGame(char whiteGap, char blackGap)
        {
            Core.Color userColor = m_BoardRotated ? Core.Color.White : Core.Color.Black;

            m_User = new HumanPlayer(userColor);
            m_User.TurnTaken += EnableControl;
            m_User.MoveProduced += DisableControl;

            Player opponent;
            if (m_LocalMultiplayer)
            {
                opponent = new HumanPlayer(userColor.Inverse());
                if (!m_BoardRotated)
                {
                    m_ControlEnabled = true;
                }
            }
            else
            {
                opponent = new RandomAI(userColor.Inverse());
            }

            m_GameManager = new GameManager(whiteGap, blackGap, m_User, opponent);
            m_GameManager.MoveMade += RenderChanges;
        }

        protected void OnPawnTapped(object sender, EventArgs e)
        {
            if (!m_ControlEnabled)
            {
                return;
            }
            //Getting the selected Square
            IPlayer currentPlayer = m_GameManager.CurrentPlayer;
            Square currentSquare = SquareFromImage(sender);

            if (m_Source == null)
            {
                //Necesary for displaying the valid moves
                if (currentSquare.IsOccupiedBy(currentPlayer.Color))
                {
                    m_Source = currentSquare;
                    DisplayAvailableMoves();
                }
            }
            else
            {
                m_Destination = SquareFromImage(sender);
                if (m_Destination != m_Source)
                {
                    Move move = (m_Destination.X != m_Source.X)
                    ? new Move(m_Source, m_Destination, true, false)
                    : new Move(m_Source, m_Destination);

                    ManageMove(move);
                }

                UndisplayAvailableMoves();
                m_Source = m_Destination = null;
            }
        }

        protected void OnSquareTapped(object sender, EventArgs e)
        {
            if (!m_ControlEnabled || m_Source == null)
            {
                return;
            }
            m_Destination = SquareFromImage(sender);
            Move move = (m_Destination.X != m_Source.X)
                ? new Move(m_Source, m_Destination, true, true)
                : new Move(m_Source, m_Destination);
            ManageMove(move);
            UndisplayAvailableMoves();
            m_Source = m_Destination = null;
        }

        private void DisableControl() => m_ControlEnabled = false;

        private void DisableControl(IPlayer _, Move __) => DisableControl();

        private void EnableControl() => m_ControlEnabled = true;

        private void FinishGame() => DisplayEndgameAlert();

        private async Task GoToMainMenu() => await Navigation.PopToRootAsync();

        private void ManageMove(Move move)
        {
            if (m_GameManager.IsValidMove(move))
            {
                m_User.ParseMove(move);
                if (m_GameManager.IsFinished)
                {
                    FinishGame();
                }
                else if (m_LocalMultiplayer)
                {
                    m_User = m_User.Opponent as HumanPlayer;
                    EnableControl();
                }
            }
        }

        private void Restart()
        {
            SetUpGame();
            RenderAllPawns();
            m_GameManager.CurrentPlayer.TakeTurn();
        }

        private char[] SelectGaps()
        {
            Random random = new Random();
            return new char[2]
            {
                (char)('a' + random.Next(8)), (char)('a' + random.Next(8))
            };
        }

        private void SetNavBar()
        {
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }

        private void SetUpGame()
                    => SetUpGame((char)('a' + m_GapIndecies[0]), (char)('a' + m_GapIndecies[1]));

        private Square SquareFromImage(object sender)
        {
            Image senderImage = (Image)sender;
            return m_GameManager.Board.GetSquare
                (Grid.GetColumn(senderImage), YBasedOnBoardRotation(Grid.GetRow(senderImage)));
        }
    }
}