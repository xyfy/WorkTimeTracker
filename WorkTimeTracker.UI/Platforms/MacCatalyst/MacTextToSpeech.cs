using System.Threading.Tasks;
using AVFoundation;

namespace WorkTimeTracker.UI.Platforms.MacCatalyst
{
    public static class MacTextToSpeech
    {
        // volume 范围 0.0 ~ 1.0，1.0 表示最大音量
        public static Task SpeakAsync(string text, float volume = 1.0f)
        {
            var utterance = new AVSpeechUtterance(text)
            {
                Volume = volume,
                Rate = 0.5f  // 适中的语速
            };
            var synthesizer = new AVSpeechSynthesizer();
            synthesizer.SpeakUtterance(utterance);
            return Task.CompletedTask;
        }
    }
}
