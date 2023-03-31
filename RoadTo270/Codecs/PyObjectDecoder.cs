using System.Collections.Generic;
using Python.Runtime;

namespace RoadTo270.Codecs;

public static class PyObjectDecoder
{
    public static T[] DecodeToArray<T>(PyList list)
    {
        T[] arr = new T[list.Length()];
        
        if (!PythonEngine.IsInitialized) PythonEngine.Initialize();

        using (Py.GIL())
        {
            for (int arrIndex = 0; arrIndex < arr.Length; arrIndex++)
            {
                var convertedValue = list[arrIndex].As<T>();

                arr[arrIndex] = convertedValue;
            }
        }
        
        if (!PythonEngine.IsInitialized) PythonEngine.Shutdown();

        return arr;
    }
    
    public static List<T> DecodeToList<T>(PyList list)
    {
        List<T> decodedList = new List<T>();
        
        if (!PythonEngine.IsInitialized) PythonEngine.Initialize();

        using (Py.GIL())
        {
            foreach (var listObject in list)
            {
                var convertedValue = listObject.As<T>();
                
                decodedList.Add(convertedValue);
            }
        }
        
        if (!PythonEngine.IsInitialized) PythonEngine.Shutdown();

        return decodedList;
    }
}