using DermAI.Services;
using System.Diagnostics;
using Microsoft.Maui.Graphics.Platform;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;

namespace DermAI;
public partial class MainPage : ContentPage
{
    private readonly IMediaPicker mediaPicker;
    OpenAIService openAIService;

    private const int ImageMaxSizeBytes = 4194304;
    private const int ImageMaxResolution = 1024;

    public MainPage(IMediaPicker mediaPicker, OpenAIService svc)
    {
        openAIService = svc;
        InitializeComponent();
       this.mediaPicker = mediaPicker;
    }
    private async void OnPhotoClicked(object sender, EventArgs e)
    {
       if (mediaPicker.IsCaptureSupported)
       {
           FileResult photo = await mediaPicker.CapturePhotoAsync();
           if (photo != null)
           {
               var resizedPhoto = await ResizePhotoStream(photo);
                
                // Custom Vision API call
                var result = await ClassifyImage(new MemoryStream(resizedPhoto));

                // Change the percentage notation from 0.9 to display 90.0%
                var percent = result.Probability.ToString("P1");

                Photo.Source = ImageSource.FromStream(() => new MemoryStream(resizedPhoto));

                Result.Text = result.TagName.Equals("Negative") ? "It's not skin." : $"It was detected that you have {percent} {result.TagName}.";

                Recommendation.Text = "Open AI recommendation: " + await openAIService.CallOpenAI(result.TagName);
           }
       } 
    }

    private async Task<byte[]> ResizePhotoStream(FileResult photo)
    {
        byte[] result = null;

        using (var stream = await photo.OpenReadAsync())
        {
            if (stream.Length > ImageMaxSizeBytes)
            {
                var image = PlatformImage.FromStream(stream);
                if (image != null)
                {
                    var newImage = image.Downsize(ImageMaxResolution, true);
                    result = newImage.AsBytes();
                }
            }
            else
            {
                using (var binaryReader = new BinaryReader(stream))
                {
                    result = binaryReader.ReadBytes((int)stream.Length);
                }
            }
        }

        return result;
    }
    private async Task<PredictionModel> ClassifyImage(Stream photoStream)
    {
            try
            {
                var endpoint = new CustomVisionPredictionClient(
                    new ApiKeyServiceClientCredentials(ApiKeys.PredictionKey))
                {
                    Endpoint = ApiKeys.CustomVisionEndPoint
                };

                // Send image to the Custom Vision API
                var results = await endpoint.ClassifyImageAsync(
                    Guid.Parse(ApiKeys.ProjectId),
                    ApiKeys.PublishedName,
                    photoStream);

                // Return the most likely prediction
                return results.Predictions?.OrderByDescending(x => x.Probability).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new PredictionModel();
            }
            finally
            {

            }
    }
}