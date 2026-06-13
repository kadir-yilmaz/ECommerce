using System.Collections.Generic;

namespace ECommerce.Application.Abstractions.Services
{
    public interface IContentModerationService
    {
        ContentAnalysisResult Analyze(string text);
    }

    public class ContentAnalysisResult
    {
        public bool HasProfanity { get; set; }
        public bool HasPriceInfo { get; set; }
        public List<string> DetectedProfanities { get; set; } = new();
        public List<string> DetectedPricePatterns { get; set; } = new();
    }
}
