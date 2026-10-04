namespace VStore.Services;

public interface IGameLocalizer
{
    string Title(Game game);
    string Description(Game game);
    string Genre(Game game);
    string Developer(Game game);
    string Publisher(Game game);
    string Requirements(string value);
    string AchievementTitle(Achievement achievement);
    string AchievementDescription(Achievement achievement);
}

public sealed class GameLocalizer(ILocalizer localizer) : IGameLocalizer
{
    private static readonly Dictionary<string, string[]> Titles = new()
    {
        ["Последний рассвет"] = ["Останній світанок", "The Last Dawn"],
        ["Хроники Нефрита"] = ["Хроніки Нефриту", "Jade Chronicles"],
        ["Тень Самурая"] = ["Тінь самурая", "Shadow of the Samurai"],
        ["Кибер Гонка 2099"] = ["Кіберперегони 2099", "Cyber Race 2099"],
        ["Королевство Пепла"] = ["Королівство попелу", "Kingdom of Ash"],
        ["Звёздный Дальнобойщик"] = ["Зоряний далекобійник", "Star Hauler"],
        ["Подземелья Глубин"] = ["Підземелля глибин", "Dungeons of the Deep"],
        ["Тихая Ферма"] = ["Тиха ферма", "Quiet Farm"],
        ["Стальной Рубеж"] = ["Сталевий рубіж", "Steel Frontier"],
        ["Лига Карточных Чемпионов"] = ["Ліга карткових чемпіонів", "Card Champions League"],
        ["Осада Крепости"] = ["Облога фортеці", "Fortress Siege"],
        ["Пиксельные Рогалики"] = ["Піксельні рогалики", "Pixel Roguelikes"],
        ["Арена Героев"] = ["Арена героїв", "Heroes Arena"],
        ["Тайна Старого Маяка"] = ["Таємниця старого маяка", "The Old Lighthouse Mystery"]
    };

    private static readonly Dictionary<string, string[]> Descriptions = new()
    {
        ["Последний рассвет"] = ["Виживання у світі після катастрофи. Досліджуйте руїни міст, збирайте ресурси та боріться за останній світанок людства.", "Survive in a world after a catastrophe. Explore city ruins, gather resources, and fight for humanity's last dawn."],
        ["Хроники Нефрита"] = ["Величезна рольова гра з відкритим світом, глибокою системою розвитку та рішеннями, що змінюють долю королівства.", "A vast open-world RPG with deep character progression and choices that shape the kingdom's fate."],
        ["Тень Самурая"] = ["Покрокова тактика у феодальній Японії. Змінюйте стилі атаки, влаштовуйте засідки та ведіть свій клан до перемоги.", "Turn-based tactics in feudal Japan. Switch attack styles, set ambushes, and lead your clan to victory."],
        ["Кибер Гонка 2099"] = ["Швидкісні перегони неоновими мегаполісами майбутнього з налаштуванням болідів і мережевими турнірами.", "High-speed races through neon cities of the future, with customizable cars and online tournaments."],
        ["Королевство Пепла"] = ["Глобальна стратегія: розвивайте економіку, укладайте союзи та завойовуйте континенти.", "A grand strategy game: grow your economy, forge alliances, and conquer continents."],
        ["Звёздный Дальнобойщик"] = ["Затишний симулятор космічних вантажних перевезень між станціями галактики.", "A relaxing space-hauling simulator delivering cargo between stations across the galaxy."],
        ["Подземелья Глубин"] = ["Динамічний рогалик із процедурно згенерованими рівнями та сотнями артефактів.", "A fast-paced roguelike with procedurally generated levels and hundreds of artifacts."],
        ["Тихая Ферма"] = ["Затишна гра про фермерське життя: вирощуйте врожай, доглядайте тварин і товаришуйте із сусідами.", "A cozy farming game: grow crops, raise animals, and become friends with your neighbors."],
        ["Стальной Рубеж"] = ["Тактичний шутер із командним режимом, руйнованим оточенням і реалістичною балістикою.", "A tactical shooter with team play, destructible environments, and realistic ballistics."],
        ["Лига Карточных Чемпионов"] = ["Колекційна карткова гра з рейтинговими матчами та щомісячними сезонами.", "A collectible card game with ranked matches and monthly seasons."],
        ["Осада Крепости"] = ["Будуйте оборону, командуйте військами та відбивайте хвилі ворогів у реальному часі.", "Build your defenses, command troops, and repel waves of enemies in real time."],
        ["Пиксельные Рогалики"] = ["Збірка з десяти коротких рогаликів в єдиному піксельному стилі.", "A collection of ten bite-sized roguelikes in a cohesive pixel-art style."],
        ["Арена Героев"] = ["Безкоштовна командна арена: оберіть героя, зберіть загін і бийтеся за трон.", "A free team arena: choose a hero, form a squad, and battle for the throne."],
        ["Тайна Старого Маяка"] = ["Детективна пригода про загадкове зникнення доглядача маяка.", "A mystery adventure about the strange disappearance of a lighthouse keeper."]
    };

