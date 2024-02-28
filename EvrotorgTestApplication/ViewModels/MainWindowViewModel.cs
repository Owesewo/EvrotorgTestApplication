using EvrotorgTestApplication.Models;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace EvrotorgTestApplication.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string openedFilePath;
        private bool isDataLoaded;
        private readonly string generalUrl = @"https://api.nbrb.by/exrates/rates";

        private ObservableCollection<RateModel> changedRatesCollection;
        private ObservableCollection<RateModel> cachedCurrencies;
        private ObservableCollection<RateModel> visibleRatesCollection;
        private RateModel selectedCurrency;

        private RelayCommand loadDataFromSite;
        private RelayCommand loadDataFromFile;
        private RelayCommand saveDataToFile;
        private RelayCommand saveDataToNewFile;

        private DateTime firstDateTime;
        private DateTime lastDateTime;
        private RelayCommand acceptDateTimeFilter;
        private RelayCommand clearDateTimeFilter;

        private decimal? changedRate;
        private string changedAbbreviation;
        private RelayCommand acceptDataChanges;
        private RelayCommand revertDataChanges;

        
        public MainWindowViewModel()
        {
            FirstDateTime = DateTime.Parse("01.01.1991");
            LastDateTime = DateTime.Now;

            IsDataLoaded = false;
        }

        public ObservableCollection<RateModel> VisibleRatesCollection
        {
            get
            {
                return this.visibleRatesCollection;
            }
            set
            {
                this.visibleRatesCollection = value;
                OnPropertyChanged("VisibleRatesCollection");
            }
        }

        public string OpenedFilePath
        {
            get
            {
                return this.openedFilePath;
            }
            set
            {
                this.openedFilePath = value;
                OnPropertyChanged("OpenedFilePath");
            }
        }

        public RateModel SelectedCurrency
        {
            get
            {
                return this.selectedCurrency;
            }
            set
            {
                this.selectedCurrency = value;

                if (!(selectedCurrency is null))
                {
                    this.ChangedAbbreviation = this.selectedCurrency.Cur_Abbreviation;
                    this.ChangedRate = this.selectedCurrency.Cur_OfficialRate;
                }
                
                OnPropertyChanged("SelectedCurrency");
            }
        }

        public bool IsDataLoaded
        {
            get
            {
                return this.isDataLoaded;
            }
            set
            {
                this.isDataLoaded = value;
                OnPropertyChanged("IsDataLoaded");
            }
        }

        public DateTime FirstDateTime
        {
            get
            {
                return this.firstDateTime;
            }
            set
            {
                this.firstDateTime = value;
                OnPropertyChanged("FirstDateTime");
            }
        }

        public DateTime LastDateTime
        {
            get
            {
                return this.lastDateTime;
            }
            set
            {
                this.lastDateTime = value;
                OnPropertyChanged("LastDateTime");
            }
        }

        public decimal? ChangedRate
        {
            get
            {
                return this.changedRate;
            }
            set
            {
                this.changedRate = value;
                OnPropertyChanged("ChangedRate");
            }
        }

        public string ChangedAbbreviation
        {
            get
            {
                return this.changedAbbreviation;
            }
            set
            {
                this.changedAbbreviation = value;
                OnPropertyChanged("ChangedAbbreviation");
            }
        }

        public RelayCommand LoadDataFromSite
        {
            get
            {
                return loadDataFromSite ??
                    (loadDataFromSite = new RelayCommand(async obj =>
                    {
                        var httpClient = new HttpClient();
                        var url = generalUrl + "?periodicity=0";

                        cachedCurrencies = await httpClient.GetFromJsonAsync<ObservableCollection<RateModel>>(url);
                        VisibleRatesCollection = cachedCurrencies;

                        IsDataLoaded = true;
                    }));
            }
        }

        public RelayCommand LoadDataFromFile
        {
            get
            {
                return loadDataFromFile ??
                    (loadDataFromFile = new RelayCommand(
                    async obj =>
                    {
                        var filePicker = new OpenFileDialog();
                        filePicker.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

                        if (!filePicker.ShowDialog().HasValue)
                            return;

                        OpenedFilePath = filePicker.FileName;
                        using (StreamReader reader = new StreamReader(filePicker.FileName, true))
                        {
                            var line = await reader.ReadLineAsync();
                            cachedCurrencies = JsonSerializer.Deserialize<ObservableCollection<RateModel>>(line);
                        }

                        VisibleRatesCollection = cachedCurrencies;

                        IsDataLoaded = true;
                    }));
            }
        }

        public RelayCommand SaveDataToFile
        {
            get
            {
                return saveDataToFile ??
                    (saveDataToFile = new RelayCommand(
                    async obj =>
                    {
                        using (var writer = new StreamWriter(OpenedFilePath, true))
                        {
                            var json = JsonSerializer.Serialize(VisibleRatesCollection);
                            await writer.WriteLineAsync(json);
                        }
                    },
                    obj =>
                    {
                        return !(VisibleRatesCollection is null) && !(OpenedFilePath is null);
                    }));
            }
        }

        public RelayCommand SaveDataToNewFile
        {
            get
            {
                return saveDataToNewFile ??
                    (saveDataToNewFile = new RelayCommand(
                    async obj =>
                    {
                        var fileSaver = new SaveFileDialog();
                        fileSaver.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                        if (!fileSaver.ShowDialog().HasValue)
                            return;

                        OpenedFilePath = fileSaver.FileName;
                        using (var writer = new StreamWriter(fileSaver.FileName, true))
                        {
                            var json = JsonSerializer.Serialize(VisibleRatesCollection);
                            await writer.WriteLineAsync(json);
                        }
                    },
                    obj =>
                    {
                        return !(VisibleRatesCollection is null);
                    }));
            }
        }

        public RelayCommand AcceptDateTimeFilter
        {
            get
            {
                return acceptDateTimeFilter ??
                    (acceptDateTimeFilter = new RelayCommand(
                    obj =>
                    {
                        //var newlist = new ObservableCollection<CurrencyRateModel>(VisibleCurrencies.Where(cur => cur.Cur_DateStart >= FirstDateTime && cur.Cur_DateEnd <= LastDateTime)).ToList();
                        //VisibleCurrencies = new ObservableCollection<CurrencyRateModel>(newlist);
                        throw new NotImplementedException();
                    }));
            }
        }

        public RelayCommand ClearDateTimeFilter
        {
            get
            {
                return clearDateTimeFilter ??
                    (clearDateTimeFilter = new RelayCommand(
                    obj =>
                    {
                        throw new NotImplementedException();
                    }));
            }
        }

        public RelayCommand AcceptDataChanges
        {
            get
            {
                return acceptDataChanges ??
                    (acceptDataChanges = new RelayCommand(
                    obj =>
                    {
                        var changedRate = new RateModel()
                        {
                            Cur_ID = SelectedCurrency.Cur_ID,
                            Date = SelectedCurrency.Date,
                            Cur_Abbreviation = ChangedAbbreviation,
                            Cur_Scale = SelectedCurrency.Cur_Scale,
                            Cur_Name = SelectedCurrency.Cur_Name,
                            Cur_OfficialRate = ChangedRate
                        };

                        var changedRates = new ObservableCollection<RateModel>();

                        foreach (var rate in VisibleRatesCollection)
                        {
                            if (rate.Cur_ID == changedRate.Cur_ID)
                            {
                                changedRates.Add(changedRate);
                                continue;
                            }

                            changedRates.Add(rate);
                        }

                        VisibleRatesCollection = changedRates;
                        SelectedCurrency = null;
                        ChangedAbbreviation = null;
                        ChangedRate = null;

                        if (this.changedRatesCollection is null)
                            this.changedRatesCollection = new ObservableCollection<RateModel>();

                        this.changedRatesCollection.Add(changedRate);
                    },
                    obj =>
                    {
                        return !(SelectedCurrency is null);
                    }));
            }
        }

        public RelayCommand RevertDataChanges
        {
            get
            {
                return revertDataChanges ??
                    (revertDataChanges = new RelayCommand(
                    obj =>
                    {
                        VisibleRatesCollection = cachedCurrencies;
                        changedRatesCollection?.Clear();
                    }));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
