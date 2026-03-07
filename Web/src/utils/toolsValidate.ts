/**
 * 2020.11.29 lyt wholePrinciple
 * Utility class collection，Suitable for flattimeDevelop
 * Add NewMulti-line comment information，ratSimply place the label on the method nameView
 */

/**
 * Verify hundredpointsthan（Decimals are not allowed）
 * @param val CurrentvalueString
 * @returns Return the processed string
 */
export function verifyNumberPercentage(val: string): string {
	// Match spaces
	let v = val.replace(/(^\s*)|(\s*$)/g, '');
	// Can only be numbers and decimal points, not other inputs
	v = v.replace(/[^\d]/g, '');
	// cannot start with 0
	v = v.replace(/^0/g, '');
	// If the number exceeds 100, assign it to the maximum value of 100
	v = v.replace(/^[1-9]\d\d{1,3}$/, '100');
	// Return results
	return v;
}

/**
 * Verify hundredpointsthan（Decimals are allowed）
 * @param val CurrentvalueString
 * @returns Return the processed string
 */
export function verifyNumberPercentageFloat(val: string): string {
	let v = verifyNumberIntegerAndFloat(val);
	// If the number exceeds 100, assign it to the maximum value of 100
	v = v.replace(/^[1-9]\d\d{1,3}$/, '100');
	// No further input will be given after exceeding 100
	v = v.replace(/^100\.$/, '100');
	// Return results
	return v;
}

/**
 * Decimal orwholenumber(Cannot be negative)
 * @param val CurrentvalueString
 * @returns Return the processed string
 */
export function verifyNumberIntegerAndFloat(val: string) {
	// Match spaces
	let v = val.replace(/(^\s*)|(\s*$)/g, '');
	// Can only be numbers and decimal points, not other inputs
	v = v.replace(/[^\d.]/g, '');
	// Starting with 0, only one can be entered.
	v = v.replace(/^0{2}$/g, '0');
	// Ensure that the first digit can only be a number, not a dot
	v = v.replace(/^\./g, '');
	// Only 1 decimal place can appear
	v = v.replace('.', '$#$').replace(/\./g, '').replace('$#$', '.');
	// Keep 2 digits after the decimal point
	v = v.replace(/^(\-)*(\d+)\.(\d\d).*$/, '$1$2.$3');
	// Return results
	return v;
}

/**
 * justwholedigital verification
 * @param val CurrentvalueString
 * @returns Return the processed string
 */
export function verifiyNumberInteger(val: string) {
	// Match spaces
	let v = val.replace(/(^\s*)|(\s*$)/g, '');
	// Remove '.' to prevent problems when pasting, such as 0.1.12.12
	v = v.replace(/[\.]*/g, '');
	// Remove the numbers starting with 0 to prevent problems when pasting, such as 00121323
	v = v.replace(/(^0[\d]*)$/g, '0');
	// The first bit is 0 and can only appear once
	v = v.replace(/^0\d$/g, '0');
	// Match only numbers
	v = v.replace(/[^\d]/g, '');
	// Return results
	return v;
}

/**
 * Removeintext andnullgrid
 * @param val CurrentvalueString
 * @returns Return the processed string
 */
export function verifyCnAndSpace(val: string) {
	// Match Chinese characters and spaces
	let v = val.replace(/[\u4e00-\u9fa5\s]+/g, '');
	// Match spaces
	v = v.replace(/(^\s*)|(\s*$)/g, '');
	// Return results
	return v;
}

/**
 * Remove English andnullgrid
 * @param val CurrentvalueString
 * @returns Return the processed string
 */
export function verifyEnAndSpace(val: string) {
	// Match English and spaces
	let v = val.replace(/[a-zA-Z]+/g, '');
	// Match spaces
	v = v.replace(/(^\s*)|(\s*$)/g, '');
	// Return results
	return v;
}

/**
 * Input prohibitednullgrid
 * @param val CurrentvalueString
 * @returns Return the processed string
 */
export function verifyAndSpace(val: string) {
	// Match spaces
	let v = val.replace(/(^\s*)|(\s*$)/g, '');
	// Return results
	return v;
}

/**
 * Amountuse `,` DistrictpointsOpen
 * @param val CurrentvalueString
 * @returns Return the processed string
 */
