// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

public static class ComputerUtil
{
    /// <summary>
    /// memory information
    /// </summary>
    /// <returns></returns>
    public static MemoryMetrics GetComputerInfo()
    {
        MemoryMetrics memoryMetrics;
        if (IsMacOS())
        {
            memoryMetrics = MemoryMetricsClient.GetMacOSMetrics();
        }
        else if (IsUnix())
        {
            memoryMetrics = MemoryMetricsClient.GetUnixMetrics();
        }
        else
        {
            memoryMetrics = MemoryMetricsClient.GetWindowsMetrics();
        }
        memoryMetrics.FreeRam = Math.Round(memoryMetrics.Free / 1024, 2) + "GB";
        memoryMetrics.UsedRam = Math.Round(memoryMetrics.Used / 1024, 2) + "GB";
        memoryMetrics.TotalRam = Math.Round(memoryMetrics.Total / 1024, 2) + "GB";
        memoryMetrics.RamRate = Math.Ceiling(100 * memoryMetrics.Used / memoryMetrics.Total) + "%";
        var cpuRates = GetCPURates();
        if (cpuRates != null)
        {
            memoryMetrics.CpuRates = cpuRates.Select(u => Math.Ceiling(u.ParseToDouble()) + "%").ToList();
        }
        memoryMetrics.CpuRate = memoryMetrics.CpuRates[0];
        return memoryMetrics;
    }

    /// <summary>
    /// Get the correct operating system version (Linux gets the distribution version)
    /// </summary>
    /// <returns></returns>
    public static String GetOSInfo()
    {
        string operation = string.Empty;
        if (IsMacOS())
        {
            var output = ShellUtil.Bash("sw_vers | awk 'NR<=2{printf \"%s \", $NF}'");
            if (output != null)
            {
                operation = output.Replace("%", string.Empty);
            }
        }
        else if (IsUnix())
        {
            var output = ShellUtil.Bash("awk -F= '/^VERSION_ID/ {print $2}' /etc/os-release | tr -d '\"'");
            operation = output ?? string.Empty;
        }
        else
        {
            operation = RuntimeInformation.OSDescription;
        }
        return operation;
    }

    /// <summary>
    /// disk information
    /// </summary>
    /// <returns></returns>
    public static List<DiskInfo> GetDiskInfos()
    {
        var diskInfos = new List<DiskInfo>();
        if (IsMacOS())
        {
            var output = ShellUtil.Bash(@"df -m | awk '/^\/dev\/disk/ {print $1,$2,$3,$4,$5}'");
            var disks = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (disks.Length < 1) return diskInfos;
            foreach (var item in disks)
            {
                var disk = item.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
                if (disk.Length < 5) continue;

                var diskInfo = new DiskInfo()
                {
                    DiskName = disk[0],
                    TypeName = ShellUtil.Bash("diskutil info " + disk[0] + " | awk '/File System Personality/ {print $4}'").Replace("\n", string.Empty),
                    TotalSize = Math.Round(long.Parse(disk[1]) / 1024.0m, 2, MidpointRounding.AwayFromZero),
                    Used = Math.Round(long.Parse(disk[2]) / 1024.0m, 2, MidpointRounding.AwayFromZero),
                    AvailableFreeSpace = Math.Round(long.Parse(disk[3]) / 1024.0m, 2, MidpointRounding.AwayFromZero),
                    AvailablePercent = decimal.Parse(disk[4].Replace("%", ""))
                };
                diskInfos.Add(diskInfo);
            }
        }
        else if (IsUnix())
        {
            var output = ShellUtil.Bash(@"df -mT | awk '/^\/dev\/(sd|vd|xvd|nvme|sda|vda|mapper)/ {print $1,$2,$3,$4,$5,$6}'");
            var disks = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (disks.Length < 1) return diskInfos;

            //var rootDisk = disks[1].Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
            //if (rootDisk == null || rootDisk.Length < 1)
            //    return diskInfos;

            foreach (var item in disks)
            {
                var disk = item.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
                if (disk.Length < 6) continue;

                var diskInfo = new DiskInfo()
                {
                    DiskName = disk[0],
                    TypeName = disk[1],
                    TotalSize = Math.Round(long.Parse(disk[2]) / 1024.0m, 2, MidpointRounding.AwayFromZero),
                    Used = Math.Round(long.Parse(disk[3]) / 1024.0m, 2, MidpointRounding.AwayFromZero),
                    AvailableFreeSpace = Math.Round(long.Parse(disk[4]) / 1024.0m, 2, MidpointRounding.AwayFromZero),
                    AvailablePercent = decimal.Parse(disk[5].Replace("%", ""))
                };
                diskInfos.Add(diskInfo);
            }
        }
        else
        {
            var driveList = DriveInfo.GetDrives().Where(u => u.IsReady);
            foreach (var item in driveList)
            {
                if (item.DriveType == DriveType.CDRom) continue;
                var diskInfo = new DiskInfo()
                {
                    DiskName = item.Name,
                    TypeName = item.DriveType.ToString(),
                    TotalSize = Math.Round(item.TotalSize / 1024 / 1024 / 1024.0m, 2, MidpointRounding.AwayFromZero),
                    AvailableFreeSpace = Math.Round(item.AvailableFreeSpace / 1024 / 1024 / 1024.0m, 2, MidpointRounding.AwayFromZero),
                };
                diskInfo.Used = diskInfo.TotalSize - diskInfo.AvailableFreeSpace;
                diskInfo.AvailablePercent = decimal.Ceiling(diskInfo.Used / (decimal)diskInfo.TotalSize * 100);
                diskInfos.Add(diskInfo);
            }
        }
        return diskInfos;
    }

