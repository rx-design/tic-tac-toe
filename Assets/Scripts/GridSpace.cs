using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridSpace : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI buttonText;

    private GameController _gameController;

    public void SetGameControllerReference(GameController controller)
    {
        _gameController = controller;
    }

    public void SetSpace()
    {
        button.interactable = false;
        buttonText.text = _gameController.GetPlayerSide();

        _gameController.EndTurn();
    }
}
