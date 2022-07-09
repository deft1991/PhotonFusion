using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class StartAsteroidGameTrigger : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        SceneManager.LoadScene("AsteroidGame/Scenes/AsteroidsSimple-Menu");
    }
}