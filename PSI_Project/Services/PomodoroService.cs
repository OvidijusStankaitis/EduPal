using PSI_Project.Models;

namespace PSI_Project.Services;

public class PomodoroService
{
    private readonly Dictionary<string, PomodoroSession> _sessions = new();

    public void StartTimer(string userId, string intensity)
    {
        if (_sessions.TryGetValue(userId, out var session))
        {
            session.Start(intensity);
        }
        else
        {
            session = new PomodoroSession(userId, intensity);
            _sessions.Add(userId, session);
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

    public (int RemainingTime, string Mode, bool IsActive) GetTimerState(string userId)
    {
        if (_sessions.TryGetValue(userId, out var session))
        {
            return session.GetState();
        }

        return (0, "Inactive", false);
    }
}