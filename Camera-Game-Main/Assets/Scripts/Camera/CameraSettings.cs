using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraSettings : MonoBehaviour
{
    private Volume volumeComponent;

    private DepthOfField dof;

    private Dictionary<string, Coroutine> currentInterpolations = new Dictionary<string, Coroutine>();

    void Awake(){
        volumeComponent = GetComponent<Volume>();
        volumeComponent.profile.TryGet<DepthOfField>(out dof);
    }

    public void SetDepthOfField(float focusDistance, float focusLength, float lerpTime){
        if(currentInterpolations.ContainsKey("focusDistance")) {
            StopCoroutine(currentInterpolations["focusDistance"]);
            currentInterpolations.Remove("focusDistance");
        }
        if(currentInterpolations.ContainsKey("focusLength")) {
            StopCoroutine(currentInterpolations["focusLength"]);
            currentInterpolations.Remove("focusLength");
        }

        currentInterpolations.Add("focusDistance", StartCoroutine(LerpVolumeParameter(dof.focusDistance, focusDistance, lerpTime, "focusDistance")));
        currentInterpolations.Add("focusLength", StartCoroutine(LerpVolumeParameter(dof.focalLength, focusLength, lerpTime, "focusLength")));
    }

    public void SetFieldOfView(Camera camera, float targetFOV, float lerpTime){
        if(currentInterpolations.ContainsKey("FOV")) {
            StopCoroutine(currentInterpolations["FOV"]);
            currentInterpolations.Remove("FOV");
        }

        currentInterpolations.Add("FOV", StartCoroutine(LerpCameraProperty(camera, "fieldOfView", targetFOV, lerpTime, "FOV")));
    }

    public void AddFieldOfView(Camera camera, float addedFov, float lerpTime){
        if(currentInterpolations.ContainsKey("FOV")) {
            StopCoroutine(currentInterpolations["FOV"]);
            currentInterpolations.Remove("FOV");
        }

        currentInterpolations.Add("FOV", StartCoroutine(LerpCameraProperty(camera, "fieldOfView", camera.fieldOfView + addedFov, lerpTime, "FOV")));
    }

    public IEnumerator LerpVolumeParameter<T>(VolumeParameter<T> parameter, T targetValue, float duration, string key){
        float elapsed = 0f;
        T startValue = parameter.value;

        while (elapsed < duration){
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            parameter.value = LerpVolumeParameterValue(startValue, targetValue, t);

            yield return null;
        }

        parameter.value = targetValue;

        if(currentInterpolations.ContainsKey(key)) {
            currentInterpolations.Remove(key);
        }
    }

    private T LerpVolumeParameterValue<T>(T start, T end, float t){
        if (typeof(T) == typeof(float)) return (T)(object)Mathf.Lerp((float)(object)start, (float)(object)end, t);

        if (typeof(T) == typeof(Color)) return (T)(object)Color.Lerp((Color)(object)start, (Color)(object)end, t);

        if (typeof(T) == typeof(Vector2)) return (T)(object)Vector2.Lerp((Vector2)(object)start, (Vector2)(object)end, t);

        if (typeof(T) == typeof(Vector3)) return (T)(object)Vector3.Lerp((Vector3)(object)start, (Vector3)(object)end, t);

        if (typeof(T) == typeof(Vector4)) return (T)(object)Vector4.Lerp((Vector4)(object)start, (Vector4)(object)end, t);

        return end; 
    }

    public IEnumerator LerpCameraProperty(Camera camera, string propertyName, float targetValue, float duration, string key){
        float elapsed = 0f;
        float startValue = GetCameraPropertyValue(camera, propertyName);
    
        while (elapsed < duration){
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float currentValue = Mathf.Lerp(startValue, targetValue, t);
            SetCameraPropertyValue(camera, propertyName, currentValue);

            yield return null;
        }

        SetCameraPropertyValue(camera, propertyName, targetValue);

        if(currentInterpolations.ContainsKey(key)){
            currentInterpolations.Remove(key);
        }
    }

    private float GetCameraPropertyValue(Camera camera, string propertyName){
        switch(propertyName){
            case "fieldOfView": return camera.fieldOfView;
            case "nearClipPlane": return camera.nearClipPlane;
            case "farClipPlane": return camera.farClipPlane;
            case "orthographicSize": return camera.orthographicSize;
            default: return 0f;
        }
    }

    private void SetCameraPropertyValue(Camera camera, string propertyName, float value){
        switch(propertyName){
            case "fieldOfView": camera.fieldOfView = value; break;
            case "nearClipPlane": camera.nearClipPlane = value; break;
            case "farClipPlane": camera.farClipPlane = value; break;
            case "orthographicSize": camera.orthographicSize = value; break;
        }
    }

}