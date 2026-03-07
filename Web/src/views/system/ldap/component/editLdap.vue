<template>
	<div class="sysLdap-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="900px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ props.title }} </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto" :rules="rules">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Host" prop="host">
							<el-input v-model="state.ruleForm.host" placeholder="Please enter the host" maxlength="128" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="port" prop="port">
							<el-input v-model="state.ruleForm.port" type="number" placeholder="Please enter the port" maxlength="5" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Search benchmark" prop="baseDn">
							<el-input v-model="state.ruleForm.baseDn" placeholder="Please enter the user search benchmark" maxlength="128" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="filter rules" prop="authFilter">
							<el-input v-model="state.ruleForm.authFilter" placeholder="Please enter user filter rules" maxlength="128" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Bind DN" prop="bindDn">
							<el-input v-model="state.ruleForm.bindDn" placeholder="Please enter an account with domain admin privileges" maxlength="128" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Bind password" prop="bindPass">
							<el-input v-model="state.ruleForm.bindPass" placeholder="Please enter a password with domain administrative rights" maxlength="512" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Field Attributes" prop="bindAttrAccount">
							<el-input v-model="state.ruleForm.bindAttrAccount" placeholder="Please enter the domain account field attribute value" maxlength="24" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="User properties" prop="bindAttrEmployeeId">
							<el-input v-model="state.ruleForm.bindAttrEmployeeId" placeholder="Please enter the bound user EmployeeId attribute!" maxlength="24" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Bind the Code property" prop="bindAttrCode">
							<el-input v-model="state.ruleForm.bindAttrCode" placeholder="Please enterBind the Code property!" maxlength="64" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="LDAP Version" prop="version">
							<el-input v-model="state.ruleForm.version" type="number" placeholder="Please enter Ldap version" maxlength="4" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="state" prop="status">
							<el-switch v-model="state.ruleForm.status" active-text="Yes" inactive-text="no" />
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

<script lang="ts" setup>
import { ref, reactive } from 'vue';
import type { FormRules } from 'element-plus';

import { getAPI } from '/@/utils/axios-utils';
import { SysLdapApi } from '/@/api-services/api';

const props = defineProps({
	title: String,
});
const emits = defineEmits(['handleQuery']);
const ruleFormRef = ref();
const state = reactive({
	isShowDialog: false,
	ruleForm: {} as any,
});

// Open pop-up window
const openDialog = (row: any) => {
	state.ruleForm = JSON.parse(JSON.stringify(row));
	state.isShowDialog = true;
	ruleFormRef.value?.resetFields();
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
			await getAPI(SysLdapApi).apiSysLdapUpdatePost(state.ruleForm);
		} else {
			await getAPI(SysLdapApi).apiSysLdapAddPost(state.ruleForm);
		}
		closeDialog();
	});
};

// Validation rules
const rules = ref<FormRules>({
	host: [{ required: true, message: 'Please enter the host!', trigger: 'blur' }],
	port: [{ required: true, message: 'Please enter the port!', trigger: 'blur' }],
	baseDn: [{ required: true, message: 'Please enter user search criteria!', trigger: 'blur' }],
	bindDn: [{ required: true, message: 'Please enter the bind DN!', trigger: 'blur' }],
	bindPass: [{ required: true, message: 'Please enter the binding password!', trigger: 'blur' }],
	authFilter: [{ required: true, message: 'Please enter user filter rules!', trigger: 'blur' }],
	version: [{ required: true, message: 'Please enter the Ldap version!', trigger: 'blur' }],
	bindAttrAccount: [{ required: true, message: 'Please enter the account binding field!', trigger: 'blur' }],
	bindAttrEmployeeId: [{ required: true, message: "Bind the user's EmployeeId property!", trigger: 'blur' }],
	bindAttrCode: [{ required: true, message: 'Bind the Code property!', trigger: 'blur' }],
});

// Export object
defineExpose({ openDialog });
</script>
