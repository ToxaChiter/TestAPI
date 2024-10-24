﻿namespace Application.DTOs;

public class EventDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime EventDateTime { get; set; }
    public string Location { get; set; }
    public string Category { get; set; }
    public int MaxParticipants { get; set; }
    public string ImagePath { get; set; }
}
