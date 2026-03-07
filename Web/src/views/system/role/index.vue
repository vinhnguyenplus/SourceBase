<template>
	<div class="sys-role-container">
		<el-card shadow="hover" :body-style="{ padding: 5 }">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<el-form-item label="tenant" v-if="userStore.userInfos.accountType == 999">
					<TenantSelect v-model="state.queryParams.tenantId" clearable />
				</el-form-item>
				<el-form-item label="Character namecall">
					<el-input v-model="state.queryParams.name" placeholder="Character namecall" clearable />
				</el-form-item>
				<el-form-item label="Character Encoding">
					<el-input v-model="state.queryParams.code" placeholder="Character Encoding" clearable />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysRole:page'"> Query </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> reset </el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" icon="ele-Plus" @click="openAddRole" v-auth="'sysRole:add'"> Add New </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.roleData" style="width: 100%" v-loading="state.loading" border>
				<el-table-column type="index" label="No" width="55" align="center" fixed />
				<el-table-column prop="name" label="Character namecall" align="center" show-overflow-tooltip />
				<el-table-column prop="code" label="Character Encoding" align="center" show-overflow-tooltip />
				<el-table-column label="Data Range" align="center" show-overflow-tooltip>
					<template #default="scope">
            <g-sys-dict v-model="scope.row.dataScope" code="DataScopeEnum" />
					</template>
				</el-table-column>
				<el-table-column prop="orderNo" label="Sort" width="70" align="center" show-overflow-tooltip />
				<el-table-column label="state" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-switch v-model="scope.row.status" :active-value="1" :inactive-value="2" size="small" @change="changeStatus(scope.row)" v-auth="'sysRole:setStatus'" />
					</template>
				</el-table-column>
				<el-table-column label="Modify records" width="100" align="center" show-overflow-tooltip>
					<template #default="scope">
						<ModifyRecord :data="scope.row" />
					</template>
				</el-table-column>
				<el-table-column label="Operation" width="240" fixed="right" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-OfficeBuilding" size="small" text type="primary" @click="openGrantData(scope.row)" v-auth="'sysRole:grantDataScope'"> Data Range </el-button>
						<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditRole(scope.row)" v-auth="'sysRole:update'"> Edit </el-button>
						<el-button icon="ele-Delete" size="small" text type="danger" @click="delRole(scope.row)" v-auth="'sysRole:delete'"> Delete </el-button>
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

		<EditRole ref="editRoleRef" :title="state.editRoleTitle" @handleQuery="handleQuery" />
		<GrantData ref="grantDataRef" @handleQuery="handleQuery" />
	</div>
</template>

<script lang="ts" setup name="sysRole">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import EditRole from '/@/views/system/role/component/editRole.vue';
import GrantData from '/@/views/system/role/component/grantData.vue';
import ModifyRecord from '/@/components/table/modifyRecord.vue';
import { getAPI } from '/@/utils/axios-utils';
import { SysRoleApi } from '/@/api-services/api';
import { SysRole } from '/@/api-services/models';
import { useUserInfo } from "/@/stores/userInfo";
import TenantSelect from '/@/views/system/tenant/component/tenantSelect.vue';

const userStore = useUserInfo();
const editRoleRef = ref<InstanceType<typeof EditRole>>();
const grantDataRef = ref<InstanceType<typeof GrantData>>();
const state = reactive({
	loading: false,
	roleData: [] as Array<SysRole>,
	queryParams: {
		tenantId: undefined,
		name: undefined,
		code: undefined,
	},
	tableParams: {
		page: 1,
		pageSize: 50,
		total: 0 as any,
	},
	editRoleTitle: '',
});

onMounted(async () => {
	if (userStore.userInfos.accountType == 999) {
		state.queryParams.tenantId = userStore.userInfos.currentTenantId as any;
	}
	await handleQuery();
});

// Query operation
const handleQuery = async () => {
	state.loading = true;
	let params = Object.assign(state.queryParams, state.tableParams);
	let res = await getAPI(SysRoleApi).apiSysRolePagePost(params);
	state.roleData = res.data.result?.items ?? [];
	state.tableParams.total = res.data.result?.total;
	state.loading = false;
};

// reset operation
const resetQuery = async () => {
	state.queryParams.name = undefined;
	state.queryParams.code = undefined;
	await handleQuery();
};

// Open new page
const openAddRole = () => {
	state.editRoleTitle = 'Add role';
	editRoleRef.value?.openDialog({ id: undefined, status: 1, tenantId: state.queryParams.tenantId, orderNo: 100 });
};

// Open the edit page
const openEditRole = async (row: any) => {
	state.editRoleTitle = 'EditRole';
	editRoleRef.value?.openDialog(row);
};

// Open the authorized data scope page
const openGrantData = (row: any) => {
	grantDataRef.value?.openDialog(row);
};

// delete
const delRole = (row: any) => {
	ElMessageBox.confirm(`Are you sure to delete the role: [${row.name}]?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysRoleApi).apiSysRoleDeletePost({ id: row.id });
			await handleQuery();
			ElMessage.success('Deleted successfully');
		})
		.catch(() => {});
};

// Change page capacity
const handleSizeChange = async (val: number) => {
	state.tableParams.pageSize = val;
	await handleQuery();
};

// Change page number
const handleCurrentChange = async (val: number) => {
	state.tableParams.page = val;
	await handleQuery();
};

// Modify status
const changeStatus = async (row: any) => {
	await getAPI(SysRoleApi)
		.apiSysRoleSetStatusPost({ id: row.id, status: row.status })
		.then(() => {
			ElMessage.success('Role status set successfully');
		})
		.catch(() => {
			row.status = row.status == 1 ? 2 : 1;
		});
};
</script>
