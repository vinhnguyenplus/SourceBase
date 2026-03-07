// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// WeChat development return code
/// </summary>
[Description("WeChat development return code")]
public enum WechatReturnCodeEnum
{
    SenparcWeixinSDKConfigurationmistake = -99, // 0xFFFFFF9D
    System is busythistimePlease try again later, developers = -1, // 0xFFFFFFFF
    Requestsuccess = 0,
    Industry and CommerceDataReturn_The company has been deregistered = 101, // 0x00000065
    Industry and CommerceDataReturn_The company does not exist or company informationNot yetUpdate = 102, // 0x00000066
    Industry and CommerceDataReturn_Legal representative of the enterpriseNameNooneTo = 103, // 0x00000067
    Industry and CommerceDataReturn_Legal representative of the enterpriseID numberCode or notoneTo = 104, // 0x00000068
    Legal RepresentativeID numbercode_Industry and CommerceDataNot yetUpdate_Please5_15a jobdayTry later = 105, // 0x00000069
    Industry and CommerceDataReturn_Enterprise information or legal representative information is notoneTo = 1000, // 0x000003E8
    The other party does notYesfans = 10700, // 0x000029CC
    SendinformationFailure_the other partyCloseto receiveinformation = 10703, // 0x000029CF
    SendinformationFailure_48smalltimeinsideUserNot yetInteraction = 10706, // 0x000029D2
    POSTParameterIllegal = 20002, // 0x00004E22
    Obtainaccess_tokentimeAppSecretmistakeoraccess_tokenNoneeffect = 40001, // 0x00009C41

    /// <summary>
    /// <para>Official Account: Illegal voucher type</para>
    /// <para>Mini Program: No permission to generate yet</para>
    /// </summary>
    Invalid credentialsType = 40002, // 0x00009C42

    IllegalOpenID = 40003, // 0x00009C43
    Illegal mediaFile type = 40004, // 0x00009C44
    IllegalFile type = 40005, // 0x00009C45
    Illegal file size = 40006, // 0x00009C46
    Illegal media fileid = 40007, // 0x00009C47
    IllegalinformationType_40008 = 40008, // 0x00009C48
    Invalid image file size = 40009, // 0x00009C49
    Invalid voice file size = 40010, // 0x00009C4A
    Illegal video file size = 40011, // 0x00009C4B
    Invalid thumbnail file size = 40012, // 0x00009C4C

    /// <summary>
    /// <para>WeChat: Illegal APPID</para>
    /// <para>Mini program: generation permission is blocked</para>
    /// </summary>
    IllegalAPPID = 40013, // 0x00009C4D

    Illegalaccess_token = 40014, // 0x00009C4E
    IllegalmenuType = 40015, // 0x00009C4F
    IllegalbuttonQuantity1 = 40016, // 0x00009C50
    IllegalbuttonQuantity2 = 40017, // 0x00009C51
    IllegalbuttonNamelength = 40018, // 0x00009C52
    IllegalbuttonKEYlength = 40019, // 0x00009C53
    IllegalbuttonURLlength = 40020, // 0x00009C54
    Illegalmenuversion number = 40021, // 0x00009C55
    IllegalchildmenuSeries = 40022, // 0x00009C56
    IllegalchildmenubuttonQuantity = 40023, // 0x00009C57
    IllegalchildmenubuttonType = 40024, // 0x00009C58
    IllegalchildmenubuttonNamelength = 40025, // 0x00009C59
    IllegalchildmenubuttonKEYlength = 40026, // 0x00009C5A
    IllegalchildmenubuttonURLlength = 40027, // 0x00009C5B
    IllegalCustomizemenuUseUser = 40028, // 0x00009C5C
    Illegaloauth_code = 40029, // 0x00009C5D
    Illegalrefresh_token = 40030, // 0x00009C5E
    IllegalopenidList = 40031, // 0x00009C5F
    IllegalopenidListlength = 40032, // 0x00009C60
    Illegal request characters are not allowedincludeuxxxxFormatting character = 40033, // 0x00009C61
    IllegalParameter = 40035, // 0x00009C63
    template_idNojustSure = 40037, // 0x00009C65
    Illegal request format = 40038, // 0x00009C66
    IllegalURLlength = 40039, // 0x00009C67
    IllegalGroupid = 40050, // 0x00009C72
    GroupInvalid name = 40051, // 0x00009C73

