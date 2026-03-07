// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

/*
 *━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 *  File name：BaiDuTranslationService
 *  Creation Time：2025Year03Moon25day weekTwo 20:54:04
 *  Create Build person:Do not hear the cry
 *━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 *  FunctionDescription:
 *     Call Baidu TranslateApiInterface Online Translation,at/in/onDeBugin modeGenerateFrontendi18n TsTranslatekey value,Need to maintain the corresponding firstTable of Contentsunderzh-CN.ts,Compare and correspondLanguageThere is no packagekey,willvalueCarry out the translation andAdd Newto correspondLanguagePackage filein
 *
 *━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 */

using System.Security.Cryptography;

namespace Admin.NET.Core;

/// <summary>
/// Baidu Translate
/// </summary>
[ApiDescriptionSettings("Extend", Module = "Extend", Order = 200)]
public class BaiDuTranslationService : IDynamicApiController, ITransient
{
    // http remote request
    private readonly IHttpRemoteService _httpRemoteService;

    /// <summary>
    /// Baidu translation appId
    /// </summary>
    private static readonly string _appId = "xxxxxxxxxxx";

    /// <summary>
    /// Baidu translation appKey
    /// </summary>
    private static readonly string _appKey = "xxxxxxxxxxx";

    /// <summary>
    /// Baidu translation api address
    /// </summary>
    private static readonly string _baseUrl = "https://fanyi-api.baidu.com/api/trans/vip/translate?";

    // Language mapping dictionary
    private static readonly Dictionary<string, string> langMap = new Dictionary<string, string>
    {
        ["en"] = "en",
        ["de"] = "de",
        ["fi"] = "fin",
        ["es"] = "spa",
        ["fr"] = "fra",
        ["it"] = "it",
        ["ja"] = "jp",
        ["ko"] = "kor",
        ["no"] = "nor",
        ["pl"] = "pl",
        ["pt"] = "pt",
        ["ru"] = "ru",
        ["th"] = "th",
        ["id"] = "id",
        ["ms"] = "may",
        ["vi"] = "vie",
        ["zh-HK"] = "yue",
        ["zh-TW"] = "cht"
    };

    /// <summary>
    /// Initialize a new instance of type <see cref="BaiDuTranslationService"/>.
    /// </summary>
    /// <param name="httpRemoteService"></param>
    public BaiDuTranslationService(IHttpRemoteService httpRemoteService)
    {
        _httpRemoteService = httpRemoteService;
    }

    /// <summary>
    /// Baidu online translation
    /// </summary>
    /// <param name="from">Translate source language</param>
    /// <param name="to">Translation target language</param>
    /// <param name="content">text content</param>
    ///<remarks>
    ///Source and target language support:
    ///zh:Simplified Chinese
    ///cht: Traditional Chinese (Taiwan)
    ///yue: Traditional Chinese (Hong Kong)
    ///en:English
    ///de:German
    ///spa: Spanish
    ///fin:Finnish
    ///fra: French
    ///it: Italian
    ///jp:Japanese
    ///kor: Korean
    ///nor:Norwegian
    ///pl:Polish
    ///pt:Portuguese
    ///ru:Russian
    ///th:Thai
    ///id: Indonesian
    ///may:Malaysia
    ///vie:Vietnamese
    ///
    ///For more languages, please check: https://api.fanyi.baidu.com/doc/21
    /// </remarks>
    /// <returns>Translated text content</returns>
    [DisplayName("Baidu online translation")]
    [HttpGet]
    public async Task<BaiDuTranslationResult> Translation([FromQuery][Required] string from, [FromQuery][Required] string to, [FromQuery][Required] string content)
    {
        // The standard version API authorization can only translate the basic 18 languages. 201 languages ​​require the enterprise exclusive version support. See Baidu API documentation.
        Random rd = new Random();
        string salt = rd.Next(100000).ToString();
        // Change to your key
        string secretKey = _appKey;
        string sign = EncryptString(_appId + content + salt + secretKey);
        string url = $"{_baseUrl}q={HttpUtility.UrlEncode(content)}&from={from}&to={to}&appid={_appId}&salt={salt}&sign={sign}";
        var res = await _httpRemoteService.GetAsAsync<BaiDuTranslationResult>(url);

        if (!res.error_code.Equals("0"))
        {
            throw Oops.Bah($"Translation failed, error code: {res.error_code}, error message: {res.error_msg}");
        }

        return res;
    }

#if DEBUG

