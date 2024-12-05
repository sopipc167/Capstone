using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class DataManager : MonoBehaviour
{
    public bool isUpdated = false;
    public Dictionary<string, CelestialObject> celestialObjects = new Dictionary<string, CelestialObject>();

    public void ParseData(string json)
    {
        JArray array = JArray.Parse(json);
        if (array.Count == 13)
        {
            isUpdated = true;
        }
        else
        {
            isUpdated = false;
        }

        foreach (var item in array)
        {
            string name = item["name"].ToString();
            string type = item["type"].ToString();

            if (celestialObjects.ContainsKey(name))
            {
                UpdateCelestialObject(celestialObjects[name], item);
            }
            else
            {
                CelestialObject celestialObject;

                if (type == "constellation")
                {
                    celestialObject = item.ToObject<Constellation>();
                }
                else
                {
                    celestialObject = item.ToObject<Star>();
                }

                celestialObjects[name] = celestialObject;
            }
            
        }
    }

    private void UpdateCelestialObject(CelestialObject celestialObject, JToken newData)
    {
        celestialObject.alt = newData["alt"].ToObject<float>();
        celestialObject.az = newData["az"].ToObject<float>();
    }
}
