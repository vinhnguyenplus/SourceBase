/* eslint-disable */
import { hiprint } from 'vue-plugin-hiprint';
import logoImg from '/@/assets/logo.png';

// Custom design elements 1
export const aProvider = function () {
	var addElementTypes = function (context: any) {
		context.removePrintElementTypes('aProviderModule');
		context.addPrintElementTypes('aProviderModule', [
			new hiprint.PrintElementTypeGroup('【Public components】', [
				{
					tid: 'aProviderModule.barcode',
					title: 'barcode',
					data: '18012345678',
					type: 'text',
					options: {
						field: 'barCode',
						testData: 'Z18012345678',
						height: 32,
						fontSize: 12,
						lineHeight: 18,
						fontWeight: '700',
						textAlign: 'left',
						textContentVerticalAlign: 'middle',
						textType: 'barcode',
					},
				},
				{
					tid: 'aProviderModule.qrcode',
					title: 'QR code',
					data: 'Z18012345678',
					type: 'text',
					options: {
						field: 'qrCode',
						testData: 'Z18012345678',
						height: 64,
						width: 64,
						fontSize: 12,
						lineHeight: 18,
						fontWeight: '700',
						textAlign: 'left',
						textContentVerticalAlign: 'middle',
						textType: 'qrcode',
					},
				},
				{
					tid: 'aProviderModule.table',
					title: 'table',
					type: 'table',
					options: {
						field: 'table',
						tableHeaderRepeat: 'first',
						tableFooterRepeat: 'last',
						fields: [
							{ text: 'name', field: 'NAME' },
							{ text: 'quantity', field: 'SL' },
							{ text: 'Specification', field: 'GG' },
							{ text: 'barcode', field: 'TM' },
							{ text: 'Unit price', field: 'DJ' },
							{ text: 'Amount', field: 'JE' },
						],
					},
					editable: true,
					columnDisplayEditable: true, // Column shows whether it can be edited
					columnDisplayIndexEditable: true, // Column order shows whether it can be edited
					columnTitleEditable: true, // Whether column titles can be edited
					columnResizable: true, // Can the column width be adjusted?
					columnAlignEditable: true, // Whether column alignment is adjusted
					isEnableEditField: true, // Edit field
					isEnableContextMenu: true, // Enable right-click menu. Default is true.
					isEnableInsertRow: true, // Insert row
					isEnableDeleteRow: true, // Delete row
					isEnableInsertColumn: true, // insert column
					isEnableDeleteColumn: true, // Delete column
					isEnableMergeCell: true, // Merge cells
					columns: [
						[
							{ title: 'name', align: 'center', field: 'NAME', width: 150 },
							{ title: 'quantity', align: 'center', field: 'SL', width: 80 },
							{ title: 'Specification', align: 'center', field: 'GG', width: 80, checked: false },
							{ title: 'barcode', align: 'center', field: 'TM', width: 100, checked: false },
							{ title: 'Unit price', align: 'center', field: 'DJ', width: 100 },
							{ title: 'Amount', align: 'center', field: 'JE', width: 100, checked: false },
						],
					],
					// footerFormatter: function (options: unknown, rows: unknown, data: any, currentPageGridRowsData: unknown) {
					//   if (data && data['totalCap']) {
					//     return `<td style="padding:0 10px" colspan="100">${'Amount receivable in capital letters: ' + data['totalCap']}</td>`
					//   }
					//   return '<td style="padding:0 10px" colspan="100">Amount receivable in capital letters: </td>'
					// },
				},
				{
					tid: 'aProviderModule.customText',
					title: 'text',
					customText: 'Custom text',
					custom: true,
					type: 'text',
					options: {
						width: 200,
						testData: 'Long text pagination/non-pagination test',
					},
				},
				{
					tid: 'aProviderModule.longText',
					title: 'Long text',
					type: 'longText',
					options: {
						field: 'test.longText',
						width: 200,
						testData: 'Long text pagination/non-pagination test',
					},
				},
				{ tid: 'aProviderModule.logo', title: 'Logo', data: logoImg, type: 'image', options: { field: 'imageUrl' } },
				{ tid: 'aProviderModule.hline', title: 'horizontal line', type: 'hline' },
				{ tid: 'aProviderModule.vline', title: 'Vertical line', type: 'vline' },
				{ tid: 'aProviderModule.rect', title: 'Rectangle', type: 'rect' },
				{ tid: 'aProviderModule.oval', title: 'Ellipse', type: 'oval' },
			]),
			new hiprint.PrintElementTypeGroup('[View Fields]', [
				{
					tid: 'aProviderModule.creater',
					title: 'Tabulator',
					data: 'Admin.NET',
					type: 'text',
					options: {
						field: 'creater',
						testData: 'Admin.NET',
						height: 16,
						fontSize: 6.75,
						fontWeight: '700',
						textAlign: 'left',
						textContentVerticalAlign: 'middle',
					},
				},
				{
					tid: 'aProviderModule.printDate',
					title: 'Print Time',
					data: '2023-07-20 09:00',
					type: 'text',
					options: {
						field: 'printDate',
						testData: '2023-07-20 09:00',
						height: 16,
						fontSize: 6.75,
						fontWeight: '700',
						textAlign: 'left',
						textContentVerticalAlign: 'middle',
					},
				},
				{
					tid: 'aProviderModule.signer',
					title: "Warehouse manager's signature",
					data: 'Admin.NET',
					type: 'text',
					options: {
						field: 'signer',
						testData: 'Admin.NET',
						height: 16,
						fontSize: 6.75,
						fontWeight: '700',
						textAlign: 'left',
						textContentVerticalAlign: 'middle',
					},
				},
				{
					tid: 'aProviderModule.director',
					title: "Manager's signature",
					data: 'Admin.NET',
					type: 'text',
					options: {
						field: 'director',
						testData: 'Admin.NET',
						height: 16,
						fontSize: 6.75,
						fontWeight: '700',
						textAlign: 'left',
						textContentVerticalAlign: 'middle',
					},
				},
			]),
		]);
	};
	return {
		addElementTypes: addElementTypes,
	};
};

