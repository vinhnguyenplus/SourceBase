// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System notification and announcement service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 380, Description = "Notices and Announcements")]
public class SysNoticeService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysUser> _sysUserRep;
    private readonly SqlSugarRepository<SysNotice> _sysNoticeRep;
    private readonly SqlSugarRepository<SysNoticeUser> _sysNoticeUserRep;
    private readonly SysOnlineUserService _sysOnlineUserService;

    public SysNoticeService(
        UserManager userManager,
        SqlSugarRepository<SysUser> sysUserRep,
        SqlSugarRepository<SysNotice> sysNoticeRep,
        SqlSugarRepository<SysNoticeUser> sysNoticeUserRep,
        SysOnlineUserService sysOnlineUserService)
    {
        _userManager = userManager;
        _sysUserRep = sysUserRep;
        _sysNoticeRep = sysNoticeRep;
        _sysNoticeUserRep = sysNoticeUserRep;
        _sysOnlineUserService = sysOnlineUserService;
    }

    /// <summary>
    /// Get the notification and announcement paginated list 📢
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get paginated list of notifications and announcements")]
    public async Task<SqlSugarPagedList<SysNotice>> Page(PageNoticeInput input)
    {
        return await _sysNoticeRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Title), u => u.Title.Contains(input.Title.Trim()))
            .WhereIF(input.Type > 0, u => u.Type == input.Type)
            .WhereIF(!_userManager.SuperAdmin, u => u.CreateUserId == _userManager.UserId)
            .OrderBy(u => u.CreateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// Add notification announcement 📢
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("Add Notice")]
    public async Task AddNotice(AddNoticeInput input)
    {
        var notice = input.Adapt<SysNotice>();
        InitNoticeInfo(notice);
        await _sysNoticeRep.InsertAsync(notice);
    }

    /// <summary>
    /// Update notification announcement 📢
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("Update Notice")]
    public async Task UpdateNotice(UpdateNoticeInput input)
    {
        if (input.CreateUserId != _userManager.UserId)
            throw Oops.Oh(ErrorCodeEnum.D7003);

        var notice = input.Adapt<SysNotice>();
        InitNoticeInfo(notice);
        await _sysNoticeRep.UpdateAsync(notice);
    }

    /// <summary>
    /// Deletion notification announcement 📢
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("Delete notification announcement")]
    public async Task DeleteNotice(DeleteNoticeInput input)
    {
        var sysNotice = await _sysNoticeRep.GetByIdAsync(input.Id);

        if (sysNotice.CreateUserId != _userManager.UserId) throw Oops.Oh(ErrorCodeEnum.D7003);

        if (sysNotice.Status == NoticeStatusEnum.PUBLIC) throw Oops.Oh(ErrorCodeEnum.D7001);

        await _sysNoticeRep.DeleteAsync(u => u.Id == input.Id);

        await _sysNoticeUserRep.DeleteAsync(u => u.NoticeId == input.Id);
    }

    /// <summary>
    /// Release notification announcement 📢
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Issue Notice and Announcement")]
    public async Task Public(NoticeInput input)
    {
        if (!(await _sysNoticeRep.IsAnyAsync(u => u.Id == input.Id && u.CreateUserId == _userManager.UserId)))
            throw Oops.Oh(ErrorCodeEnum.D7003);

        // Update release status and time
        await _sysNoticeRep.UpdateAsync(u => new SysNotice() { Status = NoticeStatusEnum.PUBLIC, PublicTime = DateTime.Now }, u => u.Id == input.Id);

        var notice = await _sysNoticeRep.GetByIdAsync(input.Id);

        // People notified (all accounts)
        var userIdList = await _sysUserRep.AsQueryable().Select(u => u.Id).ToListAsync();

        await _sysNoticeUserRep.DeleteAsync(u => u.NoticeId == notice.Id);
        var noticeUserList = userIdList.Select(u => new SysNoticeUser
        {
            NoticeId = notice.Id,
            UserId = u,
        }).ToList();
        await _sysNoticeUserRep.InsertRangeAsync(noticeUserList);

        // Broadcast to all online accounts
        await _sysOnlineUserService.PublicNotice(notice, userIdList);
    }

    /// <summary>
    /// Set notification announcement read status 📢
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Set notification announcement as read")]
    public async Task SetRead(NoticeInput input)
    {
        await _sysNoticeUserRep.UpdateAsync(u => new SysNoticeUser
        {
            ReadStatus = NoticeUserStatusEnum.READ,
            ReadTime = DateTime.Now
        }, u => u.NoticeId == input.Id && u.UserId == _userManager.UserId);
    }

    /// <summary>
    /// Get received notification announcements
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get received notification announcements")]
    public async Task<SqlSugarPagedList<SysNoticeUser>> PageReceived(PageNoticeInput input)
    {
        return await _sysNoticeUserRep.AsQueryable().Includes(u => u.SysNotice)
            .Where(u => u.UserId == _userManager.UserId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Title), u => u.SysNotice.Title.Contains(input.Title.Trim()))
            .WhereIF(input.Type is > 0, u => u.SysNotice.Type == input.Type)
            .OrderBy(u => u.SysNotice.CreateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// Get unread notification announcements 📢
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get unread notifications and announcements")]
    public async Task<List<SysNotice>> GetUnReadList()
    {
        var noticeUserList = await _sysNoticeUserRep.AsQueryable().Includes(u => u.SysNotice)
            .Where(u => u.UserId == _userManager.UserId && u.ReadStatus == NoticeUserStatusEnum.UNREAD)
            .OrderBy(u => u.SysNotice.CreateTime, OrderByType.Desc).ToListAsync();
        return noticeUserList.Select(t => t.SysNotice).ToList();
    }

    /// <summary>
    /// Initialization notification announcement information
    /// </summary>
    /// <param name="notice"></param>
    [NonAction]
    private void InitNoticeInfo(SysNotice notice)
    {
        notice.PublicUserId = _userManager.UserId;
        notice.PublicUserName = _userManager.RealName;
        notice.PublicOrgId = _userManager.OrgId;
    }
}