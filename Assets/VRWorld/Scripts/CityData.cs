using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CityData", menuName = "ScriptableObjects/CityDataScriptableObject", order = 1)]
[System.Serializable]
public class CityData : ScriptableObject
{
    [SerializeField]
    public List<City> Cities = new List<City>();
    [System.Serializable]
    public class City
    {
        public string Name;
        public string AlternateName;
        public string Coordinates;
    }


}