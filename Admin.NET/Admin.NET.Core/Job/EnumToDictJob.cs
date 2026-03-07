// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Convert enumeration to dictionary
/// </summary>
[JobDetail("job_EnumToDictJob", Description = "Convert enumeration to dictionary", GroupName = "default", Concurrent = false)]
[PeriodSeconds(1, TriggerId = "trigger_EnumToDictJob", Description = "Convert enumeration to dictionary", MaxNumberOfRuns = 1, RunOnStart = true)]
public class EnumToDictJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private const string DefaultTagType = null;
    private const int OrderOffset = 10;

    public EnumToDictJob(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        var originColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[{DateTime.Now}] System Enum Conversion Dictionary");

        using var serviceScope = _scopeFactory.CreateScope();
        var db = serviceScope.ServiceProvider.GetRequiredService<ISqlSugarClient>().CopyNew();

        var sysEnumService = serviceScope.ServiceProvider.GetRequiredService<SysEnumService>();
        var sysDictTypeList = GetDictByEnumType(sysEnumService.GetEnumTypeList());

        // Verify the enumeration class naming convention. In dictionary-related functions, you need to use the suffix to determine whether it is an enumeration type.
        Console.ForegroundColor = ConsoleColor.Red;
        foreach (var dictType in sysDictTypeList.Where(x => !x.Code.EndsWith("Enum")))
            Console.WriteLine($"[{DateTime.Now}] The enumeration class name of the system enumeration conversion dictionary must end with Enum: {dictType.Code} ({dictType.Name})");
        sysDictTypeList = sysDictTypeList.Where(x => x.Code.EndsWith("Enum")).ToList();

        await SyncEnumToDictInfoAsync(db, sysDictTypeList);

        Console.ForegroundColor = ConsoleColor.Yellow;
        try
        {
            await db.BeginTranAsync();
            var storageable1 = await db.Storageable(sysDictTypeList)
                .SplitUpdate(it => it.Any())
                .SplitInsert(_ => true)
                .ToStorageAsync();
            await storageable1.AsInsertable.ExecuteCommandAsync(stoppingToken);
            await storageable1.AsUpdateable.ExecuteCommandAsync(stoppingToken);

            Console.WriteLine($"[{DateTime.Now}] System enumeration class to dictionary type data: insert {storageable1.InsertList.Count} items, update {storageable1.UpdateList.Count} items, totaling {storageable1.TotalList.Count} items.");

            var storageable2 = await db.Storageable(sysDictTypeList.SelectMany(x => x.Children).ToList())
                .WhereColumns(it => new { it.DictTypeId, it.Value })
                .SplitUpdate(it => it.Any())
                .SplitInsert(_ => true)
                .ToStorageAsync();
            await storageable2.AsInsertable.ExecuteCommandAsync(stoppingToken);
            await storageable2.AsUpdateable.UpdateColumns(u => new
            {
                u.Label,
                u.Code,
                u.Value
            }).ExecuteCommandAsync(stoppingToken);

            Console.WriteLine($"[{DateTime.Now}] Convert system enumeration items to dictionary value data: insert {storageable2.InsertList.Count} items, update {storageable2.UpdateList.Count} items, totaling {storageable2.TotalList.Count} items.");

            await db.CommitTranAsync();
        }
        catch (Exception error)
        {
            await db.RollbackTranAsync();
            Log.Error($"System enum conversion dictionary operation error: {error.Message}
Stack trace: {error.StackTrace}", error);
            throw;
        }
        finally
        {
            Console.ForegroundColor = originColor;
        }
    }

    /// <summary>
    /// Used to synchronize enumeration to dictionary data
    /// </summary>
    /// <param name="db"></param>
    /// <param name="list"></param>
    private async Task SyncEnumToDictInfoAsync(SqlSugarClient db, List<SysDictType> list)
    {
        var codeList = list.Select(x => x.Code).ToList();
        foreach (var dbDictType in await db.Queryable<SysDictType>().ClearFilter().Where(x => codeList.Contains(x.Code)).ToListAsync() ?? new())
        {
            var enumDictType = list.First(x => x.Code == dbDictType.Code);
            if (enumDictType.Id == dbDictType.Id)
            {
                // After the dictionary value table field is changed, there will be one more dictionary value record for each dictionary value record. This is used to delete redundant dictionary value data.
                var dataValueList = enumDictType.Children.Select(e => e.Value).ToList();
                await db.Deleteable<SysDictData>().Where(x => x.DictTypeId == dbDictType.Id && !dataValueList.Contains(x.Value)).ExecuteCommandAsync();
                continue;
            }

            // If the data is inconsistent, delete it
            await db.Deleteable<SysDictData>().Where(x => x.DictTypeId == dbDictType.Id).ExecuteCommandAsync();
            await db.Deleteable<SysDictType>().Where(x => x.Id == dbDictType.Id).ExecuteCommandAsync();
            Console.WriteLine($"[{DateTime.Now}] Delete dictionary data: {dbDictType.Name}-{dbDictType.Code}");
        }
    }

    /// <summary>
    /// Convert enumeration information to dictionary
    /// </summary>
    /// <param name="enumTypeList"></param>
    /// <returns></returns>
    private List<SysDictType> GetDictByEnumType(List<EnumTypeOutput> enumTypeList)
    {
        var orderNo = 1;
        var list = new List<SysDictType>();
        foreach (var type in enumTypeList)
        {
            var dictType = new SysDictType
            {
                Id = 900000000000 + CommonUtil.GetFixedHashCode(type.TypeFullName),
                SysFlag = YesNoEnum.Y,
                Code = type.TypeName,
                Name = type.TypeDescribe,
                Remark = type.TypeFullName
            };
            dictType.Children = type.EnumEntities.Select(x => new SysDictData
            {
                Id = dictType.Id + orderNo++,
                DictTypeId = dictType.Id,
                Code = x.Name,
                Label = x.Describe,
                Value = x.Value.ToString(),
                OrderNo = x.Value + OrderOffset,
                TagType = x.Theme != "" ? x.Theme : DefaultTagType
            }).ToList();
            list.Add(dictType);
        }
        return list;
    }
}