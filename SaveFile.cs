using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using SFB;
using TMPro;
using System;
public class SaveFile : MonoBehaviour
{
    private string fileContent = "Hello, this is a sample text. Just pretend I am the content of the file you want to save.";

#if UNITY_WEBGL && !UNITY_EDITOR
    //WebGL
    [DLLImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

    public void OnClickSave()
    {
        byte[] bytes =Encoding.UTF8.GetBytes(fileContent);
        DownloadFile(gameObject.name,"OnFileDownload","model.obj",bytes,bytes.Lenght);
    }

    //Called from browser
    public void OnFieldDownload(){}

#else
    // Standalone platforms & editor
    public void OnClickSave()
    {
        string path = StandaloneFileBrowser.SaveFilePanel("Save File", "", "model", "obj");
        if(!string.IsNullOrEmpty(path) )
        {
            File.WriteAllText(path, fileContent);
        }
    }

#endif

}
