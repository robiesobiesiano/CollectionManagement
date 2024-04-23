using CollectionSystem.ViewModels;
using CommunityToolkit.Maui.Views;

namespace CollectionSystem.Views;

public partial class EditItemPopUp : Popup
{
    public EditItemPopUp()
    {
        InitializeComponent();
        BindingContext = ItemViewModel.Instance;
    }
}