    /// <summary>
    /// <para>Public account: Incorrect input parameters</para>
    /// <para>Mini program: The parameter expire_time is incorrectly filled in</para>
    /// </summary>
    InputParameterIncorrect = 40097, // 0x00009CA1

    appsecretNojustSure = 40125, // 0x00009CBD
    Calling the interfaceIP addressNot on the whitelistin = 40164, // 0x00009CE4
    ParameterpathFill inmistake = 40165, // 0x00009CE5
    Mini ProgramAppidDoes not exist = 40166, // 0x00009CE6
    ParameterqueryFill inmistake = 40212, // 0x00009D14
    Lackingaccess_tokenParameter = 41001, // 0x0000A029
    LackingappidParameter = 41002, // 0x0000A02A
    Lackingrefresh_tokenParameter = 41003, // 0x0000A02B
    LackingsecretParameter = 41004, // 0x0000A02C
    Missing multimedia filesData = 41005, // 0x0000A02D
    Lackingmedia_idParameter = 41006, // 0x0000A02E
    LackingchildmenuData = 41007, // 0x0000A02F
    Lackingoauth_code = 41008, // 0x0000A030
    Lackingopenid = 41009, // 0x0000A031
    form_idNojustSure_orExpired = 41028, // 0x0000A044
    form_idAlready used = 41029, // 0x0000A045
    pageNojustSure = 41030, // 0x0000A046
    access_tokenSupertime = 42001, // 0x0000A411
    refresh_tokenSupertime = 42002, // 0x0000A412
    oauth_codeSupertime = 42003, // 0x0000A413
    NeedGETRequest = 43001, // 0x0000A7F9
    NeedPOSTRequest = 43002, // 0x0000A7FA
    NeedHTTPSRequest = 43003, // 0x0000A7FB
    Requires the recipient's attention = 43004, // 0x0000A7FC
    Friend relationship required = 43005, // 0x0000A7FD

    /// <summary>[Mini Program Subscription Message] The user refuses to accept the message. If the user has subscribed before, it means that the user has canceled the subscription relationship.</summary>
    UserRefuse to acceptinformation = 43101, // 0x0000A85D

    No permission = 43104, // 0x0000A860
    multimedia file isnull = 44001, // 0x0000ABE1
    POSTofDataBao Weinull = 44002, // 0x0000ABE2
    Graphics and textinformationcontentfornull = 44003, // 0x0000ABE3
    textinformationcontentfornull = 44004, // 0x0000ABE4
    The multimedia file size exceeds the limit = 45001, // 0x0000AFC9
    informationcontentExceeds the limit = 45002, // 0x0000AFCA
    titleFieldExceeds the limit = 45003, // 0x0000AFCB
    DescriptionFieldExceeds the limit = 45004, // 0x0000AFCC
    LinkFieldExceeds the limit = 45005, // 0x0000AFCD
    Image linkFieldExceeds the limit = 45006, // 0x0000AFCE
    Voice playbacktimeExceeds the limit = 45007, // 0x0000AFCF
    Graphics and textinformationExceeds the limit = 45008, // 0x0000AFD0
    Interface call exceeds the limit = 45009, // 0x0000AFD1
    CreatemenuThe number exceeds the limit = 45010, // 0x0000AFD2
    ReplytimeExceeds the limit = 45015, // 0x0000AFD7
    systemGroupModification not allowed = 45016, // 0x0000AFD8
    GroupName too long = 45017, // 0x0000AFD9
    GroupquantityExceeds the limit = 45018, // 0x0000AFDA
    Exceed responsequantityLimit = 45047, // 0x0000AFF7
    The number of tags created is too many, please note that it cannot exceed100piece = 45056, // 0x0000B000
    markSignatureIllegal, please note cannot be withOtherDuplicate label = 45157, // 0x0000B065
    markSignaturelengthexceed30byte = 45158, // 0x0000B066
    No mediaData = 46001, // 0x0000B3B1
    Does not existmenuVersion = 46002, // 0x0000B3B2
    Does not existmenuData = 46003, // 0x0000B3B3
    AnalysisJSON_XMLcontentmistake = 47001, // 0x0000B799

