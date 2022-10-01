using SeleniumTests.Extensions;

namespace SeleniumTests.Infrastructure.Seeders
{
    public class BasicSeedingReport
    {
        public BasicSeedingReport(
            IReadOnlyCollection<Tag> users,
            IReadOnlyCollection<Tag> notes,
            IReadOnlyCollection<Tag> directories,
            IReadOnlyCollection<Tag> events,
            IReadOnlyCollection<Tag> images)
        {
            Users = users;
            Notes = notes;
            Directories = directories;
            Events = events;
            Images = images;
        }

        public IReadOnlyCollection<Tag> Users { get; }
        public IReadOnlyCollection<Tag> Notes { get; }
        public IReadOnlyCollection<Tag> Directories { get; }
        public IReadOnlyCollection<Tag> Events { get; }
        public IReadOnlyCollection<Tag> Images { get; }
    }
}
