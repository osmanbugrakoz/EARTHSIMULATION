using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using SFB;
using UnityEngine.Networking;

using Dummiesman;
using CesiumForUnity;

public class OpenFile : MonoBehaviour
{
    public CesiumSubScene cesiumSubScene;
    public CesiumGeoreference cesiumGeoreference;
    public Material defaultMaterial;

    private Display display;

    private void Start()
    {
        display = gameObject.GetComponent<Display>(); 
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    //WebGL
    [DLLImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void OnClickOpen()
    {
        UploadFile(gameObject.name, "OnFileUpload", ".obj", false);
    }

    public void OnFileUpload(string url)
    {
        StarCoroutine(OutputRoutineOpen(url));
    }


#else
    //Standalone platforms & editor
    public void OnClickOpen()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "obj", false);
        if (paths.Length > 0)
        {
            StartCoroutine(OutputRoutineOpen(new System.Uri(paths[0]).AbsoluteUri));
        }
    }
#endif

    private IEnumerator OutputRoutineOpen(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("WWW ERROR: " + www.error);
        }
        else
        {
            //Debug.Log(www.downloadHandler.text);
            
            //LOAD OBJ MODEL
            MemoryStream textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text));
            GameObject model = new OBJLoader().Load(textStream);
            model.transform.localScale= new Vector3(1,1,1); //OBJLoader set X scale as -1 by default

            //Set model position to be above the search address/middle of the screen
            Bounds bound = GetBound(model);
            model.transform.localPosition = Vector3.zero - bound.center - new Vector3(0,(float)cesiumGeoreference.height,0) + new Vector3(0, (bound.size.y),0) + new Vector3(0,60,0);

            //Geo_Anchoring the model so it doesn't move when we search another address
            model.transform.parent = cesiumSubScene.transform;
            model.AddComponent<CesiumGlobeAnchor>();

            //Assign default material
            display.SetMaterial(model, defaultMaterial);

            //Add Collider and tag to make the model selectable by the mouse usşng the Select script
            SetCollider(model.transform);
            SetTag(model.transform, "Selectable");
        }
    }

    private void SetTag(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            GameObject childGO =child.gameObject;
            childGO.tag = tag;
            SetTag(child,tag);
        }
    }

    private void SetCollider (Transform parent)
    {
        foreach (Transform child in parent)
        {
            GameObject childGO = child.gameObject;
            childGO.AddComponent<BoxCollider>();
            SetCollider(child);
        }
    }

    private Bounds GetBound(GameObject gameObj)
    {
        Bounds bound = new Bounds(gameObj.transform.position, Vector3.zero);
        Component[] rList = gameObj.GetComponentsInChildren(typeof(Renderer));

        foreach (Renderer r in rList)
        {
            bound.Encapsulate(r.bounds);
        }
        return bound;
    }
}

