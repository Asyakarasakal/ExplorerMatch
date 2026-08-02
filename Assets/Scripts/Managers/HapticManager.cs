using UnityEngine;

public class HapticManager : MonoBehaviour
{
    public static HapticManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Vibrate()
    {
        // 1. Ayarlar menüsündeki titreþim açýk/kapalý durumunu kontrol et
        bool isVibrationOn = PlayerPrefs.GetInt("VibrationOn", 1) == 1;

        // 2. Eðer titreþim KAPALI ise fonksiyondan çýk (titreme)
        if (!isVibrationOn)
            return;

        Debug.Log("Haptic Vibrate Worked!");

#if UNITY_ANDROID && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
    }
}