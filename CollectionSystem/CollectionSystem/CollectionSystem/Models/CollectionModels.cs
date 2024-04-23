using CommunityToolkit.Mvvm.ComponentModel;

namespace CollectionSystem.Models
{
    public partial class CollectionModels : ObservableObject
    {
        [ObservableProperty]
        public string id;

        [ObservableProperty]
        public string name;
    }
}
