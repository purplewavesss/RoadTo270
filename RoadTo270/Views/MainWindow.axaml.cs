using System.IO;
using Avalonia.Controls;
using Python.Runtime;

namespace RoadTo270.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        Runtime.PythonDLL = "C:/Users/gavin/AppData/Local/Programs/Python/Python311/python311.dll";
        PythonEngine.Initialize();

        using (Py.GIL())
        {
            dynamic json = Py.Import("json");
            PyDict test = json.loads(File.ReadAllText("../../../test.json"));
        }
        
        InitializeComponent();
    }
}