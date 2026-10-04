using Microsoft.AspNetCore.Mvc.Filters;

namespace VStore.Services;

public sealed class LocalizedValidationFilter(ILocalizer localizer) : IAsyncActionFilter
{
    private static readonly IReadOnlyDictionary<string, string> Keys = new Dictionary<string, string>
    {
        ["Введите игровое имя"] = "validation_nickname_required",
        ["Имя должно содержать от 3 до 30 символов"] = "validation_nickname_length",
        ["Введите почту"] = "validation_email_required",
        ["Некорректный адрес почты"] = "validation_email_invalid",
        ["Придумайте пароль"] = "validation_password_required",
        ["Пароль должен содержать минимум 6 символов"] = "password_too_short",
        ["Необходимо принять условия соглашения"] = "validation_terms_required",
        ["Введите имя аккаунта или почту"] = "validation_login_required",
        ["Введите пароль"] = "validation_password_required",
        ["Введите название"] = "validation_title_required",
        ["Введите описание"] = "validation_description_required",
        ["Введите разработчика"] = "validation_developer_required",
        ["Введите издателя"] = "validation_publisher_required",
        ["Введите жанр"] = "validation_genre_required",
        ["Укажите дату выхода"] = "validation_release_date_required",
        ["Некорректная цена"] = "validation_price_invalid",
        ["Скидка от 0 до 100%"] = "validation_discount_invalid",
        ["Некорректный возрастной рейтинг"] = "validation_age_rating_invalid",
        ["Введите имя"] = "validation_name_required",
        ["Введите тему обращения"] = "validation_subject_required",
        ["Введите сообщение"] = "validation_message_required"
    };

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var entry in context.ModelState.Values)
        {
            var translated = entry.Errors.Select(error =>
            {
                var message = error.ErrorMessage;
                return Keys.TryGetValue(message, out var key) ? localizer[key] : message;
            }).ToArray();

            if (translated.Where((message, index) => message != entry.Errors[index].ErrorMessage).Any())
            {
                entry.Errors.Clear();
                foreach (var message in translated)
                    entry.Errors.Add(message);
            }
        }

        await next();
    }
}
