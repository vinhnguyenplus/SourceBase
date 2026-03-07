/**
 * timespacedayTerm Conversion
 * @param date Currenttimespace，new Date() Format
 * @param format Needs to be convertedtime formatString
 * @description format Arbitrary string，such as `YYYY-mm、YYYY-mm-dd`
 * @description format quarter："YYYY-mm-dd HH:MM:SS QQQQ"
 * @description format week："YYYY-mm-dd HH:MM:SS WWW"
 * @description format how many / severalweek："YYYY-mm-dd HH:MM:SS ZZZ"
 * @description format quarter + week + how many / severalweek："YYYY-mm-dd HH:MM:SS WWW QQQQ ZZZ"
 * @returns Return the concatenatedtimeintermediate string
 */
export function formatDate(date: Date, format: string): string {
	let we = date.getDay(); // Week
	let z = getWeek(date); // week
	let qut = Math.floor((date.getMonth() + 3) / 3).toString(); // quarter
	const opt: { [key: string]: string } = {
		'Y+': date.getFullYear().toString(), // Year
		'm+': (date.getMonth() + 1).toString(), // Month (the month starts from 0 and needs to be +1)
		'd+': date.getDate().toString(), // day
		'H+': date.getHours().toString(), // hour
		'M+': date.getMinutes().toString(), // point
		'S+': date.getSeconds().toString(), // Second
		'q+': qut, // quarter
	};
	// Chinese numerals (weekday)
	const week: { [key: string]: string } = {
		'0': 'day',
		'1': 'one',
		'2': 'Two',
		'3': 'Three',
		'4': 'Four',
		'5': 'five',
		'6': 'Six',
	};
	// Chinese numbers (quarterly)
	const quarter: { [key: string]: string } = {
		'1': 'one',
		'2': 'Two',
		'3': 'Three',
		'4': 'Four',
	};
	if (/(W+)/.test(format)) format = format.replace(RegExp.$1, RegExp.$1.length > 1 ? (RegExp.$1.length > 2 ? 'week' + week[we] : 'week' + week[we]) : week[we]);
	if (/(Q+)/.test(format)) format = format.replace(RegExp.$1, RegExp.$1.length == 4 ? 'Number' + quarter[qut] + 'quarter' : quarter[qut]);
	if (/(Z+)/.test(format)) format = format.replace(RegExp.$1, RegExp.$1.length == 3 ? 'Number' + z + 'week' : z + '');
	for (let k in opt) {
		let r = new RegExp('(' + k + ')').exec(format);
		// If the input length is not 1, zeros will be added to the front.
		if (r) format = format.replace(r[1], RegExp.$1.length == 1 ? opt[k] : opt[k].padStart(RegExp.$1.length, '0'));
	}
	return format;
}

/**
 * ObtainCurrentdayperiodYesNumberhow many / severalweek
 * @param dateTime Currently passed indayperiodvalue
 * @returns ReturnNumberhow many / severalweekNumbervalue
 */
export function getWeek(dateTime: Date): number {
	let temptTime = new Date(dateTime.getTime());
	// Day of the week
	let weekday = temptTime.getDay() || 7;
	// 1+5 days of the week = Saturday
	temptTime.setDate(temptTime.getDate() - weekday + 1 + 5);
	let firstDay = new Date(temptTime.getFullYear(), 0, 1);
	let dayOfWeek = firstDay.getDay();
	let spendDay = 1;
	if (dayOfWeek != 0) spendDay = 7 - dayOfWeek + 1;
	firstDay = new Date(temptTime.getFullYear(), 0, 1 + spendDay);
	let d = Math.ceil((temptTime.valueOf() - firstDay.valueOf()) / 86400000);
	let result = Math.ceil(d / 7);
	return result;
}

/**
 * willtimeconvert to `a few seconds ago`、`A few minutes ago`、`hours ago`、`a few days ago`
 * @param param Currenttimespace，new Date() FormatorStringtime format
 * @param format Needs to be convertedtime formatString
 * @description param 10second：  10 * 1000
 * @description param 1points：   60 * 1000
 * @description param 1smalltime： 60 * 60 * 1000
 * @description param 24smalltime：60 * 60 * 24 * 1000
 * @description param 3sky：   60 * 60* 24 * 1000 * 3
 * @returns Return the concatenatedtimeintermediate string
 */
