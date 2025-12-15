using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
public class Display : MonoBehaviour
{
    public void SetMaterial (GameObject model, Material material)
    {
        foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>())
        {
            renderer.material = material;
        }
    }
}
