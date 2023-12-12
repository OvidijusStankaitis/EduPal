namespace PSI_Project.Models;

public class PomodoroSession
{
    public string UserId { get; private set; }
    public bool IsActive { get; private set; }
    private DateTime _startTime;
    private PomodoroIntensity _currentIntensity;
    private int _phaseCounter = 0; // Keeps track of completed study/break phases
    private string _currentMode; // Current mode: "Study", "Short Break", "Long Break"

    public PomodoroSession(string userId, string intensity)
    {
        UserId = userId;
        SetIntensity(intensity);
        IsActive = false;
        _currentMode = "Study"; // Start with Study mode
    }

    public void Start(string intensity)
    {
        if (!IsActive)
        {
            SetIntensity(intensity);
            _startTime = DateTime.Now;
            IsActive = true;
            _currentMode = "Study"; // Reset to Study mode when timer starts
            _phaseCounter = 0;
        }
    }

    public void Stop()
    {
        IsActive = false;
    }

    public (int RemainingTime, string Mode, bool IsActive) GetState()
    {
        if (!IsActive)
        {
            return (0, "Inactive", false);
        }

        UpdateMode();

        var elapsedTime = (int)(DateTime.Now - _startTime).TotalSeconds;
        int duration = GetDurationForCurrentMode();
        var remainingTime = Math.Max(0, duration * 60 - elapsedTime);

        return (remainingTime, _currentMode, IsActive);
    }

    private void SetIntensity(string intensity)
    {
        _currentIntensity = intensity switch
        {
            "Low" => new PomodoroIntensity("Study", 1, 1, 1),
            "Medium" => new PomodoroIntensity("Study", 25, 5, 15),
            "High" => new PomodoroIntensity("Study", 30, 5, 20),
            _ => throw new ArgumentException("Invalid intensity level.")
        };
    }

    private int GetDurationForCurrentMode()
    {
        return _currentMode switch
        {
            "Study" => _currentIntensity.StudyDuration,
            "Short Break" => _currentIntensity.ShortBreakDuration,
            "Long Break" => _currentIntensity.LongBreakDuration,
            _ => 0
        };
    }

    private void UpdateMode()
    {
        var elapsedTime = (int)(DateTime.Now - _startTime).TotalSeconds;
        int duration = GetDurationForCurrentMode() * 60;

        if (elapsedTime >= duration)
        {
            _phaseCounter++;
            _startTime = DateTime.Now; // Reset start time for the next phase

            // Reset the counter after completing one full cycle
            if (_phaseCounter >= 6) 
            {
                _phaseCounter = 0;
            }

            if (_currentMode == "Study")
            {
                // Switch to "Long Break" after the 5th phase (3rd Study session)
                if (_phaseCounter == 5)
                {
                    _currentMode = "Long Break";
                }
                else
                {
                    _currentMode = "Short Break";
                }
            }
            else // Current mode is either "Short Break" or "Long Break"
            {
                _currentMode = "Study";
            }
        }
    }
}