    private static readonly Dictionary<string, string[]> Genres = new()
    {
        ["Экшен"] = ["Екшн", "Action"], ["Гонки"] = ["Перегони", "Racing"],
        ["Стратегия"] = ["Стратегія", "Strategy"], ["Симулятор"] = ["Симулятор", "Simulation"],
        ["Рогалик"] = ["Рогалик", "Roguelike"], ["Шутер"] = ["Шутер", "Shooter"],
        ["Приключения"] = ["Пригоди", "Adventure"]
    };

    private static readonly Dictionary<string, string[]> Developers = new()
    {
        ["Северный Ветер"] = ["Північний Вітер", "North Wind"], ["Нефритовая Студия"] = ["Нефритова Студія", "Jade Studio"],
        ["Кагэ Геймс"] = ["Каге Ґеймз", "Kage Games"], ["Нео Моторс"] = ["Нео Моторс", "Neo Motors"],
        ["Пепельный Трон"] = ["Попелястий Трон", "Ashen Throne"], ["Орбита Софт"] = ["Орбіта Софт", "Orbit Soft"],
        ["Пиксель Форж"] = ["Піксель Фордж", "Pixel Forge"], ["Зелёный Луг"] = ["Зелений Луг", "Green Meadow"],
        ["Бронзовая Башня"] = ["Бронзова Вежа", "Bronze Tower"], ["Колода Студио"] = ["Колода Студіо", "Deck Studio"],
        ["Каменная Стена"] = ["Кам'яна Стіна", "Stonewall Studio"], ["Героика"] = ["Героїка", "Heroica"],
        ["Морская Тень"] = ["Морська Тінь", "Sea Shadow"]
    };

    private static readonly string[][] AchievementTitles =
    [
        ["Перші кроки", "First Steps"], ["Досвідчений воїн", "Seasoned Warrior"],
        ["Дослідник", "Explorer"], ["Легенда", "Legend"]
    ];
    private static readonly string[][] AchievementDescriptions =
    [
        ["Завершіть навчання", "Complete the tutorial"], ["Переможіть 100 супротивників", "Defeat 100 enemies"],
        ["Відкрийте всі локації", "Discover all locations"], ["Пройдіть гру на найвищій складності", "Finish the game on the highest difficulty"]
    ];

    private int LanguageIndex => localizer.Lang switch { "uk" => 0, "en" => 1, _ => -1 };

    private string Translate(Dictionary<string, string[]> entries, string value)
    {
        var index = LanguageIndex;
        return index < 0 || !entries.TryGetValue(value, out var translations) ? value : translations[index];
    }

    public string Title(Game game) => Translate(Titles, game.Title);
    public string Description(Game game) => Translate(Descriptions, game.Title) is var value && value != game.Title ? value : game.Description;
    public string Genre(Game game) => Translate(Genres, game.Genre);
    public string Developer(Game game) => Translate(Developers, game.Developer);
    public string Publisher(Game game) => Translate(Developers, game.Publisher);

    public string Requirements(string value)
    {
        if (localizer.Lang == "ru") return value;
        var labels = localizer.Lang == "uk"
            ? new[] { ("ОС:", "ОС:"), ("Процессор:", "Процесор:"), ("Память:", "Пам'ять:"), ("Видеокарта:", "Відеокарта:"), ("Место на диске:", "Місце на диску:") }
            : new[] { ("ОС:", "OS:"), ("Процессор:", "Processor:"), ("Память:", "Memory:"), ("Видеокарта:", "Graphics:"), ("Место на диске:", "Storage:") };
        foreach (var (source, target) in labels) value = value.Replace(source, target, StringComparison.Ordinal);
        return value;
    }

    public string AchievementTitle(Achievement achievement) => TranslateAchievement(achievement.Title, AchievementTitles);
    public string AchievementDescription(Achievement achievement) => TranslateAchievement(achievement.Title, AchievementDescriptions, achievement.Description);

    private string TranslateAchievement(string source, string[][] entries, string? fallback = null)
    {
        var index = LanguageIndex;
        if (index < 0) return fallback ?? source;
        var achievementIndex = source switch { "Первые шаги" => 0, "Опытный воин" => 1, "Исследователь" => 2, "Легенда" => 3, _ => -1 };
        return achievementIndex < 0 ? fallback ?? source : entries[achievementIndex][index];
    }
}
