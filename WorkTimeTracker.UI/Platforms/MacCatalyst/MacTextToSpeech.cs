using System.Threading.Tasks;
using AVFoundation;

namespace Phoneword
{
    public static class MacTextToSpeech
    {
        // volume 范围 0.0 ~ 1.0，1.0 表示最大音量
        public static Task SpeakAsync(string text, float volume = 1.0f)
        {
            var utterance = new AVSpeechUtterance(text)
            {
                Volume = volume
            };
            var synthesizer = new AVSpeechSynthesizer();
            synthesizer.SpeakUtterance(utterance);
            return Task.CompletedTask;
        }
    }
}
