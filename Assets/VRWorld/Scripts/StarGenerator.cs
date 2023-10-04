using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using DG.Tweening;
using System.Threading.Tasks;

public class StarGenerator : MonoBehaviour
{
    public List<GameObject> stars;
    public static StarGenerator starGenerator;
    public StarData starData;
    //public GPSService gPSService;
    private float latitude, longitude;
    //prefab used to instantiate stars
    public GameObject StarPrefab;
    public GameObject StarParent;
    public GameObject Compass;
    //flags
    private bool starsGenerated, JulianDateWebRequestDone;
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
        public double RA, Declination; //data is stored in degrees
    }
    private StarInfo siriusStarInfo = new StarInfo();
    public DateTime currentGameTime;

    public double azimuth, altitude, julianDate, referenceStarDecDegree, referenceStarRaDegree;
    public Timer timer;

    //properties


    //variables used to parse excel sheet
    //public TextAsset StarData;
    public TextAsset StarDataDegreesAndRadians;
    private string[] DataLines, ReferenceStarData;
    private string[] lines;

    //variables used to generate stars from excel sheet
    public int maxParticles = 10000;
    private Vector3 normalizedPosition,siriusAtHorizonPosition;
    public float DistanceMultiplier;
    float  degreeToRadianMultiplier = (float)Math.PI/180;
    double r = 100.0f, mag, ra, dec, x, y, z, raInRadian, decInRadian;
    // whenever we use RA it can be seen multiplied with a factor of 15.
    // this is because RA is in hours and to convert it to degrees 
    void Start()
    {
        DOTween.SetTweensCapacity(25000, 50);
        starGenerator = this;
        InitializeFlags();
        InitializeProperties();
        DateTime TimeInUTC = DateTime.Now.ToUniversalTime();
        currentGameTime = TimeInUTC;
        FindJulianDate(TimeInUTC);
    }
   
    public void SetLatitudeAndLongitude(CityButtonData cityButtonData)
    {
        latitude = float.Parse(cityButtonData.Latitude);
        longitude = float.Parse(cityButtonData.Longitude);
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
            StartCoroutine("GenerateStarsAtRealtimeLocation");
    }
    /*
    IEnumerator GetLocationData()
    {
        if(gPSService.locationWebRequestDone)
        {
            (latitude, longitude) = gPSService.GetLatitudeAndLongitude();
            locationDataFetched = true;
            Debug.Log("Location data received ");
        }
        else
        {
            //Debug.Log("locationWebRequest not done ");
            if(Input.location.status != LocationServiceStatus.Running)
            {
                Input.location.Start();
            }
            yield return new WaitForSeconds(1);         //waits till locationWebRequest is Done
            StartCoroutine(GetLocationData());
        }
    }
    */
    #region Functions to calculate realtime position
    /*
    IEnumerator CalculateSiriusRealtimePosition()
    {
        if (JulianDateWebRequestDone == false)
        {
            Debug.Log("Julian date received : " + JulianDateWebRequestDone + " location data received : " + locationDataFetched);
            yield return new WaitForSeconds(1);         //waits till julian date webrequest and location data web request is done
            StartCoroutine(CalculateSiriusRealtimePosition());
        }
        else
        {
            if (julianDate != 0d )
            {
                //Debug.Log("Sirius star RA is " + siriusStarInfo.RA + " and DEC is " + siriusStarInfo.Declination);
                //Debug.Log("RA " + siriusStarInfo.RA + " DEC " + siriusStarInfo.Declination + " lat " + latitude + " lon " + longitude + " jd " + julianDate);
                (altitude,azimuth) = RaDectoAltAz(siriusStarInfo.RA*15 *degreeToRadianMultiplier, siriusStarInfo.Declination * degreeToRadianMultiplier, latitude*degreeToRadianMultiplier, longitude * degreeToRadianMultiplier, julianDate);
                Debug.Log("altitude " + altitude + " azimuth" + azimuth + "RA " + siriusStarInfo.RA + " DEC " + siriusStarInfo.Declination + " lat " + latitude + " lon " + longitude + " jd " + julianDate);
                PositionSiriusAtRealtimePosition(altitude, azimuth);

            }
            else
                Debug.Log("jd was 0");
        }

    }
    */
    //!Do not touch this
    //Function working properly
    //reference website https://astrogreg.com/convert_ra_dec_to_alt_az.html
    (float,float) RaDectoAltAz(float ra, float dec, float lat, float lon, double jd)
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
        return ((float)a,(float)az);
    }
    /*
    public void StartTimerForUpdatingStarPosition()
    {
        timer.TimerFinished += UpdateStarPosition;
        FindJulianDate();
        timer.SetTimer(60f);
    }
*/
    public void UpdateStarPosition(int timeToChangeInHours)
    {
        currentGameTime = currentGameTime.AddHours(timeToChangeInHours).ToUniversalTime();
        FindJulianDate(currentGameTime);
        StartCoroutine("MoveStarsToRealtimePosition");
        
    }
    IEnumerator MoveStarsToRealtimePosition()
    {
            foreach (StarData.Star star in starData.stars)
            {
                (star.altitude, star.azimuth) = RaDectoAltAz(star.rightAscensionInRadian, star.declinationInRadian, latitude * degreeToRadianMultiplier, longitude * degreeToRadianMultiplier, julianDate);
                SphericalToCartesian(star.azimuth, star.altitude, DistanceMultiplier, ref x, ref y, ref z);
                normalizedPosition = new Vector3((float)x, (float)y, (float)z);
                MoveStar(star.starNumber, normalizedPosition);
            }
        
        starsGenerated = true;
        return null;
    }
    void MoveStar(int StarID, Vector3 position)
    {
        stars[StarID].transform.DOMove(position, 1f);
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

    /*
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


                SphericalToCartesian(raInRadian, decInRadian, DistanceMultiplier, ref x, ref y, ref z);
                normalizedPosition = new Vector3((float)x, (float)y, (float)z);
                InstantiateStar(starNumber, mag, normalizedPosition);

                if(starNumber == 1)         //storing data of star sirius
                {
                    siriusStarInfo.RA = ra;
                    siriusStarInfo.Declination = dec;
                }
            }
      
        }
    }*/
    /*
    void GenerateStars()
    {
        if (starsGenerated == false)
        {
            starsGenerated = true;
            //DataLines = StarDataDegreesAndRadians.text.Split('\n');
            foreach (StarData.Star star in starData.stars)
            {
                SphericalToCartesian((double)star.rightAscensionInRadian, (double)star.declinationInRadian, DistanceMultiplier, ref x, ref y, ref z);
                normalizedPosition = new Vector3((float)x, (float)y, (float)z);
                InstantiateStar(star.starNumber, star.magnitude, normalizedPosition);

                if (star.starNumber == 1)         //storing data of star sirius
                {
                    siriusStarInfo.RA = ra;
                    siriusStarInfo.Declination = dec;
                }
            }        
        }
    }
    */
    void InstantiateStar(int starID,double magnitude,Vector3 position)
    {
        var star = Instantiate(StarPrefab, position, Quaternion.identity);
        star.transform.SetParent(StarParent.transform);
        star.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white * (5.0f - (float)(magnitude)));
        star.gameObject.name = "star" + starID;
        stars.Insert(starID, star);
        //stars[starID] = star;
    }
   
    void SphericalToCartesian(float azimuth, float altitude, double r, ref double x, ref double y, ref double z)
    {
        float azimuthInRadian = azimuth * degreeToRadianMultiplier;
        float altitudeInRadian = altitude * degreeToRadianMultiplier;
        altitudeInRadian = (Mathf.PI / 2) - altitudeInRadian;           //Spherical to cartesian is calculated from y axis, but altitude is calculated from x or z, so we subtract from 90 to get the angle
        //dec = (Mathf.PI / 2) - dec;//x y z in cartesian is completely different from xyz in unity y
        x = r * Math.Sin(altitudeInRadian) * Math.Cos(azimuthInRadian); //x in cartesian = x in unity
        z = -( r * Math.Sin(altitudeInRadian) * Math.Sin(azimuthInRadian));//y in cartesian = -z in unity
        y = r * Math.Cos(altitudeInRadian);//z in cartesian = y in unity
    }

    /*
   void SphericalToCartesian(float ra, float dec, double r, ref double x, ref double y, ref double z)
   {   
       dec = (Mathf.PI / 2) - dec;
       var rr = r * Math.Sin(dec);
       z = rr * Math.Cos(ra);
       x = rr * Math.Sin(ra);
       y = r * Math.Cos(dec);
   }
   */
    IEnumerator GenerateStarsAtRealtimeLocation()
    {
        if (starsGenerated == false)
        {
            
            //DataLines = StarDataDegreesAndRadians.text.Split('\n');
            foreach (StarData.Star star in starData.stars)
            {
                (star.altitude,star.azimuth) = RaDectoAltAz(star.rightAscensionInRadian, star.declinationInRadian, latitude * degreeToRadianMultiplier, longitude * degreeToRadianMultiplier, julianDate);
                SphericalToCartesian(star.azimuth, star.altitude, DistanceMultiplier, ref x, ref y, ref z);
                normalizedPosition = new Vector3((float)x, (float)y, (float)z);
                InstantiateStar(star.starNumber, star.magnitude, normalizedPosition);
            }
        }
        starsGenerated = true;
        return null;
    }
    #region Julian Date calculation
    void FindJulianDate(DateTime currentDate)
    {
            julianDate = currentDate.ToOADate() + 2415018.5;
    }
    /*
    void FindJulianDate(DateTime currentTime)
    {
        currentGameTime = currentTime;
        //DateTime TimeInUTC = new DateTime(2023, 01, 19, 12, 00, 00); //today noon
        string request = String.Format("https://ssd-api.jpl.nasa.gov/jd_cal.api?cd={0}-{1}-{2}%20{3}", currentTime.Year, currentTime.Month, currentTime.Day, currentTime.ToString("HH:mm"));
        StartCoroutine(SendRequestForJulianDate(request));
    }
    */
    /*
    IEnumerator SendRequestForJulianDate(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            if(webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonString = webRequest.downloadHandler.text;
                julianDateData = JsonUtility.FromJson<JulianDateData>(jsonString);
                julianDate = Convert.ToDouble(julianDateData.jd);
                JulianDateWebRequestDone = true;
                Debug.Log("Julian Date found "+ julianDate);
            }
            else
            {
                Debug.Log("Web request error " + webRequest.result);
            }

            /*
            JulianDateWebRequestDone = true;
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
                    Debug.Log("Julian date is " + julianDate);
                    StartCoroutine(CalculateSiriusRealtimePosition());
                    yield return null;
                    break;
            }

        }
    }
    */
    #endregion

    #region Initialization functions
    void InitializeFlags()
    {
        JulianDateWebRequestDone = false;
        starsGenerated = false;
    }
    void InitializeProperties()
    {
        altitude = 0d;
        azimuth = 0d;
        julianDate = 0d;
        latitude = 0f;
        longitude = 0f;
    }
    /*
    void PositionSiriusAtHorizon()
    {
        ReferenceStarData = DataLines[1].Split(',');
        referenceStarDecDegree = double.Parse(ReferenceStarData[1]);
        referenceStarRaDegree = double.Parse(ReferenceStarData[2]) * 15d;  //ra is expressed in hours, need to convert to degree.
        StarParent.transform.DOLocalRotate(new Vector3(((float)referenceStarDecDegree), ((float)referenceStarRaDegree),0f ), .00000001f);
        //StarParent.transform.eulerAngles = new Vector3(0f, -((float)referenceStarRaDegree), -((float)referenceStarDecDegree));
        siriusAtHorizonPosition = StarParent.transform.eulerAngles;
        Debug.Log("Positioned sirius at horizon");
    }
    void PositionSiriusAtRealtimePosition(double localAltitude, double localAzimuth)
    {
        Vector3 offsetVector = new Vector3(-(float)localAltitude, (float)localAzimuth,0f);
        Vector3 newPositionOfSirius = siriusAtHorizonPosition + offsetVector;
        //StarParent.transform.eulerAngles += offsetVector;
        StarParent.transform.DOLocalRotate(newPositionOfSirius, 0.1f);
        Debug.Log("PositionSiriusAtRealtimePosition offset vector " + offsetVector + " newPositionOfSirius "+ newPositionOfSirius + " localAltitude "+ localAltitude + " localAzimuth "+ localAzimuth);
    }
    */
    #endregion
}
