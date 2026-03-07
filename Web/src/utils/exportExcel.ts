import XLSXS from 'xlsx-js-style';
/**
* @description:
* @param {Object} json Sent from the serverData
* @param {String} name ExportExcelFile name

* @param {String} titleArr ExportExcelTable Header

* @param {String} sheetName ExportsheetNameName
* @return:
**/
export function exportExcel(jsonarr: Array<EmptyObjectType>, name: string, header: Array<EmptyObjectType>, sheetName: string) {
	var data = new Array();
	var wpxArr = new Array(); // column width
	const borderStyle = {
		// border style
		top: {
			style: 'thin',
			color: {
				rgb: '000000',
			},
		},
		bottom: {
			style: 'thin',
			color: {
				rgb: '000000',
			},
		},
		left: {
			style: 'thin',
			color: {
				rgb: '000000',
			},
		},
		right: {
			style: 'thin',
			color: {
				rgb: '000000',
			},
		},
	};
	let headerDepth = getMaxDepth(header);
	let headerColumns = getTotalColumns(header);

	// Create a header two-dimensional array
	let headerArr = new Array(headerDepth);
	for (let i = 0; i < headerArr.length; i++) {
		headerArr[i] = new Array(headerColumns);
	}
	// Compute column index
	let colIndex = 0;
	for (let i = 0; i < header.length; i++) {
		let col = header[i];

		// Get the corresponding length of the column
		let colNum = getTotalColumns([col]);
		let colDepth = getMaxDepth([col]);
		for (let y = 0; y < colNum; y++) {
			colIndex = colIndex + y;
			for (let z = 0; z < colDepth; z++) {
				headerRec(z, colIndex, y, col, headerArr);
				// headerArr[z][colIndex]=col;
			}
		}
		colIndex++;
	}

	// Fill the header column with an empty column. The empty column must be consistent with the previous row of this column. Merge cells through consistent cells.
	for (let i = 0; i < headerArr.length; i++) {
		let row = headerArr[i];
		for (let j = 0; j < row.length; j++) {
			if (headerArr[i][j] == null && i > 0) {
				headerArr[i][j] = headerArr[i - 1][j];
			}
		}
	}

	// Recursive header
	function headerRec(rowindex: number, colindex: number, childrenindex: number, col: any, arr: any) {
		if (rowindex > 0) {
			if (col.children) {
				headerRec(rowindex, colIndex, childrenindex, col.children, arr);
			} else {
				arr[rowindex][colindex] = col[childrenindex];
			}
		} else {
			arr[rowindex][colindex] = col;
		}
	}

	for (let i = 0; i < headerArr.length; i++) {
		var headrow = new Array();
		let ha = headerArr[i];
		for (let j = 0; j < ha.length; j++) {
			let item = ha[j];
			let width = 200;
			if (item.width && !isNaN(item.width)) {
				width = parseInt(item.width) * 0.7;
			}
			wpxArr.push({ wpx: width });
			headrow.push({
				v: item.label,
				t: 's',
				s: {
					font: { bold: true },
					alignment: { wrapText: true, horizontal: item.headerAlign ? item.headerAlign : item.align ? item.align : '', vertical: 'center' },
					border: borderStyle,
				},
			});
		}
		data.push(headrow); // write title
	}

	// Calculate merged cell information
	var mergedCells = [];
	var mergedflg = false;
	var mergedcell = { s: { r: 0, c: 0 }, e: { r: 0, c: 0 } };

	// Column merge
	for (let i = 0; i < headerColumns; i++) {
		let rowcol = headerArr[0][i];
		for (let j = 0; j < headerDepth; j++) {
			let col = headerArr[j][i];
			if (col == rowcol && mergedflg == false) {
				mergedflg = true;
				mergedcell.s = { r: j, c: i };
			} else if (col != rowcol && mergedflg == true) {
				mergedcell.e = { r: j - 1, c: i };

				if (mergedcell.s.r != mergedcell.e.r) {
					mergedCells.push(JSON.parse(JSON.stringify(mergedcell)));
					mergedflg = false;
					mergedcell = { s: { r: 0, c: 0 }, e: { r: 0, c: 0 } };
				} else if (j == headerDepth - 1 && mergedflg == true) {
					mergedcell.e = { r: j, c: i };
					if (mergedcell.s.r != mergedcell.e.r && col == rowcol) {
						mergedCells.push(JSON.parse(JSON.stringify(mergedcell)));
					}
					mergedflg = false;
					mergedcell = { s: { r: 0, c: 0 }, e: { r: 0, c: 0 } };
				} else {
					rowcol = col;
					mergedflg = true;
					mergedcell.s = { r: j, c: i };
				}
				// rowcol=col;
				// mergedflg=false;
				// mergedcell={s:{r:0,c:0},e:{r:0,c:0}}
			} else if (j == headerDepth - 1 && mergedflg == true) {
				mergedcell.e = { r: j, c: i };
				if (mergedcell.s.r != mergedcell.e.r && col == rowcol) {
					mergedCells.push(JSON.parse(JSON.stringify(mergedcell)));
				}
				mergedflg = false;
				mergedcell = { s: { r: 0, c: 0 }, e: { r: 0, c: 0 } };
			}
		}
	}

	// row merge
	mergedflg = false;
	for (let i = 0; i < headerDepth; i++) {
		let rowcol = headerArr[i][0];
		for (let j = 0; j < headerColumns; j++) {
			let col = headerArr[i][j];
			if (col == rowcol && mergedflg == false) {
				mergedflg = true;
				mergedcell.s = { r: i, c: j };
			} else if (col != rowcol && mergedflg == true) {
				mergedcell.e = { r: i, c: j - 1 };

				if (mergedcell.s.c != mergedcell.e.c) {
					mergedCells.push(JSON.parse(JSON.stringify(mergedcell)));
					mergedflg = false;
					mergedcell = { s: { r: 0, c: 0 }, e: { r: 0, c: 0 } };
				} else if (j == headerColumns - 1 && mergedflg == true) {
					mergedcell.e = { r: i, c: j };
					if (mergedcell.s.r != mergedcell.e.r && col == rowcol) {
						mergedCells.push(JSON.parse(JSON.stringify(mergedcell)));
					}
					mergedflg = false;
					mergedcell = { s: { r: 0, c: 0 }, e: { r: 0, c: 0 } };
				} else {
					rowcol = col;
					mergedflg = true;
					mergedcell.s = { r: i, c: j };
				}
				// rowcol=col;
				// mergedflg=false;
				// mergedcell={s:{r:0,c:0},e:{r:0,c:0}}
			} else if (j == headerColumns - 1 && mergedflg == true) {
				mergedcell.e = { r: i, c: j };
				if (mergedcell.s.c != mergedcell.e.c && col == rowcol) {
					mergedCells.push(JSON.parse(JSON.stringify(mergedcell)));
				}
				mergedflg = false;
				mergedcell = { s: { r: 0, c: 0 }, e: { r: 0, c: 0 } };
			}
		}
	}

	jsonarr.forEach((json) => {
		var row = new Array();
		headerArr[headerArr.length - 1].forEach((item: any) => {
			if (json.hasOwnProperty(item.prop) || getProperty(json, item.prop)) {
				let val = '';
				if (json[item.prop] != null) {
					if (item.formatter) {
						var itemf = item.formatter(json);
						val = formatterRec(itemf); // Recursively obtain formatter information
					} else {
						val = json[item.prop];
					}
				} else if (getProperty(json, item.prop)) {
					if (item.formatter) {
						var itemf = item.formatter(json);
						val = formatterRec(itemf); // Recursively obtain formatter information
					} else {
						val = getProperty(json, item.prop);
					}
				}
				row.push({
					v: val,
					t: 's',
					s: {
						alignment: { wrapText: true, horizontal: item.align ? item.align : '', vertical: 'center' },
						border: borderStyle,
					},
				});
			}
		});
		data.push(row);
	});
	const ws = XLSXS.utils.aoa_to_sheet(data);
	const wb = XLSXS.utils.book_new();
	ws['!cols'] = wpxArr;
	ws['!merges'] = mergedCells; // Set merged cell information
	XLSXS.utils.book_append_sheet(wb, ws, sheetName);
	/* generate file and send to client */
	XLSXS.writeFile(wb, name + '.xlsx');
}

