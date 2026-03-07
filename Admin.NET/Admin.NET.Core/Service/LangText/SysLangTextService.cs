// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Newtonsoft.Json;

namespace Admin.NET.Core.Service;

/// <summary>
/// Translation service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 100, Description = "Translation services")]
public partial class SysLangTextService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysLangText> _sysLangTextRep;
    private readonly ISqlSugarClient _sqlSugarClient;
    private readonly SysLangTextCacheService _sysLangTextCacheService;

    public SysLangTextService(
        SqlSugarRepository<SysLangText> sysLangTextRep,
        SysLangTextCacheService sysLangTextCacheService,
        ISqlSugarClient sqlSugarClient)
    {
        _sysLangTextRep = sysLangTextRep;
        _sqlSugarClient = sqlSugarClient;
        _sysLangTextCacheService = sysLangTextCacheService;
    }

    /// <summary>
    /// Paginated query translation table 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Paginated query translation table")]
    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    public async Task<SqlSugarPagedList<SysLangTextOutput>> Page(PageSysLangTextInput input)
    {
        input.Keyword = input.Keyword?.Trim();
        var query = _sysLangTextRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Keyword), u => u.EntityName.Contains(input.Keyword) || u.FieldName.Contains(input.Keyword) || u.LangCode.Contains(input.Keyword) || u.Content.Contains(input.Keyword))
            .WhereIF(!string.IsNullOrWhiteSpace(input.EntityName), u => u.EntityName.Contains(input.EntityName.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.FieldName), u => u.FieldName.Contains(input.FieldName.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.LangCode), u => u.LangCode.Contains(input.LangCode.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Content), u => u.Content.Contains(input.Content.Trim()))
            .WhereIF(input.EntityId != null, u => u.EntityId == input.EntityId)
            .Select<SysLangTextOutput>();
        return await query.OrderBuilder(input).ToPagedListAsync(input.Page, input.PageSize);
    }

    [DisplayName("ObtainTranslation Table")]
    [ApiDescriptionSettings(Name = "List"), HttpPost]
    public async Task<List<SysLangTextOutput>> List(ListSysLangTextInput input)
    {
        var query = _sysLangTextRep.AsQueryable()
            .Where(u => u.EntityName == input.EntityName.Trim() && u.FieldName == input.FieldName.Trim() && u.EntityId == input.EntityId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.LangCode), u => u.LangCode == input.LangCode.Trim())
            .Select<SysLangTextOutput>();
        return await query.ToListAsync();
    }

    /// <summary>
    /// Get translation table details ℹ️
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get translation table details")]
    [ApiDescriptionSettings(Name = "Detail"), HttpGet]
    public async Task<SysLangText> Detail([FromQuery] QueryByIdSysLangTextInput input)
    {
        return await _sysLangTextRep.GetFirstAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// Add translation table ➕
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Add translation table")]
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    public async Task<long> Add(AddSysLangTextInput input)
    {
        var entity = input.Adapt<SysLangText>();
        return await _sysLangTextRep.InsertAsync(entity) ? entity.Id : 0;
    }

    /// <summary>
    /// Update translation table ✏️
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Update translation table")]
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    public async Task Update(UpdateSysLangTextInput input)
    {
        var entity = input.Adapt<SysLangText>();
        await _sysLangTextRep.AsUpdateable(entity)
        .ExecuteCommandAsync();
        _sysLangTextCacheService.UpdateCache(entity.EntityName, entity.FieldName, entity.EntityId, entity.LangCode, entity.Content);
    }

    /// <summary>
    /// Delete translation table ❌
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("DeleteTranslation Table")]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    public async Task Delete(DeleteSysLangTextInput input)
    {
        var entity = await _sysLangTextRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);

        await _sysLangTextRep.DeleteAsync(entity);   // Really delete
        _sysLangTextCacheService.DeleteCache(entity.EntityName, entity.FieldName, entity.EntityId, entity.LangCode);
    }

    /// <summary>
    /// Delete translation tables in batches ❌
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Batch delete translation table")]
    [ApiDescriptionSettings(Name = "BatchDelete"), HttpPost]
    public async Task BatchDelete([Required(ErrorMessage = "Primary key list cannot be empty")] List<DeleteSysLangTextInput> input)
    {
        var exp = Expressionable.Create<SysLangText>();
        foreach (var row in input) exp = exp.Or(it => it.Id == row.Id);
        var list = await _sysLangTextRep.AsQueryable().Where(exp.ToExpression()).ToListAsync();

        await _sysLangTextRep.DeleteAsync(list);   // Really delete
        foreach (var item in list)
        {
            _sysLangTextCacheService.DeleteCache(item.EntityName, item.FieldName, item.EntityId, item.LangCode);
        }
    }

    private static readonly object _sysLangTextBatchSaveLock = new object();

    /// <summary>
    /// Save translation tables in batches ✏️
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Batch save translation table")]
    [ApiDescriptionSettings(Name = "BatchSave"), HttpPost]
    public void BatchSave([Required(ErrorMessage = "The list cannot be empty")] List<ImportSysLangTextInput> input)
    {
        lock (_sysLangTextBatchSaveLock)
        {
            // Verify and filter fields whose required basic type is null
            var rows = input.Where(x =>
            {
                if (!string.IsNullOrWhiteSpace(x.Error)) return false;
                if (x.EntityId == null)
                {
                    x.Error = "The entity ID cannot be empty";
                    return false;
                }
                return true;
            }).Adapt<List<SysLangText>>();

            var storageable = _sysLangTextRep.Context.Storageable(rows)
                .SplitError(it => string.IsNullOrWhiteSpace(it.Item.EntityName), "The name of the entity to which it belongs cannot be empty.")
                .SplitError(it => it.Item.EntityName?.Length > 255, "The length of the entity name cannot exceed 255 characters.")
                .SplitError(it => string.IsNullOrWhiteSpace(it.Item.FieldName), "Field name cannot be empty")
                .SplitError(it => it.Item.FieldName?.Length > 255, "Field name length cannot exceed 255 characters")
                .SplitError(it => string.IsNullOrWhiteSpace(it.Item.LangCode), "Language code cannot be empty")
                .SplitError(it => it.Item.LangCode?.Length > 255, "The language code length cannot exceed 255 characters")
                .SplitError(it => string.IsNullOrWhiteSpace(it.Item.Content), "Translation content cannot be empty")
                .WhereColumns(it => new { it.EntityId, it.EntityName, it.FieldName, it.LangCode })
                .SplitInsert(it => it.NotAny())
                .SplitUpdate(it => it.Any())
                .ToStorage();

            storageable.AsInsertable.ExecuteCommand();// There is no insertion
            storageable.AsUpdateable.UpdateColumns(it => new
            {
                it.EntityName,
                it.EntityId,
                it.FieldName,
                it.LangCode,
                it.Content,
            }).ExecuteCommand();// There is an update
            foreach (var item in rows)
            {
                _sysLangTextCacheService.DeleteCache(item.EntityName, item.FieldName, item.EntityId, item.LangCode);
            }
            if (storageable.ErrorList.Any())
            {
                throw Oops.Oh($"The following error occurred during processing: {string.Join("；", storageable.ErrorList.Distinct())}");
            }
        }
    }

    /// <summary>
    /// Export translation table records 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Export translation table records")]
    [ApiDescriptionSettings(Name = "Export"), HttpPost, NonUnify]
    public async Task<IActionResult> Export(PageSysLangTextInput input)
    {
        var list = (await Page(input)).Items?.Adapt<List<ExportSysLangTextOutput>>() ?? new();
        if (input.SelectKeyList?.Count > 0) list = list.Where(x => input.SelectKeyList.Contains(x.Id)).ToList();
        return ExcelHelper.ExportTemplate(list, "Translation table export records");
    }

    /// <summary>
    /// Download translation table data import template ⬇️
    /// </summary>
    /// <returns></returns>
    [DisplayName("Download translation table data import template")]
    [ApiDescriptionSettings(Name = "Import"), HttpGet, NonUnify]
    public IActionResult DownloadTemplate()
    {
        return ExcelHelper.ExportTemplate(new List<ExportSysLangTextOutput>(), "Translation table import template");
    }

    private static readonly object _sysLangTextImportLock = new object();

    /// <summary>
    /// Import translation table records 💾
    /// </summary>
    /// <returns></returns>
    [DisplayName("Import translation table records")]
    [ApiDescriptionSettings(Name = "Import"), HttpPost, NonUnify, UnitOfWork]
    public IActionResult ImportData([Required] IFormFile file)
    {
        lock (_sysLangTextImportLock)
        {
            var stream = ExcelHelper.ImportData<ImportSysLangTextInput, SysLangText>(file, (list, markerErrorAction) =>
            {
                _sqlSugarClient.Utilities.PageEach(list, 2048, pageItems =>
                {
                    // Verify and filter fields whose required basic type is null
                    var rows = pageItems.Where(x =>
                    {
                        if (!string.IsNullOrWhiteSpace(x.Error)) return false;
                        if (x.EntityId == null)
                        {
                            x.Error = "The entity ID cannot be empty";
                            return false;
                        }
                        return true;
                    }).Adapt<List<SysLangText>>();

                    var storageable = _sysLangTextRep.Context.Storageable(rows)
                        .SplitError(it => string.IsNullOrWhiteSpace(it.Item.EntityName), "The name of the entity to which it belongs cannot be empty.")
                        .SplitError(it => it.Item.EntityName?.Length > 255, "The length of the entity name cannot exceed 255 characters.")
                        .SplitError(it => string.IsNullOrWhiteSpace(it.Item.FieldName), "Field name cannot be empty")
                        .SplitError(it => it.Item.FieldName?.Length > 255, "Field name length cannot exceed 255 characters")
                        .SplitError(it => string.IsNullOrWhiteSpace(it.Item.LangCode), "Language code cannot be empty")
                        .SplitError(it => it.Item.LangCode?.Length > 255, "The language code length cannot exceed 255 characters")
                        .SplitError(it => string.IsNullOrWhiteSpace(it.Item.Content), "Translation content cannot be empty")
                        .SplitError(it => it.Item.Content?.Length > 255, "Translated content cannot exceed 255 characters in length")
                        .WhereColumns(it => new { it.EntityId, it.EntityName, it.FieldName, it.LangCode })
                        .SplitInsert(it => it.NotAny())
                        .SplitUpdate(it => it.Any())
                        .ToStorage();

                    storageable.AsInsertable.ExecuteCommand();// There is no insertion
                    storageable.AsUpdateable.UpdateColumns(it => new
                    {
                        it.EntityName,
                        it.EntityId,
                        it.FieldName,
                        it.LangCode,
                        it.Content,
                    }).ExecuteCommand();// There is an update

                    foreach (var item in rows)
                    {
                        _sysLangTextCacheService.DeleteCache(item.EntityName, item.FieldName, item.EntityId, item.LangCode);
                    }
                    // Mark error message
                    markerErrorAction.Invoke(storageable, pageItems, rows);
                });
            });

            return stream;
        }
    }

    /// <summary>
    /// DEEPSEEK translation interface
    /// </summary>
    /// <returns></returns>
    [DisplayName("DEEPSEEK Translation API")]
    [ApiDescriptionSettings(Name = "AiTranslateText"), HttpPost]
    public async Task<string> AiTranslateText(AiTranslateTextInput input)
    {
        // You need to first copy DeepSeek.example and rename it to DeepSeek.json file, and add your API KEY
        var deepSeekOptions = App.GetConfig<DeepSeekOptions>("DeepSeekSettings", true);
        if (deepSeekOptions == null)
        {
            throw new InvalidOperationException("DeepSeek.jsonDocument Not yetDefinition");
        }
        if (string.IsNullOrEmpty(deepSeekOptions.ApiKey))
        {
            throw new InvalidOperationException("Environment variable DEEPSEEK_API_KEY is not defined");
        }

        using (HttpClient client = new HttpClient())
        {
            // Build request headers
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {deepSeekOptions.ApiKey}");

            // Build system prompt words
            string systemPrompt = BuildSystemPrompt(deepSeekOptions.SourceLang, input.TargetLang);

            // Build request body
            var requestBody = new
            {
                model = "deepseek-chat",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = input.OriginalText }
                },
                temperature = 0.3,
                max_tokens = 2000
            };

            // Serialization using Newtonsoft.Json
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send request
            HttpResponseMessage response = await client.PostAsync(deepSeekOptions.ApiUrl, content);

            // Handle response
            string responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                // Deserializing error responses using Newtonsoft.Json
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseBody);
                string errorMsg = errorResponse?.error?.message ?? $"HTTP {response.StatusCode}: {response.ReasonPhrase}";
                throw new HttpRequestException($"Translation API returned an error: {errorMsg}");
            }

            // Parse valid responses
            var result = JsonConvert.DeserializeObject<TranslationResponse>(responseBody);

            if (result?.choices == null || result.choices.Length == 0 ||
                result.choices[0]?.message?.content == null)
            {
                throw new InvalidOperationException("The API returned an invalid translation result");
            }

            return result.choices[0].message.content.Trim();
        }
    }

    // JSON response model
    private class TranslationResponse
    {
        public Choice[] choices { get; set; }
    }

    private class Choice
    {
        public Message message { get; set; }
    }

    private class Message
    {
        public string content { get; set; }
    }

    private class ErrorResponse
    {
        public ErrorInfo error { get; set; }
    }

    private class ErrorInfo
    {
        public string message { get; set; }
    }

    /// <summary>
    /// Generate prompt words
    /// </summary>
    /// <param name="sourceLang"></param>
    /// <param name="targetLang"></param>
    /// <returns></returns>
    private static string BuildSystemPrompt(string sourceLang, string targetLang)
    {
        return $@"As a professional translator of enterprise software systems，Strictly adhere to the following iron rules：

■ nuclearPrinciple of the Heart
1. Strictly translate symbol by symbol（{sourceLang}→{targetLang}）
2. ProhibitedAdd to/Delete/Rewrite anythingcontent
3. Keep batch translationNumberFormat

■ Symbol Retention Rules
! All symbols must be preserved as is：
• Programming symbols：\${{ }} <% %> @ # & |
• UIPlaceholder：{{0}} %s [ ]
• Currency unit：¥100.00 kg cm²
• inText symbol：【 】 《 》 ：

■ inSpecification for the Position of Text Symbols
# ThreeLevel processing mechanism：
1. Paired symbols must remain completewholeStructure：
   ✓ justSure：【Warning】Text
   ✗ Prohibited：Warning【 】Text

2. Independent symbol position：
   • Prefer sentence endings → Text】?
   • Secondary sentence starter → 】Text?
   • Prohibited sentencein → Text】Text?

3. Cross-string symbol processing：
   • Contains the front section【time → Keep at the end of the paragraph（""Synchronize【""）
   • Contains the latter part】time → Keep at the beginning of the paragraph（""】authorization data?""）
   • A symbol followed by a lettertimeAdd tonullgrid：】 Authorization

■ Grammar rules
• Foreign language → Passive voice（""Item was created""）
• intext → Active voice（""Project created""）
• Do not speculate about the context（Translate only the current stringcontent）

■ mistakePrevention（Absolutely prohibited）
✗ willinChange Chinese punctuation to Western-style punctuation（】→]）
✗ Mobile noninCharacter Symbol Position
✗ Add toThe original text does not existcontent
✗ Merge/demolishpointsOriginal string

■ Batch processing
▸ Strictly maintain the originalJSONStructure
▸ LanguageExact key name match（zh-cn/en/itWait）";
    }
}