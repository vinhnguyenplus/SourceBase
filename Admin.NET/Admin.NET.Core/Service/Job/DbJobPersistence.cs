// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// Job persistence (database)
/// </summary>
public class DbJobPersistence : IJobPersistence
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public DbJobPersistence(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    /// <summary>
    /// When the job scheduling service starts
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public async Task<IEnumerable<SchedulerBuilder>> PreloadAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>().CopyNew();
        var dynamicJobCompiler = scope.ServiceProvider.GetRequiredService<DynamicJobCompiler>();

        // Get all defined jobs
        var allJobs = App.EffectiveTypes.ScanToBuilders().ToList();
        // If there is no job in the database, return directly
        if (!await db.Queryable<SysJobDetail>().AnyAsync(u => true, stoppingToken)) return allJobs;

        // Iterate through all defined jobs
        foreach (var schedulerBuilder in allJobs)
        {
            // Get job information builder
            var jobBuilder = schedulerBuilder.GetJobBuilder();

            // Load database data
            var dbDetail = await db.Queryable<SysJobDetail>().FirstAsync(u => u.JobId == jobBuilder.JobId, stoppingToken);
            if (dbDetail == null) continue;

            // Synchronize database data
            jobBuilder.LoadFrom(dbDetail);

            // Get triggers for all databases of a job
            var dbTriggers = await db.Queryable<SysJobTrigger>().Where(u => u.JobId == jobBuilder.JobId).ToListAsync(stoppingToken);
            // Iterate through all job triggers
            foreach (var (_, triggerBuilder) in schedulerBuilder.GetEnumerable())
            {
                // Load database data
                var dbTrigger = dbTriggers.FirstOrDefault(u => u.JobId == jobBuilder.JobId && u.TriggerId == triggerBuilder.TriggerId);
                if (dbTrigger == null) continue;

                triggerBuilder.LoadFrom(dbTrigger).Updated(); // Mark updates
            }
            // Traverse all non-compile-time defined triggers and add them to the job
            foreach (var dbTrigger in dbTriggers)
            {
                if (schedulerBuilder.GetTriggerBuilder(dbTrigger.TriggerId)?.JobId == jobBuilder.JobId) continue;
                var triggerBuilder = TriggerBuilder.Create(dbTrigger.TriggerId).LoadFrom(dbTrigger);
                schedulerBuilder.AddTriggerBuilder(triggerBuilder); // add first
                triggerBuilder.Updated(); // mark update again
            }

            // Mark updates
            schedulerBuilder.Updated();
        }

        // Get all jobs created by scripts in the database
        var allDbScriptJobs = await db.Queryable<SysJobDetail>().Where(u => u.CreateType != JobCreateTypeEnum.BuiltIn).ToListAsync(stoppingToken);
        foreach (var dbDetail in allDbScriptJobs)
        {
            // Create jobs dynamically
            Type jobType = dbDetail.CreateType switch
            {
                JobCreateTypeEnum.Script => dynamicJobCompiler.BuildJob(dbDetail.ScriptCode),
                JobCreateTypeEnum.Http => typeof(HttpJob),
                _ => throw new NotSupportedException(),
            };

            // The assembly name of the dynamically built jobType is a random name and needs to be reset.
            dbDetail.AssemblyName = jobType.Assembly.FullName!.Split(',')[0];
            var jobBuilder = JobBuilder.Create(jobType).LoadFrom(dbDetail);

            // Forcibly set not to scan the IJob implementation class [Trigger] feature trigger, otherwise SchedulerBuilder.Create will scan again, resulting in repeated addition of triggers with the same name.
            jobBuilder.SetIncludeAnnotations(false);

            // Get the triggers of all databases of the job and add them to the job
            var dbTriggers = await db.Queryable<SysJobTrigger>().Where(u => u.JobId == jobBuilder.JobId).ToListAsync();
            var triggerBuilders = dbTriggers.Select(u => TriggerBuilder.Create(u.TriggerId).LoadFrom(u).Updated());
            var schedulerBuilder = SchedulerBuilder.Create(jobBuilder, triggerBuilders.ToArray());

            // Mark updates
            schedulerBuilder.Updated();

            allJobs.Add(schedulerBuilder);
        }

        return allJobs;
    }

    /// <summary>
    /// Job plan initialization notification
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    public Task<SchedulerBuilder> OnLoadingAsync(SchedulerBuilder builder, CancellationToken stoppingToken)
    {
        return Task.FromResult(builder);
    }

    /// <summary>
    /// When the JobDetail of the job plan Scheduler changes
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task OnChangedAsync(PersistenceContext context)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>().CopyNew();

        var jobDetail = context.JobDetail.Adapt<SysJobDetail>();
        switch (context.Behavior)
        {
            case PersistenceBehavior.Appended:
                await db.Insertable(jobDetail).ExecuteCommandAsync();
                break;

            case PersistenceBehavior.Updated:
                await db.Updateable(jobDetail).WhereColumns(u => new { u.JobId }).IgnoreColumns(u => new { u.Id, u.CreateType, u.ScriptCode }).ExecuteCommandAsync();
                break;

            case PersistenceBehavior.Removed:
                await db.Deleteable<SysJobDetail>().Where(u => u.JobId == jobDetail.JobId).ExecuteCommandAsync();
                break;
        }
    }

    /// <summary>
    /// When the trigger Trigger of the job plan Scheduler changes
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task OnTriggerChangedAsync(PersistenceTriggerContext context)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>().CopyNew();

        var jobTrigger = context.Trigger.Adapt<SysJobTrigger>();
        switch (context.Behavior)
        {
            case PersistenceBehavior.Appended:
                await db.Insertable(jobTrigger).ExecuteCommandAsync();
                break;

            case PersistenceBehavior.Updated:
                await db.Updateable(jobTrigger).WhereColumns(u => new { u.TriggerId, u.JobId }).IgnoreColumns(u => new { u.Id }).ExecuteCommandAsync();
                break;

            case PersistenceBehavior.Removed:
                await db.Deleteable<SysJobTrigger>().Where(u => u.TriggerId == jobTrigger.TriggerId && u.JobId == jobTrigger.JobId).ExecuteCommandAsync();
                break;
        }
    }

    /// <summary>
    /// Job trigger run record
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task OnExecutionRecordAsync(PersistenceExecutionRecordContext context)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>().CopyNew();

        var jobTriggerRecord = context.Timeline.Adapt<SysJobTriggerRecord>();
        await db.Insertable(jobTriggerRecord).ExecuteCommandAsync();

        await scope.ServiceProvider.GetRequiredService<SysJobService>().ClearExpireJobTriggerRecord(jobTriggerRecord);
    }
}