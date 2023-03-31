using System.Collections.Generic;
using System.Linq;

namespace RoadTo270.Models;

public class NamedObject
{
    public string Name { get; }
    
    public NamedObject(string name)
    {
        Name = name;
    }

    public static NamedObject GetObject(string name, IEnumerable<NamedObject> namedObjects)
    {
        return (from namedObject in namedObjects
            where namedObject.Name == name
            select namedObject).ToList()[0];
    }
}