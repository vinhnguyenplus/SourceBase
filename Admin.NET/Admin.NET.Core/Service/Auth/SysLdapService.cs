// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Novell.Directory.Ldap;

namespace Admin.NET.Core;

/// <summary>
/// System domain login configuration service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 496, Description = "Domain login configuration")]
public class SysLdapService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysLdap> _sysLdapRep;

    public SysLdapService(SqlSugarRepository<SysLdap> sysLdapRep)
    {
        _sysLdapRep = sysLdapRep;
    }

    /// <summary>
    /// Get the system domain login configuration paging list 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get the system domain login configuration paging list")]
    public async Task<SqlSugarPagedList<SysLdap>> Page(SysLdapInput input)
    {
        return await _sysLdapRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Keyword), u => u.Host.Contains(input.Keyword.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Host), u => u.Host.Contains(input.Host.Trim()))
            .OrderBy(u => u.CreateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// Add system domain login configuration 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("Add system domain login configuration")]
    public async Task<long> Add(AddSysLdapInput input)
    {
        var entity = input.Adapt<SysLdap>();
        entity.BindPass = CryptogramUtil.Encrypt(input.BindPass);
        await _sysLdapRep.InsertAsync(entity);
        return entity.Id;
    }

    /// <summary>
    /// Update system domain login configuration 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("Update system domain login configuration")]
    public async Task Update(UpdateSysLdapInput input)
    {
        var entity = input.Adapt<SysLdap>();
        if (!string.IsNullOrEmpty(input.BindPass) && input.BindPass.Length < 32)
        {
            entity.BindPass = CryptogramUtil.Encrypt(input.BindPass); // encryption
        }

        await _sysLdapRep.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
    }

    /// <summary>
    /// Delete system domain login configuration 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("Delete system domain login configuration")]
    public async Task Delete(DeleteSysLdapInput input)
    {
        var entity = await _sysLdapRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        await _sysLdapRep.FakeDeleteAsync(entity); // fake delete
        //await _rep.DeleteAsync(entity); // true delete
    }

    /// <summary>
    /// Get system domain login configuration details 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get system domain login configuration details")]
    public async Task<SysLdap> GetDetail([FromQuery] DetailSysLdapInput input)
    {
        return await _sysLdapRep.GetFirstAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// Get the system domain login configuration list 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get system domain login configuration list")]
    public async Task<List<SysLdap>> GetList()
    {
        return await _sysLdapRep.AsQueryable().Select<SysLdap>().ToListAsync();
    }

    /// <summary>
    /// Verify account
    /// </summary>
    /// <param name="account">domain user</param>
    /// <param name="password">password</param>
    /// <param name="tenantId">tenant</param>
    /// <returns></returns>
    [NonAction]
    public async Task<bool> AuthAccount(long? tenantId, string account, string password)
    {
        var sysLdap = await _sysLdapRep.GetFirstAsync(u => u.TenantId == tenantId) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        var ldapConn = new LdapConnection();
        try
        {
            await ldapConn.ConnectAsync(sysLdap.Host, sysLdap.Port);
            string bindPass = CryptogramUtil.Decrypt(sysLdap.BindPass);
            await ldapConn.BindAsync(sysLdap.Version, sysLdap.BindDn, bindPass);
            var ldapSearchResults = await ldapConn.SearchAsync(sysLdap.BaseDn, LdapConnection.ScopeSub, sysLdap.AuthFilter.Replace("%s", account), null, false);
            string dn = string.Empty;
            while (await ldapSearchResults.HasMoreAsync())
            {
                var ldapEntry = await ldapSearchResults.NextAsync();
                var sAmAccountName = ldapEntry.GetAttributeSet().GetAttribute(sysLdap.BindAttrAccount)?.StringValue;
                if (string.IsNullOrEmpty(sAmAccountName)) continue;
                dn = ldapEntry.Dn;
                break;
            }

            if (string.IsNullOrEmpty(dn)) throw Oops.Oh(ErrorCodeEnum.D1002);
            // var attr = new LdapAttribute("userPassword", password);
            await ldapConn.BindAsync(dn, password);
        }
        catch (LdapException e)
        {
            return e.ResultCode switch
            {
                LdapException.NoSuchObject or LdapException.NoSuchAttribute => throw Oops.Oh(ErrorCodeEnum.D0009),
                LdapException.InvalidCredentials => false,
                _ => throw Oops.Oh(e.Message),
            };
        }
        finally
        {
            ldapConn.Disconnect();
        }

        return true;
    }

    /// <summary>
    /// Sync domain users 🔖
    /// </summary>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    [DisplayName("Synchronized Domain User")]
    [NonAction]
    public async Task<List<SysUserLdap>> SyncUserTenant(long tenantId)
    {
        var sysLdap = await _sysLdapRep.GetFirstAsync(c => c.TenantId == tenantId && c.IsDelete == false && c.Status == StatusEnum.Enable) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        return await SysLdapService.SyncUser(sysLdap);
    }

    /// <summary>
    /// Sync domain users 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Synchronized Domain User")]
    public async Task<List<SysUserLdap>> SyncUser(SyncSysLdapInput input)
    {
        var sysLdap = await _sysLdapRep.GetByIdAsync(input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        return await SysLdapService.SyncUser(sysLdap);
    }

    /// <summary>
    /// Sync domain users 🔖
    /// </summary>
    /// <param name="sysLdap"></param>
    /// <returns></returns>
    private static async Task<List<SysUserLdap>> SyncUser(SysLdap sysLdap)
    {
        if (sysLdap == null) throw Oops.Oh(ErrorCodeEnum.D1002);
        var ldapConn = new LdapConnection();
        try
        {
            await ldapConn.ConnectAsync(sysLdap.Host, sysLdap.Port);
            string bindPass = CryptogramUtil.Decrypt(sysLdap.BindPass);
            await ldapConn.BindAsync(sysLdap.Version, sysLdap.BindDn, bindPass);
            var ldapSearchResults = await ldapConn.SearchAsync(sysLdap.BaseDn, LdapConnection.ScopeOne, "(objectClass=*)", null, false);
            var userLdapList = new List<SysUserLdap>();
            while (await ldapSearchResults.HasMoreAsync())
            {
                LdapEntry ldapEntry;
                try
                {
                    ldapEntry = await ldapSearchResults.NextAsync();
                    if (ldapEntry == null) continue;
                }
                catch (LdapException)
                {
                    continue;
                }

                var attrs = ldapEntry.GetAttributeSet();
                var deptCode = GetDepartmentCode(attrs, sysLdap.BindAttrCode);
                if (attrs.Count == 0 || attrs.ContainsKey("OU"))
                {
                    await SearchDnLdapUser(ldapConn, sysLdap, userLdapList, ldapEntry.Dn, deptCode);
                }
                else
                {
                    var sysUserLdap = CreateSysUserLdap(attrs, sysLdap.BindAttrAccount, sysLdap.BindAttrEmployeeId, deptCode);
                    sysUserLdap.Dn = ldapEntry.Dn;
                    sysUserLdap.TenantId = sysLdap.TenantId;
                    userLdapList.Add(sysUserLdap);
                }
            }

            if (userLdapList.Count == 0) return null;

            await App.GetRequiredService<SysUserLdapService>().InsertUserLdapList(sysLdap.TenantId!.Value, userLdapList);
            return userLdapList;
        }
        catch (LdapException e)
        {
            throw e.ResultCode switch
            {
                LdapException.NoSuchObject or LdapException.NoSuchAttribute => Oops.Oh(ErrorCodeEnum.D0009),
                _ => Oops.Oh(e.Message),
            };
        }
        finally
        {
            ldapConn.Disconnect();
        }
    }

    /// <summary>
    /// Get department code
    /// </summary>
    /// <param name="attrs"></param>
    /// <param name="bindAttrCode"></param>
    /// <returns></returns>
    private static string GetDepartmentCode(LdapAttributeSet attrs, string bindAttrCode)
    {
        return bindAttrCode == "objectGUID"
            ? new Guid(attrs.GetAttribute(bindAttrCode)?.ByteValue!).ToString()
            : attrs.GetAttribute(bindAttrCode)?.StringValue ?? "0";
    }

    /// <summary>
    /// Create synchronization object
    /// </summary>
    /// <param name="attrs"></param>
    /// <param name="bindAttrAccount"></param>
    /// <param name="bindAttrEmployeeId"></param>
    /// <param name="deptCode"></param>
    /// <returns></returns>
    private static SysUserLdap CreateSysUserLdap(LdapAttributeSet attrs, string bindAttrAccount, string bindAttrEmployeeId, string deptCode)
    {
        var userLdap = new SysUserLdap
        {
            Account = attrs.ContainsKey(bindAttrAccount) ? attrs.GetAttribute(bindAttrAccount)?.StringValue : null,
            EmployeeId = attrs.ContainsKey(bindAttrEmployeeId) ? attrs.GetAttribute(bindAttrEmployeeId)?.StringValue : null,
            DeptCode = deptCode,
            UserName = attrs.ContainsKey("name") ? attrs.GetAttribute("name")?.StringValue : null,
            Mail = attrs.ContainsKey("mail") ? attrs.GetAttribute("mail")?.StringValue : null
        };
        var pwdLastSet = attrs.ContainsKey("pwdLastSet") ? attrs.GetAttribute("pwdLastSet")?.StringValue : null;
        if (pwdLastSet != null && !pwdLastSet.Equals("0")) userLdap.PwdLastSetTime = DateTime.FromFileTime(Convert.ToInt64(pwdLastSet));
        var userAccountControl = attrs.ContainsKey("userAccountControl") ? attrs.GetAttribute("userAccountControl")?.StringValue : null;
        if ((Convert.ToInt32(userAccountControl) & 0x2) == 0x2) // Check if the account has expired (by checking specific bits of the userAccountControl attribute)
            userLdap.AccountExpiresFlag = true;
        if ((Convert.ToInt32(userAccountControl) & 0x10000) == 0x10000) // Check whether the account password setting never expires
            userLdap.DontExpiresFlag = true;
        return userLdap;
    }

    /// <summary>
    /// Traverse query domain users
    /// </summary>
    /// <param name="ldapConn"></param>
    /// <param name="sysLdap"></param>
    /// <param name="userLdapList"></param>
    /// <param name="baseDn"></param>
    /// <param name="deptCode"></param>
    private static async Task SearchDnLdapUser(LdapConnection ldapConn, SysLdap sysLdap, List<SysUserLdap> userLdapList, string baseDn, string deptCode)
    {
        var ldapSearchResults = await ldapConn.SearchAsync(baseDn, LdapConnection.ScopeOne, "(objectClass=*)", null, false);
        while (await ldapSearchResults.HasMoreAsync())
        {
            LdapEntry ldapEntry;
            try
            {
                ldapEntry = await ldapSearchResults.NextAsync();
                if (ldapEntry == null) continue;
            }
            catch (LdapException)
            {
                continue;
            }

            var attrs = ldapEntry.GetAttributeSet();
            deptCode = GetDepartmentCode(attrs, sysLdap.BindAttrCode);

            if (attrs.Count == 0 || attrs.ContainsKey("OU"))
                await SearchDnLdapUser(ldapConn, sysLdap, userLdapList, ldapEntry.Dn, deptCode);
            else
            {
                var sysUserLdap = CreateSysUserLdap(attrs, sysLdap.BindAttrAccount, sysLdap.BindAttrEmployeeId, deptCode);
                sysUserLdap.Dn = ldapEntry.Dn;
                sysUserLdap.TenantId = sysLdap.TenantId;
                if (string.IsNullOrEmpty(sysUserLdap.EmployeeId)) continue;
                userLdapList.Add(sysUserLdap);
            }
        }
    }

    /// <summary>
    /// Sync Domain Organization 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Synchronize domain organization")]
    public async Task SyncDept(SyncSysLdapInput input)
    {
        var sysLdap = await _sysLdapRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        var ldapConn = new LdapConnection();
        try
        {
            await ldapConn.ConnectAsync(sysLdap.Host, sysLdap.Port);
            string bindPass = CryptogramUtil.Decrypt(sysLdap.BindPass);
            await ldapConn.BindAsync(sysLdap.Version, sysLdap.BindDn, bindPass);
            var ldapSearchResults = await ldapConn.SearchAsync(sysLdap.BaseDn, LdapConnection.ScopeOne, "(objectClass=*)", null, false);
            var orgList = new List<SysOrg>();
            while (await ldapSearchResults.HasMoreAsync())
            {
                LdapEntry ldapEntry;
                try
                {
                    ldapEntry = await ldapSearchResults.NextAsync();
                    if (ldapEntry == null) continue;
                }
                catch (LdapException)
                {
                    continue;
                }

                var attrs = ldapEntry.GetAttributeSet();
                if (attrs.Count != 0 && !attrs.ContainsKey("OU")) continue;

                var sysOrg = CreateSysOrg(attrs, sysLdap, orgList, new SysOrg { Id = 0, Level = 0 });
                orgList.Add(sysOrg);

                await SearchDnLdapDept(ldapConn, sysLdap, orgList, ldapEntry.Dn, sysOrg);
            }

            if (orgList.Count == 0)
                return;

            await App.GetRequiredService<SysOrgService>().BatchAddOrgs(orgList);
        }
        catch (LdapException e)
        {
            throw e.ResultCode switch
            {
                LdapException.NoSuchObject or LdapException.NoSuchAttribute => Oops.Oh(ErrorCodeEnum.D0009),
                _ => Oops.Oh(e.Message),
            };
        }
        finally
        {
            ldapConn.Disconnect();
        }
    }

    /// <summary>
    /// Traverse query domain users
    /// </summary>
    /// <param name="ldapConn"></param>
    /// <param name="sysLdap"></param>
    /// <param name="listOrgs"></param>
    /// <param name="baseDn"></param>
    /// <param name="org"></param>
    private static async Task SearchDnLdapDept(LdapConnection ldapConn, SysLdap sysLdap, List<SysOrg> listOrgs, string baseDn, SysOrg org)
    {
        var ldapSearchResults = await ldapConn.SearchAsync(baseDn, LdapConnection.ScopeOne, "(objectClass=*)", null, false);
        while (await ldapSearchResults.HasMoreAsync())
        {
            LdapEntry ldapEntry;
            try
            {
                ldapEntry = await ldapSearchResults.NextAsync();
                if (ldapEntry == null) continue;
            }
            catch (LdapException)
            {
                continue;
            }

            var attrs = ldapEntry.GetAttributeSet();
            if (attrs.Count != 0 && !attrs.ContainsKey("OU")) continue;

            var sysOrg = CreateSysOrg(attrs, sysLdap, listOrgs, org);
            listOrgs.Add(sysOrg);

            await SearchDnLdapDept(ldapConn, sysLdap, listOrgs, ldapEntry.Dn, sysOrg);
        }
    }

    /// <summary>
    /// Create schema objects
    /// </summary>
    /// <param name="attrs"></param>
    /// <param name="sysLdap"></param>
    /// <param name="listOrgs"></param>
    /// <param name="org"></param>
    /// <returns></returns>
    private static SysOrg CreateSysOrg(LdapAttributeSet attrs, SysLdap sysLdap, List<SysOrg> listOrgs, SysOrg org)
    {
        return new SysOrg
        {
            Pid = org.Id,
            Id = YitIdHelper.NextId(),
            Code = attrs.ContainsKey(sysLdap.BindAttrCode) ? new Guid(attrs.GetAttribute(sysLdap.BindAttrCode)?.ByteValue).ToString() : null,
            Level = org.Level + 1,
            Name = attrs.ContainsKey(sysLdap.BindAttrAccount) ? attrs.GetAttribute(sysLdap.BindAttrAccount)?.StringValue : null,
            OrderNo = listOrgs.Count + 1,
        };
    }
}