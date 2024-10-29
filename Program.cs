

// Samples
// https://github.com/sandrohanea/whisper.net

// Download models:
// https://ggml.ggerganov.com/
// https://huggingface.co/ggerganov/whisper.cpp
// ou usando os scripts

// extrair o áudio...
// ffmpeg -i input.mp4 -ar 16000 -ac 1 output.wav

using System.Diagnostics;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Whisper.net;
using Whisper.net.Ggml;
using YoutubeExplode;

Console.WriteLine("Whisper Test!");

// ffmpeg -i input.mp4 -ar 16000 -ac 1 output.wav

Process.Start("ffmpeg", "-i Radio_Cidade.mp4 -ar 16000 -ac 1 Radio_Cidade.wav").WaitForExit();

var modelName = "ggml-base.bin";
// var modelName = "ggml-model-whisper-large-q5_0.bin";

if (!File.Exists(modelName))
{
    Console.WriteLine("Downloading model...");
    using var modelStream = await WhisperGgmlDownloader.GetGgmlModelAsync(GgmlType.Base);
    using var fileWriter = File.OpenWrite(modelName);
    await modelStream.CopyToAsync(fileWriter);
}

Console.WriteLine("Processing audio...");

using var whisperFactory = WhisperFactory.FromPath(modelName);

using var processor = whisperFactory.CreateBuilder()
    .WithLanguage("portuguese")
    .Build();

using var fileStream = File.OpenRead(@"Radio_Cidade.wav");

await foreach (var result in processor.ProcessAsync(fileStream))
{
    Console.WriteLine($"{result.Start}->{result.End}: {result.Text}");
}

// async Task<YouTubeService> GetYoutubeServiceAsync()
// {
//     UserCredential credential;
//     using var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read);
//     credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
//         GoogleClientSecrets.FromStream(stream).Secrets,
//         new[] { YouTubeService.Scope.Youtube },
//         "user",
//         CancellationToken.None,
//         new FileDataStore(typeof(Program).FullName) // this.GetType().ToString()
//     );

//     var youtubeService = new YouTubeService(new BaseClientService.Initializer()
//     {
//         HttpClientInitializer = credential,
//         ApplicationName = typeof(Program).FullName // this.GetType().ToString()
//     });

//     return youtubeService;
// }

// async Task DownloadYoutubeVideoAsync(string videoId)
// {
//     var youtube = new YoutubeClient();
//     var video = youtube.Videos.GetAsync(videoId).Result;
//     var streamInfoSet = youtube.Videos.Streams.GetManifestAsync(videoId).Result;
//     var streamInfo = streamInfoSet.GetAudioStreams().FirstOrDefault();
//     var stream = await youtube.Videos.Streams.GetAsync(streamInfo);
//     var fileStream = File.OpenWrite($"{video.Title}.{streamInfo.Container.GetFileExtension()}");
//     stream.CopyTo(fileStream);
//     fileStream.Close();
// }

