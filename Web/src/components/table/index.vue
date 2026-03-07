<template>
	<div class="table-container">
		<div v-if="!hideTool" class="table-header mb8">
			<div>
				<slot name="command"></slot>
			</div>
			<div v-loading="state.exportLoading" class="table-footer-tool">
                <el-dropdown v-if="!config.hideExport" split-button trigger="click" @click="onExportTable">
                    <!-- <el-button icon="ele-Download"  /> -->
                     Export
                    <template #dropdown>
                        <el-dropdown-menu>
                            <el-dropdown-item @click="onExportTable">Export data on this page</el-dropdown-item>
                            <el-dropdown-item @click="onExportTableAll">Export all data</el-dropdown-item>
                        </el-dropdown-menu>
                    </template>
                </el-dropdown>

                <el-button-group>
                    <el-button icon="ele-Refresh" v-if="!config.hideRefresh" title="Refresh" @click="() => onRefreshTable()" />
                    <el-button icon="ele-Switch" v-if="state.haveFixed" :title="state.switchFixedContent" :color="state.fixedIconColor" @click="switchFixed" />
                    <el-button icon="ele-Printer" v-if="!config.hidePrint" title="Print" @click="onPrintTable"  />
                    <el-popover v-if="!config.hideSet" placement="bottom-end" trigger="click" transition="el-zoom-in-top" popper-class="table-tool-popper" :width="200" :persistent="false" @show="onSetTable">
                        <template #reference>
                            <el-button icon="ele-Setting"  />
                        </template>
                        <template #default>
                            <div class="tool-box">
                                <el-checkbox v-model="state.checkListAll" :indeterminate="state.checkListIndeterminate" class="ml10 mr1" label="Column Display" @change="onCheckAllChange" />
                                <el-checkbox v-model="getConfig.isSerialNo" class="ml12 mr1" label="No" />
                                <el-checkbox v-if="getConfig.showSelection" v-model="getConfig.isSelection" class="ml12 mr1" label="Multiple Choice" />
                                <el-tooltip content="Drag to sort" placement="top-start">
                                    <SvgIcon style="position: absolute; right: 15px; line-height: 32px;" name="fa fa-question-circle-o" :size="17" class="ml11 cursor-pointer" color="#909399" />
                                </el-tooltip>
                            </div>
                            <el-scrollbar>
                                <div ref="toolSetRef" class="tool-sortable">
                                    <div class="tool-sortable-item" v-for="v in columns" :key="v.prop" :data-key="v.prop" :data-fixed="v.hideCheck ?? false">
                                        <i class="fa fa-arrows-alt handle"></i>
                                        <el-checkbox v-model="v.isCheck" size="default" class="ml12 mr8" :label="v.label" @change="onCheckChange" />
                                    </div>
                                </div>
                            </el-scrollbar>
                        </template>
                    </el-popover>
                </el-button-group>
			</div>
		</div>
		<el-table
            ref="tableRef"
            :data="state.data"
            :border="setBorder"
            :stripe="setStripe"
            v-bind="$attrs"
            row-key="id"
            default-expand-all
            style="width: 100%"
            v-loading="state.loading"
            :default-sort="defaultSort"
            @selection-change="onSelectionChange"
            @sort-change="sortChange"
		>
			<el-table-column type="selection" :reserve-selection="true" :width="30" v-if="config.isSelection && config.showSelection" />
			<el-table-column type="index" :fixed="state.currentFixed && state.serialNoFixed" label="No" align="center" :width="60" v-if="config.isSerialNo" />
			<el-table-column v-for="(item, index) in setHeader" :key="index" v-bind="item">
				<template #header v-if="!item.children && $slots[item.prop]">
					<slot :name="`${item.prop}header`" />
				</template>
				<!-- Custom column slot, the slot name is the prop of the columns attribute -->
				<template #default="scope" v-if="!item.children && $slots[item.prop]">
					<formatter v-if="item.formatter" :fn="item.formatter(scope.row, scope.column, scope.cellValue, scope.index)"> </formatter>
					<slot v-else :name="item.prop" v-bind="scope"></slot>
				</template>
				<template v-else-if="!item.children" v-slot="scope">
					<formatter v-if="item.formatter" :fn="item.formatter(scope.row, scope.column, scope.cellValue, scope.index)"> </formatter>
					<template v-else-if="item.type === 'image'">
						<el-image
						    :style="{ width: `${item.width}px`, height: `${item.height}px` }"
						    :src="scope.row[item.prop]"
						    :zoom-rate="1.2"
						    :preview-src-list="[scope.row[item.prop]]"
						    preview-teleported
						    fit="cover"
						/>
					</template>
					<template v-else>
						{{ getProperty(scope.row, item.prop) }}
					</template>
				</template>
				<el-table-column v-for="(childrenItem, childrenIndex) in item.children" :key="childrenIndex" v-bind="childrenItem">
					<!-- Custom column slot, the slot name is the prop of the columns attribute -->
					<template #default="scope" v-if="$slots[childrenItem.prop]">
						<formatter v-if="childrenItem.formatter" :fn="childrenItem.formatter(scope.row, scope.column, scope.cellValue, scope.index)"> </formatter>
						<slot v-else :name="childrenItem.prop" v-bind="scope"></slot>
					</template>
					<template v-else v-slot="scope">
						<formatter v-if="childrenItem.formatter" :fn="childrenItem.formatter(scope.row, scope.column, scope.cellValue, scope.index)"> </formatter>
						<template v-else-if="childrenItem.type === 'image'">
							<el-image
							    :style="{ width: `${childrenItem.width}px`, height: `${childrenItem.height}px` }"
							    :src="scope.row[childrenItem.prop]"
							    :zoom-rate="1.2"
							    :preview-src-list="[scope.row[childrenItem.prop]]"
							    preview-teleported
							    fit="cover"
							/>
						</template>
						<template v-else>
							{{ getProperty(scope.row, childrenItem.prop) }}
						</template>
					</template>
				</el-table-column>
			</el-table-column>
			<!-- <template #empty>
				<el-empty description="No data available" />
			</template> -->
		</el-table>
		<div v-if="!config.hidePagination && state.showPagination" class="table-footer mt2">
			<el-pagination 
                v-model:current-page="state.page.page"
                v-model:page-size="state.page.pageSize"
                size="small"
                :pager-count="5"
                :page-sizes="config.pageSizes"
                :total="state.total"
                layout="total, sizes, prev, pager, next, jumper"
                background
                @size-change="onHandleSizeChange"
                @current-change="onHandleCurrentChange"
            >
            </el-pagination>
		</div>
	</div>
