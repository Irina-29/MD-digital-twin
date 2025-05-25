using System;
using System.Collections.Generic;

[Serializable]
public class WristPressureEntry
{
    public string label;
    public float flexionExtension;
    public float radialUlnar;
    public float pressureTyping;
    public float pressureStatic;
}

[Serializable]
public class WristPressureDataset
{
    public List<WristPressureEntry> entries;
}
