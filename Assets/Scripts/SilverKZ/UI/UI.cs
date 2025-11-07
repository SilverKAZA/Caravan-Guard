using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private Text _textWagonHP;
    [SerializeField] private Text _textProgress;
    [SerializeField] private Text _textWaveEnemies;

    private void OnEnable()
    {
        WagonTracker.onUpdateProgress += UpdateProgress;
        Wagon.onUpdateWagonHP += UpdateWagonHP;
    }

    private void OnDisable()
    {
        WagonTracker.onUpdateProgress -= UpdateProgress;
        Wagon.onUpdateWagonHP -= UpdateWagonHP;
    }

    private void UpdateWagonHP(int amount)
    {
        _textWagonHP.text = "Wagon HP: " + amount.ToString();
    }

    private void UpdateProgress(int amount)
    {
        _textProgress.text = "Progress: " + amount.ToString();
    }

    private void UpdateWaveEnemies(int amount)
    {
        _textWaveEnemies.text = "Enemies: " + amount.ToString();
    }
}
