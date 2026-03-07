import { nextTick, defineAsyncComponent } from 'vue';
import type { App } from 'vue';
import * as svg from '@element-plus/icons-vue';
import router from '/@/router/index';
import pinia from '/@/stores/index';
import { storeToRefs } from 'pinia';
import { useThemeConfig } from '/@/stores/themeConfig';
//import { i18n } from '/@/i18n/index';
import { Local } from '/@/utils/storage';
import { verifyUrl } from '/@/utils/toolsValidate';

// Introduce components
const SvgIcon = defineAsyncComponent(() => import('/@/components/svgIcon/index.vue'));

/**
 * ExportGlobal Registration element plus svg icon
 * @param app vue Example
 * @description Use：https://element-plus.gitee.io/zh-CN/component/icon.html
 */
export function elSvg(app: App) {
	const icons = svg as any;
	for (const i in icons) {
		app.component(`ele-${icons[i].name}`, icons[i]);
	}
	app.component('SvgIcon', SvgIcon);
}

/**
 * SettingsBrowsertitleInternationalization
 * @method const title = useTitle(); ==> title()
 */
export function useTitle() {
	const stores = useThemeConfig(pinia);
	const { themeConfig } = storeToRefs(stores);
	nextTick(() => {
		let webTitle = '';
		let globalTitle: string = themeConfig.value.globalTitle;
		const { path, meta } = router.currentRoute.value;
		if (path === '/login') {
			webTitle = <string>meta.title;
		} else {
			webTitle = setTagsViewNameI18n(router.currentRoute.value);
		}
		document.title = `${webTitle} - ${globalTitle}` || globalTitle;
	});
}

/**
 * Settings Customize tagsView name、 Customize tagsView nameInternationalization
 * @param params Router query、params inof tagsViewName
 * @returns Return current tagsViewName name
 */
export function setTagsViewNameI18n(item: any) {
	let tagsViewName: string = '';
	const { query, params, meta } = item;
	// Fix tagsViewName to match other routes containing the following words
	const pattern = /^\{("(zh-cn|en|zh-tw)":"[^,]+",?){1,3}}$/;
	if (query?.tagsViewName || params?.tagsViewName) {
		tagsViewName = query?.tagsViewName || params?.tagsViewName;
	} else {
		// Non-custom tagsView name
		tagsViewName = meta.title;
	}
	return tagsViewName;
}

/**
 * Image lazy loading
 * @param el dom GoalYuanplain
 * @param arr ListData
 * @description data-xxx AttributeUsed to store private pages or applicationscustom data
 */
export const lazyImg = (el: string, arr: EmptyArrayType) => {
	const io = new IntersectionObserver((res) => {
		res.forEach((v: any) => {
			if (v.isIntersecting) {
				const { img, key } = v.target.dataset;
				v.target.src = img;
				v.target.onload = () => {
					io.unobserve(v.target);
					arr[key]['loading'] = false;
				};
			}
		});
	});
	nextTick(() => {
		document.querySelectorAll(el).forEach((img) => io.observe(img));
	});
};

/**
 * GlobalComponent size
 * @returns Return `window.localStorage` inReadcachevalue `globalComponentSize`
 */
export const globalComponentSize = (): string => {
	const stores = useThemeConfig(pinia);
	const { themeConfig } = storeToRefs(stores);
	return themeConfig.value.globalComponentSize;
};

/**
 * Deep cloning of objects
 * @param obj Source Object
 * @returns Cloned object
 */
export function deepClone(obj: EmptyObjectType) {
	let newObj: EmptyObjectType;
	try {
		newObj = obj.push ? [] : {};
	} catch (error) {
		newObj = {};
	}
	for (let attr in obj) {
		if (obj[attr] && typeof obj[attr] === 'object') {
			newObj[attr] = deepClone(obj[attr]);
		} else {
			newObj[attr] = obj[attr];
		}
	}
	return newObj;
}

/**
 * JudgmentYesnoYesMobile
 */
export function isMobile() {
	if (navigator.userAgent.match(/('phone|pad|pod|iPhone|iPod|ios|iPad|Android|Mobile|BlackBerry|IEMobile|MQQBrowser|JUC|Fennec|wOSBrowser|BrowserNG|WebOS|Symbian|Windows Phone')/i)) {
		return true;
	} else {
		return false;
	}
}

/**
 * Judgment numbergroupObjectinAllAttributeYesnofornull，fornullthenDeleteCurrent row object
 * @description @Thanks, Big Huang
 * @param list numbergroupObject
 * @returns Deletenullvaluethe number aftergroupObject
 */
export function handleEmpty(list: EmptyArrayType) {
	const arr = [];
	for (const i in list) {
		const d = [];
		for (const j in list[i]) {
			d.push(list[i][j]);
		}
		const leng = d.filter((item) => item === '').length;
		if (leng !== d.length) {
			arr.push(list[i]);
		}
	}
	return arr;
}

/**
 * OpenExternal links
 * @param val Current clicked itemmenu
 */
export function handleOpenLink(val: RouteItem) {
	const { origin, pathname } = window.location;
	router.push(val.path);
	if (verifyUrl(<string>val.meta?.isLink)) window.open(val.meta?.isLink);
	else window.open(`${origin}${pathname}#${val.meta?.isLink}`);
}

/**
 * uniteoneBatchExport
 * @method elSvg ExportGlobal Registration element plus svg icon
 * @method useTitle SettingsBrowsertitleInternationalization
 * @method setTagsViewNameI18n Settings Customize tagsView name、 Customize tagsView nameInternationalization
 * @method lazyImg Image lazy loading
 * @method globalComponentSize() element plus GlobalComponent size
 * @method deepClone Deep cloning of objects
 * @method isMobile JudgmentYesnoYesMobile
 * @method handleEmpty Judgment numbergroupObjectinAllAttributeYesnofornull，fornullthenDeleteCurrent row object
 * @method handleOpenLink OpenExternal links
 */
const other = {
	elSvg: (app: App) => {
		elSvg(app);
	},
	useTitle: () => {
		useTitle();
	},
	setTagsViewNameI18n(route: RouteToFrom) {
		return setTagsViewNameI18n(route);
	},
	lazyImg: (el: string, arr: EmptyArrayType) => {
		lazyImg(el, arr);
	},
	globalComponentSize: () => {
		return globalComponentSize();
	},
	deepClone: (obj: EmptyObjectType) => {
		return deepClone(obj);
	},
	isMobile: () => {
		return isMobile();
	},
	handleEmpty: (list: EmptyArrayType) => {
		return handleEmpty(list);
	},
	handleOpenLink: (val: RouteItem) => {
		handleOpenLink(val);
	},
};

// Unified batch export
export default other;
