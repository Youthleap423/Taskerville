using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvitationViewController : SingletonComponent<InvitationViewController>
{
    public void SendInvitation(string id, EInviteType type, System.Action<bool, string> callback)
    {
        LUser me = UserViewController.Instance.GetCurrentUser();
        //if (me.created_coalition.Equals(""))
        //{
        //    callback(false, "You've not created any coalition");
        //    return;
        //}
        CheckUserExist(id, (isSuccess, errMsg, user) =>
        {
            if (isSuccess)
            {
                FInvitation invitation = new FInvitation(me, user, type);
                DataManager.Instance.CreateInvitation(invitation, callback);
            }
            else
            {
                callback(isSuccess, errMsg);
            }
        });
    }

    public void SendInvitationToVillage(string vName, EInviteType type, System.Action<bool, string> callback)
    {
        LUser me = UserViewController.Instance.GetCurrentUser();

        CheckVillageExist(vName, (isSuccess, errMsg, user) =>
        {
            if (isSuccess)
            {
                FInvitation invitation = new FInvitation(me, user, type);
                DataManager.Instance.CreateInvitation(invitation, callback);
            }
            else
            {
                callback(isSuccess, errMsg);
            }
        });
    }

    public void SendTradeInvitation(LUser receiver, ETradeInviteType type, EResources res, ETradeRepeat repeat, System.Action<bool, string> callback)
    {
        LUser me = UserViewController.Instance.GetCurrentUser();
        FTradeInvitation invitation = new FTradeInvitation(me, receiver, res, repeat, type);
        DataManager.Instance.CreateInvitation(invitation, callback);
    }

    public void LoadInvitation(System.Action<bool, string, List<FInvitation>> callback)
    {
        LUser me = UserViewController.Instance.GetCurrentUser();
        DataManager.Instance.GetInvitations(me.id, callback);
    }

    public void LoadInvitation(string type, System.Action<bool, string, List<FInvitation>> callback)
    {
        LUser me = UserViewController.Instance.GetCurrentUser();
        DataManager.Instance.GetInvitations(me.id, type, callback);
    }

    public void LoadTradeInvitation(System.Action<bool, string, List<FTradeInvitation>> callback)
    {
        LUser me = UserViewController.Instance.GetCurrentUser();
        DataManager.Instance.GetTradeInvitations(me.id, callback);
    }


    private void CheckUserExist(string Pid, System.Action<bool, string, LUser> callback)
    {
        UserViewController.Instance.LoadCurrentUserList((isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                List<LUser> userList = UserViewController.Instance.GetCurrentUserList();
                foreach(LUser user in userList)
                {
                    if (user.id.Equals(Pid))
                    {
                        callback(true, "", user);
                        return;
                    }
                }
            }
            callback(false, "There is no user of id : " + Pid, null);
        });
    }

    private void CheckVillageExist(string vName, System.Action<bool, string, LUser> callback)
    {
        UserViewController.Instance.LoadCurrentUserList((isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                List<LUser> userList = UserViewController.Instance.GetCurrentUserList();
                foreach (LUser user in userList)
                {
                    if (user.Village_Name.Equals(vName))
                    {
                        callback(true, "", user);
                        return;
                    }
                }
            }
            callback(false, "There is no village of name : " + vName, null);
        });
    }

    public void AcceptInvitation(FInvitation invitation, System.Action<bool, string> callback)
    {
        FUser me = UserViewController.Instance.GetCurrentFUser();
        FUser other = UserViewController.Instance.GetFUser(invitation.Pid);
        
        if (other == null)
        {
            callback(false, "Can't find inviter.");
            return;
        }

        LUser lme = me.GetLUser();
        LUser lother = other.GetLUser();
        LUser currentUser = UserViewController.Instance.GetCurrentUser();

        invitation.state = EState.Agreed.ToString();
        invitation.reply_at = Convert.DateTimeToFDate(System.DateTime.Now);
        string str = invitation.Pid;
        invitation.Pid = invitation.receiver_Id;
        invitation.receiver_Id = str;
        if (invitation.type == EInviteType.Invite_Coalition.ToString())
        {
            Debug.LogError(invitation.Pid);
            var fCoaltion = CommunicationViewController.Instance.GetCoalitionWithId(lother.id);
            lme.joined_coalition = fCoaltion.name;
            me.SetLUser(lme);
            currentUser.UpdateJoinCoalition(fCoaltion.name);
            fCoaltion.AddMember(lme.id, (isSuccess, errMsg) =>
            {
                if (isSuccess)
                {
                    DataManager.Instance.UpdateInvitation(invitation, me, callback);
                }
                else
                {
                    callback(isSuccess, errMsg);
                }
            });
        }
        else
        {
            var fCoaltion = CommunicationViewController.Instance.GetCoalitionWithId(lme.id);
            lother.joined_coalition = fCoaltion.name;
            other.SetLUser(lother);
            fCoaltion.AddMember(lother.id, (isSuccess, errMsg) =>
            {
                if (isSuccess)
                {
                    DataManager.Instance.UpdateInvitation(invitation, other, callback);
                }
                else
                {
                    callback(isSuccess, errMsg);
                }
            });
            
        }
    }

    public void AcceptInvitation(FTradeInvitation invitation, System.Action<bool, string> callback)
    {
        LUser me = UserViewController.Instance.GetCurrentUser();

        UserViewController.Instance.LoadCurrentUserList((isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                List<LUser> userList = UserViewController.Instance.GetCurrentUserList();
                LUser other = null;
                foreach (LUser user in userList)
                {
                    if (user.id.Equals(invitation.Pid))
                    {
                        other = user;
                        break;
                    }
                }

                if (other == null)
                {
                    callback(false, "Can't find inviter.");
                    return;
                }

                FTrade trade = new FTrade(invitation);
                DataManager.Instance.CreateTrade(trade, (isSuccess, errMsg) =>
                {
                    if (isSuccess)
                    {
                        DataManager.Instance.RemoveData(invitation, callback);
                    }
                    else
                    {
                        callback(isSuccess, errMsg);
                    }
                });
            }
            else
            {
                callback(false, "There is no user of id : " + invitation.Pid);
            }
            
        });
        
    }

    public void DeclineInvitation(FInvitation invitation, System.Action<bool, string> callback)
    {
        FUser me = UserViewController.Instance.GetCurrentFUser();
        invitation.reply_at = Convert.DateTimeToFDate(System.DateTime.Now);
        invitation.state = EState.Declined.ToString();
        string str = invitation.Pid;
        invitation.Pid = invitation.receiver_Id;
        invitation.receiver_Id = str;
        DataManager.Instance.UpdateInvitation(invitation, me, callback);
    }

    public void RemoveInvitation(FInvitation invitation, System.Action<bool, string> callback = null)
    {
        DataManager.Instance.RemoveData(invitation, (isSuccess, errMsg) =>
        {

        });
    }
    public void DeclineInvitation(FTradeInvitation invitation, System.Action<bool, string> callback)
    {
        DataManager.Instance.RemoveData(invitation, callback);
    }
    /// <summary>
    /// Notification 
    /// </summary>
    public void GetMessage(FInvitation invitation, System.Action<bool, string> callback)
    {
        UserViewController.Instance.LoadCurrentUserList((isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                string result = "";
                LUser user = null;
                foreach (LUser fUser in UserViewController.Instance.GetCurrentUserList())
                {
                    if (fUser.id == invitation.Pid)
                    {
                        user = fUser;
                        break;
                    }
                }

                if (user == null)
                {
                    callback(false, "Can't find invitation creator.");
                    return;
                }

                if (invitation.state == EState.Created.ToString())
                {
                    if (invitation.type == EInviteType.Invite_Coalition.ToString())
                    {
                        result = string.Format("-  {0} of {1} has invited you to their coalition", user.GetFullName(), user.Village_Name);
                    }
                    else
                    {
                        result = string.Format("-  {0} of {1} has asked to join your coalition", user.GetFullName(), user.Village_Name);
                    }
                }
                else if (invitation.state == EState.Agreed.ToString())
                {
                    if (invitation.type == EInviteType.Invite_Coalition.ToString())
                    { 
                        result = string.Format("-  {0} of {1} has agreed to join your coalition", user.GetFullName(), user.Village_Name);
                    }
                    else
                    {
                        result = string.Format("-  {0} of {1} has accepted your admission to their coalition", user.GetFullName(), user.Village_Name);
                    }
                }
                else
                {
                    if (invitation.type == EInviteType.Invite_Coalition.ToString())
                    {
                        result = string.Format("-  {0} of {1} has declined to join your coalition", user.GetFullName(), user.Village_Name);
                    }
                    else
                    {
                        result = string.Format("-  {0} of {1} has declined your admission to their coalition", user.GetFullName(), user.Village_Name);
                    }

                        
                }

                callback(true, result);
            }
            else
            {
                callback(false, errMsg);
            }
        });
        
    }


}
