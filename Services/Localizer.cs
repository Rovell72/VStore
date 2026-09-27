namespace VStore.Services;

public interface ILocalizer
{
    string Lang { get; }
    string this[string key] { get; }
}

public class Localizer(IHttpContextAccessor accessor) : ILocalizer
{
    public static readonly string[] Langs = { "ru", "uk", "en" };

    public string Lang
    {
        get
        {
            var v = accessor.HttpContext?.Request.Cookies["lang"];
            return Langs.Contains(v) ? v! : "ru";
        }
    }

    public string this[string key]
    {
        get
        {
            if (!Dict.TryGetValue(key, out var row)) return key;
            var idx = Array.IndexOf(Langs, Lang);
            return row[idx < 0 ? 0 : idx];
        }
    }

    static readonly Dictionary<string, string[]> Dict = new()
    {
        ["nav_store"] = new[] { "Обзор", "Огляд", "Store" },
        ["nav_support"] = new[] { "Поддержка", "Підтримка", "Support" },
        ["nav_news"] = new[] { "Новости", "Новини", "News" },
        ["nav_library"] = new[] { "Библиотека", "Бібліотека", "Library" },
        ["nav_admin"] = new[] { "Админка", "Адмінка", "Admin" },
        ["search_placeholder"] = new[] { "Поиск игр", "Пошук ігор", "Search games" },
        ["wishlist"] = new[] { "Список желаемого", "Список бажань", "Wishlist" },
        ["cart"] = new[] { "Корзина", "Кошик", "Cart" },
        ["login"] = new[] { "Войти", "Увійти", "Log in" },
        ["logout"] = new[] { "Выйти", "Вийти", "Log out" },
        ["download"] = new[] { "Скачать", "Завантажити", "Download" },
        ["profile"] = new[] { "Профиль", "Профіль", "Profile" },
        ["all_genres"] = new[] { "Все", "Всі", "All" },
        ["home_title"] = new[] { "Рекомендуемое и популярное", "Рекомендоване й популярне", "Featured and popular" },
        ["home_specials"] = new[] { "Специальные предложения", "Спеціальні пропозиції", "Special offers" },
        ["home_newreleases"] = new[] { "Новинки", "Новинки", "New releases" },
        ["home_free"] = new[] { "Бесплатные игры", "Безкоштовні ігри", "Free games" },
        ["home_topsellers"] = new[] { "Хиты продаж", "Хіти продажів", "Top sellers" },
        ["home_search_results"] = new[] { "Результаты поиска", "Результати пошуку", "Search results" },
        ["home_nothing_found"] = new[] { "Ничего не найдено. Попробуйте изменить запрос.", "Нічого не знайдено. Спробуйте змінити запит.", "Nothing found. Try a different search." },
        ["game_about"] = new[] { "Об игре", "Про гру", "About the game" },
        ["game_reqs"] = new[] { "Системные требования", "Системні вимоги", "System requirements" },
        ["game_reqs_min"] = new[] { "Минимальные", "Мінімальні", "Minimum" },
        ["game_reqs_rec"] = new[] { "Рекомендуемые", "Рекомендовані", "Recommended" },
        ["game_achievements"] = new[] { "Достижения", "Досягнення", "Achievements" },
        ["game_reviews"] = new[] { "Отзывы покупателей", "Відгуки покупців", "Customer reviews" },
        ["game_add_to_cart"] = new[] { "В корзину", "У кошик", "Add to cart" },
        ["game_go_to_cart"] = new[] { "Перейти в корзину", "Перейти до кошика", "Go to cart" },
        ["game_already_in_library"] = new[] { "Уже в библиотеке", "Вже в бібліотеці", "Already owned" },
        ["game_login_to_buy"] = new[] { "Войдите, чтобы купить", "Увійдіть, щоб купити", "Log in to buy" },
        ["game_add_wishlist"] = new[] { "В желаемое", "До бажаного", "Add to wishlist" },
        ["game_remove_wishlist"] = new[] { "Убрать из желаемого", "Прибрати з бажаного", "Remove from wishlist" },
        ["game_recommend"] = new[] { "Рекомендую", "Рекомендую", "Recommend" },
        ["game_not_recommend"] = new[] { "Не рекомендую", "Не рекомендую", "Not recommended" },
        ["game_publish"] = new[] { "Опубликовать", "Опублікувати", "Publish" },
        ["account_login_title"] = new[] { "Вход", "Вхід", "Log in" },
        ["account_login_label"] = new[] { "Войти с именем аккаунта или почтой", "Увійти за іменем акаунта або поштою", "Sign in with account name or email" },
        ["account_password_label"] = new[] { "Пароль", "Пароль", "Password" },
        ["account_remember"] = new[] { "Запомнить меня", "Запам'ятати мене", "Remember me" },
        ["account_login_btn"] = new[] { "Войти", "Увійти", "Log in" },
        ["account_no_account"] = new[] { "Нет аккаунта?", "Немає акаунта?", "No account?" },
        ["account_create_new"] = new[] { "Создать новый", "Створити новий", "Create one" },
        ["account_forgot"] = new[] { "Забыли пароль?", "Забули пароль?", "Forgot password?" },
        ["account_or"] = new[] { "или", "або", "or" },
        ["account_google"] = new[] { "Войти через Google", "Увійти через Google", "Continue with Google" },
        ["account_register_title"] = new[] { "Создать аккаунт", "Створити акаунт", "Create account" },
        ["account_nickname_label"] = new[] { "Игровое имя", "Ігрове ім'я", "Nickname" },
        ["account_email_label"] = new[] { "Электронная почта", "Електронна пошта", "Email" },
        ["account_accept_terms"] = new[] { "Мне исполнилось 13 лет, и я принимаю условия соглашения подписчика и политику конфиденциальности", "Мені виповнилося 13 років, і я приймаю умови угоди та політику конфіденційності", "I am 13 or older and accept the subscriber agreement and privacy policy" },
        ["account_register_btn"] = new[] { "Создать аккаунт", "Створити акаунт", "Create account" },
        ["account_have_account"] = new[] { "Уже есть аккаунт?", "Вже є акаунт?", "Already have an account?" },
        ["account_created_title"] = new[] { "Аккаунт создан!", "Акаунт створено!", "Account created!" },
        ["account_created_text"] = new[] { "Аккаунт был успешно создан.", "Акаунт успішно створено.", "Your account has been created." },
        ["forgot_title"] = new[] { "Восстановление пароля", "Відновлення пароля", "Reset password" },
        ["forgot_text"] = new[] { "Укажите почту, привязанную к аккаунту, и мы сформируем ссылку для сброса пароля.", "Вкажіть пошту, прив'язану до акаунта, і ми сформуємо посилання для скидання пароля.", "Enter the email linked to your account and we'll generate a reset link." },
        ["forgot_send"] = new[] { "Получить ссылку", "Отримати посилання", "Get reset link" },
        ["forgot_back"] = new[] { "Вернуться ко входу", "Повернутися до входу", "Back to login" },
        ["forgot_sent_title"] = new[] { "Ссылка сформирована", "Посилання сформовано", "Reset link ready" },
        ["forgot_sent_text"] = new[] { "Отправка почты в проекте не настроена, поэтому ссылка для сброса пароля показана ниже.", "Надсилання пошти в проєкті не налаштоване, тому посилання для скидання пароля показано нижче.", "Email sending isn't configured in this project, so the reset link is shown below." },
        ["reset_title"] = new[] { "Новый пароль", "Новий пароль", "New password" },
        ["reset_new_password"] = new[] { "Новый пароль", "Новий пароль", "New password" },
        ["reset_save"] = new[] { "Сохранить пароль", "Зберегти пароль", "Save password" },
        ["reset_invalid"] = new[] { "Ссылка недействительна или устарела.", "Посилання недійсне або застаріле.", "This link is invalid or has expired." },
        ["reset_done_title"] = new[] { "Пароль изменён", "Пароль змінено", "Password changed" },
        ["reset_done_text"] = new[] { "Теперь вы можете войти с новым паролем.", "Тепер ви можете увійти з новим паролем.", "You can now log in with your new password." },
        ["profile_level"] = new[] { "Уровень", "Рівень", "Level" },
        ["profile_games"] = new[] { "Игры", "Ігри", "Games" },
        ["profile_achievements"] = new[] { "Достижения", "Досягнення", "Achievements" },
        ["profile_show_all"] = new[] { "Показать все", "Показати всі", "Show all" },
        ["profile_collection"] = new[] { "Коллекция игр", "Колекція ігор", "Game collection" },
        ["profile_empty"] = new[] { "Библиотека пуста. Загляните в магазин.", "Бібліотека порожня. Загляньте до магазину.", "Your library is empty. Check out the store." },
        ["library_title"] = new[] { "Библиотека игр", "Бібліотека ігор", "Game library" },
        ["library_search"] = new[] { "Поиск по библиотеке", "Пошук у бібліотеці", "Search your library" },
        ["library_empty"] = new[] { "У вас пока нет игр.", "У вас поки немає ігор.", "You don't own any games yet." },
        ["library_hours"] = new[] { "Наиграно", "Награно", "Playtime" },
        ["library_play"] = new[] { "Играть", "Грати", "Play" },
        ["cart_title"] = new[] { "Моя корзина", "Мій кошик", "My cart" },
        ["cart_empty"] = new[] { "Корзина пуста.", "Кошик порожній.", "Your cart is empty." },
        ["cart_go_store"] = new[] { "Перейти в магазин", "Перейти до магазину", "Go to store" },
        ["cart_remove"] = new[] { "Удалить", "Видалити", "Remove" },
        ["cart_to_wishlist"] = new[] { "В желаемое", "До бажаного", "Move to wishlist" },
        ["cart_total"] = new[] { "Итого", "Разом", "Total" },
        ["cart_checkout"] = new[] { "Оформить заказ", "Оформити замовлення", "Checkout" },
        ["checkout_title"] = new[] { "Оплата", "Оплата", "Payment" },
        ["checkout_card"] = new[] { "Банковская карта", "Банківська картка", "Bank card" },
        ["checkout_wallet"] = new[] { "Кошелёк V", "Гаманець V", "V Wallet" },
        ["checkout_card_number"] = new[] { "Номер карты", "Номер картки", "Card number" },
        ["checkout_holder"] = new[] { "Имя владельца", "Ім'я власника", "Cardholder name" },
        ["checkout_expiry"] = new[] { "Срок действия", "Термін дії", "Expiry date" },
        ["checkout_cvv"] = new[] { "CVV", "CVV", "CVV" },
        ["checkout_pay"] = new[] { "Оплатить", "Оплатити", "Pay" },
        ["checkout_order"] = new[] { "Ваш заказ", "Ваше замовлення", "Your order" },
        ["success_title"] = new[] { "Спасибо за покупку!", "Дякуємо за покупку!", "Thanks for your purchase!" },
        ["success_text"] = new[] { "Игры добавлены в вашу библиотеку.", "Ігри додано до вашої бібліотеки.", "The games were added to your library." },
        ["success_go_profile"] = new[] { "Перейти в профиль", "Перейти до профілю", "Go to profile" },
        ["wishlist_title"] = new[] { "Мой список желаемого", "Мій список бажань", "My wishlist" },
        ["wishlist_empty"] = new[] { "Список пуст.", "Список порожній.", "Your wishlist is empty." },
        ["wishlist_added"] = new[] { "Добавлено", "Додано", "Added" },
        ["wishlist_in_cart"] = new[] { "В корзине", "У кошику", "In cart" },
        ["wishlist_to_cart"] = new[] { "В корзину", "У кошик", "Add to cart" },
        ["news_title"] = new[] { "Актуальные новости", "Актуальні новини", "Latest news" },
        ["support_title"] = new[] { "Поддержка", "Підтримка", "Support" },
        ["support_search"] = new[] { "Найти помощь", "Знайти допомогу", "Search help" },
        ["support_questions_left"] = new[] { "Остались вопросы?", "Залишилися питання?", "Still have questions?" },
        ["support_contact_email"] = new[] { "Написать нам на почту", "Написати нам на пошту", "Email us" },
        ["support_form_title"] = new[] { "Написать в поддержку", "Написати до підтримки", "Contact support" },
        ["support_name"] = new[] { "Имя", "Ім'я", "Name" },
        ["support_email"] = new[] { "Почта для ответа", "Пошта для відповіді", "Reply email" },
        ["support_subject"] = new[] { "Тема обращения", "Тема звернення", "Subject" },
        ["support_message"] = new[] { "Сообщение", "Повідомлення", "Message" },
        ["support_send"] = new[] { "Отправить обращение", "Надіслати звернення", "Send ticket" },
        ["support_sent"] = new[] { "Обращение отправлено. Мы ответим вам на почту.", "Звернення надіслано. Ми відповімо на пошту.", "Ticket sent. We'll reply by email." },
        ["support_reply"] = new[] { "Ответ поддержки", "Відповідь підтримки", "Support reply" },
        ["theme_toggle"] = new[] { "Тема", "Тема", "Theme" },
        ["theme_dark"] = new[] { "Тёмная", "Темна", "Dark" },
        ["theme_light"] = new[] { "Светлая", "Світла", "Light" },
        ["admin_dashboard"] = new[] { "Панель администратора", "Панель адміністратора", "Admin dashboard" },
        ["admin_games"] = new[] { "Игры", "Ігри", "Games" },
        ["admin_users"] = new[] { "Пользователи", "Користувачі", "Users" },
        ["admin_tickets"] = new[] { "Обращения", "Звернення", "Tickets" },
        ["admin_add_game"] = new[] { "Добавить игру", "Додати гру", "Add game" },
        ["admin_edit"] = new[] { "Редактировать", "Редагувати", "Edit" },
        ["admin_delete"] = new[] { "Удалить", "Видалити", "Delete" },
        ["admin_save"] = new[] { "Сохранить", "Зберегти", "Save" },
        ["admin_block"] = new[] { "Заблокировать", "Заблокувати", "Block" },
        ["admin_unblock"] = new[] { "Разблокировать", "Розблокувати", "Unblock" },
        ["admin_reply"] = new[] { "Ответить", "Відповісти", "Reply" },
        ["admin_send_reply"] = new[] { "Отправить ответ", "Надіслати відповідь", "Send reply" },
        ["admin_no_tickets"] = new[] { "Обращений пока нет.", "Звернень поки немає.", "No tickets yet." },
        ["back"] = new[] { "Назад", "Назад", "Back" },
    };
}
