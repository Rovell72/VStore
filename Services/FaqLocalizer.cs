namespace VStore.Services;

public interface IFaqLocalizer
{
    string Question(FaqItem item);
    string Answer(FaqItem item);
}

public sealed class FaqLocalizer(ILocalizer localizer) : IFaqLocalizer
{
    private static readonly Dictionary<string, (string[] Questions, string[] Answers)> Translations = new()
    {
        ["Проблемы с играми"] = (
            ["Проблеми з іграми", "Game issues"],
            ["Якщо гра не запускається, перевірте системні вимоги, оновіть драйвери відеокарти та перевірте цілісність файлів гри у бібліотеці.", "If a game won't launch, check its system requirements, update your graphics drivers, and verify the game files in your library."]),
        ["Возврат средств"] = (
            ["Повернення коштів", "Refunds"],
            ["Ви можете повернути гру протягом 14 днів після купівлі, якщо награли в неї менше двох годин. Зверніться до підтримки та вкажіть номер замовлення.", "You can request a refund within 14 days of purchase if you've played for less than two hours. Contact support with your order number."]),
        ["Мой аккаунт"] = (
            ["Мій акаунт", "My account"],
            ["Змінити пароль, пошту та ігрове ім'я можна в налаштуваннях профілю. Якщо ви втратили доступ, скористайтеся відновленням пароля електронною поштою.", "You can change your password, email, and nickname in your profile settings. If you've lost access, use email password recovery."]),
        ["Клиент"] = (
            ["Клієнт", "Client"],
            ["Якщо клієнт V Store не запускається, перевстановіть його, тимчасово вимкніть антивірус під час встановлення та запустіть від імені адміністратора.", "If the V Store client won't launch, reinstall it, temporarily disable your antivirus during installation, and run it as an administrator."]),
        ["Проблемы сообщества"] = (
            ["Проблеми спільноти", "Community issues"],
            ["Щоб поскаржитися на користувача або відгук, натисніть кнопку «Поскаржитися» біля повідомлення. Модератори розглянуть звернення протягом 24 годин.", "To report a user or review, select the Report button next to the post. Moderators will review it within 24 hours."]),
        ["Проблемы с устройством"] = (
            ["Проблеми з пристроєм", "Device issues"],
            ["Переконайтеся, що операційну систему й драйвери оновлено, а на диску достатньо вільного місця для встановлення гри.", "Make sure your operating system and drivers are up to date and that you have enough free disk space to install the game."]),
        ["Подарки"] = (
            ["Подарунки", "Gifts"],
            ["Ви можете подарувати гру другу на його адресу електронної пошти. Подарунок з'явиться в його бібліотеці після прийняття.", "You can gift a game to a friend using their email address. It will appear in their library once they accept it."]),
        ["Часто задаваемые вопросы"] = (
            ["Поширені запитання", "Frequently asked questions"],
            ["Відповіді на популярні запитання про платежі, знижки, регіональні ціни та подарункові картки зібрано в цьому розділі.", "Find answers to common questions about payments, discounts, regional pricing, and gift cards in this section."])
    };

    private int LanguageIndex => localizer.Lang switch { "uk" => 0, "en" => 1, _ => -1 };

    public string Question(FaqItem item) => Translate(item, question: true);
    public string Answer(FaqItem item) => Translate(item, question: false);

    private string Translate(FaqItem item, bool question)
    {
        var index = LanguageIndex;
        if (index < 0 || !Translations.TryGetValue(item.Question, out var translation))
            return question ? item.Question : item.Answer;
        return question ? translation.Questions[index] : translation.Answers[index];
    }
}
