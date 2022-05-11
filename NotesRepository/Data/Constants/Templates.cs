namespace NotesRepository.Data.Constants
{
    public static class Templates
    {
        public static readonly string ToDoTemplate = @"## ToDo List

---
Things that I need to do this weekend:
* ~~do grocery shopping~~
* `pay the bills`
* clean up the garage
    * wash the car
    * wash the bike";


        public static readonly string ToDoWithTableTemplate = @"## ToDo List

---
Things that I need to do this weekend:

|  to do  | doing  |  done |
|----------|----------|---------|
| pay the bills  |  wash the car | do grocery shopping   |   
| wash the bike |  |  |";

        public static Dictionary<string, string> NotesTemplates { get; set; }
            = new Dictionary<string, string>() { { "To-do list", ToDoTemplate }, { "To-do list with table", ToDoWithTableTemplate } };

    }
}
