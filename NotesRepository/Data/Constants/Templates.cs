namespace NotesRepository.Data.Constants
{
    public class Templates
    {
        public static readonly string ToDoTemplate = @"## ToDo List

---
Things that I need to do this weekend:
* ~~do grocery shopping~~
* `pay the bills`
* clean up the garage
    * wash the car
    * wash the bike";

            
        public static readonly string ToDoWithColumnTemplate = @"# ToDo List
~~Przekreślenie~~tekstu

---
lub
___

> Akapit? Czy coś innego Xd

[Mój Github] (https://github.com/jacek13)

> Lista:
* element 1
* element 2
* element 3
    * dzieli sie na pod element
    * podelement prim

> Lista numerowana:
1. element 1
2. element 2
3. element 3

`Jakis tekst`

![zdjecie] (https://avatars.githubusercontent.com/u/56163434?s=400&u=b60309cd30e98cc4c84079a406350c6d4b6f9c20&v=4)";
    }
}