    /// <summary>
    /// Generate front-end page i18n file
    /// </summary>
    [DisplayName("Generate front-end page i18n file")]
    [HttpPost]
    public async Task GeneratePageI18nFile()
    {
        try
        {
            // Get base path
            var i18nPath = AppContext.BaseDirectory;

            for (int i = 0; i < 6; i++)
            {
                i18nPath = Directory.GetParent(i18nPath).FullName;
            }

            i18nPath = Path.Combine(i18nPath, "Web", "src", "i18n", "pages", "systemMenu");

            // Read base language file
            var dic = await ReadBaseLanguageFile(i18nPath);

            if (dic.Count == 0)
            {
                throw Oops.Bah("The attribute definition was not found and cannot be generated.");
            }

            // Process all language files in parallel
            var files = Directory.GetFiles(i18nPath, "*.ts").Where(f => !f.EndsWith("zh-CN.ts")).ToList();

            foreach (var file in files)
            {
                var langCode = Path.GetFileNameWithoutExtension(file);
                var langDic = await ReadLanguageFile(file);

                // Query out the key-value pairs that are not generated

                // Linq query
                // var notGen = dic.Where(kv => !langDic.ContainsKey(kv.Key)).ToDictionary(kv => kv.Key, kv => kv.Value);
                // Convert to HashSet to improve performance
                var langDicKey = new HashSet<string>(langDic.Keys);
                var notGen = dic.Where(kv => !langDicKey.Contains(kv.Key)).ToDictionary(kv => kv.Key, kv => kv.Value);

                // No ungenerated bounces
                if (notGen.Count == 0)
                {
                    Console.WriteLine($"{langCode,-6} language pack: {langDic.Count}/Total: {dic.Count} has been fully generated, no need to generate again");
                    continue;
                }

                var str = string.Empty;
                Console.WriteLine($"{langCode,-6} starts to generate language pack, not generated: {notGen.Count}/generated: {langDic.Count}/total {dic.Count}");
                foreach (var gen in notGen)
                {
                    try
                    {
                        if (!langMap.TryGetValue(langCode, out var targetLang))
                        {
                            continue;
                        }

                        var result = await Translation("zh", targetLang, $"{gen.Value}");

                        if (!result.error_code.Equals("0"))
                        {
                            continue;
                        }

                        var translationValue = result.trans_result[0].Dst;
                        LogTranslationProgress(gen.Key, gen.Value, translationValue, ConsoleColor.DarkMagenta);

                        // If the translation result is an empty string, do not append it
                        if (string.IsNullOrEmpty(translationValue))
                        {
                            continue;
                        }

                        // If the translation result contains "'", which often appears in French and Italian, add an escape character before "'"
                        if (translationValue.Contains("'"))
                        {
                            translationValue = translationValue.Replace("'", "\\'");
                        }

                        str += ($"        {gen.Key}: '{translationValue}',{Environment.NewLine}");
                    }
                    catch (Exception e)
                    {
                        LogError(e);
                    }
                }

                if (str.Length > 0)
                {
                    str = str.TrimStart();
                    await FileHelper.InsertsStringAtSpecifiedLocationInFile(file, str, '}', 2, false);
                }
            }
        }
        catch (Exception e)
        {
            throw Oops.Bah(e.Message);
        }
    }

