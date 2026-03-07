import { ElMessage } from 'element-plus';

/**
 * Color conversion function
 * @method hexToRgb hex Color transition rgb Color
 * @method rgbToHex rgb Color transition Hex Color
 * @method getDarkColor Deepen the colorvalue
 * @method getLightColor Lighter colorvalue
 */
export function useChangeColor() {
	// str color value string
	const hexToRgb = (str: string): any => {
		let hexs: any = '';
		let reg = /^\#?[0-9A-Fa-f]{6}$/;
		if (!reg.test(str)) {
			ElMessage.warning('Inputmistakeofhex');
			return '';
		}
		str = str.replace('#', '');
		hexs = str.match(/../g);
		for (let i = 0; i < 3; i++) hexs[i] = parseInt(hexs[i], 16);
		return hexs;
	};
	// r represents red | g represents green | b represents blue
	const rgbToHex = (r: any, g: any, b: any): string => {
		let reg = /^\d{1,3}$/;
		if (!reg.test(r) || !reg.test(g) || !reg.test(b)) {
			ElMessage.warning('Entered incorrect RGB color value');
			return '';
		}
		let hexs = [r.toString(16), g.toString(16), b.toString(16)];
		for (let i = 0; i < 3; i++) if (hexs[i].length == 1) hexs[i] = `0${hexs[i]}`;
		return `#${hexs.join('')}`;
	};
	// color color value string | level the degree of lightening, limited to 0-1
	const getDarkColor = (color: string, level: number): string => {
		let reg = /^\#?[0-9A-Fa-f]{6}$/;
		if (!reg.test(color)) {
			ElMessage.warning('Entering wrong hex color value');
			return '';
		}
		let rgb = useChangeColor().hexToRgb(color);
		for (let i = 0; i < 3; i++) rgb[i] = Math.floor(rgb[i] * (1 - level));
		return useChangeColor().rgbToHex(rgb[0], rgb[1], rgb[2]);
	};
	// color color value string | level the degree of deepening, limited to 0-1
	const getLightColor = (color: string, level: number): string => {
		let reg = /^\#?[0-9A-Fa-f]{6}$/;
		if (!reg.test(color)) {
			ElMessage.warning('Entering wrong hex color value');
			return '';
		}
		let rgb = useChangeColor().hexToRgb(color);
		for (let i = 0; i < 3; i++) rgb[i] = Math.floor((255 - rgb[i]) * level + rgb[i]);
		return useChangeColor().rgbToHex(rgb[0], rgb[1], rgb[2]);
	};
	return {
		hexToRgb,
		rgbToHex,
		getDarkColor,
		getLightColor,
	};
}
