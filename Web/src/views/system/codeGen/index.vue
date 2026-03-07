<template>
	<div class="sys-codeGen-container">
		<el-card shadow="hover" :body-style="{ padding: 5 }">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<el-form-item label="Business Name">
					<el-input placeholder="Business Name" clearable @keyup.enter="handleQuery" v-model="state.queryParams.busName" />
				</el-form-item>
				<el-form-item label="Database table name">
					<el-input placeholder="Database table name" clearable @keyup.enter="handleQuery" v-model="state.queryParams.tableName" />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysMenu:list'"> Query </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> reset </el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" icon="ele-Plus" @click="openAddDialog"> increase </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.tableData" style="width: 100%" v-loading="state.loading" border>
				<el-table-column type="index" label="No" width="55" align="center" />
				<el-table-column prop="configId" label="library locator" align="center" show-overflow-tooltip />
				<el-table-column prop="dbNickName" label="Library name" align="center" show-overflow-tooltip />
				<el-table-column prop="tableName" label="Table Name" align="center" show-overflow-tooltip />
				<el-table-column prop="busName" label="Business Name" align="center" show-overflow-tooltip />
				<el-table-column prop="nameSpace" label="namespace" align="center" show-overflow-tooltip />
				<el-table-column prop="authorName" label="Author's Name" align="center" show-overflow-tooltip />
				<el-table-column prop="generateType" label="Generation method" align="center" show-overflow-tooltip>
					<template #default="scope">
            <g-sys-dict v-model="scope.row.generateType" code="code_gen_create_type" />
					</template>
				</el-table-column>
				<el-table-column label="Operation" width="280" fixed="right" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-Delete" size="small" text type="danger" title="Delete" @click="deleConfig(scope.row)" />
						<el-button icon="ele-Edit" size="small" text type="primary" title="Edit and fully refresh the field list" @click="openEditDialog(scope.row)" />
						<el-button icon="ele-CopyDocument" size="small" text type="primary" title="Copy" @click="openCopyDialog(scope.row)" />
						<el-button icon="ele-View" size="small" text type="primary" title="Preview" @click="handlePreview(scope.row)" />
						<el-button icon="ele-Setting" size="small" text type="primary" title="Configuration" @click="openConfigDialog(scope.row)" />
						<el-button icon="ele-Refresh" size="small" text type="primary" title="Synchronize fields and retain historical field role types" @click="syncCodeGen(scope.row)" />
						<el-button icon="ele-Position" size="small" text type="primary" @click="handleGenerate(scope.row)">Generate</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination
				v-model:currentPage="state.tableParams.page"
				v-model:page-size="state.tableParams.pageSize"
				:total="state.tableParams.total"
				:page-sizes="[10, 20, 50, 100]"
				size="small"
				background
				@size-change="handleSizeChange"
				@current-change="handleCurrentChange"
				layout="total, sizes, prev, pager, next, jumper"
			/>
		</el-card>

		<EditCodeGenDialog :title="state.editTitle" ref="EditCodeGenRef" @handleQuery="handleQuery" :application-namespaces="state.applicationNamespaces" />
		<CodeConfigDialog ref="CodeConfigRef" @handleQuery="handleQuery" />
		<PreviewDialog :title="state.editTitle" ref="PreviewRef" />
	</div>
</template>

<script lang="ts" setup name="sysCodeGen">
import { onMounted, reactive, ref, defineAsyncComponent } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import EditCodeGenDialog from './component/editCodeGenDialog.vue';
import CodeConfigDialog from './component/genConfigDialog.vue';
import { downloadByUrl } from '/@/utils/download';
import { getAPI } from '/@/utils/axios-utils';
import { SysCodeGenApi } from '/@/api-services/api';
import { SysCodeGen } from '/@/api-services/models';

const PreviewDialog = defineAsyncComponent(() => import('./component/previewDialog.vue'));

