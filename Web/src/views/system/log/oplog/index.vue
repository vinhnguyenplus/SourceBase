<template>
	<div class="sys-oplog-container" v-loading="state.loading">
		<el-card shadow="hover" :body-style="{ padding: 5 }">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<el-form-item label="tenant" v-if="userStore.userInfos.accountType == 999">
					<TenantSelect v-model="state.queryParams.tenantId" clearable />
				</el-form-item>
				<el-form-item label="start time">
					<el-date-picker v-model="state.queryParams.startTime" type="datetime" placeholder="start time" value-format="YYYY-MM-DD HH:mm:ss" :shortcuts="shortcuts" />
				</el-form-item>
				<el-form-item label="end time">
					<el-date-picker v-model="state.queryParams.endTime" type="datetime" placeholder="end time" value-format="YYYY-MM-DD HH:mm:ss" :shortcuts="shortcuts" />
				</el-form-item>
				<el-form-item label="Module Name">
					<el-input v-model="state.queryParams.controllerName" placeholder="Module Name" clearable />
				</el-form-item>
				<el-form-item label="Method Name">
					<el-input v-model="state.queryParams.actionName" placeholder="Method Name" clearable />
				</el-form-item>
				<el-form-item label="Account name">
					<el-input v-model="state.queryParams.account" placeholder="Account name" clearable />
				</el-form-item>
				<el-form-item label="Time consuming">
					<el-input v-model="state.queryParams.elapsed" placeholder="Time taken >? MS" clearable />
				</el-form-item>
				<el-form-item label="IP address">
					<el-input v-model="state.queryParams.remoteIp" placeholder="IP address" clearable />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysOplog:page'"> Query </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> reset </el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button icon="ele-DeleteFilled" type="danger" @click="clearLog" v-auth="'sysOplog:clear'"> Clear </el-button>
					<el-button icon="ele-FolderOpened" @click="exportLog" v-auth="'sysOplog:export'"> Export </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.logData" @sort-change="sortChange" style="width: 100%" border :row-class-name="tableRowClassName">
				<el-table-column type="index" label="No" width="55" align="center" />
				<el-table-column prop="controllerName" label="Module Name" min-width="120" header-align="center" show-overflow-tooltip />
				<el-table-column prop="displayTitle" label="Display Name" width="170" header-align="center" show-overflow-tooltip />
				<el-table-column prop="actionName" label="Method Name" width="150" header-align="center" show-overflow-tooltip />
				<el-table-column prop="httpMethod" label="Request Method" width="90" align="center" show-overflow-tooltip />
				<el-table-column prop="requestUrl" label="Request address" width="300" header-align="center" show-overflow-tooltip />
				<!-- <el-table-column prop="requestParam" label="Request Param" show-overflow-tooltip />
				<el-table-column prop="returnResult" label="Return result" show-overflow-tooltip /> -->
				<el-table-column prop="logLevel" label="level" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag v-if="scope.row.logLevel === 1">Debug</el-tag>
						<el-tag v-else-if="scope.row.logLevel === 2">information</el-tag>
						<el-tag v-else-if="scope.row.logLevel === 3">warning</el-tag>
						<el-tag v-else-if="scope.row.logLevel === 4">mistake</el-tag>
						<el-tag v-else>Other</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="eventId" label="Event ID" width="70" align="center" show-overflow-tooltip />
				<el-table-column prop="threadId" label="ThreadId" sortable="custom" width="90" align="center" show-overflow-tooltip />
				<el-table-column prop="traceId" label="Request Tracking ID" width="150" header-align="center" sortable="custom" show-overflow-tooltip />
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
				<!-- <el-table-column prop="exception" label="Exception object" width="150" show-overflow-tooltip /> -->
				<!-- <el-table-column prop="message" label="Log message" width="160" fixed="right" show-overflow-tooltip /> -->
				<el-table-column prop="logDateTime" label="Log Time" width="160" align="center" fixed="right" show-overflow-tooltip />
				<el-table-column label="Operation" width="80" align="center" fixed="right" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-InfoFilled" size="small" text type="primary" @click="viewDetail(scope.row)" v-auth="'sysOplog:page'">Details </el-button>
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
		<el-dialog v-model="state.dialogVisible" draggable fullscreen>
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Document /> </el-icon>
					<span> Log details </span>
				</div>
			</template>
			<pre v-loading="state.loadingDetail">{{ state.content }}</pre>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysOpLog">
