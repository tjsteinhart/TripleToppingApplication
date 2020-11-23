using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    PhaseManager phaseManager = new PhaseManager();

    private void Awake()
    {
        phaseManager = FindObjectOfType<PhaseManager>();    
    }
}
