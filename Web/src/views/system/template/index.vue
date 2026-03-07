<template>
	<div class="sys-config-container">
		<el-card shadow="hover" :body-style="{ paddingBottom: 5 }">
			<TableSearch :search="tb.tableData.search" @search="onSearch" />
		</el-card>
		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<Table ref="tableRef" v-bind="tb.tableData" :getData="getData" @sortHeader="onSortHeader" border>
				<template #command>
					<el-button type="primary" icon="ele-Plus" @click="openAddTemplate" v-auth="'sysConfig:add'"> Add New </el-button>
				</template>
				<template #type="scope">
					<g-sys-dict v-model="scope.row.type" code="TemplateTypeEnum" />
				</template>
				<template #remark="scope">
					<ModifyRecord :data="scope.row" />
				</template>
				<template #action="scope">
					<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditTemplate(scope.row)" v-auth="'sysConfig:update'"> Edit </el-button>
					<el-button icon="ele-Delete" size="small" text type="danger" @click="delTemplate(scope.row)" v-auth="'sysConfig:delete'" :disabled="scope.row.sysFlag === 1"> Delete </el-button>
				</template>
			</Table>
		</el-card>
		<EditTemplate ref="editTemplateRef" :title="state.editTemplateTitle" :groupList="state.groupList" @updateData="updateData" />
	</div>
</template>

<script lang="ts" setup name="sysConfig">
import { onMounted, reactive, ref, defineAsyncComponent, nextTick } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { getAPI } from '/@/utils/axios-utils';
import { SysTemplateApi } from "/@/api-services";
import ModifyRecord from '/@/components/table/modifyRecord.vue';
import EditTemplate from './component/editTemplate.vue';
import GSysDict from "/@/components/sysDict/sysDict.vue";

// Introduce components
const TableSearch = defineAsyncComponent(() => import('/@/components/table/search.vue'));
const Table = defineAsyncComponent(() => import('/@/components/table/index.vue'));
const editTemplateRef = ref<InstanceType<typeof EditTemplate>>();
const tableRef = ref<RefType>();

const state = reactive({
	editTemplateTitle: '',
	selectList: [] as EmptyObjectType[],
	groupList: [] as Array<String>,
});

const tb = reactive<TableDemoState>({
	tableData: {
		// Header content (required, pay attention to the format)
		columns: [
			{ prop: 'name', minWidth: 150, label: 'Template Name', headerAlign: 'center', sortable: 'custom', isCheck: true, hideCheck: true },
			{ prop: 'code', minWidth: 150, label: 'template encoding', headerAlign: 'center', toolTip: true, sortable: 'custom', isCheck: true },
			{ prop: 'type', width: 120, label: 'Template Type', align: 'center', sortable: 'custom', isCheck: true },
			{ prop: 'groupName', width: 120, label: 'GroupEncoding', align: 'center', sortable: 'custom', isCheck: true },
			{ prop: 'orderNo', width: 80, label: 'Sort', align: 'center', sortable: 'custom', isCheck: true },
			{ prop: 'remark', width: 100, label: 'Modify records', align: 'center', headerAlign: 'center', showOverflowTooltip: true, isCheck: true },
			{ prop: 'action', width: 140, label: 'Operation', type: 'action', align: 'center', isCheck: true, fixed: 'right', hideCheck: true },
		],
		// Configuration items (required)
		config: {
			isStripe: true, // Whether to display table zebra pattern
			isBorder: false, // Whether to display table borders
			isSerialNo: true, // Whether to display the table No
			isSelection: false, // Whether to check multiple selections in the form
			showSelection: false, // Whether to display table multi-select
			pageSize: 50, // Number of items per page
			hideExport: true, // Whether to hide the export button
		},
		// Search form, dynamically generated (when an empty array is passed, the search will not be displayed, there are 3 types of type: input, date, select)
		search: [
            { label: 'name', prop: 'name', placeholder: 'Search template name', required: false, type: 'input' },
			{ label: 'Encoding', prop: 'code', placeholder: 'Search template encoding', required: false, type: 'input' },
			{ label: 'Type', prop: 'type', placeholder: 'Search template type', required: false, type: 'select', dictCode: 'TemplateTypeEnum' },
		],
		param: {},
		defaultSort: {
			prop: 'orderNo',
			order: 'ascending',
		},
	},
});
const getData = (param: any) => {
	return getAPI(SysTemplateApi)
		.apiSysTemplatePagePost(param)
		.then((res) => {
			return res.data;
		});
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

// Get group list
const getGroupList = async () => {
	const res = await getAPI(SysTemplateApi).apiSysTemplateGroupListGet();
	const groupSearch = {
		label: 'GroupEncoding',
		prop: 'groupName',
		placeholder: 'Please select',
		required: false,
		type: 'select',
		options: [],
	} as TableSearchType;
	state.groupList = res.data.result ?? [];
	res.data.result?.forEach((item) => {
		groupSearch.options?.push({ label: item, value: item });
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

onMounted(async () => {
	getGroupList();
});

// Update data
const updateData = () => {
	tableRef.value.handleList();
	getGroupList();
};

// Open new page
const openAddTemplate = () => {
	state.editTemplateTitle = 'Add template';
	editTemplateRef.value?.openDialog({ type: 1, orderNo: 100 });
};

// Open the edit page
const openEditTemplate = (row: any) => {
	state.editTemplateTitle = 'EditTemplate';
	editTemplateRef.value?.openDialog(row);
};

// delete
const delTemplate = (row: any) => {
	ElMessageBox.confirm(`Are you sure to delete the template: [${row.name}]?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	}).then(async () => {
			await getAPI(SysTemplateApi).apiSysTemplateDeletePost({ id: row.id });
			tableRef.value.handleList();
			ElMessage.success('Deleted successfully');
	}).catch(() => {});
};
</script>
