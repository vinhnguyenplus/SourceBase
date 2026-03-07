// general function
import useClipboard from 'vue-clipboard3';
import { ElMessage } from 'element-plus';
import { formatDate } from '/@/utils/formatTime';

export default function () {
	const { toClipboard } = useClipboard();

	// Percent formatting
	const percentFormat = (row: EmptyArrayType, column: number, cellValue: string) => {
		return cellValue ? `${cellValue}%` : '-';
	};
	// List date time formatting
	const dateFormatYMD = (row: EmptyArrayType, column: number, cellValue: string) => {
		if (!cellValue) return '-';
		return formatDate(new Date(cellValue), 'YYYY-mm-dd');
	};
	// List date time formatting
	const dateFormatYMDHMS = (row: EmptyArrayType, column: number, cellValue: string) => {
		if (!cellValue) return '-';
		return formatDate(new Date(cellValue), 'YYYY-mm-dd HH:MM:SS');
	};
	// List date time formatting
	const dateFormatHMS = (row: EmptyArrayType, column: number, cellValue: string) => {
		if (!cellValue) return '-';
		let time = 0;
		if (typeof row === 'number') time = row;
		if (typeof cellValue === 'number') time = cellValue;
		return formatDate(new Date(time * 1000), 'HH:MM:SS');
	};
	// Decimal formatting
	const scaleFormat = (value: string = '0', scale: number = 4) => {
		return Number.parseFloat(value).toFixed(scale);
	};
	// Decimal formatting
	const scale2Format = (value: string = '0') => {
		return Number.parseFloat(value).toFixed(2);
	};
    // Thousands, default to two decimal places
	const groupSeparator = (value: number, minimumFractionDigits: number = 2) => {
		return value.toLocaleString('en-US', {
			minimumFractionDigits: minimumFractionDigits,
			maximumFractionDigits: 2,
		});
	};

	/**
	 * DeleteSpecify characters at the beginning and end of a string
	 * @param Str Source Character
	 * @param char Specified characters to remove
	 * @param type Type，Right side or left side，fornullYesReplace the beginning and the end
	 */
	const trimChar =(Str:string,char:string, type:string) =>{
		if (char) {
			if (type == 'left') {
				return Str.replace(new RegExp('^\\'+char+'+', 'g'), '');
			} else if (type == 'right') {
				return Str.replace(new RegExp('\\'+char+'+$', 'g'), '');
			}
			return Str.replace(new RegExp('^\\'+char+'+|\\'+char+'+$', 'g'), '');
		}
		return Str.replace(/^\s+|\s+$/g, '');
	}
	// Click to copy text
	const copyText = (text: string) => {
		return new Promise((resolve, reject) => {
			try {
				//copy
				toClipboard(text);
				//You can set the prompt box for successful copying and other operations below.
				ElMessage.success(t('message.layout.copyTextSuccess'));
				resolve(text);
			} catch (e) {
				//Copy failed
				ElMessage.error(t('message.layout.copyTextError'));
				reject(e);
			}
		});
	};
	// Remove the Html tag (take the first 5 characters)
	const removeHtmlSub = (value: string) => {
		var str = value.replace(/<[^>]+>/g, '');
		if (str.length > 50) return str.substring(0, 50) + '......';
		else return str;
	};
	// Remove Html tag
	const removeHtml = (value: string) => {
		return value.replace(/<[^>]+>/g, '');
	};
	// Get enumeration description
	const getEnumDesc = (key: any, lstEnum: any) => {
		return lstEnum.find((x: any) => x.value == key)?.describe;
	};
	// Append query parameters to url
	const appendQueryParams = (url: string, params: { [key : string]: any }) => {
		if (!params || Object.keys(params).length == 0) return url;
		const queryString = Object.keys(params).map(key => `${encodeURIComponent(key)}=${encodeURIComponent(params[key])}`).join('&');
		return `${url}${url.includes('?') ? '&' : '?'}${queryString}`;
	};
	return {
		percentFormat,
		dateFormatYMD,
		dateFormatYMDHMS,
		dateFormatHMS,
		scaleFormat,
		scale2Format,
        groupSeparator,
		copyText,
		removeHtmlSub,
		removeHtml,
		getEnumDesc,
		appendQueryParams,
		trimChar,
	};
}
