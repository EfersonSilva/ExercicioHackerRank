using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

class Result
{
    //public static readonly string testJson = File.ReadAllText("C:\\Users\\EfersonSantos\\Documents\\testJson.txt");

    /*
     * Complete the 'bodyTemperature' function below.
     *
     * The function is expected to return an INTEGER_ARRAY.
     * The function accepts following parameters:
     *  1. STRING doctorName
     *  2. INTEGER diagnosisId
     * API URL: https://jsonmock.hackerrank.com/api/medical_records?page={page_no}
     */

    public static List<int> bodyTemperature(string doctorName, int diagnosisId)
    {
        List<int> temperatures = new List<int>();

        int page = 1;
        int maxPages = 1; // Limite máximo de páginas para evitar um loop infinito

        while (page <= maxPages)
        {
            Relatorio relatorio = GetHttp($"https://jsonmock.hackerrank.com/api/medical_records?page={page}").Result;

            var data = relatorio?.data.FirstOrDefault(x => x.doctor.name == doctorName && x.diagnosis.id == diagnosisId);

            if (data != null)
            {


                temperatures.Add(data.vitals.bloodPressureDiastole);
                temperatures.Add(data.vitals.bloodPressureSystole);
                break; // Encontrou o registro, sai do loop
            }

            maxPages = relatorio?.total_pages ?? 1;
            page++;
        }

        return temperatures;
    }

    public static async Task<Relatorio> GetHttp(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Relatorio relatorio = JsonConvert.DeserializeObject<Relatorio>(responseBody);
                    return relatorio;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
    }
}

public class Relatorio
{
    public int page { get; set; }
    public int per_page { get; set; }
    public int total { get; set; }
    public int total_pages { get; set; }
    public List<Data> data { get; set; }
}

public class Data
{
    public int id { get; set; }
    public ulong timestamp { get; set; }
    public int userid { get; set; }
    public string userName { get; set; }
    public string userDob { get; set; }
    public Vitals vitals { get; set; }
    public Diagnosis diagnosis { get; set; }
    public Doctor doctor { get; set; }
    public Meta meta { get; set; }
}

public class Vitals
{
    public int bloodPressureDiastole { get; set; }
    public int bloodPressureSystole { get; set; }
    public int pulse { get; set; }
    public int breathingRate { get; set; }
    public double bodyTemperature { get; set; }
}

public class Diagnosis
{
    public int id { get; set; }
    public string name { get; set; }
    public int severity { get; set; }
}

public class Doctor
{
    public int id { get; set; }
    public string name { get; set; }
}

public class Meta
{
    public int height { get; set; }
    public int weight { get; set; }
}

class Solution
{
    public static void Main(string[] args)
    {
        Environment.SetEnvironmentVariable("OUTPUT_PATH", "C:\\Users\\exe.txt");
        TextWriter textWriter = new StreamWriter(@System.Environment.GetEnvironmentVariable("OUTPUT_PATH"), true);

        string doctorName = Console.ReadLine();

        int diagnosisId = Convert.ToInt32(Console.ReadLine().Trim());

        List<int> result = Result.bodyTemperature(doctorName, diagnosisId);

        textWriter.WriteLine(String.Join("\n", result));

        textWriter.Flush();
        textWriter.Close();
    }
}

