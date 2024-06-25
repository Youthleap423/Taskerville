using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommunicationViewController : SingletonComponent<CommunicationViewController>
{
    private List<FCoalition> coalitionList = new List<FCoalition>();

    public FCoalition GetCurrentCoalition()
    {
        var currentUser = UserViewController.Instance.GetCurrentUser();
        if (currentUser.joined_coalition == "")
        {
            return null;
        }
        else
        {
            return GetCoalitionWithName(currentUser.joined_coalition);
        }
    }

    public FCoalition GetCoalitionWithId(string id)
    {
        return GetCurrentCoalitionList().Find(item => item.id == id);
    }

    public FCoalition GetCoalitionWithName(string name)
    {
        return GetCurrentCoalitionList().Find(item => item.name == name);
    }
    
    public List<FCoalition> GetCurrentCoalitionList()
    {
        return DataManager.Instance.GetCurrentCoalitions().ToList();
    }

    public void CreateCoalition(string name, System.Action<bool, string> callback)
    {
        foreach (FCoalition fCoalition in GetCurrentCoalitionList())
        {
            if (fCoalition.name.ToLower().Equals(name.ToLower()))
            {
                callback(false, "There is a coalition with same name. Please input another name.");
                return;
            }
        }

        DataManager.Instance.CreateCoalition(name, Convert.GetSTZ(System.TimeZoneInfo.Local), callback);
    }

    public FCoalition FindCoalition(string id)
    {
        foreach(FCoalition coalition in this.coalitionList)
        {
            if (coalition.isMemberOf(id))
            {
                return coalition;
            }
        }
        return null;
    }

    public FCoalition FindMyCoalition()
    {
        LUser me = UserViewController.Instance.GetCurrentUser();
        return FindCoalition(me.id);
    }


    public void GetCoalitionMembers(System.Action<bool, string, List<LUser>> callback)
    {
        LUser fUser = UserViewController.Instance.GetCurrentUser();

        if (fUser.joined_coalition.Equals(""))
        {
            callback(false, "You've not joined any coalition.", null);
            return;
        }

        List<LUser> resultList = new List<LUser>();
        DataManager.Instance.GetCoalitionMembers((isSuccess, errMsg, list) =>
        {
            if (isSuccess)
            {
                callback(true, "", list);
            }
            else
            {
                callback(false, "Error to get coalition members: " + errMsg, null);
            }
        });
    }

    public void LoadCoalitions(System.Action<bool, string> callback)
    {
        DataManager.Instance.LoadCoalitionData(callback);
    }

    public void ChangePublic(bool isOpen, System.Action<bool, string> callback)
    {
        if (isOpen)
        {
            DataManager.Instance.OpenCoalition(callback);
        }
        else
        {
            DataManager.Instance.CloseCoalition(callback);
        }
    }

    public void JoinCoalition(string name, System.Action<bool, string> callback)
    {
        LUser user = UserViewController.Instance.GetCurrentUser();

        if (!user.joined_coalition.Equals(""))
        {
            callback(false, "You've already joined a coalition.");
            return;
        }

        DataManager.Instance.SendJoinInvite(name, callback);
    }

    public void JoinedCoalition(string id)
    {
        var coalition = GetCoalitionWithId(id);
        if (coalition != null)
        {
            LUser user = UserViewController.Instance.GetCurrentUser();
            user.UpdateJoinCoalition(coalition.name);
            coalition.AddMember(user.id,  (isSuccess, errMsg) =>
            {

            });
        }
    }

    public void JoinedCoalition(FCoalition coalition)
    {
        LUser user = UserViewController.Instance.GetCurrentUser();
        if (user.fJC == false)
        {
            DataManager.Instance.UpdateResource(EResources.Culture, 10f);
            user.fJC = true;
            user.Save();
        }
        user.UpdateJoinCoalition(coalition.name);
    }

    public void LeaveCoalition(System.Action<bool, string> callback)
    {
        DataManager.Instance.LeaveCoalition(callback);
    }

    public void InviteUserToCoalition(string invitee, System.Action<bool, string> callback)
    {
        DataManager.Instance.InviteUserToCoalition(invitee, callback);
    }

    public void InviteVillagerToCoalition(string villager, System.Action<bool, string> callback)
    {
        DataManager.Instance.InviteVillagerToCoalition(villager, callback);
    }

    public void KickCoalitionMember(LUser user, System.Action<bool, string> callback)
    {
        DataManager.Instance.InviteVillagerToCoalition(user.id, callback);
    }

    public string GetSimilarNames(string prefix)
    {
        string result = "";

        foreach(FCoalition coalition in GetCurrentCoalitionList())
        {
            if (coalition.name.ToLower().StartsWith(prefix.ToLower()))
            {
                if (result == "")
                {
                    result = coalition.name;
                }
                else
                {
                    result = result + ", " + coalition.name;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Messages Part
    /// </summary>
    public void SendFMessageToCoalition(string msg, System.Action<bool, string, FMessage> callback)
    {
        DataManager.Instance.SendCoalitionMessage(msg, callback);
    }

    public void SendFMessage(LUser receiver, string msg, System.Action<bool, string, FMessage> callback)
    {
        DataManager.Instance.SendPrivateMessage(receiver.id, msg, callback);
    }

    public void LoadPublicMessages(string coalitionName, System.Action<bool, string, List<FMessage>> callback)
    {
        DataManager.Instance.GetPublicMessages(coalitionName, callback);
    }

    public void LoadPrivateMessages(string otheruser, System.Action<bool, string, List<FMessage>> callback)
    {
        DataManager.Instance.GetPrivateMessages((isSuccess, errMsg, list) =>
        {
            if (isSuccess)
            {
                var newList = list.FindAll((item) => item.members.Contains(otheruser)).ToList();
                callback?.Invoke(isSuccess, errMsg, newList);
            }
            else
            {
                callback?.Invoke(isSuccess, errMsg, null);
            }
        });
    }
}
