using CollectionSystem.ViewModels;
namespace CollectionSystem.Pages;

public partial class ItemsPage : ContentPage
{
    public ItemsPage()
    {
        InitializeComponent();

        BindingContext = ItemViewModel.Instance;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ItemViewModel viewModel)
        {
            await viewModel.LoadItems();
        }
    }
}