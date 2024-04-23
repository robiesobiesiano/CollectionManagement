using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using CollectionSystem.Models;
using CollectionSystem.Pages;

namespace CollectionSystem.ViewModels
{
    public partial class CollectionsViewModel : ObservableObject
    {
        private readonly string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "collections.txt");

        private static CollectionsViewModel? _instance;
        public static CollectionsViewModel Instance => _instance ??= new CollectionsViewModel();

        public ObservableCollection<CollectionModels> Collections { get; set; } = [];

        [ObservableProperty]
        public CollectionModels? selectedCollection;

        [RelayCommand]
        public async Task GoToItemsPage()
        {
            if (SelectedCollection != null)
            {
                await Shell.Current.GoToAsync($"{nameof(ItemsPage)}?CollectionId={SelectedCollection.Id}&CollectionName={SelectedCollection.Name}");
                SelectedCollection = null;
            }
        }

        [RelayCommand]
        public async Task OpenPopUp()
        {
            var name = await Application.Current.MainPage.DisplayPromptAsync("Dodaj kolekcje", "Podaj nazwe kolekcji: ");
            if (!string.IsNullOrWhiteSpace(name))
            {
                Collections.Add(new CollectionModels { Id = NanoidDotNet.Nanoid.Generate(size: 5), Name = name });
                await SaveCollections();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nie wprowadzono nazwy", "OK");
            }
        }


        public async Task DeleteCollection(string CollectionId)
        {
            var DeletedCollection = Collections.FirstOrDefault(c => c.Id == CollectionId);
            if (DeletedCollection != null)
            {
                Collections.Remove(DeletedCollection);
                await SaveCollections();

                var itemsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "items.txt");
                if (File.Exists(itemsFilePath))
                {
                    var allItems = await File.ReadAllLinesAsync(itemsFilePath);
                    var itemsToKeep = allItems.Where(line => !line.StartsWith($"{CollectionId}||"));

                    await File.WriteAllLinesAsync(itemsFilePath, itemsToKeep);
                }

            }
        }

        public async Task RenameCollection(string CollectionId, string newName)
        {
            var RenamedCollection = Collections.FirstOrDefault(c => c.Id == CollectionId);
            if (RenamedCollection != null)
            {
                RenamedCollection.Name = newName;
                await SaveCollections();
            }
        }

        public async Task SaveCollections()
        {
            var lines = Collections.Select(c => $"{c.Id}||{c.Name}").ToList();
            await File.WriteAllLinesAsync(filePath, lines);
        }

        public async Task LoadCollections()
        {
            Collections.Clear();
            if (File.Exists(filePath))
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                foreach (var line in lines)
                {
                    var parts = line.Split("||");
                    if (parts.Length == 2)
                    {
                        Collections.Add(new CollectionModels { Id = parts[0], Name = parts[1] });
                    }
                }
            }

        }

    }

}