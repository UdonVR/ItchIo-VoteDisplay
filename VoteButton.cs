using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Data;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class VoteButton : UdonSharpBehaviour
{
    public VRCUrl apiKey = new VRCUrl("https://itch.io/jam/387088/entries.json");
    [HideInInspector] public double submissionId;

    public TextMeshProUGUI voteCount;
    public TextMeshProUGUI worldTitle;
    public InputField urlField;


    public float UpdateRate = 10f;
    private float retryTimer = 5f;

    public GameObject voteDisplay;
    public GameObject dateTimeDisplay;
    public TextMeshProUGUI dateTimeText;
    public TextMeshProUGUI voteButtonText;
    public int votingStarts;
    [HideInInspector]public DateTime votingStartsDateTime;
    
    void Start()
    {
        _TryUrl();
        votingStartsDateTime = DateTimeOffset.FromUnixTimeSeconds(votingStarts).DateTime;
        if (votingStartsDateTime <= DateTime.Now)
        {
            voteButtonText.text = "Vote";
            dateTimeDisplay.SetActive(false);
            voteDisplay.SetActive(true);
            return;
        }
        
        DateTime time = DateTimeOffset.FromUnixTimeSeconds(votingStarts).DateTime;
        time = time.ToLocalTime();
        string tmp = $"{time:MM}";
        switch (tmp)
        {
            case "01":
                tmp = "Jan";
                break;
            case "02":
                tmp = "Feb";
                break;
            case "03":
                tmp = "March";
                break;
            case "04":
                tmp = "April";
                break;
            case "05":
                tmp = "May";
                break;
            case "06":
                tmp = "June";
                break;
            case "07":
                tmp = "July";
                break;
            case "08":
                tmp = "Aug";
                break;
            case "09":
                tmp = "Sep";
                break;
            case "10":
                tmp = "Oct";
                break;
            case "11":
                tmp = "Nov";
                break;
            case "12":
                tmp = "Dec";
                break;
        }
        dateTimeText.text = $"Voting Begins:\n{tmp}, {time:dd}";
        
    }

    public void _TryUrl()
    {
        VRCStringDownloader.LoadUrl(apiKey, (IUdonEventReceiver)this);
    }
    
    public override void OnStringLoadError(IVRCStringDownload result)
    {
        Debug.LogError("[VoteButton] OnStringLoadError");
        SendCustomEventDelayedSeconds(nameof(_TryUrl),retryTimer);
    }

    public override void OnStringLoadSuccess(IVRCStringDownload result)
    {
        Debug.Log("[VoteButton] OnStringLoadSuccess");
        if (!(VRCJson.TryDeserializeFromJson(result.Result, out DataToken uwu)))
        {
            Debug.LogError("[VoteButton] Parse Fail_1");
        }
        
        if (!(uwu.DataDictionary.TryGetValue("jam_games",out DataToken _games)))
        {
            Debug.LogError("[VoteButton] Parse Fail_2");
        }
        
        for (int i = 0; i < _games.DataList.Count; i++)
        {
            if (!(_games.DataList.TryGetValue(i, out DataToken _game)))
            {
                Debug.LogError("[VoteButton] Parse Fail_3");
                continue;
            }
            
            if (_game.DataDictionary.TryGetValue("id",out DataToken _id))
            {
                //Debug.Log(_id.Double);
                if (_id.Double == submissionId)
                {
                    Debug.Log("[VoteButton] Found ID Match");
                    _Pull(_game);
                    break;
                }
            }
        }
        SendCustomEventDelayedSeconds(nameof(_TryUrl),UpdateRate * 60);
    }

    private void _Pull(DataToken _foundGame)
    {
        if (_foundGame.DataDictionary.TryGetValue("rating_count",out DataToken _rating_count))
        {
            voteCount.text = $"{_rating_count.Double.ToString()}";
        }
        if (_foundGame.DataDictionary.TryGetValue("url",out DataToken _url))
        {
            urlField.text = $"https://itch.io{_url.String}";
        }
        if (_foundGame.DataDictionary.TryGetValue("game",out DataToken _game2))
        {
            if (_game2.DataDictionary.TryGetValue("title",out DataToken _title))
            {
                worldTitle.text = $"{_title.String}";
            }
        }
    }
}