using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Camera, Camera2;
    //sensitivity of the mouse
    private float rotateSpeed = 150.0f;

    private float zoomSpeed = 500.0f;
    private float zoomAmount = 0.0f;
    private TourManager tourManager;

    // Start is called before the first frame update
    void Start()
    {
        tourManager = GetComponent<TourManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (tourManager.isCameraMove)
        {
            if (Input.GetMouseButton(0))
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x
                + Input.GetAxis("Mouse Y") * Time.deltaTime * rotateSpeed, transform.localEulerAngles.y
                + Input.GetAxis("Mouse X") * Time.deltaTime * rotateSpeed, 0);
            }

            if (Input.GetAxis("Mouse ScrollWheel")!=0f)
            {
                zoomAmount = Mathf.Clamp(zoomAmount + Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime
                * zoomSpeed, -5.0f, 5.0f);
                /*if(zoomAmount < 0.0f){
                    zoomAmount=0.0f;
                }*/
                Camera.transform.GetComponent<Camera>().transform.localPosition = new Vector3(0, 0, zoomAmount);
            }

            if(Input.GetMouseButton(1)){
                ResetZoom();
            }
        }
    }
    
    public void ResetCamera(){
        transform.localEulerAngles = new Vector3(0, 0, 0);
        zoomAmount = 0.0f;
        ResetZoom();
    }
    public void ResetZoom(){
        zoomAmount = 0.0f;
        Camera.transform.GetComponent<Camera>()
        .transform.localPosition = new Vector3(0, 0, zoomAmount);
    }
}
