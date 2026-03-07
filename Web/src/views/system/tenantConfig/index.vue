<template>
	<div class="sys-config-container">
		<el-card shadow="hover" :body-style="{ padding: 5 }">
			<TableSearch :search="tb.tableData.search" @search="onSearch" />
		</el-card>
		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<Table ref="tableRef" v-bind="tb.tableData" :getData="getData" :exportChangeData="exportChangeData" @sortHeader="onSortHeader" @selectionChange="tableSelection" border>
				<template #command>
					<el-button type="primary" icon="ele-Plus" @click="openAddConfig" v-auth="'sysTenantConfig:add'"> Add New </el-button>

					<el-button v-if="state.selectlist.length > 0" type="danger" icon="ele-Delete" @click="bacthDelete" v-auth="'sysTenantConfig:batchDelete'"> Batch Delete </el-button>
				</template>
				<template #sysFlag="scope">
					<g-sys-dict v-model="scope.row.sysFlag" code="YesNoEnum" />
				</template>
				<template #remark="scope">
					<ModifyRecord :data="scope.row" />
				</template>
				<template #action="scope">
					<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditConfig(scope.row)" v-auth="'sysTenantConfig:update'"> Edit </el-button>
					<el-button icon="ele-Delete" size="small" text type="danger" @click="delConfig(scope.row)" v-auth="'sysTenantConfig:delete'" :disabled="scope.row.sysFlag === 1"> Delete </el-button>
				</template>
			</Table>
		</el-card>
		<EditConfig ref="editConfigRef" :title="state.editConfigTitle" :groupList="state.groupList" @updateData="updateData" />
	</div>
</template>

<script lang="ts" setup name="sysTenantConfig">
import { onMounted, reactive, ref, defineAsyncComponent, nextTick } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { auth } from '/@/utils/authFunction';
import { getAPI } from '/@/utils/axios-utils';
import { SysTenantConfigApi } from '/@/api-services/api';
import ModifyRecord from '/@/components/table/modifyRecord.vue';
import EditConfig from '/@/views/system/tenantConfig/component/editConfig.vue';
//import { EmptyObjectType, RefType } from '/@/types/global';

// Introduce components
const Table = defineAsyncComponent(() => import('/@/components/table/index.vue'));
const TableSearch = defineAsyncComponent(() => import('/@/components/table/search.vue'));
const editConfigRef = ref<InstanceType<typeof EditConfig>>();
const tableRef = ref<RefType>();

const state = reactive({
	editConfigTitle: '',
	selectlist: [] as EmptyObjectType[],
	groupList: [] as Array<String>,
});

