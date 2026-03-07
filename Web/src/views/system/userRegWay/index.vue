<template>
	<div class="sys-user-reg-way-container">
		<el-card shadow="hover" :body-style="{ padding: 5 }" v-auth="'sysUserRegWay:list'">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<el-form-item label="tenant" v-if="userStore.userInfos.accountType == 999">
					<TenantSelect v-model="state.queryParams.tenantId" clearable />
				</el-form-item>
				<el-form-item label="Keywords">
					<el-input v-model="state.queryParams.keyword" placeholder="Keywords" clearable />
				</el-form-item>
				<el-form-item label="name">
					<el-input v-model="state.queryParams.name" placeholder="name" clearable />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery"> Query
						</el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> reset </el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" icon="ele-Plus" @click="openAddRegWay" v-auth="'sysUserRegWay:add'"> Add New
					</el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.regWayData" style="width: 100%" v-loading="state.loading" border>
				<el-table-column type="index" label="No" width="55" align="center" fixed />
				<el-table-column prop="name" label="name" align="center" show-overflow-tooltip />
				<el-table-column prop="orgName" label="mechanism" align="center" show-overflow-tooltip />
				<el-table-column prop="roleName" label="Role" align="center" show-overflow-tooltip />
				<el-table-column prop="posName" label="Position" align="center" show-overflow-tooltip />
				<el-table-column prop="orderNo" label="Sort" width="70" show-overflow-tooltip />
				<el-table-column label="Modify records" width="100" align="center" show-overflow-tooltip>
					<template #default="scope">
						<ModifyRecord :data="scope.row" />
					</template>
				</el-table-column>
				<el-table-column label="Operation" width="200" fixed="right" align="center" show-overflow-tooltip v-if="auths(['sysUserRegWay:update', 'sysUserRegWay:delete'])">
					<template #default="scope">
						<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditRegWay(scope.row)" v-auth="'sysUserRegWay:update'"> Edit </el-button>
						<el-button icon="ele-Delete" size="small" text type="danger" @click="delRegWay(scope.row)" v-auth="'sysUserRegWay:delete'"> Delete </el-button>
					</template>
				</el-table-column>
			</el-table>
		</el-card>
		<EditRegWay ref="editRegWayRef" :title="state.editRegWayTitle" @handleQuery="handleQuery" />
	</div>
</template>

<script lang="ts" setup name="sysUserRegWay">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { getAPI } from '/@/utils/axios-utils';
import { UserRegWayOutput } from '/@/api-services/models';
import { SysUserRegWayApi} from '/@/api-services/api';
import { auths } from "/@/utils/authFunction";
import { useUserInfo } from "/@/stores/userInfo";
import EditRegWay from './component/editRegWay.vue';
import ModifyRecord from '/@/components/table/modifyRecord.vue';
import TenantSelect from '/@/views/system/tenant/component/tenantSelect.vue';

const userStore = useUserInfo();
const editRegWayRef = ref<InstanceType<typeof EditRegWay>>();
const state = reactive({
	loading: false,
	regWayData: [] as Array<UserRegWayOutput>,
	queryParams: {
		name: undefined,
		keyword: undefined,
		tenantId: undefined,
	},
	editRegWayTitle: '',
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
	state.regWayData = await getAPI(SysUserRegWayApi).apiSysUserRegWayListPost(state.queryParams).then(res => res.data.result ?? []);
	state.loading = false;
};

// reset operation
const resetQuery = () => {
	state.queryParams.name = undefined;
	state.queryParams.keyword = undefined;
	state.queryParams.tenantId = undefined;
	handleQuery();
};

// Open new page
const openAddRegWay = () => {
	state.editRegWayTitle = 'Add registration plan';
	editRegWayRef.value?.openDialog({ tenantId: state.queryParams.tenantId, orderNo: 100 });
};

// Open the edit page
const openEditRegWay = (row: any) => {
	state.editRegWayTitle = 'Edit registration plan';
	editRegWayRef.value?.openDialog(row);
};

// delete
const delRegWay = (row: any) => {
	ElMessageBox.confirm(`Are you sure you want to delete the plan: 【${row.name}】?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	}).then(async () => {
			await getAPI(SysUserRegWayApi).apiSysUserRegWayDeletePost({ id: row.id });
			handleQuery();
			ElMessage.success('Deleted successfully');
	}).catch(() => { });
};
</script>
