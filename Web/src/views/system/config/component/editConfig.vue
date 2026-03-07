<template>
	<div class="sys-config-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="700px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ props.title }} </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Configuration name" prop="name" :rules="[{ required: true, message: 'Configuration name cannot be empty', trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.name" placeholder="Configuration name" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Configuration Encoding" prop="code" :rules="[{ required: true, message: 'Configuration code cannot be empty', trigger: 'blur' } ]">
							<el-input v-model="state.ruleForm.code" placeholder="Configuration Encoding" clearable :disabled="state.ruleForm.sysFlag == 1" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="value" prop="value" :rules="[{ required: true, message: 'Value cannot be empty', trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.value" placeholder="value">
								<template #append>
									<el-space :size="10" spacer="|">
										<el-dropdown
											style="color: inherit"
											trigger="click"
											@command="
												(value: string) => {
													state.ruleForm.value = value;
												}
											"
										>
											<el-button style="margin: 0 -20px; color: inherit"> Options </el-button>
											<template #dropdown>
												<el-dropdown-menu>
													<el-dropdown-item command="True"> True </el-dropdown-item>
													<el-dropdown-item command="False"> False </el-dropdown-item>
												</el-dropdown-menu>
											</template>
										</el-dropdown>
									</el-space>
								</template>
							</el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="built-in parameters" prop="sysFlag" :rules="[{ required: true, message: 'The built-in parameter cannot be empty', trigger: 'blur' }]">
							<el-radio-group v-model="state.ruleForm.sysFlag" :disabled="state.ruleForm.sysFlag == 1 && state.ruleForm.id != undefined">
								<el-radio :value="1">Yes</el-radio>
								<el-radio :value="2">no</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="GroupEncoding">
							<el-input v-model="state.ruleForm.groupCode" placeholder="GroupEncoding" clearable :disabled="state.ruleForm.sysFlag == 1" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Sort">
							<el-input-number v-model="state.ruleForm.orderNo" placeholder="Sort" class="w100" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Remarks">
							<el-input v-model="state.ruleForm.remark" placeholder="Please enter the remark content" clearable type="textarea" />
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

<script lang="ts" setup name="sysEditConfig">
import { reactive, ref } from 'vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysConfigApi } from '/@/api-services/api';
import { UpdateConfigInput } from '/@/api-services/models';

const props = defineProps({
	title: String,
});
const emits = defineEmits(['updateData']);
const ruleFormRef = ref();
const state = reactive({
	isShowDialog: false,
	ruleForm: {} as UpdateConfigInput,
});

// Open pop-up window
const openDialog = (row: any) => {
	state.ruleForm = JSON.parse(JSON.stringify(row));
	state.isShowDialog = true;
	ruleFormRef.value?.resetFields();
};

// Close pop-up window
const closeDialog = () => {
	emits('updateData');
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
			await getAPI(SysConfigApi).apiSysConfigUpdatePost(state.ruleForm);
		} else {
			await getAPI(SysConfigApi).apiSysConfigAddPost(state.ruleForm);
		}
		closeDialog();
	});
};

// Export object
defineExpose({ openDialog });
</script>
