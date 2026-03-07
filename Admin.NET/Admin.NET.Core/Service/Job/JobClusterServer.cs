// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// Job cluster control
/// </summary>
public class JobClusterServer : IJobClusterServer
{
    private static readonly SqlSugarRepository<SysJobCluster> _sysJobClusterRep = App.GetRequiredService<SqlSugarRepository<SysJobCluster>>();
    private readonly Random _random = new(DateTime.Now.Millisecond);

    /// <summary>
    /// Current job scheduler startup notification
    /// </summary>
    /// <param name="context">Job cluster service context</param>
    public async void Start(JobClusterContext context)
    {
        // In the job cluster table, if clusterId does not exist, add one (otherwise update one), and set status to ClusterStatus.Waiting
        if (await _sysJobClusterRep.IsAnyAsync(u => u.ClusterId == context.ClusterId))
        {
            await _sysJobClusterRep.AsUpdateable().SetColumns(u => u.Status == ClusterStatus.Waiting).Where(u => u.ClusterId == context.ClusterId).ExecuteCommandAsync();
        }
        else
        {
            await _sysJobClusterRep.AsInsertable(new SysJobCluster { ClusterId = context.ClusterId, Status = ClusterStatus.Waiting }).ExecuteCommandAsync();
        }
    }

    /// <summary>
    /// waiting to be awakened
    /// </summary>
    /// <param name="context">Job cluster service context</param>
    /// <returns><see cref="Task"/></returns>
    public async Task WaitingForAsync(JobClusterContext context)
    {
        var clusterId = context.ClusterId;

        while (true)
        {
            // Control the cluster heartbeat frequency (put it in the header to prevent IsAnyAsync continue from taking up a lot of IO and CPU without sleep)
            await Task.Delay(3000 + _random.Next(500, 1000)); // Start staggered clusters at the same time

            try
            {
                ICache cache = App.GetRequiredService<ICacheProvider>().Cache;
                // Use distributed locks
                using (cache.AcquireLock("lock:JobClusterServer:WaitingForAsync", 1000))
                {
                    // Query the database here and handle it according to the following two situations
                    // 1) If the job cluster table already has status ClusterStatus.Working, continue the cycle
                    // 2) If there are no other services in the job cluster table or only itself, insert a cluster service or call await WorkNowAsync(clusterId); and then return;
                    // 3) If there is no status of ClusterStatus.Working in the job cluster table, call await WorkNowAsync(clusterId); and then return;
                    if (await _sysJobClusterRep.IsAnyAsync(u => u.Status == ClusterStatus.Working)) continue;

                    await WorkNowAsync(clusterId);
                    return;
                }
            }
            catch { }
        }
    }

    /// <summary>
    /// Current job scheduler stop notification
    /// </summary>
    /// <param name="context">Job cluster service context</param>
    public async void Stop(JobClusterContext context)
    {
        // In the job cluster table, update the status of clusterId to ClusterStatus.Crashed
        await _sysJobClusterRep.UpdateAsync(u => new SysJobCluster { Status = ClusterStatus.Crashed }, u => u.ClusterId == context.ClusterId);
    }

    /// <summary>
    /// The current job scheduler is down
    /// </summary>
    /// <param name="context">Job cluster service context</param>
    public async void Crash(JobClusterContext context)
    {
        // In the job cluster table, update the status of clusterId to ClusterStatus.Crashed
        await _sysJobClusterRep.UpdateAsync(u => new SysJobCluster { Status = ClusterStatus.Crashed }, u => u.ClusterId == context.ClusterId);
    }

    /// <summary>
    /// Indicates that the cluster can work
    /// </summary>
    /// <param name="clusterId">Cluster ID</param>
    /// <returns></returns>
    private static async Task WorkNowAsync(string clusterId)
    {
        // In the job cluster table, update the status of clusterId to ClusterStatus.Working
        await _sysJobClusterRep.UpdateAsync(u => new SysJobCluster { Status = ClusterStatus.Working }, u => u.ClusterId == clusterId);
    }
}