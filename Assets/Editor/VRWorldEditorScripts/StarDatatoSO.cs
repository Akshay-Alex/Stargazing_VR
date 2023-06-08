using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public class StarDatatoSO : MonoBehaviour
{
    static float degreeToRadianMultiplier = (float)Math.PI / 180f;
    //static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    //static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    //static char[] TRIM_CHARS = { '\"' };
    [MenuItem("Utilities/Generate StarData")]
    public static void GenerateStarData()
    {
        int index = 0;
        //cityData.cities = new City[12000];
        //string[] DataLines = StarDataDegreesAndRadians.text.Split('\n');
        string[] DataLines = File.ReadAllLines("Assets/Editor/VRWorldEditorScripts/StarDataText.csv");
        //string[] allLines = File.ReadAllLines(Application.dataPath + CityDataCSVPath);
        StarData starData = ScriptableObject.CreateInstance<StarData>();
        for (var i = 0; i < 10000; i++)
        {
            string[] components = DataLines[i+1].Split(',');
            StarData.Star currentStar = new StarData.Star();
            currentStar.starNumber = i;
            currentStar.magnitude = float.Parse(components[0]);
            currentStar.declination = float.Parse(components[1]);
            currentStar.rightAscension = float.Parse(components[2]);
            currentStar.declinationInRadian = currentStar.declination * degreeToRadianMultiplier;
            currentStar.rightAscensionInRadian = currentStar.rightAscension * 15.0f * degreeToRadianMultiplier;
            starData.stars.Add(currentStar);

            //currentCity.Name = data[i]["ASCII Name"].ToString();
            //currentCity.AlternateName = data[i]["Alternate Names"].ToString();
            //currentCity.Coordinates = data[i]["Coordinates"].ToString();
            //cityData.Cities.Add(currentCity);
        }
        //Debug.Log(allLines + " " + Application.dataPath + CityDataCSVPath);
        /*
        foreach (string s in allLines)
        {
            string[] splitData = s.Split(',');
            CityData.City currentCity = new CityData.City();
            currentCity.Name = splitData[0];
            currentCity.AlternateName = splitData[1];
            currentCity.Coordinates = splitData[2];
            //Debug.Log(currentCity.Name + currentCity.AlternateName + currentCity.Coordinates);
            cityData.Cities.Add(currentCity);

            
            cityData.cities[index].Name = splitData[0];
            cityData.cities[index].AlternateName = splitData[1];
            cityData.cities[index].Coordinates = splitData[2];
            //index++;  
        }
        */
        /*
        foreach (CityData.City city in cityData.Cities)
        {
            Debug.Log(city.Name);
        }
        */
        //UnityEditor.EditorUtility.SetDirty(cityData);
        AssetDatabase.CreateAsset(starData, $"Assets/VRWorld/Data/Objects/StarDataSO.asset");
        AssetDatabase.SaveAssets();
    }
    /*
    public static List<Dictionary<string, object>> Read(string file)
    {
        var list = new List<Dictionary<string, object>>();
        TextAsset data = Resources.Load(file) as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list.Add(entry);
        }
        return list;
    }
    */

}
