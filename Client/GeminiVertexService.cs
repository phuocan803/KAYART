using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Client
{

    public class GeminiVertexService
    {
        private readonly string apiKey;
        private readonly HttpClient httpClient;

        public GeminiVertexService(string apiKey, string unused1 = "", string unused2 = "", string unused3 = "")
        {
            this.apiKey = apiKey;
            this.httpClient = new HttpClient();
            this.httpClient.Timeout = TimeSpan.FromSeconds(60);
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }


        public async Task<Bitmap> EnhanceImageAsync(Bitmap originalImage, string stylePrompt = "professional digital artwork")
        {
            try
            {

                string prompt = $"Transform into {stylePrompt}, vibrant colors, professional quality, enhanced details, polished, beautiful, high quality";

                byte[] imageBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    originalImage.Save(ms, ImageFormat.Png);
                    imageBytes = ms.ToArray();
                }

                using (var formData = new MultipartFormDataContent())
                {
                    var imageContent = new ByteArrayContent(imageBytes);
                    imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                    formData.Add(imageContent, "init_image", "image.png");

                    formData.Add(new StringContent("stable-diffusion-xl-1024-v1-0"), "text_prompts[0][text]");
                    formData.Add(new StringContent(prompt), "text_prompts[0][text]");
                    formData.Add(new StringContent("200"), "text_prompts[0][weight]");
                    
                    formData.Add(new StringContent("0.35"), "image_strength");
                    formData.Add(new StringContent("7"), "cfg_scale");
                    formData.Add(new StringContent("30"), "steps");
                    formData.Add(new StringContent("1024"), "width");
                    formData.Add(new StringContent("1024"), "height");

                    var request = new HttpRequestMessage(HttpMethod.Post, "https://api.stability.ai/v1/generation/stable-diffusion-xl-1024-v1-0/image-to-image");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    request.Content = formData;


                    var response = await httpClient.SendAsync(request);
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Stability AI error: {response.StatusCode} - {responseContent}");
                    }


                    var jsonResponse = JObject.Parse(responseContent);
                    var artifacts = jsonResponse["artifacts"] as JArray;

                    if (artifacts != null && artifacts.Count > 0)
                    {
                        string base64Image = artifacts[0]["base64"]?.ToString();
                        
                        if (!string.IsNullOrEmpty(base64Image))
                        {
                            
                            byte[] generatedBytes = Convert.FromBase64String(base64Image);
                            using (MemoryStream ms = new MemoryStream(generatedBytes))
                            {
                                return new Bitmap(ms);
                            }
                        }
                    }

                    throw new Exception("No image in Stability AI response");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"AI Enhancement failed: {ex.Message}", ex);
            }
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}
