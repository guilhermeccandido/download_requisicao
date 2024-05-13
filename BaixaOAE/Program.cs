using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        // Token de autentica��o
        string token = "token";

        // Lista de URLs
        string[] urls = { "link" };

        // Caminho local onde os arquivos ZIP ser�o salvos
        string caminhoDestino = @"D:\OAEs";

        // Loop sobre cada URL na lista
        foreach (var url in urls)
        {
            await FazerRequisicaoEGravarArquivo(url, caminhoDestino, token);
        }
    }

    static async Task FazerRequisicaoEGravarArquivo(string url, string caminhoDestino, string token)
    {
        try
        {
            using (HttpClient cliente = new HttpClient())
            {
                // Adiciona o token de autentica��o ao cabe�alho
                cliente.DefaultRequestHeaders.Add("Authorization", "Basic " + token);

                // Obt�m o nome do arquivo da URL
                string nomeArquivoLocal = Path.GetFileName(url);
                nomeArquivoLocal = nomeArquivoLocal.ReplaceInvalidFileNameChars('_');

                // Combina o caminho local com o nome do arquivo
                string caminhoArquivoLocal = Path.Combine(caminhoDestino, nomeArquivoLocal);

                // Faz a requisi��o GET
                using (HttpResponseMessage resposta = await cliente.GetAsync(url))
                {
                    resposta.EnsureSuccessStatusCode();

                    // Salva o arquivo localmente
                    using (FileStream arquivoStream = File.Create(caminhoArquivoLocal))
                    {
                        await resposta.Content.CopyToAsync(arquivoStream);
                        arquivoStream.Close();
                    }

                    Console.WriteLine($"Arquivo ZIP salvo em: {caminhoArquivoLocal}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro durante a requisi��o: {ex.Message}");
        }
    }
}

public static class StringExtensions
{
    // Fun��o para substituir caracteres inv�lidos em nomes de arquivos
    public static string ReplaceInvalidFileNameChars(this string str, char replacement)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        return string.Join(replacement.ToString(), str.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
    }
}

