<template>
	<div class="sys-onlineUser-container">
		<el-drawer v-model="state.isVisible" title="Online User List" size="45%">
			<el-card shadow="hover" :body-style="{ paddingBottom: '0' }" style="margin: 8px">
				<el-form :model="state.queryParams" ref="queryForm" :inline="true">
					<el-form-item label="tenant" v-if="userStore.userInfos.accountType == 999">
						<el-select v-model="state.queryParams.tenantId" placeholder="tenant" style="width: 100%">
							<el-option :value="item.value" :label="`${item.label} (${item.host})`" v-for="(item, index) in state.tenantList" :key="index" />
						</el-select>
					</el-form-item>
					<el-form-item label="Account number" prop="userName">
						<el-input placeholder="Account number" clearable @keyup.enter="handleQuery" v-model="state.queryParams.userName" />
					</el-form-item>
					<el-form-item label="Name" prop="realName">
						<el-input placeholder="Name" clearable @keyup.enter="handleQuery" v-model="state.queryParams.realName" />
					</el-form-item>
					<el-form-item>
						<el-button-group>
							<el-button type="primary" icon="ele-Search" @click="handleQuery"> Query </el-button>
							<el-button icon="ele-Refresh" @click="resetQuery"> reset </el-button>
						</el-button-group>
					</el-form-item>
				</el-form>
			</el-card>

			<el-card shadow="hover" style="margin: 8px; padding-bottom: 15px">
				<el-table :data="state.onlineUserList" style="width: 100%" v-loading="state.loading" border>
					<el-table-column type="index" label="No" width="55" align="center" />
					<el-table-column prop="userName" label="Account number" header-align="center" show-overflow-tooltip />
					<el-table-column prop="realName" label="Name" header-align="center" show-overflow-tooltip />
					<el-table-column prop="ip" label="IP address" min-width="100" header-align="center" show-overflow-tooltip />
					<el-table-column prop="browser" label="Browser" header-align="center" show-overflow-tooltip />
					<!-- <el-table-column prop="connectionId" label="ConnectionId" show-overflow-tooltip></el-table-column> -->
					<el-table-column prop="time" label="Login Time" min-width="120" header-align="center" show-overflow-tooltip />
					<el-table-column label="Operation" width="81" fixed="right" align="center" show-overflow-tooltip>
						<template #default="scope">
							<el-button icon="ele-CircleCloseFilled" size="small" text type="danger" v-auth="'sysOnlineUser:forceOffline'" @click="forceOffline(scope.row)"> offline </el-button>
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
		</el-drawer>
	</div>
</template>

<script lang="ts" setup>
import { onMounted, reactive } from 'vue';
import { ElMessageBox, ElNotification } from 'element-plus';
import { throttle } from 'lodash-es';

import { getAPI, clearAccessAfterReload } from '/@/utils/axios-utils';
import {SysOnlineUserApi, SysAuthApi, SysTenantApi} from '/@/api-services/api';
import { SysOnlineUser } from '/@/api-services/models';
import { useUserInfo } from "/@/stores/userInfo";
import { signalR } from './signalR';

const userStore = useUserInfo();
const state = reactive({
	loading: false,
	isVisible: false,
	tenantList: [] as Array<any>,
	queryParams: {
		userName: undefined,
		realName: undefined,
		tenantId: undefined,
	},
	tableParams: {
		page: 1,
		pageSize: 50,
		total: 1 as any,
	},
	onlineUserList: [] as Array<SysOnlineUser>, // Online user list
	lastUserState: {
		online: false,
		realName: '',
	}, // Last received user change status information
});

onMounted(async () => {
	if (userStore.userInfos.accountType == 999) {
		state.tenantList = await getAPI(SysTenantApi).apiSysTenantListGet().then(res => res.data.result ?? []);
		state.queryParams.tenantId = userStore.userInfos.currentTenantId as any;
	}
	// Online user list
	signalR.off('OnlineUserList');
	signalR.on('OnlineUserList', (data: any) => {
		state.onlineUserList = data.userList;
		state.lastUserState = {
			online: data.online,
			realName: data.realName,
		};
		notificationThrottle();
	});
	// Forced offline
	signalR.off('ForceOffline');
	signalR.on('ForceOffline', async (data: any) => {
		console.log('Forced offline', data);
		await signalR.stop();

		await getAPI(SysAuthApi).apiSysAuthLogoutPost();
		clearAccessAfterReload();
	});
});

// Notification prompts throttling
const notificationThrottle = throttle(
	function () {
		ElNotification({
			title: 'Prompt',
			message: `${state.lastUserState.online ? `【${state.lastUserState.realName}】Launched` : `【${state.lastUserState.realName}】Left`}`,
			type: `${state.lastUserState.online ? 'info' : 'error'}`,
			position: 'bottom-right',
		});
	},
	3000,
	{
		leading: true,
		trailing: false,
	}
);

// open page
const openDrawer = () => {
	state.isVisible = true;
	handleQuery();
};

// Query operation
const handleQuery = async () => {
	state.loading = true;
	let params = Object.assign(state.queryParams, state.tableParams);
	var res = await getAPI(SysOnlineUserApi).apiSysOnlineUserPagePost(params);
	state.onlineUserList = res.data.result?.items ?? [];
	state.tableParams.total = res.data.result?.total;
	state.loading = false;
};

// reset operation
const resetQuery = () => {
	state.queryParams.userName = undefined;
	state.queryParams.realName = undefined;
	handleQuery();
};

// Forced offline
const forceOffline = async (row: any) => {
	ElMessageBox.confirm(`Are you sure you want to remove the account: 【${row.realName}】?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	})
		.then(async () => {
			await signalR.send('ForceOffline', { connectionId: row.connectionId }).catch(function (err: any) {
				console.log(err);
			});
		})
		.catch(() => {});
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

// Export object
defineExpose({ openDrawer });
</script>
