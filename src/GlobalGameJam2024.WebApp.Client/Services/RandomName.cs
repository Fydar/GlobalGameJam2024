namespace GlobalGameJam2024.WebApp.Client.Services;

public static class RandomName
{
	private static readonly Random random = new();

	private static readonly string[] prefix = new[]
	{
			"Smart",
			"Quick",
			"Rabid",
			"Ace",
			"Furry",
			"Alert",
			"Black",
			"Blue",
			"Red",
			"Pink",
			"White",
			"Cute"
	};

	private static readonly string[] animals = new[]
	{
			"Fox",
			"Owl",
			"Seal",
			"Penguin",
			"Chicken"
	};

	public static string Generate()
	{
		return $"{prefix[random.Next(0, prefix.Length)]} {animals[random.Next(0, animals.Length)]}";
	}
}
