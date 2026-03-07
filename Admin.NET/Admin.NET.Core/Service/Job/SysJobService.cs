// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System job task service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 320, Description = "Homework tasks")]
public class SysJobService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysJobDetail> _sysJobDetailRep;
    private readonly SqlSugarRepository<SysJobTrigger> _sysJobTriggerRep;
    private readonly SqlSugarRepository<SysJobTriggerRecord> _sysJobTriggerRecordRep;
    private readonly SqlSugarRepository<SysJobCluster> _sysJobClusterRep;
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly DynamicJobCompiler _dynamicJobCompiler;

    public SysJobService(SqlSugarRepository<SysJobDetail> sysJobDetailRep,
        SqlSugarRepository<SysJobTrigger> sysJobTriggerRep,
        SqlSugarRepository<SysJobTriggerRecord> sysJobTriggerRecordRep,
        SqlSugarRepository<SysJobCluster> sysJobClusterRep,
        ISchedulerFactory schedulerFactory,
        DynamicJobCompiler dynamicJobCompiler)
    {
        _sysJobDetailRep = sysJobDetailRep;
        _sysJobTriggerRep = sysJobTriggerRep;
        _sysJobTriggerRecordRep = sysJobTriggerRecordRep;
        _sysJobClusterRep = sysJobClusterRep;
        _schedulerFactory = schedulerFactory;
        _dynamicJobCompiler = dynamicJobCompiler;
    }

    /// <summary>
    /// Get the paged list of jobs ⏰
    /// </summary>
    [DisplayName("Get the homework pagination list")]
    public async Task<SqlSugarPagedList<JobDetailOutput>> PageJobDetail(PageJobDetailInput input)
    {
        var jobDetails = await _sysJobDetailRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.JobId), u => u.JobId.Contains(input.JobId.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.GroupName), u => u.GroupName.Contains(input.GroupName.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Description), u => u.Description.Contains(input.Description.Trim()))
            .Select(d => new JobDetailOutput
            {
                JobDetail = d,
            }).ToPagedListAsync(input.Page, input.PageSize);
        await _sysJobDetailRep.AsSugarClient().ThenMapperAsync(jobDetails.Items, async u =>
        {
            u.JobTriggers = await _sysJobTriggerRep.GetListAsync(t => t.JobId == u.JobDetail.JobId);
        });

        // Extract parameter values ​​inside square brackets
        var rgx = new Regex(@"(?i)(?<=\[)(.*)(?=\])");
        foreach (var job in jobDetails.Items)
        {
            foreach (var jobTrigger in job.JobTriggers)
            {
                jobTrigger.Args = rgx.Match(jobTrigger.Args ?? "").Value;
            }
        }
        return jobDetails;
    }

    /// <summary>
    /// Get the collection of job group names ⏰
    /// </summary>
    [DisplayName("Get the collection of job group names")]
    public async Task<List<string>> ListJobGroup()
    {
        return await _sysJobDetailRep.AsQueryable().Distinct().Select(e => e.GroupName).ToListAsync();
    }

    /// <summary>
    /// Add a job ⏰
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "AddJobDetail"), HttpPost]
    [DisplayName("Add Homework")]
    public async Task AddJobDetail(AddJobDetailInput input)
    {
        var isExist = await _sysJobDetailRep.IsAnyAsync(u => u.JobId == input.JobId && u.Id != input.Id);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1006);

        // Create jobs dynamically
        Type jobType;
        switch (input.CreateType)
        {
            case JobCreateTypeEnum.Script when string.IsNullOrEmpty(input.ScriptCode):
                throw Oops.Oh(ErrorCodeEnum.D1701);
            case JobCreateTypeEnum.Script:
                {
                    jobType = _dynamicJobCompiler.BuildJob(input.ScriptCode);

                    if (jobType.GetCustomAttributes(typeof(JobDetailAttribute)).FirstOrDefault() is not JobDetailAttribute jobDetailAttribute)
                        throw Oops.Oh(ErrorCodeEnum.D1702);
                    if (jobDetailAttribute.JobId != input.JobId)
                        throw Oops.Oh(ErrorCodeEnum.D1703);
                    break;
                }
            case JobCreateTypeEnum.Http:
                jobType = typeof(HttpJob);
                break;

            default:
                throw new NotSupportedException();
        }

        _schedulerFactory.AddJob(JobBuilder.Create(jobType).LoadFrom(input.Adapt<SysJobDetail>()).SetJobType(jobType));

        // Delay and wait for persistent writing before performing updates to other fields.
        await Task.Delay(500);
        await _sysJobDetailRep.AsUpdateable()
            .SetColumns(u => new SysJobDetail { CreateType = input.CreateType, ScriptCode = input.ScriptCode })
            .Where(u => u.JobId == input.JobId).ExecuteCommandAsync();
    }

    /// <summary>
    /// Update job ⏰
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "UpdateJobDetail"), HttpPost]
    [DisplayName("UpdateHomework")]
    public async Task UpdateJobDetail(UpdateJobDetailInput input)
    {
        var isExist = await _sysJobDetailRep.IsAnyAsync(u => u.JobId == input.JobId && u.Id != input.Id);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1006);

        var sysJobDetail = await _sysJobDetailRep.GetFirstAsync(u => u.Id == input.Id);
        if (sysJobDetail.JobId != input.JobId) throw Oops.Oh(ErrorCodeEnum.D1704);

        var scheduler = _schedulerFactory.GetJob(sysJobDetail.JobId);
        var oldScriptCode = sysJobDetail.ScriptCode; // old script code
        input.Adapt(sysJobDetail);

        if (input.CreateType == JobCreateTypeEnum.Script)
        {
            if (string.IsNullOrEmpty(input.ScriptCode)) throw Oops.Oh(ErrorCodeEnum.D1701);

            if (input.ScriptCode != oldScriptCode)
            {
                // Create jobs dynamically
                var jobType = _dynamicJobCompiler.BuildJob(input.ScriptCode);

                if (jobType.GetCustomAttributes(typeof(JobDetailAttribute)).FirstOrDefault() is not JobDetailAttribute jobDetailAttribute)
                    throw Oops.Oh(ErrorCodeEnum.D1702);
                if (jobDetailAttribute.JobId != input.JobId) throw Oops.Oh(ErrorCodeEnum.D1703);

                scheduler?.UpdateDetail(JobBuilder.Create(jobType).LoadFrom(sysJobDetail).SetJobType(jobType));
            }
        }
        else
        {
            scheduler?.UpdateDetail(scheduler.GetJobBuilder().LoadFrom(sysJobDetail));
        }

        // Tip: If the JobId is changed in this update, the persistent update execution triggered after changing the JobId will not be able to update the data because the JobId cannot be found.
        // Delay and wait for persistent writing before performing updates to other fields.
        await Task.Delay(500);
        await _sysJobDetailRep.UpdateAsync(sysJobDetail);
    }

    /// <summary>
    /// Delete job ⏰
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "DeleteJobDetail"), HttpPost]
    [DisplayName("Delete homework")]
    public async Task DeleteJobDetail(DeleteJobDetailInput input)
    {
        _schedulerFactory.RemoveJob(input.JobId);

        // If JodId does not exist in _schedulerFactory, persistence cannot be triggered. The following code ensures that jobs and triggers can be deleted
        await _sysJobDetailRep.DeleteAsync(u => u.JobId == input.JobId);
        await _sysJobTriggerRep.DeleteAsync(u => u.JobId == input.JobId);
    }

    /// <summary>
    /// Get trigger list ⏰
    /// </summary>
    [DisplayName("Get trigger list")]
    public async Task<List<SysJobTrigger>> GetJobTriggerList([FromQuery] JobDetailInput input)
    {
        return await _sysJobTriggerRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.JobId), u => u.JobId.Contains(input.JobId))
            .ToListAsync();
    }

    /// <summary>
    /// Add trigger ⏰
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "AddJobTrigger"), HttpPost]
    [DisplayName("Add trigger")]
    public async Task AddJobTrigger(AddJobTriggerInput input)
    {
        var isExist = await _sysJobTriggerRep.IsAnyAsync(u => u.TriggerId == input.TriggerId && u.Id != input.Id);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1006);

        var jobTrigger = input.Adapt<SysJobTrigger>();
        jobTrigger.Args = "[" + jobTrigger.Args + "]";

        var scheduler = _schedulerFactory.GetJob(input.JobId);
        scheduler?.AddTrigger(Triggers.Create(input.AssemblyName, input.TriggerType).LoadFrom(jobTrigger));
    }

    /// <summary>
    /// Update trigger ⏰
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "UpdateJobTrigger"), HttpPost]
    [DisplayName("update trigger")]
    public async Task UpdateJobTrigger(UpdateJobTriggerInput input)
    {
        var isExist = await _sysJobTriggerRep.IsAnyAsync(u => u.TriggerId == input.TriggerId && u.Id != input.Id);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1006);

        var jobTrigger = input.Adapt<SysJobTrigger>();
        if (jobTrigger.EndTime.HasValue && jobTrigger.EndTime.Value.Year < 1901)
        {
            jobTrigger.EndTime = null;
        }
        if (jobTrigger.StartTime.HasValue && jobTrigger.StartTime.Value.Year < 1901)
        {
            jobTrigger.StartTime = null;
        }
        jobTrigger.Args = "[" + jobTrigger.Args + "]";

        var scheduler = _schedulerFactory.GetJob(input.JobId);
        scheduler?.UpdateTrigger(Triggers.Create(input.AssemblyName, input.TriggerType).LoadFrom(jobTrigger));
    }

    /// <summary>
    /// Delete trigger ⏰
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "DeleteJobTrigger"), HttpPost]
    [DisplayName("Delete trigger")]
    public async Task DeleteJobTrigger(DeleteJobTriggerInput input)
    {
        var scheduler = _schedulerFactory.GetJob(input.JobId);
        scheduler?.RemoveTrigger(input.TriggerId);

        // If JodId does not exist in _schedulerFactory, persistence cannot be triggered. The following code ensures that the trigger can be deleted.
        await _sysJobTriggerRep.DeleteAsync(u => u.JobId == input.JobId && u.TriggerId == input.TriggerId);
    }

    /// <summary>
    /// Pause all jobs ⏰
    /// </summary>
    /// <returns></returns>
    [DisplayName("Pause all jobs")]
    public void PauseAllJob()
    {
        _schedulerFactory.PauseAll();
    }

    /// <summary>
    /// Start all jobs ⏰
    /// </summary>
    /// <returns></returns>
    [DisplayName("Start all jobs")]
    public void StartAllJob()
    {
        _schedulerFactory.StartAll();
    }

    /// <summary>
    /// Pause job ⏰
    /// </summary>
    [DisplayName("Suspend operations")]
    public void PauseJob(JobDetailInput input)
    {
        _schedulerFactory.TryPauseJob(input.JobId, out _);
    }

    /// <summary>
    /// Start job ⏰
    /// </summary>
    [DisplayName("Start Job")]
    public void StartJob(JobDetailInput input)
    {
        _schedulerFactory.TryStartJob(input.JobId, out _);
    }

    /// <summary>
    /// Cancel job ⏰
    /// </summary>
    [DisplayName("Cancel job")]
    public void CancelJob(JobDetailInput input)
    {
        _schedulerFactory.TryCancelJob(input.JobId, out _);
    }

    /// <summary>
    /// Execute job ⏰
    /// </summary>
    /// <param name="input"></param>
    [DisplayName("Execute job")]
    public void RunJob(JobDetailInput input)
    {
        if (_schedulerFactory.TryRunJob(input.JobId, out _) != ScheduleResult.Succeed) throw Oops.Oh(ErrorCodeEnum.D1705);
    }

    /// <summary>
    /// Pause trigger ⏰
    /// </summary>
    [DisplayName("Pause trigger")]
    public void PauseTrigger(JobTriggerInput input)
    {
        var scheduler = _schedulerFactory.GetJob(input.JobId);
        scheduler?.PauseTrigger(input.TriggerId);
    }

    /// <summary>
    /// Start trigger ⏰
    /// </summary>
    [DisplayName("Start trigger")]
    public void StartTrigger(JobTriggerInput input)
    {
        var scheduler = _schedulerFactory.GetJob(input.JobId);
        scheduler?.StartTrigger(input.TriggerId);
    }

    /// <summary>
    /// Force wake up job scheduler ⏰
    /// </summary>
    [DisplayName("Force wake up the job scheduler")]
    public void CancelSleep()
    {
        _schedulerFactory.CancelSleep();
    }

    /// <summary>
    /// Force the persistence of all jobs to be triggered ⏰
    /// </summary>
    [DisplayName("Force the persistence of all jobs to be triggered")]
    public void PersistAll()
    {
        _schedulerFactory.PersistAll();
    }

    /// <summary>
    /// Get cluster list ⏰
    /// </summary>
    [DisplayName("ObtainCluster List")]
    public async Task<List<SysJobCluster>> GetJobClusterList()
    {
        return await _sysJobClusterRep.GetListAsync();
    }

    /// <summary>
    /// Get the paged list of job trigger running records ⏰
    /// </summary>
    [DisplayName("ObtainJob trigger run recordspointsPage List")]
    public async Task<SqlSugarPagedList<SysJobTriggerRecord>> PageJobTriggerRecord(PageJobTriggerRecordInput input)
    {
        return await _sysJobTriggerRecordRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.JobId), u => u.JobId == input.JobId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.TriggerId), u => u.TriggerId == input.TriggerId)
            .OrderByDescending(u => u.Id)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// Clear job trigger running records 🔖
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "ClearJobTriggerRecord"), HttpPost]
    [DisplayName("Clear job trigger run records")]
    public void ClearJobTriggerRecord()
    {
        _sysJobTriggerRecordRep.AsSugarClient().DbMaintenance.TruncateTable<SysJobTriggerRecord>();
    }

    /// <summary>
    /// Clear unretained job trigger running records 🔖
    /// </summary>
    /// <returns></returns>
    [NonAction]
    [DisplayName("Clear expired job trigger running records")]
    public async Task ClearExpireJobTriggerRecord(SysJobTriggerRecord input)
    {
        int keepRecords = 30;// Number of records retained
        // Use CopyNew() to create a new database connection instance to avoid connection conflicts
        var db = _sysJobTriggerRecordRep.AsSugarClient().CopyNew();
        await db.Deleteable<SysJobTriggerRecord>().In(it => it.Id,
            db.Queryable<SysJobTriggerRecord>()
                .Skip(keepRecords)
                .OrderByDescending(it => it.LastRunTime)
                .Where(u => u.JobId == input.JobId && u.TriggerId == input.TriggerId)
                .Select(it => it.Id) // Note that Select does not require ToList(). ToList only requires two queries.
        ).ExecuteCommandAsync();
    }
}