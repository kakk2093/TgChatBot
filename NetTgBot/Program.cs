using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WebSocket4Net;


public class Program
{
    // public string _apiToken = "6746756605:AAFu0gL0rdR3ahhdBUHg6TaGzEB_fO-EFUo";
    //  public static ITelegramBotClient _botClient;
    //  private static ReceiverOptions _receiverOptions;

    public static WebSocket ws;

    public async static Task Main()
    {

        ITelegramBotClient botClient = new TelegramBotClient("6746756605:AAFu0gL0rdR3ahhdBUHg6TaGzEB_fO-EFUo");

        ReceiverOptions receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[]
        {
                UpdateType.Message,
            },

            ThrowPendingUpdates = true,
        };

        using var cts = new CancellationTokenSource();


        botClient.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions, cts.Token);


        var me = await botClient.GetMeAsync();

        Console.WriteLine($"{me.FirstName} was started!");

        using (ws = new WebSocket("ws://localhost:8080"))
        {

            ws.Opened += (sender, e) =>
            {
                Console.WriteLine("Connect");
            };

            ws.MessageReceived += (sender, e) =>
                              Console.WriteLine("Laputa says: " + e.Message);

            ws.Open();

            Console.ReadKey(true);

        }

        await Task.Delay(-1);
    }


    static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            if (update.Message is not { } message)
                return;

            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;

            var messageTextRecive = message.Text;
            Message sentMessage = await botClient.SendTextMessageAsync(chatId, text: "You said:\n" + messageText, cancellationToken: cancellationToken);
            // Message sentMessage = await botClient.SendTextMessageAsync(chatId, text: "You said:\n" + messageText);
            Console.WriteLine(messageTextRecive);

            ws.Send(messageTextRecive);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}