export function verifyNumberComma(val: string) {
	// Call the decimal or integer (not negative) method
	let v: any = verifyNumberIntegerAndFloat(val);
	// Convert string to array
	v = v.toString().split('.');
	// \B matches non-word boundaries, either with word characters on either side or with non-word characters on both sides
	v[0] = v[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
	// Array to string
	v = v.join('.');
	// Return results
	return v;
}

/**
 * Change the color of matching text（Searchtime）
 * @param val CurrentvalueString
 * @param text String to be processedvalue
 * @param color FoundtimeFont highlight color
 * @returns Return the processed string
 */
export function verifyTextColor(val: string, text = '', color = 'red') {
	// Return content, add color
	let v = text.replace(new RegExp(val, 'gi'), `<span style='color: ${color}'>${val}</span>`);
	// Return results
	return v;
}

/**
 * digital conversioninCapitalized text
 * @param val CurrentvalueString
 * @param unit Default：Thousand Hundred Ten Hundred Million Thousand Hundred Ten Ten Thousand Thousand Hundred Ten Yuan Jiao Fen
 * @returns Return the processed string
 */
export function verifyNumberCnUppercase(val: any, unit = 'Thousand Hundred Ten Hundred Million Thousand Hundred Ten Ten Thousand Thousand Hundred Ten Yuan Jiao Fen', v = '') {
	// The current content string adds 2 0s, why??
	val += '00';
	// Returns the position where a specified string value first appears in the string. If it does not appear, the method returns -1
	let lookup = val.indexOf('.');
	// substring: does not contain the ending subscript content, substr: contains the ending subscript content
	if (lookup >= 0) val = val.substring(0, lookup) + val.substr(lookup + 1, 2);
	// According to the length of the content val, intercept and return the corresponding uppercase
	unit = unit.substr(unit.length - val.length);
	// Loop interception and splicing uppercase
	for (let i = 0; i < val.length; i++) {
		v += 'Zero, one, two, three, four, five, six, eight, nine'.substr(val.substr(i, 1), 1) + unit.substr(i, 1);
	}
	// Regular processing
	v = v
		.replace(/ZerocornerZeropoints$/, 'whole')
		.replace(/Zero[thousands, hundreds, tens]/g, 'Zero')
		.replace(/Zero{2,}/g, 'Zero')
		.replace(/Zero([hundred million|ten thousand])/g, '$1')
		.replace(/Zero+Yuan/, 'Yuan')
		.replace(/hundred millionZero{0,3}ten thousand/, 'hundred million')
		.replace(/^Yuan/, 'Zero yuan');
	// Return results
	return v;
}

/**
 * Mobile phone number
 * @param val CurrentvalueString
 * @returns Return true: Mobile phone numberjustSure
 */
export function verifyPhone(val: string) {
	// false: The mobile phone number is incorrect
	if (!/^1[3456789][0-9]{9}$/.test(val)) return false;
	// true: The mobile phone number is correct
	else return true;
}

/**
 * domesticTelephoneNumber
 * @param val CurrentvalueString
 * @returns Return true: domesticTelephoneNumberjustSure
 */
export function verifyTelPhone(val: string) {
	// false: The domestic phone number is incorrect
	if (!/\d{3}-\d{8}|\d{4}-\d{7}/.test(val)) return false;
	// true: The domestic phone number is correct
	else return true;
}

/**
 * LoginAccount number (Starting with a letter，Allow5-16Byte，Allow letters, numbers, and underscores)
 * @param val CurrentvalueString
 * @returns Return true: LoginAccount numberjustSure
 */
export function verifyAccount(val: string) {
	// false: The login account is incorrect
	if (!/^[a-zA-Z][a-zA-Z0-9_]{4,15}$/.test(val)) return false;
	// true: The login account is correct
	else return true;
}

/**
 * password (Starting with a letter，lengthat/in/on6~16between，can onlyincludeletter、Numbers and underscores)
 * @param val CurrentvalueString
 * @returns Return true: passwordjustSure
 */
export function verifyPassword(val: string) {
	// false: The password is incorrect
	if (!/^[a-zA-Z]\w{5,15}$/.test(val)) return false;
	// true: the password is correct
	else return true;
}

/**
 * Strongpassword (letter+Number+Special characters，lengthat/in/on6-16between)
 * @param val CurrentvalueString
 * @returns Return true: StrongpasswordjustSure
 */
export function verifyPasswordPowerful(val: string) {
	// false: strong password is incorrect
	if (!/^(?![a-zA-z]+$)(?!\d+$)(?![!@#$%^&\.*]+$)(?![a-zA-z\d]+$)(?![a-zA-z!@#$%^&\.*]+$)(?![\d!@#$%^&\.*]+$)[a-zA-Z\d!@#$%^&\.*]{6,16}$/.test(val)) return false;
	// true: strong password is correct
	else return true;
}

/**
 * passwordStrongdegree
 * @param val CurrentvalueString
 * @description weak：Pure numbers，Pure letters，Pure special characters
 * @description in：letter+Number，letter+Special characters，Number+Special characters
 * @description Strong：letter+Number+Special characters
 * @returns Return the processed string：weak、in、Strong
 */
export function verifyPasswordStrength(val: string) {
	let v = '';
	// Weak: pure numbers, pure letters, pure special characters
	if (/^(?:\d+|[a-zA-Z]+|[!@#$%^&\.*]+){6,16}$/.test(val)) v = 'weak';
	// Medium: letters + numbers, letters + special characters, numbers + special characters
	if (/^(?![a-zA-z]+$)(?!\d+$)(?![!@#$%^&\.*]+$)[a-zA-Z\d!@#$%^&\.*]{6,16}$/.test(val)) v = 'in';
	// Strong: letters + numbers + special characters
	if (/^(?![a-zA-z]+$)(?!\d+$)(?![!@#$%^&\.*]+$)(?![a-zA-z\d]+$)(?![a-zA-z!@#$%^&\.*]+$)(?![\d!@#$%^&\.*]+$)[a-zA-Z\d!@#$%^&\.*]{6,16}$/.test(val)) v = 'Strong';
	// Return results
	return v;
}

/**
 * IP address
 * @param val CurrentvalueString
 * @returns Return true: IP addressjustSure
 */
export function verifyIPAddress(val: string) {
	// false: IP address is incorrect
	if (!/^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$/.test(val)) return false;
	// true: IP address is correct
	else return true;
}

/**
 * Email
 * @param val CurrentvalueString
 * @returns Return true: EmailjustSure
 */
export function verifyEmail(val: string) {
	// false: Email is incorrect
	if (!/^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/.test(val)) return false;
	// true: the email is correct
	else return true;
}

/**
 * ID card
 * @param val CurrentvalueString
 * @returns Return true: ID cardjustSure
 */
export function verifyIdCard(val: string) {
	// false: ID card is incorrect
	if (!/^[1-9]\d{5}(18|19|20)\d{2}((0[1-9])|(1[0-2]))(([0-2][1-9])|10|20|30|31)\d{3}[0-9Xx]$/.test(val)) return false;
	// true: ID card is correct
	else return true;
}

/**
 * Name
 * @param val CurrentvalueString
 * @returns Return true: NamejustSure
 */
export function verifyFullName(val: string) {
	// false: name is incorrect
	if (!/^[\u4e00-\u9fa5]{1,6}(·[\u4e00-\u9fa5]{1,6}){0,2}$/.test(val)) return false;
	// true: name is correct
	else return true;
}

/**
 * Postal code
 * @param val CurrentvalueString
 * @returns Return true: Postal codejustSure
 */
export function verifyPostalCode(val: string) {
	// false: Postal code is incorrect
	if (!/^[1-9][0-9]{5}$/.test(val)) return false;
	// true: the zip code is correct
	else return true;
}

/**
 * url Handle
 * @param val CurrentvalueString
 * @returns Return true: url justSure
 */
export function verifyUrl(val: string) {
	if (typeof val !== 'string' || !val.trim()) return false;
	// Strict URL regex, allow private IP addresses
	const strictUrlRegex =
		/^(?:(?:(?:https?|ftp):)?\/\/)(?:\S+(?::\S*)?@)?(?:(?:(?:[1-9]\d?|1\d\d|2[01]\d|22[0-3])(?:\.(?:1?\d{1,2}|2[0-4]\d|25[0-5])){2}(?:\.(?:[1-9]\d?|1\d\d|2[0-4]\d|25[0-4])))|(?:(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)(?:\.(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)*(?:\.(?:[a-z\u00a1-\uffff]{2,})))(?::\d{2,5})?(?:[/?#]\S*)?$/i;
	return strictUrlRegex.test(val);
}

/**
 * License plate number
 * @param val CurrentvalueString
 * @returns Return true：License plate numberjustSure
 */
export function verifyCarNum(val: string) {
	// false: The license plate number is incorrect
	if (
		!/^(([Beijing, Tianjin, Shanghai, Chongqing, Hebei, Henan, Yunnan, Liaoning, Heilongjiang, Hunan, Anhui, Shandong, Xinjiang, Jiangsu, Zhejiang, Jiangxi, Hubei, Guangxi, Gansu, Shanxi, Inner Mongolia, Shaanxi, Jilin, Fujian, Guizhou, Guangdong, Qinghai, Tibet, Sichuan, Ningxia, Hainan, Consulates/Embassies][A-Z](([0-9]{5}[DF])|([DF]([A-HJ-NP-Z0-9])[0-9]{4})))|([Beijing, Tianjin, Shanghai, Chongqing, Hebei, Henan, Yunnan, Liaoning, Heilongjiang, Hunan, Anhui, Shandong, Xinjiang, Jiangsu, Zhejiang, Jiangxi, Hubei, Guangxi, Gansu, Shanxi, Inner Mongolia, Shaanxi, Jilin, Fujian, Guizhou, Guangdong, Qinghai, Tibet, Sichuan, Ningxia, Hainan, Consulates/Embassies][A-Z][A-HJ-NP-Z0-9]{4}[A-HJ-NP-Z0-9Academic Police Liaison to Hong Kong and Macau Consulates]))$/.test(
			val
		)
	)
		return false;
	// true: the license plate number is correct
	else return true;
}

/**
 * AnalysisID card
 */
export function judgementIdCard(idCard: string) {
	if (!idCard?.trim()) return null;
	let entity = {} as any;
	let currentDate = new Date();
	let yearNow = currentDate.getFullYear();
	let birthDateCode = idCard.substring(6, 14);
	let genderCode = parseInt(idCard.substring(16, 17), 10);

	entity.sex = genderCode % 2 === 0 ? 2 : 1;
	entity.age = yearNow - parseInt(birthDateCode.substring(0, 4));
	entity.birthday = `${birthDateCode.substring(0, 4)}-${birthDateCode.substring(4, 6)}-${birthDateCode.substring(6, 8)}`;
	return entity;
}
