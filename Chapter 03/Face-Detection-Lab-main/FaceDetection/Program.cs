using System;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;


namespace AZureAI_FaceDetection
{
    internal class Program
    {
        // Replace <Subscription Key> with your subscription key.
        private const string faceDetectionSubscriptionKey = "<Subscription Key>";

        // replace <myresourcename> with the string found in your endpoint URL
        private const string uriBase =
            "https://azureaiface1.cognitiveservices.azure.com//face/v1.0/detect";

        private static void Main(string[] args)
        {
            Console.WriteLine("Detect Faces: ");
            Console.WriteLine("Enter the path of an image for analysis: ");
            string imgFilePath = Console.ReadLine();

            if (File.Exists(imgFilePath))
                FaceAnalysisRequest(imgFilePath);
            else
                Console.WriteLine("\nFile path is Invalid.\nPress Enter to exit...\n");

            Console.ReadLine();
        }

        // Uses Face REST API to Analyze the Image.
        private static async void FaceAnalysisRequest(string imageFilePath)
        {
            HttpClient httpClient = new HttpClient();

            // Request headers.
            httpClient.DefaultRequestHeaders.Add(
                "Ocp-Apim-Subscription-Key", faceDetectionSubscriptionKey);

            // Request parameters.
            string parameters = "returnFaceId=true&returnFaceLandmarks=false" +
                                "&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses," +
                                "emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";

            // Below URI for the REST API Call.
            string requestUri = uriBase + "?" + parameters;

            HttpResponseMessage response;

            // Request body. Posts a locally stored image.
            byte[] imgByteData = ConvertImageToByteArray(imageFilePath);

            using (ByteArrayContent content = new ByteArrayContent(imgByteData))
            {
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                response = await httpClient.PostAsync(requestUri, content);

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();

                // Display the JSON response.
                Console.WriteLine("\nResponse:\n");
                Console.WriteLine(JsonPrettyPrint(contentString));
                Console.WriteLine("\nPress Enter key to exit...");
            }
        }

        // Converts specified file to byte array.
        private static byte[] ConvertImageToByteArray(string imageFilePath)
        {
            using (FileStream fileStreamReader =
                   new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binReader = new BinaryReader(fileStreamReader);
                return binReader.ReadBytes((int)fileStreamReader.Length);
            }
        }

        // Formats the given JSON string by adding line breaks and indents.
        private static string JsonPrettyPrint(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;

            json = json.Replace(Environment.NewLine, "").Replace("\t", "");

            StringBuilder sb = new StringBuilder();
            bool quote = false;
            bool ignore = false;
            int offset = 0;
            int indentLength = 3;

            foreach (char c in json)
            {
                switch (c)
                {
                    case '"':
                        if (!ignore) quote = !quote;
                        break;
                    case '\'':
                        if (quote) ignore = !ignore;
                        break;
                }

                if (quote)
                    sb.Append(c);
                else
                    switch (c)
                    {
                        case '{':
                        case '[':
                            sb.Append(c);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', ++offset * indentLength));
                            break;
                        case '}':
                        case ']':
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', --offset * indentLength));
                            sb.Append(c);
                            break;
                        case ',':
                            sb.Append(c);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', offset * indentLength));
                            break;
                        case ':':
                            sb.Append(c);
                            sb.Append(' ');
                            break;
                        default:
                            if (c != ' ') sb.Append(c);
                            break;
                    }
            }

            return sb.ToString().Trim();
        }
    }
}