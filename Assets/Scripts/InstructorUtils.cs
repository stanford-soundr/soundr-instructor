public static class InstructorUtils
{
    public static double CurrentTimestamp() {
        var epochStart = 
            new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        return (System.DateTime.UtcNow - epochStart).TotalSeconds;
    }
}