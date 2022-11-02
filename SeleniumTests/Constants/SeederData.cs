namespace SeleniumTests.Constants;

public static class SeederData
{
    public static string NoteTitle(int index) => $"DefaultNote{index}";
    public static string EventContent(int index) => $"DefaultEvent{index}";
    public const string password = "Password123!";
    public const string sharedNoteTitle = "Shared";
    public const string sharedNoteContent = "This note was shared!";
}
