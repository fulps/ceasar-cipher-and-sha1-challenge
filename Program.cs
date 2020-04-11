using System;
using System.Text;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace codenation
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();
        static async Task Main()
        {
            try
            {
                String token = string.Empty; // Coloque seu token aqui <--
                String decifrado = string.Empty;
                String hash = string.Empty;
                byte[] hashData;

                String url = $"https://api.codenation.dev/v1/challenge/dev-ps/generate-data?token={token}";

                char[] alphabet = (Enumerable.Range('a', 'z' - 'a' + 1).Select(i => (Char)i)).ToArray(); // Alfabeto
                char[] reverso = alphabet.Reverse().ToArray(); // Alfabeto reverso caso o numero_casas exceder o limite

                if (token != string.Empty)
                {
                    HttpResponseMessage response = await client.GetAsync(url); // GET da url de cima com o seu token
                    response.EnsureSuccessStatusCode();

                    JObject rules = JObject.Parse(await response.Content.ReadAsStringAsync()); // GET -> objeto

                    int numero_casas = rules.Value<int>("numero_casas") > alphabet.Length ? rules.Value<int>("numero_casas") - alphabet.Length : rules.Value<int>("numero_casas");
                    // Garantindo que o numero de casas nao exceda o alfabeto
                    foreach (char c in rules.Value<string>("cifrado"))
                    {
                        if (alphabet.Contains(c))
                        {
                            int resto = (numero_casas - Array.IndexOf(alphabet, c)) - 1; // Pega o resto que falta do numero de casas caso exceda e coloca no alfabeto reverso
                            decifrado += Array.IndexOf(alphabet, c) < numero_casas ? Char.ToLower(reverso[resto]) : Char.ToLower(alphabet[Array.IndexOf(alphabet, c) - numero_casas]);
                            // Se a letra for menor que numero_casas, recorro ao alfabeto reverso, se não, sigo o alfabeto normal subtraindo o numero_casas, 
                        }
                        else
                        {
                            decifrado += c; // Caso não esteja no alfabeto
                        }
                    }
                    hashData = new SHA1Managed().ComputeHash(Encoding.ASCII.GetBytes(decifrado)); // Codificando o valor decifrado...
                    foreach (var b in hashData)
                    {
                        hash += b.ToString("X2"); // Adicionando pra hexadecimal em letra maiuscula
                    }
                    Console.WriteLine(decifrado);
                    Console.WriteLine(hash.ToLower());
                } else {
                    throw new Exception("Digite o seu token da codenation na variável token!");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
