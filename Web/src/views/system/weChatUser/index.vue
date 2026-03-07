<template>
	<div class="weChatUser-container">
		<el-card shadow="hover" :body-style="{ padding: 5 }">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<el-form-item label="WeChat nickname">
					<el-input v-model="state.queryParams.nickName" placeholder="WeChat nickname" clearable />
				</el-form-item>
				<el-form-item label="Mobile phone number">
					<el-input v-model="state.queryParams.mobile" placeholder="Mobile phone number" clearable />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysWechatUser:page'"> Query </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> reset </el-button>
					</el-button-group>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.weChatUserData" style="width: 100%" v-loading="state.loading" border>
				<el-table-column type="index" label="No" width="55" align="center" />
				<el-table-column prop="openId" label="OpenId" align="center" show-overflow-tooltip />
				<el-table-column prop="unionId" label="UnionId" align="center" show-overflow-tooltip />
				<el-table-column prop="platformType" label="Platform Type" width="110" align="center" show-overflow-tooltip>
					<template #default="scope">
            <g-sys-dict v-model="scope.row.platformType" code="PlatformTypeEnum" default-value="Other" />
					</template>
				</el-table-column>
				<el-table-column prop="nickName" label="Nickname" align="center" show-overflow-tooltip />
				<el-table-column prop="avatar" label="Avatar" width="70" align="center">
					<template #default="scope">
						<el-avatar :src="scope.row.avatar" :size="24" style="vertical-align: middle" />
					</template>
				</el-table-column>
				<el-table-column prop="mobile" label="Mobile phone number" align="center" show-overflow-tooltip />
				<el-table-column prop="sex" label="gender" width="60" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag v-if="scope.row.sex === 0"> male </el-tag>
						<el-tag type="danger" v-else> Female </el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="city" label="city" align="center" show-overflow-tooltip />
				<el-table-column prop="province" label="Province" align="center" show-overflow-tooltip />
				<el-table-column prop="country" label="Country" align="center" show-overflow-tooltip />
				<el-table-column label="Operation" width="140" fixed="right" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditWeChatUser(scope.row)" v-auth="'sysWechatUser:update'"> Edit </el-button>
						<el-button icon="ele-Delete" size="small" text type="danger" @click="delWeChatUser(scope.row)" v-auth="'sysWechatUser:delete'"> Delete </el-button>
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

		<EditWeChatUser ref="editWeChatUserRef" :title="state.editWeChatUserTitle" @handleQuery="handleQuery" />
	</div>
</template>

<script lang="ts" setup name="sysWechatUser">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import EditWeChatUser from '/@/views/system/weChatUser/component/editWeChatUser.vue';
import { getAPI } from '/@/utils/axios-utils';
import { SysWechatUserApi } from '/@/api-services/api';
import { SysWechatUser } from '/@/api-services/models';

const editWeChatUserRef = ref<InstanceType<typeof EditWeChatUser>>();
const state = reactive({
	loading: false,
	weChatUserData: [] as Array<SysWechatUser>,
	queryParams: {
		nickName: undefined,
		mobile: undefined,
	},
	tableParams: {
		page: 1,
		pageSize: 50,
		total: 0 as any,
	},
	editWeChatUserTitle: '',
});

onMounted(async () => {
	handleQuery();
});

// Query operation
const handleQuery = async () => {
	state.loading = true;
	let params = Object.assign(state.queryParams, state.tableParams);
	var res = await getAPI(SysWechatUserApi).apiSysWechatUserPagePost(params);
	state.weChatUserData = res.data.result?.items ?? [];
	state.tableParams.total = res.data.result?.total;
	state.loading = false;
};

// reset operation
const resetQuery = () => {
	state.queryParams.nickName = undefined;
	state.queryParams.mobile = undefined;
	handleQuery();
};

// Open the edit page
const openEditWeChatUser = (row: any) => {
	state.editWeChatUserTitle = 'Edit WeChat account';
	editWeChatUserRef.value?.openDialog(row);
};

// delete
const delWeChatUser = (row: any) => {
	ElMessageBox.confirm(`Are you sure you want to delete the WeChat account: 【${row.nickName}】?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysWechatUserApi).apiSysWechatUserDeletePost({ id: row.id });
			handleQuery();
			ElMessage.success('Deleted successfully');
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
</script>
