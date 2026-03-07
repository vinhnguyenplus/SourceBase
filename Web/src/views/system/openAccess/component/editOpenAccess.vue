<template>
	<div class="sys-open-access-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="600px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ props.title }} </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Identity mark" prop="accessKey" :rules="[{ required: true, message: 'Identity cannot be empty', trigger: 'blur' } ]">
							<el-input v-model="state.ruleForm.accessKey" placeholder="Identity mark" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="key" prop="accessSecret" :rules="[{ required: true, message: 'The key cannot be empty', trigger: 'blur' } ]">
							<el-input v-model="state.ruleForm.accessSecret" placeholder="key" clearable>
								<template #append>
									<el-button @click="createSecret">Generate key</el-button>
								</template>
							</el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Bind Tenant" prop="bindTenantId" :rules="[{ required: true, message: 'Bound tenant cannot be empty', trigger: 'blur' }]">
							<el-select v-model="state.ruleForm.bindTenantId" placeholder="Bind Tenant" filterable default-first-option style="width: 100%" @change="tenantChange">
								<el-option v-for="item in state.tenantData" :key="item.id" :label="item.name" :value="item.id" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Bind user" prop="bindUserId" :rules="[{ required: true, message: 'Bound user cannot be empty', trigger: 'blur' }]">
							<el-select v-model="state.ruleForm.bindUserId" placeholder="Bind user" filterable default-first-option style="width: 100%">
								<el-option v-for="item in state.userData" :key="item.id" :label="`${item.account}(${item.realName})`" :value="item.id" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="cancel">Cancel</el-button>
					<el-button type="primary" @click="submit">Confirm</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysOpenAccessEdit">
import { onMounted, reactive, ref } from 'vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysOpenAccessApi, SysTenantApi } from '/@/api-services/api';
import { SysUser, TenantOutput, UpdateOpenAccessInput } from '/@/api-services/models';

const props = defineProps({
	title: String,
});
const emits = defineEmits(['handleQuery']);
const ruleFormRef = ref();
const state = reactive({
	isShowDialog: false,
	ruleForm: {} as UpdateOpenAccessInput,
	tenantData: [] as Array<TenantOutput>, // Tenant data
	userData: [] as Array<SysUser>, // User data
});

onMounted(async () => {
	var res = await getAPI(SysTenantApi).apiSysTenantPagePost({ page: 1, pageSize: 10000 });
	state.tenantData = res.data.result?.items ?? [];
});

// Open pop-up window
const openDialog = (row: any) => {
	state.ruleForm = JSON.parse(JSON.stringify(row));
	state.isShowDialog = true;
	ruleFormRef.value?.resetFields();

	tenantChange(false);
};

// Close pop-up window
const closeDialog = () => {
	emits('handleQuery');
	state.isShowDialog = false;
};

// Cancel
const cancel = () => {
	state.isShowDialog = false;
};

// submit
const submit = () => {
	ruleFormRef.value.validate(async (valid: boolean) => {
		if (!valid) return;
		if (state.ruleForm.id != undefined && state.ruleForm.id > 0) {
			await getAPI(SysOpenAccessApi).apiSysOpenAccessUpdatePost(state.ruleForm);
		} else {
			await getAPI(SysOpenAccessApi).apiSysOpenAccessAddPost(state.ruleForm);
		}
		closeDialog();
	});
};

/**
 * tenantvalueChange
 * @param clearBindUserId YesnoClearBind user
 */
const tenantChange = async (clearBindUserId: boolean = true) => {
	var res = await getAPI(SysTenantApi).apiSysTenantUserListPost({ tenantId: state.ruleForm.bindTenantId ?? 0 });
	state.userData = res.data.result ?? [];
	if (clearBindUserId) {
		state.ruleForm.bindUserId = undefined!;
	}
};

/** Generate key */
const createSecret = async () => {
	var res = await getAPI(SysOpenAccessApi).apiSysOpenAccessSecretPost();
	state.ruleForm.accessSecret = res.data.result!;
};

// Export object
defineExpose({ openDialog });
</script>
