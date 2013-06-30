using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;


//NetSquare C#.NET FourSquare API v2 Class Library
//Alpha release v4

//Add a Reference to your project to:
//System.Web.Extensions.dll
//This will be found at something like: 
//C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.Web.Extensions.dll
//Target Framework: .NET Framework 4

//Written by Kevin D. MacDonald
//July, 2011
//Vancouver, Canada
//Kevin@geekfrog.ca

//Todo: Error handling for incorrect parameters and calls.
//Todo: Use GetDictionaryValue whereever possible
//Pass the Versioning date and watch for depeciated messages - like VenueEditor

public class NetSquare
{
    #region Users

    /// <summary>
    /// Returns profile information for a given user, including selected badges and mayorships. 
    /// </summary>
    /// <param name="USERID">Identity of the user to get details for. Pass self to get details of the acting user</param>
    public static FourSquareUser User(string AccessToken)
    {
        return User("", AccessToken);
    }

    /// <summary>
    /// Returns profile information for a given user, including selected badges and mayorships. 
    /// </summary>
    /// <param name="USERID">Identity of the user to get details for. Pass self to get details of the acting user</param>
    public static FourSquareUser User(string USERID, string AccessToken)
    {
        if (USERID.Equals(""))
        {
            USERID = "self";
        }
        HTTPGet GET = new HTTPGet();
        string EndPoint = "https://api.foursquare.com/v2/users/" + USERID + "?oauth_token=" + AccessToken;
        GET.Request(EndPoint);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(GET.ResponseBody);
        JSONDictionary = ExtractDictionary(JSONDictionary, "response:user");
        FourSquareUser User = new FourSquareUser(JSONDictionary);
        return User;
    }

    /// <summary>
    /// Returns the user's leaderboard.  
    /// </summary>
    /// <param name="neighbors">Number of friends' scores to return that are adjacent to your score, in ranked order. </param>
    public static FourSquareLeaderBoard UserLeaderBoard(string AccessToken)
    {
        return UserLeaderBoard(2, AccessToken);
    }

    /// <summary>
    /// Returns the user's leaderboard.  
    /// </summary>
    /// <param name="neighbors">Number of friends' scores to return that are adjacent to your score, in ranked order. </param>
    public static FourSquareLeaderBoard UserLeaderBoard(int Neighbors, string AccessToken)
    {
        HTTPGet GET = new HTTPGet();
        string EndPoint = "https://api.foursquare.com/v2/users/leaderboard?neighbors=" + Neighbors.ToString() + "&oauth_token=" + AccessToken;
        GET.Request(EndPoint);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(GET.ResponseBody);
        FourSquareLeaderBoard FSLeaderBoard = new FourSquareLeaderBoard(JSONDictionary);
        return FSLeaderBoard;
    }

    /// <summary>
    /// Helps a user locate friends.   
    /// </summary>
    /// <param name="Phone">A comma-delimited list of phone numbers to look for.</param>
    /// <param name="EMail">A comma-delimited list of email addresses to look for.</param>
    /// <param name="Twitter">A comma-delimited list of Twitter handles to look for.</param>
    /// <param name="TwitterSource">A single Twitter handle. Results will be friends of this user who use Foursquare.</param>
    /// <param name="Fbid">A comma-delimited list of Facebook ID's to look for.</param>
    /// <param name="Name">A single string to search for in users' names</param>
    public static List<FourSquareUser> UserSearch(string Phone, string Email, string Twitter, string TwitterSource, string Fbid, string Name, string AccessToken)
    {
        List<FourSquareUser> FoundUsers = new List<FourSquareUser>();

        HTTPGet GET = new HTTPGet();
        string Query = "";

        //Phone
        if (!Phone.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "phone=" + Phone;
        }

        //Email
        if (!Email.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "email=" + Email;
        }

        //Twitter
        if (!Twitter.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "twitter=" + Twitter;
        }

        //TwitterSource
        if (!TwitterSource.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "twitterSource=" + TwitterSource;
        }

        //Fbid
        if (!Fbid.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "fbid=" + Fbid;
        }

        //Name
        if (!Name.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "name=" + Name;
        }
        if (!Query.Equals(""))
        {
            string EndPoint = "https://api.foursquare.com/v2/users/search" + Query + "&oauth_token=" + AccessToken;
            GET.Request(EndPoint);
            Dictionary<string, object> JSONDictionary = JSONDeserializer(GET.ResponseBody);
            Dictionary<string, object> ItemsDictionary = new Dictionary<string, object>();
            ItemsDictionary = ExtractDictionary(JSONDictionary, "response");
            object[] Items = ((object[])ItemsDictionary["results"]);
            for (int x = 0; x < Items.Length; x++)
            {
                FoundUsers.Add(new FourSquareUser(((Dictionary<string, object>)Items[x])));
            }
        }
        return FoundUsers;
    }

    /// <summary>
    /// Shows a user the list of users with whom they have a pending friend request    
    /// </summary>
    public static List<FourSquareUser> UserRequests(string AccessToken)
    {
        List<FourSquareUser> FoundUserRequests = new List<FourSquareUser>();

        HTTPGet GET = new HTTPGet();
        string EndPoint = "https://api.foursquare.com/v2/users/requests" + "?oauth_token=" + AccessToken;
        GET.Request(EndPoint);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(GET.ResponseBody);
        foreach (object obj in (object[])ExtractDictionary(JSONDictionary, "response")["requests"])
        {
            FoundUserRequests.Add(new FourSquareUser((Dictionary<string, object>)obj));
        }
        return FoundUserRequests;
    }

    /// <summary>
    /// Returns badges for the current user.    
    /// </summary>
    public static FourSquareBadgesAndSets UserBadges(string AccessToken)
    {
        return UserBadges("", AccessToken);
    }

    /// <summary>
    /// Returns badges for a given user.    
    /// </summary>
    /// <param name="USER_ID">ID for user to view badges for..</param>
    public static FourSquareBadgesAndSets UserBadges(string USERID, string AccessToken)
    {
        FourSquareBadgesAndSets BadgesAndSets = new FourSquareBadgesAndSets();

        if (USERID.Equals(""))
        {
            USERID = "self";
        }
        HTTPGet GET = new HTTPGet();
        string EndPoint = "https://api.foursquare.com/v2/users/" + USERID + "/badges?oauth_token=" + AccessToken;
        GET.Request(EndPoint);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(GET.ResponseBody);

        JSONDictionary = ExtractDictionary(JSONDictionary, "response");

        if (JSONDictionary.ContainsKey("defaultSetType"))
        {
            BadgesAndSets.defaultSetType = JSONDictionary["defaultSetType"].ToString();
        }

        foreach (KeyValuePair<string, object> Obj in (Dictionary<string, object>)JSONDictionary["badges"])
        {
            BadgesAndSets.Badges.Add(new FourSquareBadge((Dictionary<string, object>)Obj.Value));
        }

        foreach (object Obj in (object[])((Dictionary<string, object>)JSONDictionary["sets"])["groups"])
        {
            BadgesAndSets.BadgeSets.Add(new FourSquareBadgeSet((Dictionary<string, object>)Obj));
        }
        return BadgesAndSets;
    }

    /// <summary>
    /// Returns a history of checkins for the authenticated user. 

    /// </summary>
    /// <param name="USER_ID">For now, only "self" is supported</param>
    public static List<FourSquareCheckin> UsersCheckins(string USER_ID, string AccessToken)
    {
        return UsersCheckins(USER_ID, 100, 0, 0, 0, AccessToken);
    }

    /// <summary>
    /// Returns a history of checkins for the authenticated user. 
    /// </summary>
    /// <param name="USER_ID">For now, only "self" is supported</param>
    /// <param name="Limit">For now, only "self" is supported</param>
    /// <param name="Offset">Used to page through results.</param>
    /// <param name="afterTimestamp">Retrieve the first results to follow these seconds since epoch.</param>
    /// <param name="beforeTimeStamp">Retrieve the first results prior to these seconds since epoch</param>
    public static List<FourSquareCheckin> UsersCheckins(string USER_ID, int Limit, int Offset, double afterTimestamp, double beforeTimeStamp, string AccessToken)
    {
        List<FourSquareCheckin> Checkins = new List<FourSquareCheckin>();

        HTTPGet GET = new HTTPGet();
        string Query = "";

        if (Limit > 0)
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "Limit=" + Limit.ToString();
        }