// Custom design elements 2
export const bProvider = function () {
	var addElementTypes = function (context: any) {
		context.removePrintElementTypes('bProviderModule');
		context.addPrintElementTypes('bProviderModule', [
			new hiprint.PrintElementTypeGroup('【General】', [
				{
					tid: 'bProviderModule.header',
					title: 'Document Header',
					data: 'Document Header',
					type: 'text',
					options: {
						testData: 'Document Header',
						height: 17,
						fontSize: 16.5,
						fontWeight: '700',
						textAlign: 'center',
						hideTitle: true,
					},
				},
				{
					tid: 'bProviderModule.type',
					title: 'Document type',
					data: 'Document type',
					type: 'text',
					options: {
						testData: 'Document type',
						height: 16,
						fontSize: 15,
						fontWeight: '700',
						textAlign: 'center',
						hideTitle: true,
					},
				},
				{
					tid: 'bProviderModule.order',
					title: 'Order Number',
					data: 'Z18012345678',
					type: 'text',
					options: {
						field: 'orderId',
						testData: 'Z18012345678',
						height: 16,
						fontSize: 6.75,
						fontWeight: '700',
						textAlign: 'left',
						textContentVerticalAlign: 'middle',
					},
				},
				{
					tid: 'bProviderModule.date',
					title: 'business date',
					data: '2023-07-20',
					type: 'text',
					options: {
						field: 'date',
						testData: '2023-07-20',
						height: 16,
						fontSize: 6.75,
						fontWeight: '700',
						textAlign: 'left',
						textContentVerticalAlign: 'middle',
					},
				},
				{
					tid: 'bProviderModule.barcode',
					title: 'barcode',
					data: '18012345678',
					type: 'text',
					options: {
						testData: 'Z18012345678',
						height: 32,
						fontSize: 12,
						lineHeight: 18,
						fontWeight: '700',
						textAlign: 'left',
						textContentVerticalAlign: 'middle',
						textType: 'barcode',
					},
				},
				{
					tid: 'bProviderModule.qrcode',
					title: 'QR code',
					data: 'Z18012345678',
					type: 'text',
					options: {
						testData: 'Z18012345678',
						height: 64,
						width: 64,
						fontSize: 12,
						lineHeight: 18,
						fontWeight: '700',
						textAlign: 'left',
						textContentVerticalAlign: 'middle',
						textType: 'qrcode',
					},
				},
				{
					tid: 'bProviderModule.platform',
					title: 'Platform name',
					data: 'Platform name',
					type: 'text',
					options: {
						testData: 'Platform name',
						height: 17,
						fontSize: 16.5,
						fontWeight: '700',
						textAlign: 'center',
						hideTitle: true,
					},
				},
				{ tid: 'bProviderModule.image', title: 'Logo', data: logoImg, type: 'image' },
			]),
			new hiprint.PrintElementTypeGroup('[Customer]', [
				{
					tid: 'bProviderModule.khname',
					title: 'Customer name',
					data: 'Premium customers',
					type: 'text',
					options: {
						field: 'name',
						testData: 'Premium customers',
						height: 16,
						fontSize: 6.75,
						fontWeight: '700',
						textAlign: 'left',
						textContentVerticalAlign: 'middle',
					},
				},
				{
					tid: 'bProviderModule.tel',
					title: 'Customer phone number',
					data: '18012345678',
					type: 'text',
					options: {
						field: 'tel',
						testData: '18012345678',
						height: 16,
						fontSize: 6.75,
						fontWeight: '700',
						textAlign: 'left',
						textContentVerticalAlign: 'middle',
					},
				},
			]),
			new hiprint.PrintElementTypeGroup('[Table/Other]', [
				{
					tid: 'bProviderModule.table',
					title: 'Order Data',
					type: 'table',
					options: {
						field: 'table',
						fields: [
							{ text: 'name', field: 'NAME' },
							{ text: 'quantity', field: 'SL' },
							{ text: 'Specification', field: 'GG' },
							{ text: 'barcode', field: 'TM' },
							{ text: 'Unit price', field: 'DJ' },
							{ text: 'Amount', field: 'JE' },
							{ text: 'Remarks', field: 'DETAIL' },
						],
					},
					editable: true,
					columnDisplayEditable: true, // Column shows whether it can be edited
					columnDisplayIndexEditable: true, // Column order shows whether it can be edited
					columnTitleEditable: true, // Whether column titles can be edited
					columnResizable: true, // Can the column width be adjusted?
					columnAlignEditable: true, // Whether column alignment is adjusted
					isEnableEditField: true, // Edit field
					isEnableContextMenu: true, // Enable right-click menu. Default is true.
					isEnableInsertRow: true, // Insert row
					isEnableDeleteRow: true, // Delete row
					isEnableInsertColumn: true, // insert column
					isEnableDeleteColumn: true, // Delete column
					isEnableMergeCell: true, // Merge cells
					columns: [
						[
							{ title: 'name', align: 'center', field: 'NAME', width: 100 },
							{ title: 'quantity', align: 'center', field: 'SL', width: 100 },
							{ title: 'barcode', align: 'center', field: 'TM', width: 100 },
							{ title: 'Specification', align: 'center', field: 'GG', width: 100 },
							{ title: 'Unit price', align: 'center', field: 'DJ', width: 100 },
							{ title: 'Amount', align: 'center', field: 'JE', width: 100 },
							{ title: 'Remarks', align: 'center', field: 'DETAIL', width: 100 },
						],
					],
					// footerFormatter: function (options: unknown, rows: unknown, data: any, currentPageGridRowsData: unknown) {
					//   if (data && data['totalCap']) {
					//     return `<td style="padding:0 10px" colspan="100">${'Amount receivable in capital letters: ' + data['totalCap']}</td>`
					//   }
					//   return '<td style="padding:0 10px" colspan="100">Amount receivable in capital letters: </td>'
					// },
				},
				{ tid: 'bProviderModule.customText', title: 'text', customText: 'Custom text', custom: true, type: 'text' },
				{
					tid: 'bProviderModule.longText',
					title: 'Long text',
					type: 'longText',
					options: {
						field: 'test.longText',
						width: 200,
						testData: 'Long text pagination/non-pagination test',
					},
				},
			]),
			new hiprint.PrintElementTypeGroup('[Support]', [
				{
					tid: 'bProviderModule.hline',
					title: 'horizontal line',
					type: 'hline',
				},
				{
					tid: 'bProviderModule.vline',
					title: 'Vertical line',
					type: 'vline',
				},
				{
					tid: 'bProviderModule.rect',
					title: 'Rectangle',
					type: 'rect',
				},
				{
					tid: 'bProviderModule.oval',
					title: 'Ellipse',
					type: 'oval',
				},
				{
					tid: 'bProviderModule.barcode',
					title: 'barcode',
					type: 'barcode',
				},
				{
					tid: 'bProviderModule.qrcode',
					title: 'QR code',
					type: 'qrcode',
				},
			]),
		]);
	};
	return {
		addElementTypes: addElementTypes,
	};
};

// type: 1 supplier 2 dealer
export default [
	{
		name: 'A design',
		value: 'aProviderModule',
		type: 1,
		f: aProvider(),
	},
	{
		name: 'B Design',
		value: 'bProviderModule',
		type: 2,
		f: bProvider(),
	},
];
