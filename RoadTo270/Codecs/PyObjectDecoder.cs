using System.Collections.Generic;
using System.Linq;
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
        var decodedList = new List<T>();
        
        if (!PythonEngine.IsInitialized) PythonEngine.Initialize();

        using (Py.GIL())
        {
            decodedList.AddRange(list.Select(listObject => listObject.As<T>()));
        }
        
        if (!PythonEngine.IsInitialized) PythonEngine.Shutdown();

        return decodedList;
    }

    public static Dictionary<TKey, TValue> DecodeToDictionary<TKey, TValue>(PyDict dict) where TKey : notnull where TValue : notnull
    {
        var decodedDict = new Dictionary<TKey, TValue>();
        var keys = new List<TKey>();
        var values = new List<TValue>();

        foreach (var key in dict.Keys()) keys.Add(key.As<TKey>());
        foreach (var value in dict.Values()) values.Add(value.As<TValue>());

        for (int dictIndex = 0; dictIndex < keys.Count; dictIndex++) decodedDict.Add(keys[dictIndex], values[dictIndex]);

        return decodedDict;
    }
}