    /// <summary>[Mini program subscription message] The template parameters are inaccurate and may be empty or do not meet the rules. errmsg will prompt which field is wrong.</summary>
    TemplateParameterInaccurate = 47003, // 0x0000B79B

    apiFunctionUnauthorized = 48001, // 0x0000BB81
    UserUnauthorizedshouldapi = 50001, // 0x0000C351
    nameInvalid format = 53010, // 0x0000CF12
    nameDestiny testinginFrequency limit = 53011, // 0x0000CF13
    Prohibited from usename = 53012, // 0x0000CF14
    Official Account_nameWith an existing public accountnameRepeat_Mini Program_shouldnameWith existing mini programsnameRepeat = 53013, // 0x0000CF15
    Official Account_Public account already exists_nameA_time_Must be the same entity as this accountApplyPlease_nameA_Mini Program_The mini program already exists_nameA_time_Must be the same entity as this accountApplyPlease_nameA_ = 53014, // 0x0000CF16
    Official Account_shouldnameWith existing mini programsnameRepeat_Must have the same entity as this mini program accountApplyPlease_Mini Program_shouldnameWith an existing public accountnameRepeat_Must be the same entity as this public accountApplyPlease = 53015, // 0x0000CF17
    Official Account_shouldnameHas multiple existing mini programsnameRepeat_Not supported for nowApplyPlease_Mini Program_shouldnameWith multiple existing public accountsnameRepeat_Not supported for nowApplyPlease = 53016, // 0x0000CF18
    Official Account_The mini program already exists_nameA_time_Must be the same entity as this accountApplyPlease_nameA_Mini Program_Public account already exists_nameA_time_Must be the same entity as this accountApplyPlease_nameA = 53017, // 0x0000CF19
    nameLifeinWeChatnumber = 53018, // 0x0000CF1A
    nameDuring the protection period = 53019, // 0x0000CF1B
    Legal entityNameandWeChatNumber or notoneTo = 61070, // 0x0000EE8E
    systemmistakesystem_error = 61450, // 0x0000F00A
    Parametermistakeinvalid_parameter = 61451, // 0x0000F00B
    NoneEfficiency Customer ServiceAccount numberinvalid_kf_account = 61452, // 0x0000F00C
    Customer service account already existskf_account_exsited = 61453, // 0x0000F00D

    /// <summary>
    /// The length of the customer service account name exceeds the limit (only 10 English characters are allowed, excluding @ and the WeChat account of the official account after @) (invalid kf_acount length)
    /// </summary>
    Customer Service Account NamelengthExceeds the limit = 61454, // 0x0000F00E

    /// <summary>
    /// The customer service account name contains illegal characters (only English + numbers are allowed) (illegal character in kf_account)
    /// </summary>
    Customer Service Account NameincludeIllegal character = 61455, // 0x0000F00F

    /// <summary>The number of customer service accounts exceeds the limit (10 customer service accounts) (kf_account count exceeded)</summary>
    The number of customer service accounts exceeds the limit = 61456, // 0x0000F010

