using System;

[Serializable]
public class ScoreData
{
    public string nickname;
    public float time;
    public int deaths;

    public ScoreData(string nickname, float time, int deaths)
    {
        this.nickname = nickname;
        this.time = time;
        this.deaths = deaths;
    }
}