const tb = reactive<TableDemoState>({
	tableData: {
		// Header content (required, pay attention to the format)
		columns: [
			{ prop: 'name', minWidth: 150, label: 'Configuration name', headerAlign: 'center', sortable: 'custom', isCheck: true, hideCheck: true },
			{ prop: 'code', minWidth: 150, label: 'Configuration Encoding', headerAlign: 'center', toolTip: true, sortable: 'custom', isCheck: true },
			{ prop: 'value', minWidth: 150, label: 'attribute value', headerAlign: 'center', isCheck: true },
			{ prop: 'sysFlag', width: 100, label: 'built-in parameters', align: 'center', isCheck: true },
			{ prop: 'groupCode', width: 120, label: 'GroupEncoding', align: 'center', sortable: 'custom', isCheck: true },
			{ prop: 'orderNo', width: 80, label: 'Sort', align: 'center', sortable: 'custom', isCheck: true },
			{ prop: 'remark', width: 100, label: 'Modify records', align: 'center', headerAlign: 'center', showOverflowTooltip: true, isCheck: true },
			{ prop: 'action', width: 140, label: 'Operation', type: 'action', align: 'center', isCheck: true, fixed: 'right', hideCheck: true },
		],
		// Configuration items (required)
		config: {
			isStripe: true, // Whether to display table zebra pattern
			isBorder: false, // Whether to display table borders
			isSerialNo: true, // Whether to display the table No
			isSelection: true, // Whether to check multiple selections in the form
			showSelection: auth('sysTenantConfig:batchDelete'), // Whether to display table multi-select
			pageSize: 50, // Number of items per page
			hideExport: false, // Whether to hide the export button
			exportFileName: 'System Parameters', // The file name of the exported report. If not filled in, take the application name.
		},
		// Search form, dynamically generated (when an empty array is passed, the search will not be displayed, there are 3 types of type: input, date, select)
		search: [
			{ label: 'Configuration name', prop: 'name', placeholder: 'Search configuration name', required: false, type: 'input' },
			{ label: 'Configuration Encoding', prop: 'code', placeholder: 'Search configuration code', required: false, type: 'input' },
			// { label: 'Creation time', prop: 'time', placeholder: 'Please select', required: false, type: 'date' },
		],
		param: {},
		defaultSort: {
			prop: 'orderNo',
			order: 'ascending',
		},
	},
});
const getData = (param: any) => {
	return getAPI(SysTenantConfigApi)
		.apiSysTenantConfigPagePost(param)
		.then((res) => {
			return res.data;
		});
};
const exportChangeData = (data: Array<EmptyObjectType>) => {
	data.forEach((item) => {
		item.sysFlag = item.sysFlag == 1 ? 'Yes' : 'no';
	});
	return data;
};
// Drag display column sort callback
const onSortHeader = (data: object[]) => {
	tb.tableData.columns = data;
};
// Form callback when search is clicked
const onSearch = (data: EmptyObjectType) => {
	tb.tableData.param = Object.assign({}, tb.tableData.param, { ...data });
	nextTick(() => {
		tableRef.value.pageReset();
	});
};

const getGroupList = async () => {
	const res = await getAPI(SysTenantConfigApi).apiSysTenantConfigGroupListGet();
	const groupSearch = {
		label: 'GroupEncoding',
		prop: 'groupCode',
		placeholder: 'Please select',
		required: false,
		type: 'select',
		options: [],
	} as TableSearchType;
	state.groupList = res.data.result ?? [];
	res.data.result?.forEach((item) => {
		if(item) groupSearch.options?.push({ label: item, value: item });
	});
	let group = tb.tableData.search.filter((item) => {
		return item.prop == 'groupCode';
	});
	if (group.length == 0) {
		tb.tableData.search.push(groupSearch);
	} else {
		group[0] = groupSearch;
	}
};

//Form multiple selection event
const tableSelection = (data: EmptyObjectType[]) => {
	state.selectlist = data;
};

onMounted(async () => {
	getGroupList();
});

// Update data
const updateData = () => {
	tableRef.value.handleList();
	getGroupList();
};

// Open new page
const openAddConfig = () => {
	state.editConfigTitle = 'Add configuration';
	editConfigRef.value?.openDialog({ sysFlag: 2, orderNo: 100 });
};

// Open the edit page
const openEditConfig = (row: any) => {
	state.editConfigTitle = 'Edit Configuration';
	editConfigRef.value?.openDialog(row);
};

// delete
const delConfig = (row: any) => {
	ElMessageBox.confirm(`Confirm to delete configuration: [${row.name}]?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysTenantConfigApi).apiSysTenantConfigDeletePost({ id: row.id });
			tableRef.value.handleList();
			ElMessage.success('Deleted successfully');
		})
		.catch(() => {});
};

//Batch delete
const bacthDelete = () => {
	if (state.selectlist.length == 0) return false;
	ElMessageBox.confirm(`Are you sure to delete [${state.selectlist[0].name}] and other ${state.selectlist.length} configurations in batches?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	})
		.then(async () => {
			const ids = state.selectlist.map((item) => {
				return item.id;
			});
			var res = await getAPI(SysTenantConfigApi).apiSysTenantConfigBatchDeletePost(ids);
			tableRef.value.pageReset();
			ElMessage.success('Deleted successfully');
		})
		.catch(() => {});
};
</script>
