using ECommerce.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ECommerce.Persistence.Services
{
    public class ContentModerationService : IContentModerationService
    {
        private readonly string[] _profanityRoots;
        private readonly string[] _priceKeywords;

        public ContentModerationService()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            
            // Try to find the source project directory first to write/read there during development
            string sourceDataDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "ECommerce.Persistence", "Data"));
            string dataDir;
            
            if (Directory.Exists(Path.Combine(baseDir, "..", "..", "..", "..", "ECommerce.Persistence")))
            {
                dataDir = sourceDataDir;
            }
            else
            {
                dataDir = Path.Combine(baseDir, "Data");
            }

            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }

            string profanityPath = Path.Combine(dataDir, "profanity-words.json");
            string priceKeywordsPath = Path.Combine(dataDir, "price-keywords.json");

            // Handle Profanities
            if (!File.Exists(profanityPath))
            {
                var defaultProfanities = new[]
                {
                    "amk", "aq", "amına", "amina", "sikeyim", "sikerim", "siktir", "piç", "pic",
                    "orospu", "oç", "oc", "pezevenk", "gavat", "ibne", "göt", "got",
                    "yarrak", "yarak", "taşak", "tasak", "meme", "sikmek", "sikik",
                    "dangalak", "gerizekalı", "gerizekali", "salak", "aptal", "mal",
                    "haysiyetsiz", "şerefsiz", "serefsiz", "namussuz", "kaltak",
                    "kahpe", "puşt", "pust", "sürtük", "surtuk", "kevaşe", "kevase",
                    "amcık", "amcik", "dalyarak", "dlyarak", "yavşak", "yavsak",
                    "boktan", "bok", "hassiktir", "hsktr", "sktr", "mkrds",
                    "am", "sik", "zik"
                };
                var json = JsonSerializer.Serialize(defaultProfanities, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(profanityPath, json, System.Text.Encoding.UTF8);
                _profanityRoots = defaultProfanities;
            }
            else
            {
                try
                {
                    var json = File.ReadAllText(profanityPath);
                    _profanityRoots = JsonSerializer.Deserialize<string[]>(json) ?? Array.Empty<string>();
                }
                catch
                {
                    _profanityRoots = Array.Empty<string>();
                }
            }

            // Handle Price Keywords
            if (!File.Exists(priceKeywordsPath))
            {
                var defaultPriceKeywords = new[]
                {
                    "fiyat", "ücret", "ucret", "maliyet", "pahalı", "pahali",
                    "ucuz", "indirim", "tl", "lira", "kuruş", "kurus",
                    "para", "bedava", "bedel"
                };
                var json = JsonSerializer.Serialize(defaultPriceKeywords, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(priceKeywordsPath, json, System.Text.Encoding.UTF8);
                _priceKeywords = defaultPriceKeywords;
            }
            else
            {
                try
                {
                    var json = File.ReadAllText(priceKeywordsPath);
                    _priceKeywords = JsonSerializer.Deserialize<string[]>(json) ?? Array.Empty<string>();
                }
                catch
                {
                    _priceKeywords = Array.Empty<string>();
                }
            }
        }

        // Harf kaydırma haritası (leet speak / obfuscation)
        private static readonly Dictionary<char, char[]> CharSubstitutions = new()
        {
            { 'a', new[] { '@', '4', 'â', 'ä' } },
            { 'e', new[] { '3', 'é', 'ê', 'ë' } },
            { 'i', new[] { '1', '!', 'ı', 'î', 'ï', '|' } },
            { 'o', new[] { '0', 'ö', 'ô' } },
            { 'u', new[] { 'ü', 'û', 'ù' } },
            { 's', new[] { '$', '5', 'ş' } },
            { 'g', new[] { '9', 'ğ' } },
            { 'c', new[] { 'ç', '(' } },
            { 't', new[] { '7', '+' } },
            { 'k', new[] { 'q' } }
        };

        private static readonly Regex PricePatternRegex = new(
            @"(\d+[\.,]?\d*)\s*(tl|₺|lira|kuruş|kurus|\$|€|dolar|euro)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex CurrencySymbolRegex = new(
            @"[₺\$€]\s*\d+[\.,]?\d*|\d+[\.,]?\d*\s*[₺\$€]",
            RegexOptions.Compiled);

        private static readonly Regex NumberWithLiraRegex = new(
            @"\d+\s*(bin|milyon)?\s*(tl|lira)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public ContentAnalysisResult Analyze(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new ContentAnalysisResult();

            var result = new ContentAnalysisResult();

            // Küfür tespiti
            DetectProfanity(text, result);

            // Fiyat bilgisi tespiti
            DetectPriceInfo(text, result);

            return result;
        }

        private void DetectProfanity(string text, ContentAnalysisResult result)
        {
            // Orijinal metni normalize et
            var normalizedText = NormalizeText(text);

            // Boşluk/noktalama ile ayrılmış gizli küfür tespiti
            var strippedText = StripSeparators(normalizedText);

            foreach (var profanity in _profanityRoots)
            {
                // Direkt eşleşme (normalize edilmiş metin)
                if (ContainsWord(normalizedText, profanity))
                {
                    result.HasProfanity = true;
                    if (!result.DetectedProfanities.Contains(profanity))
                        result.DetectedProfanities.Add(profanity);
                    continue;
                }

                // Ayraçlar temizlenmiş metinde arama (a.m.k, a-m-k, a m k vb.)
                if (strippedText.Contains(profanity))
                {
                    result.HasProfanity = true;
                    if (!result.DetectedProfanities.Contains(profanity))
                        result.DetectedProfanities.Add(profanity);
                    continue;
                }

                // Leet speak / harf kaydırma tespiti
                var normalizedProfanity = profanity;
                if (ContainsWithSubstitutions(strippedText, normalizedProfanity))
                {
                    result.HasProfanity = true;
                    if (!result.DetectedProfanities.Contains(profanity))
                        result.DetectedProfanities.Add(profanity);
                }
            }
        }

        private void DetectPriceInfo(string text, ContentAnalysisResult result)
        {
            var lowerText = text.ToLowerInvariant();

            // Fiyat anahtar kelimeleri
            foreach (var keyword in _priceKeywords)
            {
                if (lowerText.Contains(keyword))
                {
                    result.HasPriceInfo = true;
                    if (!result.DetectedPricePatterns.Contains(keyword))
                        result.DetectedPricePatterns.Add(keyword);
                }
            }

            // Regex tabanlı fiyat desenleri
            var priceMatch = PricePatternRegex.Match(text);
            if (priceMatch.Success)
            {
                result.HasPriceInfo = true;
                result.DetectedPricePatterns.Add(priceMatch.Value);
            }

            // Para birimi sembolleri
            var currencyMatch = CurrencySymbolRegex.Match(text);
            if (currencyMatch.Success)
            {
                result.HasPriceInfo = true;
                result.DetectedPricePatterns.Add(currencyMatch.Value);
            }

            // Sayı + TL/lira deseni
            var numberLiraMatch = NumberWithLiraRegex.Match(text);
            if (numberLiraMatch.Success)
            {
                result.HasPriceInfo = true;
                if (!result.DetectedPricePatterns.Contains(numberLiraMatch.Value))
                    result.DetectedPricePatterns.Add(numberLiraMatch.Value);
            }
        }

        /// <summary>
        /// Metni normalize eder: Türkçe karakterleri base hallerine çevirir, küçük harfe dönüştürür
        /// </summary>
        private static string NormalizeText(string text)
        {
            return text
                .ToLowerInvariant()
                .Replace('ı', 'i')
                .Replace('ğ', 'g')
                .Replace('ü', 'u')
                .Replace('ş', 's')
                .Replace('ö', 'o')
                .Replace('ç', 'c')
                .Replace('â', 'a')
                .Replace('î', 'i')
                .Replace('û', 'u');
        }

        /// <summary>
        /// Boşluk, nokta, tire, alt çizgi vb. ayraçları temizler (gizli küfür tespiti için)
        /// </summary>
        private static string StripSeparators(string text)
        {
            return Regex.Replace(text, @"[\s\.\-_\*\+\#\!\?\,\;\:\'\""\/\\]", "");
        }

        /// <summary>
        /// Kelime bazlı eşleşme kontrol eder
        /// </summary>
        private static bool ContainsWord(string text, string word)
        {
            // Kısa kelimeler (2-3 karakter) için tam kelime sınırı kontrolü
            if (word.Length <= 3)
            {
                return Regex.IsMatch(text, $@"(?<!\w){Regex.Escape(word)}(?!\w)");
            }

            // Uzun kelimeler için kısmi eşleşme (kök bazlı)
            return text.Contains(word);
        }

        /// <summary>
        /// Leet speak / harf kaydırma ile gizlenmiş küfürleri tespit eder
        /// </summary>
        private static bool ContainsWithSubstitutions(string text, string word)
        {
            // Her harf için olası alternatifleri oluştur ve regex pattern yap
            var patternParts = new List<string>();
            foreach (var ch in word)
            {
                if (CharSubstitutions.TryGetValue(ch, out var substitutions))
                {
                    var escapedSubs = substitutions.Select(s => Regex.Escape(s.ToString()));
                    var charPattern = $"[{Regex.Escape(ch.ToString())}{string.Join("", escapedSubs)}]";
                    patternParts.Add(charPattern);
                }
                else
                {
                    patternParts.Add(Regex.Escape(ch.ToString()));
                }
            }

            var fullPattern = string.Join("", patternParts);

            try
            {
                return Regex.IsMatch(text, fullPattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}
