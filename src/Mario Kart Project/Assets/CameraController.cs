using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform camera;
    public Transform targetPosition;
    public Transform cameraLookAt;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        camera.position -= (camera.position - targetPosition.position) / 2;
        camera.LookAt(cameraLookAt);
    }
}