        if (Offset > 0)
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "Offset=" + Offset.ToString();
        }

        if (afterTimestamp > 0)
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "afterTimestamp=" + afterTimestamp.ToString();
        }

        if (beforeTimeStamp > 0)
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "beforeTimeStamp=" + beforeTimeStamp.ToString();
        }

        if (Query.Equals(""))
        {
            Query = "?";
        }
        else
        {
            Query += "&";
        }
        string EndPoint = "https://api.foursquare.com/v2/users/" + USER_ID + "/checkins" + Query + "oauth_token=" + AccessToken;
        GET.Request(EndPoint);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(GET.ResponseBody);

        Dictionary<string, object> ItemsDictionary = ExtractDictionary(JSONDictionary, "response:checkins");
        object[] Items = ((object[])ItemsDictionary["items"]);

        foreach (object obj in Items)
        {
            Checkins.Add(new FourSquareCheckin((Dictionary<string, object>)obj));
        }


        return Checkins;
    }

    /// <summary>
    /// Returns an array of a user's friends. 
    /// </summary>
    /// <param name="USER_ID">Identity of the user to get friends of. Pass self to get friends of the acting user</param>
    public static List<FourSquareUser> UserFriends(string USER_ID, string AccessToken)
    {
        return UserFriends(USER_ID, 0, 0, AccessToken);
    }

    /// <summary>
    /// Returns an array of a user's friends. 
    /// </summary>
    /// <param name="USER_ID">Identity of the user to get friends of. Pass self to get friends of the acting user</param>
    /// <param name="Limit">Number of results to return, up to 500.</param>
    /// <param name="Offset">Used to page through results</param>
    public static List<FourSquareUser> UserFriends(string USER_ID, int Limit, int Offset, string AccessToken)
    {
        List<FourSquareUser> FriendUsers = new List<FourSquareUser>();

        string Query = "";

        if (Limit > 0)
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "Limit=" + Limit.ToString();
        }

        if (Offset > 0)
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "Offset=" + Offset.ToString();
        }

        if (Query.Equals(""))
        {
            Query = "?";
        }
        else
        {
            Query += "&";
        }

        HTTPGet GET = new HTTPGet();
        string EndPoint = "https://api.foursquare.com/v2/users/" + USER_ID + "/friends" + Query + "oauth_token=" + AccessToken;
        GET.Request(EndPoint);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(GET.ResponseBody);

        Dictionary<string, object> ItemsDictionary = ExtractDictionary(JSONDictionary, "response:friends");

        foreach (object obj in (object[])ItemsDictionary["items"])
        {
            FourSquareUser FSU = new FourSquareUser((Dictionary<string, object>)obj);
            FriendUsers.Add(FSU);
        }

        return FriendUsers;
    }

    /// <summary>
    /// Returns tips from a user.  
    /// </summary>
    /// <param name="USER_ID">Identity of the user to get friends of. Pass self to get friends of the acting user</param>
    /// <param name="Sort">One of recent, nearby, or popular. Nearby requires geolat and geolong to be provided.</param>
    /// <param name="LL">Latitude and longitude of the user's location. (Comma separated)</param>
    /// <param name="Limit">Number of results to return, up to 500.</param>
    /// <param name="Offset">Used to page through results</param>
    public static List<FourSquareTip> UserTips(string USER_ID, string Sort, string LL, int Limit, int Offset, string AccessToken)
    {
        List<FourSquareTip> Tips = new List<FourSquareTip>();

        string Query = "";

        if (!Sort.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "sort=" + Sort;
        }

        if (!LL.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "ll=" + LL;
        }

        if (Limit > 0)
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "limit=" + Limit.ToString();
        }

        if (Offset > 0)
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "offset=" + Offset.ToString();
        }

        if (Query.Equals(""))
        {
            Query = "?";
        }
        else
        {
            Query += "&";
        }


        HTTPGet GET = new HTTPGet();
        string EndPoint = "https://api.foursquare.com/v2/users/" + USER_ID + "/tips" + Query + "oauth_token=" + AccessToken;
        GET.Request(EndPoint);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(GET.ResponseBody);
        JSONDictionary = ExtractDictionary(JSONDictionary, "response:tips");
        foreach (object obj in (object[])JSONDictionary["items"])
        {
            FourSquareTip Tip = new FourSquareTip((Dictionary<string, object>)obj);
            Tips.Add(Tip);
        }
        return Tips;
    }

    /// <summary>
    /// Returns todos from a user. 
    /// </summary>
    /// <param name="USER_ID">Identity of the user to get todos for. Pass self to get todos of the acting user.</param>
    /// <param name="Sort">One of recent, nearby, or popular. Nearby requires geolat and geolong to be provided.</param>
    /// <param name="LL">Latitude and longitude of the user's location (Comma separated)</param>
    public static List<FourSquareTodo> UserTodos(string USER_ID, string Sort, string LL, string AccessToken)
    {
        List<FourSquareTodo> ReturnTodos = new List<FourSquareTodo>();

        string Query = "";

        if (!Sort.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "sort=" + Sort;
        }

        if (!LL.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "ll=" + LL;
        }

        if (Query.Equals(""))
        {
            Query = "?";
        }
        else
        {
            Query += "&";
        }


        HTTPGet GET = new HTTPGet();
        string EndPoint = "https://api.foursquare.com/v2/users/" + USER_ID + "/todos" + Query + "oauth_token=" + AccessToken;
        GET.Request(EndPoint);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(GET.ResponseBody);
        JSONDictionary = ExtractDictionary(JSONDictionary, "response:todos");
        foreach (object obj in (object[])JSONDictionary["items"])
        {
            FourSquareTodo Todo = new FourSquareTodo((Dictionary<string, object>)obj);
            ReturnTodos.Add(Todo);
        }

        return ReturnTodos;
    }

    /// <summary>
    /// Returns a list of all venues visited by the specified user, along with how many visits and when they were last there.  
    /// </summary>
    /// <param name="USER_ID">For now, only "self" is supported</param>
    /// <param name="BeforeTimeStamp">Seconds since epoch.</param>
    /// <param name="AfterTimeStamp">Seconds after epoch.</param>
    /// <param name="CategoryID">Limits returned venues to those in this category. If specifying a top-level category, all sub-categories will also match the query.</param>
    public static List<FourSquareVenue> UserVenueHistory(string USER_ID, string BeforeTimeStamp, string AfterTimeStamp, string CategoryID, string AccessToken)
    {
        List<FourSquareVenue> VenueHistory = new List<FourSquareVenue>();

        string Query = "";

        if (!BeforeTimeStamp.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "beforeTimestamp=" + BeforeTimeStamp;
        }

        if (!AfterTimeStamp.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "afterTimestamp=" + AfterTimeStamp;
        }

        if (!CategoryID.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "categoryId=" + CategoryID;
        }

        if (Query.Equals(""))
        {
            Query = "?";
        }
        else
        {
            Query += "&";
        }


        HTTPGet GET = new HTTPGet();
        string EndPoint = "https://api.foursquare.com/v2/users/" + USER_ID + "/venuehistory" + Query + "oauth_token=" + AccessToken;
        GET.Request(EndPoint);
        string json = GET.ResponseBody;
        JavaScriptSerializer JSONDeserializer = new JavaScriptSerializer();
        Dictionary<string, object> DeserializedDictionary = (Dictionary<string, object>)JSONDeserializer.Deserialize(json, typeof(object));
        DeserializedDictionary = ExtractDictionary(DeserializedDictionary, "response:venues");
        foreach (object Obj in (object[])DeserializedDictionary["items"])
        {
            int beenHere = Int32.Parse(((Dictionary<string, object>)Obj)["beenHere"].ToString());
            FourSquareVenue Venue = new FourSquareVenue((Dictionary<string, object>)((Dictionary<string, object>)Obj)["venue"]);
            Venue.beenHere = beenHere;
            VenueHistory.Add(Venue);
        }
        return VenueHistory;
    }

    /// <summary>
    /// Sends a friend request to another user.     
    /// </summary>
    /// <param name="USER_ID">required The user ID to which a request will be sent</param>
    public static FourSquareUser UserRequest(string USER_ID, string AccessToken)
    {
        Dictionary<string, string> Parameters = new Dictionary<string, string>();

        Parameters.Add("callback", "XXX");
        Parameters.Add("oauth_token", AccessToken);

        HTTPPost POST = new HTTPPost(new Uri("https://api.foursquare.com/v2/users/" + USER_ID + "/request"), Parameters);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(POST.ResponseBody);
        JSONDictionary = ExtractDictionary(JSONDictionary, "response:user");
        return new FourSquareUser(JSONDictionary);
    }

    /// <summary>
    /// Cancels any relationship between the acting user and the specified user.    
    /// </summary>
    /// <param name="USER_ID">Identity of the user to unfriend.</param>
    public static FourSquareUser UserUnFriend(string USER_ID, string AccessToken)
    {
        Dictionary<string, string> Parameters = new Dictionary<string, string>();

        Parameters.Add("callback", "XXX");
        Parameters.Add("oauth_token", AccessToken);

        HTTPPost POST = new HTTPPost(new Uri("https://api.foursquare.com/v2/users/" + USER_ID + "/unfriend"), Parameters);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(POST.ResponseBody);
        JSONDictionary = ExtractDictionary(JSONDictionary, "response:user");
        return new FourSquareUser(JSONDictionary);
    }

    /// <summary>
    /// Denies a pending friend request from another user.     
    /// </summary>
    /// <param name="USER_ID">required The user ID of a pending friend.</param>
    public static FourSquareUser UserDeny(string USER_ID, string AccessToken)
    {
        Dictionary<string, string> Parameters = new Dictionary<string, string>();

        Parameters.Add("callback", "XXX");
        Parameters.Add("oauth_token", AccessToken);

        HTTPPost POST = new HTTPPost(new Uri("https://api.foursquare.com/v2/users/" + USER_ID + "/deny"), Parameters);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(POST.ResponseBody);
        JSONDictionary = ExtractDictionary(JSONDictionary, "response:user");
        return new FourSquareUser(JSONDictionary);
    }

    /// <summary>
    /// Approves a pending friend request from another user.   
    /// </summary>
    /// <param name="USER_ID">required The user ID of a pending friend.</param>
    public static FourSquareUser UserApprove(string USER_ID, string AccessToken)
    {
        Dictionary<string, string> Parameters = new Dictionary<string, string>();

        Parameters.Add("callback", "XXX");
        Parameters.Add("oauth_token", AccessToken);

        HTTPPost POST = new HTTPPost(new Uri("https://api.foursquare.com/v2/users/" + USER_ID + "/approve"), Parameters);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(POST.ResponseBody);
        JSONDictionary = ExtractDictionary(JSONDictionary, "response:user");
        return new FourSquareUser(JSONDictionary);
    }

    /// <summary>
    /// Changes whether the acting user will receive pings (phone notifications) when the specified user checks in.  
    /// </summary>
    /// <param name="USER_ID">required The user ID of a friend.</param>
    /// <param name="Value">required True or false.</param>
    public static FourSquareUser UserSetPings(string USER_ID, bool Value, string AccessToken)
    {
        Dictionary<string, string> Parameters = new Dictionary<string, string>();

        Parameters.Add("callback", "XXX");
        Parameters.Add("oauth_token", AccessToken);
        if (Value)
        {
            Parameters.Add("value", "True");
        }
        else
        {
            Parameters.Add("value", "False");
        }
        HTTPPost POST = new HTTPPost(new Uri("https://api.foursquare.com/v2/users/" + USER_ID + "/setpings"), Parameters);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(POST.ResponseBody);
        JSONDictionary = ExtractDictionary(JSONDictionary, "response:user");
        return new FourSquareUser(JSONDictionary);
    }

    #endregion Users

    #region Venues

    /// <summary>
    /// Gives details about a venue, including location, mayorship, tags, tips, specials, and category.
    /// </summary>
    /// <param name="VENUE_ID">required ID of venue to retrieve</param>
    public static FourSquareVenue Venue(string VENUE_ID, string AccessToken)
    {
        HTTPGet GET = new HTTPGet();
        string EndPoint = "https://api.foursquare.com/v2/venues/" + VENUE_ID + "?oauth_token=" + AccessToken;
        GET.Request(EndPoint);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(GET.ResponseBody);
        JSONDictionary = ExtractDictionary(JSONDictionary, "response:venue");
        FourSquareVenue Venue = new FourSquareVenue(JSONDictionary);
        return Venue;
    }

    /// <summary>
    /// Allows users to add a new venue.  
    /// </summary>
    /// <param name="name">required the name of the venue NOTE: One of either a valid address or a geolat/geolong pair must be provided</param>
    /// <param name="address">The address of the venue.</param>
    /// <param name="crossStreet">The nearest intersecting street or streets.</param>
    /// <param name="city">The city name where this venue is.</param>
    /// <param name="state">The nearest state or province to the venue.</param>
    /// <param name="zip">The zip or postal code for the venue.</param>
    /// <param name="phone">The phone number of the venue.</param>
    /// <param name="ll">required Latitude and longitude of the venue, as accurate as is known. NOTE: One of either a valid address or a geolat/geolong pair must be provided</param>
    /// <param name="primaryCategoryId">The ID of the category to which you want to assign this venue.</param>
    public static FourSquareVenue VenueAdd(string name, string address, string crossStreet, string city, string state, string zip, string phone, string ll, string primaryCategoryId, string AccessToken)
    {
        Dictionary<string, string> Parameters = new Dictionary<string, string>();

        Parameters.Add("callback", "XXX");
        Parameters.Add("oauth_token", AccessToken);

        #region Parameter Conditioning

        //address
        if (!address.Equals(""))
        {
            Parameters.Add("address", address);
        }

        //city
        if (!city.Equals(""))
        {
            Parameters.Add("city", city);
        }

        //crossStreet
        if (!crossStreet.Equals(""))
        {
            Parameters.Add("crossStreet", crossStreet);
        }

        //ll
        if (!ll.Equals(""))
        {
            Parameters.Add("ll", ll);
        }

        //name
        if (!name.Equals(""))
        {
            Parameters.Add("name", name);
        }

        //phone
        if (!phone.Equals(""))
        {
            Parameters.Add("phone", phone);
        }

        //primaryCategoryId
        if (!primaryCategoryId.Equals(""))
        {
            Parameters.Add("primaryCategoryId", primaryCategoryId);
        }

        //state
        if (!state.Equals(""))
        {
            Parameters.Add("state", state);
        }

        //zip
        if (!zip.Equals(""))
        {
            Parameters.Add("zip", zip);
        }

        #endregion Parameter Conditioning

        HTTPPost POST = new HTTPPost(new Uri("https://api.foursquare.com/v2/venues/add"), Parameters);

        string SampleReply = "{\"meta\":{\"code\":200},\"response\":{\"venue\":{\"id\":\"4e12000a1f6e671dd9ec9341\",\"name\":\"McLean Park Playground\",\"contact\":{},\"location\":{\"city\":\"Richmond\",\"state\":\"BC\",\"lat\":49.17213,\"lng\":-122.973382,\"distance\":0},\"categories\":[{\"id\":\"4bf58dd8d48988d1e7941735\",\"name\":\"Playground\",\"pluralName\":\"Playgrounds\",\"icon\":\"https://foursquare.com/img/categories/parks_outdoors/playground.png\",\"parents\":[\"Great Outdoors\"],\"primary\":true}],\"verified\":false,\"stats\":{\"checkinsCount\":0,\"usersCount\":0},\"hereNow\":{\"count\":0,\"groups\":[{\"type\":\"friends\",\"name\":\"friends here\",\"count\":0,\"items\":[]},{\"type\":\"others\",\"name\":\"other people here\",\"count\":0,\"items\":[]}]},\"mayor\":{\"count\":0},\"tips\":{\"count\":0,\"groups\":[]},\"tags\":[],\"specials\":[],\"specialsNearby\":[],\"shortUrl\":\"http://4sq.com/mMoxBz\",\"timeZone\":\"America/Vancouver\",\"beenHere\":{\"count\":0},\"photos\":{\"count\":0,\"groups\":[{\"type\":\"checkin\",\"name\":\"friends' checkin photos\",\"count\":0,\"items\":[]},{\"type\":\"venue\",\"name\":\"venue photos\",\"count\":0,\"items\":[]}]},\"todos\":{\"count\":0,\"items\":[]}}}}";
        Dictionary<string, object> JSONDictionary = JSONDeserializer(POST.ResponseBody);
        JSONDictionary = ExtractDictionary(JSONDictionary, "response:venue");
        return new FourSquareVenue(JSONDictionary);
    }

    /// <summary>
    /// Returns a hierarchical list of categories applied to venues. By default, top-level categories do not have IDs. 
    /// </summary>
    public static List<FourSquareVenueCategory> VenueCategories(string AccessToken)
    {
        HTTPGet GET = new HTTPGet();
        List<FourSquareVenueCategory> VenueCategories = new List<FourSquareVenueCategory>();
        string EndPoint = "https://api.foursquare.com/v2/venues/categories" + "?oauth_token=" + AccessToken;
        GET.Request(EndPoint);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(GET.ResponseBody);
        foreach (object Obj in (object[])ExtractDictionary(JSONDictionary, "response")["categories"])
        {
            VenueCategories.Add(new FourSquareVenueCategory((Dictionary<string, object>)Obj));
        }
        return VenueCategories;
    }

    /// <summary>
    /// Returns a list of recommended venues near the current location. 
    /// </summary>
    /// <param name="ll">required Latitude and longitude of the user's location, so response can include distance.</param>
    /// <param name="llAcc">Accuracy of latitude and longitude, in meters.</param>
    /// <param name="alt">Altitude of the user's location, in meters.</param>
    /// <param name="altAcc">Accuracy of the user's altitude, in meters.</param>
    /// <param name="radius">Radius to search within, in meters.</param>
    /// <param name="section">One of food, drinks, coffee, shops, or arts.</param>
    /// <param name="query">A search term to be applied against tips, category, tips, etc. at a venue.</param>
    /// <param name="limit">Number of results to return, up to 50.</param>
    /// <param name="basis">If present and set to friends or me, limits results to only places where friends have visited or user has visited, respectively.</param>
    public static FourSquareRecommendedVenues VenueExplore(string ll, string llAcc, string alt, string altAcc, string radius, string section, string query, string limit, string basis, string AccessToken)
    {
        HTTPGet GET = new HTTPGet();
        string Query = "";



        //ll
        if (!ll.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "ll=" + ll;
        }

        //llAcc
        if (!llAcc.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "llAcc=" + llAcc;
        }

        //alt
        if (!alt.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "alt=" + alt;
        }

        //altAcc
        if (!altAcc.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "altAcc=" + altAcc;
        }

        //radius
        if (!radius.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "radius=" + radius;
        }

        //section
        if (!section.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "section=" + section;
        }

        //query
        if (!query.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "query=" + query;
        }

        //limit
        if (!limit.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "limit=" + limit;
        }

        //basis
        if (!basis.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "basis=" + basis;
        }
        string EndPoint = "https://api.foursquare.com/v2/venues/explore" + Query + "&oauth_token=" + AccessToken;
        GET.Request(EndPoint);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(GET.ResponseBody);
        JSONDictionary = ExtractDictionary(JSONDictionary, "response");

        FourSquareRecommendedVenues RecommendedVenues = new FourSquareRecommendedVenues(JSONDictionary);

        return RecommendedVenues;
    }

    /// <summary>
    /// Returns a list of venues near the current location, optionally matching the search term.  
    /// </summary>
    /// <param name="ll">required Latitude and longitude of the user's location, so response can include distance.</param>
    /// <param name="llAcc">Accuracy of latitude and longitude, in meters.</param>
    /// <param name="alt">Altitude of the user's location, in meters.</param>
    /// <param name="altAcc">Accuracy of the user's altitude, in meters.</param>
    /// <param name="query">A search term to be applied against titles.</param>
    /// <param name="limit">Number of results to return, up to 50.</param>
    /// <param name="intent">Indicates your intent in performing the search. checkin, match, specials</param>
    /// <param name="categoryId">A category to limit results to. </param>
    /// <param name="url">A third-party URL which we will attempt to match against our map of venues to URLs.</param>
    /// <param name="providerId">Identifier for a known third party that is part of our map of venues to URLs, used in conjunction with linkedId</param>
    /// <param name="linkedId">Identifier used by third party specifed in providerId, which we will attempt to match against our map of venues to URLs.</param>
    public static Dictionary<string, List<FourSquareVenue>> VenueSearch(string ll, string llAcc, string alt, string altAcc, string query, string limit, string intent, string categoryId, string url, string providerId, string linkedId, string AccessToken)
    {
        HTTPGet GET = new HTTPGet();
        string Query = "";

        Dictionary<string, List<FourSquareVenue>> VenueSearchResults = new Dictionary<string, List<FourSquareVenue>>();

        #region Query Conditioning

        //ll
        if (!ll.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "ll=" + ll;
        }

        //llAcc
        if (!llAcc.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "llAcc=" + llAcc;
        }

        //alt
        if (!alt.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "alt=" + alt;
        }

        //altAcc
        if (!altAcc.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "altAcc=" + altAcc;
        }

        //query
        if (!query.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "query=" + query;
        }

        //limit
        if (!limit.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "limit=" + limit;
        }

        //intent
        if (!intent.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "intent=" + intent;
        }

        //categoryId
        if (!categoryId.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "categoryId=" + categoryId;
        }

        //url
        if (!url.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "url=" + url;
        }

        //providerId
        if (!providerId.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "providerId=" + providerId;
        }

        //linkedId
        if (!linkedId.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "linkedId=" + linkedId;
        }

        #endregion Query Conditioning

        string EndPoint = "https://api.foursquare.com/v2/venues/search" + Query + "&oauth_token=" + AccessToken;
        GET.Request(EndPoint);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(GET.ResponseBody);
        foreach (object GroupObj in (object[])ExtractDictionary(JSONDictionary, "response")["groups"])
        {
            string GroupType = ((Dictionary<string, object>)GroupObj)["type"].ToString();
            List<FourSquareVenue> Venues = new List<FourSquareVenue>();
            foreach (object VenueObj in (object[])((Dictionary<string, object>)GroupObj)["items"])
            {
                Venues.Add(new FourSquareVenue((Dictionary<string, object>)VenueObj));
            }
            VenueSearchResults.Add(GroupType, Venues);
        }

        return VenueSearchResults;
    }

    /// <summary>
    /// Returns a list of venues near the current location with the most people currently checked in.   
    /// </summary>
    /// <param name="ll">required Latitude and longitude of the user's location.</param>
    /// <param name="limit">Number of results to return, up to 50.</param>
    /// <param name="radius">Radius in meters, up to approximately 2000 meters.</param>
    public static List<FourSquareVenue> VenueTrending(string ll, string limit, string radius, string AccessToken)
    {
        List<FourSquareVenue> TrendingVenues = new List<FourSquareVenue>();
        HTTPGet GET = new HTTPGet();
        string Query = "";

        #region Query Conditioning

        //ll
        if (!ll.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "ll=" + ll;
        }

        //limit
        if (!limit.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "limit=" + limit;
        }

        //radius
        if (!radius.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "radius=" + radius;
        }

        #endregion Query Conditioning

        string EndPoint = "https://api.foursquare.com/v2/venues/trending" + Query + "&oauth_token=" + AccessToken;
        GET.Request(EndPoint);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(GET.ResponseBody);
        foreach (object VenueObj in (object[])ExtractDictionary(JSONDictionary, "response")["venues"])
        {
            TrendingVenues.Add(new FourSquareVenue((Dictionary<string, object>)VenueObj));
        }
        return TrendingVenues;
    }

    /// <summary>
    /// Provides a count of how many people are at a given venue. If the request is user authenticated, also returns a list of the users there, friends-first.    
    /// </summary>
    /// <param name="VENUE_ID">required ID of venue to retrieve</param>
    /// <param name="limit">Number of results to return, up to 500.</param>
    /// <param name="offset">Used to page through results.</param>
    /// <param name="afterTimestamp">Retrieve the first results to follow these seconds since epoch</param>
    public static List<FourSquareCheckin> VenueHereNow(string VENUE_ID, string limit, string offset, string afterTimestamp, string AccessToken)
    {
        List<FourSquareCheckin> HereNowCheckins = new List<FourSquareCheckin>();
        HTTPGet GET = new HTTPGet();
        string Query = "";

        #region Query Conditioning

        //limit
        if (!limit.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "limit=" + limit;
        }

        //offset
        if (!offset.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "offset=" + offset;
        }

        //afterTimestamp
        if (!afterTimestamp.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "afterTimestamp=" + afterTimestamp;
        }

        #endregion Query Conditioning

        string EndPoint = "https://api.foursquare.com/v2/venues/" + VENUE_ID + "/herenow" + Query + "&oauth_token=" + AccessToken;
        GET.Request(EndPoint);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(GET.ResponseBody);
        foreach (object CheckinObj in (object[])ExtractDictionary(JSONDictionary, "response:hereNow")["items"])
        {
            HereNowCheckins.Add(new FourSquareCheckin((Dictionary<string, object>)CheckinObj));
        }
        return HereNowCheckins;
    }

    /// <summary>
    /// Returns tips for a venue.     
    /// </summary>
    /// <param name="VENUE_ID">required The venue you want tips for.</param>
    /// <param name="sort">One of recent or popular.</param>
    /// <param name="limit">Number of results to return, up to 500</param>
    /// <param name="offset">Used to page through results.</param>
    public static List<FourSquareTip> VenueTips(string VENUE_ID, string sort, string limit, string offset, string AccessToken)
    {
        List<FourSquareTip> Tips = new List<FourSquareTip>();
        HTTPGet GET = new HTTPGet();
        string Query = "";

        #region Query Conditioning

        //sort
        if (!sort.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "sort=" + sort;
        }

        //limit
        if (!limit.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "limit=" + limit;
        }

        //offset
        if (!offset.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "offset=" + offset;
        }

        if (Query.Equals(""))
        {
            Query = "?";
        }
        else
        {
            Query += "&";
        }

        #endregion Query Conditioning

        string EndPoint = "https://api.foursquare.com/v2/venues/" + VENUE_ID + "/tips" + Query + "oauth_token=" + AccessToken;
        GET.Request(EndPoint);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(GET.ResponseBody);
        foreach (object TipObj in (object[])ExtractDictionary(JSONDictionary, "response:tips")["items"])
        {
            Tips.Add(new FourSquareTip((Dictionary<string, object>)TipObj));
        }
        return Tips;
    }

    /// <summary>
    /// Returns photos for a venue    
    /// </summary>
    /// <param name="VENUE_ID">required The venue you want photos for.</param>
    /// <param name="group">required. Pass checkin for photos added by friends on their recent checkins. Pass venue for public photos added to the venue by anyone. Use multi to fetch both.</param>
    /// <param name="limit">Number of results to return, up to 500</param>
    /// <param name="offset">Used to page through results.</param>
    public static List<FourSquarePhoto> VenuePhotos(string VENUE_ID, string group, string limit, string offset, string AccessToken)
    {
        List<FourSquarePhoto> Photos = new List<FourSquarePhoto>();
        HTTPGet GET = new HTTPGet();
        string Query = "";

        #region Query Conditioning

        //group
        if (!group.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "group=" + group;
        }

        //limit
        if (!limit.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "limit=" + limit;
        }

        //offset
        if (!offset.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "offset=" + offset;
        }

        if (Query.Equals(""))
        {
            Query = "?";
        }
        else
        {
            Query += "&";
        }

        #endregion Query Conditioning

        string EndPoint = "https://api.foursquare.com/v2/venues/" + VENUE_ID + "/photos" + Query + "oauth_token=" + AccessToken;
        GET.Request(EndPoint);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(GET.ResponseBody);
        foreach (object PhotoObj in (object[])ExtractDictionary(JSONDictionary, "response:photos")["items"])
        {
            Photos.Add(new FourSquarePhoto((Dictionary<string, object>)PhotoObj));
        }
        return Photos;
    }

    /// <summary>
    ///Returns URLs or identifiers from third parties that have been applied to this venue, such as how the New York Times refers to this venue and a URL for additional information from nytimes.com.    
    /// </summary>
    /// <param name="VENUE_ID">required The venue you want annotations for..</param>
    public static void VenueLinks(string VENUE_ID, string AccessToken)
    {
        List<FourSquarePhoto> Photos = new List<FourSquarePhoto>();
        HTTPGet GET = new HTTPGet();

        string EndPoint = "https://api.foursquare.com/v2/venues/" + VENUE_ID + "/links?oauth_token=" + AccessToken;
        GET.Request(EndPoint);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(GET.ResponseBody);

        throw new Exception("Find a Venue with Links, to complete");
        //todo Find a Venue with Links, to complete
    }

    /// <summary>
    /// Allows you to mark a venue to-do, with optional text.     
    /// </summary>
    /// <param name="VENUE_ID">required The venue you want to mark to-do.</param>
    /// <param name="text">The text of the tip.</param>
    public static FourSquareTodo VenueMarkTodo(string VENUE_ID, string text, string AccessToken)
    {
        Dictionary<string, string> Parameters = new Dictionary<string, string>();

        Parameters.Add("callback", "XXX");
        Parameters.Add("oauth_token", AccessToken);

        #region Parameter Conditioning

        //text
        if (!text.Equals(""))
        {
            Parameters.Add("text", text);
        }

        #endregion Parameter Conditioning

        HTTPPost POST = new HTTPPost(new Uri("https://api.foursquare.com/v2/venues/" + VENUE_ID + "/marktodo"), Parameters);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(POST.ResponseBody);
        JSONDictionary = ExtractDictionary(JSONDictionary, "response:todo");
        return new FourSquareTodo(JSONDictionary);
    }

    /// <summary>
    /// Allows users to indicate a venue is incorrect in some way.      
    /// </summary>
    /// <param name="VENUE_ID">required The venue id for which an edit is being proposed.</param>
    /// <param name="problem">required One of mislocated, closed, duplicate.</param>
    public static void VenueFlag(string VENUE_ID, string problem, string AccessToken)
    {
        Dictionary<string, string> Parameters = new Dictionary<string, string>();

        Parameters.Add("callback", "XXX");
        Parameters.Add("oauth_token", AccessToken);

        #region Parameter Conditioning

        //problem
        if (!problem.Equals(""))
        {
            Parameters.Add("problem", problem);
        }

        #endregion Parameter Conditioning

        HTTPPost POST = new HTTPPost(new Uri("https://api.foursquare.com/v2/venues/" + VENUE_ID + "/marktodo"), Parameters);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(POST.ResponseBody);
        //todo: Test response, return something.
    }

    /// <summary>
    /// Allows you to make a change to a venue. Requires Superuser privileges     
    /// </summary>
    /// <param name="VENUE_ID">required The venue id for which an edit is being proposed</param>
    /// <param name="name">The name of the venue.</param>
    /// <param name="address">The address of the venue.</param>
    /// <param name="crossStreet">The nearest intersecting street or streets</param>
    /// <param name="city">The city name where this venue is.</param>
    /// <param name="state">The nearest state or province to the venue.</param>
    /// <param name="zip">The zip or postal code for the venue.</param>
    /// <param name="phone">The phone number of the venue.</param>
    /// <param name="ll">Latitude and longitude of the user's location, as accurate as is known.</param>
    /// <param name="primaryCategoryId">The ID of the category to which you want to assign this venue.</param>
    public static void VenueEdit(string VENUE_ID, string name, string address, string crossStreet, string city, string state, string zip, string phone, string ll, string primaryCategoryId, string AccessToken)
    {
        VenueEditor("edit", VENUE_ID, name, address, crossStreet, city, state, zip, phone, ll, primaryCategoryId, AccessToken);
    }

    /// <summary>
    /// Allows you to propose a change to a venue.      
    /// </summary>
    /// <param name="VENUE_ID">required The venue id for which an edit is being proposed</param>
    /// <param name="name">The name of the venue.</param>
    /// <param name="address">The address of the venue.</param>
    /// <param name="crossStreet">The nearest intersecting street or streets</param>
    /// <param name="city">The city name where this venue is.</param>
    /// <param name="state">The nearest state or province to the venue.</param>
    /// <param name="zip">The zip or postal code for the venue.</param>
    /// <param name="phone">The phone number of the venue.</param>
    /// <param name="ll">Latitude and longitude of the user's location, as accurate as is known.</param>
    /// <param name="primaryCategoryId">The ID of the category to which you want to assign this venue.</param>
    public static void VenueProposeEdit(string VENUE_ID, string name, string address, string crossStreet, string city, string state, string zip, string phone, string ll, string primaryCategoryId, string AccessToken)
    {
        VenueEditor("proposeedit", VENUE_ID, name, address, crossStreet, city, state, zip, phone, ll, primaryCategoryId, AccessToken);
    }

    /// <summary>
    /// Allows you to propose or make a change to a venue.      
    /// </summary>
    /// <param name="VENUE_ID">required The venue id for which an edit is being proposed</param>
    /// <param name="EditType">either edit or proposeedit</param>
    /// <param name="name">The name of the venue.</param>
    /// <param name="address">The address of the venue.</param>
    /// <param name="crossStreet">The nearest intersecting street or streets</param>
    /// <param name="city">The city name where this venue is.</param>
    /// <param name="state">The nearest state or province to the venue.</param>
    /// <param name="zip">The zip or postal code for the venue.</param>
    /// <param name="phone">The phone number of the venue.</param>
    /// <param name="ll">Latitude and longitude of the user's location, as accurate as is known.</param>
    /// <param name="primaryCategoryId">The ID of the category to which you want to assign this venue.</param>
    private static void VenueEditor(string EditType, string VENUE_ID, string name, string address, string crossStreet, string city, string state, string zip, string phone, string ll, string primaryCategoryId, string AccessToken)
    {
        //Venue Edit and Venue ProposeEdit are essentially the same call. Edit requires Superuser privileges.

        Dictionary<string, string> Parameters = new Dictionary<string, string>();


        Parameters.Add("callback", "XXX");
        #region Parameter Conditioning

        //address
        if (!address.Equals(""))
        {
            Parameters.Add("address", address);
        }

        //city
        if (!city.Equals(""))
        {
            Parameters.Add("city", city);
        }

        //crossStreet
        if (!crossStreet.Equals(""))
        {
            Parameters.Add("crossStreet", crossStreet);
        }

        //ll
        if (!ll.Equals(""))
        {
            Parameters.Add("ll", ll);
        }

        //name
        if (!name.Equals(""))
        {
            Parameters.Add("name", name);
        }


        //phone
        if (!phone.Equals(""))
        {
            Parameters.Add("phone", phone);
        }

        //primaryCategoryId
        if (!primaryCategoryId.Equals(""))
        {
            Parameters.Add("primaryCategoryId", primaryCategoryId);
        }

        //state
        if (!state.Equals(""))
        {
            Parameters.Add("state", state);
        }

        //zip
        if (!zip.Equals(""))
        {
            Parameters.Add("zip", zip);
        }

        Parameters.Add("v", "20110704");

        Parameters.Add("oauth_token", AccessToken);

        #endregion Parameter Conditioning

        HTTPPost POST = new HTTPPost(new Uri("https://api.foursquare.com/v2/venues/" + VENUE_ID + "/" + EditType), Parameters);
        //todo: Interpret response.
    }

    #endregion Venues

    #region Checkins

    /// <summary>
    /// Retrieves information on a specific checkin.
    /// </summary>
    /// <param name="CHECKIN_ID">The ID of the checkin to retrieve specific information for.</param>
    /// <param name="signature">When checkins are sent to public feeds such as Twitter, foursquare appends a signature (s=XXXXXX) allowing users to bypass the friends-only access check on checkins. The same value can be used here for programmatic access to otherwise inaccessible checkins. Callers should use the bit.ly API to first expand 4sq.com links.</param>
    public static FourSquareCheckin CheckinDetails(string CHECKIN_ID, string signature, string AccessToken)
    {
        string Query = "";

        if (!signature.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "signature=" + signature;
        }
        if (Query.Equals(""))
        {
            Query = "?";
        }
        else
        {
            Query += "&";
        }
        FourSquareCheckin Checkin = new FourSquareCheckin();
        HTTPGet GET = new HTTPGet();
        string EndPoint = "https://api.foursquare.com/v2/checkins/" + CHECKIN_ID + Query + "oauth_token=" + AccessToken;
        GET.Request(EndPoint);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(GET.ResponseBody);
        JSONDictionary = ExtractDictionary(JSONDictionary, "response:checkin");
        Checkin = new FourSquareCheckin(JSONDictionary);
        return Checkin;
    }

    /// <summary>
    /// Retrieves information on a specific checkin.
    /// </summary>
    /// <param name="CHECKIN_ID">The ID of the checkin to retrieve specific information for.</param>
    public static FourSquareCheckin CheckinDetails(string CHECKIN_ID, string AccessToken)
    {
        FourSquareCheckin Checkin = new FourSquareCheckin();
        HTTPGet GET = new HTTPGet();
        string EndPoint = "https://api.foursquare.com/v2/checkins/" + CHECKIN_ID + "?oauth_token=" + AccessToken;
        GET.Request(EndPoint);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(GET.ResponseBody);
        JSONDictionary = ExtractDictionary(JSONDictionary, "response:checkin");
        Checkin = new FourSquareCheckin(JSONDictionary);
        return Checkin;
    }

    /// <summary>
    /// Allows you to check in to a place.
    /// </summary>
    /// <param name="venueId">The venue where the user is checking in. No venueid is needed if shouting or just providing a venue name.</param>
    /// <param name="Broadcast">Required. How much to broadcast this check-in, ranging from private (off-the-grid) to public,facebook,twitter. Can also be just public or public,facebook, for example. If no valid value is found, the default is public. Shouts cannot be private.</param>
    /// <param name="LL">Latitude and longitude of the user's location. Only specify this field if you have a GPS or other device reported location for the user at the time of check-in.</param>
    public static FourSquareCheckin CheckinAdd(string VenueId, string Broadcast, string LL, string AccessToken)
    {
        return CheckinAdd(VenueId, "", "", Broadcast, LL, "1", "0", "1", AccessToken);
    }

    /// <summary>
    /// Allows you to check in to a place.
    /// </summary>
    /// <param name="venueId">The venue where the user is checking in. No venueid is needed if shouting or just providing a venue name.</param>
    /// <param name="venue">If are not shouting, but you don't have a venue ID or would rather prefer a 'venueless' checkin</param>
    /// <param name="shout">A message about your check-in. The maximum length of this field is 140 characters.</param>
    /// <param name="Broadcast">Required. How much to broadcast this check-in, ranging from private (off-the-grid) to public,facebook,twitter. Can also be just public or public,facebook, for example. If no valid value is found, the default is public. Shouts cannot be private.</param>
    /// <param name="LL">Latitude and longitude of the user's location. Only specify this field if you have a GPS or other device reported location for the user at the time of check-in.</param>
    public static FourSquareCheckin CheckinAdd(string VenueId, string Venue, string Shout, string Broadcast, string LL, string LLAcc, string Alt, string AltAcc, string AccessToken)
    {
        FourSquareCheckin CheckinResponse = new FourSquareCheckin();

        Dictionary<string, string> parameters = new Dictionary<string, string>();

        if (!Alt.Equals(""))
        {
            parameters.Add("alt", Alt);
        }
        if (!AltAcc.Equals(""))
        {
            parameters.Add("altAcc", AltAcc);
        }
        if (!Broadcast.Equals(""))
        {
            parameters.Add("broadcast", Broadcast);
        }
        if (!LL.Equals(""))
        {
            parameters.Add("ll", LL);
        }
        if (!LLAcc.Equals(""))
        {
            parameters.Add("llAcc", LLAcc);
        }
        if (!Shout.Equals(""))
        {
            parameters.Add("shout", Shout);
        }
        if (!Venue.Equals(""))
        {
            parameters.Add("venue", Venue);
        }
        if (!VenueId.Equals(""))
        {
            parameters.Add("venueId", VenueId);
        }

        parameters.Add("oauth_token", AccessToken);

        HTTPPost POST = new HTTPPost(new Uri("https://api.foursquare.com/v2/checkins/add"), parameters);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(POST.ResponseBody);

        CheckinResponse = new FourSquareCheckin(ExtractDictionary(JSONDictionary, "response:checkin"));

        foreach (object Obj in (object[])JSONDictionary["notifications"])
        {
            FourSquareNotification Notification = new FourSquareNotification((Dictionary<string, object>)Obj);
            CheckinResponse.notifications.Add(Notification);
        }
        return CheckinResponse;
    }

    /// <summary>
    /// Recent checkins by friends 
    /// </summary>
    /// <param name="ll">Latitude and longitude of the user's location, so response can include distance. "44.3,37.2"</param>
    /// <param name="limit">Number of results to return, up to 100.</param>
    /// <param name="afterTimestamp">Seconds after which to look for checkins</param>
    public static List<FourSquareCheckin> CheckinRecent(string LL, string Limit, string AfterTimestamp, string AccessToken)
    {
        List<FourSquareCheckin> Checkins = new List<FourSquareCheckin>();

        string Query = "";

        if (!LL.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "ll=" + LL;
        }

        if (!Limit.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "limit=" + Limit;
        }

        if (!AfterTimestamp.Equals(""))
        {
            if (Query.Equals(""))
            {
                Query = "?";
            }
            else
            {
                Query += "&";
            }
            Query += "afterTimestamp=" + AfterTimestamp;
        }

        if (Query.Equals(""))
        {
            Query = "?";
        }
        else
        {
            Query += "&";
        }

        HTTPGet GET = new HTTPGet();
        string EndPoint = "https://api.foursquare.com/v2/checkins/recent" + Query + "oauth_token=" + AccessToken;
        GET.Request(EndPoint);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(GET.ResponseBody);
        JSONDictionary = ExtractDictionary(JSONDictionary, "response");
        foreach (object obj in (object[])JSONDictionary["recent"])
        {
            FourSquareCheckin RecentCheckin = new FourSquareCheckin((Dictionary<string, object>)obj);
            Checkins.Add(RecentCheckin);
        }
        return Checkins;
    }

    /// <summary>
    /// Add a comment to a check-in  
    /// </summary>
    /// <param name="CHECKIN_ID">The ID of the checkin to add a comment to.</param>
    /// <param name="text">The text of the comment, up to 200 characters.</param>
    public static FourSquareComment CheckinAddComment(string CHECKIN_ID, string Text, string AccessToken)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();

        parameters.Add("callback", "XXX");
        parameters.Add("oauth_token", AccessToken);
        parameters.Add("text", Text);

        HTTPPost POST = new HTTPPost(new Uri("https://api.foursquare.com/v2/checkins/" + CHECKIN_ID + "/addcomment"), parameters);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(POST.ResponseBody);
        JSONDictionary = ExtractDictionary(JSONDictionary, "response:comment");
        FourSquareComment Comment = new FourSquareComment(JSONDictionary);
        return Comment;
    }


    /// <summary>
    /// Remove commment from check-in   
    /// </summary>
    /// <param name="CHECKIN_ID">The ID of the checkin to remove a comment from.</param>
    /// <param name="commentId">The id of the comment to remove.</param>
    public static FourSquareCheckin CheckinDeleteComment(string CHECKIN_ID, string commentId, string AccessToken)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();

        parameters.Add("callback", "XXX");
        parameters.Add("commentId", commentId);
        parameters.Add("oauth_token", AccessToken);


        HTTPPost POST = new HTTPPost(new Uri("https://api.foursquare.com/v2/checkins/" + CHECKIN_ID + "/deletecomment"), parameters);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(POST.ResponseBody);
        JSONDictionary = ExtractDictionary(JSONDictionary, "response:checkin");
        return new FourSquareCheckin(JSONDictionary);
    }

    #endregion Checkins

    #region Tips

    public static void Tip()
    {
        //Todo
    }

    public static void TipAdd()
    {
        //Todo
    }

    public static void TipSearch()
    {
        //Todo
    }

    public static void TipMarkTodo()
    {
        //Todo
    }

    public static void TipMarkDone()
    {
        //Todo
    }

    public static void TipUnMark()
    {
        //Todo
    }

    #endregion Tips

    #region Photos

    public static void Photo()
    {
        //Todo
    }

    public static void PhotoAdd()
    {
        //Todo
    }

    #endregion Photos

    #region Settings

    /// <summary>
    /// Returns all settings for the acting user.   
    /// </summary>
    public static Dictionary<string, Object> Settings(string AccessToken)
    {
        return Settings(FourSquareSettingOptions.all, AccessToken);
    }

    /// <summary>
    /// Returns a setting for the acting user.   
    /// </summary>
    /// <param name="Setting">The name of a setting</param>
    public static Dictionary<string, Object> Settings(FourSquareSettingOptions Setting, string AccessToken)
    {
        Dictionary<string, Object> SettingDictionary = new Dictionary<string, Object>();

        HTTPGet GET = new HTTPGet();
        string EndPoint = "https://api.foursquare.com/v2/settings/" + Setting + "?oauth_token=" + AccessToken;
        GET.Request(EndPoint);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(GET.ResponseBody);
        JSONDictionary = ExtractDictionary(JSONDictionary, "response:settings");
        foreach (object Obj in JSONDictionary)
        {
            SettingDictionary.Add((((KeyValuePair<string, object>)Obj)).Key, (((KeyValuePair<string, object>)Obj)).Value);
        }
        return SettingDictionary;
    }

    /// <summary>
    /// Change a setting for the given user.    
    /// </summary>
    /// <param name="Setting">The name of a setting</param>
    /// <param name="value">required 1 for true, and 0 for false.</param>
    public static Dictionary<string, Object> Settings(FourSquareSettingOptions Setting, bool Value, string AccessToken)
    {
        Dictionary<string, Object> SettingDictionary = new Dictionary<string, Object>();

        if (!(Setting == FourSquareSettingOptions.all))
        {
            string StrValue = "0";
            if (Value)
            {
                StrValue = "1";
            }
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            parameters.Add("callback", "XXX");
            parameters.Add("oauth_token", AccessToken);
            parameters.Add("value", StrValue);

            HTTPPost POST = new HTTPPost(new Uri("https://api.foursquare.com/v2/settings/" + Setting + "/set"), parameters);
            Dictionary<string, object> JSONDictionary = JSONDeserializer(POST.ResponseBody);
            JSONDictionary = ExtractDictionary(JSONDictionary, "response:settings");
            foreach (object Obj in JSONDictionary)
            {
                SettingDictionary.Add((((KeyValuePair<string, object>)Obj)).Key, (((KeyValuePair<string, object>)Obj)).Value);
            }
        }
        return SettingDictionary;
    }

    #endregion Settings

    #region Specials

    /// <summary>
    /// Gives details about a special, including text and whether it is unlocked for the current user. 
    /// </summary>
    /// <param name="SPECIAL_ID">required ID of special to retrieve</param>
    /// <param name="venueId">required ID of a venue the special is running at</param>
    public static void Special(string SPECIAL_ID, string venueId)
    {
        HTTPGet GET = new HTTPGet();
        string EndPoint = "https://api.foursquare.com/v2/specials/" + SPECIAL_ID + "?venueId=" + venueId;
        GET.Request(EndPoint);
        Dictionary<string, object> JSONDictionary = JSONDeserializer(GET.ResponseBody);
        //JSONDictionary = ExtractDictionary(JSONDictionary, "response:user");
        //FourSquareUser User = new FourSquareUser(JSONDictionary);
        //return User;
        throw new Exception("Todo");
    }

    public static void SpecialSearch()
    {
        //Todo
    }

    #endregion Specials

    #region FourSquare Classes

    public enum FourSquareSettingOptions { all, sendToTwitter, sendMayorshipsToTwitter, sendBadgesToTwitter, sendToFacebook, sendMayorshipsToFacebook, sendBadgesToFacebook, receivePings, receiveCommentPings };

    public class FourSquareBadge
    {
        public string id = "";
        public string badgeID = "";
        public string name = "";
        public string description = "";
        public string hint = "";
        public FourSquareImage image;
        public List<FourSquareCheckin> unlocks = new List<FourSquareCheckin>();
        private string JSON = "";

        public FourSquareBadge(Dictionary<string, object> JSONDictionary)
        {
            JSON = JSONSerializer(JSONDictionary);
            id = JSONDictionary["id"].ToString();
            if (JSONDictionary.ContainsKey("badgeID"))
            {
                badgeID = JSONDictionary["badgeID"].ToString();
            }
            else
            {
                badgeID = id;
            }
            name = JSONDictionary["name"].ToString();
            if (JSONDictionary.ContainsKey("description"))
            {
                description = JSONDictionary["description"].ToString();
            }
            if (JSONDictionary.ContainsKey("hint"))
            {
                hint = JSONDictionary["hint"].ToString();
            }
            image = new FourSquareImage(((Dictionary<string, object>)JSONDictionary["image"]));
            foreach (object Obj in (object[])ExtractDictionary(JSONDictionary, "response")["unlocks"])
            {
                Dictionary<string, object> UnlockCheckin = (Dictionary<string, object>)((object[])((Dictionary<string, object>)Obj)["checkins"])[0];
                unlocks.Add(new FourSquareCheckin(UnlockCheckin));
            }
        }
    }

    public class FourSquareBadgeSet
    {
        public string type = "";
        public string name = "";
        public FourSquareImage image;
        public List<string> items = new List<string>();
        public List<FourSquareBadgeSet> groups = new List<FourSquareBadgeSet>();
        private string JSON = "";

        public FourSquareBadgeSet(Dictionary<string, object> JSONDictionary)
        {
            JSON = JSONSerializer(JSONDictionary);
            type = JSONDictionary["type"].ToString();
            name = JSONDictionary["name"].ToString();
            image = new FourSquareImage((Dictionary<string, object>)JSONDictionary["image"]);

            foreach (object Obj in (object[])JSONDictionary["items"])
            {
                items.Add((string)Obj);
            }

            foreach (object Obj in (object[])JSONDictionary["groups"])
            {
                groups.Add(new FourSquareBadgeSet((Dictionary<string, object>)Obj));
            }
        }
    }

    public class FourSquareBadgesAndSets
    {
        public string defaultSetType = "";
        public List<FourSquareBadgeSet> BadgeSets = new List<FourSquareBadgeSet>();
        public List<FourSquareBadge> Badges = new List<FourSquareBadge>();
    }

    public class FourSquareCheckin
    {
        public string id = "";
        public string type = "";
        public string _private = "";
        public FourSquareUser user;
        public string timeZone = "";
        public FourSquareVenue venue;
        public FourSquareLocation location;
        public string shout = "";
        public string createdAt = "";
        public FourSquareSource source;
        public object photos = "";
        public List<FourSquareComment> comments = new List<FourSquareComment>();
        public List<FourSquareOverlaps> overlaps = new List<FourSquareOverlaps>();
        public bool IsMayor = false;
        public List<FourSquareNotification> notifications = new List<FourSquareNotification>();
        private string JSON = "";



        public FourSquareCheckin()
        {

        }

        public FourSquareCheckin(Dictionary<string, object> JSONDictionary)
        {

            JSON = JSONSerializer(JSONDictionary);

            id = GetDictionaryValue(JSONDictionary, "id");
            type = GetDictionaryValue(JSONDictionary, "type");
            _private = GetDictionaryValue(JSONDictionary, "private");
            if (JSONDictionary.ContainsKey("user"))
            {
                user = new FourSquareUser((Dictionary<string, object>)JSONDictionary["user"]);
            }

            timeZone = GetDictionaryValue(JSONDictionary, "timeZone");

            if (JSONDictionary.ContainsKey("venue"))
            {
                venue = new FourSquareVenue((Dictionary<string, object>)JSONDictionary["venue"]);
            }

            if (JSONDictionary.ContainsKey("location"))
            {
                location = new FourSquareLocation((Dictionary<string, object>)JSONDictionary["location"]);
            }

            shout = GetDictionaryValue(JSONDictionary, "shout");
            createdAt = GetDictionaryValue(JSONDictionary, "createdAt");

            if (JSONDictionary.ContainsKey("source"))
            {
                source = new FourSquareSource((Dictionary<string, object>)JSONDictionary["source"]);
            }

            if (JSONDictionary.ContainsKey("photos"))
            {
                Dictionary<string, object> PhotoThings = (Dictionary<string, object>)JSONDictionary["photos"];
                if (Int32.Parse(PhotoThings["count"].ToString()) > 0)
                {
                    foreach (object obj in (object[])PhotoThings["items"])
                    {
                        //TODO: Photos as an object
                        throw new Exception("Photos here!");
                    }
                }
            }

            if (JSONDictionary.ContainsKey("comments"))
            {
                if (((object[])ExtractDictionary(JSONDictionary, "comments")["items"]).Length > 0)
                {
                    foreach (object obj in ((object[])ExtractDictionary(JSONDictionary, "comments")["items"]))
                    {
                        comments.Add(new FourSquareComment((Dictionary<string, object>)obj));
                    }
                }
            }

            if (JSONDictionary.ContainsKey("overlaps"))
            {
                foreach (object obj in ((object[])ExtractDictionary(JSONDictionary, "overlaps")["items"]))
                {
                    overlaps.Add(new FourSquareOverlaps((Dictionary<string, object>)obj));
                }
            }

            if (JSONDictionary.ContainsKey("isMayor"))
            {
                if (JSONDictionary["isMayor"].ToString().Equals("True"))
                {
                    IsMayor = true;
                }
            }
        }
    }

    public class FourSquareCategory
    {
        public string id = "";
        public string icon = "";
        public List<string> parents = new List<string>();
        public bool primary = false;
        public string name = "";
        public string pluralName = "";
        private string JSON = "";

        public FourSquareCategory(Dictionary<string, object> JSONDictionary)
        {
            JSON = JSONSerializer(JSONDictionary);

            id = GetDictionaryValue(JSONDictionary, "ID");
            icon = GetDictionaryValue(JSONDictionary, "icon");
            foreach (object obj in ((object[])JSONDictionary["parents"]))
            {
                parents.Add((string)obj);
            }
            primary = GetDictionaryValue(JSONDictionary, "primary").Equals("True");
            name = GetDictionaryValue(JSONDictionary, "name");
            pluralName = GetDictionaryValue(JSONDictionary, "pluralName");
        }
    }

    public class FourSquareComment
    {
        public string id;
        public string createdAt;
        public FourSquareUser User;
        public string text;
        private string JSON = "";

        public FourSquareComment(Dictionary<string, object> JSONDictionary)
        {
            JSON = JSONSerializer(JSONDictionary);
            id = JSONDictionary["id"].ToString();
            createdAt = JSONDictionary["createdAt"].ToString();
            User = new FourSquareUser((Dictionary<string, object>)JSONDictionary["user"]);
            text = JSONDictionary["text"].ToString();
        }
    }

    public class FourSquareContact
    {
        public string Twitter = "";
        public string Phone = "";
        private string JSON = "";

        public FourSquareContact(Dictionary<string, object> JSONDictionary)
        {
            JSON = JSONSerializer(JSONDictionary);
            Twitter = GetDictionaryValue(JSONDictionary, "twitter");
            Phone = GetDictionaryValue(JSONDictionary, "phone");
        }
    }

    public class FourSquareImage
    {
        public string Prefix = "";
        public List<string> sizes = new List<string>();
        public string Name = "";
        private string JSON = "";

        public FourSquareImage(Dictionary<string, object> JSONDictionary)
        {
            JSON = JSONSerializer(JSONDictionary);
            Prefix = JSONDictionary["prefix"].ToString();
            foreach (object Size in ((object[])JSONDictionary["sizes"]))
            {
                sizes.Add(Size.ToString());
            }
            Name = JSONDictionary["name"].ToString();
        }
    }

    public class FourSquareLeaderBoard
    {
        public List<LeaderBoardItem> LeaderBoard = new List<LeaderBoardItem>();

        public struct LeaderBoardItem
        {
            public int Rank;
            public FourSquareUser User;
            public FourSquareScore Score;

            public LeaderBoardItem(Dictionary<string, object> JSONDictionary)
            {
                Rank = Int32.Parse(JSONDictionary["rank"].ToString());
                User = new FourSquareUser((Dictionary<string, object>)JSONDictionary["user"]);
                Score = new FourSquareScore((Dictionary<string, object>)JSONDictionary["scores"]);
            }
        }
        private string JSON = "";

        public FourSquareLeaderBoard(Dictionary<string, object> JSONDictionary)
        {
            JSON = JSONSerializer(JSONDictionary);

            foreach (Object Obj in (Object[])ExtractDictionary(JSONDictionary, "response:leaderboard")["items"])
            {
                LeaderBoard.Add(new LeaderBoardItem((Dictionary<string, object>)Obj));
            }
        }
    }

    public class FourSquareLocation
    {
        public string Address = "";
        public string CrossStreet = "";
        public string City = "";
        public string State = "";
        public string PostalCode = "";
        public string Country = "";
        public string Lat = "";
        public string Long = "";
        public string Distance = "";
        private string JSON = "";

        public FourSquareLocation(Dictionary<string, object> JSONDictionary)
        {
            JSON = JSONSerializer(JSONDictionary);
            Address = GetDictionaryValue(JSONDictionary, "address");
            CrossStreet = GetDictionaryValue(JSONDictionary, "crossStreet");
            City = GetDictionaryValue(JSONDictionary, "city");
            State = GetDictionaryValue(JSONDictionary, "state");
            PostalCode = GetDictionaryValue(JSONDictionary, "postalCode");
            Country = GetDictionaryValue(JSONDictionary, "country");
            Lat = GetDictionaryValue(JSONDictionary, "lat");
            Long = GetDictionaryValue(JSONDictionary, "lng");
            Distance = GetDictionaryValue(JSONDictionary, "distance");
        }

        public FourSquareLocation()
        {
            // TODO: Complete member initialization
        }
    }

    public class FourSquareMayorship
    {
        public string Type = "";
        public string Checkins = "";
        public string DaysBehind = "";
        public string Message = "";
        public FourSquareUser User;
        public string ImageURL = "";
        private string JSON = "";

        public FourSquareMayorship(Dictionary<string, object> JSONDictionary)
        {
            JSON = JSONSerializer(JSONDictionary);
            Type = GetDictionaryValue(JSONDictionary, "type");

            Checkins = GetDictionaryValue(JSONDictionary, "Checkins");
            DaysBehind = GetDictionaryValue(JSONDictionary, "DaysBehind");
            Message = GetDictionaryValue(JSONDictionary, "Message");
            ImageURL = GetDictionaryValue(JSONDictionary, "ImageURL");
            if (JSONDictionary.ContainsKey("user"))
            {
                User = new FourSquareUser((Dictionary<string, object>)JSONDictionary["user"]);
            }
        }

    }

    public class FourSquareNotification
    {
        public string Type = "";
        public string Message = "";
        public FourSquareMayorship Mayor;
        public FourSquareLeaderBoard LeaderBoard;
        public FourSquareTip Tip;
        public FourSquareScore Score;
        private string JSON = "";

        public FourSquareNotification(Dictionary<string, object> JSONDictionary)
        {
            JSON = JSONSerializer(JSONDictionary);
            Type = JSONDictionary["type"].ToString();
            switch (Type)
            {
                case "message":
                    Message = ((Dictionary<string, object>)JSONDictionary["item"])["message"].ToString();
                    break;
                case "mayorship":
                    //TODO: Notification Mayorships
                    throw new Exception("Finish FourSquareNotification");

                case "leaderboard":
                    //TODO: Notification Leaderboards
                    throw new Exception("Finish FourSquareNotification");

                case "tip":
                    //TODO: Notification Tips
                    throw new Exception("Finish FourSquareNotification");

                case "score":
                    //TODO: Notification Scores
                    throw new Exception("Finish FourSquareNotification");

                default:
                    throw new Exception("New Type of Notification");
            }

        }
    }

    public class FourSquareOverlaps
    {
        public string id = "";
        public string createdAt = "";
        public string type = "";
        public string timeZone = "";
        public FourSquareUser user;
        private string JSON = "";

        public FourSquareOverlaps(Dictionary<string, object> JSONDictionary)
        {
            JSON = JSONSerializer(JSONDictionary);
            id = JSONDictionary["id"].ToString();
            createdAt = JSONDictionary["createdAt"].ToString();
            type = JSONDictionary["type"].ToString();
            timeZone = JSONDictionary["timeZone"].ToString();
            user = new FourSquareUser((Dictionary<string, object>)JSONDictionary["user"]);
        }
    }

    public class FourSquarePhoto
    {
        public string id = "";
        public string createdAt = "";
        public string url = "";
        public object sizes;
        public object source;
        public FourSquareUser user;
        public FourSquareVenue venue;
        public FourSquareTip tip;
        public FourSquareCheckin checkin;
        private string JSON = "";

        public FourSquarePhoto(Dictionary<string, object> JSONDictionary)
        {
            JSON = JSONSerializer(JSONDictionary);
            id = JSONDictionary["id"].ToString();
            createdAt = JSONDictionary["createdAt"].ToString();
            url = JSONDictionary["url"].ToString();

            if (JSONDictionary.ContainsKey("sizes"))
            {
                //todo
            }

            if (JSONDictionary.ContainsKey("source"))
            {
                //todo
            }

            if (JSONDictionary.ContainsKey("user"))
            {
                //todo
            }

            if (JSONDictionary.ContainsKey("venue"))
            {
                //todo
            }

            if (JSONDictionary.ContainsKey("tip"))
            {
                //todo
            }

            if (JSONDictionary.ContainsKey("checkin"))
            {
                //todo
            }

            //throw new Exception("todo");
            //todo
        }
    }

    public class FourSquareRecommendedVenues
    {
        public Dictionary<string, string> keywords = new Dictionary<string, string>();
        public string warning = "";
        public Dictionary<string, List<recommends>> places = new Dictionary<string, List<recommends>>();
        private string JSON = "";


        public struct reason
        {
            public string type;
            public string message;
        }

        public struct recommends
        {
            public List<reason> reasons;
            public FourSquareVenue venue;
            public List<FourSquareTip> tips;
        }

        public FourSquareRecommendedVenues(Dictionary<string, object> JSONDictionary)
        {
            JSON = JSONSerializer(JSONDictionary);
            //foreach (object Obj in (object[])((Dictionary<string, object>)JSONDictionary["keywords"])["items"])
            //{
            //    keywords.Add(((Dictionary<string, object>)Obj)["displayName"].ToString(), ((Dictionary<string, object>)Obj)["keyword"].ToString());
            //}
            //if (JSONDictionary.ContainsKey("warning"))
            //{
            //    warning = ((Dictionary<string, object>)JSONDictionary["warning"])["text"].ToString();
            //}
            foreach (object GroupObj in ((object[])JSONDictionary["groups"]))
            {
                string Type = ((Dictionary<string, object>)GroupObj)["type"].ToString();

                List<recommends> recs = new List<recommends>();
                foreach (object ItemObj in (object[])((Dictionary<string, object>)GroupObj)["items"])
                {
                    recommends r = new recommends();
                    r.tips = new List<FourSquareTip>();
                    r.reasons = new List<reason>();

                    r.venue = new FourSquareVenue((Dictionary<string, object>)((Dictionary<string, object>)ItemObj)["venue"]);
                    if (((Dictionary<string, object>)ItemObj).ContainsKey("tips"))
                    {
                        foreach (object TipObj in (object[])((Dictionary<string, object>)ItemObj)["tips"])
                        {
                            r.tips.Add(new FourSquareTip((Dictionary<string, object>)TipObj));
                        }
                    }
                    foreach (object ReasonObj in (object[])ExtractDictionary((Dictionary<string, object>)ItemObj, "reasons")["items"])
                    {
                        reason reas = new reason();
                        reas.type = ((Dictionary<string, object>)ReasonObj)["type"].ToString();
                        reas.message = ((Dictionary<string, object>)ReasonObj)["message"].ToString();
                        r.reasons.Add(reas);
                    }
                    recs.Add(r);
                }
                places.Add(Type, recs);
            }
        }
    }

    public class FourSquareScore
    {
        public int recent = 0;
        public int max = 0;
        public int checkinsCount = 0;
        public int goal = 0;
        private string JSON = "";

        public FourSquareScore(Dictionary<string, object> JSONDictionary)
        {
            JSON = JSONSerializer(JSONDictionary);
            recent = (int)JSONDictionary["recent"];
            max = (int)JSONDictionary["max"];
            checkinsCount = (int)JSONDictionary["checkinsCount"];
            if (JSONDictionary.ContainsKey("goal"))
            {
                goal = (int)JSONDictionary["goal"];
            }
        }
    }

    public class FourSquareSource
    {
        public string Name = "";
        public string URL = "";
        private string JSON = "";

        public FourSquareSource(Dictionary<string, object> JSONDictionary)
        {
            JSON = JSONSerializer(JSONDictionary);
            Name = JSONDictionary["name"].ToString();
            URL = JSONDictionary["url"].ToString();
        }
    }

    public class FourSquareSpecial
    {
        public string id = "";
        public string type = "";
        public string message = "";
        public string description = "";
        public string finePrint = "";
        public bool unlocked = false;
        public string icon = "";
        public string title = "";
        public string state = "";
        public string progress = "";
        public string progressDescription = "";
        public string detail = "";
        public string target = "";
        public List<FourSquareUser> friendsHere = new List<FourSquareUser>();
        private string JSON = "";

        public FourSquareSpecial(Dictionary<string, object> JSONDictionary)
        {
            JSON = JSONSerializer(JSONDictionary);

            id = GetDictionaryValue(JSONDictionary, "id");
            type = GetDictionaryValue(JSONDictionary, "type");
            message = GetDictionaryValue(JSONDictionary, "message");
            description = GetDictionaryValue(JSONDictionary, "description");
            finePrint = GetDictionaryValue(JSONDictionary, "finePrint");
            if (GetDictionaryValue(JSONDictionary, "unlocked").ToLower().Equals("true"))
            {
                unlocked = true;
            }
            icon = GetDictionaryValue(JSONDictionary, "icon");
            title = GetDictionaryValue(JSONDictionary, "title");
            state = GetDictionaryValue(JSONDictionary, "state");
            progress = GetDictionaryValue(JSONDictionary, "progress");
            progressDescription = GetDictionaryValue(JSONDictionary, "progressDescription");
            detail = GetDictionaryValue(JSONDictionary, "detail");
            target = GetDictionaryValue(JSONDictionary, "target");
            //if (JSONDictionary.ContainsKey("friendsHere"))
            //{
            //    throw new Exception("Todo");
            //}
        }
    }

    public class FourSquareStats
    {
        public string checkinsCount = "";
        public string usersCount = "";
        private string JSON = "";

        public FourSquareStats(Dictionary<string, object> JSONDictionary)
        {
            JSON = JSONSerializer(JSONDictionary);
            checkinsCount = GetDictionaryValue(JSONDictionary, "checkinsCount");
            usersCount = GetDictionaryValue(JSONDictionary, "usersCount");
        }

        public FourSquareStats()
        {
            // TODO: Complete member initialization
        }
    }

    public class FourSquareTip
    {
        public string id = "";
        public string text = "";
        public string createdAt = "";
        public string status = "";
        public string url = "";
        public object photo = "";
        public FourSquareUser user;
        public FourSquareVenue venue;
        public int todocount = 0;
        public object done = "";
        private string JSON = "";

        public FourSquareTip(Dictionary<string, object> JSONDictionary)
        {

            JSON = JSONSerializer(JSONDictionary);
            id = GetDictionaryValue(JSONDictionary, "id");
            text = GetDictionaryValue(JSONDictionary, "text");
            createdAt = GetDictionaryValue(JSONDictionary, "createdAt");
            status = GetDictionaryValue(JSONDictionary, "status");
            url = GetDictionaryValue(JSONDictionary, "url");

            if (JSONDictionary.ContainsKey("photo"))
            {
                //throw new Exception("To Do Item for this class");
                //todo
            }

            if (JSONDictionary.ContainsKey("user"))
            {
                user = new FourSquareUser((Dictionary<string, object>)JSONDictionary["user"]);
            }
            if (JSONDictionary.ContainsKey("venue"))
            {
                venue = new FourSquareVenue((Dictionary<string, object>)JSONDictionary["venue"]);
            }
            if (JSONDictionary.ContainsKey("todo"))
            {
                if (((int)((Dictionary<string, object>)JSONDictionary["todo"])["count"]) > 0)
                {
                    todocount = ((int)((Dictionary<string, object>)JSONDictionary["todo"])["count"]);
                }
            }
            if (JSONDictionary.ContainsKey("done"))
            {
                if (((Dictionary<string, object>)JSONDictionary["done"]).ContainsKey("groups"))
                {
                    throw new Exception("To Do Item for this class");
                }
                if (((Dictionary<string, object>)JSONDictionary["done"]).ContainsKey("friends"))
                {
                    throw new Exception("To Do Item for this class");
                }
            }
        }

    }

    public class FourSquareTodo
    {
        public string id = "";
        public string createdAt = "";
        public FourSquareTip tip;
        private string JSON = "";

        public FourSquareTodo(Dictionary<string, object> JSONDictionary)
        {
            JSON = JSONSerializer(JSONDictionary);
            id = GetDictionaryValue(JSONDictionary, "ID");
            createdAt = GetDictionaryValue(JSONDictionary, "createdAt");
            tip = new FourSquareTip((Dictionary<string, object>)JSONDictionary["tip"]);
        }
    }

    public class FourSquareUser
    {
        public string id = "";
        public string firstName = "";
        public string lastName = "";
        public string homeCity = "";
        public string photo = "";
        public string gender = "";
        public string relationship = "";
        public string type = "";
        public List<KeyValue> contact = new List<KeyValue>();
        public string pings = "";
        public string badges = "0";
        public string checkins = "0";
        public string mayorships = "0";
        public List<FourSquareVenue> mayorshipItems = new List<FourSquareVenue>();
        public string tips = "0";
        public string todos = "0";
        public string friends = "0";
        public string followers = "0";
        public string requests = "0";
        private string JSON = "";

        public struct KeyValue
        {
            string Key;
            string Value;
            public KeyValue(string StringKey, string StringValue)
            {
                Key = StringKey;
                Value = StringValue;
            }
        }

        public FourSquareUser(Dictionary<string, object> JSONDictionary)
        {
            JSON = JSONSerializer(JSONDictionary);

            id = GetDictionaryValue(JSONDictionary, "id");

            firstName = GetDictionaryValue(JSONDictionary, "firstName");
            lastName = GetDictionaryValue(JSONDictionary, "lastName");
            homeCity = GetDictionaryValue(JSONDictionary, "homeCity");

            photo = JSONDictionary["photo"].ToString();
            gender = JSONDictionary["gender"].ToString();
            if (JSONDictionary.ContainsKey("relationship"))
            {
                relationship = JSONDictionary["relationship"].ToString();
            }

            photo = GetDictionaryValue(JSONDictionary, "photo");
            gender = GetDictionaryValue(JSONDictionary, "gender");
            relationship = GetDictionaryValue(JSONDictionary, "relationship");

            if (JSONDictionary.ContainsKey("badges"))
            {
                badges = ExtractDictionary(JSONDictionary, "badges")["count"].ToString();
            }
            if (JSONDictionary.ContainsKey("mayorships"))
            {
                mayorships = ExtractDictionary(JSONDictionary, "mayorships")["count"].ToString();
                foreach (object Obj in (Object[])ExtractDictionary(JSONDictionary, "mayorships")["items"])
                {
                    mayorshipItems.Add(new FourSquareVenue((Dictionary<string, object>)Obj));
                }
            }
            if (JSONDictionary.ContainsKey("checkins"))
            {
                checkins = ExtractDictionary(JSONDictionary, "checkins")["count"].ToString();
            }
            if (JSONDictionary.ContainsKey("friends"))
            {
                friends = ExtractDictionary(JSONDictionary, "friends")["count"].ToString();
            }
            if (JSONDictionary.ContainsKey("followers"))
            {
                followers = ExtractDictionary(JSONDictionary, "followers")["count"].ToString();
            }
            if (JSONDictionary.ContainsKey("requests"))
            {
                requests = ExtractDictionary(JSONDictionary, "requests")["count"].ToString();
            }
            if (JSONDictionary.ContainsKey("tips"))
            {
                tips = ExtractDictionary(JSONDictionary, "tips")["count"].ToString();
            }
            if (JSONDictionary.ContainsKey("todos"))
            {
                todos = ExtractDictionary(JSONDictionary, "todos")["count"].ToString();
            }


            type = GetDictionaryValue(JSONDictionary, "type");
            if (JSONDictionary.ContainsKey("contact"))
            {
                string email = GetDictionaryValue((Dictionary<string, object>)JSONDictionary["contact"], "email");
                if (!email.Equals(""))
                {
                    contact.Add(new KeyValue("email", email));
                }

                string twitter = GetDictionaryValue((Dictionary<string, object>)JSONDictionary["contact"], "twitter");
                if (!twitter.Equals(""))
                {
                    contact.Add(new KeyValue("twitter", twitter));
                }

                string facebook = GetDictionaryValue((Dictionary<string, object>)JSONDictionary["contact"], "facebook");
                if (!facebook.Equals(""))
                {
                    contact.Add(new KeyValue("facebook", facebook));
                }

                string phone = GetDictionaryValue((Dictionary<string, object>)JSONDictionary["contact"], "phone");
                if (!phone.Equals(""))
                {
                    contact.Add(new KeyValue("phone", phone));
                }
            }
            pings = GetDictionaryValue(JSONDictionary, "pings");
        }
    }

    public class FourSquareVenue
    {
        public string id = "";
        public string name = "";
        public bool verified = false;
        public FourSquareContact contact;
        public FourSquareLocation location;
        public List<FourSquareCategory> categories = new List<FourSquareCategory>();
        public List<FourSquareSpecial> specials = new List<FourSquareSpecial>();
        public object hereNow;
        public string description = "";
        public FourSquareStats stats;
        public FourSquareMayorship mayor;
        public Dictionary<string, List<FourSquareTip>> tips = new Dictionary<string, List<FourSquareTip>>();
        public List<FourSquareTodo> todos = new List<FourSquareTodo>();
        public List<string> tags = new List<string>();
        public int beenHere = 0;
        public string shortUrl = "";
        public string url = "";
        public string timeZone = "";
        public List<FourSquareSpecial> specialsNearby = new List<FourSquareSpecial>();
        public object photos = "";
        private string JSON = "";

        public FourSquareVenue()
        {

        }

        public FourSquareVenue(Dictionary<string, object> JSONDictionary)
        {
            JSON = JSONSerializer(JSONDictionary);
            id = GetDictionaryValue(JSONDictionary, "id");
            name = GetDictionaryValue(JSONDictionary, "name");
            verified = GetDictionaryValue(JSONDictionary, "verified").Equals("True");

            contact = new FourSquareContact((Dictionary<string, object>)JSONDictionary["contact"]);

            if (JSONDictionary.ContainsKey("location"))
            {
                location = new FourSquareLocation((Dictionary<string, object>)JSONDictionary["location"]);
            }

            if (JSONDictionary.ContainsKey("categories"))
            {
                foreach (object obj in ((object[])JSONDictionary["categories"]))
                {
                    categories.Add(new FourSquareCategory((Dictionary<string, object>)obj));
                }
            }

            if (JSONDictionary.ContainsKey("specials"))
            {
                foreach (object Obj in (object[])JSONDictionary["specials"])
                {
                    specials.Add(new FourSquareSpecial((Dictionary<string, object>)Obj));
                }
            }

            if (JSONDictionary.ContainsKey("hereNow"))
            {

                if (((int)ExtractDictionary(JSONDictionary, "hereNow")["count"]) > 0)
                {
                    //TODO here now
                    //throw new Exception("hereNow");
                }
            }

            description = GetDictionaryValue(JSONDictionary, "description");

            if (JSONDictionary.ContainsKey("stats"))
            {
                stats = new FourSquareStats((Dictionary<string, object>)JSONDictionary["stats"]);
            }

            if (JSONDictionary.ContainsKey("mayor"))
            {
                mayor = new FourSquareMayorship(ExtractDictionary(JSONDictionary, "mayor"));
                mayor.Checkins = ExtractDictionary(JSONDictionary, "mayor")["count"].ToString();
            }

            if (JSONDictionary.ContainsKey("tips"))
            {
                foreach (object Obj in (object[])ExtractDictionary(JSONDictionary, "tips")["groups"])
                {
                    Dictionary<string, object> Group = ((Dictionary<string, object>)Obj);
                    List<FourSquareTip> TipList = new List<FourSquareTip>();
                    foreach (object Tip in (object[])((Dictionary<string, object>)Obj)["items"])
                    {
                        TipList.Add(new FourSquareTip((Dictionary<string, object>)Tip));
                    }
                    tips.Add(GetDictionaryValue(Group, "type"), TipList);
                }
            }

            if (JSONDictionary.ContainsKey("todos"))
            {
                //TODO: Todos
                //IRONY
                if ((int)((Dictionary<string, Object>)JSONDictionary["todos"])["count"] > 0)
                {
                    //throw new Exception("todos");
                }
            }

            if (JSONDictionary.ContainsKey("tags"))
            {
                foreach (object Obj in (object[])JSONDictionary["tags"])
                {
                    tags.Add((string)Obj);
                }
            }

            Int32.TryParse(GetDictionaryValue(JSONDictionary, "beenHere"), out beenHere);
            shortUrl = GetDictionaryValue(JSONDictionary, "shortUrl");
            url = GetDictionaryValue(JSONDictionary, "url");
            timeZone = GetDictionaryValue(JSONDictionary, "timeZone");

            if (JSONDictionary.ContainsKey("specialsNearby"))
            {
                foreach (object Obj in (object[])JSONDictionary["specialsNearby"])
                {
                    specialsNearby.Add(new FourSquareSpecial((Dictionary<string, object>)Obj));
                    throw new Exception("See if this actually worlks");
                }
            }

            //if (JSONDictionary.ContainsKey("photos"))
            //{
            //    if ((int)((Dictionary<string, object>)JSONDictionary["photos"])["count"] > 0)
            //    {
            //        throw new Exception("To Do Item for this class");
            //    }
            //}
        }
    }

    public class FourSquareVenueCategory
    {
        public string id = "";
        public string name = "";
        public string pluralName = "";
        public string icon = "";
        public List<FourSquareVenueCategory> categories = new List<FourSquareVenueCategory>();
        private string JSON = "";

        public FourSquareVenueCategory(Dictionary<string, object> JSONDictionary)
        {
            JSON = JSONSerializer(JSONDictionary);
            id = GetDictionaryValue(JSONDictionary, "id");
            name = GetDictionaryValue(JSONDictionary, "name");
            pluralName = GetDictionaryValue(JSONDictionary, "pluralName");
            icon = GetDictionaryValue(JSONDictionary, "icon");
            if (JSONDictionary.ContainsKey("categories"))
            {
                foreach (object Obj in (object[])JSONDictionary["categories"])
                {
                    categories.Add(new FourSquareVenueCategory((Dictionary<string, object>)Obj));
                }
            }
        }
    }

    #endregion FourSquareClasses

    #region Helpers

    public class HTTPGet
    {
        private HttpWebRequest request;
        private HttpWebResponse response;

        private string responseBody;
        private string escapedBody;
        private int statusCode;

        public string ResponseBody { get { return responseBody; } }
        public string EscapedBody { get { return GetEscapedBody(); } }
        public int StatusCode { get { return statusCode; } }
        public string Headers { get { return GetHeaders(); } }
        public string StatusLine { get { return GetStatusLine(); } }



        public void Request(string url)
        {
            StringBuilder respBody = new StringBuilder();

            this.request = (HttpWebRequest)WebRequest.Create(url);

            try
            {
                this.response = (HttpWebResponse)this.request.GetResponse();
                byte[] buf = new byte[8192];
                Stream respStream = this.response.GetResponseStream();
                int count = 0;
                do
                {
                    count = respStream.Read(buf, 0, buf.Length);
                    if (count != 0)
                        respBody.Append(Encoding.ASCII.GetString(buf, 0, count));
                }
                while (count > 0);

                this.responseBody = respBody.ToString();
                this.statusCode = (int)(HttpStatusCode)this.response.StatusCode;
            }
            catch (WebException ex)
            {
                this.response = (HttpWebResponse)ex.Response;
                this.responseBody = "No Server Response";
                this.escapedBody = "No Server Response";
            }
        }

        private string GetEscapedBody()
        {  // HTML escaped chars
            string escapedBody = responseBody;
            escapedBody = escapedBody.Replace("&", "&amp;");
            escapedBody = escapedBody.Replace("<", "&lt;");
            escapedBody = escapedBody.Replace(">", "&gt;");
            escapedBody = escapedBody.Replace("'", "&apos;");
            escapedBody = escapedBody.Replace("\"", "&quot;");
            this.escapedBody = escapedBody;

            return escapedBody;
        }

        private string GetHeaders()
        {
            if (response == null)
                return "No Server Response";
            else
            {
                StringBuilder headers = new StringBuilder();
                for (int i = 0; i < this.response.Headers.Count; ++i)
                    headers.Append(String.Format("{0}: {1}\n",
                        response.Headers.Keys[i], response.Headers[i]));

                return headers.ToString();
            }
        }

        private string GetStatusLine()
        {
            if (response == null)
                return "No Server Response";
            else
                return String.Format("HTTP/{0} {1} {2}", response.ProtocolVersion,
                    (int)response.StatusCode, response.StatusDescription);
        }
    }

    public class HTTPPost
    {

        private HttpWebRequest request;
        private HttpWebResponse response;

        public string ResponseBody;
        public string EscapedBody;
        public string StatusCode;
        public string Headers;


        public HTTPPost(Uri Url, Dictionary<string, string> Parameters)
        {
            StringBuilder respBody = new StringBuilder();

            request = (HttpWebRequest)HttpWebRequest.Create(Url);
            request.UserAgent = "Mozilla/5.0 (iPhone; U; CPU like Mac OS X; en) AppleWebKit/420+ (KHTML, like Gecko) Version/3.0 Mobile/1C10 Safari/419.3";
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            string content = "?";
            foreach (string k in Parameters.Keys)
            {
                content += k + "=" + Parameters[k] + "&";
            }
            content = content.TrimEnd(new char[] { '&' });
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] byte1 = encoding.GetBytes(content);
            request.ContentLength = byte1.Length;
            byte[] buf = new byte[8192];
            using (Stream rs = request.GetRequestStream())
            {
                rs.Write(byte1, 0, byte1.Length);
                rs.Close();

                response = (HttpWebResponse)request.GetResponse();
                Stream respStream = response.GetResponseStream();

                int count = 0;
                do
                {
                    count = respStream.Read(buf, 0, buf.Length);
                    if (count != 0)
                        respBody.Append(Encoding.ASCII.GetString(buf, 0, count));
                }
                while (count > 0);

                respStream.Close();
                ResponseBody = respBody.ToString();
                EscapedBody = GetEscapedBody();
                StatusCode = GetStatusLine();
                Headers = GetHeaders();

                response.Close();
            }
        }

        private string GetEscapedBody()
        {  // HTML escaped chars
            string escapedBody = ResponseBody;
            escapedBody = escapedBody.Replace("&", "&amp;");
            escapedBody = escapedBody.Replace("<", "&lt;");
            escapedBody = escapedBody.Replace(">", "&gt;");
            escapedBody = escapedBody.Replace("'", "&apos;");
            escapedBody = escapedBody.Replace("\"", "&quot;");
            return escapedBody;
        }

        private string GetHeaders()
        {
            if (response == null)
                return "No Server Response";
            else
            {
                StringBuilder headers = new StringBuilder();
                for (int i = 0; i < this.response.Headers.Count; ++i)
                    headers.Append(String.Format("{0}: {1}\n",
                        response.Headers.Keys[i], response.Headers[i]));

                return headers.ToString();
            }
        }

        private string GetStatusLine()
        {
            if (response == null)
                return "No Server Response";
            else
                return String.Format("HTTP/{0} {1} {2}", response.ProtocolVersion,
                    (int)response.StatusCode, response.StatusDescription);
        }
    }

    private static Dictionary<string, object> ExtractDictionary(Dictionary<string, object> JSONDictionary, string DictionaryPath)
    {
        string Key = "";
        Dictionary<string, object> DictionaryObject = JSONDictionary;

        while (DictionaryPath.Length > 0)
        {
            if (DictionaryPath.Contains(":"))
            {
                Key = DictionaryPath.Substring(0, DictionaryPath.IndexOf(":"));
                DictionaryPath = DictionaryPath.Substring(DictionaryPath.IndexOf(":") + 1);
                if (DictionaryObject.ContainsKey(Key))
                {
                    DictionaryObject = (Dictionary<string, object>)DictionaryObject[Key];
                }
                else
                {
                    return DictionaryObject;
                }
            }
            else
            {
                Key = DictionaryPath;
                DictionaryPath = "";
                if (DictionaryObject.ContainsKey(Key))
                {
                    DictionaryObject = (Dictionary<string, object>)DictionaryObject[Key];
                }
                else
                {
                    return DictionaryObject;
                }
            }
        }
        return DictionaryObject;
    }

    private static string GetDictionaryValue(Dictionary<string, object> JSONDictionary, string Key)
    {
        string ReturnString = "";
        if (JSONDictionary.ContainsKey(Key))
        {
            ReturnString = JSONDictionary[Key].ToString();
        }
        return ReturnString;
    }

    private static string JSONSerializer(Dictionary<string, object> DictionaryObject)
    {
        JavaScriptSerializer JSONSerializer = new JavaScriptSerializer();
        return JSONSerializer.Serialize(DictionaryObject);
    }

    private static Dictionary<string, object> JSONDeserializer(string JSON)
    {
        JavaScriptSerializer JSONDeserializer = new JavaScriptSerializer();
        return (Dictionary<string, object>)JSONDeserializer.Deserialize(JSON, typeof(object));
    }

    #endregion Helpers
}