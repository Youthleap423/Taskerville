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
        DataManager.Instance.CreateFUser(email, password, username, callback);
    }

    public void OnSignIn(string username, string password, System.Action<bool, string> callback)
    {
        DataManager.Instance.GetEmailWithUserName(username, (isSuccess, errMsg, email) =>
        {
            if (isSuccess == false)
            {
                callback(isSuccess, "Can't find user with username. Please check again");
            }
            else
            {
                FAuth.Instance.SignIn(email, password, (isSuccess, errMsg, userId) =>
                {
                    if (isSuccess)
                    {
                        DataManager.Instance.GetFUser(userId, (isSuccess, errMsg) =>
                        {
                            if (isSuccess)
                            {
                                //NotificationManager.Instance.RescheduleAllTaskNotification();
                                //ResourceViewController.Instance.LoadData(fUser);
                                //BuildManager.Instance.LoadBuildings();
                                //BuildManager.Instance.HandleAIPlayer();
                                //AppManager.Instance.singedIn = true;
                            }
                            callback(isSuccess, errMsg);
                        });
                        
                    }
                    else
                    {
                        callback(isSuccess, errMsg);
                    }
                });
            }
            Debug.LogError(email);
            
        });
        /*
        

        DataManager.Instance.CheckUsername(username, (isSuccess, errMsg, fUser, lUser) =>
        {
            if (isSuccess)
            {
                
                
            }
            else
            {
                callback(false, errMsg);
            }
        });*/
    }

    public void OnGetFUser(string userId, System.Action<bool, string> callback)
    {        
        DataManager.Instance.GetFUser(userId, callback);
        //ResourceViewController.Instance.LoadData(userId);
        //BuildManager.Instance.LoadBuildings();
        //BuildManager.Instance.HandleAIPlayer();
        //AppManager.Instance.singedIn = true;
        //DataManager.Instance.SerializeUser(false, callback);
    }

    public void OnSignOut(System.Action<bool> callback)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.SignOut((isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                NotificationManager.Instance.CancelAllPendingLocalNotifications();
                NotificationManager.Instance.RemoveAllDeliveredNotifications();
                AppManager.Instance.singedIn = false;
                FAuth.Instance.SignOut();
                callback?.Invoke(true);
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
                callback?.Invoke(false);
            }
        });
        
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
        return GetCurrentFUserList().Find(item => item.id == id);
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

    public void UpdateSetting(Game_Mode game_Mode, System.Action<bool, string> callback)
    {
        DataManager.Instance.UpdateSetting(game_Mode, callback);
    }

    public Sprite GetCurrentAvatarSprite()
    {
        return DataManager.Instance.GetCurrentAvatarSprite();
    }

    public Sprite GetAvatarSprite(int nIndex)
    {
        return DataManager.Instance.GetAvatarSprite(nIndex);
    }

    public void GetCurrentUserList(System.Action<bool, List<LUser>> callback)
    {
        var list =  DataManager.Instance.GetCurrentMemberList().ToList();
        if (list.Count == 0)
        {
            UIManager.Instance.ShowLoadingBar(true);
            DataManager.Instance.GetCoalitionMembers((isSuccess, errMsg, list) =>
            {
                UIManager.Instance.ShowLoadingBar(false);
                if (isSuccess)
                {
                    callback?.Invoke(true, list);
                }
                else
                {
                    UIManager.Instance.ShowErrorDlg(errMsg);
                    callback?.Invoke(false, null);
                }
            });
        }
        else
        {
            callback?.Invoke(true, list);
        }
    }

    public void AddExport(EResources res, float amount)
    {
        GetCurrentUser().AddExport(res, amount);
        if (GetCurrentSetting().game_mode == (int)Game_Mode.Game_Only)
        {
            AITaskManager.Instance.CheckOnCompleteWithExports();
        }
    }
}
