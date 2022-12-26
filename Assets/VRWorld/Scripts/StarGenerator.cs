using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject StarPrefab;
    public int maxParticles = 10000;
    public TextAsset starCSV;
    public TextAsset StarData;
    public string[] DataLines;
    public string[] lines;
    public bool starsGenerated;
    private Vector3 normalizedPosition;
    public float DistanceMultiplier;
    float r = 1000.0f,mag,ra,dec,x,y,z;
    void Awake()
    {
        GenerateStars();
    }
    void GenerateStars()
    {
        starsGenerated = true;
        DataLines = StarData.text.Split('\n');
        for (int i = 0; i < maxParticles; i++)
        {
            string[] components = DataLines[i].Split(',');
            mag = float.Parse(components[0]);
            ra = float.Parse(components[1]);
            dec = float.Parse(components[2]);
            SphericalToCartesian(ra, dec, r, ref x, ref y, ref z);
            normalizedPosition = new Vector3(x/10, y/10, z/10);
            var star = Instantiate(StarPrefab, normalizedPosition, Quaternion.identity);
            star.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white * (5.0f - mag));
            star.gameObject.name = "star" + i;
        }
    }
    void SphericalToCartesian(float ra, float dec, float r, ref float x, ref float y, ref float z)
    {   
        dec = (Mathf.PI / 2) - dec;
        var rr = r * Mathf.Sin(dec);
        z = rr * Mathf.Cos(ra);
        x = rr * Mathf.Sin(ra);
        y = r * Mathf.Cos(dec);
    }
}
