using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

//This Class Is Used To Make The Storage And Manipulation Of Two Variables Easier
public class PlayerInfo
{
    public string name;
    public float score;

    public PlayerInfo(string name, float score)
    {
        this.name = name;
        this.score = score;
    }
}

public class LeaderBoard : MonoBehaviour
{
    
    // InputFields To Populate The List
    public TMP_InputField userName;
    public TextMeshProUGUI score;
    public TMP_InputField display;
    public int maxEntries;
    public GameObject scoreControls;

    //List To Hold "PlayerInfo" Objects
    List<PlayerInfo> collectedStats;

    // Use this for initialization
    void Start()
    {
        collectedStats = new List<PlayerInfo>();
        LoadLeaderBoard();
        if (ScoreCounter.GetScore() > 0)
        {
            score.text = Helpers.FormatSpaceIntoThousands(ScoreCounter.GetScore());
            CheckScoreValidity(ScoreCounter.GetScore());
        } else
        {
            scoreControls.SetActive(false);
            score.gameObject.SetActive(false);
        }
    }

    public void SubmitButton()
    {
        //Create Object Using Values From InputFields, This Is Done So That A Name And Score Can Easily Be Moved/Sorted At The Same Time
        PlayerInfo stats = new PlayerInfo(userName.text.ToUpper(), ScoreCounter.GetScore());//Depending On How You Obtain The Score, It May Be Necessary To Parse To Integer

        //Add The New Player Info To The List
        collectedStats.Add(stats);

        //Clear InputFields Now That The Object Has Been Created
        userName.text = "";
        score.text = "";

        //Start Sorting Method To Place Object In Correct Index Of List
        SortStats();

        scoreControls.SetActive(false);
        score.gameObject.SetActive(false);
        ScoreCounter.ResetScore();        

        SoundSpawner.SpawnSound(transform.position, transform, SoundLibrary.GetClip("menu_button_select"), 0);
    }

    void SortStats()
    {
        collectedStats.Sort((a, b) => b.score.CompareTo(a.score));

        if (collectedStats.Count > maxEntries)
        {
            collectedStats.RemoveRange(maxEntries, collectedStats.Count - maxEntries); //Starting from index 'maxEntries' remove 'collectedStats.Count - maxEntries' entries onwards. ex: after i 10, remove (25 - 10 = 15) entries
        }

        // Persist sorted leaderboard
        UpdatePlayerPrefsString();
    }

    void UpdatePlayerPrefsString()
    {
        //Start With A Blank String
        string stats = "";

        //Add Each Name And Score From The Collection To The String
        for (int i = 0; i < collectedStats.Count; i++)
        {
            //Be Sure To Add A Comma To Both The Name And Score, It Will Be Used To Separate The String Later
            stats += collectedStats[i].name + ";";
            stats += collectedStats[i].score + ";";
        }

        //Add The String To The PlayerPrefs, This Allows The Information To Be Saved Even When The Game Is Turned Off
        PlayerPrefs.SetString("LeaderBoards", stats);

        //Now Update The On Screen LeaderBoard
        UpdateLeaderBoardVisual();
    }

    void UpdateLeaderBoardVisual()
    {
        //Clear Current Displayed LeaderBoard
        display.text = "";

        //Loop Through The List And Add The Names And Scores To The Display Text
        for (int i = 0; i <= collectedStats.Count - 1; i++)
        {
            display.text += collectedStats[i].name + " - " + Helpers.FormatSpaceIntoThousands(collectedStats[i].score) + "\n";
        }
    }

    void CheckScoreValidity(float _score)
    {
        if (collectedStats.Count < maxEntries)
            return;

        if (_score < collectedStats.Last().score)
        {
            LoadMainMenuFromReplay.instance.LoadMainMenu();
            scoreControls.SetActive(false);            
        }
    }

    void LoadLeaderBoard()
    {
        //Load The String Of The Leaderboard That Was Saved In The "UpdatePlayerPrefsString" Method
        string stats = PlayerPrefs.GetString("LeaderBoards", "");

        //Assign The String To An Array And Split Using The Comma Character
        //This Will Remove The Comma From The String, And Leave Behind The Separated Name And Score
        string[] stats2 = stats.Split(';');

        //Loop Through The Array 2 At A Time Collecting Both The Name And Score
        for (int i = 0; i < stats2.Length - 2; i += 2)
        {
            //Use The Collected Information To Create An Object
            PlayerInfo loadedInfo = new PlayerInfo(stats2[i], float.Parse(stats2[i + 1]));

            //Add The Object To The List
            collectedStats.Add(loadedInfo);

            //Update On Screen LeaderBoard
            UpdateLeaderBoardVisual();
        }
    }

    public void ClearPrefs()
    {
        //Use This To Delete All Names And Scores From The LeaderBoard
        PlayerPrefs.DeleteAll();

        collectedStats = new List<PlayerInfo>();

        //Clear Current Displayed LeaderBoard
        display.text = "";
    }
}
