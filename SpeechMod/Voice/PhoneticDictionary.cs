using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace SpeechMod.Voice;

public static class PhoneticDictionary
{
	private static readonly Dictionary<string, string> LitteralDictionary = new()
	{
		{ "—", Constants.BREAK_TOKEN_SHORT },
		{ "...", Constants.BREAK_TOKEN_MEDIUM },
		{ " - ", Constants.BREAK_TOKEN_LONG }
	};
	private static Dictionary<string, string> s_PhoneticDictionary;

	private static string SpaceOutDate(string text)
	{
		var pattern = @"([0-9]{2})\/([0-9]{2})\/([0-9]{4})";
		return Regex.Replace(text, pattern, "$1 / $2 / $3");
	}

	public static string PrepareText(this string text)
	{
		if (s_PhoneticDictionary == null || !s_PhoneticDictionary.Any())
			LoadDictionary();

		//text = text.ToLower();
		text = text.Replace("\"", "");
		text = text.Replace("\n", ". ");
		text = text.Replace("\r", ". ");
		text = text.Replace(" - ", "");
		text = text.Trim();
		text = text.TrimEnd('.');

		text = SpaceOutDate(text);

		text = LitteralDictionary.Aggregate(text, (current, entry) => current?.Replace(entry.Key, $" {entry.Value} "));
		// Remove multiple spaces
		text = Regex.Replace(text, @"\s+", " ");
		// Remove multiple dots
		text = Regex.Replace(text, @"[.]+", ".");
		return s_PhoneticDictionary?.Aggregate(text, (current, entry) => Regex.Replace(current, $@"\b{entry.Key}\b", $"{entry.Value}"));
	}

	public static void LoadDictionary()
	{
		Main.Logger?.Log("Loading phonetic dictionary from: ");
		try
		{
			var assemblyLocation = AppDomain.CurrentDomain.BaseDirectory;
			var file = Path.Combine(assemblyLocation, "Mods", "PFKingmakerSpeechMod", "PhoneticDictionary.json");
			Main.Logger?.Log(file);
			var json = File.ReadAllText(file, Encoding.UTF8);

			var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

			s_PhoneticDictionary = new Dictionary<string, string>(dict);

			Main.Logger?.Log("Phonetic dictionary loaded!");
		}
		catch (Exception ex)
		{
			Main.Logger?.LogException(ex);
			Main.Logger?.Warning("Loading backup dictionary!");
			LoadBackupDictionary();
		}

#if DEBUG
		foreach (var entry in s_PhoneticDictionary)
		{
			Main.Logger?.Log($"'{entry.Key}' = '{entry.Value}'");
		}
		Main.Logger?.Log("Phonetic dictionary listed!");
#endif
	}

	private static void LoadBackupDictionary()
	{
		s_PhoneticDictionary = new Dictionary<string, string>
		{
			{ Constants.BREAK_TOKEN_SHORT, "<silence msec=\"350\"/>" },
			{ Constants.BREAK_TOKEN_MEDIUM, "<silence msec=\"500\"/>" },
			{ Constants.BREAK_TOKEN_LONG, "<silence msec=\"750\"/>" }
		};
	}
}
