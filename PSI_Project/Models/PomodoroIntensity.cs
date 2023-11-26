namespace PSI_Project.Models;

public class PomodoroIntensity
{
    public string Mode { get; }
    public int StudyDuration { get; }
    public int ShortBreakDuration { get; }
    public int LongBreakDuration { get; }

    public PomodoroIntensity(string mode, int studyDuration, int shortBreakDuration, int longBreakDuration)
    {
        Mode = mode;
        StudyDuration = studyDuration;
        ShortBreakDuration = shortBreakDuration;
        LongBreakDuration = longBreakDuration;
    }
}