namespace LiteRP.WebApp.ViewModels;

public class ChatSummary
{
    public int Id { get; set; }
    public required string CharacterName { get; set; }
    public required string AvatarUrl { get; set; }
    public required string LastMessage { get; set; }
    public required string Timestamp { get; set; }
}