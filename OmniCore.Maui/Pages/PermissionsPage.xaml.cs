using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmniCore.Maui.Models;

namespace OmniCore.Maui.Pages;

public partial class PermissionsPage : ContentPage
{
    public PermissionsPage(PermissionsModel model)
    {
        InitializeComponent();
        this.WithModel(model);
    }
}