namespace Practice.Core;

public static class Grader
{
    public static string Grade(int score)
    {
        if (score < 0 || score > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(score), "Score must be between 0 and 100.");
        }

        if (score >= 70)
        {
            return "A";
        }

        if (score > 60)
        {
            return "B";
        }

        if (score >= 50)
        {
            return "C";
        }

        return "Fail";
    }
}