    /// <summary>
    /// Get external IP address
    /// </summary>
    /// <returns></returns>
    public static string GetIpFromOnline()
    {
        try
        {
            var url = "https://4.ipw.cn";
            var httpRemoteService = App.GetRequiredService<IHttpRemoteService>();
            var ip = httpRemoteService.GetAsString(url);
            var (ipLocation, _, _) = CommonUtil.GetIpAddress(ip);
            return ip + " " + ipLocation;
        }
        catch
        {
            return "unknow";
        }
    }

    public static bool IsUnix()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }

    public static bool IsMacOS()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
    }

    public static List<string> GetCPURates()
    {
        var cpuRates = new List<string>();
        string output = "";
        if (IsMacOS())
        {
            output = ShellUtil.Bash("top -l 1 | grep \"CPU usage\" | awk '{print $3 + $5}'");
            cpuRates.Add(output.Trim());
        }
        else if (IsUnix())
        {
            output = ShellUtil.Bash("awk '{u=$2+$4; t=$2+$4+$5; if (NR==1){u1=u; t1=t;} else print ($2+$4-u1) * 100 / (t-t1); }' <(grep 'cpu ' /proc/stat) <(sleep 1;grep 'cpu ' /proc/stat)");
            cpuRates.Add(output.Trim());
        }
        else
        {
            try
            {
                output = ShellUtil.Cmd("wmic", "cpu get LoadPercentage");
            }
            catch (Exception)
            {
                output = ShellUtil.PowerShell("Get-CimInstance -ClassName Win32_Processor | Select-Object LoadPercentage");
                output = output.Replace("@", string.Empty).Replace("{", string.Empty).Replace("}", string.Empty).Replace("=", string.Empty).Trim();
            }
            cpuRates.AddRange(output.Replace("LoadPercentage", string.Empty).Trim().Split("\r\r\n"));
        }
        return cpuRates;
    }

    /// <summary>
    /// Get system running time
    /// </summary>
    /// <returns></returns>
    public static string GetRunTime()
    {
        string runTime = string.Empty;
        string output = "";
        if (IsMacOS())
        {
            // macOS gets the system startup time:
            // sysctl -n kern.boottime | awk '{print $4}' | tr -d ','
            // Return: 1705379131
            // Just use date formatting
            output = ShellUtil.Bash("date -r $(sysctl -n kern.boottime | awk '{print $4}' | tr -d ',') +\"%Y-%m-%d %H:%M:%S\"").Trim();
            runTime = DateTimeUtil.FormatTime((DateTime.Now - output.ParseToDateTime()).TotalMilliseconds.ToString().Split('.')[0].ParseToLong());
        }
        else if (IsUnix())
        {
            output = ShellUtil.Bash("date -d \"$(awk -F. '{print $1}' /proc/uptime) second ago\" +\"%Y-%m-%d %H:%M:%S\"").Trim();
            runTime = DateTimeUtil.FormatTime((DateTime.Now - output.ParseToDateTime()).TotalMilliseconds.ToString().Split('.')[0].ParseToLong());
        }
        else
        {
            try
            {
                output = ShellUtil.Cmd("wmic", "OS get LastBootUpTime/Value");
                string[] outputArr = output.Split('=', (char)StringSplitOptions.RemoveEmptyEntries);
                if (outputArr.Length == 2)
                    runTime = DateTimeUtil.FormatTime((DateTime.Now - outputArr[1].Split('.')[0].ParseToDateTime()).TotalMilliseconds.ToString().Split('.')[0].ParseToLong());
            }
            catch (Exception)
            {
                output = ShellUtil.PowerShell("Get-CimInstance -ClassName Win32_OperatingSystem | Select-Object LastBootUpTime");
                output = output.Replace("LastBootUpTime", string.Empty).Replace("@", string.Empty).Replace("{", string.Empty).Replace("}", string.Empty).Replace("=", string.Empty).Trim();
                runTime = DateTimeUtil.FormatTime((DateTime.Now - output.ParseToDateTime()).TotalMilliseconds.ToString().Split('.')[0].ParseToLong());
            }
        }
        return runTime;
    }
}

