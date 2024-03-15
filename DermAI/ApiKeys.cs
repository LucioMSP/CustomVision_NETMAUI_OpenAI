namespace DermAI
{
    static internal class ApiKeys
    {
        #region Custom Vision
        public static string CustomVisionEndPoint => "https://NOMBREPROYECTO-prediction.cognitiveservices.azure.com/"; // Proviene del Portal de Azure (Custon  Vision)
        public static string ProjectId => "Project ID"; //Proviene del Custom Vision
        public static string PublishedName => "Numero de la Iteracion";
        public static string PredictionKey => "Prediction Key"; // Este valor aparece al publicar la Iteración en Custom Vision

        #endregion

        #region Open AI

        public const string OpenAIKey = "sk-YOUR API KEY";
        public const string OpenAIEndpoint = null;

        #endregion

    }
}