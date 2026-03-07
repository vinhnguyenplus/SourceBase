<template>
	<div class="sys-vislog-container">
		<el-card shadow="hover" :body-style="{ padding: 5 }">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<el-form-item label="tenant" v-if="userStore.userInfos.accountType == 999">
					<TenantSelect v-model="state.queryParams.tenantId" clearable />
				</el-form-item>
				<el-form-item label="start time">
					<el-date-picker v-model="state.queryParams.startTime" type="datetime" placeholder="start time" value-format="YYYY-MM-DD HH:mm:ss" :shortcuts="shortcuts" />
				</el-form-item>
				<el-form-item label="end time" prop="code">
					<el-date-picker v-model="state.queryParams.endTime" type="datetime" placeholder="end time" value-format="YYYY-MM-DD HH:mm:ss" :shortcuts="shortcuts" />
				</el-form-item>
				<el-form-item label="Method Name">
					<el-input v-model="state.queryParams.actionName" placeholder="Method Name" clearable />
				</el-form-item>
				<el-form-item label="Account name">
					<el-input v-model="state.queryParams.account" placeholder="Account name" clearable />
				</el-form-item>
				<el-form-item label="state">
					<el-select v-model="state.queryParams.status" placeholder="state" clearable>
						<el-option label="success" :value="200" />
						<el-option label="Failure" :value="400" />
					</el-select>
				</el-form-item>
				<el-form-item label="Time consuming">
					<el-input v-model="state.queryParams.elapsed" placeholder="Time taken >? MS" clearable />
				</el-form-item>
				<el-form-item label="IP address">
					<el-input v-model="state.queryParams.remoteIp" placeholder="IP address" clearable />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysVislog:page'"> Query </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> reset </el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button icon="ele-DeleteFilled" type="danger" @click="clearLog" v-auth="'sysVislog:clear'" disabled> Clear </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.logData" style="width: 100%" v-loading="state.loading" border>
				<el-table-column type="index" label="No" width="55" align="center" />
				<el-table-column prop="displayTitle" label="Display Name" width="150" align="center" show-overflow-tooltip />
				<el-table-column prop="actionName" label="Method Name" width="150" header-align="center" show-overflow-tooltip />
				<el-table-column prop="account" label="Account name" width="100" align="center" show-overflow-tooltip />
				<el-table-column prop="realName" label="Real Name" width="100" align="center" show-overflow-tooltip />
				<el-table-column prop="remoteIp" label="IP address" min-width="120" align="center" show-overflow-tooltip />
				<el-table-column prop="location" label="Login location" min-width="150" align="center" show-overflow-tooltip />
				<el-table-column prop="longitude" label="longitude" min-width="100" align="center" show-overflow-tooltip />
				<el-table-column prop="latitude" label="Latitude" min-width="100" align="center" show-overflow-tooltip />
				<el-table-column prop="browser" label="Browser" min-width="150" align="center" show-overflow-tooltip />
				<el-table-column prop="os" label="operating system" width="120" align="center" show-overflow-tooltip />
				<el-table-column prop="status" label="state" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="success" v-if="scope.row.status === '200'">success</el-tag>
						<el-tag type="danger" v-else>Failure</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="elapsed" label="Time taken(ms)" width="90" align="center" show-overflow-tooltip />
				<el-table-column prop="logDateTime" label="Log Time" width="160" align="center" fixed="right" show-overflow-tooltip />
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
	</div>
</template>

<script lang="ts" setup name="sysVisLog">
import { onMounted, reactive } from 'vue';
import { ElMessage } from 'element-plus';
import { getAPI } from '/@/utils/axios-utils';
import { SysLogVis } from '/@/api-services/models';
import { useUserInfo } from "/@/stores/userInfo";
import { SysLogVisApi, SysTenantApi } from '/@/api-services/api';
import TenantSelect from '/@/views/system/tenant/component/tenantSelect.vue';

const userStore = useUserInfo();
const state = reactive({
	loading: false,
	queryParams: {
		tenantId: undefined,
		startTime: undefined,
		endTime: undefined,
		status: undefined,
		actionName: undefined,
		account: undefined,
		elapsed: undefined,
		remoteIp: undefined,
	},
	tableParams: {
		page: 1,
		pageSize: 50,
		total: 0 as any,
	},
	logData: [] as Array<SysLogVis>,
});

onMounted(async () => {
	if (userStore.userInfos.accountType == 999) {
		state.queryParams.tenantId = userStore.userInfos.currentTenantId as any;
	}
	handleQuery();
});

// Query operation
const handleQuery = async () => {
	if (state.queryParams.startTime == null) state.queryParams.startTime = undefined;
	if (state.queryParams.endTime == null) state.queryParams.endTime = undefined;
	if (state.queryParams.status == null) state.queryParams.status = undefined;
	if (state.queryParams.actionName == null) state.queryParams.actionName = undefined;
	if (state.queryParams.account == null) state.queryParams.account = undefined;
	if (state.queryParams.elapsed == null) state.queryParams.elapsed = undefined;
	if (state.queryParams.remoteIp == null) state.queryParams.remoteIp = undefined;

	state.loading = true;
	let params = Object.assign(state.queryParams, state.tableParams);
	var res = await getAPI(SysLogVisApi).apiSysLogVisPagePost(params);
	state.logData = res.data.result?.items ?? [];
	state.tableParams.total = res.data.result?.total;
	state.loading = false;
};

// reset operation
const resetQuery = () => {
	state.queryParams.startTime = undefined;
	state.queryParams.endTime = undefined;
	state.queryParams.status = undefined;
	state.queryParams.actionName = undefined;
	state.queryParams.account = undefined;
	state.queryParams.elapsed = undefined;
	state.queryParams.remoteIp = undefined;
	handleQuery();
};

// Clear log
const clearLog = async () => {
	state.loading = true;
	await getAPI(SysLogVisApi).apiSysLogVisClearPost();
	state.loading = false;

	ElMessage.success('Clearsuccess');
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

const shortcuts = [
	{
		text: 'Today',
		value: new Date(),
	},
	{
		text: 'yesterday',
		value: () => {
			const date = new Date();
			date.setTime(date.getTime() - 3600 * 1000 * 24);
			return date;
		},
	},
	{
		text: 'last week',
		value: () => {
			const date = new Date();
			date.setTime(date.getTime() - 3600 * 1000 * 24 * 7);
			return date;
		},
	},
];
</script>
