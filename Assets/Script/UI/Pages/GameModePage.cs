using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameModePage : Page
{
    [SerializeField] private GameObject detailPageObj;
    [SerializeField] private GameObject confirmDlgObj;

    [SerializeField] private TMP_Text task_text;
    [SerializeField] private TMP_Text taskGame_text;
    [SerializeField] private TMP_Text game_text;

    private Color normalColor = new Color(0f / 255f, 55f / 255f, 255f / 255f);
    private Color selectColor = new Color(161f, 63f / 255f, 255f / 253f);

    private Game_Mode mode = Game_Mode.Task_And_Game;
    // Start is called before the first frame update
    private void Start()
    {

    }

    private void OnEnable()
    {
        Initialize();
    }

    public override void Initialize()
    {
        base.Initialize();

        mode = AppManager.Instance.GetCurrentMode();
        UpdateUI(mode);
    }


    private void ChangeMode(Game_Mode game_mode)
    {
        var currentMode = AppManager.Instance.GetCurrentMode();
        if (currentMode == game_mode)
        {
            return;
        }

        mode = game_mode;
        confirmDlgObj.SetActive(true);
        
    }

    private Color GetColor(Game_Mode currentMode, Game_Mode selfMode)
    {
        return currentMode == selfMode ? selectColor : normalColor;
    }

    private void UpdateUI(Game_Mode mode)
    {
        task_text.color = GetColor(mode, Game_Mode.Task_Only);
        taskGame_text.color = GetColor(mode, Game_Mode.Task_And_Game);
        game_text.color = GetColor(mode, Game_Mode.Game_Only);
    }

    public void OnTaskOnlyMode()
    {
        ChangeMode(Game_Mode.Task_Only);
    }

    public void OnTaskAndGameMode()
    {
        ChangeMode(Game_Mode.Task_And_Game);
    }

    public void OnGameOnlyMode()
    {
        ChangeMode(Game_Mode.Game_Only);
    }

    public void ShowInfo()
    {
        detailPageObj.SetActive(true);
    }

    public void OnProceed()
    {
        confirmDlgObj.SetActive(false);
        UpdateUI(mode);
        AppManager.Instance.ChangeMode(mode, true);        
    }

    public void OnNevermind()
    {
        mode = AppManager.Instance.GetCurrentMode();
        confirmDlgObj.SetActive(false);
    }

}