/// <summary>
/// memory information
/// </summary>
public class MemoryMetrics
{
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public double Total { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public double Used { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public double Free { get; set; }

    /// <summary>
    /// Used memory
    /// </summary>
    public string UsedRam { get; set; }

    /// <summary>
    /// CPU usage%
    /// </summary>
    public List<string> CpuRates { get; set; }

    public string CpuRate { get; set; }

    /// <summary>
    /// Total memory GB
    /// </summary>
    public string TotalRam { get; set; }

    /// <summary>
    /// Memory usage %
    /// </summary>
    public string RamRate { get; set; }

    /// <summary>
    /// free memory
    /// </summary>
    public string FreeRam { get; set; }
}

/// <summary>
/// disk information
/// </summary>
public class DiskInfo
{
    /// <summary>
    /// disk name
    /// </summary>
    public string DiskName { get; set; }

    /// <summary>
    /// Type name
    /// </summary>
    public string TypeName { get; set; }

    /// <summary>
    /// total surplus
    /// </summary>
    public decimal TotalFree { get; set; }

    /// <summary>
    /// total amount
    /// </summary>
    public decimal TotalSize { get; set; }

    /// <summary>
    /// Already used
    /// </summary>
    public decimal Used { get; set; }

    /// <summary>
    /// Available
    /// </summary>
    public decimal AvailableFreeSpace { get; set; }

    /// <summary>
    /// Use percentage
    /// </summary>
    public decimal AvailablePercent { get; set; }
}

public class MemoryMetricsClient
{
    /// <summary>
    /// Windows system obtains memory information
    /// </summary>
    /// <returns></returns>
    public static MemoryMetrics GetWindowsMetrics()
    {
        string output = "";
        var metrics = new MemoryMetrics();
        try
        {
            output = ShellUtil.Cmd("wmic", "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value");
            var lines = output.Trim().Split('\n', (char)StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length <= 1) return metrics;

            var freeMemoryParts = lines[0].Split('=', (char)StringSplitOptions.RemoveEmptyEntries);
            var totalMemoryParts = lines[1].Split('=', (char)StringSplitOptions.RemoveEmptyEntries);
            metrics.Total = Math.Round(double.Parse(totalMemoryParts[1]) / 1024, 0);
            metrics.Free = Math.Round(double.Parse(freeMemoryParts[1]) / 1024, 0);//m
        }
        catch (Exception)
        {
            output = ShellUtil.PowerShell("Get-CimInstance -ClassName Win32_OperatingSystem | Select-Object FreePhysicalMemory, TotalVisibleMemorySize");
            output = output.Replace("@", string.Empty).Replace("{", string.Empty).Replace("}", string.Empty).Trim();
            var lines = output.Trim().Split(';', (char)StringSplitOptions.RemoveEmptyEntries);

            // Skip headers and separators (usually the first two rows)
            if (lines.Length >= 2)
            {
                // Parse and convert to MB (original unit is KB)
                metrics.Free = Math.Round(double.Parse(lines[0].Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[1]) / 1024, 0);
                metrics.Total = Math.Round(double.Parse(lines[1].Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[1]) / 1024, 0);
            }
        }
        metrics.Used = metrics.Total - metrics.Free;

        return metrics;
    }

    /// <summary>
    /// Unix system acquisition
    /// </summary>
    /// <returns></returns>
    public static MemoryMetrics GetUnixMetrics()
    {
        string output = ShellUtil.Bash("awk '/MemTotal/ {total=$2} /MemAvailable/ {available=$2} END {print total,available}' /proc/meminfo");
        var metrics = new MemoryMetrics();
        var memory = output.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
        if (memory.Length != 2) return metrics;

        metrics.Total = double.Parse(memory[0]) / 1024;
        metrics.Free = double.Parse(memory[1]) / 1024;
        metrics.Used = metrics.Total - metrics.Free;
        return metrics;
    }

    /// <summary>
    /// macOS system acquisition
    /// </summary>
    /// <returns></returns>
    public static MemoryMetrics GetMacOSMetrics()
    {
        var metrics = new MemoryMetrics();
        //physical memory size
        var total = ShellUtil.Bash("sysctl -n hw.memsize | awk '{printf \"%.2f\", $1/1024/1024}'");
        metrics.Total = float.Parse(total.Replace("%", string.Empty));
        //TODO: Occupy memory, check efficiency
        var free = ShellUtil.Bash("top -l 1 -s 0 | awk '/PhysMem/ {print $6+$8}'");
        metrics.Free = float.Parse(free);
        metrics.Used = metrics.Total - metrics.Free;
        return metrics;
    }
}

public class ShellUtil
{
    /// <summary>
    /// linux system commands
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static string Bash(string command)
    {
        var escapedArgs = command.Replace("\"", "\\\"");
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{escapedArgs}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        process.Dispose();
        return result;
    }

    /// <summary>
    /// windows CMD system command
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string Cmd(string fileName, string args)
    {
        var info = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = args,
            RedirectStandardOutput = true
        };

        var output = string.Empty;
        using (var process = Process.Start(info))
        {
            output = process.StandardOutput.ReadToEnd();
        }
        return output;
    }

    /// <summary>
    /// Windows POWERSHELL system commands
    /// </summary>
    /// <param name="script"></param>
    /// <returns></returns>
    public static string PowerShell(string script)
    {
        using var PowerShellInstance = System.Management.Automation.PowerShell.Create();
        PowerShellInstance.AddScript(script);
        var PSOutput = PowerShellInstance.Invoke();

        var output = new StringBuilder();
        foreach (var outputItem in PSOutput)
        {
            output.AppendLine(outputItem.ToString());
        }
        return output.ToString();
    }
}

public class ShellHelper
{
    /// <summary>
    /// Linux system commands
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static string Bash(string command)
    {
        var escapedArgs = command.Replace("\"", "\\\"");
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{escapedArgs}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        process.Dispose();
        return result;
    }

    /// <summary>
    /// Windows CMD system command
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string Cmd(string fileName, string args)
    {
        var info = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = args,
            RedirectStandardOutput = true
        };

        var output = string.Empty;
        using (var process = Process.Start(info))
        {
            output = process.StandardOutput.ReadToEnd();
        }
        return output;
    }

    /// <summary>
    /// Windows POWERSHELL system commands
    /// </summary>
    /// <param name="script"></param>
    /// <returns></returns>
    public static string PowerShell(string script)
    {
        using var PowerShellInstance = System.Management.Automation.PowerShell.Create();
        PowerShellInstance.AddScript(script);
        var PSOutput = PowerShellInstance.Invoke();

        var output = new StringBuilder();
        foreach (var outputItem in PSOutput)
        {
            output.AppendLine(outputItem.BaseObject.ToString());
        }
        return output.ToString();
    }
}