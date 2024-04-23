using CollectionSystem.ViewModels;
using System.Diagnostics;

namespace CollectionSystem
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = CollectionsViewModel.Instance;
            Debug.WriteLine($"Ścieżka do plików aplikacji: {Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CollectionsViewModel.Instance.LoadCollections();
        }
    }
}
