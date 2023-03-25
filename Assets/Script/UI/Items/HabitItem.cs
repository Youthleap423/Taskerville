using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HabitItem : EntryItem
{
    private LHabitEntry task = null;

    [Space]
    [SerializeField] private Image toggleImage;
    [SerializeField] private Text unitType_TF;
    [SerializeField] private Dropdown durationH_Dropdown;
    [SerializeField] private Dropdown durationM_Dropdown;
    [SerializeField] private Dropdown timespanH_Dropdown;
    [SerializeField] private Dropdown timespanM_Dropdown;
    [SerializeField] private Dropdown timespan1H_Dropdown;
    [SerializeField] private Dropdown timespan1M_Dropdown;
    [SerializeField] private Slider timespan_Slider;
    [SerializeField] private List<Toggle> unitToggles;
    [SerializeField] private Image timespan_Slider_Fill;
    [SerializeField] private Text span_TF;
    [Space]
    [SerializeField] private GameObject unitObj;
    [SerializeField] private GameObject unitTextObj;
    [SerializeField] private GameObject durationObj;
    [SerializeField] private GameObject spanObj;
    [SerializeField] private GameObject span1Obj;
    [SerializeField] private GameObject sliderObj;

    private int selectedUnit = 0;
    protected override void Start()
    {
        base.Start();
        sublist_Toggle.onValueChanged.AddListener(delegate
        {
            ShowSubTaskList(sublist_Toggle);
        });

        complete_Toggle.onValueChanged.AddListener(delegate
        {
            OnComplete(complete_Toggle);
        });

        
        //timespan_Slider_Fill.color = Color.green;

        durationH_Dropdown.onValueChanged.AddListener(delegate {
            ChangeDurationHour(durationH_Dropdown);
        });

        durationM_Dropdown.onValueChanged.AddListener(delegate {
            ChangeDurationMinute(durationM_Dropdown);
        });

        timespanH_Dropdown.onValueChanged.AddListener(delegate {
            ChangeTimeSpanHour(timespanH_Dropdown);
        });

        timespanM_Dropdown.onValueChanged.AddListener(delegate {
            ChangeTimeSpanMinute(timespanM_Dropdown);
        });

        timespan1H_Dropdown.onValueChanged.AddListener(delegate {
            ChangeTimeSpanHour(timespan1H_Dropdown);
        });

        timespan1M_Dropdown.onValueChanged.AddListener(delegate {
            ChangeTimeSpanMinute(timespan1M_Dropdown);
        });

    }

    protected override void OnEnable()
    {
        //StartCoroutine(FixedUpdatePerMin());
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

    private void FixedUpdate()
    {
        if (this.task == null)
        {
            return;
        }

        if (this.task.timespan == false)
        {
            return;
        }

        if (this.task.isCompleted())
        {
            this.timespan_Slider.value = this.task.progress;
            complete_Toggle.isOn = true;
            UpdateCanvasGroup(false);
            return;
        }

        if (this.task.span_startTime == "" && this.task.unit == true)
        {
            return;
        }

        var timeSpanMins = (int.Parse(this.task.span_h) * 60 + int.Parse(this.task.span_m)) * 60;
        var pastMins = (int)Convert.TimeDifferenceSeconds(this.task.span_startDate + "_" + this.task.span_startTime, System.DateTime.Now);
        var progress = (float)(pastMins) / (float)(timeSpanMins);
        progress = Mathf.Clamp01(progress);
        this.timespan_Slider.value = progress;
        if (progress >= 1.0f)
        {
            timespan_Slider_Fill.color = Color.blue;
            
            if (this.task.unit == true)
            {
                this.task.span_startTime = "";
                unitToggles[this.task.complete_unit].transform.GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = 1.0f;
            }
            this.task.OnCompleteTimeSpan();
        }
        else
        {
            unitToggles[this.task.complete_unit].transform.GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = 0.5f;
            timespan_Slider_Fill.color = Color.green;
        }
    }
    
    #region Public Members
    public override void ShowSubTaskList(bool bShow)
    {
        if (!this.task.isPositive)
        {
            return;
        }

        subTaskListGroup.gameObject.SetActive(bShow);
    }

    public void SetTask(LHabitEntry task)
    {
        this.task = task;

        taskNameIF.text = task.taskName;

        complete_Toggle.isOn = this.task.isCompleted();
        float ratio = UnityEngine.Screen.height / (float)UnityEngine.Screen.width;
        taskNameIF.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(1330.0f / ratio, 0f);
        
        if (this.task.isPositive)
        {
            toggleImage.color = Color.green;
        }
        else
        {
            toggleImage.color = Color.red;
        }

        durationObj.SetActive(this.task.duration);
        unitObj.SetActive(this.task.unit);
        if (this.task.unit == true)
        {
            spanObj.SetActive(this.task.timespan);
            span1Obj.SetActive(false);
        }
        else
        {            
            span1Obj.SetActive(this.task.timespan);
            spanObj.SetActive(false);
            if (this.task.timespan == true)
            {
                span_TF.text = string.Format("({0}-{1})", this.task.span_startTime, Convert.GetTimeString(this.task.span_startTime, int.Parse(this.task.span_h), int.Parse(this.task.span_m)));
            }
        }
        

        sliderObj.SetActive(this.task.timespan);

        unitType_TF.text = this.task.unitType;
        durationH_Dropdown.value = GetIndex(durationH_Dropdown.options, this.task.dur_h);
        durationM_Dropdown.value = GetIndex(durationM_Dropdown.options, this.task.dur_m);
        timespanH_Dropdown.value = GetIndex(timespanH_Dropdown.options, this.task.span_h);
        timespanM_Dropdown.value = GetIndex(timespanM_Dropdown.options, this.task.span_m);
        timespan1H_Dropdown.value = GetIndex(timespanH_Dropdown.options, this.task.span_h);
        timespan1M_Dropdown.value = GetIndex(timespanM_Dropdown.options, this.task.span_m);
        timespan_Slider.value = this.task.progress;

        
        if (this.task.progress >= 1.0f)
        {
            timespan_Slider_Fill.color = Color.blue;
        }
        else
        {
            timespan_Slider_Fill.color = Color.green;
        }
        for(int i = 0; i < 8; i++)
        {
            if (i <= this.task.numberOfUnit)
            {
                unitToggles[i].gameObject.SetActive(true);
            }
            else
            {
                unitToggles[i].gameObject.SetActive(false);
            }

            unitToggles[i].transform.GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = i < this.task.complete_unit ? 1.0f : 0.0f;
        }

        if (this.task.timespan)
        {
            unitTextObj.SetActive(false);
        }
        else
        {
            unitTextObj.SetActive(this.task.unit);
        }


        if (UserViewController.Instance.GetCurrentSetting().shelter_storm)
        {
            UpdateCanvasGroup(false);
        }
        else
        {
            UpdateCanvasGroup(this.task.isEnabled(System.DateTime.Now));
            var isAvailable = this.task.IsAvailable(System.DateTime.Now);
            toggleCanvasGroup.blocksRaycasts = isAvailable;
            toggleCanvasGroup.interactable = isAvailable;
        }

        ShowSubTaskList(false);
    }

    public LHabitEntry GetTask()
    {
        return this.task;
    }

    public void EditTask()
    {
        if (UserViewController.Instance.GetCurrentSetting().current_mode != (int)Game_Mode.Game_Only)
        {
            transform.GetComponentInParent<NavigationPage>().Show<HabitEntryPage>("habit_entry", this.task);
        }
    }

    public void SetUnitClicked(int index)
    {
        if (index == this.task.complete_unit)
        {
            if (this.task.timespan == true)
            {
                if (this.task.span_startTime == "")
                {
                    this.task.span_startTime = System.DateTime.Now.ToString("hh:mm tt");
                    NotificationManager.Instance.ScheduleUnitLocalNotification(this.task);
                    //span_TF.text = string.Format("({0}-{1})", this.task.span_startTime, Convert.GetTimeString(this.task.span_startTime, int.Parse(this.task.span_h), int.Parse(this.task.span_m)));
                }
            }
            else
            {
                //unitToggles[index].isOn = true;
                unitToggles[this.task.complete_unit].transform.GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = 1.0f;
                this.task.complete_unit++;
                if (this.task.complete_unit > this.task.numberOfUnit) //ideally complete_unit = numberOfUnit + 1
                {
                    complete_Toggle.isOn = true;
                    OnComplete();
                }
            }
            
        }
    }



    #endregion
    #region Private Members


    private void ShowSubTaskList(Toggle toggle)
    {
        ShowSubTaskList(toggle.isOn);
    }

    private void OnComplete(Toggle toggle)
    {
        if (toggle.isOn == true && task.isEnabled() == true)
        {
            OnComplete();
        }

        if (toggle.isOn == false)
        {
            //TaskViewController.Instance.CancelComplete(task);
            //UpdateCanvasGroup(true);
        }
    }

    private void OnComplete()
    {
        TaskViewController.Instance.OnComplete(task);
        UpdateCanvasGroup(false);
    }


    private int GetIndex(List<Dropdown.OptionData> optionDatas, string content)
    {
        var result = 0;

        if (optionDatas.Count > 0)
        {
            for (int i = 0; i < optionDatas.Count; i++)
            {
                if (optionDatas[i].text == content)
                {
                    result = i;
                    break;
                }
            }
        }

        return result;
    }

    private void ChangeDurationHour(Dropdown dropdown)
    {
        this.task.dur_h = dropdown.options[dropdown.value].text;
        this.task.Update();
    }

    private void ChangeDurationMinute(Dropdown dropdown)
    {
        this.task.dur_m = dropdown.options[dropdown.value].text;
        this.task.Update();
    }

    private void ChangeTimeSpanHour(Dropdown dropdown)
    {
        this.task.span_h = dropdown.options[dropdown.value].text;
        span_TF.text = string.Format("({0}-{1})", this.task.span_startTime, Convert.GetTimeString(this.task.span_startTime, int.Parse(this.task.span_h), int.Parse(this.task.span_m)));

        if (this.task.unit && this.task.timespan && this.task.progress > 0f && this.task.progress < 1f)
        {
            NotificationManager.Instance.ReScheduleUnitLocalNotification(this.task);
        }
        this.task.Update();
    }

    private void ChangeTimeSpanMinute(Dropdown dropdown)
    {
        this.task.span_m = dropdown.options[dropdown.value].text;
        span_TF.text = string.Format("({0}-{1})", this.task.span_startTime, Convert.GetTimeString(this.task.span_startTime, int.Parse(this.task.span_h), int.Parse(this.task.span_m)));

        if (this.task.unit && this.task.timespan && this.timespan_Slider.value > 0f && this.timespan_Slider.value < 1f)
        {
            NotificationManager.Instance.ReScheduleUnitLocalNotification(this.task);
        }
        this.task.Update();
    }

    #endregion
    
}
