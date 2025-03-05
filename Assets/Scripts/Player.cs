
using System.Drawing;

public class Player
{
    public int LifePoint { get; private set; }
    public int ScorePoint { get; private set; }

    public void IncreaseLifePoint() => LifePoint++;
    public void DecreaseLifePoint() => LifePoint--;
    public void ResetLifePoint() => LifePoint = 0;
    public void SetLifePoint(int point) => LifePoint = point;

    public void IncreaseScorePoint() => ScorePoint++;
    public void DecreaseScorePoint() => ScorePoint--;
    public void ResetScorePoint() => ScorePoint = 0;
    public void SetScorePoint(int point) => ScorePoint = point;
}
