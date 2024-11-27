using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string type;
    public List<Dictionary<string, object>> data;
}

[System.Serializable]
public class CelestialObject
{
    public string name;
    public string type;
    public float alt;
    public float az;
    public float distance;
}

[System.Serializable]
public class Star : CelestialObject
{
    public float fluxV;
    public SphereCollider collider;
}

[System.Serializable]
public class Constellation : CelestialObject
{
    public List<Star> stars;
    public List<List<int>> lines;
}