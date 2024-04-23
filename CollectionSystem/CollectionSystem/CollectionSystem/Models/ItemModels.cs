using CommunityToolkit.Mvvm.ComponentModel;

namespace CollectionSystem.Models
{
    public partial class ItemModels : ObservableObject
    {
        [ObservableProperty]
        public string id;

        [ObservableProperty]
        public string collectionId;

        [ObservableProperty]
        public string name;

        [ObservableProperty]
        public string description;

        [ObservableProperty]
        public double price;

        [ObservableProperty]
        public string status;

        [ObservableProperty]
        public int rating;
    }
}
