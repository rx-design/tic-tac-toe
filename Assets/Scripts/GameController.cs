using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable] public class Player
{
    public Image panel;
    public TextMeshProUGUI text;
    public Button button;
}

[Serializable] public class PlayerColor
{
    public Color panelColor;
    public Color textColor;
}

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI[] buttonList;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public GameObject restartButton;
    public GameObject startInfo;

    public Player playerX;
    public Player playerO;
    public PlayerColor activePlayerColor;
    public PlayerColor inactivePlayerColor;

    private int _moveCount;
    private PlayerSide _playerSide;

    private const int MaxMoveCount = 9;

    private readonly int[][] _lines =
    {
        new [] { 0, 1, 2 },
        new [] { 3, 4, 5 },
        new [] { 6, 7, 8 },

        new [] { 0, 3, 6 },
        new [] { 1, 4, 7 },
        new [] { 2, 5, 8 },

        new [] { 0, 4, 8 },
        new [] { 2, 4, 6 },
    };

    private enum PlayerSide
    {
        X = 0,
        O = 1,
    }

    private enum Result
    {
        Win = 0,
        Tie = 1,
    }

    public void SetStartingSide(string startingSide)
    {
        if (!Enum.TryParse(startingSide, out _playerSide))
        {
            return;
        }

        if (_playerSide == PlayerSide.X)
        {
            SetPlayerColors(playerX, playerO);
        }
        else
        {
            SetPlayerColors(playerO, playerX);
        }

        StartGame();
    }

    private void SetPlayerButtons(bool toggle)
    {
        playerX.button.interactable = toggle;
        playerO.button.interactable = toggle;
    }

    private void SetPlayerColorsInactive()
    {
        playerX.panel.color = inactivePlayerColor.panelColor;
        playerX.text.color = inactivePlayerColor.textColor;
        playerO.panel.color = inactivePlayerColor.panelColor;
        playerO.text.color = inactivePlayerColor.textColor;
    }

    public string GetPlayerSide()
    {
        return _playerSide.ToString();
    }

    public void EndTurn()
    {
        _moveCount++;

        var isWin = _lines.Any(line => line.All(IsOccupied));

        if (isWin)
        {
            EndGame(Result.Win);
        }
        else if (_moveCount >= MaxMoveCount)
        {
            EndGame(Result.Tie);
            SetPlayerColorsInactive();
        }
        else
        {
            ChangeSides();
        }
    }

    public void RestartGame()
    {
        _moveCount = 0;
        gameOverPanel.SetActive(false);
        restartButton.SetActive(false);
        startInfo.SetActive(true);
        SetBoardInteractable(false); // Bug fix
        SetPlayerButtons(true);
        SetPlayerColorsInactive();

        foreach (var button in buttonList)
        {
            button.text = "";
        }
    }

    private void Awake()
    {
        SetGameControllerReferenceOnButtons();
        _moveCount = 0;
    }

    private void SetGameControllerReferenceOnButtons()
    {
        foreach (var button in buttonList)
        {
            button
                .GetComponentInParent<GridSpace>()
                .SetGameControllerReference(this);
        }
    }

    private void SetBoardInteractable(bool toggle)
    {
        foreach (var button in buttonList)
        {
            button
                .GetComponentInParent<Button>()
                .interactable = toggle;
        }
    }

    private void StartGame()
    {
        SetBoardInteractable(true);
        SetPlayerButtons(false);
        startInfo.SetActive(false);
    }

    private void SetPlayerColors(Player p1, Player p2)
    {
        p1.panel.color = activePlayerColor.panelColor;
        p1.text.color = activePlayerColor.textColor;
        p2.panel.color = inactivePlayerColor.panelColor;
        p2.text.color = inactivePlayerColor.textColor;
    }

    private bool IsOccupied(int i)
    {
        return buttonList[i].text == GetPlayerSide();
    }

    private void ChangeSides()
    {
        if (_playerSide == PlayerSide.X)
        {
            _playerSide = PlayerSide.O;
            SetPlayerColors(playerO, playerX);
        }
        else
        {
            _playerSide = PlayerSide.X;
            SetPlayerColors(playerX, playerO);
        }
    }

    private string GetMessage(Result result)
    {
        return result switch
        {
            Result.Win => $"{_playerSide} Wins!",
            Result.Tie => "It's a draw!",
            _ => ""
        };
    }

    private void EndGame(Result result)
    {
        SetBoardInteractable(false);
        gameOverText.text = GetMessage(result);
        gameOverPanel.SetActive(true);
        restartButton.SetActive(true);
    }
}
