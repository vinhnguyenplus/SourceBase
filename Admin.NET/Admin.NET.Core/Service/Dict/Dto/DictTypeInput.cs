// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

public class DictTypeInput : BaseStatusInput
{
}

public class PageDictTypeInput : BasePageInput
{
    /// <summary>
    /// name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// coding
    /// </summary>
    public string Code { get; set; }
}

public class AddDictTypeInput : SysDictType
{
    /// <summary>
    /// Whether it is a tenant dictionary (Y-yes, N-no)
    /// </summary>
    public override YesNoEnum IsTenant { get; set; } = YesNoEnum.Y;

    /// <summary>
    /// Whether it is a built-in dictionary (Y-yes, N-no)
    /// </summary>
    public override YesNoEnum SysFlag { get; set; } = YesNoEnum.N;
}

public class UpdateDictTypeInput : AddDictTypeInput
{
}

public class DeleteDictTypeInput : BaseIdInput
{
}

public class GetDataDictTypeInput
{
    /// <summary>
    /// coding
    /// </summary>
    [Required(ErrorMessage = "Dictionary type code cannot be empty")]
    public string Code { get; set; }
}