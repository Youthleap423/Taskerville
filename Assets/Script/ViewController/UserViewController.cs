using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIControllersAndData.Store;
using System.Linq;

public class UserViewController : SingletonComponent<UserViewController>
{
    private void Start()
    {
        //LoadCurrentUserList((isSuccess, errMsg) =>
        //{

        //});
    }
    private List<LUser> currentUserList = new List<LUser>();

    public void OnRegister(string username, string email, string password, System.Action<bool, string> callback)
    {
        DataManager.Instance.CheckUsername(username, (isSuccess, errMsg, user, lUser) =>
        {
            if (!isSuccess)
            {
                FAuth.Instance.Register(username, email, password, (isSuccess, errMsg, userId) =>
                {
                    if (isSuccess)
                    {
                        var newFUser = new FUser(userId);
                        DataManager.Instance.UpdateNewUser(newFUser);
                        DataManager.Instance.UpdateUser(userId, username, email);
                        DataManager.Instance.SerializeUser(true, callback);
                        //ResourceViewController.Instance.Initialize();
                        //BuildManager.Instance.LoadBuildings();

                        //AppManager.Instance.singedIn = true;
                    }
                    else
                    {
                        callback(isSuccess, errMsg);
                    }
                });
            }
            else
            {
                callback(false, "This user email already exists. Plese input new user & email.");
            }
        });
    }

    public void OnSignIn(string username, string password, System.Action<bool, string> callback)
    {
        
        DataManager.Instance.CheckUsername(username, (isSuccess, errMsg, fUser, lUser) =>
        {
            if (isSuccess)
            {
                FAuth.Instance.SignIn(lUser.Email, password, (isSuccess, errMsg, userId) =>
                {
                    if (isSuccess)
                    {
                        DataManager.Instance.UpdateUser(fUser, callback);
                        NotificationManager.Instance.RescheduleAllTaskNotification();
                        //ResourceViewController.Instance.LoadData(fUser);
                        //BuildManager.Instance.LoadBuildings();
                        //BuildManager.Instance.HandleAIPlayer();
                        //AppManager.Instance.singedIn = true;
                    }
                    else
                    {
                        callback(isSuccess, errMsg);
                    }
                });
            }
            else
            {
                callback(false, errMsg);
            }
        });
    }

    public void OnAuthLogin(string userId, System.Action<bool, string> callback)
    {
        DataManager.Instance.UpdateUser(userId, callback);
        //ResourceViewController.Instance.LoadData(userId);
        //BuildManager.Instance.LoadBuildings();
        //BuildManager.Instance.HandleAIPlayer();
        //AppManager.Instance.singedIn = true;
        //DataManager.Instance.SerializeUser(false, callback);
    }

    public void OnSignOut()
    {
        FAuth.Instance.SignOut();
        DataManager.Instance.SignOut();
        NotificationManager.Instance.CancelAllPendingLocalNotifications();
        NotificationManager.Instance.RemoveAllDeliveredNotifications();
        AppManager.Instance.singedIn = false;
    }

    public void OnResetPassword(string strEmail, System.Action<bool, string> callback)
    {
        FAuth.Instance.SendPasswordResetEmail(strEmail, callback);
    }

    /// <summary>
    /// Setting Part
    /// </summary>
    /// <returns></returns>
    ///
    public LUser GetCurrentUser()
    {
        return DataManager.Instance.GetCurrentUser();
    }

    public FUser GetCurrentFUser()
    {
        LUser me = GetCurrentUser();
        return GetFUser(me);
    }

    public FUser GetFUser(LUser user)
    {
        return GetFUser(user.id);
    }

    public FUser GetFUser(string id)
    {
        return GetCurrentFUserList().Find(item => item.Id == id);
    }

    public List<FUser> GetCurrentFUserList()
    {
        return DataManager.Instance.GetCurrentFUserList();
    }

    public void SerializeUser(bool bForce = false, System.Action<bool, string> callback = null)
    {
        DataManager.Instance.SerializeUser(bForce, callback);
    }

    public void UpdateUser(LUser user)
    {
        DataManager.Instance.UpdateUser(user);
    }

    public LSetting GetCurrentSetting()
    {
        return DataManager.Instance.GetCurrentSetting();
    }

    public void UpdateSetting(bool shelter_storm)
    {
        DataManager.Instance.UpdateSetting(shelter_storm);
    }

    public void UpdateSetting(Game_Mode game_Mode)
    {
        DataManager.Instance.UpdateSetting(game_Mode);
    }

    public Sprite GetCurrentAvatarSprite()
    {
        return DataManager.Instance.GetCurrentAvatarSprite();
    }

    public Sprite GetAvatarSprite(int nIndex)
    {
        return DataManager.Instance.GetAvatarSprite(nIndex);
    }

    public void LoadCurrentUserList(System.Action<bool, string> callback)
    {
        DataManager.Instance.LoadUserData((isSuccess, errMsg, userList) =>
        {
            this.currentUserList.Clear();
            foreach (LUser user in userList)
            {
                this.currentUserList.Add(user);
            }
            callback(isSuccess, errMsg);
        });
    }

    public List<LUser> GetCurrentUserList()
    {
        return this.currentUserList;
    }

    public void AddExport(EResources res, float amount)
    {
        GetCurrentUser().AddExport(res, amount);
        if (GetCurrentSetting().current_mode == (int)Game_Mode.Game_Only)
        {
            AITaskManager.Instance.CheckOnCompleteWithExports();
        }
    }
}
