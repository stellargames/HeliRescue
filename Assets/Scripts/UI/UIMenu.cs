using System.IO;
using Skytanet.SimpleDatabase;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    private string _path;
    [SerializeField] private TextAsset creditsFile;
    [SerializeField] private RectTransform creditsPanel;
    [SerializeField] private Button resetButton;

    private void Start()
    {
        _path = Path.Combine(Application.persistentDataPath, "game.save");
        resetButton.gameObject.SetActive(File.Exists(_path));
        creditsPanel.GetComponentInChildren<TextMeshProUGUI>().text = creditsFile.text;
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void Reset()
    {
        SaveFile.DeleteSaveFile("game");
        resetButton.gameObject.SetActive(File.Exists(_path));
    }

    public void Credits()
    {
        creditsPanel.gameObject.SetActive(!creditsPanel.gameObject.activeSelf);
    }
}
