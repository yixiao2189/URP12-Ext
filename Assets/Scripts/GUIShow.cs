
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ConsoleGUI;

public class GUIShow : MonoBehaviour
{




    UniversalRenderPipelineAsset rp;
    float renderScale = 1f;
    bool isGamma = false;

    ConsoleStyle _consoleStyle = new ConsoleStyle();


    private void Awake()
    {
        rp = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
    }


 
    void OnGUI()
    {

        if (_consoleStyle != null)
            _consoleStyle.Update();
        GUILayout.Window(1, _consoleStyle.optTopCenterWindowRect, DrawWindow, "");
    }

    void DrawWindow(int id)
    {

        GUILayout.Label($"Scale:{rp.renderScale.ToString("f2")}");
        var tmpScale = GUILayout.HorizontalSlider( renderScale, 0.1f, 2f);
        if (tmpScale != renderScale)
        {
            renderScale = tmpScale;
            rp.renderScale = tmpScale;
        }


     

        var newGamma = _consoleStyle.DrawToggle(isGamma, isGamma ? "Linear" : "Gamma");
        if (newGamma != isGamma)
        {
            isGamma = newGamma;

            var array = FindObjectsOfType<UniversalAdditionalCameraData>();
            foreach (var cd in array)
            {
                cd.ColorSpaceUsage = isGamma ? ColorSpace.Gamma : ColorSpace.Linear;
            }
        }
    }

}
