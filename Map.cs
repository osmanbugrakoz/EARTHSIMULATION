using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using CesiumForUnity;

public class Map : MonoBehaviour
{
    public TMP_InputField inputField;
    public CesiumGeoreference cesiumGeoreference;
    public CesiumCameraController cesiumCameraController;

    private string apiKey = ""; // USE YOUR KEY
    private string urlLocation = "";
    private string urlElevation = "";
    private double lat;
    private double lon;
    private double elevation;
    private string LatLon;
    private string address = "George+St+Sydney+Town+Hall+NSW+2000";


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputField.onSubmit.AddListener(OnInputSubmitted);
        cesiumGeoreference.originAuthority = CesiumGeoreferenceOriginAuthority.LongitudeLatitudeHeight;
        inputField.text = "George St Sydney Town Hall NSW 2000";
        StartCoroutine(GetGoogleMapLocation());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) 
        
        {
              cesiumCameraController.enabled = true;
        }
        else
        {
            cesiumCameraController.enabled = false;
        }

    }

    //Return lat and long position from address using Google Maps Platform Geocoding API

    IEnumerator GetGoogleMapLocation()
    {
        urlLocation = "https://maps.googleapis.com/maps/api/geocode/json?address=" + address + "&key=" + apiKey;
        UnityWebRequest webRequest = UnityWebRequest.Get(urlLocation);
        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success )
        {
            Debug.Log ("WWW ERROR: " + webRequest.error);
        }
        else
        {
            string requestText = webRequest.downloadHandler.text;
            int locationIndex =requestText.IndexOf("\"location\"");
            if (locationIndex != -1 && requestText.IndexOf("\"lat\"") > -1 && requestText.IndexOf("\"lng\"") > -1)
            {
                int indexOfLat = requestText.IndexOf("\"lat\"");
                int startIndexLat = requestText.IndexOf(':', indexOfLat) + 1;
                int endIndexLat = requestText.IndexOf(',', startIndexLat);
                string latString = requestText.Substring(startIndexLat, endIndexLat - startIndexLat);

                if (double.TryParse(latString, out lat)) { }

                int indexOflng = requestText.IndexOf("\"lng\"");
                int startIndexLng = requestText.IndexOf(':', indexOflng) + 1;
                int endIndexLng = requestText.IndexOf('}', startIndexLng);
                string lngString = requestText.Substring(startIndexLng, endIndexLng - startIndexLng);

                if (double.TryParse(lngString, out lon)) { }

                LatLon = latString + "," + lngString;
                StartCoroutine(GetGoogleMapElevation());
            }
  
        }
    }

    IEnumerator GetGoogleMapElevation()
    {
        urlElevation = "https://maps.googleapis.com/maps/api/elevation/json?locations=" + LatLon + "&key=" + apiKey;
        UnityWebRequest webRequest =UnityWebRequest.Get(urlElevation);
        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("WWW ERROR: " + webRequest.error);
        }
        else
        {
            string requestText = webRequest.downloadHandler.text;
            if (requestText.IndexOf("\"elevation\"") > -1)
            {
                int elevationIndex = requestText.IndexOf("\"elevation\"");
                int colonIndex = requestText.IndexOf(":", elevationIndex);
                int commaIndex = requestText.IndexOf(",", colonIndex);
                string elevationSubstring = requestText.Substring(colonIndex + 1, commaIndex - colonIndex - 1);
                elevationSubstring = elevationSubstring.Trim();
                if (double.TryParse(elevationSubstring, out elevation)) { }

                cesiumGeoreference.SetOriginLongitudeLatitudeHeight(lon, lat, elevation + 400);
                cesiumCameraController.transform.position = Vector3.zero;
                cesiumCameraController.transform.rotation = Quaternion.Euler(90.0f,0,0);
            }

        }
    }

  /*  public void OnTextChange()
    {
        address = inputField.text;
        address = address.Replace(" ", "+");
        StartCoroutine(GetGoogleMapLocation());
    }
  */
    private void OnInputSubmitted(string inputText)
    {
        address =inputText.Replace(" ", "+");
        StartCoroutine (GetGoogleMapLocation());
    }

    public void OnButtonClick()
    {
        address =inputField.text;
        address=address.Replace(" ", "+");
        StartCoroutine(GetGoogleMapLocation());
    }
}
