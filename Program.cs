using JFA.Telegram.Console;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InputFiles;
using System.Threading;

var planets = new PictureService();
var userd = new servicUser();
Dictionary<string, string>? planetalar = new Dictionary<string, string>();

var botManager = new TelegramBotManager();
var bot = botManager.Create("5968230592:AAE4SlJ3cSI84ycdk5BwB9qmvfk51tzpIHs");
var botDetails = await bot.GetMeAsync();
Console.WriteLine(botDetails.FirstName + " is working..");
botManager.Start(Start);

void Start(Update update)
{
    var (xabar, chatId, istrue) = GetData(update);
    User user = userd.CheckUser(chatId); 

    if (!istrue)
    {
        return;
    }
    if (xabar.StartsWith("planet"))
    {
        var index = int.Parse(xabar.Replace("planet", ""));
        if (planets.Planets![index - 1].Gravity!.Exist)
        {
            if (planets.Planets[index - 1].Picture!.Exist) 
            {
                var satr = $"Name of planet {planets.Planets[index - 1].Name}\nYour weight: {(double)(int)(((planets.Planets[index - 1].Gravity!.G * user.UserWeight) / 10) * 100) / 100} kg";
            
               var fileBytes = System.IO.File.ReadAllBytes($"Rasmlar/{planets.Planets[index-1].Picture!.Name}.png");
                var ms = new MemoryStream(fileBytes);
                bot.SendPhotoAsync(chatId: chatId, new InputOnlineFile(ms), caption: satr);
            }
        }
        else
        {
            ShowHammasi(user, index);
            bot.SendPollAsync(chatId,"Do you like this bot's work ?", new[]{"Yes", "A little", "No"}, cancellationToken: (CancellationToken.None));
        }
    }

    switch (user.nextMessage)
    {
        case Enum.Dastavval:
        switch (xabar)
        {
            case "weight": Knowweight(user); break;
            case "create": Createplanet(user); break;
            default: bot.SendTextMessageAsync(user.ChatId, "Menu", replyMarkup: buutons1()); break;
        }
        break;
        case Enum.Massa: SendMassa(user); break;
        case Enum.Menu:
            {
                switch (xabar)
                {
                    case "back": Orqaga(user); break;
                    default:
                    bot.SendTextMessageAsync(user.ChatId, 
                    "You want to know your weight on which planet?\n\nMenu:\n", 
                    replyMarkup: GetInlineKeyboardMatrix(planetalar, 3));
                    user.nextMessage = Enum.Menu; break;
                }
            }
            break;
        case Enum.Hisobla: Hisob(user, xabar!); break;
        case Enum.CreatePlanet: SavePlanetName(user, xabar!); break;
        case Enum.SaveG: SaveG(user, xabar!); break;
        case Enum.Color: SaveColor(user, xabar!); break;
    }
}

Tuple<string, long, bool> GetData(Update update)
{
    if (update.Type == UpdateType.Message)
    {
        return new(update.Message!.Text!, update.Message.From!.Id, true);
    }
    if (update.Type == UpdateType.CallbackQuery)
    {
        return new(update.CallbackQuery!.Data!, update.CallbackQuery.From.Id, true);
    }
    return new(null!, 0, false);
}
void SaveColor(User user, string xabar)
{
    user.PersonalPlanetColor = xabar;
    bot.SendTextMessageAsync(user.ChatId, "Qoyilmaqom, planet создан\nBut because of the distance, you cannot see🔭 and determine where it is🙅");
    user.nextMessage = Enum.Dastavval;
    bot.SendTextMessageAsync(user.ChatId, "Menu", replyMarkup: buutons1());
}
void SaveG(User user, string xabar)
{
    user.PersonalPlanetG = xabar;
    bot.SendTextMessageAsync(user.ChatId, "Enter your planet color: ");
    user.nextMessage = Enum.Color;
}
void SavePlanetName(User user, string message)
{
    user.PersonalPlanetName = message;
    bot.SendTextMessageAsync(user.ChatId, "Enter your planet g = ");
    user.nextMessage = Enum.SaveG;
}
void Createplanet(User user)
{
    bot.SendTextMessageAsync(user.ChatId, "Enter your planet name:");
    user.nextMessage = Enum.CreatePlanet;
}
void Knowweight(User user)
{
    bot.SendTextMessageAsync(user.ChatId, "Send me your earth mass and find out your weight on the rest of the planets!");
    user.nextMessage = Enum.Hisobla;
}
void Hisob(User user, string massa)
{
    user.UserWeight = (int)Convert.ToInt64(massa);
    if (planets.Planets is not null)
    {
        foreach (var item in planets.Planets)
        {
            planetalar.Add("planet" + item.Id.ToString(), item.Name!);
        }
    }
    user.nextMessage = Enum.Menu;
    bot.SendTextMessageAsync(
        user.ChatId,
        "You want to know your weight on which planet?\n\nMenu:\n",
        replyMarkup: GetInlineKeyboardMatrix(planetalar, 3));
}
void Orqaga(User user)
{
    user.nextMessage = Enum.Dastavval;
    bot.SendTextMessageAsync(user.ChatId, "Menu", replyMarkup: buutons1());
}

void ShowHammasi(User user, int index)
{
    var satr = "";
    for(int i = 0; i< planets.Planets.Count-1; i++)
    {
        satr += $"Planet.{planets.Planets[i].Name} ==> {(double)(int)(((planets.Planets[i].Gravity!.G * user.UserWeight) / 10) * 100) / 100} kg\n";
    }
    var fileBytes = System.IO.File.ReadAllBytes($"Rasmlar/hammasi.png");
    var ms = new MemoryStream(fileBytes);
    bot.SendPhotoAsync(chatId: user.ChatId, new InputOnlineFile(ms), caption: satr);

    user.nextMessage = Enum.Menu;
}
void SendMassa(User user)
{
    bot.SendTextMessageAsync(user.ChatId, "Send me your earth mass and find out your weight on the rest of the planets!");
    user.nextMessage = Enum.Hisobla;
}

InlineKeyboardMarkup buutons1()
{
    var key = new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>()
        {
            new List<InlineKeyboardButton>()
            {
                InlineKeyboardButton.WithCallbackData("🔰Know your weight", "weight")
            },
            new List<InlineKeyboardButton>()
            {
                InlineKeyboardButton.WithCallbackData("♻️Create planet for you", "create")
            }
        }
       );
    return key;
}

InlineKeyboardMarkup GetInlineKeyboardMatrix(Dictionary<string, string> keys, int columns = 2)
{
    int row = 0;

    var buttonMatrix = new List<List<InlineKeyboardButton>>();

    while (keys.Skip(row).Take(columns)?.Count() > 0)
    {
        var buttons = keys.Skip(row * columns).Take(columns).Select(k => InlineKeyboardButton.WithCallbackData(k.Value, k.Key)).ToList();

        buttonMatrix.Add(buttons);

        row++;
    }
    buttonMatrix.Add(new List<InlineKeyboardButton>() { InlineKeyboardButton.WithCallbackData("↩️Back", "back") });
    return new InlineKeyboardMarkup(buttonMatrix.ToArray());
}