// recursive formatter
function formatterRec(itemf: any) {
	let r = '';
	if (itemf.children) {
		if (itemf.children.default) {
			r = itemf.children.default();
		} else {
			itemf.children.forEach((element: any) => {
				r = r + formatterRec(element);
			});
		}
	} else {
		r = itemf;
	}
	return r;
}

// get depth
function getMaxDepth(data: any) {
	let maxDepth = 1;
	function traverse(obj: any, depth: any) {
		if (obj.children && obj.children.length > 0) {
			depth++;
			if (depth > maxDepth) {
				maxDepth = depth;
			}
			obj.children.forEach((child: any) => traverse(child, depth));
		}
	}

	data.forEach((obj: any) => traverse(obj, 1));
	return maxDepth;
}

// Get the total number of columns
function getTotalColumns(data: any) {
	let totalColumns = 0;
	function traverse(obj: any) {
		if (obj.children && obj.children.length > 0) {
			obj.children.forEach((child: any) => traverse(child));
		} else {
			totalColumns++;
		}
	}

	data.forEach((obj: any) => traverse(obj));
	return totalColumns;
}

// Get child object
const getProperty = (obj: any, property: any) => {
	const keys = property.split('.');
	let value = obj;
	for (const key of keys) {
		value = value[key];
	}
	return value;
};
