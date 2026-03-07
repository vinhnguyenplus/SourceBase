<template>
	<div class="sys-app-container">
		<el-dialog v-model="state.isShowDialog" width="300" draggable :close-on-click-modal="false">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Switch /> </el-icon>
					<span>Switch Tenant</span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="tenant" prop="id" :rules="[{ required: true, message: 'Tenant cannot be empty', trigger: 'blur' }]">
							<el-select v-model="state.ruleForm.id" value-key="id" placeholder="tenant" class="w100">
								<el-option v-for="(item, index) in state.tenantList" :key="index" :label="`${item.label ?? 'Default'} (${item.host})`" :value="item.value" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="() => state.isShowDialog = false">Cancel</el-button>
					<el-button type="primary" @click="submit">Confirm</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysEditApp">
import { reactive, ref } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { SysTenantApi } from "/@/api-services";
import { reLoadLoginAccessToken } from "/@/utils/request";
import {useUserInfo} from "/@/stores/userInfo";

const userStore = useUserInfo();
const ruleFormRef = ref();
const state = reactive({
	loading: false,
	isShowDialog: false,
	ruleForm: {} as any,
	appList: [] as Array<any>,
	tenantList: [] as Array<any>
});

// Open pop-up window
const openDialog = async () => {
	state.tenantList = await getAPI(SysTenantApi).apiSysTenantListGet().then(res => res.data.result ?? []);
	state.ruleForm.id = userStore.userInfos.currentTenantId as any;
	ruleFormRef.value?.resetFields();
	state.isShowDialog = true;
	state.loading = false;
};

// submit
const submit = () => {
	ruleFormRef.value.validate(async (valid: boolean) => {
		if (!valid) return;
		state.loading = true;
		getAPI(SysTenantApi).apiSysTenantChangeTenantPost(state.ruleForm).then(res => reLoadLoginAccessToken(res.data.result));
		state.loading = false;
		state.isShowDialog = false;
	});
};

// Export object
defineExpose({ openDialog });
</script>
