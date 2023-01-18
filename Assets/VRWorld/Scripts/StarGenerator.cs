using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

public class StarGenerator : MonoBehaviour
{
    [System.Serializable]

    public class JulianDateData
    {
        public string jd;
    }
    public double azimuth, altitude;
    public JulianDateData julianDateData;
    public double julianDate;
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
    double r = 100.0f, mag, ra, dec, x, y, z, raInRadian, decInRadian;//everything is in radian
    void Start()
    {
        if (ShowOnlySirius)
        {
            maxParticles = 2;
        }
        altitude = 0d;
        azimuth = 0d;
        starsGenerated = false;
        julianDate = 0d;
        FindJulianDate();
    }
    void Awake()
    {
        GenerateStars();
        PositionSiriusAtHorizon();
        CalculateSiriusRealtimePosition();
        RaDectoAltAz(19.97944444, -20.63583333, 10.850516, 76.271080, 2459962.832704942);

    }
    void PositionSiriusAtHorizon()
    {
        ReferenceStarData = DataLines[1].Split(',');
        referenceStarDecDegree = double.Parse(ReferenceStarData[3]) * RadianToDegreeMultiplier;
        referenceStarRaDegree = double.Parse(ReferenceStarData[4]) * RadianToDegreeMultiplier;
        StarParent.transform.eulerAngles = new Vector3(0f, -((float)referenceStarRaDegree), -((float)referenceStarDecDegree));
    }
    void CalculateSiriusRealtimePosition()
    {
        if (julianDate != 0d)
        {

        }
        else
            Debug.Log("jd was 0");
    }
    double EarthRotationAngle(double jd)
    {
        double t = jd - 2451545.0;
        double f = jd % 1.0;
        double theta = 2 * Math.PI * (f + 0.7790572732640 + 0.00273781191135448 * t);
        theta %= 2 * Math.PI;
        if (theta < 0)
            theta += 2 * Math.PI;
        return theta;
    }
    double greenwichMeanSiderealTime(double jd)
    {
        double t = ((jd - 2451545.0)) / 36525.0;
        double gmst = EarthRotationAngle(jd) + (0.014506 + 4612.156534 * t + 1.3915817 * t * t - 0.00000044 * t * t * t - 0.000029956 * t * t * t * t - 0.0000000368 * t * t * t * t * t) / 60.0 / 60.0 * Math.PI / 180.0;
        gmst %= 2 * Math.PI;
        if (gmst < 0) 
            gmst += 2 * Math.PI;
        return gmst;
    }
    void RaDectoAltAz(double ra, double dec, double lat, double lon,double jd)
    {
        double gmst = greenwichMeanSiderealTime(jd);
        double localSiderealTime = (gmst + lon) % (2 * Math.PI);
        double H = (localSiderealTime - ra);
        if (H < 0)
            { H += 2 * Math.PI; }
        if (H > Math.PI)
            { H = H - 2 * Math.PI; }
        double az = (Math.Atan2(Math.Sin(H), Math.Cos(H) * Math.Sin(lat) - Math.Tan(dec) * Math.Cos(lat)));
        double a = (Math.Asin(Math.Sin(lat) * Math.Sin(dec) + Math.Cos(lat) * Math.Cos(dec) * Math.Cos(H)));
        az -= Math.PI;

        if (az < 0) 
            { az += 2 * Math.PI; }
        altitude = a;
        azimuth = az;
        Debug.Log("Altitude " + a + " Azimuth " + az);
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
        if (starsGenerated == false)
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
                normalizedPosition = new Vector3((float)x, (float)y, (float)z);
                var star = Instantiate(StarPrefab, normalizedPosition, Quaternion.identity);
                star.transform.SetParent(StarParent.transform);
                star.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white * (5.0f - (float)(mag)));
                star.gameObject.name = "star" + i;
            }
      
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
    #region Julian Date calculation
    void FindJulianDate()
    {
        DateTime TimeInUTC = DateTime.Now.ToUniversalTime();
        string request = String.Format("https://ssd-api.jpl.nasa.gov/jd_cal.api?cd={0}-{1}-{2}%20{3}", TimeInUTC.Year, TimeInUTC.Month, TimeInUTC.Day, TimeInUTC.ToString("HH:mm"));
        StartCoroutine(SendRequestForJulianDate(request));
    }
    IEnumerator SendRequestForJulianDate(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                   //Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    string jsonString = webRequest.downloadHandler.text;
                    julianDateData = JsonUtility.FromJson<JulianDateData>(jsonString);
                    julianDate = Convert.ToDouble(julianDateData.jd);
                    break;
            }
        }
    }
    #endregion
}
