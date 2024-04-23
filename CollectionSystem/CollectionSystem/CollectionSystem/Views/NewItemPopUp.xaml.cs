using CollectionSystem.ViewModels;
using CommunityToolkit.Maui.Views;

namespace CollectionSystem.Views;

public partial class NewItemPopUp : Popup
{
    public NewItemPopUp()
    {
        InitializeComponent();
        BindingContext = ItemViewModel.Instance;
    }
}