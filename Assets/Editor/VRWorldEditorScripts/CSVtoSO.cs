using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVtoSO : MonoBehaviour
{
    private static string CityDataCSVPath = "/Editor/City_Data/CityData.csv";
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };
    [MenuItem("Utilities/Generate CityData")]
    public static void GenerateCityData()
    {
        int index = 0;
        //cityData.cities = new City[12000];
        List<Dictionary<string, object>> data = Read("CityData");
        //string[] allLines = File.ReadAllLines(Application.dataPath + CityDataCSVPath);
        CityData cityData = ScriptableObject.CreateInstance<CityData>();
        for (var i = 0; i < data.Count; i++)
        {
            CityData.City currentCity = new CityData.City();
            currentCity.Name = data[i]["ASCII Name"].ToString() ;
            currentCity.AlternateName = data[i]["Alternate Names"].ToString();
            currentCity.Coordinates = data[i]["Coordinates"].ToString();
            cityData.Cities.Add(currentCity);
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
        foreach (CityData.City city in cityData.Cities)
        {
            Debug.Log(city.Name);
        }
        //UnityEditor.EditorUtility.SetDirty(cityData);
        AssetDatabase.CreateAsset(cityData, $"Assets/VRWorld/Data/Objects/CityDataSO.asset");
        AssetDatabase.SaveAssets();
    }
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

}
