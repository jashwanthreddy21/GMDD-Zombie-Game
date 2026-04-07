using UnityEngine;

public class DeathHandler : MonoBehaviour
{
    [SerializeField] Canvas gameOverCanvas;

    private void Start()
    {
        gameOverCanvas.enabled = false;
    }

    public void HandleDeath()
    {
        gameOverCanvas.enabled = true;
        Time.timeScale = 0;
        WeaponSwitcher switcher = FindObjectOfType<WeaponSwitcher>();
        if (switcher != null) switcher.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
