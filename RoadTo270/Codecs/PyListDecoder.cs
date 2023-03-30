using System.Collections.Generic;
using Python.Runtime;

namespace RoadTo270.Codecs;

public static class PyListDecoder
{
    public static List<int> Decode(PyList list)
    {
        List<int> decodedList = new List<int>();
        
        PythonEngine.Initialize();

        using (Py.GIL())
        {
            foreach (var listInteger in list)
            {
                var convertedInt = listInteger.As<int>();
                
                decodedList.Add(convertedInt);
            }
        }

        return decodedList;
    }
}