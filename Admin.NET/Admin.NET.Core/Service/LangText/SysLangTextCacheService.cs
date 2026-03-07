// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

public class LangFieldMap<TEntity>
{
    /// <summary>Entity name, such as Product</summary>
    public string EntityName { get; set; }

    /// <summary>Field name, such as Name/Description</summary>
    public string FieldName { get; set; }

    /// <summary>How to get the primary key ID</summary>
    public Func<TEntity, long> IdSelector { get; set; }

    /// <summary>How to write back translation values</summary>
    public Action<TEntity, string> SetTranslatedValue { get; set; }
}

/// <summary>
/// Translation caching service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 100, Description = "Translation caching service")]
public class SysLangTextCacheService : IDynamicApiController, ITransient
{
    private readonly SysCacheService _sysCacheService;
    private readonly SqlSugarRepository<SysLangText> _sysLangTextRep;
    private TimeSpan expireSeconds = TimeSpan.FromHours(1);

    public SysLangTextCacheService(
        SysCacheService sysCacheService,
        SqlSugarRepository<SysLangText> sysLangTextRep)
    {
        _sysCacheService = sysCacheService;
        _sysLangTextRep = sysLangTextRep;
    }

    private string BuildKey(string entityName, string fieldName, long entityId, string langCode)
    {
        return $"LangCache_{entityName}_{fieldName}_{entityId}_{langCode}";
    }

    /// <summary>
    /// [Get a single translation]
    /// Get translated content based on entity type, field, primary key ID and language encoding. <br/>
    /// Applicable to: small tables (such as menus, dictionaries), long cache time can be set. <br/>
    /// <br/>
    /// 【Example】<br/>
    /// var content = await _sysLangTextCacheService.GetTranslation("Product", "Name", 123, "en-US");
    /// </summary>
    /// <param name="entityName">Entity name, such as "Product"</param>
    /// <param name="fieldName">Field name, such as "Name"</param>
    /// <param name="entityId">Entity primary key ID</param>
    /// <param name="langCode">Language encoding, such as "zh-CN"</param>
    /// <returns>Translated content (returns null or empty if none)</returns>
    [NonAction]
    public async Task<string> GetTranslation(string entityName, string fieldName, long entityId, string langCode)
    {
        var key = BuildKey(entityName, fieldName, entityId, langCode);
        var value = _sysCacheService.Get<string>(key);
        if (!string.IsNullOrEmpty(value)) return value;

        value = await _sysLangTextRep.AsQueryable()
            .Where(u => u.EntityName == entityName && u.FieldName == fieldName && u.EntityId == entityId && u.LangCode == langCode)
            .Select(u => u.Content)
            .FirstAsync();

        if (!string.IsNullOrEmpty(value))
        {
            _sysCacheService.Set(key, value, expireSeconds); // Set expiration
        }

        return value;
    }

    /// <summary>
    /// Get translation entities based on entity type, field, primary key ID and language encoding
    /// </summary>
    /// <param name="entityName">Entity name</param>
    /// <param name="fieldName">Field name</param>
    /// <param name="entityId">Entity primary key ID</param>
    /// <param name="langCode">language encoding</param>
    /// <returns></returns>
    [NonAction]
    public async Task<SysLangText> GetTranslationEntity(string entityName, string fieldName, long entityId, string langCode)
    {
        var key = BuildKey(entityName, fieldName, entityId, langCode) + "_entity";
        var value = _sysCacheService.Get<SysLangText>(key);
        if (!value.IsNullOrEmpty()) return value;

        value = await _sysLangTextRep.AsQueryable()
            .Where(u => u.EntityName == entityName && u.FieldName == fieldName && u.EntityId == entityId && u.LangCode == langCode)
            .FirstAsync();

        if (!value.IsNullOrEmpty())
        {
            _sysCacheService.Set(key, value, expireSeconds); // Set expiration
        }

        return value;
    }

