using Archipelago.Gifting.Net.Gifts;
using Archipelago.Gifting.Net.Gifts.Versions.Current;
using Archipelago.Gifting.Net.Service;
using Archipelago.Gifting.Net.Traits;
using Newtonsoft.Json;

namespace Gift_Interface;

public class GiftHandler
{
	private readonly GiftingService GiftingService;
	private const string SavePath = "./gifts.json";
	public readonly Dictionary<string, (GiftItem item, GiftTrait[] traits)> SavedGifts;

	public GiftHandler(GiftingService giftingService)
	{
		GiftingService = giftingService;
		try
		{
			if (File.Exists(SavePath))
			{
				// Read the JSON content from the file
				string jsonContent = File.ReadAllText(SavePath);

				// Deserialize the JSON content into a dictionary
				SavedGifts =
					JsonConvert
						.DeserializeObject<Dictionary<string, (GiftItem item, GiftTrait[] traits)>>(jsonContent)!;
			}
		}
		finally
		{
			SavedGifts ??= new Dictionary<string, (GiftItem item, GiftTrait[] traits)>();
		}
	}

	public void OnGiftReceived(Dictionary<string, Gift> dictionary)
	{
		Console.WriteLine("Received Gifts : ");
		foreach ((string? _, Gift? gift) in dictionary)
		{
			Console.WriteLine(
				$"Received {gift.Amount} {gift.ItemName} (value {gift.ItemValue}) from player {gift.SenderSlot}");
			for (int index = 0; index < gift.Traits.Length; index++)
			{
				GiftTrait giftTrait = gift.Traits[index];
				Console.WriteLine(
					$"Trait {index} : {giftTrait.Trait} quality : {giftTrait.Quality} duration : {giftTrait.Duration}");
			}

			Console.WriteLine();

			GiftItem item = new(gift.ItemName, 1, gift.ItemValue);
			SavedGifts[item.Name.ToLower()] = (item, gift.Traits);
		}

		File.WriteAllTextAsync(SavePath, JsonConvert.SerializeObject(SavedGifts, new JsonGiftConverter()));
		GiftingService.RemoveGiftsFromGiftBox(dictionary.Keys);
	}
}