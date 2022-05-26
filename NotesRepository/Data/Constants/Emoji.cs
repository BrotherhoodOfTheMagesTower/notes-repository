namespace NotesRepository.Data.Constants
{
    public static class Emoji
    {
        public static readonly string[] emoji = {   "⌚️", "📱", "📲", "💻", "📞", "☎️",
                                                    "📟", "📠", "📺","📻","🎙","🎚",
                                                    "🎛", "🧭", "⏱","⏲","⏰","🕰",
                                                    "📜", "📃", "📄","📑","🧾", "📊",
                                                    "📈", "📉", "🗒","🗑","📇", "🗃",
                                                    "🗳", "🗄", "📋","📁", "📂","🗂",
                                                    "🗞", "📰", "📓","📔","📒", "📕",
                                                    "📗", "📘", "📙","📚", "📖","🔖",
                                                    "🧷", "🔗", "📎","🖇", "📐","📏",
                                                    "🧮", "📌", "📍","✂️", "🖊","🖋",
                                                    "✒️", "🖌 ","🖍","📝","✏️", "🔍",
                                                    "🔎", "🔏", "🔐","🔒","🔓"};

        public static string getRandomEmoji() => emoji[new Random().Next(0, emoji.Length - 1)];
    }
}
