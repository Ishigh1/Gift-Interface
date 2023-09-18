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
	case "send":
		string recipient = strings[1];
		string giftName = string.Join(' ', strings, 2, strings.Length - 2).ToLower();
		if (giftHandler.SavedGifts.TryGetValue(giftName, out (GiftItem item, GiftTrait[] traits) giftInfo))
			giftingService.SendGift(giftInfo.item, giftInfo.traits, recipient);
		else
			Console.WriteLine("Unknown gift");
		break;
	case "list":
		foreach (string savedGiftsKey in giftHandler.SavedGifts.Keys)
		{
			Console.WriteLine(savedGiftsKey);
		}
		break;
}

goto loop;

end: ;