using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PhotoStudioPanel : MonoBehaviour
{

    public InputField nameInput;
    public Button shootPhotoButton;
    public TextureSaver textureSaver;
    
    void Start() {
        shootPhotoButton.onClick.AddListener(ShootPhoto);
    }
    
    private void ShootPhoto() {
        textureSaver.SaveTexture(nameInput.text);
    }
}