</template>

<script setup lang="ts" name="netxTable">
import { reactive, computed, nextTick, ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import Sortable from 'sortablejs';
import { storeToRefs } from 'pinia';
import printJs from 'print-js';
//import { EmptyObjectType } from "/@/types/global";
import formatter from '/@/components/table/formatter.vue';
import { useThemeConfig } from '/@/stores/themeConfig';
import { exportExcel } from '/@/utils/exportExcel';  // TODO: This package will cause the browser console to report Module "stream" has been externalized for browser compatibility. Cannot access "stream.Readable" in client code. warning, it is recommended to replace it.

// Define the value passed by the parent component
const props = defineProps({
	// Method to obtain data, passed by the parent component
	getData: {
		type: Function,
		required: true,
	},
	// The column attribute is the same as the Table-column attribute of elementUI. Additional attributes: isCheck - whether to check and display by default, hideCheck - whether to hide the checkable and draggable properties of the column.
	columns: {
		type: Array<any>,
		default: () => [],
	},
	// Configuration items: isBorder - whether to display the table border, isSerialNo - whether to display the table No, showSelection - whether to display the table with multiple selections, isSelection - whether the table multi-selection is selected by default, pageSize - the number of items per page, hideExport - whether to hide the export button, exportFileName - the file name of the exported table, and the empty value defaults to the application name as the file name.
	config: {
		type: Object,
		default: () => ({}),
	},
	// Filter parameters
	param: {
		type: Object,
		default: () => ({}),
	},
	// Default sorting method, {prop:"sort field",order:"ascending or descending"}
	defaultSort: {
		type: Object,
		default: () => ({}),
	},
	// Export report custom data conversion method, do not export by field value
	exportChangeData: {
		type: Function,
	},
	// Print title
	printName: {
		type: String,
		default: () => '',
	},
});

// Define the child component to pass values/events to the parent component, pageChange - page turning event, selectionChange - table multi-selection event, which can handle functions such as batch deletion/modification in the parent component, sortHeader - drag and drop column order event
const emit = defineEmits(['pageChange', 'selectionChange', 'sortHeader']);

// Define variable content
const toolSetRef = ref();
const tableRef = ref();
const storesThemeConfig = useThemeConfig();
const { themeConfig } = storeToRefs(storesThemeConfig);
const state = reactive({
	data: [] as Array<EmptyObjectType>,
	loading: false,
	exportLoading: false,
	total: 0,
	page: {
		page: 1,
		pageSize: 50,
		field: '',
		order: '',
	},
	showPagination: true,
	selectlist: [] as EmptyObjectType[],
	checkListAll: true,
	checkListIndeterminate: false,
	oldColumns: [] as EmptyObjectType[],
	columns: [] as EmptyObjectType[],
	haveFixed: false,
	currentFixed: false,
	serialNoFixed: false,
	switchFixedContent: 'Unfreeze columns',
	fixedIconColor: themeConfig.value.primary,
});

const hideTool = computed(() => {
	return props.config.hideTool ?? false;
});

const getProperty = (obj: any, property: any) => {
	const keys = property.split('.');
	let value = obj;
	for (const key of keys) {
		value = value[key];
	}
	return value;
};

// Set border display/hide
const setBorder = computed(() => {
	return props.config.isBorder ? true : false;
});
// Set zebra pattern show/hide
const setStripe = computed(() => {
	return props.config.isStripe ? true : false;
});
// Get parent component configuration items (required)
const getConfig = computed(() => {
	return props.config;
});
// Set tool header data
const setHeader = computed(() => {
	return state.columns.filter((v) => v.isCheck);
});
// When tool column display select all changes
const onCheckAllChange = <T,>(val: T) => {
	if (val) state.columns.forEach((v) => (v.isCheck = true));
	else state.columns.forEach((v) => (v.isCheck = false));
	state.checkListIndeterminate = false;
};
// The tool column displays when the current item changes
const onCheckChange = () => {
	const headers = state.columns.filter((v) => v.isCheck).length;
	state.checkListAll = headers === state.columns.length;
	state.checkListIndeterminate = headers > 0 && headers < state.columns.length;
};
// When the table multi-select changes
const onSelectionChange = (val: EmptyObjectType[]) => {
	state.selectlist = val;
	emit('selectionChange', state.selectlist);
};
// Pagination changes
const onHandleSizeChange = (val: number) => {
	state.page.pageSize = val;
	onRefreshTable();
	emit('pageChange', state.page);
};
// Change current page
const onHandleCurrentChange = (val: number) => {
	state.page.page = val;
	onRefreshTable();
	emit('pageChange', state.page);
};
// Column sort
const sortChange = (column: any) => {
	state.page.field = column.prop;
	state.page.order = column.order;
	onRefreshTable();
};
// reset list
const pageReset = () => {
	tableRef.value.clearSelection();
	state.page.page = 1;
	onRefreshTable();
};
// Export current page
const onExportTable = () => {
	if (setHeader.value.length <= 0) return ElMessage.error('No columns selected for export');
	exportData(state.data);
};
// Export all
const onExportTableAll = async () => {
	if (setHeader.value.length <= 0) return ElMessage.error('No columns selected for export');
	state.exportLoading = true;
	const param = Object.assign({}, props.param, { page: 1, pageSize: 9999999 });
	const res = await props.getData(param);
	state.exportLoading = false;
	const data = res.result?.items ?? [];
	exportData(data);
};
// Export method
const exportData = (data: Array<EmptyObjectType>) => {
	if (data.length <= 0) return ElMessage.error('No data available for export');
	state.exportLoading = true;
	let exportData = JSON.parse(JSON.stringify(data));
	if (props.exportChangeData) {
		exportData = props.exportChangeData(exportData);
	}
	exportExcel(
			exportData,
			`${props.config.exportFileName ? props.config.exportFileName : themeConfig.value.globalTitle}_${new Date().toLocaleString()}`,
			setHeader.value.filter((item) => {
				return item.type != 'action';
			}),
			'Export Data'
	);
	state.exportLoading = false;
};
// Print
const onPrintTable = () => {
    let printDiv = document.createElement('div');
    let printTitle = document.createElement('div');
    let printTable = document.createElement('table');
    let printTableHeader = document.createElement('thead');
    let printTableBody = document.createElement('tbody');

    // Build header
    setHeader.value.forEach((col: EmptyObjectType) => {
        if (col.prop === 'action' || !col.isCheck) {
            return;
        }
        let th = document.createElement('th');
            th.innerText = col.label;
            th.classList.add('print-table-th');
            th.style.width = col.width ? col.width + 'px' : 'auto';
            th.style.minWidth = col.minWidth ? col.minWidth + 'px' : 'auto';
            printTableHeader.appendChild(th);
    });

    // Build table body
    state.data.forEach((row: EmptyObjectType) => {
        let tr = document.createElement('tr');
        tr.classList.add('print-table-tr');
        setHeader.value.forEach((col: EmptyObjectType) => {
            if (col.prop === 'action' || !col.isCheck) {
                return;
            }

            let td = document.createElement('td');
            td.classList.add('print-table-td');
            if (col.type === 'image') {
                let img = document.createElement('img');
                img.classList.add('print-table-img');

                // img.src = row[col.prop];
                // img.style.width = col.width + 'px';
                // img.style.height = col.height + 'px';
                td.appendChild(img);
                tr.appendChild(td);
            } else {
                td.innerText = getProperty(row, col.prop);
                td.style.textAlign = col.align ? col.align : 'left';
                tr.appendChild(td);
            }
        });
        printTableBody.appendChild(tr);
    });

    printTable.appendChild(printTableHeader);
    printTable.appendChild(printTableBody);
    printTable.classList.add('print-table');

    // Build print header
    if(props.config.printName) {
        printTitle.classList.add('print-table-title');
        printTitle.innerText = props.config.printName;
        printDiv.appendChild(printTitle);
    } else {
        printTitle.style.display = 'none';
    }

    printDiv.appendChild(printTable);
    
    printJs({
        printable: printDiv,
        type: 'html',
        //header: props.config.printName,
        scanStyles: false,
        css: ['/@/theme/media/printTable.css'],
    })

    printDiv.remove()
};

// Drag and drop settings
const onSetTable = () => {
	nextTick(() => {
		const sortable = Sortable.create(toolSetRef.value, {
			//handle: '.handle',
			dataIdAttr: 'data-key',
			animation: 150,
            onStart: () => {
                // In order to distinguish fixed columns from non-fixed columns, add a background color to non-fixed columns when dragging
                toolSetRef.value.children.forEach((element: HTMLElement) => {
                    if(element.dataset.fixed === 'false') {
                        element.classList.add('tool-sortable-item-draggable');
                    }
                });
            },
            onMove: (evt) => {
                // Disable fixed column dragging
                const srcItem = evt.dragged.dataset.fixed; // Drag item
                const targetItem = evt.related.dataset.fixed; // Target position (disable dragging to fixed column position)
                if(srcItem === 'true' || targetItem === 'true') {
                    return false;
                }
            },
			onEnd: () => {
				const headerList: EmptyObjectType[] = [];
				sortable.toArray().forEach((val: any) => {
					state.columns.forEach((v) => {
						if (v.prop === val) headerList.push({ ...v });
					});
				});
				//emit('sortHeader', headerList);
                state.columns = headerList;

                // Clear the background color added when dragging
                toolSetRef.value.children.forEach((element: HTMLElement) => {
                    element.classList.remove('tool-sortable-item-draggable');
                });
			}
		});
	});
};

const onRefreshTable = async () => {
	state.loading = true;
	let param = Object.assign({}, props.param, { ...state.page });
	Object.keys(param).forEach((key) => param[key] === undefined && delete param[key]);
	const res = await props.getData(param);
	state.loading = false;
	if (res && res.result && res.result.items) {
		state.showPagination = true;
		state.data = res.result?.items ?? [];
		state.total = res.result?.total ?? 0;
	} else {
		state.showPagination = false;
		state.data = res && res.result ? res.result : [];
	}
};

const toggleSelection = (row: any, statu?: boolean) => {
	tableRef.value!.toggleRowSelection(row, statu);
};

const getTableData = () => {
	return state.data;
};

const setTableData = (data: Array<EmptyObjectType>, add: boolean = false) => {
	if (add) {
		// Append, remove duplicates
		var repeat = false;
		for (let newItem of data) {
			repeat = false;
			for (let item of state.data) {
				if (newItem.id === item.id) {
					repeat = true;
					break;
				}
			}
			if (!repeat) {
				state.data.push(newItem);
			}
		}
	} else {
		state.data = data;
	}
};

const clearFixed = () => {
	for (let item of state.columns) delete item['fixed'];
};

const switchFixed = () => {
	state.currentFixed = !state.currentFixed;
	state.switchFixedContent = state.currentFixed ? 'Unfreeze columns' : 'enablefixedcolumn';
	if (state.currentFixed) {
		state.fixedIconColor = themeConfig.value.primary;
		state.columns = JSON.parse(JSON.stringify(state.oldColumns));
	} else {
		state.fixedIconColor = '';
		clearFixed();
	}
};

const refreshColumns = () => {
	state.oldColumns = JSON.parse(JSON.stringify(props.columns));
	state.columns = props.columns;
	for (let item of state.columns) {
		if (item.fixed !== undefined) {
			state.haveFixed = true;
			state.currentFixed = true;
			if (item.fixed == 'left') {
				state.serialNoFixed = true;
				break;
			}
		}
	}
};

onMounted(() => {
	if (props.defaultSort) {
		state.page.field = props.defaultSort.prop;
		state.page.order = props.defaultSort.order;
	}
	state.page.pageSize = props.config.pageSize ?? 10;
	refreshColumns();
	onRefreshTable();
});

const handleList = onRefreshTable;

// exposure variables
defineExpose({
	pageReset,
	handleList,
	toggleSelection,
	getTableData,
	setTableData,
	refreshColumns,
});
</script>

<style scoped lang="scss">
@import url('/@/theme/tableTool.scss');
.table-container {
	display: flex !important;
	flex-direction: column;
	height: 100%;

	.el-table {
		flex: 1;
	}

	.table-footer {
		display: flex;
		justify-content: flex-end;
	}

	.table-header {
		display: flex;

		.table-footer-tool {
			flex: 1;
			display: flex;
			align-items: center;
			justify-content: flex-end;
            gap: 8px;
		}
	}
}
</style>