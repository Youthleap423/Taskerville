using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommunicationViewController : SingletonComponent<CommunicationViewController>
{
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
        return GetCurrentCoalitionList().Find(item => item.Id == id);
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

        LUser user = UserViewController.Instance.GetCurrentUser();
        FCoalition coalition = new FCoalition(user, name);
        
        DataManager.Instance.CreateCoalition(coalition, (isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                DataManager.Instance.UpdateUser(coalition.name, coalition.name, callback);
            }
            else
            {
                callback(isSuccess, errMsg);
            }
        });
    }

    public FCoalition FindCoalition(string id)
    {
        foreach(FCoalition coalition in GetCurrentCoalitionList())
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

        var fCoalition = GetCoalitionWithName(fUser.joined_coalition);

        if (fCoalition == null)
        {
            callback(false, "Couldn't find your coalition.", null);
            return;
        }

        List<LUser> resultList = new List<LUser>();
        UserViewController.Instance.LoadCurrentUserList((isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                List<LUser> userList = UserViewController.Instance.GetCurrentUserList();
                foreach (LUser user in userList)
                {
                    
                    if (user.id == fUser.id)
                    {
                        continue;
                    }

                    if (fCoalition.members.Contains(user.id))
                    {
                        resultList.Add(user);
                    }
                }
                callback(true, "", resultList);
            }
            else
            {
                callback(false, "Error to load users", null);
            }
        });
    }

    public void LoadCoalitions(System.Action<bool, string> callback)
    {
        DataManager.Instance.LoadCoalitionData((isSuccess, errMsg, coalitionList) =>
        {
            callback(isSuccess, errMsg);
        });
    }

    public void GetCurrentCoalitionList(System.Action<bool, string, List<FCoalition>> callback)
    {
        DataManager.Instance.LoadCoalitionData(callback);
    }

    public void ChangePublic(FCoalition coalition, System.Action<bool, string> callback)
    {
        DataManager.Instance.UpdateCoalition(coalition, callback);
    }

    public void JoinCoalition(string name, System.Action<bool, string> callback)
    {
        LUser user = UserViewController.Instance.GetCurrentUser();

        if (!user.joined_coalition.Equals(""))
        {
            callback(false, "You've already joined a coalition.");
            return;
        }

        if (user.created_coalition.ToLower().Equals(name.ToLower()))
        {
            JoinOwnCoalition(callback);
            return;
        }

        FCoalition joinCoalition = GetCoalitionWithName(name);
        
        if (joinCoalition == null)
        {
            callback(false, "There is no coalition named " + name);
            return;
        }

        InvitationViewController.Instance.SendInvitation(joinCoalition.Pid, EInviteType.Join_Coalition, callback);
    }

    public void JoinOwnCoalition(System.Action<bool, string> callback)
    {
        LUser user = UserViewController.Instance.GetCurrentUser();
        DataManager.Instance.UpdateUser(user.created_coalition, user.created_coalition, (isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                var fCoalition = DataManager.Instance.GetCurrentCoalitions().ToList().Find(item => item.name.ToLower() == user.created_coalition.ToLower());
                if (fCoalition == null)
                {
                    callback(false, "Couldn't joined your coalition.");
                    return;
                }
                fCoalition.AddMember(user.id, callback);
            }
            else
            {
                callback(isSuccess, errMsg);
            }
        });
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

    public void LeaveCoalition()
    {
        LUser user = UserViewController.Instance.GetCurrentUser();
        user.UpdateJoinCoalition("");
    }

    public void LeaveCoalition(System.Action<bool, string> callback)
    {
        LUser user = UserViewController.Instance.GetCurrentUser();
        var fCoalition = GetCoalitionWithName(user.joined_coalition);
        if (fCoalition == null)
        {
            callback(false, "Couldn't find your coalition.");
            return;
        }

        fCoalition.RemoveMember(user.id, (isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                DataManager.Instance.UpdateUser(user.created_coalition, "", callback);
            }
            else
            {
                callback(isSuccess, errMsg);
            }
        });
    }

    public void KickCoalitionMember(LUser user, System.Action<bool, string> callback)
    {
        var fCoalition = GetCoalitionWithName(user.joined_coalition);
        if (fCoalition == null)
        {
            callback(false, "Couldn't find your coalition.");
            return;
        }

        fCoalition.RemoveMember(user.id, (isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                user.joined_coalition = "";
                user.Update(user.created_coalition, "", callback);
            }
            else
            {
                callback(isSuccess, errMsg);
            }
        });
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
    public void SendFMessageToCoalition(List<LUser> userList, string msg, System.Action<bool, string, FMessage> callback)
    {
        LUser me = UserViewController.Instance.GetCurrentUser();
        FMessage fMessage = new FMessage(me, userList, msg, EMessageType.Public);
        DataManager.Instance.CreateMessages(fMessage, (isSuccess, errMsg) =>
        {
            callback(isSuccess, errMsg, fMessage);
        });
    }

    public void SendFMessage(LUser receiver, string msg, System.Action<bool, string, FMessage> callback)
    {
        LUser me = UserViewController.Instance.GetCurrentUser();
        FMessage fMessage = new FMessage(me, receiver, msg, EMessageType.Private);
        DataManager.Instance.CreateMessages(fMessage, (isSuccess, errMsg) =>
        {
            callback(isSuccess, errMsg, fMessage);
        });
    }

    public void LoadPublicMessages(string coalitionName, System.Action<bool, string, List<FMessage>> callback)
    {
        DataManager.Instance.GetPublicMessages(coalitionName, callback);
    }

    public void LoadPrivateMessages(List<string> userIds, System.Action<bool, string, List<FMessage>> callback)
    {
        DataManager.Instance.GetPrivateMessages(userIds, callback);
    }
}
