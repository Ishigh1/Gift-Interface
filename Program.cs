using Archipelago.Gifting.Net.Gifts;
using Archipelago.Gifting.Net.Service;
using Archipelago.Gifting.Net.Traits;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Gift_Interface;

Console.Write("Enter the server IP address: ");
string serverIp = Console.ReadLine() ?? throw new InvalidOperationException();
if (serverIp == "")
	serverIp = "localhost";

Console.Write("Enter the server port: ");
int serverPort;
if (!int.TryParse(Console.ReadLine(), out serverPort))
{
	serverPort = 38281;
}

// Prompt the user for username and password
Console.Write("Enter your username: ");
string username = Console.ReadLine() ?? throw new InvalidOperationException();
if (username == "")
	username = "Player1";

ArchipelagoSession session = ArchipelagoSessionFactory.CreateSession(serverIp, serverPort);
session.TryConnectAndLogin("", username, ItemsHandlingFlags.NoItems, tags: new[] { "TextOnly" });

GiftingService giftingService = new(session);
GiftHandler giftHandler = new(giftingService);

giftingService.OpenGiftBox();
giftingService.SubscribeToNewGifts(giftHandler.OnGiftReceived);

loop:

string? input = Console.ReadLine(); //Stops at ctrl+z
if (input == null) goto end;

string[] strings = input.Split(' ');
switch (strings[0])
{
	case "exit":
		goto end;
	case "help":
		Console.WriteLine("exit");
		Console.WriteLine("list : lists all registered gift names");
		Console.WriteLine("send <amount> <item name> <recipient name> : item name is case insensitive");
		break;
	case "list":
		foreach (string savedGiftsKey in giftHandler.SavedGifts.Keys)
		{
			Console.WriteLine(savedGiftsKey);
		}
		break;
	case "send":
		int amount = int.Parse(strings[1]);
		string recipient = strings[^1];
		string giftName = string.Join(' ', strings, 2, strings.Length - 3).ToLower();
		if (giftHandler.SavedGifts.TryGetValue(giftName, out (GiftItem item, GiftTrait[] traits) giftInfo))
		{
			giftInfo.item.Amount = amount;
			giftingService.SendGift(giftInfo.item, giftInfo.traits, recipient);
		}
		else
			Console.WriteLine("Unknown gift");
		break;
}

goto loop;

end: ;