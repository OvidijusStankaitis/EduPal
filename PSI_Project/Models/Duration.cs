﻿namespace PSI_Project.Models;

public struct Duration
{
    public int Hours { get; set; }
    public int Minutes { get; set; }

    public static Duration ToReadableTime(double totalHours)
    {
        int totalMinutes = (int)(totalHours * 60);
        return new Duration
        {
            Hours = totalMinutes / 60,
            Minutes = totalMinutes % 60
        };
    }

    public override string ToString()
    {
        return $"{Hours} hours and {Minutes} minutes";
    }
}