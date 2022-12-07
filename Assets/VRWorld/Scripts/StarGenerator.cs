using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject StarPrefab;
    public int maxParticles = 10000;
    public TextAsset starCSV;
    public string[] lines;
    public bool starsGenerated;
    private Vector3 normalizedPosition;
    public float farclipplane;
    void Awake()
    {
        starsGenerated = false;
    }
    void LateUpdate()
    {
        if (!starsGenerated)
        {
            starsGenerated = true;
            lines = starCSV.text.Split('\n');
            for (int i = 0; i < maxParticles; i++)
            {
                string[] components = lines[i].Split(',');
                normalizedPosition = new Vector3(float.Parse(components[1]),float.Parse(components[3]),float.Parse(components[2])) * farclipplane;
                Instantiate(StarPrefab, normalizedPosition, Quaternion.identity);
                //particleStars[i].startColor = (Color.white * (1.0f - ((float.Parse(components[0]) + 1.44f) / 8)));
        }
        }
    }
}
