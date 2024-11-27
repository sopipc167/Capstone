using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class DataManager : MonoBehaviour
{
    public Dictionary<string, CelestialObject> celestialObjects = new Dictionary<string, CelestialObject>();
    ObjectManager obj;

    public void ParseData(string json)
    {
        JArray array = JArray.Parse(json);

        foreach (var item in array)
        {
            string name = item["name"].ToString();
            string type = item["type"].ToString();

            if (celestialObjects.ContainsKey(name))
            {
                UpdateCelestialObject(celestialObjects[name], item);
                if (type == "star")
                {
                    Star s = new Star();
                    s.name = name;
                    s.alt = item["alt"].ToObject<float>();
                    s.az = item["az"].ToObject<float>();
                    s.fluxV = item["fluxV"].ToObject<float>();
                    s.distance = item["distance"].ToObject<float>();
                    s.collider.enabled = false;
                    s.collider.radius = 3f;
                    GameObject starObject = new GameObject(name);
                    obj.AddStarToGameObject(starObject, s);
                }
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
