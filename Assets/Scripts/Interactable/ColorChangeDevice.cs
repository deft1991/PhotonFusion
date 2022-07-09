using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangeDevice : MonoBehaviour
{
    public void Operate()
    {
        /*
         * RGB digits in range from 0 to 1
         * usually it is range from 0 to 255
         * In Unity it is from 0 to 1
         */
        Color random = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        
        /*
         * Set color in material
         */
        GetComponent<Renderer>().material.color = random;
    }
}