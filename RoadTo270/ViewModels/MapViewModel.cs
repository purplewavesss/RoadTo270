using RoadTo270.Models.Interfaces;

namespace RoadTo270.ViewModels;

public class MapViewModel : ViewModelBase, IAccessApplication
{
    public App GameApp { get; }
}