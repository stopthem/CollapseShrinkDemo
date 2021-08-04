using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    public Material[] allMaterials;
    [SerializeField] private SkyBoxType skyBoxType = SkyBoxType.Blue;
    
    private void Start() {
        ChangeTheme(new ThemeType(){
            SkyBox = skyBoxType
        });
    }


    public void ChangeTheme(ThemeType theme) {
        Material skyBoxMaterial = Resources.Load<Material>("Materials/Sky_"+ theme.SkyBox.ToString());
        RenderSettings.skybox = skyBoxMaterial;
    }
}


[System.Serializable]
public class ThemeType {

    public SkyBoxType SkyBox;
}

public enum SkyBoxType {
    Aqua,
    AquaPurple,
    Blue,
    DarkBlue,
    Orange,
    Purple,
    Red,
}