    NoneeffectAvatarFile typeinvalid_file_type = 61457, // 0x0000F011
    date formatmistake = 61500, // 0x0000F03C
    dayperiodrangemistake = 61501, // 0x0000F03D
    SendinformationFailure_shouldUserHas been added to the blacklist_NoneSend normally to thisinformation = 62751, // 0x0000F51F
    The store does not exist = 65115, // 0x0000FE5B
    This storestateNot allowedUpdate = 65118, // 0x0000FE5E
    Label formatmistake = 85006, // 0x00014C0E
    Page Pathmistake = 85007, // 0x00014C0F
    Category Entrymistake = 85008, // 0x00014C10
    Already havejustUnder reviewnuclearversion = 85009, // 0x00014C11
    item_listThere is a project fornull = 85010, // 0x00014C12
    titleFill inmistake = 85011, // 0x00014C13
    NoneEffective reviewnuclearid = 85012, // 0x00014C14
    Version Inputmistake = 85015, // 0x00014C17
    Not reviewednuclearVersion = 85019, // 0x00014C1B
    ExaminenuclearstateNot yetsatisfyrelease = 85020, // 0x00014C1C
    stateImmutable = 85021, // 0x00014C1D
    actionIllegal = 85022, // 0x00014C1E
    ExaminenuclearThe number of items filled in the list is not within1to5within = 85023, // 0x00014C1F
    Additional relevant materials are needed_Fill inorg_codeandother_filesParameter = 85024, // 0x00014C20
    Administrator Mobile RegistrationquantityExceeded the limit = 85025, // 0x00014C21
    shouldWeChatThe account has been linked5an administrator = 85026, // 0x00014C22
    AdministratorID cardAlready registered5time = 85027, // 0x00014C23
    The subject registrationquantityExceeded the limit = 85028, // 0x00014C24
    MerchantnameAlready occupied = 85029, // 0x00014C25
    Cannot use thisname = 85031, // 0x00014C27
    shouldnameDuring the infringement complaint protection period = 85032, // 0x00014C28
    nameincludeViolationcontentorWeChatetc. reserved words = 85033, // 0x00014C29
    MerchantnameChanging the name15skyDuring the protection period = 85034, // 0x00014C2A
    Must be the same entity as this accountApplyPlease = 85035, // 0x00014C2B
    IntroductioninContains false confusioncontent = 85036, // 0x00014C2C
    AvatarorProfile edits reach the monthly limit = 85049, // 0x00014C39
    justUnder reviewnuclearin_Please do not submit repeatedly = 85050, // 0x00014C3A
    Please firstsuccessCall after creating the store = 85053, // 0x00014C3D
    to face; to overlook; to arrive; about totimemediaidNoneeffect = 85056, // 0x00014C40
    Linkmistake = 85066, // 0x00014C4A
    Test link or notYeschildLink = 85068, // 0x00014C4C
    Verification fileFailure = 85069, // 0x00014C4D
    IndividualTypeMini ProgramNoneLegal settingsQR codeRules = 85070, // 0x00014C4E
    alreadyAdd toThis link_Please do not repeatAdd to = 85071, // 0x00014C4F
    This link has already been taken = 85072, // 0x00014C50
    QR codeRules are full = 85073, // 0x00014C51
    Mini ProgramUnpublished_The mini program must start firstreleaseOnly code canreleaseQR codeJump Rules = 85074, // 0x00014C52
    IndividualTypeMini ProgramNoneLegal settingsQR codeRules1 = 85075, // 0x00014C53
    The mini program does not have an online version_Cannot perform grayscale = 85079, // 0x00014C57
    Review submitted by the mini programnuclearNot yetExaminenuclearthrough = 85080, // 0x00014C58
    NoneeffectivereleaseProportion = 85081, // 0x00014C59
    currentreleaseThe ratio needs to be higher than previously set = 85082, // 0x00014C5A
    Mini Program Submission for ReviewquantityThe monthly limit has been reached = 85085, // 0x00014C5D
    Submit code reviewnuclearNeeds to be done in advanceuploadCode = 85086, // 0x00014C5E
    Mini ProgramAlready used_api_navigateToMiniProgram_Please declare the redirect_appid_Resubmit after the list = 85087, // 0x00014C5F
    NoYesbyNumberThreeInvoke through the Fangdai mini program = 86000, // 0x00014FF0
    Does not existNumberThreeThe code that Fang has already submitted = 86001, // 0x00014FF1
    Mini program stillNot yetSettingsNickname_Avatar_Introduction_Please complete the settings first and then resubmit. = 86002, // 0x00014FF2
    NoneeffectWeChatnumber = 86004, // 0x00014FF4

    /// <summary>
    /// The applet is "Signature Error". Corresponding public account: 87009, “errmsg”: “reply is not exists” //This reply does not exist
    /// </summary>
    Signaturemistake = 87009, // 0x000153E1

