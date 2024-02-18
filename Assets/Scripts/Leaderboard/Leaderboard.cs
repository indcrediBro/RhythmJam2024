using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Dan.Main;
using System.Linq;
using Dan.Models;

public class Leaderboard : MonoBehaviour
{
    private const string PUBLIC_KEY = "2265b207f3b826508000aba16cf76f62bce67e366237d6a07cbe3c9503b0b13b";

    [SerializeField] private List<TMP_Text> names;
    [SerializeField] private List<TMP_Text> scores;

    [SerializeField] private TMP_InputField m_playeNameInputField;
    [SerializeField] private Transform m_leaderboardUITransform;
    [SerializeField] private GameObject m_leaderboardEntryPrefab;
    private void Start()
    {
        GetLeaderBoard();
    }

    public void GetLeaderBoard()
    {
        LeaderboardCreator.GetLeaderboard(PUBLIC_KEY, ((msg) =>
        {
            int loopLength = (msg.Length < names.Count) ? msg.Length : names.Count;

            for (int i = 0; i < loopLength; i++)
            {
                Dan.Models.Entry entry = msg[i];
                LeaderboardEntry newEntry = Instantiate(m_leaderboardEntryPrefab, m_leaderboardUITransform)
                                            .GetComponent<LeaderboardEntry>();

                newEntry.InitializeEntry(entry.Rank, entry.Username, entry.Score);
            }
            Debug.Log(msg);
        }));

        LeaderboardCreator.GetLeaderboard(PUBLIC_KEY, OnLeaderboardLoaded);
    }

    public void SetEntry(string _username, int _score)
    {
        LeaderboardCreator.UploadNewEntry(PUBLIC_KEY, _username, _score, ((msg) =>
        {
            GetLeaderBoard();
        }));
    }

    private void OnLeaderboardLoaded(Entry[] entries)
    {
        foreach (Transform t in m_leaderboardUITransform)
            Destroy(t.gameObject);

        foreach (var t in entries)
        {
            LeaderboardEntry newEntry = Instantiate(m_leaderboardEntryPrefab, m_leaderboardUITransform)
                                           .GetComponent<LeaderboardEntry>();

            newEntry.InitializeEntry(t.Rank, t.Username, t.Score);

        }
    }
}
