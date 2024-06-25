using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Security.Cryptography.X509Certificates;


public class ServerManager : SingletonComponent<ServerManager>
{
    [SerializeField] private string apiURL = "http://159.65.171.191:3000/";
    [SerializeField] private bool debugMode = false;


    private void Start()
    {

    }

    public string GetServerBasePath()
    {
        return apiURL;
    }

    public void CreateFUser(string email, string password, string username, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "users/create/";
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);
        form.AddField("username", username);
        form.AddField("cTime", Convert.GetTimeStringForServer(System.DateTime.Now));
        GetData(path, form, callback);
    }

    public void UpdatePersonalInfo(string userId, int avatarId, string fName, string lName, string vName, bool isVegetarian, bool hasReligion, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "users/update/personal";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("fName", fName);
        form.AddField("lName", lName);
        form.AddField("vName", vName);
        form.AddField("avatarId", avatarId);
        form.AddField("isVegetarian", isVegetarian.ToString());
        form.AddField("hasReligion", hasReligion.ToString());
        GetData(path, form, callback);
    }

    public void GetFUser(string userId, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "users/getuser/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("cTime", Convert.GetTimeStringForServer(System.DateTime.Now));
        
        GetData(path, form, callback);
    }

    public void SaveFUser(FUser fUser, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "users/saveuser/";
        WWWForm form = new WWWForm();
        form.AddField("userId", fUser.id);
        form.AddField("fUserJson", JsonUtility.ToJson(new SUser(fUser)));

        GetData(path, form, callback);
    }

    public void SetToken(string userId, string token, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "users/settoken/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("token", token);

        GetData(path, form, callback);
    }

    public void GetEmail(string userName, System.Action<bool, string, string> callback = null)
    {
        string path = apiURL + "users/getEmail/";
        WWWForm form = new WWWForm();
        form.AddField("userName", userName);
        GetData(path, form, callback);
    }

    public void ChangeGameMode(string userId, Game_Mode mode, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "users/changegamemode/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("gameMode", (int)mode);
        GetData(path, form, callback);
    }

    public void CreateDailyTask(string userId, LTaskEntry entry, List<LSubTask> subTasks, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "entry/task/create/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("taskJson", JsonUtility.ToJson(entry));
        form.AddField("subTaskJson", JsonHelper.ToJson(subTasks.ToArray()));
        GetData(path, form, callback);
    }

    public void RemoveDailyTask(string userId, LTaskEntry entry, List<LSubTask> subTasks, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "entry/task/remove/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("taskJson", JsonUtility.ToJson(entry));
        form.AddField("subTaskJson", JsonHelper.ToJson(subTasks.ToArray()));
        GetData(path, form, callback);
    }

    public void CompleteDailyTask(string userId, LTaskEntry entry, List<LSubTask> subTasks, System.Action<bool, string, FUserWithReward> callback = null)
    {
        string path = apiURL + "entry/task/complete/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("taskJson", JsonUtility.ToJson(entry));
        form.AddField("subTaskJson", JsonHelper.ToJson(subTasks.ToArray()));
        GetData(path, form, callback);
    }

    public void CheckOffYesterdayTask(string userId, List<LTaskEntry> entryList, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "users/checkyesterday/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("taskJson", JsonHelper.ToJson(entryList.ToArray()));
        GetData(path, form, callback);
    }

    public void CancelDailyTaskComplete(string userId, LTaskEntry entry, List<LSubTask> subTasks, System.Action<bool, string, FUserWithReward> callback = null)
    {
        string path = apiURL + "entry/task/cancel/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("taskJson", JsonUtility.ToJson(entry));
        form.AddField("subTaskJson", JsonHelper.ToJson(subTasks.ToArray()));
        GetData(path, form, callback);
    }

    public void ArrangeDailyTask(string userId, List<LTaskEntry> entryList, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "entry/task/rearrange/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("taskJson", JsonHelper.ToJson(entryList.ToArray()));
        GetData(path, form, callback);
    }

    public void CompleteSubTask(string userId, LSubTask subTask, System.Action<bool, string, FUserWithReward> callback = null)
    {
        string path = apiURL + "entry/subtask/complete/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("subTaskJson", JsonUtility.ToJson(subTask));
        GetData(path, form, callback);
    }

    public void CancelSubTaskComplete(string userId, LSubTask subTask, System.Action<bool, string, FUserWithReward> callback = null)
    {
        string path = apiURL + "entry/subtask/cancel/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("subTaskJson", JsonUtility.ToJson(subTask));
        GetData(path, form, callback);
    }

    public void CreateToDo(string userId, LToDoEntry entry, List<LSubTask> subTasks, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "entry/todo/create/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("todoJson", JsonUtility.ToJson(entry));
        form.AddField("subTaskJson", JsonHelper.ToJson(subTasks.ToArray()));
        GetData(path, form, callback);
    }

    public void RemoveToDo(string userId, LToDoEntry entry, List<LSubTask> subTasks, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "entry/todo/remove/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("todoJson", JsonUtility.ToJson(entry));
        form.AddField("subTaskJson", JsonHelper.ToJson(subTasks.ToArray()));
        GetData(path, form, callback);
    }

    public void CompleteAutoToDo(string userId, string todoId, System.Action<bool, string, FUserWithReward> callback = null)
    {
        string path = apiURL + "entry/autotodo/complete/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("todoId", todoId);
        GetData(path, form, callback);
    }

    public void CompleteToDo(string userId, LToDoEntry entry, List<LSubTask> subTasks, System.Action<bool, string, FUserWithReward> callback = null)
    {
        string path = apiURL + "entry/todo/complete/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("todoJson", JsonUtility.ToJson(entry));
        form.AddField("subTaskJson", JsonHelper.ToJson(subTasks.ToArray()));
        GetData(path, form, callback);
    }

    public void ArrangeToDo(string userId, List<LToDoEntry> entryList, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "entry/todo/rearrange/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("todoJson", JsonHelper.ToJson(entryList.ToArray()));
        GetData(path, form, callback);
    }

    public void CreateHabit(string userId, LHabitEntry entry, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "entry/habit/create/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("habitJson", JsonUtility.ToJson(entry));
        GetData(path, form, callback);
    }

    public void RemoveHabit(string userId, LHabitEntry entry, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "entry/habit/remove/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("habitJson", JsonUtility.ToJson(entry));
        GetData(path, form, callback);
    }

    public void CompleteHabit(string userId, LHabitEntry entry, System.Action<bool, string, FUserWithReward> callback = null)
    {
        string path = apiURL + "entry/habit/complete/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("habitJson", JsonUtility.ToJson(entry));
        GetData(path, form, callback);
    }

    public void CancelHabitComplete(string userId, LHabitEntry entry, System.Action<bool, string, FUserWithReward> callback = null)
    {
        string path = apiURL + "entry/habit/cancel/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("habitJson", JsonUtility.ToJson(entry));
        GetData(path, form, callback);
    }

    public void ArrangeHabit(string userId, List<LHabitEntry> entryList, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "entry/habit/rearrange/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("habitJson", JsonHelper.ToJson(entryList.ToArray()));
        GetData(path, form, callback);
    }


    public void CreateProject(string userId, LProjectEntry entry, List<LSubTask> subTasks, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "entry/project/create/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("projectJson", JsonUtility.ToJson(entry));
        form.AddField("subTaskJson", JsonHelper.ToJson(subTasks.ToArray()));
        GetData(path, form, callback);
    }

    public void RemoveProject(string userId, LProjectEntry entry, List<LSubTask> subTasks, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "entry/project/remove/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("projectJson", JsonUtility.ToJson(entry));
        form.AddField("subTaskJson", JsonHelper.ToJson(subTasks.ToArray()));
        GetData(path, form, callback);
    }

    public void CompleteProject(string userId, LProjectEntry entry, List<LSubTask> subTasks, System.Action<bool, string, FUserWithReward> callback = null)
    {
        string path = apiURL + "entry/project/complete/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("projectJson", JsonUtility.ToJson(entry));
        form.AddField("subTaskJson", JsonHelper.ToJson(subTasks.ToArray()));
        GetData(path, form, callback);
    }

    public void ArrangeProject(string userId, List<LProjectEntry> entryList, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "entry/project/rearrange/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("projectJson", JsonHelper.ToJson(entryList.ToArray()));
        GetData(path, form, callback);
    }

    public void LoadCResources(System.Action<bool, string, List<CResource>> callback = null)
    {
        string path = apiURL + "resource/getAllCRes/";
        GetDataList(path, new WWWForm(), callback);
    }

    public void LoadCBuildings(System.Action<bool, string, List<CBuilding>> callback = null)
    {
        string path = apiURL + "resource/getAllCBuild/";
        GetDataList(path, new WWWForm(), callback);
    }

    public void LoadCVillagers(System.Action<bool, string, List<CVillager>> callback = null)
    {
        string path = apiURL + "resource/getAllCVil/";
        GetDataList(path, new WWWForm(), callback);
    }

    public void LoadCArtworks(System.Action<bool, string, List<CArtwork>> callback = null)
    {
        string path = apiURL + "resource/getAllArtwork/";
        GetDataList(path, new WWWForm(), callback);
    }

    public void LoadCArtifacts(System.Action<bool, string, List<CArtifact>> callback = null)
    {
        string path = apiURL + "resource/getAllArtifact/";
        GetDataList(path, new WWWForm(), callback);
    }

    public void LoadCoalitions(System.Action<bool, string, List<FCoalition>> callback = null)
    {
        string path = apiURL + "connection/coalition/getall/";
        GetDataList(path, new WWWForm(), callback);
    }

    public void LoadInvitations(System.Action<bool, string, List<FInvitation>> callback = null)
    {
        string path = apiURL + "connection/invite/getall/";
        GetDataList(path, new WWWForm(), callback);
    }

    public void GetInvitations(string userId, System.Action<bool, string, List<FInvitation>> callback = null)
    {
        string path = apiURL + "connection/invite/getRInvites/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        GetDataList(path, form, callback);
    }

    public void CreateCoalition(string userId, string coalitionName, string timeZone, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "users/coalition/create/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("coalitionName", coalitionName);
        form.AddField("timeZone", timeZone);
        GetData(path, form, callback);
    }

    public void GetCoalitionMembers(string userId, System.Action<bool, string, List<FUser>> callback = null)
    {
        string path = apiURL + "users/coalition/getusers/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        GetDataList(path, form, callback);
    }


    public void LeaveCoalition(string userId, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "users/coalition/leave/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        GetData(path, form, callback);
    }

    public void OpenCoalition(string userId, System.Action<bool, string, List<FCoalition>> callback = null)
    {
        string path = apiURL + "users/coalition/open/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        GetDataList(path, form, callback);
    }

    public void CloseCoalition(string userId,System.Action<bool, string, List<FCoalition>> callback = null)
    {
        string path = apiURL + "users/coalition/close/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        GetDataList(path, form, callback);
    }

    public void SendJoinInvite(string userId, string coalitionName, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "users/coalition/sendjoininvite/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("coalitionName", coalitionName);
        GetData(path, form, callback);
    }

    public void InviteVillagerToCoalition(string userId, string villagerName, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "users/coalition/invite1/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("villagerName", villagerName);
        GetData(path, form, callback);
    }

    public void InviteUserToCoalition(string userId, string invitee, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "users/coalition/invite/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("inviteeName", invitee);
        GetData(path, form, callback);
    }

    public void AcceptCoalitionInvite(string userId, string inviteId, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "users/coalition/accept/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("inviteId", inviteId);
        GetData(path, form, callback);
    }

    public void RejectCoalitionInvite(string userId, string inviteId, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "users/coalition/reject/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("inviteId", inviteId);
        GetData(path, form, callback);
    }

    public void KickCoalitionMember(string userId, string memberId, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "users/coalition/kick/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("memberId", memberId);
        GetData(path, form, callback);
    }
    public void Excavate(string userId, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "users/art/excavate/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        GetData(path, form, callback);
    }

    public void CompleteExcavate(string userId, string artifactId, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "users/art/completeExcavate/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("artifactId", artifactId);
        GetData(path, form, callback);
    }

    public void PickArtwork(string userId, string artworkJson, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "users/art/pick/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("artworkJson", artworkJson);
        GetData(path, form, callback);
    }

    public void GetCoalitionMessage(string coalition, System.Action<bool, string, List<FMessage>> callback = null)
    {
        string path = apiURL + "connection/message/public/";
        WWWForm form = new WWWForm();
        form.AddField("coalition", coalition);
        GetDataList(path, form, callback);
    }

    public void GetPrivateMessage(string userId, System.Action<bool, string, List<FMessage>> callback = null)
    {
        string path = apiURL + "connection/message/private/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        GetDataList(path, form, callback);
    }

    public void SendCoalitionMessage(string userId, string content, System.Action<bool, string, FMessage> callback = null)
    {
        string path = apiURL + "connection/message/sendToCoalition/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("content", content);
        form.AddField("createdAt", Utilities.SystemTicks.ToString());
        GetData(path, form, callback);
    }

    public void SendPrivateMessage(string userId, string receiverId, string content, System.Action<bool, string, FMessage> callback = null)
    {
        string path = apiURL + "connection/message/sendToUser/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("receiver", receiverId);
        form.AddField("content", content);
        form.AddField("createdAt", Utilities.SystemTicks.ToString());
        GetData(path, form, callback);
    }

    public void SendTradeInvite(string userId, string receiver, string resource1, float amount1, string resource2, float amount2, int repeat, ETradeInviteType type, System.Action<bool, string, FInvitation> callback = null)
    {
        string path = apiURL + "connection/trade/sendInvite/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("receiver", receiver);
        form.AddField("resource1", resource1);
        form.AddField("amount1", amount1.ToString());
        form.AddField("resource2", resource2);
        form.AddField("amount2", amount2.ToString());
        form.AddField("repeat", repeat);
        form.AddField("type", type.ToString());
        GetData(path, form, callback);
    }

    public void AcceptTradeInvite(string userId, string inviteId, System.Action<bool, string, FInvitation> callback = null)
    {
        string path = apiURL + "connection/trade/acceptInvite/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("inviteId", inviteId);
        GetData(path, form, callback);
    }

    public void RejectTradeInvite(string userId, string inviteId, System.Action<bool, string, FInvitation> callback = null)
    {
        string path = apiURL + "connection/trade/rejectInvite/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("inviteId", inviteId);
        GetData(path, form, callback);
    }

    public void GetTradeInvites(string userId, System.Action<bool, string, List<FTradeInvitation>> callback = null)
    {
        string path = apiURL + "connection/trade/getInvites/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        GetDataList(path, form, callback);
    }

    public void GetTrades(string userId, System.Action<bool, string, List<FTrade>> callback = null)
    {
        string path = apiURL + "connection/trade/getTrades/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        GetDataList(path, form, callback);
    }

    public void CancelTrades(string userId, string tradeId, System.Action<bool, string, List<FTrade>> callback = null)
    {
        string path = apiURL + "connection/trade/cancel/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("tradeId", tradeId);
        GetDataList(path, form, callback);
    }

    public void PostArtTrades(string userId, string paint, string artist1, string artist2, System.Action<bool, string, FArtTrade> callback = null)
    {
        string path = apiURL + "connection/artTrade/post/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("paint", paint);
        form.AddField("artist1", artist1);
        form.AddField("artist2", artist2);
        GetData(path, form, callback);
    }

    public void SubmitArtTrades(string userId, string tradeId, string paint, string artist, System.Action<bool, string, FArtTrade> callback = null)
    {
        string path = apiURL + "connection/artTrade/submit/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("tradeId", tradeId);
        form.AddField("paint", paint);
        form.AddField("artist", artist);
        GetData(path, form, callback);
    }

    public void AcceptArtTrades(string userId, string tradeId, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "connection/artTrade/accept/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("tradeId", tradeId);
        GetData(path, form, callback);
    }

    public void RejectArtTrades(string userId, string tradeId, System.Action<bool, string, FArtTrade> callback = null)
    {
        string path = apiURL + "connection/artTrade/reject/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("tradeId", tradeId);
        GetData(path, form, callback);
    }

    public void RemoveArtTrades(string userId, string tradeId, System.Action<bool, string, FArtTrade> callback = null)
    {
        string path = apiURL + "connection/artTrade/remove/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("tradeId", tradeId);
        GetData(path, form, callback);
    }

    public void GetAllArtTrades(System.Action<bool, string, List<FArtTrade>> callback = null)
    {
        string path = apiURL + "connection/artTrade/getAll/";
        WWWForm form = new WWWForm();
        GetDataList(path, form, callback);
    }

    public void GetSendArtTrades(string userId, System.Action<bool, string, List<FArtTrade>> callback = null)
    {
        string path = apiURL + "connection/artTrade/getSend/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        GetDataList(path, form, callback);
    }

    public void GetReceiveArtTrades(string userId, System.Action<bool, string, List<FArtTrade>> callback = null)
    {
        string path = apiURL + "connection/artTrade/getReceive/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        GetDataList(path, form, callback);
    }

    public void StartToBuild(string userId, string bId, string cBuildingId, bool bQuick, string createdAt, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "users/build/start/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("bId", bId);
        form.AddField("cBuildingId", cBuildingId);
        form.AddField("bQuick", bQuick.ToString());
        form.AddField("createdAt", createdAt);
        GetData(path, form, callback);
    }

    public void CompleteBuilding(string userId, string bId, string createdAt, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "users/build/complete/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("bId", bId);
        form.AddField("createdAt", createdAt);
        GetData(path, form, callback);
    }

    public void CancelBuilding(string userId, string bId, string cBuildingId, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "users/build/cancel/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("bId", bId);
        form.AddField("cBuildingId", cBuildingId);
        GetData(path, form, callback);
    }

    public void ConvertQuickBuilding(string userId, string bId, string cBuildingId, string createdAt, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "users/build/cancel/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("bId", bId);
        form.AddField("cBuildingId", cBuildingId);
        form.AddField("createdAt", createdAt);
        GetData(path, form, callback);
    }

    public void ExchangeResource(string userId, EResources type1, float amount1, EResources type2, float amount2, System.Action<bool, string, FUser> callback = null)
    {
        string path = apiURL + "users/resource/exchange/";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("resType1", type1.ToString());
        form.AddField("amount1", amount1.ToString());
        form.AddField("resType2", type2.ToString());
        form.AddField("amount2", amount2.ToString());
        GetData(path, form, callback);
    }

    ////////////////////////////-----PRIVATE FUNCTIONS------////////////////////////

    private void GetData(string path, WWWForm form, System.Action<bool, string, string> callback = null)
    {
        StartCoroutine(ECallPostAPI(path, form, (code, errMsg, response) =>
        {
            callback?.Invoke(response.status == "success" && code == 200, errMsg, response.info);
        }));
    }


    private void GetData<T>(string path, WWWForm form, System.Action<bool, string, T> callback = null) where T : Data
    {
        StartCoroutine(ECallPostAPI(path, form, (code, errMsg, response) =>
        {
            if (response != null && code == 200 && response.status.ToLower().Trim() == "success" && !response.info.Equals(string.Empty))
            {
                try
                {
                    var data = DBHandler.ReadFromJSON<T>(response.info);
                    callback?.Invoke(true, response.message, data);
                }
                catch(Exception error)
                {
                    callback?.Invoke(false, "JSON Parsing Error when getting data: " + error.ToString(), null);
                }
            }
            else
            {
                callback?.Invoke(false, errMsg, null);
            }
        }));
    }

    private void GetDataList<T>(string path, WWWForm form, System.Action<bool, string, List<T>> callback = null) where T : Data
    {
        StartCoroutine(ECallPostAPI(path, form, (code, errMsg, response) =>
        {
            var list = new List<T>();
            
            if (response != null && code == 200 && response.status.ToLower().Trim() == "success")
            {
                try
                {
                    list = DBHandler.ReadListFromJSON<T>(response.info);
                    callback?.Invoke(true, response.message, list);
                }
                catch(Exception error)
                {
                    callback?.Invoke(false, "JSON Parsing Error when getting data" + error.ToString(), list);
                }
            }
            else
            {
                callback?.Invoke(false, errMsg, list);
            }
        }));
    }

    private IEnumerator ECallPostAPI(string path, WWWForm form, System.Action<long, string, WResponse> callback = null)
    {
        using UnityWebRequest www = UnityWebRequest.Post(path, form);
        www.certificateHandler = new AcceptAllCertificateSignedWithASpecificKeyPublicKey();
        yield return www.SendWebRequest();

        if (www.downloadHandler.text == "")
        {
            callback?.Invoke(www.responseCode, "Server Connection Error: " + www.error, null);
        }
        try
        {
            WResponse response = JsonUtility.FromJson<WResponse>(www.downloadHandler.text);
            callback?.Invoke(www.responseCode, response.message, response);
        }
        catch (Exception error)
        {
            Debug.Log(path);
            Debug.Log(www.downloadHandler.text);
            callback?.Invoke(www.responseCode, "JSON Parsing Error. Response has wrong format. Please check the api" + error.ToString(), null);
        }
        www.Dispose();
    }

    private IEnumerator ECallGetAPI(string path, System.Action<bool, string> callback = null)
    {
        Debug.LogError(path);
        using UnityWebRequest www = UnityWebRequest.Get(path);
        //www.certificateHandler = new AcceptAllCertificateSignedWithASpecificKeyPublicKey();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.downloadHandler.text == "" ? "Server Connection Error: " + www.error : www.downloadHandler.text);
            callback?.Invoke(false, www.downloadHandler.text == "" ? "Server Connection Error: " + www.error : www.downloadHandler.text);
        }
        else
        {
            Debug.LogError(www.downloadHandler.text);
        }
        www.Dispose();
    }


    public class AcceptAllCertificateSignedWithASpecificKeyPublicKey : CertificateHandler
    {
        //private static string PUB_KEY = "3082010A0282010100A0CF073A881D3EF46821AD0F0D4936A33504BFB68C9E79A53B067F31BBDC96B4B04DC032C8DAD360E710A44AA6D291C79EED3C1FAD4E7A2FB11A241CDE454F87B0058BA5B769E5CAAC580D63E396D0E5B6CFC9F03436C094B9A9EB9A0C8F9F9623421A724B0BF0F8E0F2B2531F615E36905A6E55BF1840F99837CD26F8ADD06D156D3F0B602A83121E3FDF2B075C2B4B165453521898F95B58BEAFEF879BD0C2BE3BD58844BC20A5C395FE36CF8B2E0298B755822DBE9A1E0E26523B56C55F055BFC26AA2369908E1FA8D7677E2DC2F89D6C2B60D06CAFB4981FFD53A4F8C5FDD39202161E8AB45108390CB6171658469D53EDC56ED9383ACC1F748ED0134BAF0203010001";

        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
            /*X509Certificate2 certificate = new(certificateData);
            string pk = certificate.GetPublicKeyString();
            if (pk.Equals(PUB_KEY))
            {
                return true;
            }

            return false;*/
        }
    }


    public class WResponse
    {
        public string status = "";
        public string message = "";
        public string info = "";
        public WResponse()
        {
            status = "";
            message = "";
            info = "";
        }
    }
}