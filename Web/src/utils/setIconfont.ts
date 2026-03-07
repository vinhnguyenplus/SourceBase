// font icon url
const cssCdnUrlList: Array<string> = [
	// Adjust to import from local, comment the url below
	// '//at.alicdn.com/t/c/font_2298093_rnp72ifj3ba.css',
	// '//netdna.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css',
];
// Third party js url
const jsCdnUrlList: Array<string> = [];

// Dynamically set font icons in batches
export function setCssCdn() {
	if (cssCdnUrlList.length <= 0) return false;
	cssCdnUrlList.map((v) => {
		let link = document.createElement('link');
		link.rel = 'stylesheet';
		link.href = v;
		link.crossOrigin = 'anonymous';
		document.getElementsByTagName('head')[0].appendChild(link);
	});
}

// Dynamically set third-party js in batches
export function setJsCdn() {
	if (jsCdnUrlList.length <= 0) return false;
	jsCdnUrlList.map((v) => {
		let link = document.createElement('script');
		link.src = v;
		document.body.appendChild(link);
	});
}

/**
 * Batch set fonticon、Dynamicjs
 * @method cssCdn Dynamically batch set fontsicon
 * @method jsCdn Dynamic batch settingsNumberThreesquarejs
 */
const setIntroduction = {
	// set css
	cssCdn: () => {
		setCssCdn();
	},
	// set js
	jsCdn: () => {
		setJsCdn();
	},
};

// Export function method
export default setIntroduction;
