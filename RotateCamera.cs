using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public Animator anim;

    public float sensitivity;
    public float yAxis;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        yAxis += sensitivity * Input.GetAxis("Mouse Y");

        anim.SetFloat("Look Angle", yAxis);

        if (yAxis >= 1)
        {
            yAxis = 1;
        }

        if (yAxis <= -1)
        {
            yAxis = -1;
        }
    }
}
