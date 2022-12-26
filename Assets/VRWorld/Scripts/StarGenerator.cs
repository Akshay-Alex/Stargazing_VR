using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StarGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private bool ShowOnlySirius;
    public GameObject StarPrefab;
    public GameObject StarParent;
    public double referenceStarDecDegree,referenceStarRaDegree; 
    public int maxParticles = 10000;
    //public TextAsset starCSV;
    public TextAsset StarData;
    public TextAsset StarDataDegreesAndRadians;
    public string[] DataLines, ReferenceStarData;
    public string[] lines;
    public bool starsGenerated;
    private Vector3 normalizedPosition;
    public float DistanceMultiplier;
    double  RadianToDegreeMultiplier = 180/ Math.PI;
    double r = 100.0f, mag, ra, dec, x, y, z, raInRadian, decInRadian;        //everything is in radian
    void Awake()
    {
        if(ShowOnlySirius)
        {
            maxParticles = 2;
        }
        GenerateStars();
        ReferenceStarData = DataLines[1].Split(',');
        referenceStarDecDegree = double.Parse(ReferenceStarData[3]) * RadianToDegreeMultiplier;
        referenceStarRaDegree = double.Parse(ReferenceStarData[4]) * RadianToDegreeMultiplier;
        StarParent.transform.eulerAngles = new Vector3(0f, -((float)referenceStarRaDegree), -((float)referenceStarDecDegree));
    }

    /*
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
    */
    void GenerateStars()
    {
        starsGenerated = true;
        DataLines = StarDataDegreesAndRadians.text.Split('\n');
        for (int i = 1; i < maxParticles; i++)
        {
            string[] components = DataLines[i].Split(',');
            //parsing data
            mag = float.Parse(components[0]);
            dec = float.Parse(components[1]);
            ra = float.Parse(components[2]);
            decInRadian = float.Parse(components[3]);
            raInRadian = float.Parse(components[4]);
           
            SphericalToCartesian(raInRadian, decInRadian, r, ref x, ref y, ref z);
            //normalizedPosition = new Vector3(x / 10, y / 10, z / 10);
            normalizedPosition = new Vector3((float) x, (float)y, (float) z);
            var star = Instantiate(StarPrefab, normalizedPosition, Quaternion.identity);
            star.transform.SetParent(StarParent.transform);
            star.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white * (5.0f - (float) (mag)));
            star.gameObject.name = "star" + i;
        }
    }
    void SphericalToCartesian(double ra, double dec, double r, ref double x, ref double y, ref double z)
    {   
        dec = (Mathf.PI / 2) - dec;
        var rr = r * Math.Sin(dec);
        z = rr * Math.Cos(ra);
        x = rr * Math.Sin(ra);
        y = r * Math.Cos(dec);
    }
    void CalculateLocalCoordinates( float RAinRadian, float DECinRadian)
    {
        float RAinDegrees = RAinRadian * 180 / Mathf.PI;
        float DECinDegrees = DECinRadian * 180 / Mathf.PI;
    }
}