    /// <summary>
    /// 【Batch translation acquisition】<br/>
    /// Obtain the corresponding translation content based on entities, fields and a batch of primary key IDs, and automatically obtain it from the cache or database. <br/>
    /// Suitable for: SKU, multiple products, batch dictionaries and other scenarios that require efficient batch acquisition. <br/>
    ///
    /// 【Example】<br/>
    /// var dict = await _sysLangTextCacheService.GetTranslations("SKU", "Name", skuIds, "en_US");
    /// </summary>
    /// <param name="entityName">Entity name</param>
    /// <param name="fieldName">Field name</param>
    /// <param name="entityIds">Primary key ID collection</param>
    /// <param name="langCode">language encoding</param>
    /// <returns>Dictionary from primary key ID to translated content</returns>
    [NonAction]
    public async Task<Dictionary<long, string>> GetTranslations(string entityName, string fieldName, List<long> entityIds, string langCode)
    {
        var result = new Dictionary<long, string>();
        var missingIds = new HashSet<long>(); // Use HashSet to improve the performance of subsequent Contains

        foreach (var id in entityIds.Distinct()) // Remove duplicates first to prevent repeated caching of Keys
        {
            var key = BuildKey(entityName, fieldName, id, langCode);
            var value = _sysCacheService.Get<string>(key);
            if (!string.IsNullOrWhiteSpace(value))
            {
                result[id] = value;
            }
            else
            {
                missingIds.Add(id);
            }
        }

        if (missingIds.Any())
        {
            var list = await _sysLangTextRep.AsQueryable()
                .Where(u => u.EntityName == entityName &&
                            u.FieldName == fieldName &&
                            missingIds.Contains(u.EntityId) &&
                            u.LangCode == langCode)
                .ToListAsync();

            foreach (var item in list)
            {
                if (string.IsNullOrWhiteSpace(item.Content)) continue; // Skip dirty data

                var key = BuildKey(item.EntityName, item.FieldName, item.EntityId, item.LangCode);
                _sysCacheService.Set(key, item.Content, expireSeconds);

                // Use TryAdd to prevent exceptions
                result[item.EntityId] = item.Content;
            }
        }

        return result;
    }

    /// <summary>
    /// 【List translation】<br/>
    /// Write the translation of the same field back to the entity list as configured. The batch translation interface will be called internally. <br/>
    /// <br/>
    /// 【Example】<br/>
    /// await _sysLangTextCacheService.TranslateList(products, "Product", "Name", p =&gt; p.Id, (p, val) =&gt; p.Name = val, "zh-CN");
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="list">List of entities to be translated</param>
    /// <param name="entityName">Entity name</param>
    /// <param name="fieldName">Field name</param>
    /// <param name="idSelector">Expression used to retrieve the primary key ID</param>
    /// <param name="setTranslatedValue">A delegate that writes back translated values</param>
    /// <param name="langCode">language encoding</param>
    /// <returns>Translated entity list (passed by reference)</returns>
    [NonAction]
    public async Task<List<TEntity>> TranslateList<TEntity>(List<TEntity> list, string entityName, string fieldName, Func<TEntity, long> idSelector, Action<TEntity, string> setTranslatedValue, string langCode)
    {
        var ids = list.Select(idSelector).Distinct().ToList();
        var dict = await GetTranslations(entityName, fieldName, ids, langCode);

        foreach (var item in list)
        {
            var id = idSelector(item);
            if (dict.TryGetValue(id, out var value))
            {
                setTranslatedValue(item, value);
            }
        }

        return list;
    }

