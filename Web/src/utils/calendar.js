/* eslint-disable */

var calendar = (function () {
  'use strict';

  var lunarInfo = [0x04bd8, 0x04ae0, 0x0a570, 0x054d5, 0x0d260, 0x0d950, 0x16554, 0x056a0, 0x09ad0, 0x055d2,
  //1900-1909
  0x04ae0, 0x0a5b6, 0x0a4d0, 0x0d250, 0x1d255, 0x0b540, 0x0d6a0, 0x0ada2, 0x095b0, 0x14977,
  //1910-1919
  0x04970, 0x0a4b0, 0x0b4b5, 0x06a50, 0x06d40, 0x1ab54, 0x02b60, 0x09570, 0x052f2, 0x04970,
  //1920-1929
  0x06566, 0x0d4a0, 0x0ea50, 0x16a95, 0x05ad0, 0x02b60, 0x186e3, 0x092e0, 0x1c8d7, 0x0c950,
  //1930-1939
  0x0d4a0, 0x1d8a6, 0x0b550, 0x056a0, 0x1a5b4, 0x025d0, 0x092d0, 0x0d2b2, 0x0a950, 0x0b557,
  //1940-1949
  0x06ca0, 0x0b550, 0x15355, 0x04da0, 0x0a5b0, 0x14573, 0x052b0, 0x0a9a8, 0x0e950, 0x06aa0,
  //1950-1959
  0x0aea6, 0x0ab50, 0x04b60, 0x0aae4, 0x0a570, 0x05260, 0x0f263, 0x0d950, 0x05b57, 0x056a0,
  //1960-1969
  0x096d0, 0x04dd5, 0x04ad0, 0x0a4d0, 0x0d4d4, 0x0d250, 0x0d558, 0x0b540, 0x0b6a0, 0x195a6,
  //1970-1979
  0x095b0, 0x049b0, 0x0a974, 0x0a4b0, 0x0b27a, 0x06a50, 0x06d40, 0x0af46, 0x0ab60, 0x09570,
  //1980-1989
  0x04af5, 0x04970, 0x064b0, 0x074a3, 0x0ea50, 0x06b58, 0x05ac0, 0x0ab60, 0x096d5, 0x092e0,
  //1990-1999
  0x0c960, 0x0d954, 0x0d4a0, 0x0da50, 0x07552, 0x056a0, 0x0abb7, 0x025d0, 0x092d0, 0x0cab5,
  //2000-2009
  0x0a950, 0x0b4a0, 0x0baa4, 0x0ad50, 0x055d9, 0x04ba0, 0x0a5b0, 0x15176, 0x052b0, 0x0a930,
  //2010-2019
  0x07954, 0x06aa0, 0x0ad50, 0x05b52, 0x04b60, 0x0a6e6, 0x0a4e0, 0x0d260, 0x0ea65, 0x0d530,
  //2020-2029
  0x05aa0, 0x076a3, 0x096d0, 0x04afb, 0x04ad0, 0x0a4d0, 0x1d0b6, 0x0d250, 0x0d520, 0x0dd45,
  //2030-2039
  0x0b5a0, 0x056d0, 0x055b2, 0x049b0, 0x0a577, 0x0a4b0, 0x0aa50, 0x1b255, 0x06d20, 0x0ada0,
  //2040-2049
  /**Add By JJonline@JJonline.Cn**/
  0x14b63, 0x09370, 0x049f8, 0x04970, 0x064b0, 0x168a6, 0x0ea50, 0x06b20, 0x1a6c4, 0x0aae0,
  //2050-2059
  0x092e0, 0x0d2e3, 0x0c960, 0x0d557, 0x0d4a0, 0x0da50, 0x05d55, 0x056a0, 0x0a6d0, 0x055d4,
  //2060-2069
  0x052d0, 0x0a9b8, 0x0a950, 0x0b4a0, 0x0b6a6, 0x0ad50, 0x055a0, 0x0aba4, 0x0a5b0, 0x052b0,
  //2070-2079
  0x0b273, 0x06930, 0x07337, 0x06aa0, 0x0ad50, 0x14b55, 0x04b60, 0x0a570, 0x054e4, 0x0d160,
  //2080-2089
  0x0e968, 0x0d520, 0x0daa0, 0x16aa6, 0x056d0, 0x04ae0, 0x0a9d4, 0x0a2d0, 0x0d150, 0x0f252,
  //2090-2099
  0x0d520]; //2100

  var solarMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

  var Gan = ["\u7532", "\u4E59", "\u4E19", "\u4E01", "\u620A", "\u5DF1", "\u5E9A", "\u8F9B", "\u58EC", "\u7678"];
  var Zhi = ["\u5B50", "\u4E11", "\u5BC5", "\u536F", "\u8FB0", "\u5DF3", "\u5348", "\u672A", "\u7533", "\u9149", "\u620C", "\u4EA5"];

  var ChineseZodiac = ["\u9F20", "\u725B", "\u864E", "\u5154", "\u9F99", "\u86C7", "\u9A6C", "\u7F8A", "\u7334", "\u9E21", "\u72D7", "\u732A"];

  var festival = {
    '1-1': {
      title: "New Year's Day"
    },
    '2-14': {
      title: 'Valentines Day'
    },
    '5-1': {
      title: 'labor day'
    },
    '5-4': {
      title: 'Youth Day'
    },
    '6-1': {
      title: 'childrens day'
    },
    '9-10': {
      title: 'teachers day'
    },
    '10-1': {
      title: 'National Day'
    },
    '12-25': {
      title: 'Christmas'
    },
    '3-8': {
      title: 'womens day'
    },
    '3-12': {
      title: 'Arbor Day'
    },
    '4-1': {
      title: 'April Fools Day'
    },
    '5-12': {
      title: 'Nurses Day'
    },
    '7-1': {
      title: 'Party Founding Day'
    },
    '8-1': {
      title: 'Army Day'
    },
    '12-24': {
      title: 'Christmas Eve'
    }
  };
  var lFestival = {
    '12-30': {
      title: 'New Years Eve'
    },
    '1-1': {
      title: 'Spring Festival'
    },
    '1-15': {
      title: 'YuanNight Festival'
    },
    '2-2': {
      title: 'Dragon Raising Its Head'
    },
    '5-5': {
      title: 'Dragon Boat Festival'
    },
    '7-7': {
      title: 'Qixi Festival'
    },
    '7-15': {
      title: 'Hungry Ghost Festival'
    },
    '8-15': {
      title: 'Mid-Autumn Festival'
    },
    '9-9': {
      title: 'Double Ninth Festival'
    },
    '10-1': {
      title: 'Winter Clothes Festival'
    },
    '10-15': {
      title: 'Lower Prime Festival'
    },
    '12-8': {
      title: 'Laba Festival'
    },
    '12-23': {
      title: 'Northern Little New Year'
    },
    '12-24': {
      title: 'Southern Little New Year'
    }
  };

  var solarTerm = ["\u5C0F\u5BD2", "\u5927\u5BD2", "\u7ACB\u6625", "\u96E8\u6C34", "\u60CA\u86F0", "\u6625\u5206", "\u6E05\u660E", "\u8C37\u96E8", "\u7ACB\u590F", "\u5C0F\u6EE1", "\u8292\u79CD", "\u590F\u81F3", "\u5C0F\u6691", "\u5927\u6691", "\u7ACB\u79CB", "\u5904\u6691", "\u767D\u9732", "\u79CB\u5206", "\u5BD2\u9732", "\u971C\u964D", "\u7ACB\u51AC", "\u5C0F\u96EA", "\u5927\u96EA", "\u51AC\u81F3"];
  var sTermInfo = ['9778397bd097c36b0b6fc9274c91aa', '97b6b97bd19801ec9210c965cc920e', '97bcf97c3598082c95f8c965cc920f', '97bd0b06bdb0722c965ce1cfcc920f', 'b027097bd097c36b0b6fc9274c91aa', '97b6b97bd19801ec9210c965cc920e', '97bcf97c359801ec95f8c965cc920f', '97bd0b06bdb0722c965ce1cfcc920f', 'b027097bd097c36b0b6fc9274c91aa', '97b6b97bd19801ec9210c965cc920e', '97bcf97c359801ec95f8c965cc920f', '97bd0b06bdb0722c965ce1cfcc920f', 'b027097bd097c36b0b6fc9274c91aa', '9778397bd19801ec9210c965cc920e', '97b6b97bd19801ec95f8c965cc920f', '97bd09801d98082c95f8e1cfcc920f', '97bd097bd097c36b0b6fc9210c8dc2', '9778397bd197c36c9210c9274c91aa', '97b6b97bd19801ec95f8c965cc920e', '97bd09801d98082c95f8e1cfcc920f', '97bd097bd097c36b0b6fc9210c8dc2', '9778397bd097c36c9210c9274c91aa', '97b6b97bd19801ec95f8c965cc920e', '97bcf97c3598082c95f8e1cfcc920f', '97bd097bd097c36b0b6fc9210c8dc2', '9778397bd097c36c9210c9274c91aa', '97b6b97bd19801ec9210c965cc920e', '97bcf97c3598082c95f8c965cc920f', '97bd097bd097c35b0b6fc920fb0722', '9778397bd097c36b0b6fc9274c91aa', '97b6b97bd19801ec9210c965cc920e', '97bcf97c3598082c95f8c965cc920f', '97bd097bd097c35b0b6fc920fb0722', '9778397bd097c36b0b6fc9274c91aa', '97b6b97bd19801ec9210c965cc920e', '97bcf97c359801ec95f8c965cc920f', '97bd097bd097c35b0b6fc920fb0722', '9778397bd097c36b0b6fc9274c91aa', '97b6b97bd19801ec9210c965cc920e', '97bcf97c359801ec95f8c965cc920f', '97bd097bd097c35b0b6fc920fb0722', '9778397bd097c36b0b6fc9274c91aa', '97b6b97bd19801ec9210c965cc920e', '97bcf97c359801ec95f8c965cc920f', '97bd097bd07f595b0b6fc920fb0722', '9778397bd097c36b0b6fc9210c8dc2', '9778397bd19801ec9210c9274c920e', '97b6b97bd19801ec95f8c965cc920f', '97bd07f5307f595b0b0bc920fb0722', '7f0e397bd097c36b0b6fc9210c8dc2', '9778397bd097c36c9210c9274c920e', '97b6b97bd19801ec95f8c965cc920f', '97bd07f5307f595b0b0bc920fb0722', '7f0e397bd097c36b0b6fc9210c8dc2', '9778397bd097c36c9210c9274c91aa', '97b6b97bd19801ec9210c965cc920e', '97bd07f1487f595b0b0bc920fb0722', '7f0e397bd097c36b0b6fc9210c8dc2', '9778397bd097c36b0b6fc9274c91aa', '97b6b97bd19801ec9210c965cc920e', '97bcf7f1487f595b0b0bb0b6fb0722', '7f0e397bd097c35b0b6fc920fb0722', '9778397bd097c36b0b6fc9274c91aa', '97b6b97bd19801ec9210c965cc920e', '97bcf7f1487f595b0b0bb0b6fb0722', '7f0e397bd097c35b0b6fc920fb0722', '9778397bd097c36b0b6fc9274c91aa', '97b6b97bd19801ec9210c965cc920e', '97bcf7f1487f531b0b0bb0b6fb0722', '7f0e397bd097c35b0b6fc920fb0722', '9778397bd097c36b0b6fc9274c91aa', '97b6b97bd19801ec9210c965cc920e', '97bcf7f1487f531b0b0bb0b6fb0722', '7f0e397bd07f595b0b6fc920fb0722', '9778397bd097c36b0b6fc9274c91aa', '97b6b97bd19801ec9210c9274c920e', '97bcf7f0e47f531b0b0bb0b6fb0722', '7f0e397bd07f595b0b0bc920fb0722', '9778397bd097c36b0b6fc9210c91aa', '97b6b97bd197c36c9210c9274c920e', '97bcf7f0e47f531b0b0bb0b6fb0722', '7f0e397bd07f595b0b0bc920fb0722', '9778397bd097c36b0b6fc9210c8dc2', '9778397bd097c36c9210c9274c920e', '97b6b7f0e47f531b0723b0b6fb0722', '7f0e37f5307f595b0b0bc920fb0722', '7f0e397bd097c36b0b6fc9210c8dc2', '9778397bd097c36b0b70c9274c91aa', '97b6b7f0e47f531b0723b0b6fb0721', '7f0e37f1487f595b0b0bb0b6fb0722', '7f0e397bd097c35b0b6fc9210c8dc2', '9778397bd097c36b0b6fc9274c91aa', '97b6b7f0e47f531b0723b0b6fb0721', '7f0e27f1487f595b0b0bb0b6fb0722', '7f0e397bd097c35b0b6fc920fb0722', '9778397bd097c36b0b6fc9274c91aa', '97b6b7f0e47f531b0723b0b6fb0721', '7f0e27f1487f531b0b0bb0b6fb0722', '7f0e397bd097c35b0b6fc920fb0722', '9778397bd097c36b0b6fc9274c91aa', '97b6b7f0e47f531b0723b0b6fb0721', '7f0e27f1487f531b0b0bb0b6fb0722', '7f0e397bd097c35b0b6fc920fb0722', '9778397bd097c36b0b6fc9274c91aa', '97b6b7f0e47f531b0723b0b6fb0721', '7f0e27f1487f531b0b0bb0b6fb0722', '7f0e397bd07f595b0b0bc920fb0722', '9778397bd097c36b0b6fc9274c91aa', '97b6b7f0e47f531b0723b0787b0721', '7f0e27f0e47f531b0b0bb0b6fb0722', '7f0e397bd07f595b0b0bc920fb0722', '9778397bd097c36b0b6fc9210c91aa', '97b6b7f0e47f149b0723b0787b0721', '7f0e27f0e47f531b0723b0b6fb0722', '7f0e397bd07f595b0b0bc920fb0722', '9778397bd097c36b0b6fc9210c8dc2', '977837f0e37f149b0723b0787b0721', '7f07e7f0e47f531b0723b0b6fb0722', '7f0e37f5307f595b0b0bc920fb0722', '7f0e397bd097c35b0b6fc9210c8dc2', '977837f0e37f14998082b0787b0721', '7f07e7f0e47f531b0723b0b6fb0721', '7f0e37f1487f595b0b0bb0b6fb0722', '7f0e397bd097c35b0b6fc9210c8dc2', '977837f0e37f14998082b0787b06bd', '7f07e7f0e47f531b0723b0b6fb0721', '7f0e27f1487f531b0b0bb0b6fb0722', '7f0e397bd097c35b0b6fc920fb0722', '977837f0e37f14998082b0787b06bd', '7f07e7f0e47f531b0723b0b6fb0721', '7f0e27f1487f531b0b0bb0b6fb0722', '7f0e397bd097c35b0b6fc920fb0722', '977837f0e37f14998082b0787b06bd', '7f07e7f0e47f531b0723b0b6fb0721', '7f0e27f1487f531b0b0bb0b6fb0722', '7f0e397bd07f595b0b0bc920fb0722', '977837f0e37f14998082b0787b06bd', '7f07e7f0e47f531b0723b0b6fb0721', '7f0e27f1487f531b0b0bb0b6fb0722', '7f0e397bd07f595b0b0bc920fb0722', '977837f0e37f14998082b0787b06bd', '7f07e7f0e47f149b0723b0787b0721', '7f0e27f0e47f531b0b0bb0b6fb0722', '7f0e397bd07f595b0b0bc920fb0722', '977837f0e37f14998082b0723b06bd', '7f07e7f0e37f149b0723b0787b0721', '7f0e27f0e47f531b0723b0b6fb0722', '7f0e397bd07f595b0b0bc920fb0722', '977837f0e37f14898082b0723b02d5', '7ec967f0e37f14998082b0787b0721', '7f07e7f0e47f531b0723b0b6fb0722', '7f0e37f1487f595b0b0bb0b6fb0722', '7f0e37f0e37f14898082b0723b02d5', '7ec967f0e37f14998082b0787b0721', '7f07e7f0e47f531b0723b0b6fb0722', '7f0e37f1487f531b0b0bb0b6fb0722', '7f0e37f0e37f14898082b0723b02d5', '7ec967f0e37f14998082b0787b06bd', '7f07e7f0e47f531b0723b0b6fb0721', '7f0e37f1487f531b0b0bb0b6fb0722', '7f0e37f0e37f14898082b072297c35', '7ec967f0e37f14998082b0787b06bd', '7f07e7f0e47f531b0723b0b6fb0721', '7f0e27f1487f531b0b0bb0b6fb0722', '7f0e37f0e37f14898082b072297c35', '7ec967f0e37f14998082b0787b06bd', '7f07e7f0e47f531b0723b0b6fb0721', '7f0e27f1487f531b0b0bb0b6fb0722', '7f0e37f0e366aa89801eb072297c35', '7ec967f0e37f14998082b0787b06bd', '7f07e7f0e47f149b0723b0787b0721', '7f0e27f1487f531b0b0bb0b6fb0722', '7f0e37f0e366aa89801eb072297c35', '7ec967f0e37f14998082b0723b06bd', '7f07e7f0e47f149b0723b0787b0721', '7f0e27f0e47f531b0723b0b6fb0722', '7f0e37f0e366aa89801eb072297c35', '7ec967f0e37f14998082b0723b06bd', '7f07e7f0e37f14998083b0787b0721', '7f0e27f0e47f531b0723b0b6fb0722', '7f0e37f0e366aa89801eb072297c35', '7ec967f0e37f14898082b0723b02d5', '7f07e7f0e37f14998082b0787b0721', '7f07e7f0e47f531b0723b0b6fb0722', '7f0e36665b66aa89801e9808297c35', '665f67f0e37f14898082b0723b02d5', '7ec967f0e37f14998082b0787b0721', '7f07e7f0e47f531b0723b0b6fb0722', '7f0e36665b66a449801e9808297c35', '665f67f0e37f14898082b0723b02d5', '7ec967f0e37f14998082b0787b06bd', '7f07e7f0e47f531b0723b0b6fb0721', '7f0e36665b66a449801e9808297c35', '665f67f0e37f14898082b072297c35', '7ec967f0e37f14998082b0787b06bd', '7f07e7f0e47f531b0723b0b6fb0721', '7f0e26665b66a449801e9808297c35', '665f67f0e37f1489801eb072297c35', '7ec967f0e37f14998082b0787b06bd', '7f07e7f0e47f531b0723b0b6fb0721', '7f0e27f1487f531b0b0bb0b6fb0722'];

  var nStr1 = ["\u65E5", "\u4E00", "\u4E8C", "\u4E09", "\u56DB", "\u4E94", "\u516D", "\u4E03", "\u516B", "\u4E5D", "\u5341"];
  var nStr2 = ["\u521D", "\u5341", "\u5EFF", "\u5345"];
  var nStr3 = ["\u6B63", "\u4E8C", "\u4E09", "\u56DB", "\u4E94", "\u516D", "\u4E03", "\u516B", "\u4E5D", "\u5341", "\u51AC", "\u814A"];

  /**
   * @1900-2100Gregorian calendar within the interval、Lunar Calendar Conversion
   * @charset UTF-8
   * @Author  JeaYang(JJonline@JJonline.Cn)
   * @Time    2014-7-21
   * @Time    2016-8-13 Fixed 2033hex、Attribution Annals
   * @Time    2016-9-25 Fixed lunar LeapMonth Param Bug
   * @Time    2017-7-24 Fixed use getTerm Func Param Error.use solar year,NOT lunar year
   * @Version 1.0.3
   * @Convert Gregorian calendar to Lunar calendar：calendar.solar2lunar(1987,11,01); //[you can ignore params of prefix 0]
   * @Convert lunar calendar to Gregorian calendar：calendar.lunar2solar(1987,09,10); //[you can ignore params of prefix 0]
   */
  var calendar = {
    /**
     * Lunar calendar1900-2100Moisture Size Information Table
     * @Array Of Property
     * @return Hex
     */
    lunarInfo: lunarInfo,
    /**
     * The months of the Gregorian calendarskyNumber ordinary table
     * @Array Of Property
     * @return Number
     */
    solarMonth: solarMonth,
    /**
     * skyHeavenly Stems and Earthly BranchesskyDry Quick Reference Table
     * @Array Of Property trans["First","B","C","Man","Wu","self","Geng","pungent","the ninth of the ten Heavenly Stems","Gui"]
     * @return Cn string
     */
    Gan: Gan,
    /**
     * skyQuick Reference Table of Heavenly Stems and Earthly Branches
     * @Array Of Property
     * @trans["child","ugly","Tiger","Mao","Chen","Si (the sixth Earthly Branch in the Chinese zodiac)","noon","Not yet","Apply","You (the tenth Earthly Branch)","Xu","Hai"]
     * @return Cn string
     */
    Zhi: Zhi,
    /**
     * skyQuick Reference Table of Heavenly Stems and Earthly Branches<=>Chinese zodiac
     * @Array Of Property
     * @trans["rat","Ox","Tiger","Rabbit","dragon","snake","Horse","sheep","monkey","chicken","dog","pig"]
     * @return Cn string
     */
    Animals: ChineseZodiac,
    /**
     * Solar Calendar Festivalday
     */
    festival: festival,
    /**
     * Lunar calendar festivalday
     */
    lFestival: lFestival,
    /**
     * 24Quick Reference Table of Solar Terms
     * @Array Of Property
     * @trans["Minor Cold","Great Cold","Beginning of spring","rainwater","Waking of Insects","Spring Equinox","Qingming","Grain Rain","Beginning of Summer","Grain Full","Miscanthus","summer solstice","Lesser Heat","Great Heat","beginning of autumn","End of Heat","White Dew","autumnal equinox","cold dew","frost","Beginning of Winter","Xiaoxue","heavy snow","winter solstice"]
     * @return Cn string
     */
    solarTerm: solarTerm,
    /**
     * 1900-2100of each year24Solar termsdayPeriod Quick Reference Table
     * @Array Of Property
     * @return 0x string For splice
     */
    sTermInfo: sTermInfo,
    /**
     * digital conversioninQuick Reference Table
     * @Array Of Property
     * @trans ['day','one','Two','Three','Four','five','Six','Seven','eight','Nine','ten']
     * @return Cn string
     */
    nStr1: nStr1,
    /**
     * dayQuick Reference Table for Converting Dates to the Lunar Calendar
     * @Array Of Property
     * @trans ['Beginning','ten','Twenty','thirty']
     * @return Cn string
     */
    nStr2: nStr2,
    /**
     * Quick Reference Table for Converting Months to Lunar Calendar Names
     * @Array Of Property
     * @trans ['just','one','Two','Three','Four','five','Six','Seven','eight','Nine','ten','Winter','preserved']
     * @return Cn string
     */
    nStr3: nStr3,
    /**
     * ReturnDefaultDefined solar calendar festivalday
     */
    getFestival: function getFestival() {
      return this.festival;
    },
    /**
     * ReturnDefaultdefinedcontentinternal jointday
     */
    getLunarFestival: function getLunarFestival() {
      return this.lFestival;
    },
    /**
     *
     * @param param {Object} According tofestivalinput in the formatData，Set Solar Calendar Festivalday
     */
    setFestival: function setFestival() {
      var param = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : {};
      this.festival = param;
    },
    /**
     *
     * @param param {Object} According tolFestivalinput in the formatData，Set lunar calendar festivalday
     */
    setLunarFestival: function setLunarFestival() {
      var param = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : {};
      this.lFestival = param;
    },
    /**
     * Return to the lunar calendaryYearonewholetotal of the yearskynumber
     * @param y lunar Year
     * @return Number
     * @eg:var count = calendar.lYearDays(1987) ;//count=387
     */
    lYearDays: function lYearDays(y) {
      var i,
        sum = 348;
      for (i = 0x8000; i > 0x8; i >>= 1) {
        sum += this.lunarInfo[y - 1900] & i ? 1 : 0;
      }
      return sum + this.leapDays(y);
    },
    /**
     * Return to the lunar calendaryLeap monthYesWhich month；ifyThe year has no leap month then return0
     * @param y lunar Year
     * @return Number (0-12)
     * @eg:var leapMonth = calendar.leapMonth(1987) ;//leapMonth=6
     */
    leapMonth: function leapMonth(y) {
      //Leap character encoding \u95f0
      return this.lunarInfo[y - 1900] & 0xf;
    },
    /**
     * Return to the lunar calendaryof the leap monthskynumber If there is no leap month in that year, then return0
     * @param y lunar Year
     * @return Number (0、29、30)
     * @eg:var leapMonthDay = calendar.leapDays(1987) ;//leapMonthDay=29
     */
    leapDays: function leapDays(y) {
      if (this.leapMonth(y)) {
        return this.lunarInfo[y - 1900] & 0x10000 ? 30 : 29;
      }
      return 0;
    },
    /**
     * Return to the lunar calendaryYearmMoon（Non-leap month）overallskynumber，Calculatemfor a leap monthtimeofskyPlease use numbersleapDaysMethod
     * @param y lunar Year
     * @param m lunar Month
     * @return Number (-1、29、30)
     * @eg:var MonthDay = calendar.monthDays(1987,9) ;//MonthDay=29
     */
    monthDays: function monthDays(y, m) {
      if (m > 12 || m < 1) {
        return -1;
      } // The month parameter ranges from 1 to 12, and -1 is returned for parameter errors.
      return this.lunarInfo[y - 1900] & 0x10000 >> m ? 30 : 29;
    },
    /**
     * Return to Gregorian calendar(!)yYearmof the moonskynumber
     * @param y solar Year
     * @param m solar Month
     * @return Number (-1、28、29、30、31)
     * @eg:var solarMonthDay = calendar.leapDays(1987) ;//solarMonthDay=30
     */
    solarDays: function solarDays(y, m) {
      if (m > 12 || m < 1) {
        return -1;
      } // If the parameter is incorrect, return -1
      var ms = m - 1;
      if (ms === 1) {
        //The leap level pattern in February is calculated and confirmed to return 28 or 29.
        return y % 4 === 0 && y % 100 !== 0 || y % 400 === 0 ? 29 : 28;
      } else {
        return this.solarMonth[ms];
      }
    },
    /**
     * Convert lunar calendar year to heavenly stems and earthly branches
     * @param  lYear The year number of the lunar calendar
     * @return Cn string
     */
    toGanZhiYear: function toGanZhiYear(lYear) {
      var ganKey = (lYear - 3) % 10;
      var zhiKey = (lYear - 3) % 12;
      if (ganKey === 0) ganKey = 10; // If the remainder is 0, it is the last heavenly stem
      if (zhiKey === 0) zhiKey = 12; // If the remainder is 0, it is the last earthly branch
      return this.Gan[ganKey - 1] + this.Zhi[zhiKey - 1];
    },
    /**
     * Gregorian calendar month、dayDetermine the zodiac sign
     * @param  cMonth [description]
     * @param  cDay [description]
     * @return Cn string
     */
    toAstro: function toAstro(cMonth, cDay) {
      var s = "\u6469\u7FAF\u6C34\u74F6\u53CC\u9C7C\u767D\u7F8A\u91D1\u725B\u53CC\u5B50\u5DE8\u87F9\u72EE\u5B50\u5904\u5973\u5929\u79E4\u5929\u874E\u5C04\u624B\u6469\u7FAF";
      var arr = [20, 19, 21, 21, 21, 22, 23, 23, 23, 23, 22, 22];
      return s.substr(cMonth * 2 - (cDay < arr[cMonth - 1] ? 2 : 0), 2) + "\u5EA7"; // seat
    },

    /**
     * Passed inoffsetOffset returns the Heavenly Stems and Earthly Branches
     * @param offset RelativeFirstchildoffset
     * @return Cn string
     */
    toGanZhi: function toGanZhi(offset) {
      return this.Gan[offset % 10] + this.Zhi[offset % 12];
    },
    /**
     * Enter the Gregorian calendar(!)yYear received that yearNumbernThe solar term's Gregorian calendar datedayperiod
     * @param y yGregorian year(1900-2100)
     * @param n nTwotenFourSolar termsinofNumberSeveral solar terms(1~24)；fromn=1(Minor Cold)Counted from
     * @return day Number
     * @eg:var _24 = calendar.getTerm(1987,3) ;// _24=4; means the beginning of spring on February 4, 1987
     */
    getTerm: function getTerm(y, n) {
      if (y < 1900 || y > 2100 || n < 1 || n > 24) {
        return -1;
      }
      var _table = this.sTermInfo[y - 1900];
      var _calcDay = [];
      for (var index = 0; index < _table.length; index += 5) {
        var chunk = parseInt('0x' + _table.substr(index, 5)).toString();
        _calcDay.push(chunk[0], chunk.substr(1, 2), chunk[3], chunk.substr(4, 2));
      }
      return parseInt(_calcDay[n - 1]);
    },
    /**
     * Pass in a lunar calendar numeric month and return the common Chinese expression
     * @param m lunar month
     * @return Cn string
     * @eg:var cnMonth = calendar.toChinaMonth(12) ;// cnMonth='Twelfth lunar month'
     */
    toChinaMonth: function toChinaMonth(m) {
      // Month => \u6708
      if (m > 12 || m < 1) {
        return -1;
      } // If the parameter is incorrect, return -1
      var s = this.nStr3[m - 1];
      s += "\u6708"; // Add the word "month"
      return s;
    },
    /**
     * Enter the lunar calendardayReturn the Chinese character representation of the period number
     * @param d lunar day
     * @return Cn string
     * @eg:var cnDay = calendar.toChinaDay(21) ;// cnMonth='Twenty-one'
     */
    toChinaDay: function toChinaDay(d) {
      //Day => \u65e5
      var s;
      switch (d) {
        case 10:
          s = "\u521D\u5341";
          break;
        case 20:
          s = "\u4E8C\u5341";
          break;
        case 30:
          s = "\u4E09\u5341";
          break;
        default:
          s = this.nStr2[Math.floor(d / 10)];
          s += this.nStr1[d % 10];
      }
      return s;
    },
    /**
     * Year to Zodiac[!Can only roughly convert] => precisely cutpointsChinese zodiacpointsBoundaryYes“Beginning of spring”
     * @param y year
     * @return Cn string
     * @eg:var animal = calendar.getAnimal(1987) ;// animal='rabbit'
     */
    getAnimal: function getAnimal(y) {
      return this.Animals[(y - 4) % 12];
    },
    /**
     * Input Gregorian calendar year and monthdayObtain a detailed Gregorian calendar、Lunar calendarobjectInformation <=>JSON
     * !important! Gregorian calendarParameterInterval1900.1.31~2100.12.31
     * @param yPara  solar year
     * @param mPara  solar month
     * @param dPara  solar day
     * @return JSON object
     * @eg:console.log(calendar.solar2lunar(1987,11,01));
     */
    solar2lunar: function solar2lunar(yPara, mPara, dPara) {
      var y = parseInt(yPara);
      var m = parseInt(mPara);
      var d = parseInt(dPara);
      //Year limit, upper limit
      if (y < 1900 || y > 2100) {
        return -1; // undefined is converted to a number and becomes NaN
      }
      //Minimum limit for passing parameters in the Gregorian calendar
      if (y === 1900 && m === 1 && d < 31) {
        return -1;
      }

      //No parameters were passed and the same day was obtained
      var objDate;
      if (!y) {
        objDate = new Date();
      } else {
        objDate = new Date(y, parseInt(m) - 1, d);
      }
      var i,
        leap = 0,
        temp = 0;
      //Correct ymd parameters
      y = objDate.getFullYear();
      m = objDate.getMonth() + 1;
      d = objDate.getDate();
      var offset = (Date.UTC(objDate.getFullYear(), objDate.getMonth(), objDate.getDate()) - Date.UTC(1900, 0, 31)) / 86400000;
      for (i = 1900; i < 2101 && offset > 0; i++) {
        temp = this.lYearDays(i);
        offset -= temp;
      }
      if (offset < 0) {
        offset += temp;
        i--;
      }

      //whether today
      var isTodayObj = new Date(),
        isToday = false;
      if (isTodayObj.getFullYear() === y && isTodayObj.getMonth() + 1 === m && isTodayObj.getDate() === d) {
        isToday = true;
      }
      //day of week
      var nWeek = objDate.getDay(),
        cWeek = this.nStr1[nWeek];
      //The number indicates the day of the week in accordance with the custom of starting on Monday in China
      if (nWeek === 0) {
        nWeek = 7;
      }
      //lunar year
      var year = i;
      leap = this.leapMonth(i); // Which month is leap?
      var isLeap = false;

      //Effective leap month
      for (i = 1; i < 13 && offset > 0; i++) {
        //leap month
        if (leap > 0 && i === leap + 1 && isLeap === false) {
          --i;
          isLeap = true;
          temp = this.leapDays(year); // Calculate the number of days in a leap month in the lunar calendar
        } else {
          temp = this.monthDays(year, i); // Calculate the number of days in ordinary months of the lunar calendar
        }
        //Remove leap month
        if (isLeap === true && i === leap + 1) {
          isLeap = false;
        }
        offset -= temp;
      }
      // Leap months cause array subscripts to overlap and be negated
      if (offset === 0 && leap > 0 && i === leap + 1) {
        if (isLeap) {
          isLeap = false;
        } else {
          isLeap = true;
          --i;
        }
      }
      if (offset < 0) {
        offset += temp;
        --i;
      }
      //lunar month
      var month = i;
      //lunar day
      var day = offset + 1;
      //Processing of Heavenly Stems and Earthly Branches
      var sm = m - 1;
      var gzY = this.toGanZhiYear(year);

      // The two solar terms of the month
      // bugfix-2017-7-24 11:03:38 use lunar Year Param `y` Not `year`
      var firstNode = this.getTerm(y, m * 2 - 1); // Returns the day on which the "festival" of the current month begins.
      var secondNode = this.getTerm(y, m * 2); // Returns the day on which the "festival" of the current month begins.

      // Correct the stems and branches according to the 12 solar terms
      var gzM = this.toGanZhi((y - 1900) * 12 + m + 11);
      if (d >= firstNode) {
        gzM = this.toGanZhi((y - 1900) * 12 + m + 12);
      }

      //Whether the incoming date has a solar term or not
      var isTerm = false;
      var Term = null;
      if (firstNode === d) {
        isTerm = true;
        Term = this.solarTerm[m * 2 - 2];
      }
      if (secondNode === d) {
        isTerm = true;
        Term = this.solarTerm[m * 2 - 1];
      }
      //The number of days between the first day of the current month and 1900/1/1
      var dayCyclical = Date.UTC(y, sm, 1, 0, 0, 0, 0) / 86400000 + 25567 + 10;
      var gzD = this.toGanZhi(dayCyclical + d - 1);
      //The zodiac sign this date belongs to
      var astro = this.toAstro(m, d);
      var solarDate = y + '-' + m + '-' + d;
      var lunarDate = year + '-' + month + '-' + day;
      var festival = this.festival;
      var lFestival = this.lFestival;
      var festivalDate = m + '-' + d;
      var lunarFestivalDate = month + '-' + day;

      // bugfix https://github.com/jjonline/calendar.js/issues/29
      // Correction of Lunar Calendar Festivals: New Year's Eve falls on the 29th in the small month of December in the lunar calendar, and New Year's Eve in the big month falls on the 30th
      // Tricky correction here: Add a judgment when the current date is December 29th of the lunar calendar and set lunarFestivalDate to 12-30 to correctly obtain New Year's Eve
      // The principle of the Chinese lunar calendar is that the leap month will not pass before or after the festival. The number of days in December of the lunar calendar is taken here without considering the leap month.
      // Lunar December only occurs 1574 times in the 200-year interval supported by this tool
      if (month === 12 && day === 29 && this.monthDays(year, month) === 29) {
        lunarFestivalDate = '12-30';
      }
      return {
        date: solarDate,
        lunarDate: lunarDate,
        festival: festival[festivalDate] ? festival[festivalDate].title : null,
        lunarFestival: lFestival[lunarFestivalDate] ? lFestival[lunarFestivalDate].title : null,
        'lYear': year,
        'lMonth': month,
        'lDay': day,
        'Animal': this.getAnimal(year),
        'IMonthCn': (isLeap ? "\u95F0" : '') + this.toChinaMonth(month),
        'IDayCn': this.toChinaDay(day),
        'cYear': y,
        'cMonth': m,
        'cDay': d,
        'gzYear': gzY,
        'gzMonth': gzM,
        'gzDay': gzD,
        'isToday': isToday,
        'isLeap': isLeap,
        'nWeek': nWeek,
        'ncWeek': "\u661F\u671F" + cWeek,
        'isTerm': isTerm,
        'Term': Term,
        'astro': astro
      };
    },
    /**
     * Pass in the lunar calendar year and monthdayand the passed-in monthYesnoGet detailed Gregorian calendar for the leap month、Lunar calendarobjectInformation <=>JSON
     * !important! ParameterInterval1900.1.31~2100.12.1
     * @param y  lunar year
     * @param m  lunar month
     * @param d  lunar day
     * @param isLeapMonth  lunar month is leap or not.[IfYesLeap month in the lunar calendarNumberFourpieceParameterFuvaluetrueThat's it]
     * @return JSON object
     * @eg:console.log(calendar.lunar2solar(1987,9,10));
     */
    lunar2solar: function lunar2solar(y, m, d, isLeapMonth) {
      y = parseInt(y);
      m = parseInt(m);
      d = parseInt(d);
      isLeapMonth = !!isLeapMonth;
      var leapMonth = this.leapMonth(y);
      this.leapDays(y);
      if (isLeapMonth && leapMonth !== m) {
        return -1;
      } // The leap month in the Gregorian calendar is required to be calculated when the parameter is passed, but the leap month obtained in that year is different from the month of the passed parameter.
      if (y === 2100 && m === 12 && d > 1 || y === 1900 && m === 1 && d < 31) {
        return -1;
      } // Maximum limit exceeded
      var day = this.monthDays(y, m);
      var _day = day;
      //bugFix 2016-9-25
      //if month is leap, _day use leapDays method
      if (isLeapMonth) {
        _day = this.leapDays(y, m);
      }
      if (y < 1900 || y > 2100 || d > _day) {
        return -1;
      } // Parameter legality verification

      //Calculate the time difference of the lunar calendar
      var offset = 0;
      var i;
      for (i = 1900; i < y; i++) {
        offset += this.lYearDays(i);
      }
      var leap = 0,
        isAdd = false;
      for (i = 1; i < m; i++) {
        leap = this.leapMonth(y);
        if (!isAdd) {
          //Handling leap months
          if (leap <= i && leap > 0) {
            offset += this.leapDays(y);
            isAdd = true;
          }
        }
        offset += this.monthDays(y, i);
      }
      //To convert the leap month to the lunar calendar, the time difference of the month before the leap month of the year needs to be added.
      if (isLeapMonth) {
        offset += day;
      }
      //The Gregorian calendar time on the first day of the first lunar month in 1900 was 0:00:00 on January 30, 1900 (this time is also the starting point of this lunar calendar)
      var strap = Date.UTC(1900, 1, 30, 0, 0, 0);
      var calObj = new Date((offset + d - 31) * 86400000 + strap);
      var cY = calObj.getUTCFullYear();
      var cM = calObj.getUTCMonth() + 1;
      var cD = calObj.getUTCDate();
      return this.solar2lunar(cY, cM, cD);
    }
  };

  return calendar;

})();

export default calendar;