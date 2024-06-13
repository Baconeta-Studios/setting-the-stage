using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StSBillboard : MonoBehaviour
{
    private Transform cameraTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraTransform)
        {
            Vector3 directionToCamera = cameraTransform.position - transform.position;
            //Flatten the vector
            directionToCamera.y = 0;
            
            Quaternion rotation = Quaternion.LookRotation(directionToCamera);
            transform.rotation = rotation;
        }
    }
}
