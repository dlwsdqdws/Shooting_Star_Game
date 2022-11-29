using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgorundScroll : MonoBehaviour
{

    [SerializeField] Vector2 scrolVelocity;

    Material material;

    void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        material.mainTextureOffset += scrolVelocity * Time.deltaTime;
    }
    
}