const EditCodeGenRef = ref<InstanceType<typeof EditCodeGenDialog>>();
const CodeConfigRef = ref<InstanceType<typeof CodeConfigDialog>>();
const PreviewRef = ref<InstanceType<typeof PreviewDialog>>();
const state = reactive({
	loading: false,
	loading1: false,
	dbData: [] as any,
	configId: '',
	tableData: [] as Array<SysCodeGen>,
	tableName: '',
	queryParams: {
		name: undefined,
		code: undefined,
		tableName: undefined,
		busName: undefined,
	},
	tableParams: {
		page: 1,
		pageSize: 50,
		total: 0 as any,
	},
	editTitle: '',
	applicationNamespaces: [] as Array<string>,
});

onMounted(async () => {
	handleQuery();
	let res = await getAPI(SysCodeGenApi).apiSysCodeGenApplicationNamespacesGet();
	state.applicationNamespaces = res.data.result as Array<string>;
});

const openConfigDialog = (row: any) => {
	CodeConfigRef.value?.openDialog(row);
};

// Table query operations
const handleQuery = async () => {
	state.loading = true;
	let params = Object.assign(state.queryParams, state.tableParams);
	let res = await getAPI(SysCodeGenApi).apiSysCodeGenPagePost(params);
	state.tableData = res.data.result?.items ?? [];
	state.tableParams.total = res.data.result?.total;
	state.loading = false;
};

// reset operation
const resetQuery = () => {
	state.queryParams.busName = undefined;
	state.queryParams.tableName = undefined;
	handleQuery();
};

// Change page capacity
const handleSizeChange = (val: number) => {
	state.tableParams.pageSize = val;
	handleQuery();
};

// Change page number
const handleCurrentChange = (val: number) => {
	state.tableParams.page = val;
	handleQuery();
};

// Open the table add page
const openAddDialog = () => {
	state.editTitle = 'increase';
	EditCodeGenRef.value?.openDialog({
		authorName: 'Admin.NET',
		generateType: '200',
		printType: 'off',
		menuIcon: 'ele-Menu',
		pagePath: 'main',
		nameSpace: state.applicationNamespaces[0],
		generateMenu: true,
	});
};

// Open the table editing page
const openEditDialog = (row: any) => {
	state.editTitle = 'Edit';
	EditCodeGenRef.value?.openDialog(row);
};

// Open copy page
const openCopyDialog = (row: any) => {
	state.editTitle = 'Copy';
	var copyRow = JSON.parse(JSON.stringify(row));
	copyRow.id = 0;
	copyRow.busName = '';
	copyRow.tableName = '';
  copyRow.tableUniqueList = undefined;
	EditCodeGenRef.value?.openDialog(copyRow);
};

// Delete table
const deleConfig = (row: any) => {
	ElMessageBox.confirm(`Are you sure to delete?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	}).then(async () => {
			await getAPI(SysCodeGenApi).apiSysCodeGenDeletePost([{ id: row.id }]);
			handleQuery();
			ElMessage.success('Operation successful');
	}).catch(() => {});
};

// Synchronous generation
const syncCodeGen = async (row: any) => {
  ElMessageBox.confirm(`Are you sure you want to synchronize? (Keep the historical field function types)`, 'Prompt', {
    confirmButtonText: 'Confirm',
    cancelButtonText: 'Cancel',
    type: 'warning',
  }).then(async () => {
	await getAPI(SysCodeGenApi).apiSysCodeFieldGenPost(row);
    handleQuery();
    ElMessage.success('Successfully synchronized the field list');
  }).catch(() => {});
}

// Start generating code
const handleGenerate = (row: any) => {
	ElMessageBox.confirm(`Are you sure you want to generate it?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	})
		.then(async () => {
			var res = await getAPI(SysCodeGenApi).apiSysCodeGenRunLocalPost(row);
			if (res.data.result != null && res.data.result.url != null) downloadByUrl({ url: res.data.result.url });
			handleQuery();
			ElMessage.success('Operation successful');
		})
		.catch(() => {});
};

// Preview code
const handlePreview = (row: any) => {
	state.editTitle = 'Preview Code';
	PreviewRef.value?.openDialog(row);
};
</script>
