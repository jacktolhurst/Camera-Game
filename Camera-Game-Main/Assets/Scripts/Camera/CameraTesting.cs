using UnityEngine;

public class CameraTesting : MonoBehaviour
{
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private GameObject followObject;
    
    private Camera cam;

    private CameraSettings cameraSettings;

    [Range(0,100)]
    [SerializeField] private float currentFov;
    [Range(1,300)]
    [SerializeField] private float focalLength;

    void Awake(){
        cam = cameraObject.GetComponent<Camera>();
        cameraSettings = cameraObject.GetComponent<CameraSettings>();
    }

    void Update(){
        cameraSettings.SetFieldOfView(cam, currentFov, 0.1f);
        if(followObject != null) cameraSettings.SetDepthOfField(Vector3.Distance(cameraObject.transform.position, followObject.transform.position), focalLength, 0.1f);
        else cameraSettings.SetDepthOfField(0, 0, 0.1f);
    }
}