export function formatPast(param: string | Date, format: string = 'YYYY-mm-dd'): string {
	// Incoming format processing, storing conversion values
	let t: any, s: number;
	// Get js timestamp
	let time: number = new Date().getTime();
	// Is it an object
	typeof param === 'string' || 'object' ? (t = new Date(param).getTime()) : (t = param);
	// current timestamp - incoming timestamp
	time = Number.parseInt(`${time - t}`);
	if (time < 10000) {
		// within 10 seconds
		return 'just now';
	} else if (time < 60000 && time >= 10000) {
		// More than 10 seconds and less than 1 minute
		s = Math.floor(time / 1000);
		return `${s} seconds ago`;
	} else if (time < 3600000 && time >= 60000) {
		// More than 1 minute and less than 1 hour
		s = Math.floor(time / 60000);
		return `${s} minutes ago`;
	} else if (time < 86400000 && time >= 3600000) {
		// More than 1 hour less than 24 hours
		s = Math.floor(time / 3600000);
		return `${s} hours ago`;
	} else if (time < 259200000 && time >= 86400000) {
		// More than 1 day and less than 3 days
		s = Math.floor(time / 86400000);
		return `${s} days ago`;
	} else {
		// more than 3 days
		let date = typeof param === 'string' || 'object' ? new Date(param) : param;
		return formatDate(date, format);
	}
}

/**
 * timeInterrogative Greeting
 * @param param Currenttimespace，new Date() Format
 * @description param Call `formatAxis(new Date())` Output `good morning`
 * @returns Return the concatenatedtimeintermediate string
 */
export function formatAxis(param: Date): string {
	let hour: number = new Date(param).getHours();
	if (hour < 6) return 'Good early morning';
	else if (hour < 9) return 'good morning';
	else if (hour < 12) return 'good morning';
	else if (hour < 14) return 'good noon';
	else if (hour < 17) return 'good afternoon';
	else if (hour < 19) return 'Good evening';
	else if (hour < 22) return 'Good evening';
	else return 'Good night';
}

/**
 * ObtainTwotimewith a time differencesecondnumber
 * @dateBegin start time，new Date() Format
 * @dateEnd end time，new Date() Format
 * @returns Returnsecondnumber
 */
export function getTimeDiff(dateBegin: Date, dateEnd: Date,) {
	var dateDiff = dateEnd.getTime() - dateBegin.getTime();
	return dateDiff / 1000;
}

/**
 * Format twotimeinterval difference
 * @dateBegin start time，new Date() Format
 * @dateEnd end time，new Date() Format
 * @description dateBegin 2025-1-1，dateEnd 2025-1-2 10:10:10 ：   1sky10time10points10second
 * @returns Return the concatenatedtimeintermediate string
 */
export function formatTimeDiff(dateBegin: Date, dateEnd: Date,) {
	var dateDiff = dateEnd.getTime() - dateBegin.getTime();// Time difference in milliseconds
	var dayDiff = Math.floor(dateDiff / (24 * 3600 * 1000));// Calculate the difference in days
	var leave1 = dateDiff % (24 * 3600 * 1000)    // Calculate the number of milliseconds remaining after days
	var hours = Math.floor(leave1 / (3600 * 1000))// Calculate hours
	//Calculate difference in minutes
	var leave2 = leave1 % (3600 * 1000)    // Calculate the number of milliseconds remaining after hours
	var minutes = Math.floor(leave2 / (60 * 1000))// Calculate difference in minutes
	//Calculate the difference in seconds
	var leave3 = leave2 % (60 * 1000)      // Calculate the number of milliseconds remaining after minutes
	var seconds = Math.round(leave3 / 1000);
	var result = "";
	if (dayDiff > 0) {
		result += dayDiff + "sky";
	}
	if (hours > 0) {
		result += hours + "time";
	}
	if (minutes > 0) {
		result += minutes + "points";
	}
	if (seconds >= 0) {
		result += seconds + "second";
	}
	return result;
}

/**
 * willtimeformatted as a string `YYYY-mm-dd HH:MM:SS` Format
 * @param timeStr timeintermediate string，Support nontimeBlock format，such as `YYYYmmdd`、`YYYYmmddHH`、`YYYYmmddHHMM`、`YYYYmmddHHMMSS`、`YYYY/mm/dd`、`YYYY year mm month dd day`Wait
 * @param length returnedtimeintermediate stringlength，Default0Indicates automatic recognitionlength
 * @returns Return the formattedtimeintermediate string
 */
export function formatDateString(timeStr: string | null | undefined, length: number = 0): string {
	if (!timeStr) return '';

	let str = timeStr.replace(/\D/g, '');
	let len = str.length;

	if (len <= 4) return str;

	// Handling odd lengths: pad 0 before last digit
	if (len & 1) {
		len++;
		str = str.slice(0, -1) + '0' + str.slice(-1);
	}

	str = str.padEnd(14, '0');

	// Extract each time part
	const [year, month, day, hour, minute, second] = [0, 4, 6, 8, 10, 12].map(index => str.slice(index, (index || 2) + 2));

	// Calculate the length. When the length is 10, minutes will be displayed.
	const targetLength = length > 0 ? length : len + (len - 4) / 2 + (len == 10 ? 3 : 0);

	// Correction function to handle 0/00
	const fixZero = (value: string) => ['0', '00'].includes(value) ? '01' : value;

	// Generate full format string
	const fullFormat = `${year}-${fixZero(month)}-${fixZero(day)} ${hour}:${minute}:${second}`;

	return fullFormat.slice(0, targetLength);
}