    /// <summary>
    /// 【Multi-field batch translation】
    /// For the entity objects in the list, perform multi-field translation processing according to the configured field mapping. <br/>
    /// Commonly used in scenarios that require multi-field translation, such as menus in multiple languages, products in multiple languages, SKUs in multiple languages, etc. <br/><br/>
    /// ✅ Features:<br/>
    /// 1️⃣ Can translate multiple fields of the same entity at the same time (such as Name, Description, Title, etc.)<br/>
    /// 2️⃣ Internally try to read from the cache first. If the cache misses, query the database in batches and automatically write back to the cache<br/>
    /// 3️⃣ Pass by reference, assign value directly to the original entity object without additional return<br/><br/>
    /// [Usage example]:<br/>
    /// <code>
    /// var fields = new List&lt;LangFieldMap&lt;Product&gt;&gt;
    /// {
    ///     new LangFieldMap&lt;Product&gt; {
    ///         EntityName = "Product",
    ///         FieldName = "Name",
    ///         IdSelector = p =&gt; p.Id,
    ///         SetTranslatedValue = (p, val) =&gt; p.Name = val
    ///     },
    ///     new LangFieldMap&lt;Product&gt; {
    ///         EntityName = "Product",
    ///         FieldName = "Description",
    ///         IdSelector = p =&gt; p.Id,
    ///         SetTranslatedValue = (p, val) =&gt; p.Description = val
    ///     }
    /// };
    /// await _sysLangTextCacheService.TranslateMultiFields(products, fields, "zh-CN");
    /// </code>
    /// </summary>
    /// <typeparam name="TEntity">Entity type to be translated, such as Product/Menu/SKU, etc.</typeparam>
    /// <param name="list">List of entity objects that need to be translated</param>
    /// <param name="fields">A collection of field mappings that need to be translated, supporting multiple fields</param>
    /// <param name="langCode">Language encoding, such as "zh-CN", "en-US", "it-IT", etc.</param>
    /// <returns>Translated entity list (passed by reference, the original object has been directly assigned)</returns>
    [NonAction]
    public async Task<List<TEntity>> TranslateMultiFields<TEntity>(
    List<TEntity> list,
    List<LangFieldMap<TEntity>> fields,
    string langCode)
    {
        var keyToField = new Dictionary<string, (TEntity Entity, LangFieldMap<TEntity> FieldMap)>();
        var missingKeys = new List<string>();

        // Try reading from cache first
        foreach (var item in list)
        {
            foreach (var field in fields)
            {
                var id = field.IdSelector(item);
                var key = BuildKey(field.EntityName, field.FieldName, id, langCode);
                var cached = _sysCacheService.Get<string>(key);
                if (!string.IsNullOrEmpty(cached))
                {
                    // Hit the cache and assign directly
                    field.SetTranslatedValue(item, cached);
                }
                else
                {
                    // Cache miss, add to lookup table
                    keyToField[key] = (item, field);
                    missingKeys.Add(key);
                }
            }
        }

        if (missingKeys.Any())
        {
            // Decompose missing keys into composite entities
            var missingTuples = missingKeys
                .Select(key =>
                {
                    var parts = key.Split('_');
                    return new
                    {
                        EntityName = parts[1],
                        FieldName = parts[2],
                        EntityId = long.Parse(parts[3])
                    };
                })
                .ToList();

            // Group by EntityName + FieldName
            var grouped = missingTuples
                .GroupBy(x => new { x.EntityName, x.FieldName })
                .ToList();

            var result = new List<SysLangText>();

            // Query in batches, each group is queried separately
            const int chunkSize = 500;
            foreach (var g in grouped)
            {
                var allIds = g.Select(x => x.EntityId).Distinct().ToList();
                for (int i = 0; i < allIds.Count; i += chunkSize)
                {
                    var chunk = allIds.Skip(i).Take(chunkSize).ToList();
                    var temp = await _sysLangTextRep.AsQueryable()
                        .Where(u => u.LangCode == langCode
                                    && u.EntityName == g.Key.EntityName
                                    && u.FieldName == g.Key.FieldName
                                    && chunk.Contains(u.EntityId))
                        .ToListAsync();
                    result.AddRange(temp);
                }
            }

            // Traverse query results, write back entities and cache
            foreach (var item in result)
            {
                var key = BuildKey(item.EntityName, item.FieldName, item.EntityId, item.LangCode);
                if (keyToField.TryGetValue(key, out var tuple))
                {
                    tuple.FieldMap.SetTranslatedValue(tuple.Entity, item.Content);
                    _sysCacheService.Set(key, item.Content, expireSeconds);
                }
            }
        }

        return list;
    }

    /// <summary>
    /// Delete cache
    /// </summary>
    /// <param name="entityName"></param>
    /// <param name="fieldName"></param>
    /// <param name="entityId"></param>
    /// <param name="langCode"></param>
    public void DeleteCache(string entityName, string fieldName, long entityId, string langCode)
    {
        var key = BuildKey(entityName, fieldName, entityId, langCode);
        _sysCacheService.Remove(key);
    }

    /// <summary>
    /// Update cache
    /// </summary>
    /// <param name="entityName"></param>
    /// <param name="fieldName"></param>
    /// <param name="entityId"></param>
    /// <param name="langCode"></param>
    /// <param name="newValue"></param>
    public void UpdateCache(string entityName, string fieldName, long entityId, string langCode, string newValue)
    {
        var key = BuildKey(entityName, fieldName, entityId, langCode);
        _sysCacheService.Set(key, newValue, expireSeconds);
    }
}