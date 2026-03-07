// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

public class DictDataInput : BaseStatusInput
{
}

public class PageDictDataInput : BasePageInput
{
    /// <summary>
    /// Dictionary typeId
    /// </summary>
    public long DictTypeId { get; set; }

    /// <summary>
    /// text
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// coding
    /// </summary>
    public string Code { get; set; }
}

public class AddDictDataInput : SysDictData
{
}

public class UpdateDictDataInput : AddDictDataInput
{
}

public class DeleteDictDataInput : BaseIdInput
{
}

public class GetDataDictDataInput
{
    /// <summary>
    /// Dictionary typeId
    /// </summary>
    [Required(ErrorMessage = "Dictionary type Id cannot be empty"), DataValidation(ValidationTypes.Numeric)]
    public long DictTypeId { get; set; }
}

public class QueryDictDataInput
{
    /// <summary>
    /// Dictionary value
    /// </summary>
    [Required(ErrorMessage = "Dictionary value cannot be empty")]
    public string Value { get; set; }

    /// <summary>
    /// state
    /// </summary>
    public int? Status { get; set; }
}