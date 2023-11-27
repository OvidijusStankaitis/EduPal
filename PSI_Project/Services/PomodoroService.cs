using PSI_Project.Models;

namespace PSI_Project.Services;

public class PomodoroService
{
    private readonly Dictionary<string, PomodoroSession> _sessions = new();

    public void StartTimer(string userEmail, string intensity)
    {
        if (_sessions.TryGetValue(userEmail, out var session))
        {
            session.Start(intensity);
        }
        else
        {
            session = new PomodoroSession(userEmail, intensity);
            _sessions.Add(userEmail, session);
            session.Start(intensity);
        }
    }

    public void StopTimer(string userEmail)
    {
        if (_sessions.TryGetValue(userEmail, out var session))
        {
            session.Stop();
        }
    }

    public (int RemainingTime, string Mode, bool IsActive) GetTimerState(string userEmail)
    {
        if (_sessions.TryGetValue(userEmail, out var session))
        {
            return session.GetState();
        }

        return (0, "Inactive", false);
    }
}