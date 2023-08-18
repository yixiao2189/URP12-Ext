
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ConsoleGUI;

public class GUIShow : MonoBehaviour
{

    public enum AAMode
    {
        None,  
        FXAA,
        SMAA,
    }

    public UniversalAdditionalCameraData mainCameraData;
    public UniversalAdditionalCameraData uiCameraData;

    UniversalRenderPipelineAsset rp;

    ConsoleStyle _consoleStyle = new ConsoleStyle();


    private void Awake()
    {
        rp = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
    }


 
    void OnGUI()
    {

        if (_consoleStyle != null)
            _consoleStyle.Update();
        GUILayout.Window(1, _consoleStyle.optTopLeftWindowRect, DrawWindow, "");
    }

    void DrawWindow(int id)
    {

        GUILayout.Label($"RenderScale:{rp.renderScale.ToString("f2")}");

        float newRenderScale = GUILayout.HorizontalSlider(rp.renderScale, 0.5f, 1.2f);
        if (newRenderScale != rp.renderScale)
        {
            rp.renderScale = newRenderScale;
        }

        GUILayout.Space(10);

 
        uiCameraData.ColorSpaceUsage = _consoleStyle.DrawEnum(uiCameraData.ColorSpaceUsage);

        uiCameraData.renderPostProcessing = uiCameraData.ColorSpaceUsage == ColorSpace.Uninitialized && mainCameraData.renderPostProcessing;

        var newPost = _consoleStyle.DrawToggle(mainCameraData.renderPostProcessing, mainCameraData.renderPostProcessing ?"Post" : "NoPost");
        if (newPost != mainCameraData.renderPostProcessing)
        {
            mainCameraData.renderPostProcessing = newPost;
        }

     
        mainCameraData.antialiasing =(AntialiasingMode) _consoleStyle.DrawEnum((AAMode)mainCameraData.antialiasing);
   


        bool fsr = rp.upscalingFilter == UpscalingFilterSelection.FSR;
        var newFsr = _consoleStyle.DrawToggle(fsr, fsr ? "FSR" : "NoFSR");
        if (newFsr != fsr)
        {
            rp.upscalingFilter = newFsr ? UpscalingFilterSelection.FSR : UpscalingFilterSelection.Auto;
        }

        GUILayout.Label($"FSR Supported : {FSRUtils.IsSupported()}");
    }

}
