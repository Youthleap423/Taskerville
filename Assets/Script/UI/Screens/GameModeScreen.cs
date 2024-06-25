using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameModeScreen : IScreen
{
    [SerializeField] private TMP_Text task_text;
    [SerializeField] private TMP_Text taskGame_text;
    [SerializeField] private TMP_Text game_text;
    [SerializeField] private GameObject detailPageObj;

    private Color normalColor = new Color(0f / 255f, 55f / 255f, 255f / 255f);
    private Color selectColor = new Color(161f, 63f / 255f, 255f / 253f);
    #region Unity Members
    // Start is called before the first frame update
    void Start()
    {
        ChangeGameMode(Game_Mode.Task_And_Game);
    }
    #endregion

    #region Public Members
    public void OnTaskOnly()
    {
        ChangeGameMode(Game_Mode.Task_Only);
    }

    public void OnTaskAndGame()
    {
        ChangeGameMode(Game_Mode.Task_And_Game);
    }

    public void OnGameOnly()
    {
        ChangeGameMode(Game_Mode.Game_Only);
    }

    public void OnBegin()
    {
        UIManager.Instance.ShowLoadingBar(true);

        UserViewController.Instance.SerializeUser(true, (isSuccess, err) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(err);
            }
            else
            {
                AppManager.Instance.LoadTaskScene();
            }
        });
    }

    public void ShowInfo()
    {
        detailPageObj.SetActive(true);
    }
    #endregion

    #region Private Members
    private void ChangeGameMode(Game_Mode mode)
    {
        UpdateUI(mode);
        AppManager.Instance.ChangeMode(mode);
    }

    private void UpdateUI(Game_Mode mode) 
    {
        task_text.color = GetColor(mode, Game_Mode.Task_Only);
        taskGame_text.color = GetColor(mode, Game_Mode.Task_And_Game);
        game_text.color = GetColor(mode, Game_Mode.Game_Only);
    }

    private Color GetColor(Game_Mode currentMode, Game_Mode selfMode)
    {
        return currentMode == selfMode ? selectColor : normalColor;
    }
    #endregion
}
