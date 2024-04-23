using CollectionSystem.Models;
using CollectionSystem.Views;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NanoidDotNet;
using System.Collections.ObjectModel;

namespace CollectionSystem.ViewModels
{
    [QueryProperty(nameof(CollectionId), nameof(CollectionId))]
    [QueryProperty(nameof(CollectionName), nameof(CollectionName))]
    public partial class ItemViewModel : ObservableObject
    {
        private readonly string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "items.txt");

        private static ItemViewModel? _instance;
        public static ItemViewModel Instance => _instance ??= new ItemViewModel();

        [ObservableProperty]
        public string? collectionId;

        [ObservableProperty]
        public string? collectionName;

        public ObservableCollection<ItemModels> Items { get; set; } = [];

        private NewItemPopUp? currentPopup;
        private EditItemPopUp? editPopup;

        [RelayCommand]
        public void OpenPopUp()
        {
            currentPopup = new NewItemPopUp();
            Shell.Current.CurrentPage.ShowPopup(currentPopup);
        }

        [RelayCommand]
        public async Task DeleteCollection()
        {
            await CollectionsViewModel.Instance.DeleteCollection(CollectionId);
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        public async Task RenameCollection()
        {
            var newName = await Application.Current.MainPage.DisplayPromptAsync("Zmień nazwe", "wpisz nowa nazwe: ");
            if (!string.IsNullOrEmpty(newName))
            {
                await CollectionsViewModel.Instance.RenameCollection(CollectionId, newName);
                CollectionName = newName;
                await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        public void ClosePopUp()
        {
            if (currentPopup != null)
            {
                currentPopup.Close();
                currentPopup = null;
                ClearEntries();
            }
        }

        [RelayCommand]
        public void CloseEditPopUp()
        {
            if (editPopup != null)
            {
                editPopup.Close();
                editPopup = null;
                ClearEntries();
            }
        }

        [ObservableProperty]
        public string? enteredName;

        [ObservableProperty]
        public string? enteredDescription;

        [ObservableProperty]
        public string? enteredStatus;

        [ObservableProperty]
        public string? enteredPrice;

        [ObservableProperty]
        public string? enteredRating;

        private void ClearEntries()
        {
            EnteredName = string.Empty;
            EnteredDescription = string.Empty;
            EnteredPrice = string.Empty;
            EnteredStatus = string.Empty;
            EnteredRating = string.Empty;
        }

        [RelayCommand]
        public async Task AddItem()
        {
            if (currentPopup == null)
                return;

            if (EnteredDescription != null &&
                EnteredName != null &&
                EnteredPrice != null &&
                EnteredStatus != null &&
                EnteredRating != null)
            {

                ItemModels newItem = new ItemModels
                {
                    Id = Nanoid.Generate(size: 5),
                    CollectionId = CollectionId,
                    Name = EnteredName,
                    Description = EnteredDescription,
                    Price = double.Parse(EnteredPrice),
                    Status = EnteredStatus,
                    Rating = int.Parse(EnteredRating)
                };

                Items.Add(newItem);
                await SaveItems();

                ClearEntries();

                currentPopup.Close();
                currentPopup = null;
            }

        }

        [ObservableProperty]
        private ItemModels? selectedItem;

        [RelayCommand]
        public async Task EditItemPopUp(ItemModels item)
        {
            SelectedItem = item;
            EnteredName = item.Name;
            EnteredDescription = item.Description;
            EnteredPrice = item.Price.ToString();
            EnteredStatus = item.Status;
            EnteredRating = item.Rating.ToString();

            editPopup = new EditItemPopUp();
            Shell.Current.CurrentPage.ShowPopup(editPopup);
        }

        [RelayCommand]
        public async Task EditItem()
        {
            if (editPopup == null || SelectedItem == null)
                return;

            var item = Items.FirstOrDefault(i => i.Id == SelectedItem.Id);
            if (item != null)
            {
                item.Name = EnteredName;
                item.Description = EnteredDescription;
                item.Price = double.Parse(EnteredPrice);
                item.Status = EnteredStatus;
                item.Rating = int.Parse(EnteredRating);

                await SaveItems();

                editPopup.Close();
                editPopup = null;
                ClearEntries();
            }
        }

        [RelayCommand]
        public async Task DeleteItem(ItemModels itemToDelete)
        {
            if (itemToDelete == null) return;

            Items.Remove(itemToDelete);

            await SaveItems();
        }

        public async Task SaveItems()
        {
            var existingLines = File.Exists(filePath) ? await File.ReadAllLinesAsync(filePath) : [];
            var newLines = existingLines.Where(line => !line.StartsWith($"{CollectionId}||")).ToList();
            newLines.AddRange(Items.Select(item => $"{CollectionId}||{item.Id}||{item.Name}||{item.Description}||{item.Price}||{item.Status}||{item.Rating}"));

            await File.WriteAllLinesAsync(filePath, newLines);
        }

        public async Task LoadItems()
        {
            Items.Clear();
            if (File.Exists(filePath))
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                foreach (var line in lines)
                {
                    var parts = line.Split("||");
                    if (parts.Length == 7 && parts[0] == CollectionId)
                    {
                        Items.Add(new ItemModels
                        {
                            CollectionId = parts[0],
                            Id = parts[1],
                            Name = parts[2],
                            Description = parts[3],
                            Price = double.Parse(parts[4]),
                            Status = parts[5],
                            Rating = int.Parse(parts[6])
                        });
                    }
                }
            }
        }
    }
}
