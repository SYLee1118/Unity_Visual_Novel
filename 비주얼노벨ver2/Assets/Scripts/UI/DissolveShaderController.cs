using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DissolveShaderController : MonoBehaviour
{
    public float dissolveProgress = 1;

    Material material;

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Image>().material;
    }

    // Update is called once per frame
    void Update()
    {
        material.SetFloat("_Progress", dissolveProgress);
    }
}
