namespace SeleniumTests.Extensions;

public class Tag
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public Tag(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