    /// <summary>
    /// Generate front-end menu i18n file
    /// </summary>
    [DisplayName("Generate front-end menu i18n file")]
    [HttpPost]
    public async Task GenerateMenuI18nFile()
    {
        try
        {
            // Get base path
            var i18nPath = AppContext.BaseDirectory;

            for (int i = 0; i < 6; i++)
            {
                i18nPath = Directory.GetParent(i18nPath).FullName;
            }

            i18nPath = Path.Combine(i18nPath, "Web", "src", "i18n", "menu");

            // Read base language file
            var dic = await ReadBaseLanguageFile(i18nPath);

            if (dic.Count == 0)
            {
                throw Oops.Bah("The attribute definition was not found and cannot be generated.");
            }

            // Process all language files in parallel
            var files = Directory.GetFiles(i18nPath, "*.ts").Where(f => !f.EndsWith("zh-CN.ts")).ToList();

            foreach (var file in files)
            {
                var langCode = Path.GetFileNameWithoutExtension(file);
                var langDic = await ReadLanguageFile(file);

                // Query out the key-value pairs that are not generated

                // Linq query
                // var notGen = dic.Where(kv => !langDic.ContainsKey(kv.Key)).ToDictionary(kv => kv.Key, kv => kv.Value);
                // Convert to HashSet to improve performance
                var langDicKey = new HashSet<string>(langDic.Keys);
                var notGen = dic.Where(kv => !langDicKey.Contains(kv.Key)).ToDictionary(kv => kv.Key, kv => kv.Value);

                // No ungenerated bounces
                if (notGen.Count == 0)
                {
                    Console.WriteLine($"{langCode,-6} language pack: {langDic.Count}/Total: {dic.Count} has been fully generated, no need to generate again");
                    continue;
                }

                var str = string.Empty;
                Console.WriteLine($"{langCode,-6} starts to generate language pack, not generated: {notGen.Count}/generated: {langDic.Count}/total {dic.Count}");
                foreach (var gen in notGen)
                {
                    try
                    {
                        if (!langMap.TryGetValue(langCode, out var targetLang))
                        {
                            continue;
                        }

                        var result = await Translation("zh", targetLang, $"{gen.Value}");

                        if (!result.error_code.Equals("0"))
                        {
                            continue;
                        }

                        var translationValue = result.trans_result[0].Dst;
                        LogTranslationProgress(gen.Key, gen.Value, translationValue, ConsoleColor.DarkMagenta);

                        // If the translation result is an empty string, do not append it
                        if (string.IsNullOrEmpty(translationValue))
                        {
                            continue;
                        }

                        // If the translation result contains "'", which often appears in French and Italian, add an escape character before "'"
                        if (translationValue.Contains("'"))
                        {
                            translationValue = translationValue.Replace("'", "\\'");
                        }

                        str += ($"        {gen.Key}: '{translationValue}',{Environment.NewLine}");
                    }
                    catch (Exception e)
                    {
                        LogError(e);
                    }
                }

                if (str.Length > 0)
                {
                    str = str.TrimStart();
                    await FileHelper.InsertsStringAtSpecifiedLocationInFile(file, str, '}', 2, false);
                }
            }
        }
        catch (Exception e)
        {
            throw Oops.Bah(e.Message);
        }
    }

    #region Helper method

    private static async Task<Dictionary<string, string>> ReadBaseLanguageFile(string i18nPath)
    {
        var baseFile = Path.Combine(i18nPath, "zh-CN.ts");
        if (!File.Exists(baseFile))
        {
            throw Oops.Bah("File 【zh-CN.ts】 not found");
        }

        var dic = new Dictionary<string, string>();
        using var reader = new StreamReader(baseFile, Encoding.UTF8);

        while (await reader.ReadLineAsync() is { } line)
        {
            if (line.Contains('{') || line.Contains('}')) continue;

            var cleanLine = line.Trim().TrimEnd(',').Replace("'", "");
            var parts = cleanLine.Split(new[] { ':' }, 2);
            if (parts.Length == 2) dic[parts[0].Trim()] = parts[1].Trim();
        }

        reader.Close();
        return dic;
    }

    private static async Task<Dictionary<string, string>> ReadLanguageFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw Oops.Bah($"[{filePath.Split('/').Last()}]File not found");
        }

        var dic = new Dictionary<string, string>();
        using var reader = new StreamReader(filePath, Encoding.UTF8);

        while (await reader.ReadLineAsync() is { } line)
        {
            if (line.Contains('{') || line.Contains('}')) continue;

            var cleanLine = line.Trim().TrimEnd(',').Replace("'", "");
            var parts = cleanLine.Split(new[] { ':' }, 2);
            if (parts.Length == 2) dic[parts[0].Trim()] = parts[1].Trim();
        }

        reader.Close();
        return dic;
    }

    private static void LogTranslationProgress(string key, string value, string res, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine($"Translation attribute: {key,-32} Value: {value,-64} Result: {res}");
        Console.ResetColor();
    }

    private static void LogError(Exception e)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($"{e.Message}");
        Console.ResetColor();
    }

    #endregion Helper method

#endif

    // Calculate MD5 value
    [NonAction]
    private static string EncryptString(string str)
    {
        MD5 md5 = MD5.Create();
        // Convert string to byte array
        byte[] byteOld = Encoding.UTF8.GetBytes(str);
        // Call encryption method
        byte[] byteNew = md5.ComputeHash(byteOld);
        // Convert encryption result to string
        StringBuilder sb = new StringBuilder();
        foreach (byte b in byteNew)
        {
            // Convert bytes into hexadecimal string representation,
            sb.Append(b.ToString("x2"));
        }

        // Returns the encrypted string
        return sb.ToString();
    }
}