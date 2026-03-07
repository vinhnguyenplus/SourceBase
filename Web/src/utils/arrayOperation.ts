/**
 * Determine two numbersgroupStringYesnoSame（used forbuttonPermission Verification），numbergroupStringinExist the sametimeWill automatically remove duplicates（buttonPermission IdentifierWill not repeat）
 * @param news NewData
 * @param old SourceData
 * @returns Two numbersgroupReturn the same `true`，The opposite applies
 */
export function judgementSameArr(newArr: unknown[] | string[], oldArr: string[]): boolean {
	const news = removeDuplicate(newArr);
	const olds = removeDuplicate(oldArr);
	let count = 0;
	const leng = news.length;
	for (let i in olds) {
		for (let j in news) {
			if (olds[i] === news[j]) count++;
		}
	}
	return count === leng ? true : false;
}

/**
 * Determine two objectsYesnoSame
 * @param a Object to be comparedone
 * @param b Object to be comparedTwo
 * @returns Return the same true，The opposite applies
 */
export function isObjectValueEqual<T extends Record<string, any>>(a: T, b: T): boolean {
	if (!a || !b) return false;
	let aProps = Object.getOwnPropertyNames(a);
	let bProps = Object.getOwnPropertyNames(b);
	if (aProps.length != bProps.length) return false;
	for (let i = 0; i < aProps.length; i++) {
		let propName = aProps[i];
		let propA = a[propName];
		let propB = b[propName];
		if (!b.hasOwnProperty(propName)) return false;
		if (propA instanceof Object) {
			if (!isObjectValueEqual(propA, propB)) return false;
		} else if (propA !== propB) {
			return false;
		}
	}
	return true;
}

/**
 * Original implementation：numbergroup、numbergroupDeduplicate objects
 * @param arr numbergroupcontent
 * @param attr Key that needs to be de-duplicatedvalue（numbergroupObject）
 * @returns
 */
/*
export function removeDuplicate(arr: EmptyArrayType, attr?: string) {
	if (!Object.keys(arr).length) {
		return arr;
	} else {
		if (attr) {
			const obj: EmptyObjectType = {};
			return arr.reduce((cur: EmptyArrayType[], item: EmptyArrayType) => {
				obj[item[attr]] ? '' : (obj[item[attr]] = true && item[attr] && cur.push(item));
				return cur;
			}, []);
		} else {
			return [...new Set(arr)];
		}
	}
}
*/
/**
 * Implemented after optimization：numbergroup、numbergroupDeduplicate objects
 * Supports ordinary numbersgroupNumber of objectsgroupRemove duplicates，TypeSafety，And is compatible with all existing call methods
 * @param arr numbergroupcontent
 * @param attr Key that needs to be de-duplicatedvalue（numbergroupObject）
 * @returns
 */
export function removeDuplicate<T>(arr: T[], attr?: string): T[] {
	if (!arr.length) {
		return arr;
	} else {
		if (attr) {
			const obj: Record<string, boolean> = {};
			return arr.reduce((cur: T[], item: T) => {
				const key = (item as any)[attr];
				if (key && !obj[key]) {
					obj[key] = true;
					cur.push(item);
				}
				return cur;
			}, []);
		} else {
			return [...new Set(arr)];
		}
	}
}

/* Array and object deep copy
 * @param value Needs to be copiedcontent
 * @returns
 */
export const clone = <T>(value: T): T => {
	if (!value) return value;

	// array
	if (Array.isArray(value)) return value.map((item) => clone(item)) as unknown as T;

	// Ordinary objects
	if (typeof value === 'object') {
		return Object.fromEntries(
			Object.entries(value).map(([k, v]: [string, any]) => {
				return [k, clone(v)];
			})
		) as unknown as T;
	}
	// basic type
	return value;
};
