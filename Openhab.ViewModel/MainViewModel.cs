﻿using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Openhab.Core;
using Openhab.Core.SDK;
using Openhab.Model;
using Openhab.Model.Messages;

namespace Openhab.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IOpenHAB _openHabsdk;
        private ObservableCollection<OpenHABSitemap> _sitemaps;
        private OpenHABSitemap _selectedSitemap;
        private OpenHABVersion _version;

        public ObservableCollection<OpenHABSitemap> Sitemaps
        {
            get { return _sitemaps; }
            set { Set(ref _sitemaps, value); }
        }

        public OpenHABSitemap SelectedSitemap
        {
            get { return _selectedSitemap; }
            set
            {
                if (Set(ref _selectedSitemap, value))
                {
                    if (_selectedSitemap.Widgets == null)
                    {
#pragma warning disable 4014
                        LoadWidgets();
#pragma warning restore 4014
                    }
                }
            }
        }

        public MainViewModel(IOpenHAB openHabsdk)
        {
            _openHabsdk = openHabsdk;

            Messenger.Default.Register<TriggerCommandMessage>(this, async msg => await TriggerCommand(msg));
#pragma warning disable 4014
            LoadData();
#pragma warning restore 4014
        }

        private async Task TriggerCommand(TriggerCommandMessage message)
        {
            await _openHabsdk.SendCommand(message.Item, message.Command);
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private async Task LoadData()
        {
           _version = await _openHabsdk.GetOpenHABVersion();
            var sitemaps = await _openHabsdk.LoadSiteMaps(_version);
            Sitemaps = new ObservableCollection<OpenHABSitemap>(sitemaps);
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private async Task LoadWidgets()
        {
            SelectedSitemap.Widgets = await _openHabsdk.LoadItemsFromSitemap(SelectedSitemap, _version);
        }
    }
}