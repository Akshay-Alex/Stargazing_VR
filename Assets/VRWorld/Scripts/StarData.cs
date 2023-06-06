using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StarData", menuName = "ScriptableObjects/StarDataScriptableObject", order = 1)]
[System.Serializable]
public class StarData : ScriptableObject
{
    [SerializeField]
    public List<Star> stars = new List<Star>();
    [System.Serializable]
    public class Star
    {
        public int starNumber;
        public float magnitude;
        public float declination;
        public float rightAscension;
        public float declinationInRadian;
        public float rightAscensionInRadian;
        public float altitude;
        public float azimuth;
    }


}