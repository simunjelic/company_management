using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praksa_projectV1.ViewModels
{
    public class WorkingCardViewModel : ViewModelBase
    {
        WorkingCardRepository cardRespository { get; set; }

        public WorkingCardViewModel()
        {
            cardRespository = new();
            gettAllDataFromCard();


        }

        private ObservableCollection<WorkingCard> _cardRecords;
        public ObservableCollection<WorkingCard> CardRecords
        {
            get
            {
                return _cardRecords;
            }
            set
            {
                _cardRecords = value;
                OnPropertyChanged(nameof(CardRecords));
            }
        }
        private WorkingCard _selectedItem;
        public WorkingCard? SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                }
            }
        }

        public async void gettAllDataFromCard()
        {
            var card = await cardRespository.GetAllData();
            CardRecords = new ObservableCollection<WorkingCard>(card);

        }
    }
}
