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
    
    void Start()
    {
        _TryUrl();
    }

    public void _TryUrl()
    {
        Debug.Log("_TryUrl");
        VRCStringDownloader.LoadUrl(apiKey, (IUdonEventReceiver)this);
    }
    
    public override void OnStringLoadError(IVRCStringDownload result)
    {
        Debug.LogError("OnStringLoadError");
        SendCustomEventDelayedSeconds(nameof(_TryUrl),retryTimer);
    }

    public override void OnStringLoadSuccess(IVRCStringDownload result)
    {
        Debug.Log("OnStringLoadSuccess");
        if (!(VRCJson.TryDeserializeFromJson(result.Result, out DataToken uwu)))
        {
            Debug.LogError("Parse Fail_1");
        }
        
        if (!(uwu.DataDictionary.TryGetValue("jam_games",out DataToken _games)))
        {
            Debug.LogError("Parse Fail_2");
        }
        
        for (int i = 0; i < _games.DataList.Count; i++)
        {
            if (!(_games.DataList.TryGetValue(i, out DataToken _game)))
            {
                Debug.LogError("Parse Fail_3");
                continue;
            }
            
            if (_game.DataDictionary.TryGetValue("id",out DataToken _id))
            {
                //Debug.Log(_id.Double);
                if (_id.Double == submissionId)
                {
                    Debug.Log("Found ID Match");
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
            voteCount.text = $"Voters: {_rating_count.Double.ToString()}";
        }
        if (_foundGame.DataDictionary.TryGetValue("url",out DataToken _url))
        {
            urlField.text = $"https://itch.io{_url.String}";
        }
        if (_foundGame.DataDictionary.TryGetValue("game",out DataToken _game2))
        {
            if (_game2.DataDictionary.TryGetValue("title",out DataToken _title))
            {
                worldTitle.text = $"<size=10>Welcome To:</size>\n{_title.String}";
            }
        }
    }
}