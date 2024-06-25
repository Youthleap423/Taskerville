using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessengerPage : Page
{
    [Space]
    [SerializeField] private NavPage navPage;
    [Space]
    [SerializeField]private Dropdown userDropDown;
    [SerializeField]private InputField message_IF;
    [SerializeField] private Text time_TF;
    [SerializeField] private Text name_TF;
    [SerializeField] private Image avatar;

    [Space]
    [Header("MessageUI")]
    [SerializeField] private GameObject messageItemPrefab;
    [SerializeField] private GameObject myMessageItemPrefab;
    [SerializeField] private Transform messageList_Transform;

    private List<LUser> userList = new List<LUser>();
    private List<FMessage> messageList = new List<FMessage>();
    private LUser me = null; 
    // Start is called before the first frame update
    void Start()
    {
        userDropDown.onValueChanged.AddListener(delegate
        {
            SelectUser(userDropDown);
        });
        
    }

    private void FixedUpdate()
    {
        time_TF.text = Convert.DateTimeToMessageTime(System.DateTime.Now);
    }

    private void OnEnable()
    {
        Initialize();
    }

    #region Public Members
    public void Back()
    {
        navPage.Back();
    }

    public override void Initialize()
    {
        base.Initialize();

        me = UserViewController.Instance.GetCurrentUser();
        name_TF.text = me.GetFullName();
        avatar.sprite = DataManager.Instance.GetAvatarSprite(me.AvatarId);
        message_IF.text = "";
        time_TF.text = Convert.DateTimeToMessageTime(System.DateTime.Now);

        userDropDown.options.Clear();
        
        userDropDown.options.Add(new Dropdown.OptionData("Group"));

        UIManager.Instance.ShowLoadingBar(true);

        CommunicationViewController.Instance.GetCoalitionMembers((isSuccess, errMsg, userList) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                this.userList = userList;
                LoadUI();
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }

    public void LoadUI()
    {
        foreach (LUser user in userList)
        {
            userDropDown.options.Add(new Dropdown.OptionData(user.GetFullName()));
        }

        userDropDown.value = 0;
        SelectUser(userDropDown);
    }

    public void Post()
    {
        int index = userDropDown.value;

        if (index == 0)
        {
            SendFMessageToAll();
            return;
        }

        LUser receiver = userList[index - 1];
        SendFMessage(receiver);
    }

    public void Load()
    {
        SelectUser(userDropDown);
    }
    #endregion

    #region Private Members
    private void SelectUser(Dropdown dropDown)
    {
        int index = userDropDown.value;
        if (index == 0)
        {
            LoadPublicMessages();
            return;
        }

        string Id = userList[index - 1].id;
        LoadPrivateMessages(Id);

    }

    private void LoadPrivateMessages(string userId)
    {
        UIManager.Instance.ShowLoadingBar(true);
        CommunicationViewController.Instance.LoadPrivateMessages(userId, (isSuccess, errMsg, messageList) =>
        {
            UIManager.Instance.ShowLoadingBar(false);

            if (isSuccess)
            {
                this.messageList = messageList;
                UpdateMessages();
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }

    private void LoadPublicMessages()
    {
        UIManager.Instance.ShowLoadingBar(true);
        CommunicationViewController.Instance.LoadPublicMessages(me.joined_coalition, (isSuccess, errMsg, messageList) =>
        {
            UIManager.Instance.ShowLoadingBar(false);

            if (isSuccess)
            {
                this.messageList = messageList;
                UpdateMessages();
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }

    private void UpdateMessages()
    {
        foreach(Transform child in messageList_Transform)
        {
            Destroy(child.gameObject);
        }

        foreach(FMessage fMessage in messageList)
        {
            LUser sender = userList.Find(x => x.id == fMessage.sender);
            GameObject obj = null;
            if (fMessage.sender == me.id)
            {
                obj = Instantiate(myMessageItemPrefab, messageList_Transform);
                sender = me;
            }
            else
            {
                obj = Instantiate(messageItemPrefab, messageList_Transform);
            }
            MessageItem item = obj.GetComponent<MessageItem>();
            if (item != null)
            {
                item.SetData(sender, fMessage);
            }
        }
        message_IF.text = "";

        messageList_Transform.GetComponent<ContentFitterRefresh>().RefreshContentFitters();
    }

    private void SendFMessageToAll()
    {
        UIManager.Instance.ShowLoadingBar(true);

        CommunicationViewController.Instance.SendFMessageToCoalition(message_IF.text, (isSuccess, errMsg, fMessage) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                messageList.Add(fMessage);
                UpdateMessages();
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }

        });
    }

    private void SendFMessage(LUser receiver)
    {
        UIManager.Instance.ShowLoadingBar(true);

        CommunicationViewController.Instance.SendFMessage(receiver, message_IF.text, (isSuccess, errMsg, fMessage) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                messageList.Add(fMessage);
                UpdateMessages();
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }
    #endregion
}