    The live network is already in grayscale.release_Cannot perform version rollback = 87011, // 0x000153E3
    This version cannot be rolled back_Possible reasons_1_NoneUponeAn online version used for rollback_2_This version is a rolled-back version_Cannot revert_3_This version is the version before the rollback feature was launched._Cannot revert = 87012, // 0x000153E4
    contentContains illegal and irregular activitiescontent = 87014, // 0x000153E6
    No permission to leave a message = 88000, // 0x000157C0
    This image and text does not exist = 88001, // 0x000157C1
    The article contains sensitive information = 88002, // 0x000157C2
    The number of featured reviews has reached the limit = 88003, // 0x000157C3
    has beenUserDelete_NoneSelected Laws = 88004, // 0x000157C4
    Already replied = 88005, // 0x000157C5
    Reply exceedslengthRestrict or may be0 = 88007, // 0x000157C7
    This comment does not exist = 88008, // 0x000157C8
    ObtainThe number of comments is invalid = 88010, // 0x000157CA
    this public account_The mini program has already been linked to an Open Platform account. = 89000, // 0x00015BA8
    Businessdomain nameNoneChange_NoneNeeds to be set again = 89019, // 0x00015BBB
    stillNot yetSet up mini program businessdomain name_Please first atNumberThreesquare platforminSet up mini program businessdomain nameThen call this interface = 89020, // 0x00015BBC
    Requestsaveofdomain nameNoYesNumberThreesquare platforminAlready set mini program businessdomain nameorchilddomain name = 89021, // 0x00015BBD
    Businessdomain namequantityExceeds the limit_At mostAdd to100a businessdomain name = 89029, // 0x00015BC5
    Personal mini programs do not support invocation_setwebviewdomain_Interface = 89231, // 0x00015C8F
    Internalmistake = 89247, // 0x00015C9F
    Enterprise CodeTypeNoneeffect_Please selectjustSureTypeFill in = 89248, // 0x00015CA0
    This entity already existsTaskExecutein_Since the last timeTask24hTry again later = 89249, // 0x00015CA1
    Not yetFound itTask = 89250, // 0x00015CA2
    Pending corporate face recognitionnuclearBody verification = 89251, // 0x00015CA3
    Legal entity_Company InformationoneChecksumin = 89252, // 0x00015CA4
    LackingParameter = 89253, // 0x00015CA5
    NumberThreeInsufficient permissions_Complete the permission set across the entire networkreleaseEffective later = 89254, // 0x00015CA6
    The system is unstable_Please try again later_As many timesFailurePlease provide feedback through the community = 89401, // 0x00015D39
    To be reviewednuclearThe order is not under reviewnuclearQueue_Please checkYesnoSubmitted for reviewnuclearMay have been reviewed = 89402, // 0x00015D3A
    This order belongs to a category that the platform does not support for expedited service._Please waitjustRegular ReviewnuclearProcess = 89403, // 0x00015D3B
    This order has been expeditedsuccess_Please do not submit repeatedly = 89404, // 0x00015D3C
    The urgent quota for this month is insufficient_Please improve the quality of the review requestObtainMore credit = 89405, // 0x00015D3D
    This business qualification has alreadyAdd to_Please do not repeatAdd to = 92000, // 0x00016760
    Nearby placesAdd toquantityReach the limit_NoneThe law continuesAdd to = 92002, // 0x00016762
    The location has beenOtherMini program usage = 92003, // 0x00016763
    Nearby features are disabled = 92004, // 0x00016764
    LocationjustUnder reviewnuclearin = 92005, // 0x00016765
    LocationjustShowing the mini program = 92006, // 0x00016766
    Site ReviewnuclearFailure = 92007, // 0x00016767
    ProgramNot yetDisplay at this location = 92008, // 0x00016768
    Mini ProgramNot yetListed or not visible = 92009, // 0x00016769
    Location does not exist = 93010, // 0x00016B52
    IndividualTypeThe mini program is not available = 93011, // 0x00016B53
    Already issuedTemplateinformationLegal person andNot yetConfirmand has exceededtime_24h_Not yetcarry outID cardVerification = 100001, // 0x000186A1
    Already issuedTemplateinformationLegal person andNot yetConfirmand has exceededtime_24h_Not yetPerform facial recognition verification = 100002, // 0x000186A2
    Already issuedTemplateinformationLegal person andNot yetConfirmand has exceededtime_24h = 100003, // 0x000186A3
    thisAccount numberHas been banned_NoneLawOperation = 200011, // 0x00030D4B
    PrivateTemplateThe number has reached the limit_Upper limit_50_piece = 200012, // 0x00030D4C
    This template has been banned_NoneMethod selection = 200013, // 0x00030D4D
    TemplatetidParametermistake = 200014, // 0x00030D4E
    List of keywordskidListParametermistake = 200020, // 0x00030D54
    SceneDescriptionsceneDescParametermistake = 200021, // 0x00030D55
}