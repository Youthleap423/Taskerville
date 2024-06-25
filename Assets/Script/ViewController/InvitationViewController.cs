using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvitationViewController : SingletonComponent<InvitationViewController>
{
    public void SendInvitationToVillage(string vName, EInviteType type, System.Action<bool, string> callback)
    {
        DataManager.Instance.InviteVillagerToCoalition(vName, callback);
    }

    public void SendTradeInvitation(string receiver, ETradeInviteType type, EResources res1, float amount1, EResources res2, float amount2, ETradeRepeat repeat, System.Action<bool, string> callback)
    {
        DataManager.Instance.SendTradeInvite(receiver, res1, amount1, res2, amount2, repeat, type, (isSuccess, errMsg, _) =>
        {
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void LoadInvitation(System.Action<bool, string, List<FInvitation>> callback)
    {
        DataManager.Instance.GetInvitations(callback);
    }

    public void LoadTradeInvitation(System.Action<bool, string, List<FTradeInvitation>> callback)
    {
        DataManager.Instance.GetTradeInvitations(callback);
    }

    public void AcceptInvitation(FInvitation invitation, System.Action<bool, string> callback)
    {
        DataManager.Instance.AcceptCoalitionInvite(invitation.id);
    }

    public void AcceptInvitation(FTradeInvitation invitation, System.Action<bool, string> callback)
    {
        DataManager.Instance.AcceptTradeInvite(invitation.id, (isSuccess, errMsg, _) =>
        {
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void DeclineInvitation(FInvitation invitation, System.Action<bool, string> callback)
    {
        DataManager.Instance.RejectCoalitionInvite(invitation.id, callback);
    }

    public void DeclineInvitation(FTradeInvitation invitation, System.Action<bool, string> callback)
    {
        DataManager.Instance.RejectTradeInvite(invitation.id, (isSuccess, errMsg, _) =>
        {
            callback?.Invoke(isSuccess, errMsg);
        });
    }
    /// <summary>
    /// Notification 
    /// </summary>
    public void GetMessage(FInvitation invitation, System.Action<bool, string> callback)
    {
        UserViewController.Instance.GetCurrentUserList((isSuccess, list) =>
        {
            string result = "";
            LUser user = null;

            foreach (LUser fUser in list)
            {
                if (fUser.id == invitation.sender)
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
        });
    }


}
