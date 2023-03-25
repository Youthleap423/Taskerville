using Firebase.Firestore;
using System.Collections.Generic;
using System.Linq;

[FirestoreData]
public class FArtTrade : FInvitation
{
    [FirestoreProperty]
    public string painting1 { get; set; }

    [FirestoreProperty]
    public string painting2 { get; set; }

    [FirestoreProperty]
    public string artistName1 { get; set; }

    [FirestoreProperty]
    public string artistName2 { get; set; }

    [FirestoreProperty]
    public List<string> readers { get; set; }
    
    public FArtTrade()
    {
        collectionId = "ArtTrade";
        painting1 = "";
        painting2 = "";
        artistName1 = "";
        artistName2 = "";
        readers = new List<string>();
        state = EState.Posted.ToString();
    }

    public FArtTrade(string painting, string artist)
    {
        collectionId = "ArtTrade";
        painting1 = painting;
        artistName1 = artist;
        painting2 = "";
        artistName2 = "";
        readers = new List<string>();
        state = EState.Posted.ToString();
    }

    public FArtTrade(CArtwork artwork)
    {
        collectionId = "ArtTrade";
        painting1 = artwork.name;
        artistName1 = artwork.artist_name;
        painting2 = "";
        artistName2 = "";
        readers = new List<string>();
        state = EState.Posted.ToString();
    }

    public void Submit(string painting, string artist)
    {
        painting2 = painting;
        artistName2 = artist;
        state = EState.Created.ToString();
        reply_at = Convert.DateTimeToFDate(System.DateTime.Now);
    }

    public void Accept()
    {
        state = EState.Agreed.ToString();
        reply_at = Convert.DateTimeToFDate(System.DateTime.Now);
    }

    public void Decline()
    {
        state = EState.Declined.ToString();
        reply_at = Convert.DateTimeToFDate(System.DateTime.Now);
    }
}
