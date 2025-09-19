using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;
using System.Net;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            Tempo? t = null;
            string chave = "6135072afe7f6cec1537d5cb08a5a1a2";
            string url = $"https://api.openweathermap.org/data/2.5/weather?" +
                         $"q={cidade}&units=metric&appid={chave}&lang=pt_br";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage resp = await client.GetAsync(url);

                    if (resp.IsSuccessStatusCode)
                    {
                        string json = await resp.Content.ReadAsStringAsync();
                        var rascunho = JObject.Parse(json);

                        // Converter Unix timestamp corretamente
                        string sunriseStr = DateTimeOffset.FromUnixTimeSeconds((long)rascunho["sys"]["sunrise"])
                                                         .ToLocalTime().ToString("HH:mm");                          //ToLocalTime() ajusta para o fuso horário local do dispositivo.
                        string sunsetStr = DateTimeOffset.FromUnixTimeSeconds((long)rascunho["sys"]["sunset"])      //HH:mm → hora:minuto.
                                                        .ToLocalTime().ToString("HH:mm");

                        t = new Tempo
                        {
                            lat = (double)rascunho["coord"]["lat"],
                            lon = (double)rascunho["coord"]["lon"],
                            description = (string)rascunho["weather"][0]["description"],
                            main = (string)rascunho["weather"][0]["main"],
                            temp_min = (double)rascunho["main"]["temp_min"],
                            temp_max = (double)rascunho["main"]["temp_max"],
                            speed = (double)rascunho["wind"]["speed"],
                            visibility = (int)rascunho["visibility"],
                            sunrise = sunriseStr,
                            sunset = sunsetStr
                        };
                    }
                    else if (resp.StatusCode == HttpStatusCode.NotFound)
                    {
                        await App.Current.MainPage.DisplayAlert("Erro", $"Cidade \"{cidade}\" não encontrada.", "OK");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Erro", $"Erro no servidor: {resp.StatusCode}", "OK");
                    }
                }
            }
            catch (HttpRequestException)
            {
                await App.Current.MainPage.DisplayAlert("Sem conexão", "Verifique sua internet e tente novamente.", "OK");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Erro inesperado", ex.Message, "OK");
            }

            return t;
        }

        internal static async Task<Tempo?> GetPrevisao(object text)
        {
            throw new NotImplementedException();
        }
    }
}