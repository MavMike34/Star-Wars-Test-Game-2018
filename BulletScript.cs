﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    public float bulletDamage;

    public bool lightSide;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(this.gameObject, 5f);
    }

    public void OnTriggerEnter(Collider other)
    {
        Destroy(this.gameObject, 0.5f);
    }
}
