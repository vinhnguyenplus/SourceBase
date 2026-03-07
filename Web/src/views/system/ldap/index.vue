<template>
	<div class="sysLdap-container">
		<el-card shadow="hover" :body-style="{ padding: 5 }">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<el-form-item label="tenant" v-if="userStore.userInfos.accountType == 999">
					<TenantSelect v-model="state.queryParams.tenantId" clearable />
				</el-form-item>
				<el-form-item label="Keywords">
					<el-input v-model="state.queryParams.keyword" clearable placeholder="Please enter fuzzy search keywords" />
				</el-form-item>
				<el-form-item label="Host">
					<el-input v-model="state.queryParams.host" clearable placeholder="Please enter the host" />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysLdap:page'"> Query </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> reset </el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" icon="ele-Plus" @click="openAddSysLdap" v-auth="'sysLdap:add'"> Add New </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.tableData" style="width: 100%" v-loading="state.loading" border>
				<el-table-column type="index" label="No" width="55" align="center" />
				<el-table-column prop="host" label="Host" min-width="150" show-overflow-tooltip />
				<el-table-column prop="port" label="port" show-overflow-tooltip />
				<el-table-column prop="baseDn" label="User search benchmark" show-overflow-tooltip />
				<el-table-column prop="bindDn" label="Bind DN" show-overflow-tooltip />
				<el-table-column prop="bindPass" label="Bind password" min-width="200" show-overflow-tooltip />
				<el-table-column prop="authFilter" label="User filtering rules" show-overflow-tooltip />
				<el-table-column prop="version" label="LDAP Version" show-overflow-tooltip />
				<el-table-column prop="status" label="state" width="80" align="center" show-overflow-tooltip>
					<template #default="scope">
            <g-sys-dict v-model="scope.row.status" code="StatusEnum" />
					</template>
				</el-table-column>
				<el-table-column label="Modify records" width="100" align="center" show-overflow-tooltip>
					<template #default="scope">
						<ModifyRecord :data="scope.row" />
					</template>
				</el-table-column>
				<el-table-column label="Operation" width="300" align="center" fixed="right" show-overflow-tooltip v-if="auth('sysLdap:update') || auth('sysLdap:delete') || auth('sysLdap:syncUser') || auth('sysLdap:syncOrg')">
					<template #default="scope">
						<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditSysLdap(scope.row)" v-auth="'sysLdap:update'"> Edit </el-button>
						<el-button icon="ele-Delete" size="small" text type="danger" @click="delSysLdap(scope.row)" v-auth="'sysLdap:delete'"> Delete </el-button>
						<el-button icon="ele-Refresh" size="small" text type="primary" @click="syncDomainUser(scope.row)" v-auth="'sysLdap:syncUser'"> Sync domain accounts </el-button>
						<el-button icon="ele-Refresh" size="small" text type="primary" @click="syncDomainOrg(scope.row)" v-auth="'sysLdap:syncOrg'"> Synchronize domain organization </el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination
				v-model:currentPage="state.tableParams.page"
				v-model:page-size="state.tableParams.pageSize"
				:total="state.tableParams.total"
				:page-sizes="[10, 20, 50, 100, 200, 500]"
				size="small"
				background
				@size-change="handleSizeChange"
				@current-change="handleCurrentChange"
				layout="total, sizes, prev, pager, next, jumper"
			/>
		</el-card>

		<EditLdap ref="editLdapRef" :title="state.dialogTitle" @handleQuery="handleQuery" />
	</div>
</template>

<script lang="ts" setup name="sysLdap">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { auth } from '/@/utils/authFunction';
import { getAPI } from '/@/utils/axios-utils';
import { SysLdapApi } from '/@/api-services/api';
import ModifyRecord from '/@/components/table/modifyRecord.vue';
import EditLdap from './component/editLdap.vue';
import { useUserInfo } from "/@/stores/userInfo";
import TenantSelect from '/@/views/system/tenant/component/tenantSelect.vue';

const userStore = useUserInfo();
const editLdapRef = ref<InstanceType<typeof EditLdap>>();
const state = reactive({
	loading: false,
	tableData: [] as any,
	queryParams: {
		tenantId: undefined,
		keyword: undefined,
		host: undefined,
	},
	tableParams: {
		page: 1,
		pageSize: 50,
		total: 0 as any,
	},
	dialogTitle: '',
});

onMounted(async () => {
	if (userStore.userInfos.accountType == 999) {
		state.queryParams.tenantId = userStore.userInfos.currentTenantId as any;
	}
	handleQuery();
});

// Query operation
const handleQuery = async () => {
	state.loading = true;
	let params = Object.assign(state.queryParams, state.tableParams);
	var res = await getAPI(SysLdapApi).apiSysLdapPagePost(params);
	state.tableData = res.data.result?.items ?? [];
	state.tableParams.total = res.data.result?.total;
	state.loading = false;
};

// reset operation
const resetQuery = () => {
	state.queryParams.keyword = undefined;
	state.queryParams.host = undefined;
	handleQuery();
};

// Open new page
const openAddSysLdap = () => {
	state.dialogTitle = 'Add system domain login information configuration';
	editLdapRef.value?.openDialog({ tenantId: state.queryParams.tenantId });
};

// Open the edit page
const openEditSysLdap = (row: any) => {
	state.dialogTitle = 'Edit system domain login information configuration';
	editLdapRef.value?.openDialog(row);
};

// delete
const delSysLdap = (row: any) => {
	ElMessageBox.confirm(`Are you sure you want to delete the domain login information configuration: [${row.host}]?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	}).then(async () => {
		await getAPI(SysLdapApi).apiSysLdapDeletePost({ id: row.id });
		handleQuery();
		ElMessage.success('Deleted successfully');
	}).catch(() => {});
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

// Sync domain accounts
const syncDomainUser = (row: any) => {
	ElMessageBox.confirm(`Are you sure you want to sync domain accounts?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	}).then(async () => {
		await getAPI(SysLdapApi).apiSysLdapSyncUserPost({ id: row.id });
		handleQuery();
		ElMessage.success('Deleted successfully');
	}).catch(() => {});
};

// Synchronize domain organization
const syncDomainOrg = (row: any) => {
	ElMessageBox.confirm(`Are you sure you want to synchronize the domain organization structure?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	}).then(async () => {
		await getAPI(SysLdapApi).apiSysLdapSyncOrgPost({ id: row.id });
		handleQuery();
		ElMessage.success('Deleted successfully');
	}).catch(() => {});
};
</script>
