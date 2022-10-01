namespace SeleniumTests.Infrastructure.Seeders;

public class BasicSeedingTask
{
    public BasicSeedingTask(int accountsCount, int notesPerAccountCount, int directoriesPerAccountCount, int eventsPerAccountCount,
        int imagesPerAccountCount, bool createCollaborators)
    {
        AccountsCount = accountsCount;
        NotesPerAccountCount = notesPerAccountCount;
        DirectoriesPerAccountCount = directoriesPerAccountCount;
        EventsPerAccountCount = eventsPerAccountCount;
        ImagesPerAccountCount = imagesPerAccountCount;
        CreateCollaborators = createCollaborators;
    }

    public int AccountsCount { get; set; }
    public int NotesPerAccountCount { get; set; }
    public int DirectoriesPerAccountCount { get; set; }
    public int EventsPerAccountCount { get; set; }
    public int ImagesPerAccountCount { get; set; }
    public bool CreateCollaborators { get; set; }
}
