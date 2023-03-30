using System.Collections.Generic;
using Python.Runtime;

namespace RoadTo270.Codecs;

public static class PyListDecoder
{
    public static List<T> Decode<T>(PyList list)
    {
        List<T> decodedList = new List<T>();
        
        PythonEngine.Initialize();

        using (Py.GIL())
        {
            foreach (var listObject in list)
            {
                var convertedValue = listObject.As<T>();
                
                decodedList.Add(convertedValue);
            }
        }

        return decodedList;
    }
}