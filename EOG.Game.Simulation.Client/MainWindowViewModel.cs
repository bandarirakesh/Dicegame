using EOG.Game.Simulation.DataModel;
using EOG.Game.Simulation.UIResources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EOG.Game.Simulation.Client
{
    public class MainWindowViewModel : PropertyChnageBase, IDataErrorInfo
    {
        public MainWindowViewModel()
        {
            GamePresets = new ObservableCollection<GamePreset>()
            {
                new GamePreset(){ NoOfPlayers = 3, NoOfGames= 100 },
                new GamePreset(){ NoOfPlayers = 4, NoOfGames= 100 },
                new GamePreset(){ NoOfPlayers = 5, NoOfGames= 100 },
                new GamePreset(){ NoOfPlayers = 5, NoOfGames= 1000 },
                new GamePreset(){ NoOfPlayers = 5, NoOfGames= 10000 },
                new GamePreset(){ NoOfPlayers = 5, NoOfGames= 100000 },
                new GamePreset(){ NoOfPlayers = 6, NoOfGames= 100 },
                new GamePreset(){ NoOfPlayers = 7, NoOfGames= 100 },
            };


            RunSimulationCommand = new RelayCommand(o => ExecuteRunSimulation(), o => SelectedGamePreset != null);
            CancelCommand = new RelayCommand(o => CancelSimulation());
            SelectionChangedCommand = new RelayCommand(o => HandleSelectedPointChangedEvent(o));
        }
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private Random randNum = new Random();

        #region Methods

        private void HandleSelectedPointChangedEvent(object o)
        {
            if (o is SelectionChangedEventArgs args)
            {
                if (args.AddedItems.Count == 0) return;
                var selectedPoint = (KeyValuePair<int, int>)args.AddedItems[0];
                var selectedGame = compGames.Find(x => x.GameId == selectedPoint.Key);
                Players.ToList().ForEach(x => x.IsWinner = false);
                Players.First(x => x.PlayerId == selectedGame.Winner.PlayerId).IsWinner = true;
            }
        }
        private void PopulatePlayers()
        {
            for (int i = 0; i < NoOfPlayers; i++)
            {
                Players.Add(new Player(i));
            }
        }

        private async void ExecuteRunSimulation()
        {
            compGames.Clear();
            ValueList.Clear();
            AverageValueList.Clear();
            Players.Clear();

            SetMinAndMax();
            PopulatePlayers();

            CancellationToken ct = tokenSource.Token;
            IsTaskRunning = true;
            try
            {
                await RunSimulation(ct);

                compGames.ToDictionary(x => x.GameId, x => x.NoOfTurns).ToList().ForEach(x =>
                {
                    ValueList.Add(x);
                });

                var values = compGames.Select(x => x.NoOfTurns);
                int avg = (int)Queryable.Average(values.AsQueryable());

                compGames.ToDictionary(x => x.GameId, x => avg).ToList().ForEach(x =>
                {
                    AverageValueList.Add(x);
                });

                OnPropertyChanged(nameof(ValueList));
                OnPropertyChanged(nameof(AverageValueList));
            }
            catch (Exception e)
            {
                Console.WriteLine($"{nameof(Exception)} thrown with message: {e.Message}");
                IsTaskRunning = false;
            }
            finally
            {
                IsTaskRunning = false;
            }
        }

        private Task RunSimulation(CancellationToken ct)
        {
            var task = Task.Run(() =>
            {
                ct.ThrowIfCancellationRequested();

                int count = 0;
                while (count < NoOfGames)
                {
                    var currentGame = new DiceGame(count + 1, NoOfPlayers);
                    compGames.Add(currentGame);

                    currentGame.PlayGame(randNum);

                    if (ct.IsCancellationRequested)
                    {
                        MessageBox.Show("Cancelled");
                        IsTaskRunning = false;
                        break;
                    }

                    count++;
                }
            }, ct);

            return task;
        }

        private void CancelSimulation()
        {
            tokenSource?.Cancel();
        }

        private void SetMinAndMax()
        {
            Minimum = 0;
            Maximum = NoOfGames;

            if (SelectedGamePreset.NoOfGames > 300)
            {
                Interval = 100;
            }
            else if (SelectedGamePreset.NoOfGames > 3000)
            {
                Interval = 1000;
            }
            else if (SelectedGamePreset.NoOfGames >= 100 && SelectedGamePreset.NoOfGames <= 300)
            {
                Interval = 10;
            }
            else
            {
                Interval = 1;
            }
        }

        #endregion

        #region Properties

        private List<DiceGame> compGames = new List<DiceGame>();


        private int minimum;
        public int Minimum
        {
            get { return minimum; }
            set
            {
                minimum = value;
                OnPropertyChanged(nameof(Minimum));
            }
        }

        private int maximum;
        public int Maximum
        {
            get { return maximum; }
            set
            {
                maximum = value;
                OnPropertyChanged(nameof(Maximum));
            }
        }

        private int interval;
        public int Interval
        {
            get { return interval; }
            set
            {
                interval = value;
                OnPropertyChanged(nameof(Interval));
            }
        }

        private bool isTaskRunning;
        public bool IsTaskRunning
        {
            get { return isTaskRunning; }
            set
            {
                isTaskRunning = value;
                OnPropertyChanged(nameof(IsTaskRunning));
            }
        }

        private bool isCancelled;
        public bool IsCancelled
        {
            get { return isCancelled; }
            set
            {
                isCancelled = value;
                OnPropertyChanged(nameof(IsCancelled));
            }
        }

        public ObservableCollection<Player> Players
        {
            get { return players; }
            set
            {
                players = value;
                OnPropertyChanged(nameof(Players));
            }
        }

        private GamePreset selectedGamePreset;
        public GamePreset SelectedGamePreset
        {
            get { return selectedGamePreset; }
            set
            {
                if (selectedGamePreset == value) return;
                selectedGamePreset = value;
                NoOfGames = selectedGamePreset.NoOfGames;
                NoOfPlayers = selectedGamePreset.NoOfPlayers;
                OnPropertyChanged(nameof(SelectedGamePreset));
            }
        }

        private int noOfGames;
        public int NoOfGames
        {
            get { return noOfGames; }
            set
            {
                noOfGames = value;
                OnPropertyChanged(nameof(NoOfGames));
            }
        }

        private int noOfPlayers;
        public int NoOfPlayers
        {
            get { return noOfPlayers; }
            set
            {
                noOfPlayers = value;
                OnPropertyChanged(nameof(NoOfPlayers));
            }
        }

        public ObservableCollection<GamePreset> GamePresets { get; set; } = new ObservableCollection<GamePreset>();

        private ObservableCollection<Player> players = new ObservableCollection<Player>();
        public ObservableCollection<KeyValuePair<int, int>> ValueList { get; set; } = new ObservableCollection<KeyValuePair<int, int>>();

        public ObservableCollection<KeyValuePair<int, int>> AverageValueList { get; set; } = new ObservableCollection<KeyValuePair<int, int>>();

        public ICommand RunSimulationCommand { get; set; }
        public ICommand SelectionChangedCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        #endregion

        #region Validation
        public string Error => string.Empty;

        public string this[string name]
        {
            get
            {
                string result = null;

                if (name == nameof(SelectedGamePreset))
                {
                    if (SelectedGamePreset == null)
                    {
                        result = "SelectedGamePreset cannot be empty.";
                    }
                }
                if (name == nameof(NoOfGames))
                {
                    if (NoOfGames <= 0)
                    {
                        result = "Number of games cannot be lees than or equal to 0.";
                    }
                }
                if (name == nameof(NoOfPlayers))
                {
                    if (NoOfPlayers <= 2)
                    {
                        result = "Number of players cannot be lees than or equal to 0.";
                    }
                }
                return result;
            }
        }
        #endregion

    }
}
