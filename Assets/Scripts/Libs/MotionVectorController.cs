            using UnityEngine.XR;
using UnityEngine;
[RequireComponent(typeof(Camera))]

public class MotionVectorController : MonoBehaviour {

    public Material MVMat;
    public Material DMVMat;
    CustomRenderTexture delayedMotionVecs;

    bool renderCurrentMV = true;

    void Start () {
        GetComponent<Camera>().depthTextureMode=DepthTextureMode.MotionVectors;
        //generate the motion vector texture @ '_CameraMotionVectorsTexture'
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if(!delayedMotionVecs)
        {
            delayedMotionVecs = new CustomRenderTexture(src.width, src.height);
            delayedMotionVecs.doubleBuffered = true;
            delayedMotionVecs.format = RenderTextureFormat.ARGBHalf;
        }

        if(renderCurrentMV)
        {
            DMVMat.SetFloat("_Lerp", 1f);
            renderCurrentMV = false;
        } else {
            DMVMat.SetFloat("_Lerp", 0.1f);
        }
        Graphics.Blit(delayedMotionVecs, delayedMotionVecs, DMVMat);
        MVMat.SetTexture("_MotionVecs", delayedMotionVecs);
        Graphics.Blit(src, dest, MVMat);
        // if(!pastFrame){
        //     pastFrame = new RenderTexture (Screen.width, Screen.height, 16);
        //     Graphics.Blit(src, dest, MVMat);
        //     Graphics.Blit(src, pastFrame);
        //     Shader.SetGlobalTexture("_MotionVecs", src);
        // } else {
        //     Graphics.Blit(pastFrame,dest,MVMat);
        // }
        // // lastSrc = src;
    }
}