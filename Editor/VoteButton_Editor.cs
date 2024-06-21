using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using VRC.SDK3.Data;

[CustomEditor(typeof(VoteButton))]
public class VoteButton_Editor : Editor
{
    private string assetURL = "";
    private static WWW request;

    private static bool run = false;
    private static IEnumerator en = null;

    private bool hasLoaded = false;

    private int _o;

    private VoteButton _script;
    private ItichJam _jsonData;

    private bool _advanceMode = false;
    public override void OnInspectorGUI()
    {
        _script = (VoteButton)target;
        assetURL = _script.apiKey.ToString();
        if (GUILayout.Button(("Fetch API Data")))
        {
            Debug.Log("Requesting");

            using (UnityWebRequest request = UnityWebRequest.Get(assetURL))
            {
                // Use DownloadHandlerAssetBundle for assets
                request.downloadHandler = new DownloadHandlerBuffer();
                
                request.SendWebRequest();
                Debug.Log("Request Sent");

                while (!request.isDone)
                {
                    Debug.Log("Request in progress");
                }

                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError(request.error);
                }
                else
                {
                    Debug.Log(request.responseCode.ToString());
                    Debug.Log(request.result.ToString());
                    
                    Debug.Log(request.downloadHandler.text);
                    _jsonData = JsonConvert.DeserializeObject<ItichJam>(request.downloadHandler.text);
                }
            }
            hasLoaded = true;
        }

        DrawApiStuff();
        DrawAdvanceMode();
        DateTime time = DateTimeOffset.FromUnixTimeSeconds(_script.votingStarts).DateTime;
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
        _script.dateTimeText.text = $"Voting Begins:\n{tmp}, {time:dd}";
        //_script.dateTimeText.text = $"{time}";

        PrefabUtility.RecordPrefabInstancePropertyModifications(_script);
    }

    private void DrawAdvanceMode()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Advance Setup: ", GUILayout.Width(100f));
        _advanceMode = EditorGUILayout.Toggle(_advanceMode);
        EditorGUILayout.EndHorizontal();
        if (_advanceMode)
        {
            EditorGUILayout.HelpBox(
                "This is for creating your own Prefab.\n!!If you are using the included prefab, do not use this!!",
                MessageType.Warning);
            base.OnInspectorGUI();
        }
    }

    private void DrawApiStuff()
    {
        if (!hasLoaded)
        {
            EditorGUILayout.HelpBox("You need to fetch the API data before editing",MessageType.Error);
        }
        EditorGUI.BeginDisabledGroup(!hasLoaded);
        if (hasLoaded)
        {
            EditorGUILayout.BeginHorizontal();
                string[] popupStrings = new string[_jsonData.jam_games.Count + 1];
                string[] popupIds = new string[_jsonData.jam_games.Count + 1];
                popupStrings[0] = "-- NONE --";
                popupIds[0] = "NONE";
                for (int i = 1; i < popupStrings.Length; i++)
                {
                    popupStrings[i] = _jsonData.jam_games[i - 1].game.title;
                    popupIds[i] = _jsonData.jam_games[i - 1].id.ToString();
                }

                _o = Array.IndexOf(popupIds, _script.submissionId.ToString());
                if (_o == -1)
                {
                    _o = 0;
                }

                if (_jsonData.jam_games == null || _jsonData.jam_games.Count == 0)
                {
                    EditorGUILayout.HelpBox("No Submissions", MessageType.Error);
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.EndDisabledGroup();
                    return;
                }

                int _oo = EditorGUILayout.Popup(_o, popupStrings);
            EditorGUILayout.EndHorizontal();

            if (_oo == 0)
            {
                _script.submissionId = 0;
                _script.worldTitle.text = "--";
                _script.voteCount.text = "--";
                _script.urlField.text = "";
                return;
            }
            _oo = _oo - 1;
            _script.submissionId = _jsonData.jam_games[_oo].id;
            if (_script.voteCount != null) _script.voteCount.text = _jsonData.jam_games[_oo].rating_count.ToString();
            if (_script.worldTitle != null) _script.worldTitle.text = _jsonData.jam_games[_oo].game.title;
            if (_script.urlField != null) _script.urlField.text = $"https://itch.io{_jsonData.jam_games[_oo].url}";

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("ID: ", GUILayout.Width(50f));
                GUILayout.TextField(_jsonData.jam_games[_oo].id.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Votes: ", GUILayout.Width(50f));
                GUILayout.TextField(_jsonData.jam_games[_oo].rating_count.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Title: ", GUILayout.Width(50f));
                GUILayout.TextField(_jsonData.jam_games[_oo].game.title);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Url: ", GUILayout.Width(50f));
                GUILayout.TextField($"https://itch.io{_jsonData.jam_games[_oo].url}");
            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.EndDisabledGroup();
    }
    
}