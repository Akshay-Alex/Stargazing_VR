using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using DG.Tweening;

public class StarGenerator : MonoBehaviour
{
    //prefab used to instantiate stars
    public GameObject StarPrefab;
    public GameObject StarParent;
    //flags
    private bool starsGenerated, webRequestDataReceived;
    public bool ShowTwoStars;
    //class used to parse json data
    [System.Serializable]
    public class JulianDateData
    {
        public string jd;
    }
    private JulianDateData julianDateData;
    public class StarInfo
    {
        public double RA, Declination; //data is stored in radians
    }
    private StarInfo siriusStarInfo = new StarInfo();

    public double azimuth, altitude, julianDate, referenceStarDecDegree, referenceStarRaDegree;

    //properties


    //variables used to parse excel sheet
    public TextAsset StarData;
    public TextAsset StarDataDegreesAndRadians;
    private string[] DataLines, ReferenceStarData;
    private string[] lines;

    //variables used to generate stars from excel sheet
    public int maxParticles = 10000;
    private Vector3 normalizedPosition,siriusAtHorizonPosition;
    public float DistanceMultiplier;
    double  degreeToRadianMultiplier = Math.PI/180d;
    double r = 100.0f, mag, ra, dec, x, y, z, raInRadian, decInRadian;//everything is in radian 
    // whenever we use RA it can be seen multiplied with a factor of 15.
    // this is because RA is in hours and to convert it to degrees 
    void Start()
    {
        InitializeFlags();
        InitializeProperties();
        FindJulianDate();
    }
   
    void Awake()
    {


        GenerateAndPositionStars();

    }
    public void ToggleStarVisibility()
    {
        Transform starParentTransform = StarParent.transform;
            for(int index = 2; index < maxParticles-1; index++)
            {
                starParentTransform.GetChild(index).gameObject.SetActive(!ShowTwoStars);
            }
        ShowTwoStars = !ShowTwoStars;
    }
  
    public void GenerateAndPositionStars()
    {
        GenerateStars();
        PositionSiriusAtHorizon();
        StartCoroutine(CalculateSiriusRealtimePosition());
    }
    #region Functions to calculate realtime position
    IEnumerator CalculateSiriusRealtimePosition()
    {
        if (webRequestDataReceived == false)
        {
            yield return new WaitForSeconds(1);         //waits till julian date webrequest is done
            StartCoroutine(CalculateSiriusRealtimePosition());
        }
        else
        {
            if (julianDate != 0d)
            {
                //Debug.Log("Sirius star RA is " + siriusStarInfo.RA + " and DEC is " + siriusStarInfo.Declination);
                Debug.Log("RA " + siriusStarInfo.RA + " DEC " + siriusStarInfo.Declination + " lat " + 10.850516 + " lon " + 76.271080 + " jd " + 2459962.832704942);
                (altitude,azimuth) = RaDectoAltAz(siriusStarInfo.RA, siriusStarInfo.Declination, 10.850516*degreeToRadianMultiplier, 76.271080 * degreeToRadianMultiplier, 2459962.832704942);
                PositionSiriusAtRealtimePosition(altitude, azimuth);
            }
            else
                Debug.Log("jd was 0");
        }

    }
    (double,double) RaDectoAltAz(double ra, double dec, double lat, double lon, double jd)
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
        a /= degreeToRadianMultiplier;
        az /= degreeToRadianMultiplier;
        return (a, az);
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

    #endregion


    void GenerateStars()
    {
        if (starsGenerated == false)
        {
            starsGenerated = true;
            DataLines = StarDataDegreesAndRadians.text.Split('\n');
            for (int starNumber = 1; starNumber < maxParticles; starNumber++)
            {
                string[] components = DataLines[starNumber].Split(',');
                //parsing data
                mag = float.Parse(components[0]);
                dec = float.Parse(components[1]);
                ra = float.Parse(components[2]);
                decInRadian = dec * degreeToRadianMultiplier;
                raInRadian = ra * 15.0d * degreeToRadianMultiplier;


                SphericalToCartesian(raInRadian, decInRadian, r, ref x, ref y, ref z);
                normalizedPosition = new Vector3((float)x, (float)y, (float)z);
                InstantiateStar(starNumber, mag, normalizedPosition);

                if(starNumber == 1)         //storing data of star sirius
                {
                    siriusStarInfo.RA = raInRadian;
                    siriusStarInfo.Declination = decInRadian;
                }
            }
      
        }
    }
    void InstantiateStar(int starID,double magnitude,Vector3 position)
    {
        var star = Instantiate(StarPrefab, position, Quaternion.identity);
        star.transform.SetParent(StarParent.transform);
        star.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white * (5.0f - (float)(mag)));
        star.gameObject.name = "star" + starID;
    }
    void SphericalToCartesian(double ra, double dec, double r, ref double x, ref double y, ref double z)
    {   
        dec = (Mathf.PI / 2) - dec;
        var rr = r * Math.Sin(dec);
        z = rr * Math.Cos(ra);
        x = rr * Math.Sin(ra);
        y = r * Math.Cos(dec);
    }
    #region Julian Date calculation
    void FindJulianDate()
    {
        DateTime TimeInUTC = DateTime.Now.ToUniversalTime();
        //DateTime TimeInUTC = new DateTime(2023, 01, 19, 12, 00, 00); //today noon
        string request = String.Format("https://ssd-api.jpl.nasa.gov/jd_cal.api?cd={0}-{1}-{2}%20{3}", TimeInUTC.Year, TimeInUTC.Month, TimeInUTC.Day, TimeInUTC.ToString("HH:mm"));
        StartCoroutine(SendRequestForJulianDate(request));
    }
    IEnumerator SendRequestForJulianDate(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            webRequestDataReceived = true;
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
                    yield return null;
                    break;
            }
        }
    }
    #endregion

    #region Initialization functions
    void InitializeFlags()
    {
        webRequestDataReceived = false;
        starsGenerated = false;
    }
    void InitializeProperties()
    {
        altitude = 0d;
        azimuth = 0d;
        julianDate = 0d;
    }
    void PositionSiriusAtHorizon()
    {
        ReferenceStarData = DataLines[1].Split(',');
        referenceStarDecDegree = double.Parse(ReferenceStarData[1]);
        referenceStarRaDegree = double.Parse(ReferenceStarData[2]) * 15d;  //ra is expressed in hours, need to convert to degree.
        StarParent.transform.eulerAngles = new Vector3(0f, -((float)referenceStarRaDegree), -((float)referenceStarDecDegree));
        siriusAtHorizonPosition = StarParent.transform.eulerAngles;
    }
    void PositionSiriusAtRealtimePosition(double localAltitude, double localAzimuth)
    {
        Vector3 offsetVector = new Vector3(0f, (float)localAzimuth, (float)localAltitude);
        Vector3 newPositionOfSirius = siriusAtHorizonPosition + offsetVector;
        //StarParent.transform.eulerAngles += offsetVector;
        StarParent.transform.DORotate(newPositionOfSirius, 1f);
    }
    #endregion
}
