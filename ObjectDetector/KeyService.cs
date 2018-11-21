using System.Threading.Tasks;

namespace ObjectDetector
{
    public static class KeyService
    {
        const string PredictionKey = "PredictionKey";
        const string ProjectId = "ProjectId";
        public static string PI = "84e7ad20-e790-4021-b86b-b31fa2f7667d";
        public static string PK = "dd90838db973467095def1b82bb01f35";

        public static Task<string> GetPredictionKey() => Xamarin.Essentials.SecureStorage.GetAsync(PK);
        public static Task SetPredictionKey(string value) => Xamarin.Essentials.SecureStorage.SetAsync(PK, value);

        public static Task<string> GetProjectId() => Xamarin.Essentials.SecureStorage.GetAsync(PI);
        public static Task SetProjectId(string value) => Xamarin.Essentials.SecureStorage.SetAsync(PI, value);
    }
}
