﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZTn.BNet.BattleNet;
using ZTn.BNet.D3;
using ZTn.BNet.D3.Careers;
using ZTn.BNet.D3.DataProviders;
using ZTn.Pcl.D3Calculator.Annotations;
using ZTn.Pcl.D3Calculator.Models;

namespace ZTn.Pcl.D3Calculator.ViewModels
{
    class CareersViewModel : INotifyPropertyChanged
    {
        private BindableTask<Career> _career;
        public BnetAccount Account { get; }

        public ObservableCollection<IListViewRowData> Details { get; set; }

        public BindableTask<Career> Career
        {
            get { return _career; }
            set
            {
                _career = value;
                OnPropertyChanged();
            }
        }

        public string CareerTitle => Resources.Lang.Career.ToUpper();
        public string HeroesTitle => Resources.Lang.Heroes.ToUpper();

        public BusyIndicatorViewModel BusyIndicatorLoadingCareer { get; }

        public CareersViewModel(BnetAccount account)
        {
            Account = account;

            BusyIndicatorLoadingCareer = new BusyIndicatorViewModel { IsBusy = true, BusyMessage = Resources.Lang.LoadingCareer };
            Details = new ObservableCollection<IListViewRowData>(new[]
            {
                new TextListViewData { Label = "LABEL", Value = "TEXT" }
            });
            RefreshCareer();
        }

        public void RefreshCareer(FetchMode fetchMode = FetchMode.OnlineIfMissing)
        {
            Career = new BindableTask<Career>(LoadCareerAsync(fetchMode));
        }

        private async Task<Career> LoadCareerAsync(FetchMode fetchMode)
        {
            BusyIndicatorLoadingCareer.IsBusy = true;

            var dataProvider = DependencyService.Get<ICacheableD3DataProvider>();
            dataProvider.FetchMode = fetchMode;

            var d3Api = new D3ApiRequester
            {
                ApiKey = App.ApiKey,
                DataProvider = dataProvider,
                Host = Account.Host
            };

            var career = await d3Api.GetCareerFromBattleTagAsync(new BattleTag(Account.BattleTag));

            BusyIndicatorLoadingCareer.IsBusy = false;

            Details.Clear();
            Details.Add(new TextListViewData { Label = "Guild Name", Value = career.GuildName });
            Details.Add(new TextListViewData { Label = "Elites", Value = $"{career.Kills.Elites}" });
            Details.Add(new TextListViewData { Label = "Monsters", Value = $"{career.Kills.Monsters}" });
            Details.Add(new TextListViewData { Label = "Hardcore Monsters", Value = $"{career.Kills.HardcoreMonsters}" });
            Details.Add(new TextListViewData { Label = "Paragon Level", Value = $"{career.ParagonLevel}" });
            Details.Add(new TextListViewData { Label = "Paragon Level Hardcore", Value = $"{career.ParagonLevelHardcore}" });
            Details.Add(new TextListViewData { Label = "Paragon Level Season", Value = $"{career.ParagonLevelSeason}" });
            Details.Add(new TextListViewData { Label = "Paragon Level Season Hardcore", Value = $"{career.ParagonLevelSeasonHardcore}" });

            return career;
        }

        #region >> INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
