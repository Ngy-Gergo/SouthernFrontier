using UnityEngine;

public class TogglePanel_Script : MonoBehaviour
{
    public GameObject Panel;
    
    public void Open_Close_Panel()
    {
        bool isActive = Panel.activeSelf;
        Panel.SetActive(!isActive);
    } 
}
