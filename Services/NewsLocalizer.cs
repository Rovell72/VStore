namespace VStore.Services;

public interface INewsLocalizer
{
    string Title(NewsItem item);
    string Text(NewsItem item);
}

public sealed class NewsLocalizer(ILocalizer localizer) : INewsLocalizer
{
    private static readonly Dictionary<string, (string[] Titles, string[] Texts)> Translations = new()
    {
        ["Большая осенняя распродажа стартовала"] = (
            ["Стартував великий осінній розпродаж", "The Big Autumn Sale Is Here"],
            ["Тисячі ігор зі знижками до 80% — встигніть поповнити бібліотеку до кінця тижня.", "Thousands of games are up to 80% off. Add to your library before the sale ends this week."]),
        ["Анонсирован новый сезон Арены Героев"] = (
            ["Анонсовано новий сезон Арени героїв", "New Heroes Arena Season Announced"],
            ["Три нові герої, оновлена мапа та рейтингові нагороди чекатимуть на вас уже наступного місяця.", "Three new heroes, an updated map, and ranked rewards arrive next month."]),
        ["Итоги фестиваля независимых игр"] = (
            ["Підсумки фестивалю незалежних ігор", "Indie Games Festival Results"],
            ["Оголошено переможців у десяти номінаціях. Серед них — «Тиха ферма» та «Підземелля глибин».", "Winners in ten categories have been announced, including Quiet Farm and Dungeons of the Deep."]),
        ["Обновление клиента V Store"] = (
            ["Оновлення клієнта V Store", "V Store Client Update"],
            ["Ми пришвидшили завантаження ігор, додали темну тему та покращили пошук у каталозі.", "We've made game downloads faster, added a dark theme, and improved catalog search."]),
        ["Разработчики «Последнего рассвета» показали дополнение"] = (
            ["Розробники «Останнього світанку» показали доповнення", "The Last Dawn Expansion Revealed"],
            ["Нова сюжетна лінія та кооперативний режим вийдуть на початку наступного року.", "A new story and co-op mode are coming early next year."]),
        ["Кибер Гонка 2099 получила крупный патч"] = (
            ["Кіберперегони 2099 отримали велике оновлення", "Cyber Race 2099 Gets a Major Patch"],
            ["Виправлено помилки, додано нові траси та покращено фізику керування.", "The patch fixes bugs, adds new tracks, and improves driving physics."])
    };

    private int LanguageIndex => localizer.Lang switch { "uk" => 0, "en" => 1, _ => -1 };

    public string Title(NewsItem item) => Translate(item, title: true);
    public string Text(NewsItem item) => Translate(item, title: false);

    private string Translate(NewsItem item, bool title)
    {
        var index = LanguageIndex;
        if (index < 0 || !Translations.TryGetValue(item.Title, out var translation))
            return title ? item.Title : item.Text;
        return title ? translation.Titles[index] : translation.Texts[index];
    }
}
