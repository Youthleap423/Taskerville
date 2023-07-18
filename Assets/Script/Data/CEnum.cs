
public enum Game_Mode
{
    Task_Only = 0,
    Task_And_Game,
    Game_Only
}

public enum Interaction_Mode
{
    Partial_Management = 0,
    Manual_Management
}

public enum Difficuly
{
    Easy = 0,
    Medium,
    Hard,
    VeryHard
}

public enum Repeatition
{
    Daily = 0,
    Weekly = 1,
    Monthly = 2,
    Yearly = 3
}

public enum EntryType
{
    NULL,
    DailyTask,
    ToDo,
    Project,
    Habit,
    AutoToDo,
    AutoGoal
}

public enum ESubEntryType
{
    NULL,
    SubDailyTask,
    SubToDo,
    SubProject
}

public enum EResources
{
    Happiness,
    Gold,
    Iron_Ore,
    Raw_Silver,
    Raw_Gold,
    Stone,
    Logs,
    Lumber,
    Iron,
    Ale,
    Wine,
    Wheat,
    Barley,
    Corn,
    Apples,
    Pears,
    Melons,
    Grapes,
    Cherries,
    Raspberries,
    Peaches,
    Malted_Barley,
    Fish,
    Bread,
    Fava_Beans,
    Eggs,
    Meat,
    Lamb,
    Swine,
    Jewelry,
    Fine_Clothes,
    Clothes,
    Spices,
    Garlic,
    Onion,
    Wagon,
    Beef,
    Loaf,
    Flour,
    Cabbage,
    Deer,
    Cattle,
    Strawberry,
    Plank,
    Ruby,
    Diamond,
    Sapphire,
    Paint,
    Culture,
    Artifact_Rare,
    Artifact_Common,
    Artifact_UnCommon,
    Goat,
    Meal,
    Cattle_Meat,
    Swine_Meat,
    Goat_Meat,
    Deer_Meat
}

public enum EEffect_Type
{
    Happiness,
    Meal
}
public enum EDates
{
    Happy70Below,
    Happy7075,
    Happy7580,
    Happy8085,
    Happy8590,
    Happy9095,
    Happy95Above,
    Salary,
    Maintenance,
    Meal,
    Sale,
    Buy,
    Museum,
    DailyTask,
    WeeklyTaskHabit,
    Assist,
    Artifact,
}

public enum EMarketType
{
    BuyFromRepublic = 0,
    SellToRepublic,
    BuyFromMerchant,
    SellToMerchant,
    BuytFromCoalition,
    SellToCoalition,
    BuyFromSpecialist
}

public enum EVillagerType
{
    Mayor,
    Administrator,
    Farmer,
    Rancher,
    Woodcutter,
    Miner,
    Quarryman,
    Baker,
    Inn_Keeper,
    Beach_Inn_Keeper,
    Barkeep,
    Blacksmith,
    Miller,
    Spiritual_Leader,
    Sawyer,
    Currator,
    Butcher,
    Wharf_Master,
    Merchant,
    Fisherman,
    Medicine_Woman,
    Brewster,
    Laborer,
    Spouse,
    Child,
    Commander,
    Kinght,
    Crossbowman,
    Cabinet,
    Archer,
    Trebuchet,
    Charger,
    Builder,
    Archeologist,
    Acquisitions_Trader,
    Judge,
    Constable,
    Bailiff,
    Swordsman,
    Philosopher,
    ShopKeeper,
    Casetle_Servant
}

public enum EBuildingType
{
    Castle,
    Farm,
    Cottage1,
    Cottage2,
    Bakery,
    Mine_Iron,
    Mine_Silver,
    Quarry,
    Tavern,
    Market,
    Iron_Froge,
    Watermill,
    Building,
    Water_Well,
    Sawmill,
    Statues,
    Museum,
    Bridge,
    Butchery,
    Cabin,
    Rance,
    BellTower,
    AdminOffice,
    Fort,
    TrainingGear,
    PlenaryHall,
    Barracks,
    HouseYard,
    Obelisk,
    Road,
    Unique,
    ArchealogicalDig,
    WharfStorage,
    Wharf,
    Ruin,
    MerchantShip,
    MerchantStore,
    Temple,
    Hospital,
    BuilderDepot,
    MedicinalGarden,
    Inn,
    FairGround
}

public enum EInviteType
{
    Join_Coalition,
    Invite_Coalition
}

public enum ETradeInviteType
{
    Request,
    Offer
}

public enum ETradeRepeat
{
    Once = 0,
    Daily = 1,
    Every2Days = 2,
    Weekly = 7
}

public enum EState
{
    Created,
    Agreed,
    Declined,
    Posted
}

public enum EMessageType
{
    Public,
    Private,
    Notification
}

public enum EConstructionDlgType
{
    None,
    NoBuilder,
    AskSpecialist,
    AskCurator,
    ImmediateBuild,
    HireSpecialist
}

public enum EAutoBuildType
{
    Building,
    Villager
}

public enum EActionState
{
    Wait,
    Ready,
    Start,
    Progess,
    End
}

public enum EPopUpDlg
{
    None,
    AdminPrompt,
    MissingTask,
    DailyReport,
    NewArtifact,
    NewArtwork,
    DailyReminder,
}

public enum EArtworkReason
{
    Init,
    Buy,
    Trade,
    WeeklyTaskComplete,
    Happy65,
    Happy70,
    Happy75,
    Happy80,
    Happy85,
    Happy90,
    Happy95,
    Happy100,
    Population65,
    Population75,
    Population85,
    Population95,
    Population100,
    Build_Religious,
    Build_Gallery,
    Build_Museum,
    Build_Park,
    Build_Cemetery,
    Build_HerbGarden
}