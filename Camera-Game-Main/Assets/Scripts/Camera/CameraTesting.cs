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
    [Range(0,1)]
    [SerializeField] private float filmGrainIntensity;
    [Range(0,1)]
    [SerializeField] private float filmGrainResponse;

    void Awake(){
        cam = cameraObject.GetComponent<Camera>();
        cameraSettings = cameraObject.GetComponent<CameraSettings>();
    }

    void Update(){
        cameraSettings.SetFieldOfView(cam, currentFov, 0.1f);

        Vector3 vpPos = cam.WorldToViewportPoint(followObject.transform.position);
        if(followObject != null && vpPos.x >= 0f && vpPos.x <= 1f && vpPos.y >= 0f && vpPos.y <= 1f && vpPos.z > 0f) cameraSettings.SetDepthOfField(Vector3.Distance(cameraObject.transform.position, followObject.transform.position), focalLength, 0.1f);
        else cameraSettings.SetDepthOfField(0, 0, 0.1f);

        cameraSettings.SetFilmGrainIntensity(filmGrainIntensity, 0.1f);
        cameraSettings.SetFilmGrainResponse(filmGrainResponse, 0.1f);
    }
}
