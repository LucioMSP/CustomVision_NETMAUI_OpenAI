using DermAI.Services;
using Microsoft.Extensions.Logging;

namespace DermAI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            // Add services
            builder.Services.AddTransient<MainPage>();

            builder.Services.AddSingleton<IMediaPicker>(MediaPicker.Default);
            OpenAIService svc = new OpenAIService();
            svc.Initialize(ApiKeys.OpenAIKey, ApiKeys.OpenAIEndpoint);
            builder.Services.AddSingleton<OpenAIService>(svc);


            return builder.Build();
        }
    }
}