import { onMounted, reactive } from 'vue';
import { ElMessage } from 'element-plus';
import { downloadByData, getFileName } from '/@/utils/download';

import { getAPI } from '/@/utils/axios-utils';
import { SysLogOp } from '/@/api-services/models';
import { useUserInfo } from "/@/stores/userInfo";
import { SysLogOpApi } from '/@/api-services/api';
import TenantSelect from '/@/views/system/tenant/component/tenantSelect.vue';

const userStore = useUserInfo();
const state = reactive({
	loading: false,
	loadingDetail: false,
	queryParams: {
		tenantId: undefined,
		startTime: undefined,
		endTime: undefined,
		controllerName: undefined,
		actionName: undefined,
		account: undefined,
		elapsed: undefined,
		remoteIp: undefined,
	},
	tableParams: {
		page: 1,
		pageSize: 50,
		field: 'createTime', // Default sort field
		order: 'descending', // Sorting direction
		descStr: 'descending', // Key characters for sorting in descending order
		total: 0 as any,
	},
	logData: [] as Array<SysLogOp>,
	dialogVisible: false,
	content: '',
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
	if (state.queryParams.controllerName == null) state.queryParams.controllerName = undefined;
	if (state.queryParams.actionName == null) state.queryParams.actionName = undefined;
	if (state.queryParams.account == null) state.queryParams.account = undefined;
	if (state.queryParams.elapsed == null) state.queryParams.elapsed = undefined;
	if (state.queryParams.remoteIp == null) state.queryParams.remoteIp = undefined;

	state.loading = true;
	let params = Object.assign(state.queryParams, state.tableParams);
	var res = await getAPI(SysLogOpApi).apiSysLogOpPagePost(params);
	state.logData = res.data.result?.items ?? [];
	state.tableParams.total = res.data.result?.total;
	state.loading = false;
};

// reset operation
const resetQuery = () => {
	state.queryParams.startTime = undefined;
	state.queryParams.endTime = undefined;
	state.queryParams.controllerName = undefined;
	state.queryParams.actionName = undefined;
	state.queryParams.account = undefined;
	state.queryParams.elapsed = undefined;
	state.queryParams.remoteIp = undefined;
	handleQuery();
};

// Clear log
const clearLog = async () => {
	state.loading = true;
	await getAPI(SysLogOpApi).apiSysLogOpClearPost();
	state.loading = false;

	ElMessage.success('Clearsuccess');
	handleQuery();
};

// Export log
const exportLog = async () => {
	state.loading = true;
	var res = await getAPI(SysLogOpApi).apiSysLogOpExportPost(state.queryParams, { responseType: 'blob' });
	state.loading = false;

	var fileName = getFileName(res.headers);
	downloadByData(res.data as any, fileName);
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

// check the details
const viewDetail = async (row: any) => {
	state.content = '';
	state.dialogVisible = true;
	state.loadingDetail = true;
	var res = await getAPI(SysLogOpApi).apiSysLogOpDetailIdGet(row.id);
	row.message = res.data.result?.message ?? '';
	state.content = row.message;
	state.loadingDetail = false;
};

// Set row color
const tableRowClassName = (row: any) => {
	return row.row.exception != null ? 'warning-row' : '';
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

// Column sort
const sortChange = (column: any) => {
	state.tableParams.field = column.prop;
	state.tableParams.order = column.order;
	handleQuery();
};
</script>

<style lang="scss" scoped>
.el-popper {
	max-width: 60%;
}
pre {
	white-space: break-spaces;
	line-height: 20px;
}
.el-table .warning-row {
	--el-table-tr-bg-color: var(--el-color-warning-light-9);
}
.el-table .success-row {
	--el-table-tr-bg-color: var(--el-color-success-light-9);
}
</style>
